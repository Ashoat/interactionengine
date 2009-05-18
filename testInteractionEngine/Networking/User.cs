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
| * User                             Class |
| * Server                           Class |
\*••••••••••••••••••••••••••••••••••••••••*/

namespace InteractionEngine.Server {

    /**
     * This class represents a networking client. It is only ever touched on the server.
     */
    public class User {
        /*
        #region Networking

        // Contains the port number we are using.
        // Used so the TcpListener knows which port to listen on for connections.
        private static int portNumber = 1337;
        // Contains a TcpListener that will monitor the port for new connections.
        // Used so that the server can catch clients who are trying
        private static System.Net.Sockets.TcpListener tcpListener;
        // Contains a list of User classes.
        // Used for looping through all the users.
        internal static System.Collections.Generic.List<Client> clientList = new System.Collections.Generic.List<Client>();
        // Contains a reference to a connection to a User.
        // Used for interfacing across a network with a client.
        private readonly System.Net.Sockets.TcpClient tcpClient;

        /// <summary>
        /// Start listening on a set port (1337 by default).
        /// </summary>
        public static void startListening() {
            if (GameWorld.GameWorld.status != GameWorld.GameWorld.Status.MULTIPLAYER_SERVER && GameWorld.GameWorld.status != GameWorld.GameWorld.Status.MULTIPLAYER_SERVERCLIENT) {
                throw new System.Exception("Something is wrong with the InteractionEngine. This is probably our bad. Sorry.");
            }
            if (tcpListener != null) return;
            tcpListener = new System.Net.Sockets.TcpListener(portNumber);
        }

        /// <summary>
        /// Start listening on a specified port.
        /// </summary>
        /// <param name="port">The port to listen on.</param>
        public static void startListening(int port) {
            if (GameWorld.GameWorld.status != GameWorld.GameWorld.Status.MULTIPLAYER_SERVER && GameWorld.GameWorld.status != GameWorld.GameWorld.Status.MULTIPLAYER_SERVERCLIENT) {
                throw new System.Exception("Something is wrong with the InteractionEngine. This is probably our bad. Sorry.");
            }
            if (tcpListener != null) return;
            portNumber = port;
            tcpListener = new System.Net.Sockets.TcpListener(portNumber);
        }

        /// <summary>
        /// Return a new client.
        /// </summary>
        /// <param name="tcpClient">The connection we are establishing the Client class around.</param>
        protected Client(System.Net.Sockets.TcpClient tcpClient) {
            this.tcpClient = tcpClient;
        }

        /// <summary>
        /// This method checks for a new 
        /// </summary>
        static internal void checkForNewUser() {
            if (tcpListener == null) return; 
            while (tcpListener.Pending()) {
                clientList.Add(new Client(tcpListener.AcceptTcpClient()));
            }
        }

        /// <summary>
        /// Sends information to this client. Only to be used by the server.
        /// </summary>
        /// <param name="information">An array of bytes to be passed to a client.</param>
        internal virtual void send(byte[] information) {
            tcpClient.GetStream().Write(information, 0, information.Length);
        }

        /// <summary>
        /// Receives a stream of bytes from the client. Returns in increments of kilobytes.
        /// </summary>
        /// <returns>A byte array containing the received information.</returns>
        internal virtual byte[] receive() {
            byte[] returnBytes = new byte[0];
            while (myNetworkStream.DataAvailable) {
                byte[] loopBytes = new byte[1024];
                tcpClient.GetStream().Read(loopBytes, 0, loopBytes.Length);
                byte[] newReturnBytes = new byte[returnBytes.Length + loopBytes.Length];
                returnBytes.CopyTo(newReturnBytes, 0);
                loopBytes.CopyTo(newReturnBytes, returnBytes.Length);
                returnBytes = newReturnBytes;
            }
            return returnBytes;
        }

        #endregion
        */
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

    /**
     * This class represents a networking server. It is only ever touched on the client.
     */
    public class Server {



    }

}