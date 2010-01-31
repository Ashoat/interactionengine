using InteractionEngine.Constructs;
using InteractionEngine.UserInterface;
using Microsoft.Xna.Framework.Input;
using GlobalGameJam.Graphics;
using InteractionEngine.Constructs.Datatypes;
using InteractionEngine.UserInterface.TwoDimensional;
using Microsoft.Xna.Framework;
using System;

namespace GlobalGameJam.GameObjects {

    public abstract class Entity : GameObject, Graphable2D {

        private UpdatableInteger health;
        private UpdatableInteger x;
        private UpdatableInteger y;
        private Direction direction;

        public int Health {
            get { return health.value; }
            set { health.value = value; }
        }

        protected Map map;

        public Map Map {
            get { return map; }
            set { map = value; }
        }

        public Microsoft.Xna.Framework.Point Position {
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
                //this.location.Position = new Vector3(screenx, screeny, 0);
            }
        }

        public Direction Direction {
            get { return this.direction; }
            set {
                this.direction = value;
                switch (direction) {
                    case Direction.NORTH:
                        this.location.yaw = 0;
                        break;
                    case Direction.SOUTH:
                        this.location.yaw = 180;
                        break;
                    case Direction.WEST:
                        this.location.yaw = -90;
                        break;
                    case Direction.EAST:
                        this.location.yaw = 90;
                        break;
                    default:
                        throw new ArgumentException("Invalid direction");
                }
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

        public virtual EntityGraphics makeGraphics() {
            return new EntityGraphics(this);
        }

        public override void construct() {
            this.location = new Location(this);
            this.graphics = makeGraphics();
            this.health = new UpdatableInteger(this);
            this.x = new UpdatableInteger(this);
            this.y = new UpdatableInteger(this);
            health.value = 100;
        }

        public virtual void wasAttacked(Character attacker, int damage) {
            health.value -= damage;
            GameObject.createGameObject<BAM>(this.getLoadRegion()).setLocationAndLifespan(this.location.Position, 300);
            if (health.value <= 0) {
                map.removeEntity(this);
                this.deconstruct();
            }
        }

    }

}