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
using Microsoft.Xna.Framework.Content;
using System.ComponentModel;
using System.IO;

namespace HeightMapTest1
{
    class Terrain
    {
        VertexPositionNormalTexture[] vertices;
        int[] indices;

        byte[] heightMap;

        VertexBuffer vb;
        IndexBuffer ib;
        int numVertices;
        int numTriangles;

        int vertexCountX;
        int vertexCountZ;
        float blockScale;
        float heightScale;

        Texture2D tex;

        ModelEffect effect;

        GraphicsDevice dev;

        //float scale = 1f;

        public Vector2 Size
        {
            get
            {
                return new Vector2(vertexCountX * blockScale, vertexCountZ * blockScale);
            }
        }

        public VertexBuffer VertexBuffer
        {
            get { return vb; }
        }
        public IndexBuffer IndexBuffer
        {
            get { return ib; }
        }
        public Texture2D Texture
        {
            get { return tex; }
            set { tex = value; }
        }
        public BasicCamera Camera
        {
            get { return effect.ActiveCamera; }
        }

        public Matrix WorldMatrix
        {
            get { return effect.World; }
            //set { effect.World = value; }
        }

        public ModelEffect Effect
        {
            get { return this.effect; }
        }

        /*public float Scale
        {
            get { return this.scale; }
            set
            {
                this.SetScale(value);
            }
        }*/

        private void LoadTexture(Texture2D asset)
        {
            tex = asset;
        }

        /*public void SetScale(float scale)
        {
            this.scale = scale;
            updateWorld();
        }*/

        private void updateWorld()
        {
            //this.WorldMatrix = Matrix.CreateScale(this.scale);
        }

        private void LoadHeightmapFromImage(Texture2D asset)
        {
            Color[] map = new Color[asset.Width * asset.Height];
            asset.GetData<Color>(map);
            heightMap = new byte[map.Length];
            for (int i = 0; i < heightMap.Length; i++)
            {
                heightMap[i] = map[i].R;
            }

            vertexCountX = asset.Width;
            vertexCountZ = asset.Height;
            numTriangles = (vertexCountX - 1) * (vertexCountZ - 1) * 2;
            numVertices = map.Length;

            //texImage.Dispose();
        }

        private void LoadHeightmapFromRaw(FileStream fileStream)
        {
            //FileStream fileStream = File.OpenRead(this.Content.RootDirectory + "/" + filename);
            heightMap = new byte[fileStream.Length];
            fileStream.Read(heightMap, 0, (int)fileStream.Length);
            fileStream.Close();

            numVertices = heightMap.Length;
            vertexCountX = (int)Math.Sqrt(numVertices);
            vertexCountZ = heightMap.Length / vertexCountX;
            numTriangles = (vertexCountX - 1) * (vertexCountZ - 1) * 2;
        }

        private void GenerateIndices()
        {
            int numIndices = numTriangles * 3;
            indices = new int[numIndices];

            int indicesCount = 0;
            for (int i = 0; i < (vertexCountZ - 1); i++) //pg 273-274
            {
                for (int j = 0; j < (vertexCountX - 1); j++)
                {
                    int index = j + i * vertexCountZ;
                    // First triangle
                    indices[indicesCount++] = index;
                    indices[indicesCount++] = index + 1;
                    indices[indicesCount++] = index + vertexCountX + 1;
                    // Second triangle
                    indices[indicesCount++] = index + vertexCountX + 1;
                    indices[indicesCount++] = index + vertexCountX;
                    indices[indicesCount++] = index;
                }
            }
        }


        private void GenerateVertices()
        {
            vertices = new VertexPositionNormalTexture[numVertices];

            int vertexCount = 0;
            for (float i = 0; i < vertexCountZ; i++)
            {
                for (float j = 0; j < vertexCountX; j++)
                {
                    vertices[vertexCount].Position = new Vector3((j - vertexCountX / 2) * blockScale, heightMap[vertexCount] * heightScale, (i - vertexCountZ / 2) * blockScale);
                    vertices[vertexCount].TextureCoordinate = new Vector2(j / vertexCountX, i / vertexCountZ);
                    vertexCount++;
                }
            }
        }

