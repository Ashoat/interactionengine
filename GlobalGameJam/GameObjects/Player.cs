using InteractionEngine.Constructs;
using InteractionEngine.UserInterface;
using Microsoft.Xna.Framework.Input;
using GlobalGameJam.Graphics;
using InteractionEngine.Constructs.Datatypes;

namespace GlobalGameJam.GameObjects {

    public class Player : Character {

        #region FACTORY

        /// <summary>
        /// All GameObjects need a parameterless constructor for calling by GameObject.createGameObject() and GameObject.createFromUpdate().
        /// NEVER CALL THIS! This constructor is exclusively for use by the InteractionEngine. If anyone else calls it things will break.
        /// If you want to construct this object, use GameObject.createGameObject(LoadRegion).
        /// </summary>
        public Player() {
        }

        // The classHash, a unique identifying string for the class. Hmm, wow, that's kind of redundant, isn't that? C# already provides such a function through reflection. Oh well.
        // Used for the factory methods called when the client receives a CREATE_NEW_OBJECT update from the server computer.
        public const string realHash = "Player";
        public override string classHash {
            get { return realHash; }
        }

        /// <summary>
        /// The static constructor. Adds the class's factory method to the GameObject factoryList when the class is first loaded.
        /// </summary>
        static Player() {
            GameObject.factoryList.Add(realHash, new GameObjectFactory(GameObject.createFromUpdate<Game>));
        }

        #endregion

        private const int PUNK_TYPE = 0;
        private const int MONK_TYPE = 1;
        private const int SKUNK_TYPE = 2;
        private UpdatableInteger characterType;

        public override void construct() {
            base.construct();
            graphics.setTexture("Player");
            characterType = new UpdatableInteger(this);
        }

        public void handleKey(Keys key) {
            switch (key) {
                case Keys.Left:
                    move(-1, 0);
                    break;
                case Keys.Right:
                    move(1, 0);
                    break;
                case Keys.Up:
                    move(0, 1);
                    break;
                case Keys.Down:
                    move(0, -1);
                    break;
                case Keys.RightShift:
                    shiftType();
                    break;
                case Keys.RightControl:
                    attack();
                    break;
                default:
                    break;
            }
        }

        private void shiftType() {
            if (this.busyPerformingAction.value > 0) return;
            if (this.map.getVisibleCharacters(this.position, 2).Count == 0) {
                this.characterType.value = (characterType.value + 1) % 3;
                // one second delay for shape-shifting?
                this.busyPerformingAction.value = 1000;
                // do animation
            } else {
                // notify of failure to shift
            }
        }

        private void attack() {
            if (this.busyPerformingAction.value > 0) return;
            // do stuff
        }

        public override void update() {
            base.update();
        }

        public override int attackModifier(Entity attackee) {
            throw new System.NotImplementedException();
            if (this.characterType.value == PUNK_TYPE) {
            } else if (this.characterType.value == MONK_TYPE) {
            } else if (this.characterType.value == SKUNK_TYPE) {
            } else throw new System.ApplicationException("Character type is neither a punk, a monk, or a skunk.");
        }

    }

}