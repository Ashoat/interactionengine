using InteractionEngine.Constructs;
using InteractionEngine.UserInterface;
using Microsoft.Xna.Framework.Input;
using GlobalGameJam.Graphics;
using InteractionEngine.Constructs.Datatypes;
using System;
using System.Collections.Generic;

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

        public override void construct() {
            base.construct();
            movementDelay = 200;
            graphics.setTexture("Player");
        }

        public void handleKey(Keys key) {
            switch (key) {
                case Keys.Left:
                    move(Direction.WEST);
                    break;
                case Keys.Right:
                    move(Direction.EAST);
                    break;
                case Keys.Up:
                    move(Direction.NORTH);
                    break;
                case Keys.Down:
                    move(Direction.SOUTH);
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
            Console.WriteLine(key);
        }

        private void shiftType() {
            if (this.busyPerformingAction.value > 0) return;
            if (this.Map.getVisibleCharacters(this.position, 2).Count == 0) {
                this.characterType = CharacterType.getNextShift(this.characterType);
                // one second delay for shape-shifting?
                this.busyPerformingAction.value = 1000;
                // do animation
            } else {
                Console.WriteLine("Can't shift, enemies nearby");
                // notify of failure to shift
            }
        }

        private void attack() {
            if (this.busyPerformingAction.value > 0) return;
            List<Character> chars = Map.getVisibleCharacters(position, 1);
            if (chars.Count >= 1) {
                Character chara = chars[0];
                attack(chara);
            }
            // do stuff
        }

        public override void update() {
            base.update();
        }

    }

}