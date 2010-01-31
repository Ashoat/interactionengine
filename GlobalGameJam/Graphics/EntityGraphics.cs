using InteractionEngine.UserInterface.TwoDimensional;
using GlobalGameJam.GameObjects;
using InteractionEngine;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;

namespace GlobalGameJam.Graphics {

    public class EntityGraphics : Graphics2D {

        private const float ENTITY_DEPTH = 0.2f;
        private Entity entity;
       
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
        public EntityGraphics(Entity gameObject, string textureName) {
            this.entity = gameObject;
            this.textureName = textureName;
            if (UserInterface2D.graphicsDevice != null) this.loadContent();
            this.LayerDepth = ENTITY_DEPTH;
        }

        public EntityGraphics(Entity gameObject) {
            this.entity = gameObject;
            this.LayerDepth = ENTITY_DEPTH;
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

        public void setTexture(string texture) {
            this.TextureName = texture;
        }

        public void setPosition(int x, int y) {
            entity.getLocation().Position = new Microsoft.Xna.Framework.Vector3(x, y, 0);
        }

        public Vector3? intersectionPoint(double x, double y) {
            return null;
        }

        public virtual void onDraw() {
            if (this.texture == null || !this.visible) return;
            SpriteBatch spriteBatch = ((UserInterface2D)InteractionEngine.Engine.userInterface).spriteBatch;
            Vector3 position3 = this.entity.getLocation().Position;
            Vector2 position = new Vector2(position3.X, position3.Y);
            float rotationDegrees = this.entity.getLocation().yaw;
            Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);
            spriteBatch.Draw(texture, position + origin, (Rectangle?)null, Color.White, (float)Math.PI / 180 * rotationDegrees,
                origin, scale, SpriteEffects.None, layerDepth);
        }

        public virtual void loadContent() {
            if (textureName == null || textureName.Length == 0) return;
            texture = UserInterface2D.content.Load<Texture2D>(textureName);
        }

    }

}