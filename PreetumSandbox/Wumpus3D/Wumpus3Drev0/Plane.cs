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
    class SimplePlane
    {
        VertexPositionColor[] vertices;
        GraphicsDevice dev;
        ModelEffect effect;

        VertexBuffer vb;
        Vector3 orgin;

        float height;
        float width;

        float scale = 1f;

        public Vector3 AmbientColor
        {
            get { return this.effect.AmbientLightColor; }
            set { this.effect.AmbientLightColor = value; }
        }
        public float Alpha
        {
            get { return this.effect.Alpha; }
            set { this.effect.Alpha = value; }
        }
        public Vector3 Orgin
        {
            get { return orgin; }
            set { orgin = value; }
        }

        public float Height
        {
            get { return height; }
            set
            {
                height = value;
                this.UpdateVertices();
                this.SetData(dev);
            }
        }

        public float Width
        {
            get { return width; }
            set
            {
                width = value;
                this.UpdateVertices();
                this.SetData(dev);
            }
        }

        public ModelEffect Effect
        {
            get { return this.effect; }
            set { this.effect = value; }
        }

        public Matrix World
        {
            get { return this.effect.World; }
        }


        public BasicCamera Camera
        {
            get { return this.effect.ActiveCamera; }
        }


        public void InitEffect()
        {

            this.effect.UpdateFromActiveCamera();
            this.effect.World = calcWorld();
            this.effect.EnableDefaultLighting();

            this.effect.AmbientLightColor = new Vector3(.2f, .2f, .6f);
            this.effect.Alpha = .7f;
        }

        public SimplePlane(Vector3 orgin, float width, float height, BasicCamera cam, GraphicsDevice dev)
        {
            this.dev = dev;
            this.orgin = orgin;
            this.width = width;
            this.height = height;

            this.effect = new ModelEffect(dev, null);
            this.effect.ActiveCamera = cam;

            this.InitEffect();

            this.UpdateVertices();
            this.SetData(dev);
 
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
        private void SetData(GraphicsDevice device)
        {
            vb = new VertexBuffer(device, 6 * VertexPositionColor.SizeInBytes,
                BufferUsage.WriteOnly);
            vb.SetData<VertexPositionColor>(vertices);
        }

        public void UpdateVertices()
        {
            vertices = new VertexPositionColor[6];

            vertices[0].Position = new Vector3(width / 2, 0, -height / 2);
            vertices[1].Position = new Vector3(width / 2, 0, height / 2);
            vertices[2].Position = new Vector3(-width / 2, 0, -height / 2);

            vertices[3] = vertices[1];
            vertices[4].Position = new Vector3(-width / 2, 0, height / 2);
            vertices[5] = vertices[2];

            /*vertices[0].Color = Color.Blue;
            vertices[1].Color = Color.Blue;
            vertices[2].Color = Color.Blue;
            vertices[3].Color = Color.Blue;*/
        }
        public void Draw()
        {
            dev.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
            dev.RenderState.DepthBufferEnable = true;

            dev.VertexDeclaration = new VertexDeclaration(dev, VertexPositionColor.VertexElements);
            dev.Vertices[0].SetSource(vb, 0, VertexPositionColor.SizeInBytes);

            
            effect.UpdateFromActiveCamera();
            effect.Begin();
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Begin();
                // Draw the mesh
                dev.DrawPrimitives(PrimitiveType.TriangleList, 0, 2);
                pass.End();
            }
            effect.End();
        }



    }

}
