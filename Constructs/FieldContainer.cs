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
| * FieldContainer               Interface |
\*••••••••••••••••••••••••••••••••••••••••*/

namespace InteractionEngine.Constructs {

    /**
     * This interface is implemented by any container of Updatables.
     */
    public interface FieldContainer {

        /// <summary>
        /// Get this FieldContainer's ID.
        /// </summary>
        /// <returns>This FieldContainer's ID.</returns>
        int getID();

        /// <summary>
        /// Add a field to the list of fields contained by this FieldContainer. ONLY USE ON THE SERVER SIDE.
        /// </summary>
        /// <param name="field">The field to add.</param>
        /// <returns>The ID of the field.</returns>
        int addField(Datatypes.Updatable field);

        /// <summary>
        /// Get a field from the list of fields contained by this FieldContainer.
        /// </summary>
        /// <param name="id">The ID of the field.</param>
        /// <returns>The field.</returns>
        Datatypes.Updatable getField(int id);

        /// <summary>
        /// Add an Updatable to the list of Updatables requiring network updates.
        /// </summary>
        /// <param name="update">The Updatable requiring an update network broadcast.</param>
        void addUpdate(Datatypes.Updatable update);

        /// <summary>
        /// Removes an Updatable from the list of Updatables requiring network updates.
        /// It's a good idea to call this method before deleting a field.
        /// </summary>
        /// <param name="update">The Updatable no longer requiring an update network broadcast.</param>
        void cancelUpdate(Datatypes.Updatable update);

        /// <summary>
        /// Write an update directly to the binary cache of network updates waiting to be sent.
        /// </summary>
        /// <param name="update">The byte array containing the update information.</param>
        void writeUpdate(byte[] update);

    }

}