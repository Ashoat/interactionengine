﻿using InteractionEngine.UserInterface.TwoDimensional;

namespace TestNetworkGame.Graphics.TwoDimensional {

    public class GameFieldGraphics2D : Graphics2D, GameFieldGraphics {

        /// <summary>
        /// Construct!
        /// </summary>
        /// <param name="gameObject">GameObject!</param>
        public GameFieldGraphics2D(InteractionEngine.Constructs.GameObject gameObject)
            : base(gameObject) {
        }

        /// <summary>
        /// Blah!
        /// </summary>
        public override void onDraw() {
            if(!hasTexture()) loadTexture("Field");
            changePosition(50, 50);
            base.onDraw();
        }

        /// <summary>
        /// Neh!
        /// </summary>
        public override void loadContent() {
        }

    }
}