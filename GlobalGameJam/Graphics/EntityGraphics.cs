using InteractionEngine.UserInterface.TwoDimensional;
using GlobalGameJam.GameObjects;

namespace GlobalGameJam.Graphics {

    public class EntityGraphics : Graphics2DTexture {

        private Entity entity;

        /// <summary>
        /// Construct!
        /// </summary>
        /// <param name="gameObject">GameObject!</param>
        public EntityGraphics(Entity entity) : base(entity) {
            this.entity = entity;
        }

        /// <summary>
        /// Construct!
        /// </summary>
        /// <param name="gameObject">GameObject!</param>
        public EntityGraphics(Entity entity, string texture) : base(entity, texture) {
            this.entity = entity;
        }

        public void setTexture(string texture) {
            this.TextureName = texture;
        }

        public void setPosition(int x, int y) {
            entity.getLocation().Position = new Microsoft.Xna.Framework.Vector3(x, y, 0);
        }

    }

}