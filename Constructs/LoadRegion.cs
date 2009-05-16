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
| * LoadRegion                       Class |
\*••••••••••••••••••••••••••••••••••••••••*/

namespace InteractionEngine.Constructs {

    /**
     * Each LoadRegion contains GameObjects, which get loaded all at once. A LoadRegion is a unit of trans-network updating.
     * It itself is a GameObject, but only because it needs to act as an Updatable container. Also, it needs to use the synchronized instantiation 
     */
    public class LoadRegion : FieldContainer {

        // Contains the lowest available ID for a LoadRegion.
        // Used for giving every LoadRegion a unique ID.
        internal static int nextFieldContainerID = 0;
        // Contains the lowest available ID for an Updatable.
        // Used for giving every Updatable a unique ID.
        internal static int nextFieldID = 0;
        // Contains the ID of this LoadRegion. Must be positive. 
        // These are the same across the networking ONLY because order of LoadRegion construction must be guaranteed. MAKE SURE ORDER IS GUARANTEED!
        // Used for passing a reference to this LoadRegion across a network.
        public readonly int id;

        /// <summary>
        /// Construct the LoadRegion.
        /// </summary>
        public LoadRegion() : base() {
            if (GameWorld.GameWorld.status == InteractionEngine.GameWorld.GameWorld.Status.MULTIPLAYER_CLIENT)
                throw new System.Exception("Oops, somebody screwed up. You can't instantiate LoadRegions without specified IDs from a client.");
            this.id = nextFieldContainerID;
            nextFieldContainerID++;
            objects = new Datatypes.UpdatableList(this);
        }

        public LoadRegion(int id) {
            if (GameWorld.GameWorld.status != InteractionEngine.GameWorld.GameWorld.Status.MULTIPLAYER_CLIENT)
                throw new System.Exception("Oops, somebody screwed up. You can't instantiate LoadRegions with specified IDs from a server.");
            this.id = id;
            objects = new Datatypes.UpdatableList(this);
        }

        #region Object List

        /**
         * OBJECT LIST
         * 
         * Every LoadRegion has GameObjects located in it; this is a list of GameObjects.
         */
        //TODO: changing from one loadregion to another
        private readonly Datatypes.UpdatableList objects;

        /// <summary>
        /// This method is triggered when a GameObject enters this LoadRegion.
        /// </summary>
        /// <param name="objectId">The id of the GameObject that enters the room.</param>
        /// <returns>A reference to this LoadRegion, so that the GameObject can assign it to its field.</returns>
        public LoadRegion addObject(int objectId) {
            this.objects.add(new Datatypes.UpdatableInteger(this, objectId));
            return this;
        }

        /// <summary>
        /// This method is triggered when a GameObject leaves this LoadRegion.
        /// Does not set the GameObject's LoadRegion field to null.
        /// </summary>
        /// <param name="objectId">The id of the GameObject that leaves the room.</param>
        public void removeObject(int objectId) {
            this.objects.remove(objectId);
        }

        /// <summary>
        /// This method confirms whether or not a GameObject is in this LoadRegion.
        /// </summary>
        /// <param name="objectId">The id of the GameObject we are checking for.</param>
        /// <returns>True if the list contains the value, false otherwise.</returns>
        public bool containsObject(int objectId) {
            return this.objects.contains(objectId);
        }

        /// <summary>
        /// This method returns the GameObject from this LoadRegion.
        /// </summary>
        /// <param name="id">The id of the GameObject you want to retrieve.</param>
        /// <returns>The GameObject being returned.</returns>
        public Constructs.GameObjectable getObject(int id) {
            return GameWorld.GameWorld.getObject(((Datatypes.UpdatableInteger)this.objects.get(id)).value);
        }

        /// <summary>
        /// This method returns the number of GameObjects in this LoadRegions.
        /// </summary>
        /// <returns>The number of GameObjects in this LoadRegion.</returns>
        public int getObjectCount() {
            return this.objects.getLength();
        }

