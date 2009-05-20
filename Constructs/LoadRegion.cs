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
| * LoadRegion                       Class |
\*••••••••••••••••••••••••••••••••••••••••*/

namespace InteractionEngine.Constructs {

    /**
     * Each LoadRegion contains GameObjects, which get loaded all at once. A LoadRegion is a unit of trans-network updating.
     * It itself is a GameObject, but only because it needs to act as an Updatable container. Also, it needs to use the synchronized instantiation 
     */
    public class LoadRegion : FieldContainer {

        // Contains the ID of this LoadRegion. Must be positive.
        // Used for passing a reference to this LoadRegion across a network.
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

        /// <summary>
        /// Constructs the LoadRegion from the GameWorld.
        /// This method isn't an actual constructor because it doesn't return a LoadRegion if you're a client. That's because we can't make the LoadRegion until the server updates us with the ID.
        /// You probably shouldn't run this method often on a networked game, because without specifying clients that will receive this LoadRegion it will sort of be orphaned. Try createLoadRegion(List<![CDATA[<Client>]]>).
        /// If you are a MULTIPLAYER_CLIENT, nothing will happen as a result of this method. You will never know this LoadRegion was ever instantiated.
        /// </summary>
        public static LoadRegion createLoadRegion() {
            // Are we a client? If so, wait for an update from the server who will independently process the Event that called this method.
            if (GameWorld.GameWorld.status == InteractionEngine.GameWorld.GameWorld.Status.MULTIPLAYER_CLIENT)
                return null;
            // Otherwise, we are a server. Let's go!
            LoadRegion returnRegion = new LoadRegion();
            // Add this FieldContainer to the GameWorld. This will set its ID.
            GameWorld.GameWorld.addLoadRegion(returnRegion);
            return returnRegion;
        }

        /// <summary>
        /// Constructs the LoadRegion from the GameWorld.
        /// This method isn't an actual constructor because it doesn't return a LoadRegion if you're a client. That's because we can't make the LoadRegion until the server updates us with the ID.
        /// If you are a MULTIPLAYER_SERVER or a MULTIPLAYER_SERVERCLIENT this constructor will alert clients that a new LoadRegion has been created.
        /// If you are a MULTIPLAYER_CLIENT, nothing will happen; you will wait for an update from the server (assuming you were listed in the parameter).
        /// </summary>
        /// <param name="clients">
        /// This list contains all the Clients who will be needing this LoadRegion once it's created.
        /// On a client, this list might have one item that's just null (assuming the game developer adds the EventMethod's second parameter to the list), but that's okay because we don't do anything on a client.
        /// </param>
        /// <returns>
        /// The constructed LoadRegion.
        /// If you are a MULTIPLAYER_CLIENT, this will return null; you will have to wait for the server to add your LoadRegion before you can interact with it.
        /// If you need to do some post-instantiation work with this LoadRegion, write a new EventMethod with the relevant code. 
        /// Then, make an if(region != null) block in the current EventMethod that calls the new EventMethod 
        /// and an else block that adds the new EventMethod to the static onCreateRegion list on CreateRegion if it isn't already there.
        /// Make sure that the new EventMethod checks that it is dealing with the right LoadRegion, because it is going to be called on each new instantiation.
        /// Also, make sure to remove this EventMethod from the list when you're done with the post-insantiation work.
        /// Note: when the onCreateRegion list is triggered, the LoadRegion will be empty of GameObjects. If you want to deal with it once the client has created all relevant GameObjects,
        /// you will have to use onCreateObject instead. You'll probably want to count the GameObjects created in the LoadRegion until you have all you need so you know the LoadRegion is ready to be played with.
        /// </returns></returns>
        public static LoadRegion createLoadRegion(System.Collections.Generic.List<Networking.Client> clients) {
            // Are we a client? If so, wait for an update from the server who will independently process the Event that called this method.
            if (GameWorld.GameWorld.status == InteractionEngine.GameWorld.GameWorld.Status.MULTIPLAYER_CLIENT)
                return null;
            // Otherwise, we are a server. Let's go!
            LoadRegion returnRegion = new LoadRegion();
            // Add this FieldContainer to the GameWorld. This will set its ID.
            GameWorld.GameWorld.addLoadRegion(returnRegion);
            foreach (Networking.Client client in clients)
                client.sendUpdate(new Networking.CreateRegion(returnRegion.id));
            return returnRegion;
        }

        /// <summary>
        /// Empty constructor for the factory methods above.
        /// </summary>
        private LoadRegion() {
        }

