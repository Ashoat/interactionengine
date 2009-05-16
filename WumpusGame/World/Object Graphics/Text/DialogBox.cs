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
| * DialogBoxTextGraphics            Class |
\*••••••••••••••••••••••••••••••••••••••••*/

using WumpusGame.World.Modules;
using InteractionEngine.Constructs;
using System.Collections.Generic;
using InteractionEngine.GameWorld;
using InteractionEngine.Client.Text;

namespace WumpusGame.World.Graphics {

    public class DialogBoxTextGraphics : InteractionEngine.Client.Graphics {

        DialogBox dialogBox;

        public DialogBoxTextGraphics(DialogBox box) {
            this.dialogBox = box;
        }

        public void onDraw() {
            ((UserInterfaceText)GameWorld.userInterface).println(dialogBox.discussion.value.getMessage());
        }

        public void loadContent() {
        }

    }

}