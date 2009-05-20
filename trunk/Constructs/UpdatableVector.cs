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

    /**
     * The GameWorld boolean datatype.
     */
    public class UpdatableVector : Updatable {

        /// <summary>
        /// Constructs a field and assigns it an ID. 
        /// </summary>
        /// <param name="fieldContainer">The FieldContainer that this datatype will be associated with.</param>
        public UpdatableVector(GameObject gameObject)
            : base(gameObject) {
        }

        /// <summary>
        /// Tests if this Updatable equals another Updatable.
        /// </summary>
        /// <param name="obj">The value to compare to.</param>
        /// <returns>True is equal; false otherwise.</returns>
        public override bool Equals(object obj) {
            if (!(obj is UpdatableVector)) return false;
            return ((UpdatableVector)obj).value == this.value;
        }

        /// <summary>
        /// The value and realValue. The value is used for changing from within the developer's game code. The value is private and stores the true value. 
        /// Funny how much encapsulation that little piece of information requires...
        /// </summary>
        private Microsoft.Xna.Framework.Vector3 realValue;
        private Microsoft.Xna.Framework.Vector3 lastServerSetValue; // Used by the client so that server updates stay in a bad-calculation-safe variable.
        public Microsoft.Xna.Framework.Vector3 value {
            get {
                return realValue;
            }
            set {
                if (realValue == value) return;
                realValue = value;
                this.gameObject.getLoadRegion().registerUpdate(this);
            }
        }

        /// <summary>
        /// Gets the value of this Updatable as an object so we can send it across to a client in an UPDATE_FIELD transfer code.
        /// This method should only ever be executed on the server.
        /// </summary>
        /// <returns>This field's value as an object.</returns>
        internal abstract object getValue() {
            return (object)realValue;
        }

        /// <summary>
        /// Sets the value of this Updatable using the information retrieved from an UPDATE_FIELD transfer code.
        /// This method should only ever be executed on a client.
        /// </summary>
        /// <param name="reader">The object containing the update.</param>
        internal abstract void setValue(object value) {
            this.realValue = (Microsoft.Xna.Framework.Vector3)value;
        }

    }

}