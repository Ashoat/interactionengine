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

namespace Wumpus3Drev0
{
    class Terrain
    {
        VertexPositionNormalTexture[] vertices;
        int[] indices;

        byte[] heightMap;
        Color[] dynTexData;

        VertexBuffer vb;
        IndexBuffer ib;
        int numVertices;
        int numTriangles;

        int vertexCountX;
        int vertexCountZ;
        float blockScale;
        float heightScale;

        //Texture2D tex;
        Texture2D dynTex;

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
            get { return dynTex; }
            set { dynTex = value; }
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
            dynTex = asset;
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

        private int getRandomWithinBounds(Random rand, int maxDiff) //returns a byte within -maxDiff, +maxDiff
        {
            return (int)rand.Next(-maxDiff, maxDiff);
        }
        private void GenerateRandomWalkMap(int width, int height, byte roughness)
        {

            Random rand = new Random();

            /*byte[] randomWalk1 = new byte[width * height];
 
            // execute 2d random walks on each ROW (+x)
            
            int h = 0, w = 0;
            for (h = 0; h < height; h++)
            {
                rand = new Random(h * w);
                randomWalk1[h * width] = (byte)rand.Next(0, 255); //set the first height of the row
                for (w = 1; w < height; w++) //calc 2nd -> last heights
                {
                    randomWalk1[w + h * width] = (byte)(randomWalk1[(w-1) + h * width]+getRandomWithinBounds(rand, roughness));
                }
            }


            heightMap = randomWalk1;
            vertexCountX = width;
            vertexCountZ = height;
            numTriangles = (vertexCountX - 1) * (vertexCountZ - 1) * 2;
            numVertices = heightMap.Length;
            */
        }
        private int i = 0;
        private void recurseMidFractalMap(ref byte[] map, int bot, int top, int mag, float magConst, Random rand) // a 1D map. 1D!!! || bot and top are index pointers. 'mag' is the magnitude of rand. modulation
        {
            i++;
            if (top - bot <= 1)
                return;
            int midPt =(bot+top)/2;
            int midVal = (map[bot] + map[top])/2;
            int midNew = -1;
            while (midNew < 0)
            {
                midNew = midVal + rand.Next(-mag, mag);
            }
            map[midPt] = (byte)midNew;
            recurseMidFractalMap(ref map, bot, midPt, (int)(mag * magConst), magConst, rand);
            recurseMidFractalMap(ref map, midPt, top, (int)(mag * magConst), magConst, rand);
        }
        private void GenerateMidFractalMap(int width, int height, int mag, float magConst)
        {
            heightMap = new byte[width * height];
            for (int h = 0; h < height; h++)
            {
                byte[] row = new byte[width];
                recurseMidFractalMap(ref row, 0, row.Length-1, mag, magConst, new Random(h)); //remove 'h' so see just 1 plane
                for (int i = 0; i < width; i++)
                    heightMap[i + h * width] = row[i];
            }
            vertexCountX = width;
            vertexCountZ = height;
            numTriangles = (vertexCountX - 1) * (vertexCountZ - 1) * 2;
            numVertices = heightMap.Length;
        }

        private class Int2
        {
            public int X;
            public int Y;
            public Int2(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
        }

        private float GetVal(Int2 v, float[,] map)
        {
            return map[v.X, v.Y];
        }

