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
        // Contains the thread that handles listening on the port.
        // Used for concurrently listening so that we don't have to wait for the main game loop to let in a new connection.
        private static System.Threading.Thread listenerThread;

        /// <summary>
        /// Start listening for new clients to connect using the set port. 
        /// This method can only be called by a server's GameWorld!
        /// </summary>
        public static void startListening() {
            if (InteractionEngine.Engine.status != InteractionEngine.Engine.Status.MULTIPLAYER_SERVER && InteractionEngine.Engine.status != InteractionEngine.Engine.Status.MULTIPLAYER_SERVERCLIENT)
                throw new GameWorldException("You cannot call Client.startListening() on a client. That method is designed for the server - it listens for new clients attempting to make connections.");
            tcpListener = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Any, portNumber);
            try {
                tcpListener.Start();
            } catch (System.Net.Sockets.SocketException e) {
                throw new InteractionEngineException("You have probably tried to run two copies of the Interaction Engine on the same port from the same machine. This is not supported. For more information, see the inner exception.", e);
            }
            listenerThread = new System.Threading.Thread(new System.Threading.ThreadStart(checkForNewConnections));
            try {
                listenerThread.Start();
            } catch (System.OutOfMemoryException e) {
                throw new InteractionEngineException("There is insufficient memory on your system to run the Interaction Engine.", e);
            }
        }

        /// <summary>
        /// Stop listening for new clients to connect.
        /// </summary>
        public static void stopListening() {
            if (tcpListener != null) {
                listenerThread.Abort();
                tcpListener.Stop();
            }
        }

        /// <summary>
        /// Contains the default port that will be used if none is provided.
        /// Used for opening up a socket.
        /// If you are currently listening for connections, setting this will stop that process. You will have to restart it.
        /// </summary>
        private static ushort portNumber = 1337;
        public static ushort listeningPort {
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
            while (true) {
                if(!tcpListener.Pending()) continue;
                try {
                    Client newClient = new Client(tcpListener.AcceptTcpClient());
                    lock (clientList) {
                        clientList.Add(newClient);
                    }
                    // Trigger the onJoin Events.
                    foreach (EventHandling.Event eventObject in onJoin) {
                        eventObject.parameter = (object)newClient;
                        InteractionEngine.Engine.addEvent(eventObject);
                    }
                } catch (System.Net.Sockets.SocketException e) {
                    throw new InteractionEngineException("A client tried to connect, but something failed. See the inner exception for more details.", e);
                }
            }
        }

        #endregion

        #region Private LoadRegion List

        /**
         * PRIVATE LOADREGION LIST
         * 
         * Contains a reference to all the LoadRegions this MULTIPLAYER_SERVERCLIENT is in.
         * Used for figuring out what to display.
         */
        private static System.Collections.Generic.Dictionary<int, Constructs.LoadRegion> privateLoadRegionHashlist = new System.Collections.Generic.Dictionary<int, InteractionEngine.Constructs.LoadRegion>();

        /// <summary>
        /// Adds a LoadRegion to this User's LoadRegion list. 
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to be added</param>
        public static void addPrivateLoadRegion(Constructs.LoadRegion loadRegion) {
            if (privateLoadRegionHashlist.ContainsKey(loadRegion.id)) return;
            lock (privateLoadRegionHashlist) {
                privateLoadRegionHashlist.Add(loadRegion.id, loadRegion);
            }
        }

        /// <summary>
        /// Removes a LoadRegion from this User's list of LoadRegions. 
        /// </summary>
        /// <param name="id">The ID of the LoadRegion to be removed</param>
        public static void removePrivateLoadRegion(int id) {
            if (privateLoadRegionHashlist.ContainsKey(id)) {
                lock (privateLoadRegionHashlist) {
                    privateLoadRegionHashlist.Remove(id);
                }
            }
        }

        /// <summary>
        /// Clear the private LoadRegion Hashlist.
        /// </summary>
        internal static void clearPrivateLoadRegionList() {
            lock (privateLoadRegionHashlist) {
                privateLoadRegionHashlist.Clear();
            }
        }

        /// <summary>
        /// Get the LoadRegion list in read-only form.
        /// </summary>
        /// <returns>The LoadRegion list in read-only form.</returns>
        internal static Constructs.LoadRegion[] getPrivateLoadRegionList() {
            Constructs.LoadRegion[] loadRegionArray;
            // Copy over to prevent 
            lock (privateLoadRegionHashlist) {
                loadRegionArray = new Constructs.LoadRegion[privateLoadRegionHashlist.Count];
                int i = 0;
                foreach(Constructs.LoadRegion loadRegion in privateLoadRegionHashlist.Values) loadRegionArray[i++] = loadRegion;
            }
            return loadRegionArray;
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
        private Client(System.Net.Sockets.TcpClient tcpClient) {
            this.tcpClient = tcpClient;
            this.reader = new System.IO.BinaryReader(tcpClient.GetStream());
            this.writer = new System.IO.BinaryWriter(tcpClient.GetStream());
            this.eventReaderThread = new System.Threading.Thread(new System.Threading.ThreadStart(readEvents));
            try {
                this.eventReaderThread.Start();
            } catch (System.OutOfMemoryException e) {
                throw new InteractionEngineException("There is insufficient memory on your system to run the Interaction Engine.", e);
            }
        }

        /// <summary>
        /// This method closes the current Client object.
        /// </summary>
        public void disconnect() {
            this.tcpClient.Close();
            lock (clientList) {
                clientList.Remove(this);
            }
            // Trigger the onQuit Events.
            foreach (EventHandling.Event eventObject in onQuit) {
                eventObject.parameter = (object)this;
                InteractionEngine.Engine.addEvent(eventObject);
            }
        }

        /// <summary>
        /// Send an update across the network to this client.
        /// </summary>
        /// <param name="update"></param>
        internal void sendUpdate(Update update) {
            try {
                update.sendUpdate(writer, tcpClient.GetStream(), formatter);
            // The connection was closed.
            } catch (System.IO.IOException ex) {
                System.Console.WriteLine(ex);
                eventReaderThread.Abort();
                disconnect();
            } catch (System.Runtime.Serialization.SerializationException ex) {
                System.Console.WriteLine(ex);
                eventReaderThread.Abort();
                disconnect();
            } catch (System.InvalidOperationException ex) {
                System.Console.WriteLine(ex);
                eventReaderThread.Abort();
                disconnect();
            }
        }

        /// <summary>
        /// Continuously reads events from the network stream... intended to be run asynchronously by the eventReaderThread.
        /// </summary>
        private void readEvents() {
            // TODO: handle IO exceptions
            while (true) {
                try {
                    int gameObjectID = reader.ReadInt32();
                    string eventHash = reader.ReadString();
                    object parameter = formatter.Deserialize(tcpClient.GetStream());
                    lock (eventBuffer) eventBuffer.Add(new InteractionEngine.EventHandling.Event(gameObjectID, eventHash, parameter));
                // The connection was closed.
                } catch (System.IO.IOException) {
                    disconnect();
                    break;
                } catch (System.Runtime.Serialization.SerializationException) {
                    disconnect();
                    break;
                }
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
        private System.Collections.Generic.Dictionary<int, Constructs.LoadRegion> loadRegionHashlist = new System.Collections.Generic.Dictionary<int, InteractionEngine.Constructs.LoadRegion>();

        /// <summary>
        /// Adds a LoadRegion to this User's LoadRegion list. 
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to be added</param>
        internal void addLoadRegion(Constructs.LoadRegion loadRegion) {
            lock (loadRegionHashlist) {
                loadRegionHashlist.Add(loadRegion.id, loadRegion);
            }
        }

        /// <summary>
        /// Removes a LoadRegion from this User's list of LoadRegions. 
        /// </summary>
        /// <param name="id">The ID of the LoadRegion to be removed</param>
        internal void removeLoadRegion(int id) {
            if (loadRegionHashlist.ContainsKey(id)) {
                lock (loadRegionHashlist) {
                    loadRegionHashlist.Remove(id);
                }
            }
        }

        /// <summary>
        /// Get the LoadRegion list in read-only form.
        /// </summary>
        /// <returns>The LoadRegion list in read-only form.</returns>
        internal Constructs.LoadRegion[] getLoadRegionList() {
            Constructs.LoadRegion[] loadRegionArray;
            // Copy over to prevent 
            lock (loadRegionHashlist) {
                loadRegionArray = new Constructs.LoadRegion[loadRegionHashlist.Count];
                int i = 0;
                foreach (Constructs.LoadRegion loadRegion in loadRegionHashlist.Values) loadRegionArray[i++] = loadRegion;
            }
            return loadRegionArray;
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
        internal bool hasPermission(int gameObjectID) {
            return permissionList.Contains(gameObjectID);
        }

        #endregion

    }

}