        /// <summary>
        /// Construct the LoadRegion on the client-side.
        /// This method should only be called by the client InteractionEngine when it receives a CREATE_REGION transfer code.
        /// </summary>
        /// <param name="id">The ID to assign this LoadRegion, provided by the server.</param>
        internal LoadRegion(int id) : base() {
            if (GameWorld.GameWorld.status != GameWorld.GameWorld.Status.MULTIPLAYER_CLIENT)
                throw new System.Exception("Something is wrong with the InteractionEngine. This is probably our bad. Sorry.");
            this.id = id;
            GameWorld.GameWorld.addLoadRegion(this);
        }

        /// <summary>
        /// Get rid of this LoadRegion. Sad, I know.
        /// </summary>
        public void deconstruct() {
            // Are we a client? If so, wait for an update from the server who will independently process the Event that called this method.
            if (GameWorld.GameWorld.status == InteractionEngine.GameWorld.GameWorld.Status.MULTIPLAYER_CLIENT) return;
            GameWorld.GameWorld.removeLoadRegion(this.id);
            foreach (int gameObjectID in objects) GameWorld.GameWorld.getGameObject(gameObjectID).deconstruct();
        }

        #endregion

        #region Object List

        /**
         * OBJECT LIST
         * 
         * Every LoadRegion has GameObjects located in it; this is a list of GameObjects.
         */
        private readonly System.Collections.Generic.List<int> objects = new System.Collections.Generic.List<int>();

        /// <summary>
        /// This method is triggered when a GameObject enters this LoadRegion.
        /// </summary>
        /// <param name="objectId">The id of the GameObject that enters the room.</param>
        /// <returns>A reference to this LoadRegion, so that the GameObject can assign it to its field.</returns>
        internal void addObject(int objectId) {
            this.objects.Add(objectId);
        }

        /// <summary>
        /// This method is triggered when a GameObject leaves this LoadRegion.
        /// Does not set the GameObject's LoadRegion field to null.
        /// </summary>
        /// <param name="objectId">The id of the GameObject that leaves the room.</param>
        internal void removeObject(int objectId) {
            this.objects.Remove(objectId);
        }

        /// <summary>
        /// This method confirms whether or not a GameObject is in this LoadRegion.
        /// </summary>
        /// <param name="objectId">The id of the GameObject we are checking for.</param>
        /// <returns>True if the list contains the value, false otherwise.</returns>
        public bool containsObject(int objectId) {
            return this.objects.Contains(objectId);
        }

        /// <summary>
        /// This method returns the number of GameObjects in this LoadRegion.
        /// </summary>
        /// <returns>The number of GameObjects in this LoadRegion.</returns>
        public int getObjectCount() {
            return this.objects.Count;
        }

        #endregion

        #region Update Buffer

        /**
         * UPDATE BUFFER
         * 
         * Every LoadRegion handles cached updates to send to users that have it loaded.
         */
    
        // Contains a list of all the Updates currently waiting to be sent to clients in this LoadRegion.
        // Used so that we send Updates only to clients who need them.
        private System.Collections.Generic.List<Networking.Update> updateBuffer = new System.Collections.Generic.List<InteractionEngine.Networking.Update>();
        // Contains a list of all the Updatables that have been modified this loop.
        // Used so that we can make sure we have no duplicate UPDATE_FIELD messages.
        private System.Collections.Generic.List<Datatypes.Updatable> updateFieldBuffer = new System.Collections.Generic.List<Datatypes.Updatable>();

        /// <summary>
        /// Add an Update to the Update Buffer.
        /// </summary>
        /// <param name="update">The Update to be added.</param>
        public void addUpdate(Networking.Update update) {
            updateBuffer.Add(update);
        }

        /// <summary>
        /// Add an Updatable to the UpdateField Buffer.
        /// </summary>
        /// <param name="field">The Updatable to be added.</param>
        public void registerUpdate(Datatypes.Updatable field) {
            updateFieldBuffer.Add(field);
        }

        /// <summary>
        /// Removes an Updatable from the list of Updatables requiring network updates.
        /// It's a good idea to call this method before deleting an object with this field.
        /// </summary>
        /// <param name="update">The Updatable no longer requiring an UPDATE_FIELD transfer code.</param>
        public void cancelUpdate(Datatypes.Updatable update) {
            updateFieldBuffer.Remove(update);
        }

        /// <summary>
        /// Reset the Update Buffer.
        /// </summary>
        public void resetBuffer() {
            updateBuffer.Clear();
        }

        /// <summary>
        /// Get the current list of Updates in this LoadRegion's Update Buffer.
        /// </summary>
        /// <returns>The list of Updates.</returns>
        public System.Collections.Generic.List<Networking.Update> getUpdates() {
            foreach (Datatypes.Updatable updating in updateFieldBuffer)
                updateBuffer.Add(new Networking.UpdateField(updating.fieldContainer.id, updating.id, updating.getValue()));
            updateFieldBuffer.Clear();
            return updateBuffer;
        }

        #endregion

    }

}