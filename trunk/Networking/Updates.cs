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
| NETWORKING                               |
| * Update                       Interface |
| * CreateRegion                 Class     |
| * DeleteRegion                 Class     |
| * CreateObject                 Class     |
| * DeleteObject                 Class     |
| * MoveObject                   Class     |
| * UpdateField                  Class     |
\*••••••••••••••••••••••••••••••••••••••••*/

namespace InteractionEngine.Networking {

    /// <summary>
    /// An interface representing an "update" from the Server.
    /// </summary>
    public abstract class Update {

        /// <summary>
        /// Transfer code constants.
        /// These constants are the first information passed in an update packet from the server. 
        /// They help determine how the client's InteractionEngine needs to handle that update.
        /// </summary>
        internal const byte CREATE_REGION = 0;    // This code tells the client to instantiate a new LoadRegion.
        internal const byte DELETE_REGION = 1;    // This code tells the client to delete a LoadRegion.
        internal const byte CREATE_OBJECT = 2;    // This code tells the client to instantiate a new GameObject using that GameObject's factory.
        internal const byte DELETE_OBJECT = 3;    // This code tells the client to delete a GameObject and to remove its reference from its LoadRegion.
        internal const byte MOVE_OBJECT = 4;      // This code tells the client to move an object from one LoadRegion to another.
        internal const byte UPDATE_FIELD = 5;     // This code tells the client to update an Updatable.

        /// <summary>
        /// Send this update across the network to a client.
        /// This method should only be used on the server side.
        /// </summary>
        /// <param name="writer">The BinaryWriter wrapped around the NetworkStream.</param>
        /// <param name="stream">The NetworkStream that we can serialize objects to.</param>
        /// <param name="formatter">The BinaryFormatter that can serialize objects to the NetworkStream.</param>
        internal abstract void sendUpdate(System.IO.BinaryWriter writer, System.Net.Sockets.NetworkStream stream, System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter);

        /// <summary>
        /// Execute the update this class contains.
        /// This method should only be used on the client side.
        /// </summary>
        internal abstract void executeUpdate();

    }

    /// <summary>
    /// Represents an Update from the server that directs the client to instantiate a new LoadRegion.
    /// </summary>
    public class CreateRegion : Update {

        // Contains a list of EventMethods that get triggered when this EventHandling.Event is executed.
        // Used on the client side so that post-instantiation work can happen, even though the original EventMethod couldn't do any post-instantiation work because the LoadRegion never instantiated.
        public static System.Collections.Generic.List<EventHandling.Event> onCreateRegion = new System.Collections.Generic.List<InteractionEngine.EventHandling.Event>();

        // Contains the ID to assign this LoadRegion.
        // Used for making sure LoadRegions have synchronized IDs across the network.
        private int loadRegionID;

        /// <summary>
        /// Construct this Update.
        /// This constructor should only be used on the server side.
        /// </summary>
        /// <param name="loadRegionID">The ID to assign this LoadRegion.</param>
        internal CreateRegion(int loadRegionID) {
            if (InteractionEngine.Engine.status != InteractionEngine.Engine.Status.MULTIPLAYER_SERVER && InteractionEngine.Engine.status != InteractionEngine.Engine.Status.MULTIPLAYER_SERVERCLIENT)
                throw new InteractionEngineException("Updates have seperate constructors for use on the client and server side. On the client, you must construct them using a BinaryReader containing the update packet from the server; on the server, you must construct them with the data that will be sent in the packet.");
            this.loadRegionID = loadRegionID;
        }

        /// <summary>
        /// Construct this Update using data from the BinaryReader wrapped around the NetworkStream.
        /// This constructor should only be used on the client side.
        /// </summary>
        /// <param name="reader">The BinaryReader containing the information needed to instantiate this class.</param>
        internal CreateRegion(System.IO.BinaryReader reader) {
            if (InteractionEngine.Engine.status != InteractionEngine.Engine.Status.MULTIPLAYER_CLIENT)
                throw new InteractionEngineException("Updates have seperate constructors for use on the client and server side. On the client, you must construct them using a BinaryReader containing the update packet from the server; on the server, you must construct them with the data that will be sent in the packet.");
            loadRegionID = reader.ReadInt32();
        }

