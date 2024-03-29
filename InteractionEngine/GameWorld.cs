﻿/*••••••••••••••••••••••••••••••••••••••••*\
| Interaction Engine                       |
| (C) Copyright Bluestone Coding 2008-2009 |
|••••••••••••••••••••••••••••••••••••••••••|
|           __    ___ ___  ___             |
|          /++\  | _ ) __|/ __|            |
|          \++/  | _ \__ \ (__             |
|           \/   |___/___/\___|            |
|                                          |
|••••••••••••••••••••••••••••••••••••••••••|
| GAMEWORLD                                |
| * GameWorld                 Static Class |
\*••••••••••••••••••••••••••••••••••••••••*/

namespace InteractionEngine {

    /**
     * Does like, everything. Seriously. w00t for procedural style!
     */
    public static class Engine {

        #region XNA Configuration

        /**
         * XNA Stuff
         *                 
         * Any field whose type is in the namespace "Microsoft.Xna.*" goes in here.
         */
        // Contains a reference to the XNA Game object.
        // Used by various XNA components. See XNA documentation.
        internal static InteractionGame game = new InteractionGame();
        // Contains the current game time.
        // Used for letting GameObjects know what the time is (it's game time).
        private static Microsoft.Xna.Framework.GameTime gameTimeField;
        // Contains the current game time.
        // Used for letting GameObjects know what the time is (it's game time).
        public static Microsoft.Xna.Framework.GameTime gameTime {
            get { return gameTimeField; }
        }

        #endregion

        #region Main Game Loop

        /**
         * MAIN GAME LOOP
         *                 
         * This is where all the action starts.
         */
        // Contains an enum telling the InteractionEngine what status this user is running as.
        // Used for distinguishing server/client-specific actions within the run loop.
        public enum Status {
            SINGLE_PLAYER,
            MULTIPLAYER_SERVER,
            MULTIPLAYER_CLIENT,
            MULTIPLAYER_SERVERCLIENT
        }
        private static Status realStatus = Status.SINGLE_PLAYER;
        public static Status status {
            get {
                return realStatus;
            }
            set {
                if (value == Status.MULTIPLAYER_SERVERCLIENT && (realStatus == Status.SINGLE_PLAYER || realStatus == Status.MULTIPLAYER_CLIENT)) {
                    // Make sure we're still graphing all our old "private" LoadRegions!
                    foreach (Constructs.LoadRegion loadRegion in Engine.getLoadRegionArray()) {
                        Networking.Client.addPrivateLoadRegion(loadRegion);
                    }
                } else if (value != Status.MULTIPLAYER_SERVERCLIENT && realStatus == Status.MULTIPLAYER_SERVERCLIENT) {

                }
                // Don't want any collisions with existing numbers if the client had an GameObject or LoadRegion with an ID the server sent them.
                // Really only need this for the server, but better safe than sorry.
                nextLoadRegionID = nextLoadRegionID + 100;
                nextGameObjectID = nextGameObjectID + 1000;
                realStatus = value;
            }
        }
        // Contains a reference to the UI class.
        // Used by the client to execute output.
        public static UserInterface.UserInterface userInterface;

        /// <summary>
        /// Start the game!
        /// </summary>
        public static void run() {
            game.Run();
        }

        /// <summary>
        /// Run the GameWorld logic.
        /// </summary>
        /// <param name="GameTime">The current game time.</param>
        internal static void runGameWorld(Microsoft.Xna.Framework.GameTime gameTime) {
            gameTimeField = gameTime;
            if (status == Status.SINGLE_PLAYER) {
                // Get Events from the GameWorld
                System.Collections.Generic.List<EventHandling.Event> events = userInterface.input();
                // Get Events from the InteractionEngine
                events.AddRange(getEvents());
                // Process the Events locally
                processEvents(events);
            } else if (status == Status.MULTIPLAYER_SERVERCLIENT) {
                // Get Events from the GameWorld
                System.Collections.Generic.List<EventHandling.Event> events = userInterface.input();
                // Get Events from the InteractionEngine
                events.AddRange(getEvents());
                // Process the Events locally
                processEvents(events);
                // Get and handle Events from Clients
                handleInput();
                // Send updates to clients
                sendUpdates();
            } else if (status == Status.MULTIPLAYER_CLIENT) {
                // Get Events from the GameWorld
                System.Collections.Generic.List<EventHandling.Event> events = userInterface.input();
                // Send Events to be processed remotely by the server
                sendInput(events);
                // Get Events from the InteractionEngine
                events.AddRange(getEvents());
                // Process the Events locally
                processEvents(events);
                // Recieve and process updates from the server
                if (status == Status.MULTIPLAYER_CLIENT) receiveUpdate();
            } else if (status == Status.MULTIPLAYER_SERVER) {
                // Get and handle Events from Clients
                handleInput();
                // Send updates to clients
                sendUpdates();
            }
        }

