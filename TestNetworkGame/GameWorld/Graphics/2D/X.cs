using InteractionEngine.UserInterface.TwoDimensional;
using InteractionEngine.Constructs;
using TestNetworkGame.Logic;

namespace TestNetworkGame.Graphics.TwoDimensional {

    public class XGraphics2D : Graphics2D, GamePieceGraphics {

        private X xObject;

        /// <summary>
        /// Construct!
        /// </summary>
        /// <param name="gameObject">GameObject!</param>
        public XGraphics2D(X gameObject)
            : base(gameObject) {
            xObject = gameObject;
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
            if (!hasTexture()) loadTexture("X");
            if (xObject.getGamePiece().display.value == true) base.onDraw();
        }

        /// <summary>
        /// Neh!
        /// </summary>
        public override void loadContent() {
        }

    }
}