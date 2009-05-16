/*••••••••••••••••••••••••••••••••••••••••*\
| Wumpus Game                              |
| (C) Copyright Bluestone Coding 2008      |
|••••••••••••••••••••••••••••••••••••••••••|
| Built using the Interaction Engine       |
| (C) Copyright Bluestone Coding 2008      |
|••••••••••••••••••••••••••••••••••••••••••|
|           __    ___ ___  ___             |
|          /++\  | _ ) __|/ __|            |
|          \++/  | _ \__ \ (__             |
|           \/   |___/___/\___|            |
|                                          |
|••••••••••••••••••••••••••••••••••••••••••|
| GRAPHICS                                 |
| * RandomCaveHoboWitchDoctor2DGraphics Class |
\*••••••••••••••••••••••••••••••••••••••••*/

using WumpusGame.World.Modules;
using InteractionEngine.Constructs;
using System.Collections.Generic;
using InteractionEngine.GameWorld;
using InteractionEngine.Client.Text;
using InteractionEngine.Client.TwoDimensional;

namespace WumpusGame.World.Graphics {

    public class RandomCaveHoboWitchDoctor2DGraphics : Graphics2D {

        public RandomCaveHoboWitchDoctor2DGraphics(InteractionEngine.Constructs.GameObject gameObject)
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
            base.loadTexture("RandomCaveHoboWitchDoctor");
            changePosition(580, 600);
        }

    }

}