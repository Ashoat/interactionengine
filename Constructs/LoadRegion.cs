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
| CONSTRUCTS                               |
| * LoadRegion                       Class |
\*••••••••••••••••••••••••••••••••••••••••*/

namespace InteractionEngine.Constructs {

    /**
     * Each LoadRegion contains GameObjects, which get loaded all at once. A LoadRegion is a unit of trans-network updating.
     * It itself is a GameObject, but only because it needs to act as an Updatable container. Also, it needs to use the synchronized instantiation 
     */
    public class LoadRegion {

        // Contains the ID of this LoadRegion. Must be positive.
        // Used for passing a reference to this LoadRegion across a network.
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

        // Contains the number of GameObjects this LoadRegion should contain after loading.
        // Used so we know when we're done transferring it.
        public int numberOfGameObjects = 0;

        /// <summary>
        /// Constructs the LoadRegion from the GameWorld.
        /// This method isn't an actual constructor because it doesn't return a LoadRegion if you're a client. That's because we can't make the LoadRegion until the server updates us with the ID.
        /// You probably shouldn't run this method often on a networked game, because without specifying clients that will receive this LoadRegion it will sort of be orphaned. Try createLoadRegion(List<![CDATA[<Client>]]>).
        /// If you are a MULTIPLAYER_CLIENT, nothing will happen as a result of this method. You will never know this LoadRegion was ever instantiated.
        /// </summary>
        public static LoadRegion createLoadRegion() {
            // Are we a client? If so, wait for an update from the server who will independently process the EventHandling.Event that called this method.
            if (InteractionEngine.Engine.status == InteractionEngine.Engine.Status.MULTIPLAYER_CLIENT)
                return null;
            // Otherwise, we are a server. Let's go!
            LoadRegion returnRegion = new LoadRegion();
            // Add this FieldContainer to the GameWorld. This will set its ID.
            InteractionEngine.Engine.addLoadRegion(returnRegion);
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
            // Are we a client? If so, wait for an update from the server who will independently process the EventHandling.Event that called this method.
            if (InteractionEngine.Engine.status == InteractionEngine.Engine.Status.MULTIPLAYER_CLIENT)
                return null;
            // Otherwise, we are a server. Let's go!
            LoadRegion returnRegion = new LoadRegion();
            // Add this FieldContainer to the GameWorld. This will set its ID.
            InteractionEngine.Engine.addLoadRegion(returnRegion);
            foreach (Networking.Client client in clients)
                client.sendUpdate(new Networking.CreateRegion(returnRegion.id, 0));
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
            if (InteractionEngine.Engine.status != InteractionEngine.Engine.Status.MULTIPLAYER_CLIENT)
                throw new InteractionEngineException("LoadRegion constructors should never be called, except by the LoadRegion.createLoadRegion() method and its overload. Please use one of those to construct a LoadRegion.");
            this.id = id;
            InteractionEngine.Engine.addLoadRegion(this);
        }

        /// <summary>
        /// Get rid of this LoadRegion. Sad, I know.
        /// </summary>
        public void deconstruct() {
            // Are we a client? If so, wait for an update from the server who will independently process the EventHandling.Event that called this method.
            if (InteractionEngine.Engine.status == InteractionEngine.Engine.Status.MULTIPLAYER_CLIENT) return;
            System.Collections.Generic.List<int> tempList = new System.Collections.Generic.List<int>();
            tempList.AddRange(objects);
            foreach (int gameObjectID in tempList) InteractionEngine.Engine.getGameObject(gameObjectID).deconstruct();
            if (Engine.status == Engine.Status.MULTIPLAYER_SERVER || Engine.status == Engine.Status.MULTIPLAYER_SERVERCLIENT) this.addUpdate(new Networking.DeleteRegion(this.id));
            this.internalDeconstruct();
        }

        /// <summary>
        /// Gets rid of this LoadRegion. Called by the above method as well as DeleteRegion.executeUpdate().
        /// </summary>
        internal void internalDeconstruct() {
            InteractionEngine.Engine.removeLoadRegion(this.id);
        }

        #endregion

        #region Client Communication

