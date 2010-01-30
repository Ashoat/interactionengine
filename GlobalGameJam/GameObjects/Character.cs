using InteractionEngine.Constructs;
using InteractionEngine.UserInterface;
using Microsoft.Xna.Framework.Input;
using GlobalGameJam.Graphics;
using InteractionEngine;
using Microsoft.Xna.Framework;
using InteractionEngine.Constructs.Datatypes;

namespace GlobalGameJam.GameObjects {

    public abstract class Character : Entity {

        private Map map; // TODO

        private UpdatableInteger attackStrength;

        public abstract int attackModifier(Entity attackee);

        public abstract void update();

        public bool move(int dx, int dy) {
            Vector3 curPosition = this.getLocation().Position;
            Vector3 newPosition = curPosition + new Vector3(dx, dy, 0);
            if (map.isEmpty(newPosition)) {
                this.getLocation().Position = newPosition;
                map.setCharacter(this);
                // to do: start animation and freeze movement until current movement is done
                return true;
            }
            return false;
        }

    }

}