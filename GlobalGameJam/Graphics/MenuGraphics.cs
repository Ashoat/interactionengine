using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InteractionEngine.UserInterface.TwoDimensional;
using Microsoft.Xna.Framework.Graphics;
using InteractionEngine;
using Microsoft.Xna.Framework;
using GlobalGameJam.GameObjects;

namespace GlobalGameJam.Graphics {
    class MenuGraphics: Graphics2D {
        public List<String> menuStrings;
        SpriteFont font;
        float layerDepth;
        bool visible;
        float menuStringWidth; // This is the largest string in the menu
        float menuStringHeight;
        public int activeMenuItemIndex;
        #region Graphics2D Members

        public float LayerDepth {
            get { return layerDepth; }
        }

        public bool Visible {
            get {
                return visible;
            }
            set {
                visible = value;
            }
        }

        public Microsoft.Xna.Framework.Vector3? intersectionPoint(double x, double y) {
            return null;
        }

        #endregion

        #region Graphics Members

        public void loadContent() {
            font = UserInterface2D.content.Load<SpriteFont>("MenuFont");
            menuStringWidth = font.MeasureString(menuStrings[0]).X+20; //This should be whichever one is longest
            menuStringHeight = font.MeasureString(menuStrings[0]).Y+20;
        }

        public void onDraw() {
            if (!Visible) {
                return;
            }
            if (Engine.userInterface.getGraphicsDevice() != null) this.loadContent();
            Viewport view = Engine.userInterface.getGraphicsDevice().Viewport;

            Texture2D menu = UserInterface2D.content.Load<Texture2D>("menu");
            
            SpriteBatch sb = ((UserInterface2D)Engine.userInterface).spriteBatch;
            //sb.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.BackToFront, SaveStateMode.SaveState)
            
            //sb.Draw(menu, Vector2.One, new Rectangle(0,0,view.Width, view.Height),Color.White);
            sb.Draw(menu, Vector2.Zero, null, Color.White, 0.0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.00001f);
            for (int i = 0; i < menuStrings.Count; i++) {
                String text = menuStrings[i];
                Vector2 position = new Vector2((view.Width - menuStringWidth) / 2, view.Height / 2 + menuStringHeight * (i - menuStrings.Count / 2.0f));
                sb.DrawString(font, text, position, Color.White, 0.0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
                if (i == activeMenuItemIndex) {
                    sb.Draw(
                        UserInterface2D.content.Load<Texture2D>("skunk_0"),
                        new Rectangle((int)(position.X - 20), 
                            (int)(position.Y), 32, 32), 
                            null, Color.White, 
                            (float)Math.PI/2.0f,
                            Vector2.Zero,
                            SpriteEffects.None,
                            0f );
                }
            }
            
            
        }

        #endregion
    }
}