        /// <summary>
        /// Send this update across the network to a client.
        /// This method should only be used on the server side.
        /// </summary>
        /// <param name="writer">The BinaryWriter wrapped around the NetworkStream.</param>
        /// <param name="stream">The NetworkStream that we can serialize objects to.</param>
        /// <param name="formatter">The BinaryFormatter that can serialize objects to the NetworkStream.</param>
        internal override void sendUpdate(System.IO.BinaryWriter writer, System.Net.Sockets.NetworkStream stream, System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter) {
            if (InteractionEngine.Engine.status != InteractionEngine.Engine.Status.MULTIPLAYER_SERVER && InteractionEngine.Engine.status != InteractionEngine.Engine.Status.MULTIPLAYER_SERVERCLIENT)
                throw new InteractionEngineException("Updates have seperate constructors for use on the client and server side. On the client, you must construct them using a BinaryReader containing the update packet from the server; on the server, you must construct them with the data that will be sent in the packet.");
            writer.Write(Update.CREATE_REGION);
            writer.Write(this.loadRegionID);
        }
        
        /// <summary>
        /// Execute the update this class contains.
        /// This method should only be used on the client side.
        /// </summary>
        internal override void executeUpdate() {
            if (InteractionEngine.Engine.status != InteractionEngine.Engine.Status.MULTIPLAYER_CLIENT)
                throw new InteractionEngineException("Updates have seperate constructors for use on the client and server side. On the client, you must construct them using a BinaryReader containing the update packet from the server; on the server, you must construct them with the data that will be sent in the packet.");
            if (InteractionEngine.Engine.getLoadRegion(this.loadRegionID) != null) return;
            Constructs.LoadRegion loadRegion = new Constructs.LoadRegion(this.loadRegionID);
            foreach (EventHandling.Event eventObject in onCreateRegion) {
                eventObject.parameter = (object)loadRegion;
                Engine.addEvent(eventObject);
            }
        }

    }

    /// <summary>
    /// Represents an Update from the server that directs the client to delete an existing LoadRegion.
    /// </summary>
    public class DeleteRegion : Update {

        // Contains the ID of the LoadRegion we want to delete.
        // Used for identifying the LoadRegion set for deletion.
        private int loadRegionID;

        /// <summary>
        /// Construct this Update.
        /// This constructor should only be used on the server side.
        /// </summary>
        /// <param name="loadRegionID">The ID of the LoadRegion we want to delete.</param>
        internal DeleteRegion(int loadRegionID) {
            if (InteractionEngine.Engine.status != InteractionEngine.Engine.Status.MULTIPLAYER_SERVER && InteractionEngine.Engine.status != InteractionEngine.Engine.Status.MULTIPLAYER_SERVERCLIENT)
                throw new InteractionEngineException("Updates have seperate constructors for use on the client and server side. On the client, you must construct them using a BinaryReader containing the update packet from the server; on the server, you must construct them with the data that will be sent in the packet.");
            this.loadRegionID = loadRegionID;
        }

        /// <summary>
        /// Construct this Update using data from the BinaryReader wrapped around the NetworkStream.
        /// This constructor should only be used on the client side.
        /// </summary>
        /// <param name="reader">The BinaryReader containing the information needed to instantiate this class.</param>
        internal DeleteRegion(System.IO.BinaryReader reader) {
            if (InteractionEngine.Engine.status != InteractionEngine.Engine.Status.MULTIPLAYER_CLIENT)
                throw new InteractionEngineException("Updates have seperate constructors for use on the client and server side. On the client, you must construct them using a BinaryReader containing the update packet from the server; on the server, you must construct them with the data that will be sent in the packet.");
            loadRegionID = reader.ReadInt32();
        }

