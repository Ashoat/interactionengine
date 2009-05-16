/*••••••••••••••••••••••••••••••••••••••••*\
| NTK+                                     |
| (C) Copyright Bluestone Coding 2009      |
|••••••••••••••••••••••••••••••••••••••••••|
| Built using the Interaction Engine       |
| (C) Copyright Bluestone Coding 2008      |
|••••••••••••••••••••••••••••••••••••••••••|
|           __    ___ ___  ___             |
|          /++\  | _ ) __|/ __|            |
|          \++/  | _ \__ \ (__             |
|           \/   |___/___/\___|            |
|                                          |
|••••••••••••••••••••••••••••••••••••••••••|
| GAME OBJECTS                             |
| * Terrain                          Class |
\*••••••••••••••••••••••••••••••••••••••••*/

using InteractionEngine.Constructs;
using InteractionEngine.GameWorld;
using System.Collections.Generic;
using System;
using InteractionEngine.Constructs.Datatypes;
using Microsoft.Xna.Framework;
using InteractionEngine.Client;
using InteractionEngine.Client.ThreeDimensional;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace NTKPlusGame.World {

    public class Terrain : GameObject, Graphable3D, Interactable3D {

        #region FACTORY

        // The classHash, a unique identifying string for the class. Hmm, wow, that's kind of redundant, isn't that? C# already provides such a function through reflection. Oh well.
        // Used for the factory methods called when the client receives a CREATE_NEW_OBJECT update from the server computer.
        internal const string classHash = "Terrain";

        /// <summary>
        /// The static constructor. Adds the class's factory method to the GameObject factoryList when the class is first loaded.
        /// </summary>
        static Terrain() {
            GameObject.factoryList.Add(classHash, new GameObjectFactory(makeTerrain));
        }

        /// <summary>
        /// A factory method that creates and returns a new instance of Terrain. Used by the client when the server requests it to make a new GameObject.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        /// <param name="id">This GameObject's ID.</param>
        /// <param name="reader">The PacketReader from which we will read the fields of the newly constructed GameObject.</param>
        /// <returns>A new instance of Terrain.</returns>
        static Terrain makeTerrain(LoadRegion loadRegion, int id, Microsoft.Xna.Framework.Net.PacketReader reader) {
            if (GameWorld.status != GameWorld.Status.MULTIPLAYER_CLIENT)
                throw new System.Exception("You're not a client, so why are you calling the GameObject factory method?");
            Terrain Terrain = new Terrain(loadRegion, id);
            // ORDER OF STUFF (where you used the reader to construct datatypes, used factory methods exclusively. also, construct modules and their datatypes here too.)
            return Terrain;
        }

        /// <summary>
        /// Constructs a GameObject and assigns it an ID.
        /// This is the constructor that should be used if and only if you are a MULTIPLAYER_CLIENT.
        /// Furthermore, it is only called by the GameObjectFactory method.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        /// <param name="id">This GameObject's ID.</param>
        private Terrain(LoadRegion loadRegion, int id)
            : base(loadRegion, id) {
        }

        /// <summary>
        /// Returns the class hash. 
        /// </summary>
        /// <returns>The class hash. Do we really have to tell you everything twice?</returns>
        public override string getClassHash() {
            return classHash;
        }

        #endregion



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

        Graphics3D.ModelEffect effect = new Graphics3D.ModelEffect(UserInterface3D.graphicsDevice, (EffectPool)null);

        private const string TERRAIN_CLICKED_HASH = "terrain clicked";

        /*••••••••••••••••••••••••••••••••••••••••*\
          MODULES
        \*••••••••••••••••••••••••••••••••••••••••*/

        /// <summary>
        /// Returns the Location module of this GameObject.
        /// </summary>
        /// <returns>The Location module associated with this GameObject.
        private readonly Location location;
        public Location getLocation() {
            return location;
        }

        /// <summary>
        /// Returns the Graphics module of this GameObject.
        /// </summary>
        /// <returns>The Graphics module associated with this GameObject.
        private TerrainGraphics graphics;
        public InteractionEngine.Client.Graphics getGraphics() {
            return graphics;
        }
        public Graphics3D getGraphics3D()
        {
            return graphics;
        }

        /*••••••••••••••••••••••••••••••••••••••••*\
          MEMBERS
        \*••••••••••••••••••••••••••••••••••••••••*/

        /// <summary>
        /// Constructs a Terrain.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        public Terrain(TerrainedLoadRegion loadRegion, Texture2D mapAsset, Texture2D texAsset, float blockScaleF, float heightScaleF)
            : base(loadRegion) {
            this.location = new Location(this);
            this.graphics = new TerrainGraphics(this);
            this.addEvent(TERRAIN_CLICKED_HASH, new EventMethod(onClicked));

            this.blockScale = blockScaleF;
            this.heightScale = heightScaleF;
            this.LoadHeightmapFromImage(mapAsset);
            this.LoadTexture(texAsset);

            this.GenerateIndices();
            this.GenerateVertices();
            this.GenerateNormals();

            this.SetData(UserInterface3D.graphicsDevice);
            this.InitDefaultEffectVal();

        }

        /// <summary>
        /// Gets an Event from this Interactable module.
        /// This specific implementation fetches the onClicked EventMethod for left-mouse-presses.
        /// </summary>
        /// <param name="invoker">The invoker of this Event. If you have multiple possible invokers (ie. mouse click and mouse over) then we recommend you define constants for them.</param>
        /// <param name="user">The User that invokes this Event. Needed often for associating User invokers with GameObject invokers.</param>
        /// <param name="position">The position where the interaction happened, if applicable.</param>
        /// <returns>An Event.</returns>
        public Event getEvent(int invoker, Vector3 coordinates) {
            if (invoker == UserInterface3D.MOUSEMASK_LEFT_PRESS) return new Event(this.getID(), TERRAIN_CLICKED_HASH, coordinates);
            return null;
        }

        /// <summary>
        /// Handler for when the terrain gets clicked.
        /// </summary>
        /// <param name="param">The Vector3 representing the location where the Terrain was clicked.</param>
        public void onClicked(object param) {
            NTKPlusUser.localUser.selectionFocus.addOnlyAsSecondSelection(this, param);
        }

        /// <summary>
        /// Returns the height of the terrain map at the given point.
        /// </summary>
        /// <param name="x">The x-coordinate of the point to look up.</param>
        /// <param name="y">The y-coordinate of the point to look up.</param>
        /// <returns>The height of the terrain map at the given point.</returns>
        public float getHeight(float x, float y)
        {
            //return Vector3.Transform(vertices[128 + vertexCountX * 128].Position, this.WorldMatrix).Y;
            // Check if the object is inside the grid
            Vector2 pos = new Vector2(this.getLocation().getPoint().X, this.getLocation().getPoint().Y);
            if (isOnTerrain(pos))
            {
                //pos += this.Size / 2;
                Vector2 blockPos = new Vector2((int)(x / blockScale), (int)(y / blockScale));
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
            return 20; //default value
        }


        public bool isOnTerrain(Vector2 pos) {
            if (pos.X > -this.Size.X / 2 && pos.X < this.Size.X / 2 &&
                pos.Y > -this.Size.Y / 2 && pos.Y < this.Size.Y / 2)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns true if there exists a line of sight from the start point to the target point, false otherwise.
        /// A line of sight exists if and only if the line segment going from the start point to the end point does not
        /// intersect the terrain.
        /// </summary>
        /// <param name="start">The point from which the line of sight should start, if it exists.</param>
        /// <param name="target">The point at which the line of sight should end, if it exists.</param>
        /// <returns></returns>
        public bool lineOfSightExists(Vector3 start, Vector3 target) {
            // TODO
            return true;
        }

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
        public Camera Camera
        {
            get { return effect.ActiveCamera; }
        }

        public Matrix WorldMatrix
        {
            get { return effect.World; }
            //set { effect.World = value; }
        }

        public Graphics3D.ModelEffect Effect
        {
            get { return this.effect; }
        }

        private void LoadTexture(Texture2D asset)
        {
            tex = asset;
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
            //updateWorld();

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


        public void onDraw()
        {
            UserInterface3D.graphicsDevice.VertexDeclaration = new VertexDeclaration(UserInterface3D.graphicsDevice, VertexPositionNormalTexture.VertexElements);
            UserInterface3D.graphicsDevice.Vertices[0].SetSource(vb, 0, VertexPositionNormalTexture.SizeInBytes);
            UserInterface3D.graphicsDevice.Indices = ib;

            this.Effect.UpdateFromActiveCamera();
            this.Effect.Begin();
            foreach (EffectPass pass in this.Effect.CurrentTechnique.Passes)
            {
                pass.Begin();
                // Draw the mesh
                UserInterface3D.graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, numVertices, 0, numTriangles);
                pass.End();
            }
            this.Effect.End();
        }


        class TerrainGraphics : Graphics3D
        {

            Terrain gameObject;

            public TerrainGraphics(Terrain gameObject)
                : base(gameObject, null, new ModelEffect(UserInterface3D.graphicsDevice, (EffectPool)null))
            {
                this.gameObject = gameObject;
            }

            public override void onDraw()
            {
                gameObject.onDraw();
            }
        }

    }

}