        /// <summary>
        /// Run the graphics.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        internal static void draw(Microsoft.Xna.Framework.GameTime gameTime) {
            gameTimeField = gameTime;
            if (userInterface == null) throw new GameWorldException("You cannot call Engine.game.Run() before you assign a UserInterface object to Engine.userInterface.");
            userInterface.startInputOutput();
            if (status != Status.MULTIPLAYER_SERVER) {
                userInterface.output();
            }
        }

        /// <summary>
        /// Process Events that have been generated locally and call methods related to them. 
        /// </summary>
        /// <param name="events">The list of events to process.</param>
        private static void processEvents(System.Collections.Generic.List<EventHandling.Event> events) {
            foreach (EventHandling.Event eventObject in events) {
                if (eventObject == null) return;
                if (Engine.getGameObject(eventObject.gameObjectID) == null) return;
                Engine.getGameObject(eventObject.gameObjectID).getEventMethod(eventObject.eventHash)(null, eventObject.parameter);
            }
        }

        #endregion

        #region Event Cache

        /**
         * EVENT CACHE
         * 
         * This is a way for the InteractionEngine (as opposed to the GameWorld) to add Events to be processed by the GameWorld.
         * On the other hand, the GameWorld using a UserInterface object to get Events for processing.
         */

        // Contains a list of events that need to be executed on every run of the main game loop. The InteractionEngine adds Events here.
        // Used so that the InteractionEngine can trigger Events on certain events; ie. the GameWorld tells the InteractionEngine to trigger an Event when a new client joins a game.
        private static System.Collections.Generic.List<EventHandling.Event> eventCache = new System.Collections.Generic.List<InteractionEngine.EventHandling.Event>();

        /// <summary>
        /// This method adds an Event to the EventCache.
        /// </summary>
        /// <param name="theEvent">The Event to add to the EventCache.</param>
        public static void addEvent(EventHandling.Event theEvent) {
            eventCache.Add(theEvent);
        }

        /// <summary>
        /// This method removes an Event from the EventCache.
        /// </summary>
        /// <param name="theEvent">The Event to add to the EventCache.</param>
        internal static void removeGameObjectEvents(Constructs.GameObject gameObject) {
            for (int i = 0; i < eventCache.Count; i++) {
                if (eventCache[i].gameObjectID == gameObject.id) eventCache.RemoveAt(i);
            }
        }

        /// <summary>
        /// This method gets and clears the EventCache.
        /// </summary>
        /// <returns>All the Events in the EventCache.</returns>
        private static System.Collections.Generic.List<EventHandling.Event> getEvents() {
            System.Collections.Generic.List<EventHandling.Event> returnCache = new System.Collections.Generic.List<InteractionEngine.EventHandling.Event>(eventCache);
            int a = 0;
            foreach (EventHandling.Event evvie in returnCache)
                a++; 
            eventCache.Clear();
            return returnCache;
        }

        #endregion

        #region Client Networking

        /**
         * CLIENT NETWORKING
         *                 
         * These methods handle communication from the client side.
         */

        // Contains a reference to the Server object.
        // Used for handling basic networking operations for clients.
        public static Networking.Server server;

        /// <summary>
        /// Send a byte array to the server containing called events. This method is only used on the client.
        /// </summary>
        /// <param name="events">Events that have been called by this client.</param>
        private static void sendInput(System.Collections.Generic.List<EventHandling.Event> events) {
            if (server == null) return;
            foreach (EventHandling.Event eventObject in events)
                server.sendEvent(eventObject);
        }