        /// <summary>
        /// Send this update across the network to a client.
        /// This method should only be used on the server side.
        /// </summary>
        /// <param name="writer">The BinaryWriter wrapped around the NetworkStream.</param>
        /// <param name="stream">The NetworkStream that we can serialize objects to.</param>
        /// <param name="formatter">The BinaryFormatter that can serialize objects to the NetworkStream.</param>
        internal override void sendUpdate(System.IO.BinaryWriter writer, System.Net.Sockets.NetworkStream stream, System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter) {
            if (InteractionEngine.Engine.status != InteractionEngine.Engine.Status.MULTIPLAYER_SERVER && InteractionEngine.Engine.status != InteractionEngine.Engine.Status.MULTIPLAYER_SERVERCLIENT)
                throw new InteractionEngineException("Updates have seperate constructors for use on the client and server side. On the client, you must construct them using a BinaryReader containing the update packet from the server; on the server, you must construct them with the data that will be sent in the packet.");
            writer.Write(Update.DELETE_REGION);
            writer.Write(this.loadRegionID);
        }

        /// <summary>
        /// Execute the update this class contains.
        /// This method should only be used on the client side.
        /// </summary>
        internal override void executeUpdate() {
            if (InteractionEngine.Engine.status != InteractionEngine.Engine.Status.MULTIPLAYER_CLIENT)
                throw new InteractionEngineException("Updates have seperate constructors for use on the client and server side. On the client, you must construct them using a BinaryReader containing the update packet from the server; on the server, you must construct them with the data that will be sent in the packet.");
            Constructs.LoadRegion loadRegion = InteractionEngine.Engine.getLoadRegion(this.loadRegionID);
            if(loadRegion == null) return;
            loadRegion.internalDeconstruct();
        }

    }

    /// <summary>
    /// Represents an Update from the server that directs the client to instantiate a specified GameObject in the specified LoadRegion.
    /// Also contains a list of values to set for the GameObject's fields.
    /// </summary>
    public class CreateObject : Update {
        
        // Contains a list of EventMethods that get triggered when this EventHandling.Event is executed.
        // Used on the client side so that post-instantiation work can happen, even though the original EventMethod couldn't do any post-instantiation work because the LoadRegion never instantiated.
        public static System.Collections.Generic.List<EventHandling.Event> onCreateObject = new System.Collections.Generic.List<InteractionEngine.EventHandling.Event>();
        // Contains the ID of the LoadRegion this GameObject will be assigned to.
        // Used for making sure every GameObject has a home.
        private int loadRegionID;
        // Contains the ID to assign this GameObject.
        // Used for making sure GameObjects have synchronized IDs across the network.
        private int gameObjectID;
        // Contains a string identifying the GameObject subclass this CreateObject is instantiating.
        // Used for linking the specified class to a factory constructor.
        private string classHash;
        // Contains a dictionary pointing field IDs to their values.
        // Used for letting the client know what the current state of the GameObject is.
        private System.Collections.Generic.Dictionary<int, object> fieldValues;

        /// <summary>
        /// Construct this Update.
        /// This constructor should only be used on the server side.
        /// </summary>
        /// <param name="loadRegionID">The ID of the LoadRegion this GameObject will be assigned to.</param>
        /// <param name="gameObjectID">The ID to assign this GameObject.</param>
        /// <param name="classHash">A string identifying the GameObject subclass this CreateObject is instantiating.</param>
        /// <param name="fieldValues">A dictionary pointing field IDs to their values.</param>
        internal CreateObject(int loadRegionID, int gameObjectID, string classHash, System.Collections.Generic.Dictionary<int, object> fieldValues) {
            if (InteractionEngine.Engine.status != InteractionEngine.Engine.Status.MULTIPLAYER_SERVER && InteractionEngine.Engine.status != InteractionEngine.Engine.Status.MULTIPLAYER_SERVERCLIENT)
                throw new InteractionEngineException("Updates have seperate constructors for use on the client and server side. On the client, you must construct them using a BinaryReader containing the update packet from the server; on the server, you must construct them with the data that will be sent in the packet.");
            this.loadRegionID = loadRegionID;
            this.gameObjectID = gameObjectID;
            this.classHash = classHash;
            this.fieldValues = fieldValues;
        }

