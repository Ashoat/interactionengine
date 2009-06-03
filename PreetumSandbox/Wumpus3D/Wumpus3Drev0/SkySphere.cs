using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using System.ComponentModel;
using System.IO;

namespace Wumpus3Drev0
{
    class SkySphere
    {
        ModelEffect mEffect;
        Model model;

        Vector3 orgin;

        float scale = 1f;



        public Vector3 Orgin
        {
            get {return orgin;}
            set{orgin = value;}
        }

        public ModelEffect Effect
        {
            get { return this.mEffect; }
        }

        public Matrix World
        {
            get { return this.mEffect.World; }
        }

        public BoundingSphere BoundingSphere
        {
            get { return this.getBoundingSphere(); }
        }

        public BasicCamera Camera
        {
            get { return this.mEffect.ActiveCamera; }
        }

        public BoundingSphere getBoundingSphere()
        {
            BoundingSphere welded = this.model.Meshes[0].BoundingSphere;
            foreach (ModelMesh mesh in this.model.Meshes)
            {
                welded = BoundingSphere.CreateMerged(welded, mesh.BoundingSphere);
            }
            BoundingSphere transBounds = new BoundingSphere(welded.Center + this.Orgin, welded.Radius * this.scale);
            return transBounds;
        }

       
        public SkySphere(Model model, ModelEffect effect)
        {
            this.model = model;
            this.mEffect = effect;
            //this.effect.Projection = effect.ActiveCamera.Projection;
            //this.effect.View = effect.ActiveCamera.View;
            this.mEffect.World = calcWorld();
        }
      
        private void updateWorld()
        {
            mEffect.World = calcWorld();
        }

        /// <summary>
        /// scale, rotate, and translation. change this to add features
        /// </summary>
        /// <returns></returns>
        private Matrix calcWorld()
        {
            return Matrix.CreateScale(this.scale) * Matrix.CreateTranslation(orgin) * Matrix.Identity; //Matrix.Identity = world container
        }

        public void SetScale(float scale)
        {
            this.scale = scale;
            updateWorld();
        }

        public bool RayIntersects(Ray ray)
        {
            foreach (ModelMesh mesh in this.model.Meshes)
            {
                if(ray.Intersects(this.BoundingSphere) != null) return true;
            }
            return false;
        }

        public void Draw()
        {

            foreach (ModelMesh mesh in this.model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.GraphicsDevice.RenderState.CullMode = CullMode.None;
                    effect.GraphicsDevice.RenderState.DepthBufferEnable = true;

                    effect.World = calcWorld();


                    effect.Projection = this.mEffect.ActiveCamera.Projection;//terrain.Camera.Projection;
                    effect.View = this.mEffect.ActiveCamera.View;
                    effect.EnableDefaultLighting();
                    
                    //
                    //Now copy all data from the ModelEffect to this BasicEffect
                    //
                    
                    effect.Alpha = this.mEffect.Alpha;
                    effect.AmbientLightColor = this.mEffect.AmbientLightColor;
                    //effect.CurrentTechnique = this.effect.CurrentTechnique;
                    effect.DiffuseColor = this.mEffect.DiffuseColor;
                    mEffect.EmissiveColor = this.mEffect.EmissiveColor;
                    mEffect.FogColor = this.mEffect.FogColor;
                    mEffect.FogEnabled = this.mEffect.FogEnabled;
                    mEffect.FogEnd = this.mEffect.FogEnd;
                    mEffect.FogStart = this.mEffect.FogStart;
                    //mEffect.LightingEnabled = this.mEffect.LightingEnabled;
                    mEffect.PreferPerPixelLighting = this.mEffect.PreferPerPixelLighting;
                    mEffect.SpecularColor = this.mEffect.SpecularColor;
                    mEffect.SpecularPower = this.mEffect.SpecularPower;
                    mEffect.Texture = this.mEffect.Texture;
                    mEffect.TextureEnabled = this.mEffect.TextureEnabled;
                    mEffect.VertexColorEnabled = this.mEffect.VertexColorEnabled;
                    
                }

                mesh.Draw();
            }

        }



    }

}
