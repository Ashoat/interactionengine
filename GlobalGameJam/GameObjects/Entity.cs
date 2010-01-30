using InteractionEngine.Constructs;
using InteractionEngine.UserInterface;
using Microsoft.Xna.Framework.Input;
using GlobalGameJam.Graphics;
using InteractionEngine.Constructs.Datatypes;

namespace GlobalGameJam.GameObjects {

    public abstract class Entity : GameObject, Graphable {

        public UpdatableInteger health;

        protected EntityGraphics graphics;
        public InteractionEngine.UserInterface.Graphics getGraphics() {
            return graphics;
        }

        protected Location location;
        public Location getLocation() {
            return location;
        }

    }

}