        private void GenerateDynamicTerrainMap(int size, float magR, float r)
        {
            Random rand = new Random((int)Camera.View.Determinant() * DateTime.Now.Millisecond);
            float[,] map = new float[size, size]; //[w,h] - by convention
            //seed 4 corners
            /*map[0, 0] = rand.Next(0, 30);
            map[0, size - 1] = rand.Next(0, 30);
            map[size - 1, 0] = rand.Next(0, 30);
            map[size - 1, size - 1] = rand.Next(0, 30);*/

            map[0, 0] = 0;
            map[0, size - 1] = 0;
            map[size - 1, 0] = 0;
            map[size - 1, size - 1] = 0;

            int sideLen = size-1;
           

            while (sideLen > 0)
            {
                //perform mid-square thing
                for (int h = 0; h < size-1; h += sideLen)
                {
                    for (int w = 0; w < size-1; w += sideLen)
                    {
                        //rand = new Random(w * h);
                        //rand = new Random((int)Camera.View.Determinant() * DateTime.Now.Millisecond * w * h);
                        //now w,h is the uper left corner of each square 
                        Int2 center = new Int2((w + sideLen / 2), (h + sideLen / 2)); //the center of the square
                        map[center.X, center.Y] = (map[w, h] + map[w + sideLen, h] + map[w, h + sideLen] + map[w + sideLen, h + sideLen]) / 4; //the center = avg of 4 corners
                        map[center.X, center.Y] += (float)(rand.NextDouble() * 2 - 1) * magR;
                    }
                }

                //perform diamond-y thing
                for (int h = 0; h < size; h += sideLen)
                {
                    for (int w = 0; w < size; w += sideLen)
                    {
                        
                        Int2 p0 = new Int2(w, h); // "orgin"
                        Int2 p1 = new Int2((w + sideLen / 2) % size, (h - sideLen / 2 + size) % size); //the (...+size)%size is to loop, but take negatives into account
                        Int2 p2 = new Int2((w + sideLen) % size, h);
                        Int2 p3 = new Int2((w + sideLen / 2) % size, (h + sideLen / 2) % size);
                        Int2 p4 = new Int2(w, (h + sideLen) % size);
                        Int2 p5 = new Int2((w - sideLen / 2 + size) % size, (h + sideLen / 2) % size);

                        Int2 c1 = new Int2(w, (h + sideLen / 2) % size);
                        Int2 c2 = new Int2((w + sideLen / 2) % size, h);
                        
                        rand = new Random(p0.X * p5.X);  //p0.X * p5.X
                        map[c1.X, c1.Y] = (GetVal(p0, map) + GetVal(p1, map) + GetVal(p2, map) + GetVal(p3, map)) / 4;
                        map[c1.X, c1.Y] += (float)(rand.NextDouble() * 2 - 1) * magR;

                        rand = new Random(p4.Y * p5.Y);
                        map[c2.X, c2.Y] = (GetVal(p0, map) + GetVal(p3, map) + GetVal(p4, map) + GetVal(p5, map)) / 4;
                        map[c2.X, c2.Y] += (float)(rand.NextDouble() * 2 - 1) * magR;
                    }
                }

                sideLen /= 2;
                magR *= r;
            }
            NormalizeToTerrain(map);
            SaveMapToImage();
        }

        public float[] ConvertTo1D(float[,] a)
        {
            int width = a.GetLength(0);
            int height = a.GetLength(1);

            float[] data = new float[width * height];
            for (int h = 0; h < height; h++)
            {
                for (int w = 0; w < width; w++)
                {
                    data[w + h * width] = a[w, h];
                }
            }
            return data;
        }

        public double Gaussian(double x, double sd)
        {
            double a = sd * Math.Sqrt((double)MathHelper.TwoPi);
            double b = -((x / sd) * (x / sd)) / 2;
            return (1 / a) * Math.Pow(Math.E, b);
        }
        /// <summary>
        /// Performs gaussian pass along ROWS. note: only checkes pixels within 3*sd of source. (for speed)
        /// </summary>
        /// <param name="data"></param>
        /// <param name="sd"></param>
        /// <returns></returns>
        private float[,] gaussianPass1(float[,] data, float sd) //ROW pass. sd = standard deviation
        {
            int width = data.GetLength(0);
            int height = data.GetLength(1);
            for (int h = 0; h < height; h++)
            {
                float[] row = new float[width];
                for (int w = 0; w < width; w++)
                {
                    float valTot = 0;
                    float sum = 0;
                    for (int k = w - (int)(3 * sd) - 1; k < w + (int)(3 * sd) + 1; k++) //for (int p = 0; p < width; p++)
                    {
                        int p = (k + width) % width;
                        float gauss = (float)Gaussian((double)(w - k), (double)sd); //(float)Gaussian((double)(w - p), (double)sd);
                        valTot += data[p, h] * gauss;
                        sum += gauss;
                    }
                    row[w] = valTot/sum;
                }
                for (int i = 0; i < width; i++) //copy row into source array
                    data[i, h] = row[i];
            }
            return data;
        }

        private float[,] gaussianPass2(float[,] data, float sd) //ROW pass. sd = standard deviation
        {
            int width = data.GetLength(0);
            int height = data.GetLength(1);
            for (int w = 0; w < width; w++)
            {
                float[] col = new float[height];
                for (int h = 0; h < height; h++)
                {
                    float valTot = 0;
                    float sum = 0;
                    for (int k = h - (int)(3 * sd) - 1; k < h + (int)(3 * sd) + 1; k++) //for (int p = 0; p < height; p++)
                    {
                        int p = (k + height) % height;
                        float gauss = (float)Gaussian((double)(h - k), (double)sd);
                        valTot += data[w, p] * gauss;
                        sum += gauss;
                    }
                    col[h] = valTot / sum;
                }
                for (int i = 0; i < height; i++) //copy row into source array
                    data[w, i] = col[i];
            }
            return data;
        }

