
using InteractionEngine.Constructs;
using InteractionEngine.UserInterface;
using InteractionEngine;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using InteractionEngine.UserInterface.TwoDimensional;
using InteractionEngine.Networking;
using InteractionEngine.EventHandling;

namespace GlobalGameJam.GameObjects {

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

        FrameRateCounterGraphics graphics;

        const int numTicks = 8;
        TimeSpan[] tickHistory = new TimeSpan[numTicks];
        int lastTickIndex = 0;
        TimeSpan lastTickRateCalculation;
        double tickRate;

        public double TickRate {
            get { return tickRate; }
        }

        public Location getLocation() {
            return null;
        }

        public InteractionEngine.UserInterface.Graphics getGraphics() {
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
            graphics = new FrameRateCounterGraphics(this);
            this.addEventMethod("tick", tick);
            Engine.addEvent(new Event(this.id, "tick", null));
        }

        public void tick(Client client, object param) {
            tickHistory[++lastTickIndex % numTicks] = Engine.gameTime.TotalRealTime;

            if ((Engine.gameTime.TotalRealTime - lastTickRateCalculation).TotalMilliseconds > 400) {
                lastTickRateCalculation = Engine.gameTime.TotalRealTime;

                TimeSpan time = tickHistory[lastTickIndex % numTicks] - tickHistory[(lastTickIndex + 1) % numTicks];

                tickRate = numTicks / time.TotalSeconds;

                tickRate = System.Math.Round(10 * tickRate) / 10;
            }
            Engine.addEvent(new Event(this.id, "tick", null));
        }

    }

    public class FrameRateCounterGraphics : Graphics2D {

        FrameRateCounter gameObject;
        SpriteFont spriteFont;

        float stringWidth;

        const int numDraws = 8;
        TimeSpan[] drawHistory = new TimeSpan[numDraws];
        int lastDrawIndex = 0;
        TimeSpan lastFrameRateCalculation;
        double frameRate;

        public FrameRateCounterGraphics(FrameRateCounter gameObject) {
            this.gameObject = gameObject;
        }

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
            fps += ", #" + lastDrawIndex;

            string tps = string.Format("tps: {0}", gameObject.TickRate);
            if (Engine.gameTime.IsRunningSlowly) tps += ", slow";

            SpriteBatch spriteBatch = ((UserInterface2D)Engine.userInterface).spriteBatch;

            Viewport view = UserInterface2D.graphicsDevice.Viewport;

            float rotation = 0;
            float scale = 1;
            Vector2 origin = Vector2.Zero;
            float whiteLayer = 0.0000000101f;
            float blackLayer = 0.0000000102f;
            Vector2 whitePosition = new Vector2(view.Width - stringWidth, view.Height - 20 - 2 * spriteFont.LineSpacing);
            Vector2 blackPosition = whitePosition + Vector2.One;
            Vector2 whiteCountPosition = whitePosition + new Vector2(0, spriteFont.LineSpacing);
            Vector2 blackCountPosition = whiteCountPosition + Vector2.One;

            spriteBatch.DrawString(spriteFont, fps, blackPosition, Color.Black, rotation, origin, scale, SpriteEffects.None, blackLayer);
            spriteBatch.DrawString(spriteFont, fps, whitePosition, Color.White, rotation, origin, scale, SpriteEffects.None, whiteLayer);
            spriteBatch.DrawString(spriteFont, tps, blackCountPosition, Color.Black, rotation, origin, scale, SpriteEffects.None, blackLayer);
            spriteBatch.DrawString(spriteFont, tps, whiteCountPosition, Color.White, rotation, origin, scale, SpriteEffects.None, whiteLayer);

        }

        public float LayerDepth {
            get { return 0.0000000102f; }
        }

        public Vector3? intersectionPoint(double x, double y) {
            return null;
        }

        public bool Visible {
            get { return true; }
            set { }
        }

        public void loadContent() {
            this.spriteFont = UserInterface2D.content.Load<SpriteFont>("MenuFont");
            this.stringWidth = spriteFont.MeasureString("fps: 00.0, slowww").X + 20;
        }


    }

}
