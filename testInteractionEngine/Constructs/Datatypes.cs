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
| * UpdatableFactory        Delegate       |
| * Updatable               Abstract Class |
| * UpdatableBoolean        Class          |
| * UpdatableInteger        Class          |
| * UpdatableCharacter      Class          |
| * UpdatableDouble         Class          |
| * UpdatableString         Class          |
| * UpdatableArray          Class          |
| * UpdatableList           Class          |
| * UpdatableGameObject     Class          |
\*••••••••••••••••••••••••••••••••••••••••*/

namespace InteractionEngine.Constructs.Datatypes {

    /**
     * This delegate contains a reference to a factory.
     * Used in the factoryList in Updatable. See below.
     */
    public delegate Updatable UpdatableFactory(FieldContainer fieldContainer, int id);

    /**
     * This interface signifies that a variable is Updatable.
     * Only Updatable variables can be used in the GameWorld, as being Updatable is vital to proper Client-Server communication.
     */
    public abstract class Updatable {

        // Contains a list of all the Updatable factory methods.
        // Used for figuring out which Updatable to instantiate when a CREATE_FIELD command is issued from the server.
        public static System.Collections.Generic.Dictionary<string, UpdatableFactory> factoryList = new System.Collections.Generic.Dictionary<string, UpdatableFactory>();
        // Contains an ID that is needed for communication between client and server, as references will be lost across a network.
        // Used for having an unique ID for the fieldHashlist in GameWorld, so that the GameWorld knows which field to update in the case of an UPDATE_FIELD command from the server.
        public readonly int id = -1;
        // Contains a reference to this Updatable's FieldContainer.
        // Used so that when a field is updated, we know what FieldContainer (and hence what LoadRegion) to register that update with.
        public FieldContainer fieldContainer;

        /// <summary>
        /// Constructs a field and assigns it an ID. 
        /// This constructor does not add the field to the fieldHashlist on the FieldContainer, as that hashlist needs only to be used by multiplayer clients. 
        /// If a multiplayer server calls this method, it will send messages to other users notifying them to create this field.
        /// </summary>
        /// <param name="fieldContainer">The FieldContainer that this datatype would be associated with.</param>
        protected Updatable(FieldContainer fieldContainer) {
            // If you're not on the server, don't use this constructor.
            if (GameWorld.GameWorld.status == GameWorld.GameWorld.Status.MULTIPLAYER_CLIENT)
                throw new System.Exception("You cannot use the Updatable(FieldContainer) constructor if you are a client in a multiplayer game. You need the ID of the Updatable from the server to prevent synchronization errors.");
            // Add the field to the fieldContainer and get an ID.
            this.fieldContainer = fieldContainer;
            this.id = fieldContainer.addField(this);
            // Add the CREATE_FIELD so that all the clients get the update.
            if (GameWorld.GameWorld.status != GameWorld.GameWorld.Status.SINGLE_PLAYER) {
            }
        }

        /// <summary>
        /// Constructs a field through the factory method.
        /// The constructor should only be used through extension by factory methods on the client side.
        /// </summary>
        /// <param name="fieldContainer">The FieldContainer that this datatype would be associated with.</param>
        /// <param name="id">ID with which to instantiate the class with.</param>
        protected Updatable(FieldContainer fieldContainer, int id) {
            this.fieldContainer = fieldContainer;
            this.id = id;
            this.fieldContainer.addField(this);
        }

        /// <summary>
        /// Contains a string hash of this type.
        /// Used when passing a CREATE_FIELD to specify which type to use.
        /// </summary>
        /// <returns>The string hash.</returns>
        internal abstract string getClassHash();

        // Whether or not an update is currently registered with the FieldContainer so that this Updatable will be updated in the next network broadcast.
        // Used in the two methods used to communicate with the FieldContainer.
        private bool updateRegistered = false;

        /// <summary>
        /// One of two methods used for communicating with the FieldContainer.
        /// Registers this Updatable to be updated the next time the FieldContainer has a network update.
        /// Avoids re-registering if this Updatable is already registered and the FieldContainer has not had a network update since last registration.
        /// This method is to be called whenever a change occurs in an Updatable field requiring updates to be sent.
        /// </summary>
        protected void registerUpdate() {
            if (!this.updateRegistered && (GameWorld.GameWorld.status == GameWorld.GameWorld.Status.MULTIPLAYER_SERVER || GameWorld.GameWorld.status == GameWorld.GameWorld.Status.MULTIPLAYER_SERVERCLIENT)) {
                this.fieldContainer.addUpdate(this);
                updateRegistered = true;
            }
        }