        /// <summary>
        /// Recieve and process updates from the server. This method is only used on the client.
        /// </summary>
        private static void receiveUpdate() {
            if (server == null) return;
            System.Collections.Generic.List<Networking.Update> updates = server.getUpdates();
            foreach (Networking.Update update in updates) {
                update.executeUpdate();
            }
        }

        #endregion

        #region Server Networking

        /**
         * SERVER NETWORKING
         *                 
         * These methods handle communication from the server side.
         */

        /// <summary>
        /// Get and process all Events sent by Clients.
        /// </summary>
        private static void handleInput() {
            lock (Networking.Client.clientList) {
                foreach (Networking.Client client in Networking.Client.clientList) {
                    // Get the events.
                    System.Collections.Generic.List<EventHandling.Event> events = client.getEvents();
                    // Process the events.
                    foreach (EventHandling.Event eventObject in events) {
                        if (eventObject != null && client.hasPermission(eventObject.gameObjectID))
                            Engine.getGameObject(eventObject.gameObjectID).getEventMethod(eventObject.eventHash)(client, eventObject.parameter);
                    }
                }
            }
        }

        /// <summary>
        /// Send an update to every User. This method is only used on the server.
        /// </summary>
        private static void sendUpdates() {
            lock (Networking.Client.clientList) {
                // Send an update to every client.
                foreach (Networking.Client client in Networking.Client.clientList) {
                    // Update each of the client's LoadRegions.
                    foreach (InteractionEngine.Constructs.LoadRegion loadRegion in client.getLoadRegionList()) {
                        foreach (Networking.Update update in loadRegion.getUpdates())
                            client.sendUpdate(update);
                    }
                }
            }
            // Reset the cache in every LoadRegion.
            foreach (Constructs.LoadRegion region in getLoadRegionArray()) region.resetBuffer();
        }

        #endregion

        #region GameObject Hashlist

        /**
         * GAMEOBJECT HASHLIST
         *                 
         * Contains a dictionary that links GameObject IDs with GameObjects.
         * Used for having a synchronized method of referring to GameObjects across the network.
         * All GameObjects add themselves to this list in their constructors.
         */
        private static System.Collections.Generic.SortedDictionary<int, Constructs.GameObjectable> gameObjectHashlist = new System.Collections.Generic.SortedDictionary<int, Constructs.GameObjectable>();
        // Contains the lowest available ID for the next GameObject.
        // Used for knowing what ID the Server should assign a new GameObject.
        private static int nextGameObjectID = 0;

        /// <summary>
        /// This adds a GameObject to the GameObject Hashlist.
        /// </summary>
        /// <param name="gameObject">The GameObject to be added.</param>
        internal static void addGameObject(Constructs.GameObjectable gameObject) {
            if (gameObjectHashlist.ContainsKey(gameObject.id)) return;
            lock (gameObjectHashlist) {
                gameObjectHashlist.Add(gameObject.id, gameObject);
            }
        }

        /// <summary>
        /// Assign an ID to a GameObject. Should only ever be called on a server, but it won't work if it's called otherwise so whatever.
        /// </summary>
        /// <param name="gameObject"></param>
        internal static void assignGameObjectID(Constructs.GameObjectable gameObject) {
            // This will only work if the GameObject doesn't already have an ID.
            gameObject.id = nextGameObjectID++;
        }

        /// <summary>
        /// This removes a GameObject from the GameObject Hashlist. 
        /// </summary>
        /// <param name="id">The id of the GameObject to be removed</param>
        internal static void removeGameObject(int id) {
            if (gameObjectHashlist.ContainsKey(id)) {
                lock (gameObjectHashlist) {
                    gameObjectHashlist.Remove(id);
                }
            }
        }

        /// <summary>
        /// This method retrieves a GameObject from the GameObject Hashlist.
        /// </summary>
        /// <param name="id">The ID of the GameObject to retrieve.</param>
        /// <returns>The requested GameObject.</returns>
        public static Constructs.GameObjectable getGameObject(int id) {
            if (gameObjectHashlist.ContainsKey(id)) return gameObjectHashlist[id];
            else return null;
        }

