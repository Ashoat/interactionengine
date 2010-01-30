using InteractionEngine.UserInterface.TwoDimensional;
using GlobalGameJam.GameObjects;

namespace GlobalGameJam.Graphics {

    public class EntityGraphics : Graphics2DTexture {

        private const float ENTITY_DEPTH = 0.2f;
        private Entity entity;

        /// <summary>
        /// Construct!
        /// </summary>
        /// <param name="gameObject">GameObject!</param>
        public EntityGraphics(Entity entity) : base(entity) {
            this.entity = entity;
            this.LayerDepth = ENTITY_DEPTH;
        }

        /// <summary>
        /// Construct!
        /// </summary>
        /// <param name="gameObject">GameObject!</param>
        public EntityGraphics(Entity entity, string texture) : base(entity, texture) {
            this.entity = entity;
            this.LayerDepth = ENTITY_DEPTH;
        }

        public void setTexture(string texture) {
            this.TextureName = texture;
        }

        public void setPosition(int x, int y) {
            entity.getLocation().Position = new Microsoft.Xna.Framework.Vector3(x, y, 0);
        }

    }

}