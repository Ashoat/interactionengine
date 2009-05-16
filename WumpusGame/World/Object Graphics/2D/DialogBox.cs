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
| * DialogBoxGraphics                Class |
\*••••••••••••••••••••••••••••••••••••••••*/

using WumpusGame.World.Modules;
using InteractionEngine.Constructs;
using System.Collections.Generic;
using InteractionEngine.GameWorld;
using InteractionEngine.Client.Text;
using InteractionEngine.Client.TwoDimensional;
using Microsoft.Xna.Framework.Graphics;
using InteractionEngine.Constructs.Datatypes;
using Microsoft.Xna.Framework;

namespace WumpusGame.World.Graphics {

    public class DialogBox2DGraphics : Graphics2D {

        SpriteFont font;
        UpdatableGameObject<DialogBox> box;

        public DialogBox2DGraphics(DialogBox gameObject)
            : base(gameObject) {
            box = new UpdatableGameObject<DialogBox>(gameObject, gameObject);
        }

        /// <summary>
        /// Blah!
        /// </summary>
        public override void onDraw() {
            if (box.value.discussion.value != null && box.value.discussion.value.getDisplaying()) {
                base.onDraw();
                SpriteBatch spriteBatch = ((UserInterface2D)GameWorld.userInterface).spriteBatch;
                int amountOfStringToDraw = 0;
                string textRemaining = box.value.discussion.value.getMessage();
                int heightMod = 0;
                while (textRemaining.Length > 0) {
                    amountOfStringToDraw = getLargestWholeWordSubstringIndex(textRemaining, font, 880);
                    spriteBatch.DrawString(font, textRemaining.Substring(0, amountOfStringToDraw), new Vector2(corners[0].X+10, corners[0].Y+10 + heightMod*font.LineSpacing), Color.Black);
                    if (amountOfStringToDraw == textRemaining.Length) return;
                    textRemaining = textRemaining.Substring(amountOfStringToDraw);
                    heightMod++;
                }
            }
        }

        private int getLargestWholeWordSubstringIndex(string text, SpriteFont font, int pixels) {
            int prevIndex = -1;
            int index = 0;
            while (index >= 0 && font.MeasureString(text.Substring(0,index)).X < pixels) {
                prevIndex = index;
                index = text.IndexOf(' ', prevIndex+1);
                if (index < 0) return text.Length;
            }
            return prevIndex;
        }

        /// <summary>
        /// Neh!
        /// </summary>
        public override void loadContent() {
            loadTexture("dialogbox");
            changePosition(20, 850);
            font = GameWorld.game.Content.Load<SpriteFont>("SpriteFont1");
        }

    }

    public class DialogButton2DGraphics : Graphics2D {

        SpriteFont font;
        UpdatableGameObject<DialogButton> button;
        int number;

        public DialogButton2DGraphics(DialogButton gameObject, int number)
            : base(gameObject) {
            button = new UpdatableGameObject<DialogButton>(gameObject, gameObject);
            this.number = number;
            changePosition(30, 920 + number * 20);
        }

        /// <summary>
        /// Blah!
        /// </summary>
        public override void onDraw() {
            if (button.value.name.value != null && button.value.dialogBox.value.discussion.value.getDisplaying()) {
                base.onDraw();
                ((UserInterface2D)GameWorld.userInterface).spriteBatch.DrawString(font, button.value.name.value, new Vector2(corners[0].X + 10, corners[0].Y + 2), Color.Black);
            }
        }

        /// <summary>
        /// Neh!
        /// </summary>
        public override void loadContent() {
            font = GameWorld.game.Content.Load<SpriteFont>("SpriteFont1");
            base.loadTexture("Button");
        }

    }
}