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
    class GameModel
    {

        public enum AnimationState
        {
            Stopped,
            Started,
            Stopping
        };

        ModelEffect effect;
        Model model;
        
        Matrix worldLocal;
        Vector2 posRel;
        Vector3 posAbs;
        Vector2 heading;
        float heightOffset = .5f;

        Terrain terrain;

        float scale = 1f;

        float speed = .006f;
        float rotation = .5f;

        float rotationOffset;


        List<Animation> anims;
        AnimationState animState = AnimationState.Stopped;

        int animIndex = 0;

        public List<Animation> Animations
        {
            get { return anims; }
            set { anims = value; }
        }
        public Animation CurrentAnimation
        {
            get { return anims[animIndex]; }
        }


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

        public Terrain Terrain
        {
            get { return terrain; }
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

        public Model CurrentModel
        {
            get
            {
                if (animState != AnimationState.Started)
                {
                    return model;
                }
                else
                {
                    return this.CurrentAnimation.CurrentFrame;
                }
            }
        }

        public BasicCamera Camera
        {
            get { return this.effect.ActiveCamera; }
        }

        public BoundingSphere getBoundingSphere()
        {
            BoundingSphere welded = this.CurrentModel.Meshes[0].BoundingSphere;
            foreach (ModelMesh mesh in this.CurrentModel.Meshes)
            {
                welded = BoundingSphere.CreateMerged(welded, mesh.BoundingSphere);
            }
            BoundingSphere transBounds = new BoundingSphere(welded.Center + this.Position3, welded.Radius * this.scale);
            return transBounds;
        }

        public GameModel(Model model, List<Animation> animations, ModelEffect effect, Terrain terrain, GraphicsDevice device)
        {
            this.model = model;
            this.anims = animations;

            this.terrain = terrain;

            this.worldLocal = Matrix.Identity;

            //this.effect = new ModelEffect(device, null);
            this.effect = effect;

            this.effect.Projection = terrain.Camera.Projection;
            this.effect.View = terrain.Camera.View;
            this.effect.World = this.worldLocal * terrain.WorldMatrix;

            this.SetPosition(Vector2.Zero);
        }

        public GameModel(Model model, ModelEffect effect, Terrain terrain, GraphicsDevice device)
            : this(model, new List<Animation>(), effect, terrain, device)
        {
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

        public void MoveTo(Vector2 pos, float speed)
        {
            this.SetTarget(pos);
            this.speed = speed;
        }

        public void SetTarget(Vector2 tar)
        {
            this.Rotation = HelperClass.GetAngle((this.Position2 - tar));
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
            foreach (ModelMesh mesh in this.CurrentModel.Meshes)
            {
                if (ray.Intersects(new BoundingSphere(mesh.BoundingSphere.Center + this.Position3, mesh.BoundingSphere.Radius * this.scale)) != null) return true;
            }
            //if (ray.Intersects(this.BoundingSphere) != null) return true;
            return false;
        }

        public void StartAnimation(string name)
        {
            this.animIndex = this.GetAnimIndexOf(this.GetAnimationByName(name));
            animState = AnimationState.Started;   
        }
        public void StopAnimation()
        {
            if (animState == AnimationState.Started)
                animState = AnimationState.Stopping;
        }
        public void ForceStopAnimation()
        {
            animState = AnimationState.Stopped;
            this.CurrentAnimation.Index = 0;
        }

        public Animation GetAnimationByName(string name)
        {
            return anims.Find(a => a.Name == name);
        }
        public int GetAnimIndexOf(Animation a)
        {
            return anims.IndexOf(a);
        }

        private void drawModel(Model m)
        {
            foreach (ModelMesh mesh in m.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {

                    effect.World = worldContainer(terrain.WorldMatrix); // so that the scale and stuff changes when the terrain scale changes


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

        public void Draw()
        {
            if (animState != AnimationState.Stopped)
            {
                drawModel(this.CurrentAnimation.CurrentFrame);
                if (!this.CurrentAnimation.NextFrame()) //if NextFrame() causes a loopback to frame index 0
                    if (animState == AnimationState.Stopping)
                        animState = AnimationState.Stopped;
            }
            else
            {
                drawModel(this.model);
            }
        }



    }

}
