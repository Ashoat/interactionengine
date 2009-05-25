using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace Wumpus3Drev0
{
    class ModelEffect : BasicEffect
    {
        /// <summary>
        /// NOTE: Any additions to the used properties of this class MUST BE ADDED to HelperClass.CloneModelEffect(...)
        /// </summary>
        private BasicCamera activeCamera;
        public BasicCamera ActiveCamera
        {
            set
            {
                activeCamera = value;
                UpdateFromActiveCamera();
            }
            get
            {
                return activeCamera;
            }
        }
        public void UpdateFromActiveCamera()
        {
            base.View = activeCamera.View;
            base.Projection = activeCamera.Projection;
        }

        public ModelEffect(GraphicsDevice device, EffectPool effectPool)
            : base(device, (EffectPool)null)
        {
        }

        public ModelEffect(GraphicsDevice device, EffectPool effectPool, ModelEffect source)
            : base(device, (EffectPool)null)
        {
            this.CloneFrom(source);
        }

        //public ModelEffect() { }

        public void CloneFrom(ModelEffect source)
        {
            this.ActiveCamera = source.ActiveCamera;

            
            this.Alpha = source.Alpha;
            this.AmbientLightColor = source.AmbientLightColor;
            this.CurrentTechnique = source.CurrentTechnique;
            this.DiffuseColor = source.DiffuseColor;
            this.EmissiveColor = source.EmissiveColor;
            this.FogColor = source.FogColor;
            this.FogEnabled = source.FogEnabled;
            this.FogEnd = source.FogEnd;
            this.FogStart = source.FogStart;
            this.LightingEnabled = source.LightingEnabled;
            this.PreferPerPixelLighting = source.PreferPerPixelLighting;
            this.Projection = source.Projection;
            this.SpecularColor = source.SpecularColor;
            this.SpecularPower = source.SpecularPower;
            this.Texture = source.Texture;
            this.TextureEnabled = source.TextureEnabled;
            this.VertexColorEnabled = source.VertexColorEnabled;
            this.View = source.View;
            this.World = source.World;
        }

        //public static implicit operator ModelEffect(BasicEffect

    }
}
