using InteractionEngine.UserInterface.TwoDimensional;
using InteractionEngine.Constructs;
using TestNetworkGame.Logic;
using InteractionEngine.Networking;
using InteractionEngine;

namespace TestNetworkGame.Graphics.TwoDimensional {

    public class JoinGraphics2D : Graphics2D, ButtonGraphics {

        /// <summary>
        /// Construct!
        /// </summary>
        /// <param name="gameObject">GameObject!</param>
        public JoinGraphics2D(GameObject gameObject)
            : base(gameObject) {
        }

        private bool recalc = false;

        public void setPosition(int x, int y) {
            base.xPos.value = x;
            base.yPos.value = y;
            recalc = true;
        }

        private const int offset = 1;

        public void onClick() {
            xPos.value = xPos.value + offset;
            yPos.value = yPos.value + offset;
            Engine.addEvent(new InteractionEngine.EventHandling.Event(this.gameObject.id, "returnAfterClick", null));
        }

        public void returnAfterClick(Client client, object parameter) {
            xPos.value = xPos.value - offset;
            yPos.value = yPos.value - offset;
        }

        /// <summary>
        /// Blah!
        /// </summary>
        public override void onDraw() {
            if (recalc) {
                loadTexture("Join");
                calculateBoundsFromTexture();
                recalc = false;
            }
            base.onDraw();
        }

        /// <summary>
        /// Neh!
        /// </summary>
        public override void loadContent() {
        }

    }
}