        /// <summary>
        /// Construct this Update using data from the BinaryReader wrapped around the NetworkStream.
        /// This constructor should only be used on the client side.
        /// </summary>
        /// <param name="reader">The BinaryReader containing the information needed to instantiate this class.</param>
        /// <param name="stream">The NetworkStream where we can deserialize objects from.</param>
        /// <param name="formatter">The BinaryFormatter that can deserialize objects from the NetworkStream.</param>
        internal CreateObject(System.IO.BinaryReader reader, System.Net.Sockets.NetworkStream stream, System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter) {
            if (InteractionEngine.Engine.status != InteractionEngine.Engine.Status.MULTIPLAYER_CLIENT)
                throw new InteractionEngineException("Updates have seperate constructors for use on the client and server side. On the client, you must construct them using a BinaryReader containing the update packet from the server; on the server, you must construct them with the data that will be sent in the packet.");
            loadRegionID = reader.ReadInt32();
            gameObjectID = reader.ReadInt32();
            classHash = reader.ReadString();
            int numberOfObjects = reader.ReadInt32();
            fieldValues = new System.Collections.Generic.Dictionary<int, object>();
            for (int i = 0; i < numberOfObjects; i++)
                fieldValues.Add(reader.ReadInt32(), formatter.Deserialize(stream));
        }

        /// <summary>
        /// Send this update across the network to a client.
        /// This method should only be used on the server side.
        /// </summary>
        /// <param name="writer">The BinaryWriter wrapped around the NetworkStream.</param>
        /// <param name="stream">The NetworkStream that we can serialize objects to.</param>
        /// <param name="formatter">The BinaryFormatter that can serialize objects to the NetworkStream.</param>
        internal override void sendUpdate(System.IO.BinaryWriter writer, System.Net.Sockets.NetworkStream stream, System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter) {
            if (InteractionEngine.Engine.status != InteractionEngine.Engine.Status.MULTIPLAYER_SERVER && InteractionEngine.Engine.status != InteractionEngine.Engine.Status.MULTIPLAYER_SERVERCLIENT)
                throw new InteractionEngineException("Updates have seperate constructors for use on the client and server side. On the client, you must construct them using a BinaryReader containing the update packet from the server; on the server, you must construct them with the data that will be sent in the packet.");
            writer.Write(Update.CREATE_OBJECT);
            writer.Write(this.loadRegionID);
            writer.Write(this.gameObjectID);
            writer.Write(this.classHash);
            writer.Write(this.fieldValues.Count);
            foreach (System.Collections.Generic.KeyValuePair<int, object> pair in fieldValues) {
                writer.Write(pair.Key);
                if (pair.Value == null) formatter.Serialize(stream, new object());
                else formatter.Serialize(stream, pair.Value);
            }
        }

        /// <summary>
        /// Execute the update this class contains.
        /// This method should only be used on the client side.
        /// </summary>
        internal override void executeUpdate() {
            if (InteractionEngine.Engine.status != InteractionEngine.Engine.Status.MULTIPLAYER_CLIENT)
                throw new InteractionEngineException("Updates have seperate constructors for use on the client and server side. On the client, you must construct them using a BinaryReader containing the update packet from the server; on the server, you must construct them with the data that will be sent in the packet.");
            if (InteractionEngine.Engine.getGameObject(this.gameObjectID) != null) return;
            Constructs.LoadRegion loadRegion = InteractionEngine.Engine.getLoadRegion(this.loadRegionID);
            if (loadRegion == null) return;
            Constructs.GameObject gameObject = Constructs.GameObject.factoryList[this.classHash](loadRegion, this.gameObjectID);
            foreach (System.Collections.Generic.KeyValuePair<int, object> pair in fieldValues) {
                Constructs.Datatypes.Updatable field = gameObject.getField(pair.Key);
                if (field == null) continue;
                field.setValue(pair.Value);
            }
            foreach (EventHandling.Event eventObject in onCreateObject) {
                Engine.addEvent(new EventHandling.Event(eventObject.gameObjectID, eventObject.eventHash, (object)gameObject));
            }
        }

    }

