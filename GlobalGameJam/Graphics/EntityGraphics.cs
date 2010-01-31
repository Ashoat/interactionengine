using InteractionEngine.UserInterface.TwoDimensional;
using GlobalGameJam.GameObjects;
using InteractionEngine;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;

namespace GlobalGameJam.Graphics {

    public class EntityGraphics : Graphics2DTexture {

        private const float ENTITY_DEPTH = 0.2f;
        private Entity entity;

        /// <summary>
        /// Loads the sprite from XNA's ContentPipeline.
        /// </summary>
        /// <param name="textureFileName">The filename of this GameObject's texture.</param>
        public EntityGraphics(Entity gameObject, string textureName) : base(gameObject, textureName){
            this.entity = gameObject;
            this.LayerDepth = ENTITY_DEPTH;
        }

        public EntityGraphics(Entity gameObject)
            : base(gameObject)
        {
            this.entity = gameObject;
            this.LayerDepth = ENTITY_DEPTH;
        }

        public void setTexture(string texture) {
            this.TextureName = texture;
        }

        public void setPosition(int x, int y) {
            entity.getLocation().Position = new Microsoft.Xna.Framework.Vector3(x, y, 0);
        }

    }

}