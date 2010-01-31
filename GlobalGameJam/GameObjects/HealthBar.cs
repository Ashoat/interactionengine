using InteractionEngine.Constructs;
using InteractionEngine.UserInterface;
using Microsoft.Xna.Framework.Input;
using GlobalGameJam.Graphics;
using InteractionEngine.UserInterface.TwoDimensional;
using Microsoft.Xna.Framework;
using InteractionEngine.Constructs.Datatypes;
using InteractionEngine.Networking;
using InteractionEngine;
using InteractionEngine.EventHandling;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace GlobalGameJam.GameObjects {

    public class HealthBar : GameObject, Graphable2D {

        #region FACTORY

        /// <summary>
        /// All GameObjects need a parameterless constructor for calling by GameObject.createGameObject() and GameObject.createFromUpdate().
        /// NEVER CALL THIS! This constructor is exclusively for use by the InteractionEngine. If anyone else calls it things will break.
        /// If you want to construct this object, use GameObject.createGameObject(LoadRegion).
        /// </summary>
        public HealthBar() {
        }

        // The classHash, a unique identifying string for the class. Hmm, wow, that's kind of redundant, isn't that? C# already provides such a function through reflection. Oh well.
        // Used for the factory methods called when the client receives a CREATE_NEW_OBJECT update from the server computer.
        public const string realHash = "HealthBar";
        public override string classHash {
            get { return realHash; }
        }

        /// <summary>
        /// The static constructor. Adds the class's factory method to the GameObject factoryList when the class is first loaded.
        /// </summary>
        static HealthBar() {
            GameObject.factoryList.Add(realHash, new GameObjectFactory(GameObject.createFromUpdate<HealthBar>));
        }

        #endregion

        public const float LAYER = 0.006f;

        UpdatableGameObject<Map> map;
        public float HealthPercentage {
            get { return (this.map.value != null && this.map.value.getPlayer() != null) ? this.map.value.getPlayer().Health : 100; }
        }

        Location location;
        public Location getLocation() {
            return location;
        }

        HealthBarGraphics graphics;
        public InteractionEngine.UserInterface.Graphics getGraphics() {
            return graphics;
        }
        public Graphics2D getGraphics2D() {
            return graphics;
        }

        public override void construct() {
            this.location = new Location(this);
            this.graphics = new HealthBarGraphics(this, "healthbar");
            this.map = new UpdatableGameObject<Map>(this);
        }

        public void setLocationAndMap(Vector3 position, Map map) {
            this.location.Position = position;
            this.map.value = map;
        }

    }

    public class HealthBarGraphics : Graphics2D {

        // Contains a reference to this Graphics module's GameObject.
        // Used for proper Updatable construction.
        public readonly HealthBar gameObject;
        // Contains texture information for the sprite.
        // Used for drawing the sprite.
        private Microsoft.Xna.Framework.Graphics.Texture2D texture;
        private string textureName;
        private bool visible = true;
        private float scale = 1;
        private float layerDepth = 0;

        /// <summary>
        /// Loads the sprite from XNA's ContentPipeline.
        /// </summary>
        /// <param name="textureFileName">The filename of this GameObject's texture.</param>
        public HealthBarGraphics(HealthBar gameObject, string textureName) {
            this.gameObject = gameObject;
            this.textureName = textureName;
            if (UserInterface2D.graphicsDevice != null) this.loadContent();
        }

        public string TextureName {
            get { return textureName; }
            set {
                this.textureName = value;
                if (UserInterface2D.graphicsDevice != null) this.loadContent();
            }
        }

        public bool Visible {
            get { return visible; }
            set { this.visible = value; }
        }

        public float Scale {
            get { return this.scale; }
            set { this.scale = value; }
        }

        /// <summary>
        /// One is back, zero is front.
        /// </summary>
        public float LayerDepth {
            get { return this.layerDepth; }
            set { this.layerDepth = value; }
        }

        /// <summary>
        /// Draw this Graphics2D onto the SpriteBatch.
        /// </summary>
        public virtual void onDraw() {
            if (this.texture == null || !this.visible) return;
            SpriteBatch spriteBatch = ((UserInterface2D)InteractionEngine.Engine.userInterface).spriteBatch;
            Vector3 position3 = this.gameObject.getLocation().Position;
            int healthBarWidth = (int)(gameObject.HealthPercentage * this.texture.Width / 100);
            Rectangle healthBarDimensions = new Rectangle((int)position3.X, (int)position3.Y, healthBarWidth, this.texture.Height);
            Rectangle sourceDimensions = new Rectangle(0, 0, healthBarWidth, this.texture.Height);
            spriteBatch.Draw(texture, healthBarDimensions, sourceDimensions, Color.White, 0, Vector2.Zero, SpriteEffects.None, HealthBar.LAYER);
        }

        /// <summary>
        /// Returns true if a point is contained within this Graphics2D.
        /// </summary>
        /// <param name="x">The x-coordinate of the point.</param>
        /// <param name="y">The y-coordinate of the point.</param>
        /// <returns>True if the point is within the Graphics2D's boundaries; false otherwise.</returns>
        public Vector3? intersectionPoint(double x, double y) {
            return null;
        }

        /// <summary>
        /// Called during InteractionGame's LoadContent loop.
        /// </summary>
        public virtual void loadContent() {
            if (textureName == null || textureName.Length == 0) return;
            texture = UserInterface2D.content.Load<Texture2D>(textureName);
        }

    }

}