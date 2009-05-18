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

        /// <summary>
        /// Construct the LoadRegion from the GameWorld on the server-side. 
        /// This method should only be called by EventMethods that are run exclusively on a server! Don't screw this up.
        /// EventMethods triggered by the server are exclusive to Client.onJoin and Client.onQuit. So you can only create LoadRegions before networking begins, or when a client joins or quits.
        /// </summary>
        public LoadRegion() : base() {
            if (GameWorld.GameWorld.status == InteractionEngine.GameWorld.GameWorld.Status.MULTIPLAYER_CLIENT)
                throw new System.Exception("The game developer screwed up. They shouldn't be calling a LoadRegion constructor on the Client-side without an ID to assign it.");
            GameWorld.GameWorld.addFieldContainer(this);
        }

        /// <summary>
        /// Construct the LoadRegion on the client-side.
        /// This method should only be called by the client InteractionEngine when it receives a CREATE_REGION transfer code.
        /// </summary>
        /// <param name="id">The ID to assign this LoadRegion, provided by the server.</param>
        internal LoadRegion(int id) : base() {
            if (GameWorld.GameWorld.status != GameWorld.GameWorld.Status.MULTIPLAYER_CLIENT)
                throw new System.Exception("Something is wrong with the InteractionEngine. This is probably our bad. Sorry.");
            this.setID(id);
            GameWorld.GameWorld.addFieldContainer(this);
        }

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

        #region Field Container

        /**
         * FIELD CONTAINER
         *                 
         * Contains a dictionary that links Updatable IDs with Updatables. Note: a "field" is synonymous to an "Updatable".
         * Used for processing CREATE_FIELD, DELETE_FIELD, and UPDATE_FIELD commands from the server. Only used by the client and Updatable (on instantiation).
         * It is only used on client GameWorld. 
         */
        private System.Collections.Generic.Dictionary<int, Constructs.Datatypes.Updatable> fieldHashlist = new System.Collections.Generic.Dictionary<int, Constructs.Datatypes.Updatable>();

        // Contains the ID of this FieldContainer. Must be positive.
        // Used for passing a reference to this FieldContainer across a network.
        private int id = -1;
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
        /// Set this FieldContainer's ID. Only works once.
        /// </summary>
        /// <param name="newId">The ID to set.</param>
        public void setID(int newId) {
            if (id == -1) id = newId;
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

    }

}