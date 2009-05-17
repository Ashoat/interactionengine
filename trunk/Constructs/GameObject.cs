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
| CONSTRUCTS                               |
| * GameObject              Abstract Class |
\*••••••••••••••••••••••••••••••••••••••••*/

namespace InteractionEngine.Constructs {

    /**
     * This delegate holds references to GameObject factory methods.
     */
    public delegate GameObject GameObjectFactory(LoadRegion loadRegion, int id, Microsoft.Xna.Framework.Net.PacketReader reader);

    /**
     * This interface is extended by Module types. See documentation for more information on Modules.
     */
    public interface GameObjectable : FieldContainer {

        /// <summary>
        /// Returns this GameObject's LoadRegion.
        /// </summary>
        /// <returns>This GameObject's LoadRegion.</returns>
        LoadRegion getLoadRegion();
        
        /// <summary>
        /// Gets an event from the eventHashlist.
        /// </summary>s
        /// <param name="hash">The string that represents the event.</param>
        /// <returns>The method that is callde by the event.</returns>
        InteractionEngine.EventHandling.EventMethod getEvent(string hash);

        /// <summary>
        /// Get the eventHashlist's enumerator.
        /// </summary>
        /// <returns>The eventHashlist's enumerator.</returns>
        System.Collections.Generic.Dictionary<string, EventMethod>.Enumerator getEventEnumerator();

    }

    /**
     * A GameObject is an object within the game.
     * It consists of modules and actions.
     */
    public abstract class GameObject : GameObjectable {

        // Contains the ID of this GameObject. Must be positive.
        // Used for passing a reference to this GameObject across a network.
        private int id;
        // Contains a reference to this GameObject's LoadRegion.
        // Used for knowing when and to whom to send updates about this GameObject.
        private LoadRegion loadRegion;
        // Contains a list of all the Updatable factory methods.
        // Used for figuring out which Updatable to instantiate when a CREATE_FIELD command is issued from the server.
        public static System.Collections.Generic.Dictionary<string, GameObjectFactory> factoryList = new System.Collections.Generic.Dictionary<string, GameObjectFactory>();

        /// <summary>
        /// Constructs the GameObject on the server-side from the GameWorld.
        /// This is the constructor that should be used, unless you are a MULTIPLAYER_CLIENT. 
        /// If you are a MULTIPLAYER_CLIENT, you need an ID before construction, which you receive from the server. Therefore, you use the GameObject(LoadRegion, int) constructor.
        /// If you are a MULTIPLAYER_SERVER or a MULTIPLAYER_SERVERCLIENT this constructor will alert clients that a new GameObject has been created.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this LoadRegion belongs.</param>
        public GameObject(LoadRegion loadRegion) {
            if (GameWorld.GameWorld.status == InteractionEngine.GameWorld.GameWorld.Status.MULTIPLAYER_CLIENT)
                throw new System.Exception("You cannot use the GameObject(LoadRegion) constructor if you are a client in a multiplayer game. You need the ID of the Updatable from the server to prevent synchronization errors.");
            this.id = LoadRegion.nextFieldContainerID++;
            // OMGTODO: I'm not sure about this next line. It wasn't there before, but it seems like it should be. -- Jonathan 4/14/09
            GameWorld.GameWorld.addObject(this);
            this.loadRegion = loadRegion.addObject(this.id);
            // Add the CREATE_OBJECT so that all the clients get the update.
            if (GameWorld.GameWorld.status != GameWorld.GameWorld.Status.SINGLE_PLAYER) {
                System.IO.MemoryStream cache = new System.IO.MemoryStream();
                System.IO.BinaryWriter writer = new System.IO.BinaryWriter(cache);
                writer.Write(GameWorld.GameWorld.CREATE_OBJECT);
                writer.Write(loadRegion.id);
                writer.Write(this.getClassHash());
                writer.Write(this.id);
                loadRegion.writeUpdate(cache.ToArray());
            }
        }

        /// <summary>
        /// Constructs a GameObject and assigns it an ID.
        /// This is the constructor that should be used if and only if you are a MULTIPLAYER_CLIENT.
        /// Furthermore, it is only called by the subclass's private constructor, which in turn is called by the GameObjectFactory method.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        /// <param name="id">This GameObject's ID.</param>
        protected GameObject(LoadRegion loadRegion, int id) {
            this.loadRegion = loadRegion;
            this.id = id;
            this.id = GameWorld.GameWorld.addObject(this);
        }

        /// <summary>
        /// Returns this GameObject's LoadRegion.
        /// </summary>
        /// <returns>This GameObject's LoadRegion.</returns>
        public LoadRegion getLoadRegion() {
            return loadRegion;
        }

        /// <summary>
        /// Contains a string hash of this type.
        /// Used when passing a CREATE_FIELD to specify which type to use.
        /// </summary>
        /// <returns>The string hash.</returns>
        public abstract string getClassHash();

