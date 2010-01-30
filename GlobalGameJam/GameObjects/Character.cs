using InteractionEngine.Constructs;
using InteractionEngine.UserInterface;
using Microsoft.Xna.Framework.Input;
using GlobalGameJam.Graphics;
using InteractionEngine;
using Microsoft.Xna.Framework;
using InteractionEngine.Constructs.Datatypes;

namespace GlobalGameJam.GameObjects {

    public abstract class Character : Entity {

        protected Map map; // TODO

        private UpdatableInteger attackStrength;

        public abstract int attackModifier(Entity attackee);

        public abstract void update();

        public override void construct() {
            base.construct();
            this.attackStrength = new UpdatableInteger(this);
        }

        public bool move(int dx, int dy) {
            Point oldPosition = this.position;
            Point newPosition = new Point(oldPosition.X + dx, oldPosition.Y + dy);
            if (map.isEmpty(newPosition)) {
                this.position = newPosition;
                map.setCharacter(this);
                // to do: start animation and freeze movement until current movement is done
                return true;
            }
            return false;
        }

    }

}