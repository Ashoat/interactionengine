using InteractionEngine.UserInterface.TwoDimensional;
using GlobalGameJam.GameObjects;

namespace GlobalGameJam.Graphics {

    public class HUDGraphics : Graphics2D {

        private HUD HUD;

        /// <summary>
        /// Construct!
        /// </summary>
        /// <param name="gameObject">GameObject!</param>
        public HUDGraphics(HUD HUD)
            : base(HUD) {
            this.HUD = HUD;
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
            if (!hasTexture()) loadTexture("HUD");
        }

        /// <summary>
        /// Neh!
        /// </summary>
        public override void loadContent() {
        }

    }

}