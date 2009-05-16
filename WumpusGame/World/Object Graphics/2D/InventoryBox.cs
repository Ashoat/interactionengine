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
| * InventoryBox2DGraphics                    Class |
\*••••••••••••••••••••••••••••••••••••••••*/

using WumpusGame.World.Modules;
using InteractionEngine.Constructs;
using System.Collections.Generic;
using InteractionEngine.GameWorld;
using InteractionEngine.Client.Text;
using InteractionEngine.Client.TwoDimensional;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace WumpusGame.World.Graphics {

    public class InventoryBox2DGraphics : Graphics2D {

        Inventory inventory;
        Texture2D gold;
        Texture2D arrows;
        SpriteFont font;

        public InventoryBox2DGraphics(InteractionEngine.Constructs.GameObject gameObject, Inventory inventory)
            : base(gameObject) {
            this.inventory = inventory;
        }

        /// <summary>
        /// Blah!
        /// </summary>
        public override void onDraw() {
            base.onDraw();
            ((UserInterface2D)GameWorld.userInterface).spriteBatch.Draw(gold, new Vector2(corners[0].X + 10, corners[0].Y + 10),
                null, Color.White);
            Gold goldy = inventory.getItem<Gold>();
            int golds = goldy == null ? 0 : goldy.getItem().getAmount();
            ((UserInterface2D)GameWorld.userInterface).spriteBatch.DrawString(font, golds + " Gold coins", new Vector2(corners[0].X + 10, corners[0].Y + 150), Color.Black);
            ((UserInterface2D)GameWorld.userInterface).spriteBatch.Draw(arrows, new Vector2(corners[0].X + 10, corners[0].Y + 180),
                null, Color.White);
            Arrow arrow = inventory.getItem<Arrow>();
            int arrowy = arrow == null ? 0 : arrow.getItem().getAmount();
            ((UserInterface2D)GameWorld.userInterface).spriteBatch.DrawString(font, arrowy + " Arrows", new Vector2(corners[0].X + 10, corners[0].Y + 330), Color.Black);
            ((UserInterface2D)GameWorld.userInterface).spriteBatch.DrawString(font, "Click here and then a door to shoot an arrow.", new Vector2(corners[0].X + 10, corners[0].Y + 360), Color.Black);
        }

        /// <summary>
        /// Neh!
        /// </summary>
        public override void loadContent() {
            gold = GameWorld.game.Content.Load<Texture2D>("GoldIcon");
            arrows = GameWorld.game.Content.Load<Texture2D>("ArrowIcon");
            font = GameWorld.game.Content.Load<SpriteFont>("SpriteFont1");
            base.loadTexture("Inventory");
            changePosition(1020, 100);
        }

    }

}