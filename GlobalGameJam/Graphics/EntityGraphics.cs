using InteractionEngine.UserInterface.TwoDimensional;
using GlobalGameJam.GameObjects;
using InteractionEngine;
using Microsoft.Xna.Framework;
using System;

namespace GlobalGameJam.Graphics {

    public class EntityGraphics : Graphics2DTexture {

        private const float ENTITY_DEPTH = 0.2f;
        private Entity entity;
       

        /// <summary>
        /// Construct!
        /// </summary>
        /// <param name="gameObject">GameObject!</param>
        public EntityGraphics(Entity entity) : base(entity) {
            this.entity = entity;
            this.LayerDepth = ENTITY_DEPTH;
            
            
        }

        public void setTexture(string texture) {
            this.TextureName = texture;
        }

        public void setPosition(int x, int y) {
            entity.getLocation().Position = new Microsoft.Xna.Framework.Vector3(x, y, 0);
        }

        public override void onDraw()
        {
            base.onDraw();
        }
    }

}