    /// <summary>
    /// Represents an Update from the server that directs the client to delete an existing GameObject.
    /// </summary>
    public class DeleteObject : Update {

        // Contains the ID of the GameObject we want to delete.
        // Used for identifying the GameObject set for deletion.
        private int gameObjectID;

        /// <summary>
        /// Construct this Update.
        /// This constructor should only be used on the server side.
        /// </summary>
        /// <param name="gameObjectID">The ID of the GameObject we want to delete.</param>
        internal DeleteObject(int gameObjectID) {
            if (InteractionEngine.Engine.status != InteractionEngine.Engine.Status.MULTIPLAYER_SERVER && InteractionEngine.Engine.status != InteractionEngine.Engine.Status.MULTIPLAYER_SERVERCLIENT)
                throw new InteractionEngineException("Updates have seperate constructors for use on the client and server side. On the client, you must construct them using a BinaryReader containing the update packet from the server; on the server, you must construct them with the data that will be sent in the packet.");
            this.gameObjectID = gameObjectID;
        }

        /// <summary>
        /// Construct this Update using data from the BinaryReader wrapped around the NetworkStream.
        /// This constructor should only be used on the client side.
        /// </summary>
        /// <param name="reader">The BinaryReader containing the information needed to instantiate this class.</param>
        internal DeleteObject(System.IO.BinaryReader reader) {
            if (InteractionEngine.Engine.status != InteractionEngine.Engine.Status.MULTIPLAYER_CLIENT)
                throw new InteractionEngineException("Updates have seperate constructors for use on the client and server side. On the client, you must construct them using a BinaryReader containing the update packet from the server; on the server, you must construct them with the data that will be sent in the packet.");
            gameObjectID = reader.ReadInt32();
        }

        /// <summary>
        /// Send this update across the network to a client.
        /// This method should only be used on the server side.
        /// </summary>
        /// <param name="writer">The BinaryWriter wrapped around the NetworkStream.</param>
        /// <param name="stream">The NetworkStream that we can serialize objects to.</param>
        /// <param name="formatter">The BinaryFormatter that can serialize objects to the NetworkStream.</param>
        internal override void sendUpdate(System.IO.BinaryWriter writer, System.Net.Sockets.NetworkStream stream, System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter) {
            if (InteractionEngine.Engine.status != InteractionEngine.Engine.Status.MULTIPLAYER_SERVER && InteractionEngine.Engine.status != InteractionEngine.Engine.Status.MULTIPLAYER_SERVERCLIENT)
                throw new InteractionEngineException("Updates have seperate constructors for use on the client and server side. On the client, you must construct them using a BinaryReader containing the update packet from the server; on the server, you must construct them with the data that will be sent in the packet.");
            writer.Write(Update.DELETE_OBJECT);
            writer.Write(this.gameObjectID);
        }

        /// <summary>
        /// Execute the update this class contains.
        /// This method should only be used on the client side.
        /// </summary>
        internal override void executeUpdate() {
            if (InteractionEngine.Engine.status != InteractionEngine.Engine.Status.MULTIPLAYER_CLIENT)
                throw new InteractionEngineException("Updates have seperate constructors for use on the client and server side. On the client, you must construct them using a BinaryReader containing the update packet from the server; on the server, you must construct them with the data that will be sent in the packet.");
            Constructs.GameObjectable gameObject = InteractionEngine.Engine.getGameObject(this.gameObjectID);
            if (gameObject == null) return;
            if (gameObject is Constructs.GameObject) ((Constructs.GameObject)gameObject).internalDeconstruct();
        }

    }

    /// <summary>
    /// Represents an update from the server that directs the client to move a GameObject from one LoadRegion to another.
    /// </summary>
    public class MoveObject : Update {

        // Contains the ID of the GameObject we want to move.
        // Used for identifying the GameObject set to be moved.
        private int gameObjectID;
        // Contains the ID of LoadRegion we are moving the specified GameObject to.
        // Used for identifying which LoadRegion to move the GameObject to.
        private int loadRegionID;