        /// <summary>
        /// One of two methods used for communicating with the FieldContainer.
        /// Writes this Updatable's update data into the PacketWriter for transmittance to other network players.
        /// Assumes that this Updatable is no longer registered for a FieldContainer update broadcast once the method returns.
        /// </summary>
        /// <param name="cache">The PacketWriter used to send updates to other network players.</param>
        public void writeUpdate(Microsoft.Xna.Framework.Net.PacketWriter cache) {
            cache.Write(GameWorld.GameWorld.UPDATE_FIELD);
            cache.Write(this.fieldContainer.getID());
            cache.Write(this.id);
            this.writeValue(cache);
            updateRegistered = false;
        }

        /// <summary>
        /// Returns the binary representation of this field for network communication purposes.
        /// Called by the method writeUpdate defined in Updatable whenever a network update is requested.
        /// </summary>
        protected abstract void writeValue(Microsoft.Xna.Framework.Net.PacketWriter cache);

        /// <summary>
        /// This method is called when the server updates this variable.
        /// </summary>
        /// <param name="reader">The BinaryReader to read the variable from.</param>
        public abstract void readUpdate(System.IO.BinaryReader reader);

    }

    /**
     * The GameWorld boolean datatype.
     */
    public class UpdatableBoolean : Updatable {

        #region FACTORY

        // Contains a string identifying this Updatable.
        // Used for the factory method and for communicating CREATE_OBJECT messages.
        internal const string classHash = "bool";

        /// <summary>
        /// This constructor adds this Updatable's factory to the static factoryList in Updatable. 
        /// </summary>
        static UpdatableBoolean() {
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
                throw new System.Exception("You're not a multiplayer client, so why are you calling the Updatable factory method?");
            return new UpdatableBoolean(fieldContainer, id);
        }

