using InteractionEngine.Constructs;
using InteractionEngine.UserInterface;
using Microsoft.Xna.Framework.Input;
using GlobalGameJam.Graphics;
using InteractionEngine.Constructs.Datatypes;
using InteractionEngine.UserInterface.TwoDimensional;
using Microsoft.Xna.Framework;

namespace GlobalGameJam.GameObjects {

    public abstract class Entity : GameObject, Graphable2D {

        public UpdatableInteger health;
        private UpdatableInteger x;
        private UpdatableInteger y;

        public Microsoft.Xna.Framework.Point position {
            get {
                return new Point(x.value, y.value);
            }
            set {
                const int GRID_WIDTH = 32;
                const int GRID_HEIGHT = 32;
                this.x.value = value.X;
                this.y.value = value.Y;
                int screenx = value.X * GRID_WIDTH;
                int screeny = value.Y * GRID_HEIGHT + 88;
                this.location.Position = new Vector3(screenx, screeny, 0);
            }
        }

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

        public override void construct() {
            this.location = new Location(this);
            this.graphics = new EntityGraphics(this);
            this.health = new UpdatableInteger(this);
            this.x = new UpdatableInteger(this);
            this.y = new UpdatableInteger(this);
        }

    }

}