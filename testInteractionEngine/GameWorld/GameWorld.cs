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
         * We want to eventually deprecate this. 
         */
        // Contains a reference to the XNA Game object.
        // Used by various XNA components. See XNA documentation.
        public static InteractionGame game;
        // Contains the current game time.
        // Used for letting GameObjects know what the time is (it's game time).
        private static Microsoft.Xna.Framework.GameTime gameTimeField;
        // Contains the current game time.
        // Used for letting GameObjects know what the time is (it's game time).
        public static Microsoft.Xna.Framework.GameTime gameTime {
            get { return gameTimeField; }
        }

        #endregion

        #region Networking Configuration

        /**
         * NETWORKING CONFIGURATION
         *                 
         * This is where you configure networking information.
         */
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
                // Get and process input from the local client
                System.Collections.Generic.List<Client.Event> localEvents = userInterface.input();
                processEvents(localEvents);
                // Run repeaters
                processRepeaters();
                // Output graphics
                userInterface.output();
             } /*else if (status == Status.MULTIPLAYER_SERVERCLIENT) {
                // Get and process input from the local client
                System.Collections.Generic.List<Client.Event> localEvents = userInterface.input();
                processEvents(localEvents);
                // Get and process input from remote clients
                System.Collections.Generic.List<Client.Event> remoteEvents = readInput();
                processEvents(remoteEvents);
                // Send updates to clients
                sendUpdate();
                // Run repeaters
                processRepeaters();
                // Output graphics
                userInterface.output();
            } else if (status == Status.MULTIPLAYER_CLIENT) {
                // Get and process input from the local client
                System.Collections.Generic.List<Client.Event> localEvents = userInterface.input();
                // Send input to server
                sendInput(localEvents);
                // Process input locally
                processEvents(localEvents);
                // Recieve and process updates from the server
                receiveUpdate();
                // Run repeaters
                processRepeaters();
                // Output graphics
                userInterface.output();
            } else if (status == Status.MULTIPLAYER_SERVER) {
                // Get and process input from remote clients
                System.Collections.Generic.List<Client.Event> remoteEvents = readInput();
                processEvents(remoteEvents);
                // Send updates to clients
                sendUpdate();
                // Run repeaters
                processRepeaters();
            }*/
        }

        #endregion

        #region Repeating Methods

        /**
         * REPEATER CLASS
         * 
         * If a method needs to be repeated every run through the game, then an in-game method can insert a Repeater into the Repeater list.
         * This class holds a RepeatMethod, how often it must be repeated, and the maximum number of times it can be repeated.
         */
        public class Repeater {

            //Constructor info
            internal RepeatMethod repeatMethod;
            internal int repeatTime = 0;

            //State info
            internal System.DateTime lastRepeat = new System.DateTime();
            internal int repeatsLeft = 0;

            /**
             * Instantiate the repeater.
             * @param repeatMethod The method that will be repeated.
             * @param repeatTime The time, in milliseconds, between repeats.
             * @param maxRepeats The maximum number of times this method can be run.
             */
            public Repeater(RepeatMethod repeatMethod, int repeatTime, int maxRepeats) {
                this.repeatMethod = repeatMethod;
                this.repeatTime = repeatTime;
                this.repeatsLeft = maxRepeats;
            }

        }

        // Contains references to any methods that we want to be automatically repeated every run through the loop.
        // Used for runtime animation.
        private static System.Collections.Generic.List<Repeater> repeaterList = new System.Collections.Generic.List<Repeater>();

        /// <summary>
        /// Add a repeater. Can only be removed by the method it calls, or by eclipsing its repeatTime or maxRepeats.
        /// </summary>
        /// <param name="repeater">The repeater that is being added.</param>
        public static void addRepeater(Repeater repeater) {
            repeaterList.Add(repeater);
        }

        /// <summary>
        /// Private method to remove a repeater.
        /// </summary>
        /// <param name="repeater">The repeater to remove.</param>
        private static void removeRepeater(int id) {
            repeaterList.RemoveAt(id);
        }

        /// <summary>
        /// Process the list of repeaters.
        /// </summary>
        private static void processRepeaters() {
            System.DateTime currentTime = new System.DateTime();
            // For every repeater...
            for (int i = 0; i < repeaterList.Count; ) {
                if ((currentTime.Ticks - repeaterList[i].lastRepeat.Ticks) / System.TimeSpan.TicksPerMillisecond <= 0) {
                    // Call the method. If it tells use to, remove the repeater.
                    if (repeaterList[i].repeatMethod()) {
                        removeRepeater(i);
                        continue;
                    }
                    repeaterList[i].lastRepeat = new System.DateTime();
                    // If no more time is left, remove the repeater.
                } else removeRepeater(i);
                // If no more repeats can be made, remove the repeater.
                if (--repeaterList[i].repeatsLeft <= 0) {
                    removeRepeater(i);
                    continue;
                }
                i++;
            }
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
        /// These constants are the first information passed in an update from the server. 
        /// They help determine how the client's GameWorld needs to handle that update.
        /// </summary>
        public const byte CREATE_OBJECT = 0;    // This code tells the GameWorld to instantiate a new GameObject using that GameObject's factory.
        public const byte DELETE_OBJECT = 1;    // This code tells the GameWorld to delete a GameObject and to remove its reference from its LoadRegion.
        public const byte UPDATE_FIELD = 2;     // This code tells the GameWorld to update an Updatable.

        /// <summary>
        /// Send a byte array to the server containing called events. This method is only used on the client.
        /// </summary>
        /// <param name="events">Events that have been called by this client.</param>
        private static void sendInput(System.Collections.Generic.List<Client.Event> events) {
            foreach (Client.Event eventObject in events) {
                Microsoft.Xna.Framework.Net.PacketWriter packetwriter = new Microsoft.Xna.Framework.Net.PacketWriter();
                // Write the informaton
                packetwriter.Write(eventObject.gameObjectID);
                packetwriter.Write(eventObject.eventHash);
                // Write the parameter
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                formatter.Serialize(packetwriter.BaseStream, eventObject.parameter);
                // Send the information to the server
                game.session.LocalGamers[0].SendData(packetwriter, Microsoft.Xna.Framework.Net.SendDataOptions.Reliable, game.session.Host);
            }
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
        // Contains a dictionary pointing all NetworkGamer objects to their corresponding Server.User objects. Local gamer is not included.
        // Used for converting the information from the XNA recieve method into the a format compatible with the Interaction Engine.
        private static System.Collections.Generic.Dictionary<Microsoft.Xna.Framework.Net.NetworkGamer, InteractionEngine.Server.User> userHashlist = new System.Collections.Generic.Dictionary<Microsoft.Xna.Framework.Net.NetworkGamer, InteractionEngine.Server.User>();

        /// <summary>
        /// Process the byte array received from the client. This method is only used on the server.
        /// </summary>
        /// <param name="reader">A PacketReader containing all data recieved.</param>
        private static System.Collections.Generic.List<Client.Event> readInput(Microsoft.Xna.Framework.Net.PacketReader reader) {
            System.Collections.Generic.List<Client.Event> returnList = new System.Collections.Generic.List<InteractionEngine.Client.Event>();
            while (game.session.LocalGamers[0].IsDataAvailable) {
                // Instantiate the reader and the gamer objects, and recieve the data.
                Microsoft.Xna.Framework.Net.PacketReader preader = new Microsoft.Xna.Framework.Net.PacketReader();
                Microsoft.Xna.Framework.Net.NetworkGamer otherGamer;
                game.session.LocalGamers[0].ReceiveData(preader, out otherGamer);
                // Get the information
                int gameObjectID = preader.ReadInt32();
                string eventHash = preader.ReadString();
                // Get the parameter
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                object parameter = formatter.Deserialize(preader.BaseStream);
                // Construct an event
                returnList.Add(new InteractionEngine.Client.Event(gameObjectID, eventHash, parameter));
            }
            return returnList;
        }

        /// <summary>
        /// Send an update to every User. This method is only used on the server.
        /// </summary>
        private static void sendUpdate() {
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

        /// <summary>
        /// Process events and call methods related to them. This is a server method.
        /// </summary>
        /// <param name="events">The list of events to process.</param>
        private static void processEvents(System.Collections.Generic.List<Client.Event> events) {
            foreach (Client.Event eventObject in events) {
                if (eventObject != null) {
                    GameWorld.getObject(eventObject.gameObjectID).getEvent(eventObject.eventHash)(eventObject.parameter);
                }
            }
        }

        #endregion

        #region Field Container Hashlist

        /**
         * FIELD CONTAINER HASHLIST
         *                 
         * Contains a dictionary that links Field Container IDs with Field Containers.
         * Used for sending across a reference to a Field Container with a CREATE_FIELD message. All fields need to be linked to Field Containers, and a centralized list is the only way to do this.
         * Also, all GameObject add themselves to this list in their constructors.
         * This list is also used . 
         */
        private static System.Collections.Generic.Dictionary<int, Constructs.FieldContainer> fieldContainerHashlist = new System.Collections.Generic.Dictionary<int, Constructs.FieldContainer>();

        // Contains the lowest available ID for the next FieldContainer.
        // Used for knowing what ID the Server should assign a new FieldContainer.
        public static int nextID = 0;

        /// <summary>
        /// Blah!
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Constructs.FieldContainer getFieldContainer(int id) {
            return fieldContainerHashlist[id];
        }

        /// <summary>
        /// This removes a GameObject from the GameObject table. 
        /// </summary>
        /// <param name="id">The id of the GameObject to be removed</param>
        public static void removeFieldContainer(int id) {
            fieldContainerHashlist.Remove(id);
        }

        /// <summary>
        /// This method returns the number of GameObjects in the GameWorld.
        /// </summary>
        /// <returns>The number of GameObjects in the GameWorld.</returns>
        public static int getFieldContainerCount() {
            return fieldContainerHashlist.Count;
        }

        #region Object List

        /// <summary>
        /// This adds a GameObjectable to the FieldContainer table.
        /// </summary>
        /// <param name="gameObject">The GameObjectable to be added.</param>
        /// <returns>The ID assigned to the GameObjectable</returns>
        public static int addObject(Constructs.GameObjectable gameObject) {
            // If you're not a MULTIPLAYER_CLIENT, get an ID.
            int assignedID = gameObject.getID();
            if (assignedID == -1) assignedID = nextID++;
            else if (assignedID > nextID) nextID = assignedID + 1;
            fieldContainerHashlist.Add(assignedID, gameObject);
            return assignedID;
        }


        /// <summary>
        /// This method returns a GameObject from the FieldContainer table.
        /// </summary>
        /// <param name="id">The ID of the GameObject you want to retrieve.</param>
        /// <returns>The GameObject being returned.</returns>
        public static Constructs.GameObjectable getObject(int id) {
            return (Constructs.GameObject)fieldContainerHashlist[id];
        }

        #endregion

        #region LoadRegion List

        /// <summary>
        /// This adds a LoadRegion to the FieldContainer table.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to be added.</param>
        /// <returns>The ID of the LoadRegion</returns>
        public static int addLoadRegion(Constructs.LoadRegion loadRegion) {
            if (GameWorld.status == InteractionEngine.GameWorld.GameWorld.Status.MULTIPLAYER_CLIENT) {
                if (loadRegion.id < 0) throw new System.Exception("Clients should not have to figure out what ID a field has.");
                fieldContainerHashlist.Add(loadRegion.id, loadRegion);
                return loadRegion.id;
            } else {
                // If you're not a MULTIPLAYER_CLIENT, get an ID.
                fieldContainerHashlist.Add(loadRegion.id, loadRegion);
                return nextID++;
            }
        }

        /// <summary>
        /// This method returns a LoadRegion from the FieldContainer table.
        /// </summary>
        /// <param name="id">The ID of the LoadRegion you want to retrieve.</param>
        /// <returns>The LoadRegion being returned.</returns>
        public static Constructs.LoadRegion getLoadRegion(int id) {
            return (Constructs.LoadRegion)fieldContainerHashlist[id];
        }

        #endregion

        #endregion

    }

}