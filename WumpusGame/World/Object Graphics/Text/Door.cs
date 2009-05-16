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
| * DoorTextGraphics                 Class |
\*••••••••••••••••••••••••••••••••••••••••*/

using WumpusGame.World.Modules;
using InteractionEngine.Constructs;
using System.Collections.Generic;
using InteractionEngine.GameWorld;
using InteractionEngine.Client.Text;

namespace WumpusGame.World.Graphics
{

    public class DoorTextGraphics : DoorGraphics
    {

        public void onDraw() { }

        /// <summary>
        /// Leave the current Room and enter a new one.
        /// </summary>
        /// <param name="direction">The corner of the Room where the door is located at. This is neccessary to know where the Player needs to be made to walk to.</param>
        /// <param name="room">The new Room that is being walked to. Neccessary as a parameter to Playable.getLocation().move().</param>
        /// <param name="player">The Player that is moving. Neccessary to call PlayerGraphics.onMove() and also to call Playable.getLocation().move() when neccessary.</param>
        public void onDoorOpen(Player player, Room room, int direction) {
            switch (direction) {
                case Room.NORTH:
                    ((UserInterfaceText)GameWorld.userInterface).println("You moved north.");
                    break;
                case Room.NORTHEAST:
                    ((UserInterfaceText)GameWorld.userInterface).println("You moved northeast.");
                    break;
                case Room.NORTHWEST:
                    ((UserInterfaceText)GameWorld.userInterface).println("You moved northwest.");
                    break;
                case Room.SOUTH:
                    ((UserInterfaceText)GameWorld.userInterface).println("You moved south.");
                    break;
                case Room.SOUTHEAST:
                    ((UserInterfaceText)GameWorld.userInterface).println("You moved southeast.");
                    break;
                case Room.SOUTHWEST:
                    ((UserInterfaceText)GameWorld.userInterface).println("You moved southwest.");
                    break;
            }
        }

        public void loadContent() {
        }

    }

}