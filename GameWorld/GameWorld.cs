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
| GAME WORLD                               |
| * RepeatMethod              Delegate     |
| * Repeater                  Class        |
| * GameWorld                 Static Class |
\*••••••••••••••••••••••••••••••••••••••••*/

namespace InteractionEngine.GameWorld {

    /**
     * This class holds a reference to a method that needs to be checked every once in a while.
     * @return True means doesn't need to be repeated anymore.
     */
    public delegate bool RepeatMethod();

    /**
     * Does like, everything. Seriously. w00t for procedural style!
     */
    public static class GameWorld {

        // TODO: Make a central reference for all GameObject factories, pointing types (string) to GameObject factory methods.

        #region XNA Configuration

        /**
         * XNA Stuff
         *                 
         * Any field whose type is in the namespace "Microsoft.Xna.*" goes in here.
         */
        // Contains a reference to the XNA Game object.
        // Used by various XNA components. See XNA documentation.
        public static InteractionGame game;
        // Contains a reference to the XNA object for the local gamer, which contains methods for sending and recieving data.
        // Used for sending and recieving data.
        internal static Microsoft.Xna.Framework.Net.LocalNetworkGamer gamer;
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
        // Contains a boolean telling the GameWorld whether or not the run loop needs to be run.
        // Used for starting and ending the use of the GameWorld class. 
        public static bool gameRunning = true;
        // Contains an enum telling the InteractionEngine what status this user is running as.
        // Used for distinguishing server/client-specific actions within the run loop.
        public enum Status {
            SINGLE_PLAYER,
            MULTIPLAYER_SERVER,
            MULTIPLAYER_CLIENT,
            MULTIPLAYER_SERVERCLIENT
        }
        public static Status status;
        // Contains a reference to the local Server.User.
        // Used for passing the Server.User to methods when calling them locally.
        public static InteractionEngine.Server.User user;
        // Contains a reference to the UI class.
        // Used by the client to execute output.
        public static Client.UserInterface userInterface;

        /// <summary>
        /// Run the game.
        /// </summary>
        /// <param name="GameTime">The current game time.</param>
        public static void run(Microsoft.Xna.Framework.GameTime gameTime) {
            gameTimeField = gameTime;
            if (status == Status.SINGLE_PLAYER) {
                // Get Events from the GameWorld
                System.Collections.Generic.List<EventHandling.Event> events = userInterface.input();
                // Get Events from the InteractionEngine
                events.AddRange(getEvents());
                // Process the Events locally
                processEvents(events);
                // Output graphics
                userInterface.output();
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
                sendUpdate();
                // Output graphics
                userInterface.output();
            } else if (status == Status.MULTIPLAYER_CLIENT) {
                // Get Events from the GameWorld
                System.Collections.Generic.List<EventHandling.Event> events = userInterface.input();
                // Get Events from the InteractionEngine
                events.AddRange(getEvents());
                // Send Events to be processed remotely by the server
                sendInput(events);
                // Process the Events locally
                processEvents(events);
                // Recieve and process updates from the server
                receiveUpdate();
                // Output graphics
                userInterface.output();
            } else if (status == Status.MULTIPLAYER_SERVER) {
                // Get and handle Events from Clients
                handleInput();
                // Send updates to clients
                sendUpdate();
            }
        }

