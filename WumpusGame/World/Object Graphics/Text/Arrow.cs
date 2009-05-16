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
| * ArrowTextGraphics                Class |
\*••••••••••••••••••••••••••••••••••••••••*/

using WumpusGame.World.Modules;
using InteractionEngine.Constructs;
using System.Collections.Generic;
using InteractionEngine.GameWorld;
using InteractionEngine.Client.Text;

namespace WumpusGame.World.Graphics
{

    public class ArrowTextGraphics : ArrowGraphics {

        public void onDraw() { }

        public void onArrowShoot(int direction) {
            switch (direction)
            {
                case Room.NORTH:
                    ((UserInterfaceText)GameWorld.userInterface).println("You shot an arrow north!");
                    break;
                case Room.NORTHEAST:
                    ((UserInterfaceText)GameWorld.userInterface).println("You shot an arrow northeast!");
                    break;
                case Room.NORTHWEST:
                    ((UserInterfaceText)GameWorld.userInterface).println("You shot an arrow northwest!");
                    break;
                case Room.SOUTH:
                    ((UserInterfaceText)GameWorld.userInterface).println("You shot an arrow south!");
                    break;
                case Room.SOUTHEAST:
                    ((UserInterfaceText)GameWorld.userInterface).println("You shot an arrow southeast!");
                    break;
                case Room.SOUTHWEST:
                    ((UserInterfaceText)GameWorld.userInterface).println("You shot an arrow southwest!");
                    break;
            }
        }

        public void loadContent() {
        }


    }

}