        #endregion

        #region Field Container

        /**
         * FIELD CONTAINER
         *                 
         * Contains a dictionary that links Updatable IDs with Updatables. Note: a "field" is synonymous to an "Updatable".
         * Used for processing CREATE_FIELD, DELETE_FIELD, and UPDATE_FIELD commands from the server. Only used by the client and Updatable (on instantiation).
         * It is only used on client GameWorld. 
         */
        private System.Collections.Generic.Dictionary<int, Constructs.Datatypes.Updatable> fieldHashlist = new System.Collections.Generic.Dictionary<int, Constructs.Datatypes.Updatable>();

        /// <summary>
        /// Get this FieldContainer's ID.
        /// </summary>
        /// <returns>This FieldContainer's ID.</returns>
        public int getID() {
            return id;
        }

        /// <summary>
        /// This adds a field to the field table. Read about the field table above.
        /// </summary>
        /// <param name="field">The field to be added.</param>
        /// <returns>The id (the component of one's mind that contains unconscious, primitive desires).</returns>
        public int addField(Constructs.Datatypes.Updatable field) {
            int id = nextFieldID++;
            fieldHashlist.Add(id, field);
            return id;
        }

        /// <summary>
        /// This method unboxes and returns the field.
        /// </summary>
        /// <param name="id">The id of the field you want to get.</param>
        /// <returns>The field being returned.</returns>
        public Constructs.Datatypes.Updatable getField(int id) {
            return fieldHashlist[id];
        }

        #endregion

        #region Update Cache

        /**
         * UPDATE CACHE
         * 
         * Every LoadRegion handles cached updates to send to users that have it loaded.
         */
        
        // Contains a stream of bytes to be sent across the network to a client or multiple clients.
        // Used for caching the message before sending it.
        private Microsoft.Xna.Framework.Net.PacketWriter cache = new Microsoft.Xna.Framework.Net.PacketWriter();

        // Contains a list of all the Updatables that have been modified this loop.
        // Used so that we can make sure we have no duplicate UPDATE_FIELD messages.
        private System.Collections.Generic.List<Datatypes.Updatable> updateList = new System.Collections.Generic.List<Datatypes.Updatable>();

        /// <summary>
        /// Add an Updatable to the list of Updatables requiring network updates.
        /// </summary>
        /// <param name="update">The Updatable requiring an update broadcast.</param>
        public void addUpdate(Datatypes.Updatable update) {
            updateList.Add(update);
        }

        /// <summary>
        /// Removes an Updatable from the list of Updatables requiring network updates.
        /// It's a good idea to call this method before deleting an object with this field.
        /// </summary>
        /// <param name="update">The Updatable no longer requiring an update network broadcast.</param>
        public void cancelUpdate(Datatypes.Updatable update) {
            updateList.Remove(update);
        }

        /// <summary>
        /// Write an update directly to the binary cache of network updates waiting to be sent.
        /// </summary>
        /// <param name="update">The byte array containing the update information.</param>
        public void writeUpdate(byte[] update) {
            cache.Write(update);
        }

        /// <summary>
        /// Reset the PacketWriter cache.
        /// </summary>
        public void resetCache() {
            cache = new Microsoft.Xna.Framework.Net.PacketWriter();
        }

        /// <summary>
        /// Send the output from the cache to a client.
        /// </summary>
        /// <param name="gamer">The NetworkGamer we are sending an update to.</param>
        /// <see>XNA documentation for more information on NetworkGamer.</see>
        public void sendUpdate(Microsoft.Xna.Framework.Net.NetworkGamer gamer) {
            foreach (Datatypes.Updatable updating in updateList) {
                updating.writeUpdate(cache);
            }
            updateList.Clear();
            GameWorld.GameWorld.gamer.SendData(cache, Microsoft.Xna.Framework.Net.SendDataOptions.Reliable, gamer);
        }

        #endregion

        // move from one LoadRegion to another!

        // step 1: get a request for LR #3

    }

}