        public float MaxAdvanced(float[] data)
        {
            float max1 = RegWeighted(data);
            float max2 = InvWeighted(data);
            float max = (max1 > max2) ? max1 : max2;
            max = (max + maximum(data)) / 2;
            return max;
        }
        public float MinAdvanced(float[] data)
        {
            float min1 = RegWeighted(data);
            float min2 = InvWeighted(data);
            float min = (min1 < min2) ? min1 : min2;
            min = (min + minimum(data)) / 2;
            return min;
        }
        public float RegWeighted(float[] data)
        {
            float sumFx = 0;
            float sumXfX = 0; //x * f(x). where f(x) is the histogram

            int[] hist = this.histogram(data);
            for (int i = 0; i < hist.Length; i++)
            {
                sumXfX += i * hist[i];
                sumFx += hist[i];
            }
            //Debug.WriteLine("max weighted: " + sumXfX / sumFx);
            return sumXfX / sumFx;
        }

        public float InvWeighted(float[] data)
        {
            float sumFx = 0;
            float sumXfX = 0; //x * f(x). where f(x) is the histogram

            int[] histInv = this.histogram(this.invert(data));
            for (int i = 0; i < histInv.Length; i++)
            {
                sumXfX += i * histInv[i];
                sumFx += histInv[i];
            }
            //Debug.WriteLine("min weighted: " + sumXfX / sumFx);
            return sumXfX / sumFx;
        }

        public int[] histogram(float[] data) //returns the number (int[x]) that the value x appears in float[] data
        {
            int[] hist = new int[(int)maximum(data)+1];
            foreach (float f in data)
            {
                    hist[(int)f]++;
            }
            return hist;
        }

        private float[] invert(float[] data)
        {
            float[] inverted = (float[])data.Clone();
            for (int i = 0; i < data.Length; i++)
            {
                inverted[i] = (maximum(data) - data[i]);
            }
            return inverted;
        }

