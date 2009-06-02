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
| CONSTRUCTS                               |
| * GameObjectFactory       Delegate       |
| * GameObjectable          Interface      |
| * GameObject              Abstract Class |
\*••••••••••••••••••••••••••••••••••••••••*/

namespace InteractionEngine.Constructs {

    /**
     * This delegate holds references to GameObject factory methods.
     */
    public delegate GameObject GameObjectFactory(LoadRegion loadRegion, int id);

    /**
     * This interface is extended by Module types. See documentation for more information on Modules.
     */
    public interface GameObjectable {

        // Contains the ID of this FieldContainer. Must be positive.
        // Used for passing a reference to this FieldContainer across a network.
        int id {
            get;
            set;
        }

        // Contains a string identifying this GameObject subclass.
        // Used as an identifying key to get to the GameObjectFactory method for this GameObject's class.
        string classHash {
            get;
        }

        /// <summary>
        /// This method creates all of this GameObject's fields in constant order. 
        /// Instantiate modules and their fields here too.
        /// Pretty much the constructor. It'll be called every time this object is instantiated.
        /// </summary>
        void construct();

        /// <summary>
        /// Returns this GameObject's LoadRegion.
        /// </summary>
        /// <returns>This GameObject's LoadRegion.</returns>
        LoadRegion getLoadRegion();

        /// <summary>
        /// Move this GameObject from one LoadRegion to another.
        /// </summary>
        /// <param name="newLoadRegion">The new LoadRegion.</param>
        void move(LoadRegion newLoadRegion);

        /// <summary>
        /// Get rid of this GameObject. Sad, I know.
        /// </summary>
        void deconstruct();

        /// <summary>
        /// This adds a field to the field table. If you are a server, it assigns an ID to the field if one is not present.
        /// </summary>
        /// <param name="field">The field to be added.</param>
        void addField(Constructs.Datatypes.Updatable field);

        /// <summary>
        /// This removes a field from the field table. Read about the field table above.
        /// </summary>
        /// <param name="id">The id of the field to be removed.</param>
        void removeField(int id);

        /// <summary>
        /// This method unboxes and returns the field.
        /// </summary>
        /// <param name="id">The id of the field you want to get.</param>
        /// <returns>The field being returned.</returns>
        Constructs.Datatypes.Updatable getField(int id);

        /// <summary>
        /// Returns a dictionary of all field ID's pointing to their realValue (cast an on object).
        /// Used mainly for when a client needs to be sent an already existing LoadRegion and all its already existing GameObjects.
        /// See LoadRegion.sendToClient().
        /// </summary>
        /// <returns>All this object's fields.</returns>
        System.Collections.Generic.Dictionary<int, object> getFieldValues();

        /// <summary>
        /// Adds an event to the eventHashlist.
        /// </summary>
        /// <param name="hash">The string that represents the event.</param>
        /// <param name="method">The method that is called by the event.</param>
        void addEventMethod(string hash, InteractionEngine.EventHandling.EventMethod method);

        /// <summary>
        /// Removes an event from the eventHashlist.
        /// </summary>
        /// <param name="hash">The string that represents the event.</param>
        void removeEventMethod(string hash);

        /// <summary>
        /// Gets an event from the eventHashlist.
        /// </summary>
        /// <param name="hash">The string that represents the event.</param>
        /// <returns>The method that is called by the event.</returns>
        InteractionEngine.EventHandling.EventMethod getEventMethod(string hash);

        /// <summary>
        /// Get the eventHashlist's enumerator.
        /// </summary>
        /// <returns>The eventHashlist's enumerator.</returns>
        System.Collections.Generic.Dictionary<string, EventHandling.EventMethod>.Enumerator getEventMethodEnumerator();

    }

    /**
     * A GameObject is an object within the game.
     * It consists of modules and actions.
     */
    public abstract class GameObject : GameObjectable {

        // Contains the ID of this GameObject. Must be positive.
        // Used for passing a reference to this GameObject across a network.
        private int realID = -1;
        public int id {
            get {
                return realID;
            }
            set {
                if (realID == -1) realID = value;
            }
        }

