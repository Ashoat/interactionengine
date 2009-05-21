using InteractionEngine.UserInterface.TwoDimensional;

namespace TestNetworkGame.Graphics.TwoDimensional {

    public class ClickySpotGraphics2D : Graphics2D, ClickySpotGraphics {

        /// <summary>
        /// Construct!
        /// </summary>
        /// <param name="gameObject">GameObject!</param>
        public ClickySpotGraphics2D(InteractionEngine.Constructs.GameObject gameObject)
            : base(gameObject) {
        }

        public void setPosition(int startingXPos, int startingYPos){
            base.changePosition(startingXPos, startingYPos);
            base.width.value = 100;
            base.height.value = 100;
            base.loadBounds();
        }

        /// <summary>
        /// Blah!
        /// </summary>
        public override void onDraw() {
        }

        /// <summary>
        /// Neh!
        /// </summary>
        public override void loadContent() {
        }

    }
}