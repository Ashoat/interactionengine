using InteractionEngine.Constructs;
using InteractionEngine.UserInterface;
using Microsoft.Xna.Framework.Input;
using GlobalGameJam.Graphics;
using InteractionEngine.Constructs.Datatypes;
using InteractionEngine.UserInterface.TwoDimensional;

namespace GlobalGameJam.GameObjects {

    public abstract class Entity : GameObject, Graphable2D {

        public UpdatableInteger health;

        protected EntityGraphics graphics;
        public InteractionEngine.UserInterface.Graphics getGraphics() {
            return graphics;
        }
        public Graphics2D getGraphics2D() {
            return graphics;
        }

        protected Location location;
        public Location getLocation() {
            return location;
        }

    }

}