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
        /// </summary>
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
    public abstract class GameObject : FieldContainer {

        // Contains a reference to this GameObject's LoadRegion.
        // Used for knowing when and to whom to send updates about this GameObject.
        private LoadRegion loadRegion;
        // Contains a list of all the Updatable factory methods.
        // Used for figuring out which Updatable to instantiate when a CREATE_FIELD command is issued from the server.
        public static System.Collections.Generic.Dictionary<string, GameObjectFactory> factoryList = new System.Collections.Generic.Dictionary<string, GameObjectFactory>();

        /// <summary>
        /// Constructs the GameObject from the GameWorld on the server-side.
        /// This method is protected, as we don't know which GameObject to instantiate at this point. Commented pseudocode is inside to give an example of what to do in the child "constructor".
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
        /// and an else block that adds the new EventMethod to the Server's onCreateObject if it isn't already there.
        /// Make sure that the new EventMethod checks that it is dealing with the right GameObject, because it is going to be called on each new instantiation.
        /// Also, make sure to remove this EventMethod from the Server's onCreateObject when you're done with the post-insantiation work.
        /// </returns>
        public static GameObject instantiateGameObject<Type>(LoadRegion loadRegion) where Type : GameObject, new() {
            // Are we a client? If so, wait for an update from the server who will independently process the Event that called this method.
            if (GameWorld.GameWorld.status == InteractionEngine.GameWorld.GameWorld.Status.MULTIPLAYER_CLIENT) return null;
            // Otherwise, we are a server. Let's go!
            Type returnObject = new Type();
            // Add this FieldContainer to the GameWorld. This will set its ID.
            GameWorld.GameWorld.addFieldContainer(returnObject);
            // Add and assign it to the LoadRegion.
            loadRegion.addObject(returnObject.id);
            returnObject.loadRegion = loadRegion;
            // Add the CREATE_OBJECT so that all the clients get the update.
            if (GameWorld.GameWorld.status == GameWorld.GameWorld.Status.MULTIPLAYER_SERVER || GameWorld.GameWorld.status == GameWorld.GameWorld.Status.MULTIPLAYER_SERVERCLIENT) {
                /*System.IO.MemoryStream cache = new System.IO.MemoryStream();
                System.IO.BinaryWriter writer = new System.IO.BinaryWriter(cache);
                writer.Write(GameWorld.GameWorld.CREATE_OBJECT);
                writer.Write(loadRegion.id);
                writer.Write(this.getClassHash());
                writer.Write(this.id);
                loadRegion.writeUpdate(cache.ToArray());*/
                
                // the networking stuff will go here.
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
            GameWorld.GameWorld.addFieldContainer(this);
            // Add and assign it to the LoadRegion.
            loadRegion.addObject(this.id);
            this.loadRegion = loadRegion;
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

        /// <summary>
        /// Add an Updatable to the list of Updatables requiring network updates.
        /// </summary>
        /// <param name="update">The Updatable requiring an update broadcast.</param>
        public override void addUpdate(Datatypes.Updatable update) {
            loadRegion.addUpdate(update);
        }

        /// <summary>
        /// Removes an Updatable from the list of Updatables requiring network updates.
        /// It's a good idea to call this method before deleting an object with this field.
        /// </summary>
        /// <param name="update">The Updatable no longer requiring an update network broadcast.</param>
        public override void cancelUpdate(Datatypes.Updatable update) {
            loadRegion.cancelUpdate(update);
        }

        /// <summary>
        /// Write an update directly to the binary cache of network updates waiting to be sent.
        /// </summary>
        /// <param name="update">The byte array containing the update information.</param>
        public override void writeUpdate(byte[] update) {
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
        public System.Collections.Generic.Dictionary<string, InteractionEngine.EventHandling.EventMethod> eventHashlist = new System.Collections.Generic.Dictionary<string, InteractionEngine.EventHandling.EventMethod>();

        /// <summary>
        /// Adds an event to the eventHashlist.
        /// </summary>
        /// <param name="hash">The string that represents the event.</param>
        /// <param name="method">The method that is called by the event.</param>
        public void addEvent(string hash, InteractionEngine.EventHandling.EventMethod method) {
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