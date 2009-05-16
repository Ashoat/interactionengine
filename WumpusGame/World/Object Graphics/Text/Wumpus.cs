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
| * WumpusTextGraphics               Class |
\*••••••••••••••••••••••••••••••••••••••••*/

using WumpusGame.World.Modules;
using InteractionEngine.Constructs;
using System.Collections.Generic;
using InteractionEngine.GameWorld;
using InteractionEngine.Client.Text;

namespace WumpusGame.World.Graphics
{

    public class WumpusTextGraphics : WumpusGraphics
    {

        public void onDraw() { }

        public void onEnterRoom()
        {
            ((UserInterfaceText)GameWorld.userInterface).println("OH SHIZNIT! YOU ENTERED A ROOM WITH A _*WUMPUS*_!!!");
        }

        public void onDeath()
        {
            ((UserInterfaceText)GameWorld.userInterface).println("The Wumpus falls down on the group, screaming in pain and you stab him multiple times with a sharp knife in places that aren't fatal, just to prolong his torture. Wow... you suck.");
        }

        public void onUserTriviaFail()
        {
            ((UserInterfaceText)GameWorld.userInterface).println("Wow, how did you fail those trivia questions? It was just Pokemon... anyways, you accidentally approach a ledge after getting beaten by the Wumpus, who fought only in self defense. You start falling down and the Wumpus offers you his hand, but you reject it, falling into the abyss below. You lose. Hah.");
        }

        public void loadContent() {
        }

    }

}