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
        public int id {
            get;
            set;
        }

        /// <summary>
        /// This method returns the class hash of this GameObject. We need this so we have an identifying key to get to the GameObjectFactory method for this GameObject's class.
        /// </summary>
        /// <returns>The class hash of this GameObject.</returns>
        string getClassHash();

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
        static GameObject createGameObject<Type>(LoadRegion loadRegion);

        /// <summary>
        /// Returns this GameObject's LoadRegion.
        /// </summary>
        /// <returns>This GameObject's LoadRegion.</returns>
        LoadRegion getLoadRegion();
        
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
        System.Collections.Generic.Dictionary<string, EventMethod>.Enumerator getEventMethodEnumerator();
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
                if (realID == -1) id = value;
            }
        }

        #region Constructors and Deconstructors

        // Contains a list of all the Updatable factory methods.
        // Used for figuring out which Updatable to instantiate when a CREATE_FIELD command is issued from the server.
        public static System.Collections.Generic.Dictionary<string, GameObjectFactory> factoryList = new System.Collections.Generic.Dictionary<string, GameObjectFactory>();

        /// <summary>
        /// This method returns the class hash of this GameObject. We need this so we have an identifying key to get to the GameObjectFactory method for this GameObject's class.
        /// </summary>
        /// <returns>The class hash of this GameObject.</returns>
        public abstract string getClassHash();

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
        public static GameObject createGameObject<Type>(LoadRegion loadRegion) where Type : GameObject, new() {
            // Are we a client? If so, wait for an update from the server who will independently process the Event that called this method.
            if (GameWorld.GameWorld.status == InteractionEngine.GameWorld.GameWorld.Status.MULTIPLAYER_CLIENT) return null;
            // Otherwise, we are a server. Let's go!
            Type returnObject = new Type();
            // Add this FieldContainer to the GameWorld. This will set its ID.
            GameWorld.GameWorld.addGameObject(returnObject);
            // Add and assign it to the LoadRegion.
            loadRegion.addObject(returnObject.id);
            returnObject.loadRegion = loadRegion;
            // Add a CreateObject to the LoadRegion's Update Buffer so that all the clients get the update.
            if (GameWorld.GameWorld.status == GameWorld.GameWorld.Status.MULTIPLAYER_SERVER || GameWorld.GameWorld.status == GameWorld.GameWorld.Status.MULTIPLAYER_SERVERCLIENT) {
                returnObject.loadRegion.addUpdate(new InteractionEngine.Networking.CreateObject(
                    returnObject.loadRegion.id,
                    returnObject.id,
                    returnObject.getClassHash(),
                    new System.Collections.Generic.Dictionary<int, object>()
                ));
            }
            return returnObject;
        }

        /// <summary>
        /// This method is meant to be called by a subclass constructor, which in turn was called by a factory method on that subclass.
        /// It instantiates a GameObject from the information that was sent to a MULTIPLAYER_CLIENT by a CREATE_OBJECT packet.
        /// </summary>
        /// <param name="loadRegion"></param>
        /// <param name="id"></param>
        protected GameObject(LoadRegion loadRegion, int id) {
            // This constructor should only ever be called by a subclass's constructor on a MULTIPLAYER_CLIENT, which in turn was called by a factory method on that subclass.
            if (GameWorld.GameWorld.status != InteractionEngine.GameWorld.GameWorld.Status.MULTIPLAYER_CLIENT)
                throw new System.Exception("The game developer screwed up. They shouldn't be calling this method, except on a GameObject's factory constructor on a MULTIPLAYER_CLIENT.");
            // Set this GameObject's ID.
            this.id = id;
            // Add this FieldContainer to the GameWorld. This will NOT set its ID.
            GameWorld.GameWorld.addGameObject(this);
            // Add and assign it to the LoadRegion.
            loadRegion.addObject(this.id);
            this.loadRegion = loadRegion;
        }

        /// <summary>
        /// Get rid of this GameObject. Sad, I know.
        /// </summary>
        public void deconstruct() {
            
            GameWorld.GameWorld.removeGameObject(this.id);
            this.loadRegion.removeObject(this.id);
            //if (
        }

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
        private static int nextID = 0;

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

        #endregion

        #region EventMethod Hashlist

        /**
         * EVENTMETHOD HASHLIST
         * 
         * Contains a dictionary that links event strings transfered through the network to methods within the GameWorld.
         * Used for transferring a reference to a method across a network, as transferring delegate references wouldn't work for obvious reasons.
         * It is only used on client GameWorld.
         */
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
        public System.Collections.Generic.Dictionary<string, EventMethod>.Enumerator getEventMethodEnumerator() {
            return eventHashlist.GetEnumerator();
        }

        #endregion

    }

}