        /// <summary>
        /// Constructs a field for the factory method.
        /// The constructor should only be used on the client side.
        /// </summary>
        /// <param name="fieldContainer">The FieldContainer that this datatype would be associated with.</param>
        /// <param name="id">ID with which to instantiate the class with.</param>
        private UpdatableBoolean(FieldContainer fieldContainer, int id)
            : base(fieldContainer, id) {
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
        public UpdatableBoolean(FieldContainer fieldContainer)
            : base(fieldContainer) {
        }

        /// <summary>
        /// Constructs a field, assigns it an ID, and gives it a value. 
        /// This constructor can be used if and only if called by the server side. 
        /// This constructor does not add the field to the fieldHashlist on the FieldContainer, as it is not used on the server. 
        /// </summary>
        /// <param name="fieldContainer">The FieldContainer that this datatype would be associated with.</param>
        /// <param name="nonUpdatable">The bool to set the value of this UpdatableBoolean to.</param>
        public UpdatableBoolean(FieldContainer fieldContainer, bool nonUpdatable)
            : base(fieldContainer) {
            value = nonUpdatable;
        }

        /// <summary>
        /// Tests if this Updatable equals another Updatable.
        /// </summary>
        /// <param name="obj">The value to compare to.</param>
        /// <returns>True is equal; false otherwise.</returns>
        public override bool Equals(object obj) {
            if (!(obj is UpdatableBoolean)) return false;
            return ((UpdatableBoolean)obj).value == this.value;
        }

        /// <summary>
        /// The value and realValue. The value is used for changing from within the developer's game code. The value is private and stores the true value. 
        /// Funny how much encapsulation that little piece of information requires...
        /// </summary>
        private bool realValue;
        private bool lastServerSetValue; // Used by the client so that server updates stay in a bad-calculation-safe variable.
        public bool value {
            get {
                return realValue;
            }
            set {
                if (realValue == value) return;
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
        /// This method is called by client when the server updates this variable.
        /// </summary>
        /// <param name="reader">The BinaryReader to read the variable from.</param>
        public override void readUpdate(System.IO.BinaryReader reader) {
            this.realValue = reader.ReadBoolean();
            this.lastServerSetValue = this.realValue;
        }

    }

    /**
     * The GameWorld integer datatype.
     */
    public class UpdatableInteger : Updatable {

        #region FACTORY

        // Contains a string identifying this Updatable.
        // Used for the factory method and for communicating CREATE_OBJECT messages.
        internal const string classHash = "int";

        /// <summary>
        /// This constructor adds this Updatable's factory to the static factoryList in Updatable. 
        /// </summary>
        static UpdatableInteger() {
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
            if (GameWorld.GameWorld.status != InteractionEngine.GameWorld.GameWorld.Status.MULTIPLAYER_CLIENT)
                throw new System.Exception("You're not a client, so why are you calling the Updatable factory method?");
            return new UpdatableInteger(fieldContainer, id, false);
        }

        /// <summary>
        /// Constructs a field through the factory method.
        /// The constructor should only be used on the client side.
        /// </summary>
        /// <param name="fieldContainer">The FieldContainer that this datatype would be associated with.</param>
        /// <param name="id">ID with which to instantiate the class with.</param>
        private UpdatableInteger(FieldContainer fieldContainer, int id, bool blah)
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
        public UpdatableInteger(FieldContainer fieldContainer)
            : base(fieldContainer) {
        }

        /// <summary>
        /// Constructs a field, assigns it an ID, and gives it a value. 
        /// This constructor can be used if and only if called by the server side. 
        /// This constructor does not add the field to the fieldHashlist on the FieldContainer, as it is not used on the server. 
        /// </summary>
        /// <param name="fieldContainer">The FieldContainer that this datatype would be associated with.</param>
        /// <param name="nonUpdatable">The int to set the value of this UpdatableInteger to.</param>
        public UpdatableInteger(FieldContainer fieldContainer, int nonUpdatable)
            : base(fieldContainer) {
            value = nonUpdatable;
        }

        /// <summary>
        /// Tests if this Updatable equals another Updatable.
        /// </summary>
        /// <param name="obj">The value to compare to.</param>
        /// <returns>True is equal; false otherwise.</returns>
        public override bool Equals(object obj) {
            if (obj is int || obj is System.Int32) {
                return (int)obj == this.value;
            } else if (obj is UpdatableInteger) {
                return ((UpdatableInteger)obj).value == this.value;
            } else return false;
        }

        /// <summary>
        /// The value and realValue. The value is used for changing from within the developer's game code. The value is private and stores the true value. 
        /// Funny how much encapsulation that little piece of information requires...
        /// </summary>
        private int realValue;
        private int lastServerSetValue; // Used by the client so that server updates stay in a bad-calculation-safe variable.
        public int value {
            get {
                return realValue;
            }
            set {
                if (realValue == value) return;
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
            this.realValue = reader.ReadInt32();
        }

    }

    /**
     * The GameWorld character datatype.
     */
    public class UpdatableCharacter : Updatable {

        #region FACTORY

        // Contains a string identifying this Updatable.
        // Used for the factory method and for communicating CREATE_OBJECT messages.
        internal const string classHash = "char";

        /// <summary>
        /// This constructor adds this Updatable's factory to the static factoryList in Updatable. 
        /// </summary>
        static UpdatableCharacter() {
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
            if (GameWorld.GameWorld.status != InteractionEngine.GameWorld.GameWorld.Status.MULTIPLAYER_CLIENT)
                throw new System.Exception("You're not a client, so why are you calling the Updatable factory method?");
            return new UpdatableCharacter(fieldContainer, id);
        }

        /// <summary>
        /// Constructs a field through the factory method.
        /// The constructor should only be used on the client side.
        /// </summary>
        /// <param name="fieldContainer">The FieldContainer that this datatype would be associated with.</param>
        /// <param name="id">ID with which to instantiate the class with.</param>
        private UpdatableCharacter(FieldContainer fieldContainer, int id)
            : base(fieldContainer, id) {
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
        public UpdatableCharacter(FieldContainer fieldContainer)
            : base(fieldContainer) {
        }

        /// <summary>
        /// Constructs a field, assigns it an ID, and gives it a value. 
        /// This constructor can be used if and only if called by the server side. 
        /// This constructor does not add the field to the fieldHashlist on the FieldContainer, as it is not used on the server. 
        /// </summary>
        /// <param name="fieldContainer">The FieldContainer that this datatype would be associated with.</param>
        /// <param name="nonUpdatable">The char to set the value of this UpdatableCharacter to.</param>
        public UpdatableCharacter(FieldContainer fieldContainer, char nonUpdatable)
            : base(fieldContainer) {
            value = nonUpdatable;
        }

        /// <summary>
        /// Tests if this Updatable equals another Updatable.
        /// </summary>
        /// <param name="obj">The value to compare to.</param>
        /// <returns>True is equal; false otherwise.</returns>
        public override bool Equals(object obj) {
            if (!(obj is UpdatableCharacter)) return false;
            return ((UpdatableCharacter)obj).value == this.value;
        }

        /// <summary>
        /// The value and realValue. The value is used for changing from within the developer's game code. The value is private and stores the true value. 
        /// Funny how much encapsulation that little piece of information requires...
        /// </summary>
        private char realValue;
        private char lastServerSetValue; // Used by the client so that server updates stay in a bad-calculation-safe variable.
        public char value {
            get {
                return realValue;
            }
            set {
                if (realValue == value) return;
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
            this.realValue = reader.ReadChar();
        }

    }

    /**
     * The GameWorld double datatype.
     */
    public class UpdatableDouble : Updatable {

        #region FACTORY

        // Contains a string identifying this Updatable.
        // Used for the factory method and for communicating CREATE_OBJECT messages.
        internal const string classHash = "double";

        /// <summary>
        /// This constructor adds this Updatable's factory to the static factoryList in Updatable. 
        /// </summary>
        static UpdatableDouble() {
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
            if (GameWorld.GameWorld.status != InteractionEngine.GameWorld.GameWorld.Status.MULTIPLAYER_CLIENT)
                throw new System.Exception("You're not a client, so why are you calling the Updatable factory method?");
            return new UpdatableDouble(fieldContainer, id);
        }

        /// <summary>
        /// Constructs a field through the factory method.
        /// The constructor should only be used on the client side.
        /// </summary>
        /// <param name="fieldContainer">The FieldContainer that this datatype would be associated with.</param>
        /// <param name="id">ID with which to instantiate the class with.</param>
        private UpdatableDouble(FieldContainer fieldContainer, int id)
            : base(fieldContainer, id) {
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
        public UpdatableDouble(FieldContainer fieldContainer)
            : base(fieldContainer) {
        }

        /// <summary>
        /// Constructs a field, assigns it an ID, and gives it a value. 
        /// This constructor can be used if and only if called by the server side. 
        /// This constructor does not add the field to the fieldHashlist on the FieldContainer, as it is not used on the server. 
        /// </summary>
        /// <param name="fieldContainer">The FieldContainer that this datatype would be associated with.</param>
        /// <param name="nonUpdatable">The double to set the value of this UpdatableDouble to.</param>
        public UpdatableDouble(FieldContainer fieldContainer, double nonUpdatable)
            : base(fieldContainer) {
            value = nonUpdatable;
        }

        /// <summary>
        /// Tests if this Updatable equals another Updatable.
        /// </summary>
        /// <param name="obj">The value to compare to.</param>
        /// <returns>True is equal; false otherwise.</returns>
        public override bool Equals(object obj) {
            if (!(obj is UpdatableDouble)) return false;
            return ((UpdatableDouble)obj).value == this.value;
        }

        /// <summary>
        /// The value and realValue. The value is used for changing from within the developer's game code. The value is private and stores the true value. 
        /// Funny how much encapsulation that little piece of information requires...
        /// </summary>
        private double realValue;
        private double lastServerSetValue; // Used by the client so that server updates stay in a bad-calculation-safe variable.
        public double value {
            get {
                return realValue;
            }
            set {
                if (realValue == value) return;
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
            this.realValue = reader.ReadDouble();
        }

    }

    /**
     * The GameWorld string datatype.
     */
    public class UpdatableString : Updatable {

        #region FACTORY

        // Contains a string identifying this Updatable.
        // Used for the factory method and for communicating CREATE_OBJECT messages.
        internal const string classHash = "string";

        /// <summary>
        /// This constructor adds this Updatable's factory to the static factoryList in Updatable. 
        /// </summary>
        static UpdatableString() {
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
            if (GameWorld.GameWorld.status != InteractionEngine.GameWorld.GameWorld.Status.MULTIPLAYER_CLIENT)
                throw new System.Exception("You're not a client, so why are you calling the Updatable factory method?");
            return new UpdatableString(fieldContainer, id);
        }

        /// <summary>
        /// Constructs a field through the factory method.
        /// The constructor should only be used on the client side.
        /// </summary>
        /// <param name="fieldContainer">The FieldContainer that this datatype would be associated with.</param>
        /// <param name="id">ID with which to instantiate the class with.</param>
        private UpdatableString(FieldContainer fieldContainer, int id)
            : base(fieldContainer, id) {
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
        public UpdatableString(FieldContainer fieldContainer)
            : base(fieldContainer) {
        }

        /// <summary>
        /// Constructs a field, assigns it an ID, and gives it a value. 
        /// This constructor can be used if and only if called by the server side. 
        /// This constructor does not add the field to the fieldHashlist on the FieldContainer, as it is not used on the server. 
        /// </summary>
        /// <param name="fieldContainer">The FieldContainer that this datatype would be associated with.</param>
        /// <param name="nonUpdatable">The string to set the value of this UpdatableString to.</param>
        public UpdatableString(FieldContainer fieldContainer, string nonUpdatable)
            : base(fieldContainer) {
            value = nonUpdatable;
        }

        /// <summary>
        /// Tests if this Updatable equals another Updatable.
        /// </summary>
        /// <param name="obj">The value to compare to.</param>
        /// <returns>True is equal; false otherwise.</returns>
        public override bool Equals(object obj) {
            if (!(obj is UpdatableString)) return false;
            return ((UpdatableString)obj).value == this.value;
        }

        /// <summary>
        /// The value and realValue. The value is used for changing from within the developer's game code. The value is private and stores the true value. 
        /// Funny how much encapsulation that little piece of information requires...
        /// </summary>
        private string realValue;
        private string lastServerSetValue; // Used by the client so that server updates stay in a bad-calculation-safe variable.
        public string value {
            get {
                return realValue;
            }
            set {
                if (realValue == value) return;
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
            this.realValue = reader.ReadString();
        }

    }

    /**
     * The GameWorld array datatype.
     * We were going to make this with generics, but sucks because it gets really hard to pass those generics across a network.
     */
    public class UpdatableArray : Updatable {

        #region FACTORY

        // Contains a string identifying this Updatable.
        // Used for the factory method and for communicating CREATE_OBJECT messages.
        internal const string classHash = "array";

        /// <summary>
        /// This constructor adds this Updatable's factory to the static factoryList in Updatable. 
        /// </summary>
        static UpdatableArray() {
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
            if (GameWorld.GameWorld.status != InteractionEngine.GameWorld.GameWorld.Status.MULTIPLAYER_CLIENT)
                throw new System.Exception("You're not a client, so why are you calling the Updatable factory method?");
            return new UpdatableArray(fieldContainer, id, false);
        }

        /// <summary>
        /// Constructs a field through the factory method.
        /// The constructor should only be used on the client side.
        /// </summary>
        /// <param name="fieldContainer">The FieldContainer that this datatype would be associated with.</param>
        /// <param name="id">ID with which to instantiate the class with.</param>
        private UpdatableArray(FieldContainer fieldContainer, int id, bool blah)
            : base(fieldContainer, id) { // Yah, I know it's hacky. We had no choice. Go away.
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
        /// Constructs a field.
        /// This constructor will send two communications: a CREATE_FIELD and an UPDATE_FIELD.
        /// The CREATE_FIELD will instantiate the Updatable and the UPDATE_FIELD will instantiate the realValue and populate it.
        /// </summary>
        /// <param name="fieldContainer">The FieldContainer that this datatype would be associated with.</param>
        /// <param name="values">The list of field IDs that will be put into the array.</param>
        public UpdatableArray(FieldContainer fieldContainer, Updatable[] values)
            : base(fieldContainer) {
            realValue = (Updatable[])values.Clone();
            if (GameWorld.GameWorld.status == GameWorld.GameWorld.Status.MULTIPLAYER_SERVER || GameWorld.GameWorld.status == GameWorld.GameWorld.Status.MULTIPLAYER_SERVERCLIENT)
                registerUpdate();
        }

        /// <summary>
        /// The value and realValue. The value is used for changing from within the developer's game code. The value is private and stores the true value. 
        /// Funny how much encapsulation that little piece of information requires...
        /// Since the array does not get set after instantiation (each one of its child Updatables is set seperately), there is no set in this property.
        /// As for get, we return a clone of the encapsulated array, for the purpose of using the .NET Framework's built-in array methods.
        /// </summary>
        private Updatable[] realValue;
        public Updatable[] value {
            get { return (Updatable[])realValue.Clone(); }
        }

        /// <summary>
        /// This method is called when the server updates this variable.
        /// </summary>
        /// <param name="reader">The BinaryReader to read the variable from.</param>
        public override void readUpdate(System.IO.BinaryReader reader) {
            this.realValue = new Updatable[reader.ReadInt32()];
            for (int i = 0; i < realValue.Length; i++)
                realValue[i] = this.fieldContainer.getField(reader.ReadInt32());
        }

        /// <summary>
        /// Get the value of an item in the array.
        /// </summary>
        /// <param name="index">index The index to check the value at.</param>
        /// <returns>The value of the item.</returns>
        public Updatable get(int index) {
            return realValue[index];
        }

        /// <summary>
        /// Returns the length of the array.
        /// </summary>
        /// <returns>The length of the array.</returns>
        public int getLength() {
            return realValue.Length;
        }

        /// <summary>
        /// Returns the binary representation of this field for network communication purposes.
        /// Called by the method writeUpdate defined in Updatable whenever a network update is requested.
        /// </summary>
        protected override void writeValue(Microsoft.Xna.Framework.Net.PacketWriter cache) {
            // The three-part message for updating of an Updatable happens 2/3 at the Update writeUpdate 
            // and 1/3 at the child writeValue
            cache.Write(this.realValue.Length);
            for (int i = 0; i < realValue.Length; i++)
                cache.Write(realValue[i].id);
        }

    }

    /**
     * The GameWorld list datatype.
     * We were going to make this with generics, but sucks because it gets really hard to pass those generics across a network.
     */
    public class UpdatableList : Updatable {

        #region FACTORY

        // Contains a string identifying this Updatable.
        // Used for the factory method and for communicating CREATE_OBJECT messages.
        public static string classHash = "list";

        /// <summary>
        /// This constructor adds this Updatable's factory to the static factoryList in Updatable. 
        /// </summary>
        static UpdatableList() {
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
            if (GameWorld.GameWorld.status != InteractionEngine.GameWorld.GameWorld.Status.MULTIPLAYER_CLIENT)
                throw new System.Exception("You're not a client, so why are you calling the Updatable factory method?");
            return new UpdatableList(fieldContainer, id);
        }

        /// <summary>
        /// Constructs a field through the factory method.
        /// The constructor should only be used on the client side.
        /// </summary>
        /// <param name="fieldContainer">The FieldContainer that this datatype would be associated with.</param>
        /// <param name="id">ID with which to instantiate the class with.</param>
        private UpdatableList(FieldContainer fieldContainer, int id)
            : base(fieldContainer, id) {
        }

        /// <summary>
        /// Returns the class hash. 
        /// </summary>
        /// <returns>The class hash. Do we really have to tell you everything twice?</returns>
        internal override string getClassHash() {
            return classHash;
        }

        #endregion


        private System.IO.MemoryStream updateList = new System.IO.MemoryStream();
        private System.IO.BinaryWriter updateWriter;

        /// <summary>
        /// Constructs a field.
        /// This constructor should only be used on the server side.
        /// </summary>
        /// <param name="fieldContainer">The FieldContainer that this datatype would be associated with.</param>
        public UpdatableList(FieldContainer fieldContainer)
            : base(fieldContainer) {
            updateWriter = new System.IO.BinaryWriter(updateList);
            realValue = new System.Collections.Generic.List<Updatable>();
        }

        /// <summary>
        /// Constructs a field.
        /// The constructor should only be used on the server side.
        /// </summary>
        /// <param name="fieldContainer">The FieldContainer that this datatype would be associated with.</param>
        /// <param name="list">Array of Updatables to include in the UpdateableList. Must have length > 0.</param>
        public UpdatableList(FieldContainer fieldContainer, Updatable[] list)
            : base(fieldContainer) {
            updateWriter = new System.IO.BinaryWriter(updateList);
            realValue = new System.Collections.Generic.List<Updatable>();
            for (int i = 0; i < list.Length; i++) {
                realValue.Add(list[i]);
                writeUpdate(list[i].id, -1); // "Append" update
            }
        }

        /// <summary>
        /// Constructs a field.
        /// The constructor should only be used on the server side.
        /// </summary>
        /// <param name="fieldContainer">The FieldContainer that this datatype would be associated with.</param>
        /// <param name="list">List of Updatables to include in the UpdateableList. Must have length > 0.</param>
        public UpdatableList(FieldContainer fieldContainer, System.Collections.Generic.List<Updatable> list)
            : base(fieldContainer) {
            updateWriter = new System.IO.BinaryWriter(updateList);
            realValue = new System.Collections.Generic.List<Updatable>();
            for (int i = 0; i < list.Count; i++) {
                realValue[i] = list[i];
                writeUpdate(list[i].id, -1); // "Append" update
            }
        }

        /// <summary>
        /// The value and realValue. The value is used for changing from within the developer's game code. The value is private and stores the true value. 
        /// Funny how much encapsulation that little piece of information requires...
        /// Since the list may not be modified outside of the methods provided in this class, there is no set property.
        /// As for get, we return a clone of the encapsulated list, for the purpose of using the .NET Framework's built-in list methods.
        /// </summary> 
        private System.Collections.Generic.List<Updatable> realValue;
        public System.Collections.ObjectModel.ReadOnlyCollection<Updatable> value {
            get { return realValue.AsReadOnly(); }
        }

        /// <summary>
        /// Add an item to the list.
        /// </summary> 
        /// <param name="index">The index to add the item at. If this is less than 0, then the item will be added at the end.</param>
        /// <param name="input">The item to add.</param>
        public void add(int index, Updatable input) {
            realValue.Insert(index, input);
            if (GameWorld.GameWorld.status == GameWorld.GameWorld.Status.MULTIPLAYER_SERVER || GameWorld.GameWorld.status == GameWorld.GameWorld.Status.MULTIPLAYER_SERVERCLIENT)
                this.writeUpdate(input.id, index); // "Add" update
        }

        /// <summary>
        /// Add an item to the end of the list.
        /// </summary> 
        /// <param name="input">The item to add.</param>
        public void add(Updatable input) {
            realValue.Add(input);
            if (GameWorld.GameWorld.status == GameWorld.GameWorld.Status.MULTIPLAYER_SERVER || GameWorld.GameWorld.status == GameWorld.GameWorld.Status.MULTIPLAYER_SERVERCLIENT)
                this.writeUpdate(input.id, -1); // "Add" update
        }

        /// <summary>
        /// Remove an item from the list.
        /// </summary>
        /// <param name="index">The index at which to remove the item.</param>
        public void remove(int index) {
            realValue.RemoveAt(index);
            if (GameWorld.GameWorld.status == GameWorld.GameWorld.Status.MULTIPLAYER_SERVER || GameWorld.GameWorld.status == GameWorld.GameWorld.Status.MULTIPLAYER_SERVERCLIENT)
                this.writeUpdate(-1, index); // "Remove" update
        }

        /// <summary>
        /// Remove an item from the list.
        /// </summary>
        /// <param name="updatable">The Updatable you want to remove.</param>
        public void remove(object toRemove) {
            int removed = this.indexOf(toRemove);
            if (removed < 0) return;
            realValue.RemoveAt(removed);
            if (GameWorld.GameWorld.status == GameWorld.GameWorld.Status.MULTIPLAYER_SERVER || GameWorld.GameWorld.status == GameWorld.GameWorld.Status.MULTIPLAYER_SERVERCLIENT)
                this.writeUpdate(-1, removed); // "Remove" update
        }

        /// <summary>
        /// Clears all items from the list.
        /// </summary>
        public void clear() {
            realValue.Clear();
            if (GameWorld.GameWorld.status == GameWorld.GameWorld.Status.MULTIPLAYER_SERVER || GameWorld.GameWorld.status == GameWorld.GameWorld.Status.MULTIPLAYER_SERVERCLIENT)
                this.writeUpdate(-1, -1); // "Clear" update
        }

        /// <summary>
        /// Get the value of an item in the array.
        /// </summary>
        /// <param name="index">index The index to check the value at.</param>
        /// <returns>The value of the item.</returns>
        public Updatable get(int index) {
            return realValue[index];
        }

        /// <summary>
        /// Get this list's length.
        /// </summary>
        /// <returns>This list's length.</returns>
        public int getLength() {
            return realValue.Count;
        }

        /// <summary>
        /// Tests if the list contains the given Updatable.
        /// </summary>
        /// <param name="test">The value to test for. If it's an Updatable, then it searches for an Updatable with the same value. Otherwise, it searches for an Updatable with the given value.</param>
        /// <returns>True if the Updatable is in the list; false otherwise.</returns>
        public bool contains(object test) {
            return this.indexOf(test) >= 0;
        }

        /// <summary>
        /// Returns the index of the first occurance of an Updatable with the given value.
        /// Currently, searching directly by value works only for UpdatableInteger, as that is the only class that overrides the Equals() to work with raw values.
        /// </summary>
        /// <param name="test">An Updatable containing the desired value or the value itself if it's an int.</param>
        /// <returns>The index of the Updatable found, or -1 if it was not found.</returns>
        public int indexOf(object test) {
            int i = 0;
            foreach (Updatable item in realValue) {
                if (item.Equals(test)) return i;
                i++;
            }
            return -1;
        }

        /// <summary>
        /// This method is called when the server updates this variable.
        /// </summary>
        /// <param name="reader">The BinaryReader to read the variable from.</param>
        public override void readUpdate(System.IO.BinaryReader reader) {
            int length = reader.ReadInt32();
            for (; length > 0; length--) {
                int id = reader.ReadInt32();
                int index = reader.ReadInt32();
                if (id < 0 && index < 0) realValue.Clear();
                else if (id < 0) realValue.RemoveAt(index);
                else if (index < 0) realValue.Add(this.fieldContainer.getField(id));
                else realValue.Insert(index, this.fieldContainer.getField(id));
            }
        }

        /// <summary>
        /// Record a change in value and then register the update with the FieldContainer.
        /// </summary>
        /// <param name="id">Uhm... this isn't really possible to explain. See below.</param>
        /// <param name="index">Same here.</param>
        /// <example>id=-1, index=-1: clear the list</example>
        /// <example>id=-1, index=x: remove the item at index x</example>
        /// <example>id=x, index=-1: append the Updatable with id x to the list</example>
        /// <example>id=x, index=y: add the Updatable with id x to the list at index y</example>
        private void writeUpdate(int id, int index) {
            this.updateWriter.Write(id);
            this.updateWriter.Write(index);
            this.registerUpdate();
        }

        /// <summary>
        /// Returns the binary representation of recent updates to this field for network communication purposes.
        /// Called by the method writeUpdate defined in Updatable whenever a network update is requested.
        /// </summary>
        protected override void writeValue(Microsoft.Xna.Framework.Net.PacketWriter cache) {
            // The three-part message for updating of an Updatable happens 2/3 at the Update writeUpdate 
            // and 1/3 at the child writeValue
            cache.Write(updateList.Length / 8);
            cache.Write(updateList.ToArray());
            updateList.SetLength(0);
        }

    }

    /**
     * Not actually an Updatable, but a wrapper class that internally stores the GameObject's ID in an UpdatableInteger.
     */
    public class UpdatableGameObject<T> where T : GameObjectable {

        /// <summary>
        /// Constructs a new UpdatableGameObject. 
        /// </summary>
        /// <param name="owner">The FieldContainer to which this field belongs. NOT the value of this field.</param>
        public UpdatableGameObject(FieldContainer owner) {
            realValue = new UpdatableInteger(owner, -1);
        }

        /// <summary>
        /// Constructs a new UpdatableGameObject. 
        /// </summary>
        /// <param name="owner">The FieldContainer to which this field belongs. NOT the value of this field.</param>]
        /// <param name="gameObject">The GameObject to be stored as the value of this field.</param>
        public UpdatableGameObject(FieldContainer owner, T gameObject) {
            realValue = new UpdatableInteger(owner, gameObject.getID());
        }

        /// <summary>
        /// Constructs a new UpdatableGameObject from the given UpdatableInteger field.
        /// Intended for use on client-side constructors.
        /// </summary>
        /// <param name="field">The UpdatableInteger field holding the GameObject's ID.</param>
        public UpdatableGameObject(UpdatableInteger field) {
            realValue = field;
        }

        /// <summary>
        /// The value and realValue. The value is used for changing from within the developer's game code. The value is private and stores the true value. 
        /// Funny how much encapsulation that little piece of information requires...
        /// </summary> 
        private UpdatableInteger realValue;
        public T value {
            // default(T) returns null for reference types
            get { return (realValue.value == -1) ? default(T) : (T)GameWorld.GameWorld.getObject(realValue.value); }
            set { this.realValue.value = (value == null) ? -1 : value.getID(); }
        }

    }

}