        /// <summary>
        /// Process Events that have been generated locally and call methods related to them. 
        /// </summary>
        /// <param name="events">The list of events to process.</param>
        private static void processEvents(System.Collections.Generic.List<EventHandling.Event> events) {
            foreach (EventHandling.Event eventObject in events)
                GameWorld.getObject(eventObject.gameObjectID).getEvent(eventObject.eventHash)(null, eventObject.parameter);
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
        internal static void addEvent(EventHandling.Event theEvent) {
            eventCache.Add(theEvent);
        }

        /// <summary>
        /// This method gets and clears the EventCache.
        /// </summary>
        /// <returns>All the Events in the EventCache.</returns>
        private static System.Collections.Generic.List<EventHandling.Event> getEvents() {
            System.Collections.Generic.List<EventHandling.Event> returnCache = eventCache;
            eventCache = new System.Collections.Generic.List<InteractionEngine.EventHandling.Event>();
            return returnCache;
        }

        #endregion

        #region Client Networking

        /**
         * CLIENT NETWORKING
         *                 
         * These methods handle communication from the client side.
         */

        /// <summary>
        /// Transfer code constants.
        /// These constants are the first information passed in an update packet from the server. 
        /// They help determine how the client's InteractionEngine needs to handle that update.
        /// </summary>
        public const byte CREATE_OBJECT = 0;    // This code tells the client to instantiate a new GameObject using that GameObject's factory.
        public const byte CREATE_REGION = 1;    // This code tells the client to instantiate a new LoadRegion.
        public const byte DELETE_REGION = 2;    // This code tells the client to delete a LoadRegion.
        public const byte DELETE_OBJECT = 3;    // This code tells the client to delete a GameObject and to remove its reference from its LoadRegion.
        public const byte UPDATE_FIELD = 4;     // This code tells the client to update an Updatable.
        public const byte MOVE_OBJECT = 5;      // This code tells the client to move an object from one LoadRegion to another.

        private static Networking.Server server;

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
            while (game.session.LocalGamers[0].IsDataAvailable) {
                // Instantiate the reader and the gamer objects, and recieve the data.
                Microsoft.Xna.Framework.Net.PacketReader reader = new Microsoft.Xna.Framework.Net.PacketReader();
                Microsoft.Xna.Framework.Net.NetworkGamer otherGamer;
                game.session.LocalGamers[0].ReceiveData(reader, out otherGamer);
                while (reader.Position < reader.Length) {
                    byte transferCode = reader.ReadByte();
                    switch (transferCode) {
                        case CREATE_OBJECT:
                            createObject(reader);
                            break;
                        case DELETE_OBJECT:
                            deleteObject(reader);
                            break;
                        case UPDATE_FIELD:
                            updateField(reader);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Process the byte array received from the server. This method is only used on the client.
        /// </summary>
        /// <param name="reader">A PacketReader containing all data recieved.</param>
        public static void createObject(Microsoft.Xna.Framework.Net.PacketReader reader) {
            Constructs.LoadRegion loadRegion = (Constructs.LoadRegion)fieldContainerHashlist[reader.ReadInt32()];
            string classHash = reader.ReadString();
            int gameObjectID = reader.ReadInt32();
            Constructs.GameObject.factoryList[classHash](loadRegion, gameObjectID, reader);
        }

        /// <summary>
        /// Process the byte array received from the server. This method is only used on the client.
        /// </summary>
        /// <param name="reader">A PacketReader containing all data recieved.</param>
        private static void deleteObject(Microsoft.Xna.Framework.Net.PacketReader reader) {
            Constructs.GameObjectable gameObject = getObject(reader.ReadInt32());
            gameObject.getLoadRegion().removeObject(gameObject.getID());
            removeFieldContainer(gameObject.getID());
        }

        /// <summary>
        /// Process the byte array received from the server. This method is only used on the client.
        /// </summary>
        /// <param name="reader">A PacketReader containing all data recieved.</param>
        public static void updateField(Microsoft.Xna.Framework.Net.PacketReader reader) {

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
            foreach (Networking.Client client in Networking.Client.clientList) {
                // Get the events.
                System.Collections.Generic.List<EventHandling.Event> events = client.getEvents();
                // Process the events.
                foreach (EventHandling.Event eventObject in events) {
                    if (client.hasPermission(eventObject.gameObjectID))
                        GameWorld.getObject(eventObject.gameObjectID).getEvent(eventObject.eventHash)(client, eventObject.parameter);
                }
            }
        }

        /// <summary>
        /// Send an update to every User. This method is only used on the server.
        /// </summary>
        private static void sendUpdate() {
            // TODO make work with the recent revamping of the networking system
            // Send an update to every user.
            foreach (System.Collections.Generic.KeyValuePair<Microsoft.Xna.Framework.Net.NetworkGamer, InteractionEngine.Server.User> pair in userHashlist) {
                // Update each of the user's LoadRegions.
                foreach (InteractionEngine.Constructs.LoadRegion loadRegion in pair.Value.getLoadRegionList())
                    loadRegion.sendUpdate(pair.Key);
            }
            // Reset the cache in every LoadRegion.
            foreach (System.Collections.Generic.KeyValuePair<int, Constructs.FieldContainer> pair in fieldContainerHashlist) {
                Constructs.FieldContainer region = pair.Value;
                if (region is Constructs.LoadRegion) ((Constructs.LoadRegion)region).resetCache();
            }
        }

        #endregion

        #region FieldContainer Hashlist

        /**
         * FIELDCONTAINER HASHLIST
         *                 
         * Contains a dictionary that links FieldContainer IDs with FieldContainers.
         * Used for having a synchronized method of referring to GameObjects and LoadRegions across the network.
         * All GameObject and LoadRegions add themselves to this list in their constructors.
         */
        private static System.Collections.Generic.Dictionary<int, Constructs.FieldContainer> fieldContainerHashlist = new System.Collections.Generic.Dictionary<int, Constructs.FieldContainer>();
        // Contains the lowest available ID for the next FieldContainer.
        // Used for knowing what ID the Server should assign a new FieldContainer.
        public static int nextID = 0;

        /// <summary>
        /// This adds a FieldContainer to the FieldContainer Hashlist.
        /// </summary>
        /// <param name="gameObject">The FieldContainer to be added.</param>
        public static void addFieldContainer(Constructs.FieldContainer fieldContainer) {
            // This will only work if the GameObject doesn't already have an ID.
            // Since on a MULTIPLAYER_CLIENT this ID would have already been set, nothing will happen in that case.
            fieldContainer.id = nextID++;
            fieldContainerHashlist.Add(fieldContainer.id, fieldContainer);
        }

        /// <summary>
        /// This removes a FieldContainer from the FieldContainer Hashlist. 
        /// </summary>
        /// <param name="id">The id of the GameObject to be removed</param>
        public static void removeFieldContainer(int id) {
            if (fieldContainerHashlist.ContainsKey(id)) fieldContainerHashlist.Remove(id);
        }

        /// <summary>
        /// This method retrieves a FieldContainer from the Field Container Hashlist.
        /// </summary>
        /// <param name="id">The ID of the FieldContainer to retrieve.</param>
        /// <returns>The requested FieldContainer.</returns>
        public static Constructs.FieldContainer getFieldContainer(int id) {
            if (fieldContainerHashlist.ContainsKey(id)) return fieldContainerHashlist[id];
            else return null;
        }

        /// <summary>
        /// This method returns the number of FieldContainers in the GameWorld.
        /// </summary>
        /// <returns>The number of FieldContainers in the GameWorld.</returns>
        public static int getFieldContainerCount() {
            return fieldContainerHashlist.Count;
        }

        #region Object List

        /// <summary>
        /// This method returns a GameObject from the FieldContainer table.
        /// </summary>
        /// <param name="id">The ID of the GameObject you want to retrieve.</param>
        /// <returns>The GameObject being returned.</returns>
        public static Constructs.GameObject getObject(int id) {
            if (fieldContainerHashlist.ContainsKey(id)) {
                Constructs.FieldContainer returnObject = fieldContainerHashlist[id];
                if (returnObject is Constructs.GameObject) return (Constructs.GameObject)returnObject;
                else return null;
            } else return null;
        }

        #endregion

        #region LoadRegion List

        /// <summary>
        /// This method returns a LoadRegion from the FieldContainer table.
        /// </summary>
        /// <param name="id">The ID of the LoadRegion you want to retrieve.</param>
        /// <returns>The LoadRegion being returned.</returns>
        public static Constructs.LoadRegion getObject(int id) {
            if (fieldContainerHashlist.ContainsKey(id)) {
                Constructs.FieldContainer returnRegion = fieldContainerHashlist[id];
                if (returnRegion is Constructs.LoadRegion) return (Constructs.LoadRegion)returnRegion;
                else return null;
            } else return null;
        }


        #endregion

        #endregion

    }

}