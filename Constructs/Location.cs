/*••••••••••••••••••••••••••••••••••••••••*\
| Interaction Engine                       |
| (C) Copyright Bluestone Coding 2009      |
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

namespace InteractionEngine.Constructs {

    /**
     * Holds all information and methods regarding location.
     */
    
    public class Location {

        // Contains a reference to the GameObject this Location module is associated with.
        // Used for constructing Updatables.
        private Locatable gameObject;
        // Contains this location in the form of a three-dimensional point.
        private InteractionEngine.Constructs.Datatypes.UpdatableVector point;
        // Contains the heading of this point. Yaw, pitch, and roll, in that order
        private InteractionEngine.Constructs.Datatypes.UpdatableVector heading;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="gameObject">The GameObject whose Location this is.</param>
        public Location(Locatable gameObject) {
            this.gameObject = gameObject;
            this.point = new InteractionEngine.Constructs.Datatypes.UpdatableVector(gameObject);
            this.heading = new InteractionEngine.Constructs.Datatypes.UpdatableVector(gameObject);
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

        /// <summary>
        /// Returns the point represented by this Location.
        /// </summary>
        /// <returns>The point represented by this Location.</returns>
        public virtual Microsoft.Xna.Framework.Vector3 getPoint() {
            return point.value;
        }

        // In radians
        public virtual Microsoft.Xna.Framework.Vector3 getHeading() {
            return heading.value;
        }

        // In radians
        public virtual float yaw {
            get { return this.heading.value.X; }
            set { this.heading.value = new Microsoft.Xna.Framework.Vector3(value, this.heading.value.Y, this.heading.value.Z); }
        }
        public virtual float pitch {
            get { return this.heading.value.Y; }
            set { this.heading.value = new Microsoft.Xna.Framework.Vector3(this.heading.value.X, value, this.heading.value.Z); }
        }
        public virtual float roll {
            get { return this.heading.value.Z; }
            set { this.heading.value = new Microsoft.Xna.Framework.Vector3(this.heading.value.X, this.heading.value.Y, value); }
        }

        public virtual void moveTo(Microsoft.Xna.Framework.Vector3 position) {
            this.point.value = position;
        }

        /// <summary>
        /// Translates this Location.
        /// </summary>
        /// <param name="translation">The vector describing the translation by which to move.</param>
        public virtual void move(Microsoft.Xna.Framework.Vector3 translation) {
            // Loving the encapsulation!
            this.point.value += translation;
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