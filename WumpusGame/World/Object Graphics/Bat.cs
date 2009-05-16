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
| * BatGraphics                  Interface |
\*••••••••••••••••••••••••••••••••••••••••*/

using WumpusGame.World.Modules;
using InteractionEngine.Constructs;
using System.Collections.Generic;

namespace WumpusGame.World.Graphics {

    /**
     * The Bat. It's this big kind-hearted creature that tries to peacefully remove cruel hunters from the cave in order to save the poor Wumpus.
     */
    public interface BatGraphics : InteractionEngine.Client.Graphics {

        /// <summary>
        /// This method is called when a player enters a room with a Bat.
        /// In the text version, this will just echo "You entered a room with a Bat!"
        /// In the graphics version, this will start an animation that onDraw() will follow through with that will...
        ///    - Move the Bat (center left) to the Player (center right).
        ///    - Move the Bat with the Player under it to an arbitrary corner. 
        ///    - Load a new Room/LoadRegion.
        ///    - Move the Bat with the Player under it from an arbitrary corner to the center right.
        ///    - Move the Bat from the center of the Room to an arbitrary corner.
        ///    - Make the Bat disappear.
        /// </summary>
        /// <param name="player">The Player entering the Room.</param>
        /// <param name="randomRoom">A random Room to send the Player to.</param>
        void onPlayerEnterRoom(WumpusGame.World.Player player, Room randomRoom);

    }

}