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
| * FieldContainer          Abstract Class |
\*••••••••••••••••••••••••••••••••••••••••*/

namespace InteractionEngine.Constructs {

    /**
     * This interface is implemented by any container of Updatables.
     */
    public abstract class FieldContainer {

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
        private int realID = -1;
        public int id {
            get {
                return realID;
            }
            set {
                if (id == -1) id = value;
            }
        }
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

        /// <summary>
        /// Add an Updatable to the list of Updatables requiring network updates.
        /// </summary>
        /// <param name="update">The Updatable requiring an update network broadcast.</param>
        public abstract void addUpdate(Datatypes.Updatable update);

        /// <summary>
        /// Removes an Updatable from the list of Updatables requiring network updates.
        /// It's a good idea to call this method before deleting a field.
        /// </summary>
        /// <param name="update">The Updatable no longer requiring an update network broadcast.</param>
        public abstract void cancelUpdate(Datatypes.Updatable update);

        /// <summary>
        /// Write an update directly to the binary cache of network updates waiting to be sent.
        /// </summary>
        /// <param name="update">The byte array containing the update information.</param>
        public abstract void writeUpdate(byte[] update);

    }

}