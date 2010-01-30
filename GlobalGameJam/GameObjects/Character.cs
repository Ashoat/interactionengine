using InteractionEngine.Constructs;
using InteractionEngine.UserInterface;
using Microsoft.Xna.Framework.Input;
using GlobalGameJam.Graphics;
using InteractionEngine;
using Microsoft.Xna.Framework;
using InteractionEngine.Constructs.Datatypes;

namespace GlobalGameJam.GameObjects {

    public abstract class Character : Entity {


        public Map Map {
            get { return map; }
            set { map = value; }
        }

        private Map map; // TODO

        private UpdatableInteger attackStrength;
        // If this value is more than zero, the character is already busy doing something.
        protected UpdatableInteger busyPerformingAction;

        public abstract int attackModifier(Entity attackee);

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
        }

        /// <summary>
        /// Moves the specified distance in grid squares and returns true if possible.
        /// Returns false if there's something in the way or the character is
        /// busy performing some other action.
        /// </summary>
        /// <param name="dx">Horizontal grid displacement.</param>
        /// <param name="dy">Vertical grid displacement.</param>
        /// <returns>True if and only if the move completed successfully.</returns>
        public bool move(int dx, int dy) {
            if (busyPerformingAction.value > 0) return false;
            Point oldPosition = this.position;
            Point newPosition = new Point(oldPosition.X + dx, oldPosition.Y + dy);
            if (map.isEmpty(newPosition)) {
                this.position = newPosition;
                map.setCharacter(oldPosition,this);
                // to do: start animation and freeze movement until current movement is done
                return true;
            }
            return false;
        }

    }

}