        #region Constructors and Deconstructors

        /// <summary>
        /// All GameObjects need a parameterless constructor for calling by GameObject.createGameObject() and GameObject.createFromUpdate().
        /// NEVER CALL THIS! This constructor is exclusively for use by the InteractionEngine. If anyone else calls it things will break.
        /// </summary>
        protected GameObject() {
        }

        /// <summary>
        /// This method creates all of this GameObject's fields in constant order. 
        /// Instantiate modules and their fields here too.
        /// Pretty much the constructor. It'll be called every time this object is instantiated.
        /// </summary>
        public abstract void construct();

        #region Client

        // Contains a string identifying this GameObject subclass.
        // Used as an identifying key to get to the GameObjectFactory method for this GameObject's class.
        public abstract string classHash {
            get;
        }

        // Contains a list of all the Updatable factory methods.
        // Used for figuring out which Updatable to instantiate when a CREATE_FIELD command is issued from the server.
        public static System.Collections.Generic.Dictionary<string, GameObjectFactory> factoryList = new System.Collections.Generic.Dictionary<string, GameObjectFactory>();

        /// <summary>
        /// A factory method that creates and returns a new instance of this GameObject. Used by the client when the server requests it to make a new GameObject.
        /// NEVER CALL FROM THE GAMEWORLD!
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        /// <param name="id">This GameObject's ID.</param>
        /// <returns>A new instance of this LoadRegion.</returns>
        public static Type createFromUpdate<Type>(InteractionEngine.Constructs.LoadRegion loadRegion, int id) where Type : GameObject, new() {
            if (InteractionEngine.Engine.status != InteractionEngine.Engine.Status.MULTIPLAYER_CLIENT)
                throw new GameWorldException("You should never call GameObject.createFromUpdate<Type>(LoadRegion, int) from the GameWorld. It is exclusively for internal InteractionEngine use as a factory method for GameObjects.");
            Type gameObject = new Type();
            // Set this GameObject's ID.
            gameObject.id = id;
            // Add this FieldContainer to the GameWorld. 
            InteractionEngine.Engine.addGameObject(gameObject);
            // Add and assign it to the LoadRegion.
            loadRegion.addObject(gameObject.id);
            gameObject.loadRegion = loadRegion; 
            // Setup its fields.
            gameObject.construct();
            // Return it!
            return gameObject;
        }

        #endregion

        #region GameWorld

        /// <summary>
        /// Constructs the GameObject from the GameWorld.
        /// This method isn't an actual constructor because it doesn't return a GameObject if you're a client. That's because we can't make the GameObject until the server updates us with the ID.
        /// If you are a MULTIPLAYER_SERVER or a MULTIPLAYER_SERVERCLIENT this constructor will alert clients that a new GameObject has been created.
        /// If you are a MULTIPLAYER_CLIENT, nothing will happen; you will wait for an update from the server.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        /// <returns>
        /// The constructed GameObject.
        /// If you are a MULTIPLAYER_CLIENT, this will return null; you will have to wait for the server to add your GameObject before you can interact with it.
        /// If you need to do some post-instantiation work with this GameObject, write a new EventMethod with the relevant code. 
        /// Then, make an if(object != null) block in the current EventMethod that calls the new EventMethod 
        /// and an else block that adds the new EventMethod to the static onCreateObject list on CreateObject if it isn't already there.
        /// Make sure that the new EventMethod checks that it is dealing with the right GameObject, because it is going to be called on each new instantiation.
        /// Also, make sure to remove this EventMethod from the lits when you're done with the post-insantiation work.
        /// </returns>
        public static Type createGameObject<Type>(LoadRegion loadRegion) where Type : GameObject, new() {
            // Are we a client? If so, wait for an update from the server who will independently process the EventHandling.Event that called this method.
            if (InteractionEngine.Engine.status == InteractionEngine.Engine.Status.MULTIPLAYER_CLIENT) return null;
            // Otherwise, we are a server. Let's go!
            Type returnObject = new Type();
            // Set its ID.
            InteractionEngine.Engine.assignGameObjectID(returnObject);
            // Assign its LoadRegion.
            returnObject.loadRegion = loadRegion;
            // Add a CreateObject to the LoadRegion's Update Buffer so that all the clients get the update.
            if (InteractionEngine.Engine.status == InteractionEngine.Engine.Status.MULTIPLAYER_SERVER || InteractionEngine.Engine.status == InteractionEngine.Engine.Status.MULTIPLAYER_SERVERCLIENT) {
                returnObject.loadRegion.addUpdate(new InteractionEngine.Networking.CreateObject(
                    returnObject.loadRegion.id,
                    returnObject.id,
                    returnObject.classHash,
                    new System.Collections.Generic.Dictionary<int, object>()
                ));
            }
            // Setup its fields.
            returnObject.construct();
            // Add it the GameObject Hashlist. Do this at the end to avoid synchronization errors!
            InteractionEngine.Engine.addGameObject(returnObject);
            // Add it to the LoadRegion.
            loadRegion.addObject(returnObject.id);
            return returnObject;
        }

