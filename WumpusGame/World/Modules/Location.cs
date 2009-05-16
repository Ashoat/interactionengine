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
| MODULE                                   |
| * Location                     Class     |
| * Locatable                    Interface |
\*••••••••••••••••••••••••••••••••••••••••*/

using InteractionEngine.Constructs.Datatypes;
using InteractionEngine.Constructs;
using InteractionEngine.GameWorld;

namespace WumpusGame.World.Modules {

    /**
     * Holds all information and methods regarding location.
     */
    
    public class Location {

        // Contains a reference to the GameObject this Location module is associated with.
        // Used for constructing Updatables.
        public GameObject gameObject;
        // Contains an Updatable instance fo the Room that this GameObject is in.
        // Used for determining Location.
        private readonly UpdatableGameObject<Room> roomLocation;
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="gameObject">The GameObject whose Location this is.</param>
        public Location(GameObject gameObject) {
            roomLocation = new UpdatableGameObject<Room>(gameObject);
            this.gameObject = gameObject;
        }

        /// <summary>
        /// Constructor for the client-side.
        /// </summary>
        /// <param name="gameObject">The GameObject of whose Location this is.</param>
        /// <param name="reader">The reader from which to read teh field datas.</param>
        internal Location(GameObject gameObject, Microsoft.Xna.Framework.Net.PacketReader reader) {
            /*byte transferCode = reader.ReadByte();
            UpdatableInteger intty = (UpdatableInteger)GameWorld.createField(reader);
            roomLocation = new UpdatableGameObject<Room>(intty);
            if (reader.ReadByte() == GameWorld.UPDATE_FIELD) GameWorld.updateField(reader);
            else reader.Position--;
            this.gameObject = gameObject;*/
        }

        /**
         * Returns the Room where the GameObject is located.
         * @return The Room where the GameObject is located.
         */
        public Room getRoomLocation() {
            return roomLocation.value;
        }

        /**
         * Sets the Room where the GameObject is located.
         * @param room The Room where the GameObject is located.
         */
        public void move(Room room) {
            Room here = this.getRoomLocation();
            if (here != null) here.loadRegion.removeObject(gameObject.id);
            roomLocation.value = room;
            // Add the object to the new LoadRegion.
            room.loadRegion.addObject(gameObject.id);
            // Do the room entering logic.
            room.objectEnteredRoom((Locatable)gameObject);
        }

    }

    /**
     * Implemented by GameObjects that have the Location module.
     */
    public interface Locatable : GameObjectable {

        /// <summary>
        /// Returns the Location module of this GameObject.
        /// </summary>
        /// <returns>The Location module associated with this GameObject.
        Location getLocation();

    }

}