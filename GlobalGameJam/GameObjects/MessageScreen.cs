
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
    public class MessageScreen : GameObject, Graphable2D {

        #region FACTORY

        /// <summary>
        /// All GameObjects need a parameterless constructor for calling by GameObject.createGameObject() and GameObject.createFromUpdate().
        /// NEVER CALL THIS! This constructor is exclusively for use by the InteractionEngine. If anyone else calls it things will break.
        /// If you want to construct this object, use GameObject.createGameObject(LoadRegion).
        /// </summary>
        public MessageScreen() {
        }

        // The classHash, a unique identifying string for the class. Hmm, wow, that's kind of redundant, isn't that? C# already provides such a function through reflection. Oh well.
        // Used for the factory methods called when the client receives a CREATE_NEW_OBJECT update from the server computer.
        public const string realHash = "MessageScreen";
        public override string classHash {
            get { return realHash; }
        }

        /// <summary>
        /// The static constructor. Adds the class's factory method to the GameObject factoryList when the class is first loaded.
        /// </summary>
        static MessageScreen() {
            GameObject.factoryList.Add(realHash, new GameObjectFactory(GameObject.createFromUpdate<MessageScreen>));
        }

        #endregion

        MessageScreenGraphics graphics;

        const int numTicks = 8;
        TimeSpan[] tickHistory = new TimeSpan[numTicks];
        int lastTickIndex = 0;
        TimeSpan lastTickRateCalculation;
        double tickRate;
        internal bool visible;

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
        /// Constructs a new MessageScreen.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        public override void construct() {
            graphics = new MessageScreenGraphics(this);
        }

        private string text;
        public string Text {
            get { return text; }
            set { text = value; }
        }

        private string texture;
        public string Texture { 
            get { return texture; }
            set { texture = value; }
        }

        public void close() {
            graphics.Visible = false;
        }

        public void open() {
            graphics.Visible = true;
        }
    }

    public class MessageScreenGraphics : Graphics2D {

        MessageScreen gameObject;
        SpriteFont spriteFont;
        Texture2D losingTexture;
        string lostText;
        int textWidth;
        float stringWidth;

        public MessageScreenGraphics(MessageScreen gameObject) {
            this.gameObject = gameObject;
        }

        public void onDraw() {
            if (Engine.userInterface.getGraphicsDevice() != null) this.loadContent();

            SpriteBatch spriteBatch = ((UserInterface2D)Engine.userInterface).spriteBatch;

            Viewport view = UserInterface2D.graphicsDevice.Viewport;

           
            Vector2 origin = Vector2.Zero;
            
            
            int textureWidth = losingTexture.Width;
            int textureHeight = losingTexture.Height;
            Rectangle textRectangle = new Rectangle((view.Width-textureWidth)/2,(view.Height-textureHeight)/2,textureWidth, textureHeight);
            Vector2 textpos = new Vector2((view.Width - textWidth) / 2, view.Height / 2 + 30);

            if (visible) {
                //spriteBatch.Draw(losingTexture, textRectangle, Color.White);
                spriteBatch.Draw(losingTexture, textRectangle, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.00001f);
                //spriteBatch.DrawString(spriteFont, lostText, textpos, Color.White, rotation, origin, scale, SpriteEffects.None, 0.000000000001f);
                //spriteBatch.DrawString(spriteFont, lostText, textpos+new Vector2(2.0f), Color.Black, rotation, origin, scale, SpriteEffects.None, 0f);
            }
        }

        public float LayerDepth {
            get { return 0.0000000102f; }
        }

        public Vector3? intersectionPoint(double x, double y) {
            return null;
        }

        private bool visible;
        public bool Visible {
            get { return visible; }
            set { visible = value; }
        }

        public void loadContent() {
            this.spriteFont = UserInterface2D.content.Load<SpriteFont>("MenuFont");
            this.losingTexture = UserInterface2D.content.Load<Texture2D>(gameObject.Texture);
            this.lostText = gameObject.Text;
            this.textWidth = (int)spriteFont.MeasureString(lostText).X;
        }


    }

}