        /// <summary>
        /// Get rid of this GameObject. Sad, I know.
        /// </summary>
        public void deconstruct() {
            // Are we a client? If so, wait for an update from the server who will independently process the EventHandling.Event that called this method.
            if (InteractionEngine.Engine.status == InteractionEngine.Engine.Status.MULTIPLAYER_CLIENT) return;
            if (Engine.status == Engine.Status.MULTIPLAYER_SERVER || Engine.status == Engine.Status.MULTIPLAYER_SERVERCLIENT) this.loadRegion.addUpdate(new Networking.DeleteObject(this.id));
            internalDeconstruct();
        }

        /// <summary>
        /// Gets rid of this GameObject. Called by the above method as well as DeleteObject.executeUpdate().
        /// </summary>
        internal void internalDeconstruct() {
            InteractionEngine.Engine.removeGameObject(this.id);
            this.loadRegion.removeObject(this.id);
        }

        #endregion

        #endregion

        #region LoadRegion

        // Contains a reference to this GameObject's LoadRegion.
        // Used for knowing when and to whom to send updates about this GameObject.
        private LoadRegion loadRegion;

        /// <summary>
        /// Returns this GameObject's LoadRegion.
        /// </summary>
        /// <returns>This GameObject's LoadRegion.</returns>
        public LoadRegion getLoadRegion() {
            return loadRegion;
        }

        /// <summary>
        /// Move this GameObject from one LoadRegion to another.
        /// </summary>
        /// <param name="newLoadRegion">The new LoadRegion.</param>
        public void move(LoadRegion newLoadRegion) {
            // Are we a client? If so, wait for an update from the server who will independently process the EventHandling.Event that called this method.
            if (InteractionEngine.Engine.status == InteractionEngine.Engine.Status.MULTIPLAYER_CLIENT) return;
            if (Engine.status == Engine.Status.MULTIPLAYER_SERVER || Engine.status == Engine.Status.MULTIPLAYER_SERVERCLIENT) this.loadRegion.addUpdate(new Networking.MoveObject(this.id, newLoadRegion.id));
            this.internalMove(newLoadRegion);
        }

        /// <summary>
        /// Moves this GameObject from one LoadRegion to another. Called by the above method as well as MoveObject.executeUpdate().
        /// </summary>
        /// <param name="newLoadRegion">The new LoadRegion.</param>
        internal void internalMove(LoadRegion newLoadRegion) {
            this.loadRegion.removeObject(this.id);
            newLoadRegion.addObject(this.id);
            this.loadRegion = newLoadRegion;
        }

        #endregion

        #region Field Container