        /// <summary>
        /// Get a list of all the GameObjects in the GameWorld.
        /// </summary>
        /// <returns>All the GameObjects in the GameWorld.</returns>
        public static Constructs.GameObjectable[] getGameObjectArray() {
            Constructs.GameObjectable[] gameObjectArray;
            // Copy over to prevent 
            lock (gameObjectHashlist) {
                gameObjectArray = new Constructs.GameObjectable[gameObjectHashlist.Count];
                int i = 0;
                foreach(Constructs.GameObjectable gameObject in gameObjectHashlist.Values) gameObjectArray[i++] = gameObject;
            }
            return gameObjectArray;
        }

        #endregion

        #region LoadRegion Hashlist

        /**
         * LOADREGION HASHLIST
         *                 
         * Contains a dictionary that links LoadRegion IDs with LoadRegions.
         * Used for having a synchronized method of referring to LoadRegions across the network.
         * All LoadRegions add themselves to this list in their constructors.
         */
        private static System.Collections.Generic.Dictionary<int, Constructs.LoadRegion> loadRegionHashlist = new System.Collections.Generic.Dictionary<int, Constructs.LoadRegion>();
        // Contains the lowest available ID for the next LoadRegion.
        // Used for knowing what ID the Server should assign a new LoadRegion.
        private static int nextLoadRegionID = 0;

        /// <summary>
        /// This adds a LoadRegion to the LoadRegion Hashlist.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to be added.</param>
        internal static void addLoadRegion(Constructs.LoadRegion loadRegion) {
            // This will only work if the LoadRegion doesn't already have an ID.
            // Since on a MULTIPLAYER_CLIENT this ID would have already been set, nothing will happen in that case.
            loadRegion.id = nextLoadRegionID++;
            if (loadRegionHashlist.ContainsKey(loadRegion.id)) return;
            lock (loadRegionHashlist) {
                loadRegionHashlist.Add(loadRegion.id, loadRegion);
            }
        }

        /// <summary>
        /// This removes a LoadRegion from the LoadRegion Hashlist. 
        /// </summary>
        /// <param name="id">The id of the LoadRegion to be removed</param>
        internal static void removeLoadRegion(int id) {
            if (loadRegionHashlist.ContainsKey(id)) {
                lock (loadRegionHashlist) {
                    loadRegionHashlist.Remove(id);
                }
            }
        }

        /// <summary>
        /// This method retrieves a LoadRegion from the LoadRegion Hashlist.
        /// </summary>
        /// <param name="id">The ID of the LoadRegion to retrieve.</param>
        /// <returns>The requested LoadRegion.</returns>
        public static Constructs.LoadRegion getLoadRegion(int id) {
            if (loadRegionHashlist.ContainsKey(id)) return loadRegionHashlist[id];
            else return null;
        }

        /// <summary>
        /// Get an array of all the LoadRegions in the GameWorld.
        /// </summary>
        /// <returns>All the LoadRegions in the GameWorld.</returns>
        public static Constructs.LoadRegion[] getLoadRegionArray() {
            Constructs.LoadRegion[] loadRegionArray;
            // Copy over to prevent 
            lock (loadRegionHashlist) {
                loadRegionArray = new Constructs.LoadRegion[loadRegionHashlist.Count];
                int i = 0;
                foreach(Constructs.LoadRegion loadRegion in loadRegionHashlist.Values) loadRegionArray[i++] = loadRegion;
            }
            return loadRegionArray;
        }

        /// <summary>
        /// Get the LoadRegions that need to be graphed from the GameWorld.
        /// </summary>
        /// <returns>All the LoadRegions that need to be graphed.</returns>
        internal static Constructs.LoadRegion[] getGraphableLoadRegions() {
            if (status == Status.MULTIPLAYER_SERVERCLIENT) return Networking.Client.getPrivateLoadRegionList();
            else if (status == Status.MULTIPLAYER_CLIENT || status == Status.SINGLE_PLAYER) return getLoadRegionArray();
            else return null;
        }

        #endregion

    }

}