using InteractionEngine.UserInterface.TwoDimensional;
using GlobalGameJam.GameObjects;

namespace GlobalGameJam.Graphics {

    public class MapGraphics : Graphics2D {

        private Map map;

        /// <summary>
        /// Construct!
        /// </summary>
        /// <param name="gameObject">GameObject!</param>
        public MapGraphics(Map map)
            : base(map) {
            this.map = map;
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
            if (!hasTexture()) loadTexture("Map");
        }

        /// <summary>
        /// Neh!
        /// </summary>
        public override void loadContent() {
        }

    }

}