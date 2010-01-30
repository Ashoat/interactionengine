using InteractionEngine.UserInterface.TwoDimensional;
using GlobalGameJam.GameObjects;

namespace GlobalGameJam.Graphics {

    public class EntityGraphics : Graphics2D {

        private Entity entity;
        private string texture;

        /// <summary>
        /// Construct!
        /// </summary>
        /// <param name="gameObject">GameObject!</param>
        public EntityGraphics(Entity entity)
            : base(entity) {
            this.entity = entity;
        }

        public void setTexture(string texture) {
            this.texture = texture;
        }

        /// <summary>
        /// Construct!
        /// </summary>
        /// <param name="gameObject">GameObject!</param>
        public EntityGraphics(Entity entity, string texture) : base(entity) {
            this.entity = entity;
            this.texture = texture;
        }

        public void setPosition(int x, int y) {
            base.xPos.value = x;
            base.yPos.value = y;
            base.loadBounds();
        }

        /// <summary>
        /// Blah!
        /// </summary>
        public override void onDraw() {
            if (!hasTexture()) loadTexture(texture);
        }

        /// <summary>
        /// Neh!
        /// </summary>
        public override void loadContent() {
        }

    }

}