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

        

        /// <summary>
        /// Add an Updatable to the list of Updatables requiring network updates.
        /// </summary>
        /// <param name="update">The Updatable requiring an update network broadcast.</param>
        public abstract void registerUpdate(Datatypes.Updatable update);

        /// <summary>
        /// Removes an Updatable from the list of Updatables requiring network updates.
        /// It's a good idea to call this method before deleting a field.
        /// </summary>
        /// <param name="update">The Updatable no longer requiring an update network broadcast.</param>
        public abstract void cancelUpdate(Datatypes.Updatable update);

    }

}