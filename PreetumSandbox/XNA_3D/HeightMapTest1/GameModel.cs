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

namespace HeightMapTest1
{
    class GameModel
    {
        ModelEffect effect;

        Model model;

        Matrix worldLocal;
        Vector2 posRel;
        Vector3 posAbs;
        Vector2 heading;
        float heightOffset = .5f;

        Terrain terrain;

        float scale = 1f;

        float speed = .6f;
        float rotation = .5f;

        float rotationOffset;

        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        /// <summary>
        /// The offset while DRAWING the model. only applies to DRAWING!
        /// </summary>
        public float RotationOffset
        {
            get { return rotationOffset; }
            set { rotationOffset = value; }
        }

        public float Speed
        {
            get { return speed; }
            set { speed = value; }
        }

        public Vector2 Position2
        {
            get
            {
                return posRel;
            }
            set
            {
                if (this.terrain.isOnTerrain(value))
                    SetPosition(value);
            }
        }

        public Vector3 Position3
        {
            get
            {
                return posAbs;
            }
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

        public BoundingSphere getBoundingSphere()
        {
            BoundingSphere welded = this.model.Meshes[0].BoundingSphere;
            foreach (ModelMesh mesh in this.model.Meshes)
            {
                welded = BoundingSphere.CreateMerged(welded, mesh.BoundingSphere);
            }
            BoundingSphere transBounds = new BoundingSphere(welded.Center + this.Position3, welded.Radius * this.scale);
            return transBounds;
        }

        public GameModel(Model model, ModelEffect effect, Terrain terrain, GraphicsDevice device)
        {
            this.model = model;
            this.terrain = terrain;

            this.worldLocal = Matrix.Identity;

            //this.effect = new ModelEffect(device, null);
            this.effect = effect;
            
            this.effect.Projection = terrain.Camera.Projection;
            this.effect.View = terrain.Camera.View;
            this.effect.World = this.worldLocal * terrain.WorldMatrix;

            this.SetPosition(Vector2.Zero);
        }

        public void SetPosition(Vector2 pos)
        {
            posAbs = (new Vector3(pos.X, terrain.getHeight(pos), pos.Y));
            posRel = pos;
            updateWorld();
        }

        public void Displace(Vector2 disp)
        {
            this.Position2 += disp;
        }

        public void Move(Vector2 strafe)
        {
            this.Position2 += HelperClass.RotateVector(strafe, rotation);
        }

        private void updateWorld()
        {
            this.worldLocal = localWorld();
            effect.World = worldContainer(terrain.WorldMatrix);
        }

        /// <summary>
        /// ports the local world matrix to the [terrain] container
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        private Matrix worldContainer(Matrix container)
        {
            return localWorld() * container;
        }

        /// <summary>
        /// scale, rotate, and translation. change this to add features
        /// </summary>
        /// <returns></returns>
        private Matrix localWorld()
        {
            return Matrix.CreateScale(this.scale) * Matrix.CreateRotationY(rotation+rotationOffset) * Matrix.CreateTranslation(posAbs); //add more later. scale + rotate.
        }

        public void SetScale(float scale)
        {
            this.scale = scale;
            updateWorld();
        }

        public bool RayIntersects(Ray ray)
        {
            foreach (ModelMesh mesh in model.Meshes)
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

                    effect.World = worldContainer(terrain.WorldMatrix); // so that the scale and stuff changes when the terrain scale changes
                    

                    effect.Projection = terrain.Camera.Projection;
                    effect.View = terrain.Camera.View;
                    effect.EnableDefaultLighting();
                    
                    //
                    //Now copy all data from the ModelEffect to this BasicEffect
                    //
                    effect.Alpha = this.effect.Alpha;
                    effect.AmbientLightColor = this.effect.AmbientLightColor;
                    effect.CurrentTechnique = this.effect.CurrentTechnique;
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
