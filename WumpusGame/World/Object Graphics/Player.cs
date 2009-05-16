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
| * PlayerGraphics               Interface |
\*••••••••••••••••••••••••••••••••••••••••*/

using WumpusGame.World.Modules;
using InteractionEngine.Constructs;
using System.Collections.Generic;

namespace WumpusGame.World.Graphics
{

    public interface PlayerGraphics : InteractionEngine.Client.Graphics
    {

        /**
         * Called by Pit and Bat when you are falling / in the air.
         * This one will probably be empty for the text version...
         * As for the graphical version, it'll have you struggling... probably just flailing your legs.
         */
        void onDanger();

        /**
         * This one will probably just be like "YOU ARE DEAD!!" for the text version.
         * As for the graphical version, it'll make you fall down DRAMATICALLY.
         */
        void onDeath();

        /**
         * This one will probably just be like "YOU WIN!!" for the text version...
         * As for the graphical version, it'll have you a do a victory dance or something. At the very least, a "peace" sign.
         */
        void onWumpusKill();

    }

}