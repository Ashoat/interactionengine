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
| * PlayerTextGraphics               Class |
\*••••••••••••••••••••••••••••••••••••••••*/

using WumpusGame.World.Modules;
using InteractionEngine.Constructs;
using System.Collections.Generic;
using InteractionEngine.GameWorld;
using InteractionEngine.Client.Text;

namespace WumpusGame.World.Graphics
{

    public class PlayerTextGraphics : PlayerGraphics
    {

        public void onDraw() { }

        /**
         * Called by Pit and Bat when you are falling / in the air.
         * This one will probably be empty for the text version...
         * As for the graphical version, it'll have you struggling... probably just flailing your legs.
         */
        public void onDanger() { }

        /**
         * This one will probably just be like "YOU ARE DEAD!!" for the text version.
         * As for the graphical version, it'll make you fall down DRAMATICALLY.
         */
        public void onDeath()
        {
            ((UserInterfaceText)GameWorld.userInterface).println("Looks like you're dead. Sucks.");
        }

        /**
         * This one will probably just be like "YOU WIN!!" for the text version...
         * As for the graphical version, it'll have you a do a victory dance or something. At the very least, a "peace" sign.
         */
        public void onWumpusKill()
        {
            ((UserInterfaceText)GameWorld.userInterface).println("You killed the Wumpus!! YAY!!!!!");
        }

        public void loadContent() {
        }

    }

}