        private void GenerateNormals()
        {
            //
            //Yo! Iz mah normalz!
            //pg 277.
            /*
             * Each vertex can be shared by multiple triangles.
             * foreach triangle, add the normal of the tringle to the normals of each vertex. then normalize
             */
            for (int i = 0; i < indices.Length; i += 3)
            {
                Vector3 v1 = vertices[indices[i]].Position;
                Vector3 v2 = vertices[indices[i + 1]].Position;
                Vector3 v3 = vertices[indices[i + 2]].Position;

                Vector3 vu = v3 - v1;
                Vector3 vt = v2 - v1;
                Vector3 normal = Vector3.Cross(vu, vt);
                normal.Normalize();

                vertices[indices[i]].Normal += normal;
                vertices[indices[i + 1]].Normal += normal;
                vertices[indices[i + 2]].Normal += normal;
            }

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].Normal.Normalize();
            }
        }


        private void SetData(GraphicsDevice device)
        {
            vb = new VertexBuffer(device, numVertices * VertexPositionNormalTexture.SizeInBytes,
                BufferUsage.WriteOnly);
            vb.SetData<VertexPositionNormalTexture>(vertices);

            ib = new IndexBuffer(device, numTriangles * 3 * sizeof(int), BufferUsage.WriteOnly,
                IndexElementSize.ThirtyTwoBits);
            ib.SetData<int>(indices);
        }

        private void InitDefaultEffectVal()
        {
            updateWorld();

            effect.EnableDefaultLighting();
            //effect.PreferPerPixelLighting = true;
            effect.TextureEnabled = true;
            effect.Texture = tex;
            effect.SpecularColor = new Vector3(.4f, .4f, .4f);
            effect.PreferPerPixelLighting = true;
            effect.SpecularPower = 4f;
            //effect.FogEnabled = true;
            //effect.FogColor = Vector3.Zero;
            //effect.FogStart = 50;
            //effect.FogEnd = 3000;
        }

        public Terrain(GraphicsDevice device, BasicCamera camera, Texture2D mapAsset, Texture2D texAsset, float blockScaleF, float heightScaleF)
        {
            this.blockScale = blockScaleF;
            this.heightScale = heightScaleF;

            dev = device;

            this.LoadHeightmapFromImage(mapAsset);
            this.LoadTexture(texAsset);

            this.GenerateIndices();
            this.GenerateVertices();
            this.GenerateNormals();

            this.SetData(device);

            //this.world

            this.effect = new ModelEffect(device, null);
            this.effect.ActiveCamera = camera;
            this.InitDefaultEffectVal();

            //this.SetScale(scale);
        }

        public void Draw()
        {
            dev.VertexDeclaration = new VertexDeclaration(dev, VertexPositionNormalTexture.VertexElements);
            dev.Vertices[0].SetSource(vb, 0, VertexPositionNormalTexture.SizeInBytes);
            dev.Indices = ib;

            effect.UpdateFromActiveCamera();
            effect.Begin();
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Begin();
                // Draw the mesh
                dev.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, numVertices, 0, numTriangles);
                pass.End();
            }
            effect.End();
        }

        //
        //
        //
        public bool isOnTerrain(Vector2 pos)
        {
            if (pos.X > -this.Size.X / 2 && pos.X < this.Size.X / 2 &&
                pos.Y > -this.Size.Y / 2 && pos.Y < this.Size.Y / 2)
            {
                return true;
            }
            return false;
        }
        public bool isOnTerrain(Vector3 pos)
        {
            return isOnTerrain(new Vector2(pos.X, pos.Z));
        }

        public float getHeight(Vector2 pos) //(x,z)
        {
            //return Vector3.Transform(vertices[128 + vertexCountX * 128].Position, this.WorldMatrix).Y;
            // Check if the object is inside the grid
            if (isOnTerrain(pos))
            {
                try
                {
                    pos += this.Size / 2;
                    Vector2 blockPos = new Vector2((int)(pos.X / blockScale), (int)(pos.Y / blockScale));
                    Vector2 posRel = pos - blockPos * blockScale;

                    int vertexIndex = (int)blockPos.X + (int)blockPos.Y * vertexCountX;
                    float height1 = vertices[vertexIndex + 1].Position.Y;
                    float height2 = vertices[vertexIndex].Position.Y;
                    float height3 = vertices[vertexIndex + vertexCountX + 1].Position.Y;
                    float height4 = vertices[vertexIndex + vertexCountX].Position.Y;

                    float heightHxLz = vertices[vertexIndex + 1].Position.Y;
                    float heightLxLz = vertices[vertexIndex].Position.Y;
                    float heightHxHz = vertices[vertexIndex + vertexCountX + 1].Position.Y;
                    float heightLxHz = vertices[vertexIndex + vertexCountX].Position.Y;

                    bool aboveLowerTri = posRel.X < posRel.Y;
                    float heightIncX, heightIncY;
                    if (aboveLowerTri)
                    {
                        heightIncX = height3 - height4;
                        heightIncY = height4 - height2;
                    }
                    else
                    {
                        heightIncX = height1 - height2;
                        heightIncY = height3 - height1;
                    }
                    float lerpHeight = height2 + heightIncX * posRel.X + heightIncY * posRel.Y;
                    return lerpHeight;
                }
                catch { }
            }
            return 20; //default value
        }

        public float getHeight(Vector3 pos)
        {
            return getHeight(new Vector2(pos.X, pos.Z));
        }

        public Vector3? RayIntersects(Ray ray)
        {
            Vector3 rayStep = ray.Direction * blockScale * .5f;
            Vector3 currPoint = ray.Position;
            Vector3 prevPoint = currPoint;

            currPoint += rayStep;

            while (currPoint.Y > this.getHeight(currPoint) && this.isOnTerrain(currPoint))
            {
                prevPoint = currPoint;
                currPoint += rayStep;
            }
            //linear search over

            Vector3 topPoint = prevPoint;
            Vector3 botPoint = currPoint;
            for (int i = 0; i < 8; i++)
            {
                Vector3 midPoint = (topPoint + botPoint) / 2;
                if (midPoint.Y < this.getHeight(midPoint))
                    botPoint = midPoint;
                else
                    topPoint = midPoint;
            }
            return prevPoint; //the last point seen above the terrain.
        }

    }
}