        #region Field Container

        /**
         * FIELD CONTAINER
         *                 
         * Contains a dictionary that links Updatable IDs with Updatables. Note: a "field" is synonymous to an "Updatable".
         * Used for processing CREATE_FIELD, DELETE_FIELD, and UPDATE_FIELD commands from the server. Only used by the client and Updatable (on instantiation).
         * It is only used on client GameWorld. 
         */
        private System.Collections.Generic.Dictionary<int, Constructs.Datatypes.Updatable> fieldHashlist = new System.Collections.Generic.Dictionary<int, Constructs.Datatypes.Updatable>();

        // Contains the lowest available ID for the next Updatable.
        // Used for knowing what ID the Server should assign a new Updatable.
        private static int nextID = 0;

        /// <summary>
        /// Get this FieldContainer's ID.
        /// </summary>
        /// <returns>This FieldContainer's ID.</returns>
        public int getID() {
            return id;
        }

        /// <summary>
        /// This adds a field to the field table. If you are a server, it assigns an ID to the field if one is not present.
        /// </summary>
        /// <param name="field">The field to be added.</param>
        /// <returns>The ID of the field. Irrelevant if you are a client.</returns>
        public int addField(Constructs.Datatypes.Updatable field) {
            if (GameWorld.GameWorld.status == InteractionEngine.GameWorld.GameWorld.Status.MULTIPLAYER_CLIENT){
                if (field.id < 0) throw new System.Exception("Clients should not have to figure out what ID a field has.");
                fieldHashlist.Add(field.id, field);
                return field.id;
            } else {
                fieldHashlist.Add(nextID, field);
                return nextID++;
            }
        }

        /// <summary>
        /// This removes a field from the field table. Read about the field table above.
        /// </summary>
        /// <param name="id">The id of the field to be removed.</param>
        public void removeField(int id) {
            fieldHashlist.Remove(id);
        }

        /// <summary>
        /// This method unboxes and returns the field.
        /// </summary>
        /// <param name="id">The id of the field you want to get.</param>
        /// <returns>The field being returned.</returns>
        public Constructs.Datatypes.Updatable getField(int id) {
            return fieldHashlist[id];
        }

        /// <summary>
        /// Add an Updatable to the list of Updatables requiring network updates.
        /// </summary>
        /// <param name="update">The Updatable requiring an update broadcast.</param>
        public void addUpdate(Datatypes.Updatable update) {
            loadRegion.addUpdate(update);
        }

        /// <summary>
        /// Removes an Updatable from the list of Updatables requiring network updates.
        /// It's a good idea to call this method before deleting an object with this field.
        /// </summary>
        /// <param name="update">The Updatable no longer requiring an update network broadcast.</param>
        public void cancelUpdate(Datatypes.Updatable update) {
            loadRegion.cancelUpdate(update);
        }

        /// <summary>
        /// Write an update directly to the binary cache of network updates waiting to be sent.
        /// </summary>
        /// <param name="update">The byte array containing the update information.</param>
        public void writeUpdate(byte[] update) {
            loadRegion.writeUpdate(update);
        }

        #endregion

        #region Event Hashlist

        /**
         * EVENT HASHLIST
         * 
         * Contains a dictionary that links event strings transfered through the network to methods within the GameWorld.
         * Used for transferring a reference to a method across a network, as transferring delegate references wouldn't work for obvious reasons.
         * It is only used on client GameWorld.
         */
        public System.Collections.Generic.Dictionary<string, EventMethod> eventHashlist = new System.Collections.Generic.Dictionary<string, EventMethod>();

        /// <summary>
        /// Adds an event to the eventHashlist.
        /// </summary>
        /// <param name="hash">The string that represents the event.</param>
        /// <param name="method">The method that is called by the event.</param>
        public void addEvent(string hash, EventMethod method) {
            eventHashlist.Add(hash, method);
        }

        /// <summary>
        /// Removes an event from the eventHashlist.
        /// </summary>
        /// <param name="hash">The string that represents the event.</param>
        public void removeEvent(string hash) {
            eventHashlist.Remove(hash);
        }

        /// <summary>
        /// Gets an event from the eventHashlist.
        /// </summary>
        /// <param name="hash">The string that represents the event.</param>
        /// <returns>The method that is called by the event.</returns>
        public EventMethod getEvent(string hash) {
            return eventHashlist[hash];
        }

        /// <summary>
        /// Get the eventHashlist's enumerator.
        /// </summary>
        /// <returns>The eventHashlist's enumerator.</returns>
        public System.Collections.Generic.Dictionary<string, EventMethod>.Enumerator getEventEnumerator() {
            return eventHashlist.GetEnumerator();
        }

        #endregion

    }

}