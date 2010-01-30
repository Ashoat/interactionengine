using InteractionEngine.Constructs;
using InteractionEngine.UserInterface;
using Microsoft.Xna.Framework.Input;
using GlobalGameJam.Graphics;
using InteractionEngine;
using Microsoft.Xna.Framework;
using InteractionEngine.Constructs.Datatypes;

namespace GlobalGameJam.GameObjects {

    public abstract class Character : Entity {

        public class CharacterType {
            public class UpdatableCharacterType {

                /// <summary>
                /// Constructs a new UpdatableGameObject. 
                /// </summary>
                /// <param name="owner">The FieldContainer to which this field belongs. NOT the value of this field.</param>
                public UpdatableCharacterType(GameObjectable owner) {
                    realValue = new UpdatableInteger(owner);
                }

                /// <summary>
                /// Constructs a new UpdatableGameObject. 
                /// </summary>
                /// <param name="owner">The FieldContainer to which this field belongs. NOT the value of this field.</param>]
                /// <param name="gameObject">The GameObject to be stored as the value of this field.</param>
                public UpdatableCharacterType(GameObject owner, CharacterType type) {
                    realValue = new UpdatableInteger(owner);
                    realValue.value = type.typeNumber;
                }

                /// <summary>
                /// Constructs a new UpdatableGameObject from the given UpdatableInteger field.
                /// Intended for use on client-side constructors.
                /// </summary>
                /// <param name="field">The UpdatableInteger field holding the GameObject's ID.</param>
                public UpdatableCharacterType(UpdatableInteger field) {
                    realValue = field;
                }

                /// <summary>
                /// The value and realValue. The value is used for changing from within the developer's game code. The value is private and stores the true value. 
                /// Funny how much encapsulation that little piece of information requires...
                /// </summary> 
                private UpdatableInteger realValue;
                public CharacterType value {
                    get { return Types[realValue.value]; }
                    set { this.realValue.value = value.typeNumber; }
                }
            }
            public static readonly CharacterType PunkType;
            public static readonly CharacterType MonkType;
            public static readonly CharacterType SkunkType;
            public static readonly CharacterType[] Types;
            static CharacterType() {
                PunkType = new CharacterType(0);
                MonkType = new CharacterType(1);
                SkunkType = new CharacterType(2);
                Types = new CharacterType[] { PunkType, MonkType, SkunkType };
            }
            public static double getAttackModifier(CharacterType attacker, CharacterType attackee) {
                if (attacker == attackee) return 1;
                if ((attacker.typeNumber - attackee.typeNumber + 3) % 3 == 1) return 0.7;
                if ((attacker.typeNumber - attackee.typeNumber + 3) % 3 == 2) return 1.5;
                return 1;
            }
            // -1 means "run away", "0" means ignore, "1" means hunt down and kill
            public static int getAttitudeToward(CharacterType seer, CharacterType seee) {
                if (seer == seee) return 0;
                if ((seer.typeNumber - seee.typeNumber + 3) % 3 == 1) return -1;
                if ((seer.typeNumber - seee.typeNumber + 3) % 3 == 2) return 1;
                return 0;
            }
            public static CharacterType getNextShift(CharacterType current) {
                return Types[(current.typeNumber + 1) % 3];
            }
            private int typeNumber;
            private CharacterType(int typeNumber) {
                this.typeNumber = typeNumber;
            }
            public double getAttackModifier(Character attackee) {
                return getAttackModifier(this, attackee.characterType);
            }
            public int getAttitudeToward(Character seee) {
                return getAttitudeToward(this, seee.characterType);
            }
        }

        protected Map map;

        public Map Map {
            get { return map; }
            set { map = value; }
        }

        private UpdatableInteger attackStrength;
        // If this value is more than zero, the character is already busy doing something.
        // Specifies how many milliseconds it'll be before it's done.
        protected UpdatableInteger busyPerformingAction;
        private CharacterType.UpdatableCharacterType updatableCharacterType;
        public CharacterType characterType {
            get { return updatableCharacterType.value; }
            set { updatableCharacterType.value = value; }
        }
        protected int movementDelay = 300;
        protected int attackDelay = 400;

        public virtual void update() {
            if (busyPerformingAction.value != 0) {
                if (busyPerformingAction.value - Engine.gameTime.ElapsedGameTime.Milliseconds <= 0) {
                    busyPerformingAction.value = 0;
                } else {
                    busyPerformingAction.value -= Engine.gameTime.ElapsedGameTime.Milliseconds;
                }
            }
        }

        public override void construct() {
            base.construct();
            this.attackStrength = new UpdatableInteger(this);
            this.busyPerformingAction = new UpdatableInteger(this);
            this.updatableCharacterType = new CharacterType.UpdatableCharacterType(this);
            this.attackStrength.value = 20;
        }

        /// <summary>
        /// Moves the specified direction in grid squares and returns true if possible.
        /// Returns false if there's something in the way or the character is
        /// busy performing some other action.
        /// </summary>
        /// <param name="moveDirection">Direction to move</param>
        /// <returns>True if and only if the move completed successfully.</returns>
        public bool move(Direction moveDirection) {
            if (busyPerformingAction.value > 0) return false;
            Point oldPosition = this.position;
            Point newPosition;
            switch (moveDirection) {
                case Direction.NORTH:
                    newPosition = new Point(oldPosition.X, oldPosition.Y - 1);
                    break;
                case Direction.EAST:
                    newPosition = new Point(oldPosition.X + 1, oldPosition.Y);
                    break;
                case Direction.SOUTH:
                    newPosition = new Point(oldPosition.X, oldPosition.Y + 1);
                    break;
                case Direction.WEST:
                    newPosition = new Point(oldPosition.X - 1, oldPosition.Y);
                    break;
                default:
                    newPosition = oldPosition;
                    break;
            }
            
            if (map.isEmpty(newPosition)) {
                this.position = newPosition;
                map.setCharacter(oldPosition,this);
                busyPerformingAction.value = movementDelay;
                // animate
                return true;
            }
            return false;
        }

        public override void wasAttacked(int damage) {
            base.wasAttacked(damage);
            // Remove the character from the map.
            if (this.Health < 0) map.setCharacter(this.position, null);
        }

        public void attack(Entity attackee) {
            if (busyPerformingAction.value > 0) return;
            double attackModifier = attackee is Character ? characterType.getAttackModifier((Character)attackee) : 1;
            attackee.wasAttacked((int)(this.attackStrength.value * attackModifier));
            busyPerformingAction.value = attackDelay;
            // animate
        }

    }

}