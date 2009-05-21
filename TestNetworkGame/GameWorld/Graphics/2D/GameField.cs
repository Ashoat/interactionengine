using InteractionEngine.UserInterface.TwoDimensional;

namespace TestNetworkGame.Graphics.TwoDimensional {

    public class GameFieldGraphics2D : Graphics2D, GameFieldGraphics {

        public GameFieldGraphics2D(InteractionEngine.Constructs.GameObject gameObject)
            : base(gameObject) {
        }

        /// <summary>
        /// Blah!
        /// </summary>
        public override void onDraw() {
            base.onDraw();
        }

        /// <summary>
        /// Neh!
        /// </summary>
        public override void loadContent() {
            loadTexture("Field");
            changePosition(200, 400);
        }

    }
}