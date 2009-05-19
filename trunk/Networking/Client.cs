/*••••••••••••••••••••••••••••••••••••••••*\
| Interaction Engine                       |
| (C) Copyright Bluestone Coding 2008-2009 |
|••••••••••••••••••••••••••••••••••••••••••|
|           __    ___ ___  ___             |
|          /++\  | _ ) __|/ __|            |
|          \++/  | _ \__ \ (__             |
|           \/   |___/___/\___|            |
|                                          |
|••••••••••••••••••••••••••••••••••••••••••|
| NETWORKING                               |
| * Client                           Class |
\*••••••••••••••••••••••••••••••••••••••••*/

namespace InteractionEngine.Networking {

    /**
     * This class represents a client in the InteractionEngine. It is the focal point for networking on the server-side.
     * This class is only ever touched on the server, with the exception of the portNumber.
     */
    public class Client {

        #region Static Server-Wide Components

        // Contains a TcpListener that will monitor the port for new connections.
        // Used so that clients can open a connection with the server.
        private static System.Net.Sockets.TcpListener tcpListener;
        // Contains a list of all the clients currently connected to this server.
        // Used for iterating through clients.
        internal static System.Collections.Generic.List<Client> clientList = new System.Collections.Generic.List<Client>();
        // Contains a list of events that need to be triggered when a new Client is established.
        // Used so that the GameWorld can deal with new Clients however it wants.
        public static System.Collections.Generic.List<EventHandling.Event> onJoin = new System.Collections.Generic.List<InteractionEngine.EventHandling.Event>();
        // Contains a list of Events that need to be triggered when a Client quits.
        // Used so that the GameWorld can deal with Client quitting however it wants.
        public static System.Collections.Generic.List<EventHandling.Event> onQuit = new System.Collections.Generic.List<InteractionEngine.EventHandling.Event>();

        /// <summary>
        /// Start listening for new clients to connect using the set port. 
        /// This method can only be called by a server's GameWorld!
        /// </summary>
        public static void startListening() {
            if (GameWorld.GameWorld.status == InteractionEngine.GameWorld.GameWorld.Status.MULTIPLAYER_CLIENT)
                throw new System.Exception("The game developer screwed up. They shouldn't be telling a client to start listening for connections!");
            if (tcpListener != null) return;
            tcpListener = new System.Net.Sockets.TcpListener(portNumber);
        }

        /// <summary>
        /// Stop listening for new clients to connect.
        /// </summary>
        public static void stopListening() {
            tcpListener = null;
        }

        /// <summary>
        /// Contains the default port that will be used if none is provided.
        /// Used for opening up a socket.
        /// If you are currently listening for connections, setting this will stop that process. You will have to restart it.
        /// </summary>
        private static int portNumber = 1337;
        public static int listeningPort {
            get {
                return portNumber;
            }
            set {
                stopListening();
                portNumber = value;
            }
        }

        /// <summary>
        /// Check the TcpListener for new connections. 
        /// This method can only be called by a server's InteractionEngine!
        /// </summary>
        internal static void checkForNewConnections() {
            if (tcpListener == null) return;
            while (tcpListener.Pending()) {
                Client newClient = new Client(tcpListener.AcceptTcpClient());
                clientList.Add(newClient);
                // Trigger the onJoin Events.
                foreach (EventHandling.Event eventObject in onJoin)
                    GameWorld.GameWorld.addEvent(eventObject);
            }
        }

        #endregion

        #region Networking

        // Contains a static reference to a BinaryFormatter.
        // Used for serialization/deserialization of objects.
        private static System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        // Contains a reference to the TcpClient connection that defines this class.
        // Used for communicating to the client this class represents.
        private readonly System.Net.Sockets.TcpClient tcpClient;
        // Contains a BinaryReader that wraps the above TcpClient.
        // Used so that we don't have to individually encode each byte into the stream.
        private readonly System.IO.BinaryReader reader;
        // Contains a BinaryReader that wraps the above TcpClient.
        // Used so that we don't have to individually encode each byte into the stream.
        private readonly System.IO.BinaryWriter writer;
        // Contains the thread that handles reading of Events from the network stream.
        // Used for concurrently reading events so that we don't block the main game loop waiting for one to completely arrive.
        private System.Threading.Thread eventReaderThread;
        // Contains the list of Events that have been read by the eventReaderThread.
        // Used for passing events from the reader thread to the main game thread.
        private System.Collections.Generic.List<EventHandling.Event> eventBuffer = new System.Collections.Generic.List<InteractionEngine.EventHandling.Event>();
        
        /// <summary>
        /// Construct a new Client using a TcpClient object.
        /// </summary>
        /// <param name="tcpClient">The connection to build the Client class around.</param>
        protected Client(System.Net.Sockets.TcpClient tcpClient) {
            this.tcpClient = tcpClient;
            this.reader = new System.IO.BinaryReader(tcpClient.GetStream());
            this.writer = new System.IO.BinaryWriter(tcpClient.GetStream());
            this.eventReaderThread = new System.Threading.Thread(new System.Threading.ThreadStart(readEvents));
        }