        /// <summary>
        /// Construct this Update.
        /// This constructor should only be used on the server side.
        /// </summary>
        /// <param name="gameObjectID">The ID of the GameObject we want to move.</param>
        /// <param name="loadRegionID">The ID of the LoadRegion we want to delete.</param>
        internal MoveObject(int gameObjectID, int loadRegionID) {
            if (InteractionEngine.Engine.status != InteractionEngine.Engine.Status.MULTIPLAYER_SERVER && InteractionEngine.Engine.status != InteractionEngine.Engine.Status.MULTIPLAYER_SERVERCLIENT)
                throw new InteractionEngineException("Updates have seperate constructors for use on the client and server side. On the client, you must construct them using a BinaryReader containing the update packet from the server; on the server, you must construct them with the data that will be sent in the packet.");
            this.gameObjectID = gameObjectID;
            this.loadRegionID = loadRegionID;
        }

        /// <summary>
        /// Construct this Update using data from the BinaryReader wrapped around the NetworkStream.
        /// This constructor should only be used on the client side.
        /// </summary>
        /// <param name="reader">The BinaryReader containing the information needed to instantiate this class.</param>
        internal MoveObject(System.IO.BinaryReader reader) {
            if (InteractionEngine.Engine.status != InteractionEngine.Engine.Status.MULTIPLAYER_CLIENT)
                throw new InteractionEngineException("Updates have seperate constructors for use on the client and server side. On the client, you must construct them using a BinaryReader containing the update packet from the server; on the server, you must construct them with the data that will be sent in the packet.");
            gameObjectID = reader.ReadInt32();
            loadRegionID = reader.ReadInt32();
        }

        /// <summary>
        /// Send this update across the network to a client.
        /// This method should only be used on the server side.
        /// </summary>
        /// <param name="writer">The BinaryWriter wrapped around the NetworkStream.</param>
        /// <param name="stream">The NetworkStream that we can serialize objects to.</param>
        /// <param name="formatter">The BinaryFormatter that can serialize objects to the NetworkStream.</param>
        internal override void sendUpdate(System.IO.BinaryWriter writer, System.Net.Sockets.NetworkStream stream, System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter) {
            if (InteractionEngine.Engine.status != InteractionEngine.Engine.Status.MULTIPLAYER_SERVER && InteractionEngine.Engine.status != InteractionEngine.Engine.Status.MULTIPLAYER_SERVERCLIENT)
                throw new InteractionEngineException("Updates have seperate constructors for use on the client and server side. On the client, you must construct them using a BinaryReader containing the update packet from the server; on the server, you must construct them with the data that will be sent in the packet.");
            writer.Write(Update.MOVE_OBJECT);
            writer.Write(this.gameObjectID);
            writer.Write(this.loadRegionID);
        }

        /// <summary>
        /// Execute the update this class contains.
        /// This method should only be used on the client side.
        /// </summary>
        internal override void executeUpdate() {
            if (InteractionEngine.Engine.status != InteractionEngine.Engine.Status.MULTIPLAYER_CLIENT)
                throw new InteractionEngineException("Updates have seperate constructors for use on the client and server side. On the client, you must construct them using a BinaryReader containing the update packet from the server; on the server, you must construct them with the data that will be sent in the packet.");
            Constructs.GameObjectable gameObject = InteractionEngine.Engine.getGameObject(this.gameObjectID);
            Constructs.LoadRegion loadRegion = InteractionEngine.Engine.getLoadRegion(this.loadRegionID);
            if (gameObject == null || loadRegion == null) return;
            if (gameObject is Constructs.GameObject) ((Constructs.GameObject)gameObject).internalMove(loadRegion);
        }

    }

    /// <summary>
    /// Represents an update from the server that directs the client to change the value of an Updatable field.
    /// </summary>
    public class UpdateField : Update {

        // Contains the ID of the FieldContainer containing the specified Updatable.
        // Used for grabbing the Updatable.
        private int fieldContainerID;
        // Contains the ID of the Updatable we need to update.
        // Used for grabbing the Updatable.
        private int fieldID;
        // Contains the new value for this Updatable, cast as an object.
        // Used for updating the Updatable.
        private object newValue;