        /// <summary>
        /// This method is utilized inside the GameWorld to send a client a LoadRegion and all its GameObjects.
        /// It will make sure that client's copy of the LoadRegion stays synchronized.
        /// NEVER CALL THIS FROM A MULTIPLAYER_CLIENT! (Or a SINGLE_PLAYER for that matter.)
        /// </summary>
        /// <param name="client">The client to give the LoadRegion to.</param>
        public void sentToClient(Networking.Client client) {
            if (InteractionEngine.Engine.status != Engine.Status.MULTIPLAYER_SERVER && InteractionEngine.Engine.status != Engine.Status.MULTIPLAYER_SERVERCLIENT)
                throw new GameWorldException("LoadRegion.sendToClient(Client) can only be called from the GameWorld by a server. Please note that the majority of EventMethods execute on both the server and client. Two notable exceptions are EventMethods triggered by Client.onJoin and Client.onQuit. You can also: 1) Manually check Engine.status within the EventMethod, 2) Make sure only the MULTIPLAYER_SERVERCLIENT can trigger the event from the UI, or 3) Call the EventMethod from the EventCache, and make sure it is only added to the EventCache on the server.");
            // The easy part. Send them the LoadRegion.
            client.sendUpdate(new Networking.CreateRegion(id, this.getGameObjectArray().Length));
            // Now we have to package up every object in the LoadRegion! Yay!
            foreach (int gameObjectID in objects) {
                GameObjectable gameObject = Engine.getGameObject(gameObjectID);
                if (gameObject != null) client.sendUpdate(new Networking.CreateObject(id, gameObjectID, gameObject.classHash, gameObject.getFieldValues()));
            }
            // Add the LoadRegion to the local list of LoadRegions for this client, so we know we need to update the client with this LoadRegion.
            client.addLoadRegion(this);
        }

        /// <summary>
        /// This method is utilized inside the GameWorld to remove a synchronized LoadRegion from a client.
        /// NEVER CALL THIS FROM A MULTIPLAYER_CLIENT! (Or a SINGLE_PLAYER for that matter.)
        /// </summary>
        /// <param name="client">The client to give the LoadRegion to.</param>
        public void removeFromClient(Networking.Client client) {
            if (InteractionEngine.Engine.status != Engine.Status.MULTIPLAYER_SERVER && InteractionEngine.Engine.status != Engine.Status.MULTIPLAYER_SERVERCLIENT)
                throw new GameWorldException("LoadRegion.removeFromClient(Client) can only be called from the GameWorld by a server. Please note that the majority of EventMethods execute on both the server and client. Two notable exceptions are EventMethods triggered by Client.onJoin and Client.onQuit. You can also: 1) Manually check Engine.status within the EventMethod, 2) Make sure only the MULTIPLAYER_SERVERCLIENT can trigger the event from the UI, or 3) Call the EventMethod from the EventCache, and make sure it is only added to the EventCache on the server.");
            // Send them a DeleteRegion
            client.sendUpdate(new Networking.DeleteRegion(id));
            // Remove the LoadRegion from the local list of LoadRegions for this client, so we don't try to update them regarding it.
            client.removeLoadRegion(id);
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
            if (objects.Contains(objectId)) return;
            lock (objects) {
                this.objects.Add(objectId);
                this.objects.Sort();
            }
        }

        /// <summary>
        /// This method is triggered when a GameObject leaves this LoadRegion.
        /// Does not set the GameObject's LoadRegion field to null.
        /// </summary>
        /// <param name="objectId">The id of the GameObject that leaves the room.</param>
        internal void removeObject(int objectId) {
            if (!objects.Contains(objectId)) return;
            lock (objects) {
                this.objects.Remove(objectId);
            }
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
        /// This method returns an array of all the GameObjects in the LoadRegion.
        /// </summary>
        /// <returns>The GameObjects in this LoadRegion.</returns>
        public Constructs.GameObjectable[] getGameObjectArray() {
            Constructs.GameObjectable[] gameObjectArray;
            // Copy over to prevent 
            lock (objects) {
                gameObjectArray = new Constructs.GameObjectable[objects.Count];
                int i = 0;
                foreach (int gameObject in objects) gameObjectArray[i++] = Engine.getGameObject(gameObject);
            }
            return gameObjectArray;
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
        internal void addUpdate(Networking.Update update) {
            updateBuffer.Add(update);
        }

        /// <summary>
        /// Add an Updatable to the UpdateField Buffer.
        /// </summary>
        /// <param name="field">The Updatable to be added.</param>
        internal void registerUpdate(Datatypes.Updatable field) {
            updateFieldBuffer.Add(field);
        }

        /// <summary>
        /// Removes an Updatable from the list of Updatables requiring network updates.
        /// It's a good idea to call this method before deleting an object with this field.
        /// </summary>
        /// <param name="update">The Updatable no longer requiring an UPDATE_FIELD transfer code.</param>
        internal void cancelUpdate(Datatypes.Updatable update) {
            updateFieldBuffer.Remove(update);
        }

        /// <summary>
        /// Reset the Update Buffer.
        /// </summary>
        internal void resetBuffer() {
            updateBuffer.Clear();
        }

        /// <summary>
        /// Get the current list of Updates in this LoadRegion's Update Buffer.
        /// </summary>
        /// <returns>The list of Updates.</returns>
        internal System.Collections.Generic.List<Networking.Update> getUpdates() {
            foreach (Datatypes.Updatable updating in updateFieldBuffer)
                updateBuffer.Add(new Networking.UpdateField(updating.gameObject.id, updating.id, updating.getValue()));
            updateFieldBuffer.Clear();
            return updateBuffer;
        }

        #endregion

    }

}