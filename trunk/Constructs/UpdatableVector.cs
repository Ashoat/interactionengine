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
| CONSTRUCTS                               |
| * UpdatableVector                  Class |
\*••••••••••••••••••••••••••••••••••••••••*/

namespace InteractionEngine.Constructs.Datatypes {

    public class UpdatableVector : Updatable {

        #region FACTORY

        // Contains a string identifying this Updatable.
        // Used for the factory method and for communicating CREATE_OBJECT messages.
        internal const string classHash = "vector";

        /// <summary>
        /// This constructor adds this Updatable's factory to the static factoryList in Updatable. 
        /// </summary>
        static UpdatableVector() {
            Updatable.factoryList.Add(classHash, new UpdatableFactory(getUpdatable));
        }

        /// <summary>
        /// The factory method. Constructs a field, assigns it an ID, adds it to the HashTable, and returns it.
        /// This method is used for instantiating a new field on the client side. It is needed to allow for easy creation of new types. For more information, research the factory method.
        /// This constructor can be used if and only if called by a gamer that is a client AND is not a server.
        /// </summary>
        /// <param name="fieldContainer">The FieldContainer that this datatype would be associated with.</param>
        /// <param name="id">The ID the server constructed this Updatable with. Passed for synchronization.</param>
        /// <returns>The Updatable that is constructed.</returns>
        public static Updatable getUpdatable(FieldContainer fieldContainer, int id) {
            if (GameWorld.GameWorld.status != GameWorld.GameWorld.Status.MULTIPLAYER_CLIENT)
                throw new System.Exception("You're not a client, so why are you calling the Updatable factory method?");
            return new UpdatableVector(fieldContainer, id, false);
        }

        /// <summary>
        /// Constructs a field through the factory method.
        /// The constructor should only be used on the client side.
        /// </summary>
        /// <param name="fieldContainer">The FieldContainer that this datatype would be associated with.</param>
        /// <param name="id">ID with which to instantiate the class with.</param>
        private UpdatableVector(FieldContainer fieldContainer, int id, bool blah)
            : base(fieldContainer, id) { // Yah, I know it's really hacky. We had no choice. Go away.
        }

        /// <summary>
        /// Contains a string hash of this type.
        /// Used when passing a CREATE_FIELD to specify which type to use.
        /// </summary>
        /// <returns>The string hash.</returns>
        internal override string getClassHash() {
            return classHash;
        }

        #endregion

        /// <summary>
        /// Constructs a field and assigns it an ID. 
        /// This constructor can be used if and only if called by the server side. 
        /// This constructor does not add the field to the fieldHashlist on the FieldContainer, as it is not used on the server. 
        /// </summary>
        /// <param name="fieldContainer">The FieldContainer that this datatype would be associated with.</param>
        public UpdatableVector(FieldContainer fieldContainer)
            : base(fieldContainer) {
        }

        /// <summary>
        /// Constructs a field, assigns it an ID, and gives it a value. 
        /// This constructor can be used if and only if called by the server side. 
        /// This constructor does not add the field to the fieldHashlist on the FieldContainer, as it is not used on the server. 
        /// </summary>
        /// <param name="fieldContainer">The FieldContainer that this datatype would be associated with.</param>
        /// <param name="nonUpdatable">The int to set the value of this UpdatableVector to.</param>
        public UpdatableVector(FieldContainer fieldContainer, Microsoft.Xna.Framework.Vector3 nonUpdatable)
            : base(fieldContainer) {
            value = nonUpdatable;
        }

        /// <summary>
        /// Tests if this Updatable equals another Updatable.
        /// </summary>
        /// <param name="obj">The value to compare to.</param>
        /// <returns>True is equal; false otherwise.</returns>
        public override bool Equals(object obj) {
            if (obj is Microsoft.Xna.Framework.Vector3) {
                return ((Microsoft.Xna.Framework.Vector3)obj).Equals(this.value);
            } else if (obj is UpdatableVector) {
                return ((UpdatableVector)obj).value.Equals(this.value);
            } else return false;
        }

        /// <summary>
        /// The value and realValue. The value is used for changing from within the developer's game code. The value is private and stores the true value. 
        /// Funny how much encapsulation that little piece of information requires...
        /// </summary>
        private Microsoft.Xna.Framework.Vector3 realValue;
        private Microsoft.Xna.Framework.Vector3 lastServerSetValue; // Used by the client so that server updates stay in a bad-calculation-safe variable.
        public Microsoft.Xna.Framework.Vector3 value {
            get {
                return new Microsoft.Xna.Framework.Vector3(realValue.X, realValue.Y, realValue.Z);
            }
            set {
                // Too much work
                // if (realValue.Equals(value)) return;
                realValue = value;
                this.registerUpdate();
            }
        }

        /// <summary>
        /// Returns the binary representation of this field for network communication purposes.
        /// Called by the method writeUpdate defined in Updatable whenever a network update is requested.
        /// </summary>
        protected override void writeValue(Microsoft.Xna.Framework.Net.PacketWriter cache) {
            // The three-part message for updating of an Updatable happens 2/3 at the Update writeUpdate 
            // and 1/3 at the child writeValue
            cache.Write(this.realValue);
        }

        /// <summary>
        /// This method is called when the server updates this variable.
        /// </summary>
        /// <param name="reader">The BinaryReader to read the variable from.</param>
        public override void readUpdate(System.IO.BinaryReader reader) {
            this.realValue = ((Microsoft.Xna.Framework.Net.PacketReader)reader).ReadVector3();
        }

    }

}