        /// <summary>
        /// Send an update across the network to this client.
        /// </summary>
        /// <param name="update"></param>
        internal void sendUpdate(Update update) {
            update.sendUpdate(writer);
        }

        /// <summary>
        /// Continuously reads events from the network stream... intended to be run asynchronously by the eventReaderThread.
        /// </summary>
        private void readEvents() {
            // TODO: handle IO exceptions
            while (true) {
                int gameObjectID = reader.ReadInt32();
                string eventHash = reader.ReadString();
                object parameter = formatter.Deserialize(tcpClient.GetStream());
                lock (eventBuffer) eventBuffer.Add(new InteractionEngine.EventHandling.Event(gameObjectID, eventHash, parameter));
            }
        }

        /// <summary>
        /// Read a list of Events from this Client.
        /// </summary>
        /// <returns>A list of Events that we got from this Client.</returns>
        internal System.Collections.Generic.List<EventHandling.Event> getEvents() {
            System.Collections.Generic.List<EventHandling.Event> events = new System.Collections.Generic.List<InteractionEngine.EventHandling.Event>();
            lock (eventBuffer) {
                events.AddRange(eventBuffer);
                eventBuffer.Clear();
            }
            return events;
        }

        #endregion

        #region LoadRegion List
        /**
         * LOADREGION LIST
         * 
         * Contains a reference to all the LoadRegions this user is in.
         * Used for sending output to the User, figuring out what to display, etc.
         */
        private System.Collections.Generic.List<Constructs.LoadRegion> loadRegionList = new System.Collections.Generic.List<InteractionEngine.Constructs.LoadRegion>();

        /// <summary>
        /// Adds a LoadRegion to this User's LoadRegion list. 
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to be added</param>
        public void addLoadRegion(Constructs.LoadRegion loadRegion) {
            loadRegionList.Add(loadRegion);
        }

        /// <summary>
        /// Removes a LoadRegion from this User's list of LoadRegions. 
        /// </summary>
        /// <param name="index">The index of the LoadRegion to be removed</param>
        public void removeLoadRegion(int index) {
            loadRegionList.RemoveAt(index);
        }

        /// <summary>
        /// Removes a LoadRegion from this User's list of LoadRegions.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to be removed.</param>
        public void removeLoadRegion(InteractionEngine.Constructs.LoadRegion loadRegion) {
            loadRegionList.Remove(loadRegion);
        }

        /// <summary>
        /// Get a LoadRegion from this User's list of LoadRegions.
        /// </summary>
        /// <param name="index">The index of the LoadRegion.</param>
        /// <returns>The LoadRegion being fetched.</returns>
        public Constructs.LoadRegion getLoadRegion(int index) {
            return loadRegionList[index];
        }

        /// <summary>
        /// Returns the number of LoadRegions this User has.
        /// </summary>
        /// <returns>The number of LoadRegions this User has.</returns>
        public int getLoadRegionCount() {
            return loadRegionList.Count;
        }

        /// <summary>
        /// Get the LoadRegion list in read-only form.
        /// </summary>
        /// <returns>The LoadRegion list in read-only form.</returns>
        public System.Collections.Generic.IList<Constructs.LoadRegion> getLoadRegionList() {
            return loadRegionList.AsReadOnly();
        }

        #endregion

        #region Permission List

        /**
         * PERMISSION LIST
         * 
         * Contains a list of GameObjects that this Client is allowed to call methods on.
         * Used for specifying which GameObjects the Client controls, and thus preventing Client from commanding other Clients' GameObjects.
        */
        private System.Collections.Generic.List<int> permissionList = new System.Collections.Generic.List<int>();

        /// <summary>
        /// This adds a GameObject to be controllable by the Client. 
        /// </summary>
        /// <param name="gameObject">The ID of the GameObject to be added.</param>
        public void addPermission(int gameObjectID) {
            permissionList.Add(gameObjectID);
        }

        /// <summary>
        /// Remove a GameObject from the list of GameObjects this Client controls.
        /// </summary>
        /// <param name="index">The ID of the GameObject to be removed.</param>
        public void removePermission(int gameObjectID) {
            permissionList.Remove(gameObjectID);
        }

        /// <summary>
        /// Checks whether or not this Client has permission to access a certain GameObject.
        /// </summary>
        /// <param name="gameObjectID">The ID of the GameObject to be checked.</param>
        /// <returns>True if the Client has permission; false if not.</returns>
        public bool hasPermission(int gameObjectID) {
            return permissionList.Contains(gameObjectID);
        }

        /// <summary>
        /// Get the number of GameObjects that this Client controls.
        /// </summary>
        /// <returns>The number of GameObjects that this Client controls.</returns>
        public int getPermissionCount() {
            return permissionList.Count;
        }

        /// <summary>
        /// Get the Permission List as a read-only list.
        /// </summary>
        /// <returns>The permission list in read-only form.</returns>
        public System.Collections.Generic.IList<int> getPermissions() {
            return permissionList.AsReadOnly();
        }

        #endregion

    }

}