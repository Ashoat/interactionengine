﻿/*••••••••••••••••••••••••••••••••••••••••*\
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
| * DoorGraphics                 Interface |
\*••••••••••••••••••••••••••••••••••••••••*/

using WumpusGame.World.Modules;
using InteractionEngine.Constructs;
using System.Collections.Generic;

namespace WumpusGame.World.Graphics {

    public interface DoorGraphics : InteractionEngine.Client.Graphics {

        void onDoorOpen(Player player, Room room, int direction);

    }

}