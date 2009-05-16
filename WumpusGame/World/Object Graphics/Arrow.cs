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
| * ArrowGraphics                Interface |
\*••••••••••••••••••••••••••••••••••••••••*/

using WumpusGame.World.Modules;
using InteractionEngine.Constructs;
using System.Collections.Generic;

namespace WumpusGame.World.Graphics
{

    public interface ArrowGraphics : InteractionEngine.Client.Graphics {

        /// <summary>
        /// This method is called when an arrow is shot. It animates the arrow going someplace.
        /// In the text version, this will just echo "You shot an arrow!"
        /// In the graphics version, this will start an animation that onDraw() will follow through with that will...
        ///    - Move the arrow from the center right (where the Player is) to the direction specified.
        ///    - Load a new Room/LoadRegion.
        ///    - Move the arrow from the corner opposite of that specified by direction to the center left, where a Wumpus could possibly be.
        /// </summary>
        /// <param name="direction"></param>
        void onArrowShoot(int direction);

    }

}