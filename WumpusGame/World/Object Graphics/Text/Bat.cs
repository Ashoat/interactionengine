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
| * BatTextGraphics                  Class |
\*••••••••••••••••••••••••••••••••••••••••*/

using WumpusGame.World.Modules;
using InteractionEngine.Constructs;
using System.Collections.Generic;
using InteractionEngine.GameWorld;
using InteractionEngine.Client.Text;

namespace WumpusGame.World.Graphics {

    /**
     * The Bat. It's this big kind-hearted creature that tries to peacefully remove cruel hunters from the cave in order to save the poor Wumpus.
     */
    public class BatTextGraphics : BatGraphics {

        public void onDraw() { }

        /// <summary>
        /// This one will probably echo something for the text version...
        /// As for the graphical version, it'll have a Bat pick you up and take you to a new Room.
        /// Then, it'll make the Bat magically dissapear. We could show the User which direction it leaves in, but that'll reveal where it actually is... so, I'd rather not.
        /// </summary>
        /// <param name="player">The Player entering the Room.</param>
        /// <param name="randomRoom">A random Room to send the Player to.</param>
        public void onPlayerEnterRoom(WumpusGame.World.Player player, Room randomRoom)
        {
            ((UserInterfaceText)GameWorld.userInterface).println("OHEMGEE! You entered a room with BATMAN!");
        }
        
        public void loadContent() {
        }

    }

}