        /// <summary>
        /// Construct this Update.
        /// This constructor should only be used on the server side.
        /// </summary>
        /// <param name="fieldContainerID">The ID of the FieldContainer containing the specified Updatable.</param>
        /// <param name="fieldID">The ID of the Updatable we need to update.</param>
        /// <param name="newValue">The new value for this Updatable, cast as an object.</param>
        internal UpdateField(int fieldContainerID, int fieldID, object newValue) {
            if (InteractionEngine.Engine.status != InteractionEngine.Engine.Status.MULTIPLAYER_SERVER && InteractionEngine.Engine.status != InteractionEngine.Engine.Status.MULTIPLAYER_SERVERCLIENT)
                throw new InteractionEngineException("Updates have seperate constructors for use on the client and server side. On the client, you must construct them using a BinaryReader containing the update packet from the server; on the server, you must construct them with the data that will be sent in the packet.");
            this.fieldContainerID = fieldContainerID;
            this.fieldID = fieldID;
            this.newValue = newValue;
        }

        /// <summary>
        /// Construct this Update using data from the BinaryReader wrapped around the NetworkStream.
        /// This constructor should only be used on the client side.
        /// </summary>
        /// <param name="reader">The BinaryReader containing the information needed to instantiate this class.</param>
        /// <param name="stream">The NetworkStream where we can deserialize objects from.</param>
        /// <param name="formatter">The BinaryFormatter that can deserialize objects from the NetworkStream.</param>
        internal UpdateField(System.IO.BinaryReader reader, System.Net.Sockets.NetworkStream stream, System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter) {
            if (InteractionEngine.Engine.status != InteractionEngine.Engine.Status.MULTIPLAYER_CLIENT)
                throw new InteractionEngineException("Updates have seperate constructors for use on the client and server side. On the client, you must construct them using a BinaryReader containing the update packet from the server; on the server, you must construct them with the data that will be sent in the packet.");
            fieldContainerID = reader.ReadInt32();
            fieldID = reader.ReadInt32();
            newValue = formatter.Deserialize(stream);
        }

        /// <summary>
        /// Send this update across the network to a client.
        /// This method should only be used on the server side.
        /// </summary>
        /// <param name="writer">The BinaryWriter wrapped around the NetworkStream.</param>
        /// <param name="stream">The NetworkStream that we can serialize objects to.</param>
        /// <param name="formatter">The BinaryFormatter that can serialize objects to the NetworkStream.</param>
        internal override void sendUpdate(System.IO.BinaryWriter writer, System.Net.Sockets.NetworkStream stream, System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter) {
            if (InteractionEngine.Engine.status != InteractionEngine.Engine.Status.MULTIPLAYER_SERVER && InteractionEngine.Engine.status != InteractionEngine.Engine.Status.MULTIPLAYER_SERVERCLIENT)
                throw new InteractionEngineException("Updates have seperate constructors for use on the client and server side. On the client, you must construct them using a BinaryReader containing the update packet from the server; on the server, you must construct them with the data that will be sent in the packet.");
            writer.Write(Update.UPDATE_FIELD);
            writer.Write(this.fieldContainerID);
            writer.Write(this.fieldID);
            if (newValue == null) newValue = new object();
            formatter.Serialize(stream, newValue);
        }

        /// <summary>
        /// Execute the update this class contains.
        /// This method should only be used on the client side.
        /// </summary>
        internal override void executeUpdate() {
            if (InteractionEngine.Engine.status != InteractionEngine.Engine.Status.MULTIPLAYER_CLIENT)
                throw new InteractionEngineException("Updates have seperate constructors for use on the client and server side. On the client, you must construct them using a BinaryReader containing the update packet from the server; on the server, you must construct them with the data that will be sent in the packet.");
            Constructs.GameObjectable gameObject = InteractionEngine.Engine.getGameObject(this.fieldContainerID);
            if (gameObject == null) return;
            Constructs.Datatypes.Updatable updatable = gameObject.getField(this.fieldID);
            if (updatable == null) return;
            updatable.setValue(this.newValue);
        }

    }

}