        /**
         * FIELD CONTAINER
         *                 
         * Contains a dictionary that links Updatable IDs with Updatables. Note: a "field" is synonymous to an "Updatable".
         * Used for processing UPDATE_FIELD commands from the server. Only used by the client and Updatable (on instantiation).
         * It is only used on client GameWorld. 
         */
        private System.Collections.Generic.Dictionary<int, Constructs.Datatypes.Updatable> fieldHashlist = new System.Collections.Generic.Dictionary<int, Constructs.Datatypes.Updatable>();

        // Contains the lowest available ID for the next Updatable.
        // Used for knowing what ID the Server should assign a new Updatable.
        private int nextID = 0;

        /// <summary>
        /// This adds a field to the field table. If you are a server, it assigns an ID to the field if one is not present.
        /// </summary>
        /// <param name="field">The field to be added.</param>
        public void addField(Constructs.Datatypes.Updatable field) {
            field.id = nextID++;
            fieldHashlist.Add(field.id, field);
        }

        /// <summary>
        /// This removes a field from the field table. Read about the field table above.
        /// </summary>
        /// <param name="id">The id of the field to be removed.</param>
        public void removeField(int id) {
            if (fieldHashlist.ContainsKey(id)) fieldHashlist.Remove(id);
        }

        /// <summary>
        /// This method unboxes and returns the field.
        /// </summary>
        /// <param name="id">The id of the field you want to get.</param>
        /// <returns>The field being returned.</returns>
        public Constructs.Datatypes.Updatable getField(int id) {
            if (fieldHashlist.ContainsKey(id)) return fieldHashlist[id];
            else return null;
        }

        /// <summary>
        /// Returns a dictionary of all field ID's pointing to their realValue (cast an on object).
        /// Used mainly for when a client needs to be sent an already existing LoadRegion and all its already existing GameObjects.
        /// See LoadRegion.sendToClient().
        /// </summary>
        /// <returns>All this object's fields.</returns>
        public System.Collections.Generic.Dictionary<int, object> getFieldValues() {
            System.Collections.Generic.Dictionary<int, object> returnList = new System.Collections.Generic.Dictionary<int, object>();
            foreach (System.Collections.Generic.KeyValuePair<int, Constructs.Datatypes.Updatable> pair in fieldHashlist)
                returnList.Add(pair.Key, pair.Value.getValue());
            return returnList;
        }

        #endregion

        #region EventMethod Hashlist

        /**
         * EVENTMETHOD HASHLIST
         * 
         * Contains a dictionary that links event strings transfered through the network to methods within the GameWorld.
         * Used for transferring a reference to a method across a network, as transferring delegate references wouldn't work for obvious reasons.
         * It is only used on client GameWorld.
         */
        [System.NonSerialized]
        public System.Collections.Generic.Dictionary<string, InteractionEngine.EventHandling.EventMethod> eventHashlist = new System.Collections.Generic.Dictionary<string, InteractionEngine.EventHandling.EventMethod>();

        /// <summary>
        /// Adds an event to the eventHashlist.
        /// </summary>
        /// <param name="hash">The string that represents the event.</param>
        /// <param name="method">The method that is called by the event.</param>
        public void addEventMethod(string hash, InteractionEngine.EventHandling.EventMethod method) {
            eventHashlist.Add(hash, method);
        }

        /// <summary>
        /// Removes an event from the eventHashlist.
        /// </summary>
        /// <param name="hash">The string that represents the event.</param>
        public void removeEventMethod(string hash) {
            eventHashlist.Remove(hash);
        }

        /// <summary>
        /// Gets an event from the eventHashlist.
        /// </summary>
        /// <param name="hash">The string that represents the event.</param>
        /// <returns>The method that is called by the event.</returns>
        public InteractionEngine.EventHandling.EventMethod getEventMethod(string hash) {
            return eventHashlist[hash];
        }

        /// <summary>
        /// Get the eventHashlist's enumerator.
        /// </summary>
        /// <returns>The eventHashlist's enumerator.</returns>
        public System.Collections.Generic.Dictionary<string, EventHandling.EventMethod>.Enumerator getEventMethodEnumerator() {
            return eventHashlist.GetEnumerator();
        }

        #endregion

    }

}