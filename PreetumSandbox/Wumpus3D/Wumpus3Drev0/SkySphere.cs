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
        ModelEffect effect;
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
            get { return this.effect; }
        }

        public Matrix World
        {
            get { return this.effect.World; }
        }

        public BoundingSphere BoundingSphere
        {
            get { return this.getBoundingSphere(); }
        }

        public BasicCamera Camera
        {
            get { return this.effect.ActiveCamera; }
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
            this.effect = effect;
            //this.effect.Projection = effect.ActiveCamera.Projection;
            //this.effect.View = effect.ActiveCamera.View;
            this.effect.World = calcWorld();
        }
      
        private void updateWorld()
        {
            effect.World = calcWorld();
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

                    effect.World = calcWorld();


                    effect.Projection = this.effect.ActiveCamera.Projection;//terrain.Camera.Projection;
                    effect.View = this.effect.ActiveCamera.View;
                    effect.EnableDefaultLighting();
                    
                    //
                    //Now copy all data from the ModelEffect to this BasicEffect
                    //
                    
                    effect.Alpha = this.effect.Alpha;
                    effect.AmbientLightColor = this.effect.AmbientLightColor;
                    //effect.CurrentTechnique = this.effect.CurrentTechnique;
                    effect.DiffuseColor = this.effect.DiffuseColor;
                    effect.EmissiveColor = this.effect.EmissiveColor;
                    effect.FogColor = this.effect.FogColor;
                    effect.FogEnabled = this.effect.FogEnabled;
                    effect.FogEnd = this.effect.FogEnd;
                    effect.FogStart = this.effect.FogStart;
                    //effect.LightingEnabled = this.effect.LightingEnabled;
                    effect.PreferPerPixelLighting = this.effect.PreferPerPixelLighting;
                    effect.SpecularColor = this.effect.SpecularColor;
                    effect.SpecularPower = this.effect.SpecularPower;
                    effect.Texture = this.effect.Texture;
                    effect.TextureEnabled = this.effect.TextureEnabled;
                    effect.VertexColorEnabled = this.effect.VertexColorEnabled;
                    
                }

                mesh.Draw();
            }

        }



    }

}
