/*••••••••••••••••••••••••••••••••••••••••*\
| Interaction Engine                       |
| (C) Copyright Bluestone Coding 2008      |
|••••••••••••••••••••••••••••••••••••••••••|
|           __    ___ ___  ___             |
|          /++\  | _ ) __|/ __|            |
|          \++/  | _ \__ \ (__             |
|           \/   |___/___/\___|            |
|                                          |
|••••••••••••••••••••••••••••••••••••••••••|
| SERVER                                   |
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

        // Contains the thread that handles reading of Events from the network stream.
        // Used for concurrently reading events so that we don't block the main game loop waiting for one to completely arrive.
        private System.Threading.Thread eventReaderThread;
        // Contains the list of events that have been read by the eventReaderThread.
        // Used for passing events from the reader thread to the main game thread.
        private System.Collections.Generic.List<EventHandling.Event> eventBuffer = new System.Collections.Generic.List<InteractionEngine.EventHandling.Event>();
        
        /// <summary>
        /// Construct a new Client using a TcpClient object.
        /// </summary>
        /// <param name="tcpClient">The connection to build the Client class around.</param>
        protected Client(System.Net.Sockets.TcpClient tcpClient) {
            this.tcpClient = tcpClient;
            this.reader = new System.IO.BinaryReader(tcpClient.GetStream());
            this.eventReaderThread = new System.Threading.Thread(new System.Threading.ThreadStart(readEvents));
        }

        /// <summary>
        /// Send information to this client. 
        /// </summary>
        /// <param name="information">An array of bytes to be passed to a client.</param>
        internal virtual void send(byte[] information) {
            if (GameWorld.GameWorld.status != GameWorld.GameWorld.Status.MULTIPLAYER_SERVER && GameWorld.GameWorld.status != GameWorld.GameWorld.Status.MULTIPLAYER_SERVERCLIENT)
                throw new System.Exception("Something is wrong with the InteractionEngine. This is probably our bad. Sorry.");
            tcpClient.GetStream().Write(information, 0, information.Length);
        }

        // Contains a constant specifying what increment of bytes to read by.
        // Used to allow easy changing of this for the receive() method.
        private const int READ_INCREMENT = 1024;

        /// <summary>
        /// Continuously reads events from the network stream... intended to be run asynchronously by the eventReaderThread.
        /// </summary>
        private void readEvents() {
            // TODO: handle IO exceptions
            while (true) {
                int gameObjectID = reader.ReadInt32();
                string eventHash = reader.ReadString();
                object parameter = formatter.Deserialize(tcpClient.GetStream());
                lock (this.eventBuffer) {
                    this.eventBuffer.Add(new InteractionEngine.EventHandling.Event(gameObjectID, eventHash, parameter));
                }
            }
        }

        /// <summary>
        /// Read a list of events from this client.
        /// </summary>
        /// <returns>A list of events that we got from this client.</returns>
        internal System.Collections.Generic.List<EventHandling.Event> getEvents() {
            // TODO we would prefer not to have to create a new list every time.
            System.Collections.Generic.List<EventHandling.Event> events = new System.Collections.Generic.List<InteractionEngine.EventHandling.Event>();
            lock (eventBuffer) {
                events.AddRange(eventBuffer);
                eventBuffer.Clear();
            }
            return events;
            /* OLD CODE THAT ASHOAT SHOULD REMOVE AS SOON AS HE VERIFIES MINE
            System.Collections.Generic.List<EventHandling.Event> events = new System.Collections.Generic.List<InteractionEngine.EventHandling.Event>();            
            // Loop through the stream and get the byte array by merging a bunch of smaller arrays
            while (tcpClient.GetStream().DataAvailable) {
                // The first four bytes are the GameObjectID.
                byte[] gameObjectIdBytes = new byte[4];
                tcpClient.GetStream().Read(gameObjectIdBytes, 0, 4);
                int gameObjectID = System.Convert.ToInt32(gameObjectIdBytes[0]) << 24 + System.Convert.ToInt32(gameObjectIdBytes[1]) << 16 + System.Convert.ToInt32(gameObjectIdBytes[2]) << 8 + System.Convert.ToInt32(gameObjectIdBytes[3]);
                // I have absolutely no idea how to read a string in bytes.
                string eventHash = "wtf";  
                // Get the parameter.
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                object parameter = formatter.Deserialize(tcpClient.GetStream());
                // Instantiate an Event and add it to the list 
                events.Add(new EventHandling.Event(gameObjectID, eventHash, parameter));
            }
            return events;
            */
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
         * Contains a list of GameObjects that this User is allowed to call methods on.
         * Used for specifying which GameObjects the User controls, and thus preventing Users from commanding other User's GameObjects.
        */
        private System.Collections.Generic.List<Constructs.GameObject> permissionList = new System.Collections.Generic.List<InteractionEngine.Constructs.GameObject>();

        /// <summary>
        /// This adds a GameObject to be controllable by the User. 
        /// </summary>
        /// <param name="gameObject">The GameObject to be added</param>
        public void addPermission(Constructs.GameObject gameObject) {
            permissionList.Add(gameObject);
        }

        /// <summary>
        /// Get a GameObject that this User controls.
        /// </summary>
        /// <param name="index">The index of the GameObject.</param>
        /// <returns>The GameObject.</returns>
        public Constructs.GameObject getPermission(int index) {
            return permissionList[index];
        }

        /// <summary>
        /// Remove a GameObject from the list of GameObjects this User controls.
        /// </summary>
        /// <param name="index">The index of the GameObject to be removed.</param>
        public void removePermission(int index) {
            permissionList.RemoveAt(index);
        }

        /// <summary>
        /// Get the number of GameObjects that this User controls.
        /// </summary>
        /// <returns>The number of GameObjects that this User controls.</returns>
        public int getPermissionCount() {
            return permissionList.Count;
        }

        /// <summary>
        /// Get the permission list as a read-only list.
        /// </summary>
        /// <returns>The permission list in read-only form.</returns>
        public System.Collections.Generic.IList<Constructs.GameObject> getPermissions() {
            return permissionList.AsReadOnly();
        }

        #endregion

    }

}