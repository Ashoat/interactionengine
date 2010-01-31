using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GlobalGameJam.GameObjects;
using InteractionEngine.UserInterface.TwoDimensional;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using InteractionEngine;


namespace GlobalGameJam.Graphics
{
    class NPCGraphics : CharacterGraphics
    {
        private Texture2D lifebar;
        NPC npc;
        public NPCGraphics(NPC npc)
            : base(npc)
        {
            this.npc = npc;
        }

        public override void loadContent()
        {
            lifebar = UserInterface2D.content.Load<Texture2D>("healthbar");
            base.loadContent();
        }

        public override void onDraw()
        {
            if (lifebar == null) return;
            SpriteBatch spriteBatch = ((UserInterface2D)InteractionEngine.Engine.userInterface).spriteBatch;
            Vector3 position3 = this.gameObject.getLocation().Position;
            int healthBarWidth = (int)(npc.Health * 26 / npc.HealthMax);
            Rectangle healthBarDimensions = new Rectangle((int)position3.X + 4, (int)position3.Y - 2, healthBarWidth, 5);
            Rectangle sourceDimensions = new Rectangle(0, 0, healthBarWidth, lifebar.Height);
    
            spriteBatch.Draw(lifebar, healthBarDimensions, sourceDimensions, new Color(1,1,1,0.8f), 0, Vector2.Zero, SpriteEffects.None, HealthBar.LAYER);
            base.onDraw();
        }
    }
}
