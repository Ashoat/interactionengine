using InteractionEngine.UserInterface.TwoDimensional;
using InteractionEngine.Constructs;
using TestNetworkGame.Logic;

namespace TestNetworkGame.Graphics.TwoDimensional {

    public class OGraphics2D : Graphics2D, GamePieceGraphics {

        private O oObject;

        /// <summary>
        /// Construct!
        /// </summary>
        /// <param name="gameObject">GameObject!</param>
        public OGraphics2D(O gameObject)
            : base(gameObject) {
            oObject = gameObject;
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
            if (!hasTexture()) loadTexture("O");
            if (oObject.getGamePiece().display.value == true) base.onDraw();
        }

        /// <summary>
        /// Neh!
        /// </summary>
        public override void loadContent() {
        }

    }
}