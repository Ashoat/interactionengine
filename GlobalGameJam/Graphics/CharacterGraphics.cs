using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GlobalGameJam.GameObjects;

namespace GlobalGameJam.Graphics
{
    public class CharacterGraphics : EntityGraphics
    {
        private DateTime frameChangeTimePrev;
        private TimeSpan frameChangeTimeDelay = new TimeSpan(5000000);// 0.5ms
        private int frameIndex = 0;
        private string[] textures;

        public CharacterGraphics(Character entity)
            : base(entity)
        {
            frameChangeTimePrev = DateTime.Now;
        }

        public void setTextures(string[] textures)
        {
            this.textures = textures;
            frameIndex = 0;
            setTexture(textures[0]);
        }

        private void UpdateAnimation()
        {
            if (textures != null &&
                DateTime.Now - frameChangeTimePrev > frameChangeTimeDelay)
            {
                if (++frameIndex >= textures.Length)
                    frameIndex = 0;
                setTexture(textures[frameIndex]);
                frameChangeTimePrev = DateTime.Now;
            }
        }


        public override void onDraw()
        {
            if (true) // change to character running property
                UpdateAnimation();
            base.onDraw();
        }

    }
}