        private float minimum(float[] data)
        {
            float min = float.MaxValue;
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] < min) min = data[i];
            }
            return min;
        }
        private float minimum(float[,] data)
        {
            float min = float.MaxValue;
            int dim1 = data.GetLength(0);
            int dim2 = data.GetLength(1);
            for (int a = 0; a < dim2; a++)
            {
                for (int i = 0; i < dim1; i++)
                {
                    if (data[i,a] < min) min = data[i,a];
                }
            }
            return min;
        }
        private float maximum(float[] data)
        {
            float max = float.MinValue;
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] > max) max = data[i];
            }
            return max;
        }

        private float[] reboundToZero(float[] data)
        {
            float min = minimum(data);
            if (min < 0)
            {
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] += -min;
                }
            }
            return data;
        }

        private float[,] reboundToZero(float[,] data)
        {
            float min = minimum(data);
            int dim1 = data.GetLength(0);
            int dim2 = data.GetLength(1);
            for (int a = 0; a < dim2; a++)
            {
                for (int i = 0; i < dim1; i++)
                {
                    data[i, a] += -min;
                }
            }
            return data;
        }

        private byte clampBounds(float a)
        {
            if (a < 0) return 255;
            if (a > 255) return 255;
            return (byte)a;
        }
        private void NormalizeToTerrain(float[,] map)
        {
            int width = map.GetLength(0);
            int height = map.GetLength(1);

            this.heightMap = new byte[width * height];

            vertexCountX = width;
            vertexCountZ = height;
            numTriangles = (vertexCountX - 1) * (vertexCountZ - 1) * 2;
            numVertices = this.heightMap.Length;

            /*
            float max = float.MinValue, min = float.MaxValue;
            for (int h = 0; h < height; h++)
            {
                for (int w = 0; w < width; w++)
                {
                    if (map[w, h] > max) max = map[w, h];
                    if (map[w, h] < min) min = map[w, h];
                }
            }
            */

            map = reboundToZero(map);

            map = gaussianPass1(map, .5f);
            map = gaussianPass2(map, .5f);

            float[] data = ConvertTo1D(map);

            
            /*float[] cubic = new float[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                cubic[i] = data[i] * data[i] * data[i];
            }

            for(int i=0;i<data.Length;i++)
            {
                data[i] = data[i] + cubic[i] * 0.0001f;
            }*/

            //data = reboundToZero(data);

            float min, max;
            //min = this.MinAdvanced(data);
            //max = this.MaxAdvanced(data);
            min = minimum(data);
            max = maximum(data);



            for (int i = 0; i < data.Length; i++)
            {
                heightMap[i] = clampBounds((255f / (max + min))*(data[i] + min));
            }
            
            
        }

        public void SaveMapToImage()
        {
            Color[] c = new Color[vertexCountX * vertexCountZ];
            for (int i = 0; i < this.heightMap.Length; i++)
            {
                c[i] = new Color();
                c[i].R = this.heightMap[i];
                c[i].G = this.heightMap[i];
                c[i].B = this.heightMap[i];
                c[i].A = 255;
            }
            Texture2D tex = new Texture2D(this.dev, vertexCountX, vertexCountX);
            tex.SetData<Color>(c);
            tex.Save("dynMap.png", ImageFileFormat.Png);
        }

        public void LoadDynTex()
        {
            dynTexData = new Color[dynTex.Width * dynTex.Height];
            dynTex.GetData<Color>(dynTexData);
        }

        private Vector2 getDynCoord(byte height, Random rand)
        {
            float w = (float)height / 255f; //horizonal co-ordinate
            float h = rand.Next(0, dynTex.Height) / (float)dynTex.Height;
            return new Vector2(w, h);
        }

        private void GenerateIndices()
        {
            indices = new int[vertexCountX * 2 * (vertexCountZ - 1)];

            int i = 0;
            int z = 0;

            while (z < vertexCountZ - 1)
            {
                for (int x = vertexCountZ - 1; x >= 0; x--)
                {
                    indices[i++] = x + z * vertexCountX;
                    indices[i++] = x + (z + 1) * vertexCountX;
                }
                z++;

                if (z >= vertexCountZ - 1) break; //or continue. it really doesnt matter

                for (int x = 0; x < vertexCountZ; x++)
                {
                    indices[i++] = x + (z + 1) * vertexCountX;
                    indices[i++] = x + z * vertexCountX;
                }
                z++;
            }

        }

        private void GenerateVertices()
        {
            vertices = new VertexPositionNormalTexture[numVertices];
            Random rand = new Random((int)Camera.View.Determinant()*DateTime.Now.Millisecond);
            int vertexCount = 0;
            for (float i = 0; i < vertexCountZ; i++)
            {
                for (float j = 0; j < vertexCountX; j++)
                {
                    vertices[vertexCount].Position = new Vector3((j - vertexCountX / 2) * blockScale, heightMap[vertexCount] * heightScale, (i - vertexCountZ / 2) * blockScale);
                    //vertices[vertexCount].TextureCoordinate = new Vector2(j / vertexCountX, i / vertexCountZ);
                    vertices[vertexCount].TextureCoordinate = getDynCoord(heightMap[vertexCount],rand);
                    vertexCount++;
                }
            }
        }


        private void GenerateNormals()
        {
            for(int i=0;i<vertices.Length;i++)
                vertices[i].Normal = Vector3.Zero;

            bool swapWinding = false;
            for (int i = 2; i < indices.Length; i++)
            {
                Vector3 v1 = vertices[indices[i - 1]].Position - vertices[indices[i]].Position;
                Vector3 v2 = vertices[indices[i - 2]].Position - vertices[indices[i]].Position;
                Vector3 norm = Vector3.Cross(v1, v2);
                norm.Normalize();

                if (swapWinding)
                    norm *= -1;

                if (!float.IsNaN(norm.X))
                {
                    vertices[indices[i]].Normal += norm;
                    vertices[indices[i - 1]].Normal += norm;
                    vertices[indices[i - 2]].Normal += norm;
                }
                swapWinding = !swapWinding;
            }

            for (int i = 0; i < vertices.Length; i++)
                vertices[i].Normal.Normalize();
        }

        private void SetData(GraphicsDevice device)
        {
            vb = new VertexBuffer(device, numVertices * VertexPositionNormalTexture.SizeInBytes,
                BufferUsage.WriteOnly);
            vb.SetData<VertexPositionNormalTexture>(vertices);

            ib = new IndexBuffer(device, typeof(int), indices.Length, BufferUsage.WriteOnly);
            ib.SetData<int>(indices);
        }

        private void InitDefaultEffectVal()
        {
            updateWorld();

            effect.EnableDefaultLighting();
            //effect.PreferPerPixelLighting = true;
            effect.TextureEnabled = true;
            effect.Texture = dynTex;
            effect.SpecularColor = new Vector3(.4f, .4f, .4f);
            effect.AmbientLightColor = new Vector3(.2f, .2f, .2f);
            effect.PreferPerPixelLighting = true;
            effect.SpecularPower = 4f;
            //effect.FogEnabled = true;
            //effect.FogColor = Vector3.Zero;
            //effect.FogStart = 50;
            //effect.FogEnd = 500;
        }

        public void LoadDynTex(Texture2D asset)
        {
            this.dynTex = asset;
        }

        public void ReGenerateTerrain()
        {
            this.GenerateDynamicTerrainMap(129, 64, .55f); //129,100,.5
            this.GenerateIndices();
            this.GenerateVertices();
            this.GenerateNormals();

            this.SetData(dev);
        }

        /*public Terrain(GraphicsDevice device, BasicCamera camera, Texture2D texAsset, float blockScaleF, float heightScaleF)
        {
            this.blockScale = blockScaleF;
            this.heightScale = heightScaleF;

            dev = device;

            //this.LoadHeightmapFromImage(mapAsset);
            //this.GenerateRandomWalkMap(128, 128, 20);
            //this.GenerateMidFractalMap(128, 128, 255, .6f);
            //this.GenerateDynamicTerrainMap(129, 64, .55f); //(129, 64, .5f);
            
            this.LoadTexture(texAsset);

            this.ReGenerateTerrain();

            //this.world

            this.effect = new ModelEffect(device, null);
            this.effect.ActiveCamera = camera;
            this.InitDefaultEffectVal();

            //this.SetScale(scale);
        }*/

        public Terrain(GraphicsDevice device, BasicCamera camera, Texture2D texDyn, float blockScaleF, float heightScaleF)
        {
            this.blockScale = blockScaleF;
            this.heightScale = heightScaleF;

            dev = device;


            this.effect = new ModelEffect(device, null);
            this.effect.ActiveCamera = camera;
            


            //this.LoadHeightmapFromImage(mapAsset);
            //this.GenerateRandomWalkMap(128, 128, 20);
            //this.GenerateMidFractalMap(128, 128, 255, .6f);
            //this.GenerateDynamicTerrainMap(129, 64, .55f); //(129, 64, .5f);

            //this.LoadTexture(texAsset);
            this.LoadDynTex(texDyn);

            this.InitDefaultEffectVal();

            this.ReGenerateTerrain();

            //this.world

            

            //this.SetScale(scale);
        }

        public void Draw(CullMode mode)
        {
            dev.RenderState.CullMode = mode; //or null
            dev.RenderState.DepthBufferEnable = true;

            dev.VertexDeclaration = new VertexDeclaration(dev, VertexPositionNormalTexture.VertexElements);
            dev.Vertices[0].SetSource(vb, 0, VertexPositionNormalTexture.SizeInBytes);
            dev.Indices = ib;

            effect.UpdateFromActiveCamera();
            effect.Begin();
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Begin();
                // Draw the mesh
                dev.DrawIndexedPrimitives(PrimitiveType.TriangleStrip, 0, 0, numVertices, 0, numTriangles);
                pass.End();
            }
            effect.End();
        }
        public void Draw()
        {
            this.Draw(CullMode.CullCounterClockwiseFace);
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

        /*
         * public float getHeight(Vector2 pos) //(x,z)
        {
            //return Vector3.Transform(vertices[128 + vertexCountX * 128].Position, this.WorldMatrix).Y;
            // Check if the object is inside the grid
            if (isOnTerrain(pos))
            {

                pos += this.Size / 2;
                Vector2 blockPos = new Vector2((int)(pos.X / blockScale), (int)(pos.Y / blockScale));
                Vector2 posRel = pos - blockPos * blockScale;

                int vertexIndex = (int)blockPos.X + (int)blockPos.Y * vertexCountX;
                //if (vertexIndex >= vertices.Length - vertexCountX || vertexIndex < 0) return 20; //default value

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
            return 20; //default value
        }*/

        public float getHeight(Vector2 pos) //(x,z)
        {
            //return Vector3.Transform(vertices[128 + vertexCountX * 128].Position, this.WorldMatrix).Y;
            // Check if the object is inside the grid
            if (isOnTerrain(pos))
            {

                pos += this.Size / 2;
                Vector2 blockPos = new Vector2((int)(pos.X / blockScale), (int)(pos.Y / blockScale));
                Vector2 posRel = pos - blockPos * blockScale;

                int vertexIndex = (int)blockPos.X + (int)blockPos.Y * vertexCountX;
                //if (vertexIndex >= vertices.Length - vertexCountX || vertexIndex < 0) return 20; //default value

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
                    //return height2 + heightIncX * posRel.X + heightIncY * posRel.Y;
                }
                else
                {
                    heightIncX = height1 - height2;
                    heightIncY = height3 - height1;
                    //return height3 + (1.0f-pos.X
                }
                float lerpHeight = height2 + heightIncX * posRel.X + heightIncY * posRel.Y;
                return lerpHeight;

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
            return topPoint; //the last point seen above the terrain.
        }

    }
}
