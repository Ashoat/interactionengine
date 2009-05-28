/*••••••••••••••••••••••••••••••••••••••••*\
| NTK+                                     |
| (C) Copyright Bluestone Coding 2009      |
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
| GAME OBJECTS                             |
| * FrameRateCounter                 Class |
\*••••••••••••••••••••••••••••••••••••••••*/

using InteractionEngine.Constructs;
using NTKPlusGame.World.Modules;
using InteractionEngine.UserInterface;
using InteractionEngine;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using InteractionEngine.UserInterface.ThreeDimensional;

namespace NTKPlusGame.World {

    /**
     * Displays frame rates.
     */
    public class FrameRateCounter : GameObject, Graphable2D {

        #region FACTORY

        /// <summary>
        /// All GameObjects need a parameterless constructor for calling by GameObject.createGameObject() and GameObject.createFromUpdate().
        /// NEVER CALL THIS! This constructor is exclusively for use by the InteractionEngine. If anyone else calls it things will break.
        /// If you want to construct this object, use GameObject.createGameObject(LoadRegion).
        /// </summary>
        public FrameRateCounter() {
        }

        // The classHash, a unique identifying string for the class. Hmm, wow, that's kind of redundant, isn't that? C# already provides such a function through reflection. Oh well.
        // Used for the factory methods called when the client receives a CREATE_NEW_OBJECT update from the server computer.
        public const string realHash = "FrameRateCounter";
        public override string classHash {
            get { return realHash; }
        }

        /// <summary>
        /// The static constructor. Adds the class's factory method to the GameObject factoryList when the class is first loaded.
        /// </summary>
        static FrameRateCounter() {
            GameObject.factoryList.Add(realHash, new GameObjectFactory(GameObject.createFromUpdate<FrameRateCounter>));
        }

        #endregion

        FrameRateCounterGraphics graphics = new FrameRateCounterGraphics();

        public Location getLocation() {
            return null;
        }

        public Graphics getGraphics() {
            return graphics;
        }
        public Graphics2D getGraphics2D() {
            return graphics;
        }

        /// <summary>
        /// Constructs a new FrameRateCounter.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        public override void construct() {
        }

    }

    public class FrameRateCounterGraphics : Graphics2D {

        SpriteFont spriteFont;

        float stringWidth;

        const int numDraws = 8;
        TimeSpan[] drawHistory = new TimeSpan[numDraws];
        int lastDrawIndex = 0;
        TimeSpan lastFrameRateCalculation;
        double frameRate;

        public void onDraw() {
            if (Engine.userInterface.getGraphicsDevice() != null) this.loadContent();

            drawHistory[++lastDrawIndex % numDraws] = Engine.gameTime.TotalRealTime;

            if ((Engine.gameTime.TotalRealTime - lastFrameRateCalculation).TotalMilliseconds > 400) {
                lastFrameRateCalculation = Engine.gameTime.TotalRealTime;

                TimeSpan time = drawHistory[lastDrawIndex % numDraws] - drawHistory[(lastDrawIndex + 1) % numDraws];

                frameRate = numDraws / time.TotalSeconds;

                frameRate = System.Math.Round(10 * frameRate) / 10;
            }

            string fps = string.Format("fps: {0}", frameRate);
            if (Engine.gameTime.IsRunningSlowly) fps += ", slow";
            SpriteBatch spriteBatch = ((UserInterface3D)Engine.userInterface).spriteBatch;

            Viewport view = UserInterface3D.graphicsDevice.Viewport;

            float rotation = 0;
            float scale = 1;
            Vector2 origin = Vector2.Zero;
            float whiteLayer = 0.00101f;
            float blackLayer = 0.00102f;
            Vector2 whitePosition = new Vector2(view.Width - stringWidth, view.Height - 20 - 2 * spriteFont.LineSpacing);
            Vector2 blackPosition = whitePosition + Vector2.One;
            Vector2 whiteCountPosition = whitePosition + new Vector2(0, spriteFont.LineSpacing);
            Vector2 blackCountPosition = whiteCountPosition + Vector2.One;

            spriteBatch.DrawString(spriteFont, fps, blackPosition, Color.Black, rotation, origin, scale, SpriteEffects.None, blackLayer);
            spriteBatch.DrawString(spriteFont, fps, whitePosition, Color.White, rotation, origin, scale, SpriteEffects.None, whiteLayer);
            spriteBatch.DrawString(spriteFont, lastDrawIndex + "", blackCountPosition, Color.Black, rotation, origin, scale, SpriteEffects.None, blackLayer);
            spriteBatch.DrawString(spriteFont, lastDrawIndex + "", whiteCountPosition , Color.White, rotation, origin, scale, SpriteEffects.None, whiteLayer);

        }

        public float LayerDepth {
            get { return 0.00102f; }
        }

        public Vector3? intersectionPoint(double x, double y) {
            return null;
        }

        public bool Visible {
            get { return true; }
            set { }
        }

        public void loadContent() {
            this.spriteFont = UserInterface3D.content.Load<SpriteFont>("SpriteFont1");
            this.stringWidth = spriteFont.MeasureString("fps: 00.0, slow").X + 20;
        }


    }

}