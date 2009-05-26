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
using InteractionEngine;
using System;
using Microsoft.Xna.Framework;
using InteractionEngine.UserInterface.ThreeDimensional;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using InteractionEngine.EventHandling;
using InteractionEngine.Networking;

namespace NTKPlusGame.World {

    public class Terrain : GameObject, Graphable3D, Interactable3D {

        #region FACTORY

        /// <summary>
        /// All GameObjects need a parameterless constructor for calling by GameObject.createGameObject() and GameObject.createFromUpdate().
        /// NEVER CALL THIS! This constructor is exclusively for use by the InteractionEngine. If anyone else calls it things will break.
        /// If you want to construct this object, use GameObject.createGameObject(LoadRegion).
        /// </summary>
        public Terrain() {
        }

        // The classHash, a unique identifying string for the class. Hmm, wow, that's kind of redundant, isn't that? C# already provides such a function through reflection. Oh well.
        // Used for the factory methods called when the client receives a CREATE_NEW_OBJECT update from the server computer.
        public const string realHash = "Terrain";
        public override string classHash {
            get { return realHash; }
        }

        /// <summary>
        /// The static constructor. Adds the class's factory method to the GameObject factoryList when the class is first loaded.
        /// </summary>
        static Terrain() {
            GameObject.factoryList.Add(realHash, new GameObjectFactory(GameObject.createFromUpdate<Terrain>));
        }

        #endregion



        private VertexPositionNormalTexture[] vertices;
        private int[] indices;

        private byte[] heightMap;

        private VertexBuffer vb;
        private IndexBuffer ib;
        private int numVertices;
        private int numTriangles;

        private int vertexCountX;
        private int vertexCountZ;
        private float blockScale;
        private float heightScale;

        private Texture2D tex;

        private ModelEffect effect;

        private const string TERRAIN_CLICKED_HASH = "terrain clicked";

        /*••••••••••••••••••••••••••••••••••••••••*\
          MODULES
        \*••••••••••••••••••••••••••••••••••••••••*/

        /// <summary>
        /// Returns the Location module of this GameObject.
        /// </summary>
        /// <returns>The Location module associated with this GameObject.
        private Location location;
        public Location getLocation() {
            return location;
        }

        /// <summary>
        /// Returns the Graphics module of this GameObject.
        /// </summary>
        /// <returns>The Graphics module associated with this GameObject.
        private TerrainGraphics graphics;
        public InteractionEngine.UserInterface.Graphics getGraphics() {
            return graphics;
        }
        public Graphics3D getGraphics3D() {
            return graphics;
        }

        /*••••••••••••••••••••••••••••••••••••••••*\
          MEMBERS
        \*••••••••••••••••••••••••••••••••••••••••*/

        /// <summary>
        /// Constructs a Terrain.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        public override void construct() {
            this.location = new Location(this);
            this.graphics = new TerrainGraphics(this);
            this.addEventMethod(TERRAIN_CLICKED_HASH, new EventMethod(onClicked));

        }

        public void initialize(float blockScaleF, float heightScaleF, LoadRegion terrainedLoadRegion) {
            this.blockScale = blockScaleF;
            this.heightScale = heightScaleF;
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
            if (invoker == UserInterface3D.MOUSEMASK_LEFT_PRESS) return new Event(this.id, TERRAIN_CLICKED_HASH, coordinates);
            return null;
        }

        /// <summary>
        /// Handler for when the terrain gets clicked.
        /// </summary>
        /// <param name="param">The Vector3 representing the location where the Terrain was clicked.</param>
        public void onClicked(Client client, object param) {
            DebugSphere sphere = GameObject.createGameObject<DebugSphere>(this.getLoadRegion());
            sphere.setPosition((Vector3)param, 1f);
            Vector3 position = sphere.getLocation().Position;
            Console.WriteLine(position.Y + ", " + this.getHeight(position.X, position.Z));
            NTKPlusUser.localUser.selectionFocus.addOnlyAsSecondSelection(this, param);
        }

        /// <summary>
        /// Returns the height of the terrain map at the given point.
        /// </summary>
        /// <param name="x">The x-coordinate of the point to look up.</param>
        /// <param name="y">The z-coordinate of the point to look up.</param>
        /// <returns>The height of the terrain map at the given point.</returns>
        public float getHeight(float x, float y) {
            //return Vector3.Transform(vertices[128 + vertexCountX * 128].Position, this.WorldMatrix).Y;
            // Check if the object is inside the grid
            Vector2 pos = new Vector2(x, y);
            if (isOnTerrain(pos)) {
                pos += this.Size / 2;
                Vector2 blockPos = new Vector2((int)(pos.X / blockScale), (int)(pos.Y / blockScale));
                Vector2 posRel = pos - blockPos * blockScale;

                int vertexIndex = (int)blockPos.X + (int)blockPos.Y * vertexCountX;
                if (vertexIndex >= vertices.Length - vertexCountX || vertexIndex < 0) return -1; // default value
                float height1 = vertices[vertexIndex + 1].Position.Y;
                float height2 = vertices[vertexIndex].Position.Y;
                float height3 = vertices[vertexIndex + vertexCountX + 1].Position.Y;
                float height4 = vertices[vertexIndex + vertexCountX].Position.Y;

                bool aboveLowerTri = posRel.X < posRel.Y;
                float heightIncX, heightIncY;
                if (aboveLowerTri) {
                    heightIncX = height3 - height4;
                    heightIncY = height4 - height2;
                } else {
                    heightIncX = height1 - height2;
                    heightIncY = height3 - height1;
                }
                float lerpHeight = height2 + heightIncX * posRel.X + heightIncY * posRel.Y;
                return lerpHeight;
            }
            return -1; //default value
        }


        public bool isOnTerrain(Vector2 pos) {
            if (pos.X > -this.Size.X / 2 && pos.X < this.Size.X / 2 &&
                pos.Y > -this.Size.Y / 2 && pos.Y < this.Size.Y / 2) {
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

        public Vector2 Size {
            get {
                return new Vector2(vertexCountX * blockScale, vertexCountZ * blockScale);
            }
        }

        public VertexBuffer VertexBuffer {
            get { return vb; }
        }
        public IndexBuffer IndexBuffer {
            get { return ib; }
        }
        public Texture2D Texture {
            get { return tex; }
            set { tex = value; }
        }
        public Camera Camera {
            get { return effect.ActiveCamera; }
        }

        public Matrix WorldMatrix {
            get { return effect.World; }
            //set { effect.World = value; }
        }

        public ModelEffect Effect {
            get { return this.effect; }
        }

        private void LoadTexture(Texture2D asset) {
            tex = asset;
        }

        private void LoadHeightmapFromImage(Texture2D asset) {
            Color[] map = new Color[asset.Width * asset.Height];
            asset.GetData<Color>(map);
            heightMap = new byte[map.Length];
            for (int i = 0; i < heightMap.Length; i++) {
                heightMap[i] = map[i].R;
            }

            vertexCountX = asset.Width;
            vertexCountZ = asset.Height;
            numTriangles = (vertexCountX - 1) * (vertexCountZ - 1) * 2;
            numVertices = map.Length;

            //texImage.Dispose();
        }

        private void LoadHeightmapFromRaw(FileStream fileStream) {
            //FileStream fileStream = File.OpenRead(this.Content.RootDirectory + "/" + filename);
            heightMap = new byte[fileStream.Length];
            fileStream.Read(heightMap, 0, (int)fileStream.Length);
            fileStream.Close();

            numVertices = heightMap.Length;
            vertexCountX = (int)Math.Sqrt(numVertices);
            vertexCountZ = heightMap.Length / vertexCountX;
            numTriangles = (vertexCountX - 1) * (vertexCountZ - 1) * 2;
        }

        private void GenerateIndices() {
            indices = new int[vertexCountX * 2 * (vertexCountZ - 1)];

            int i = 0;
            int z = 0;

            while (z < vertexCountZ - 1) {
                for (int x = vertexCountZ - 1; x >= 0; x--) {
                    indices[i++] = x + z * vertexCountX;
                    indices[i++] = x + (z + 1) * vertexCountX;
                }
                z++;

                if (z >= vertexCountZ - 1) break; //or continue. it really doesnt matter

                for (int x = 0; x < vertexCountZ; x++) {
                    indices[i++] = x + (z + 1) * vertexCountX;
                    indices[i++] = x + z * vertexCountX;
                }
                z++;
            }
        }


        private void GenerateVertices() {
            vertices = new VertexPositionNormalTexture[numVertices];

            int vertexCount = 0;
            for (float i = 0; i < vertexCountZ; i++) {
                for (float j = 0; j < vertexCountX; j++) {
                    vertices[vertexCount].Position = new Vector3((j - vertexCountX / 2) * blockScale, heightMap[vertexCount] * heightScale, (i - vertexCountZ / 2) * blockScale);
                    vertices[vertexCount].TextureCoordinate = new Vector2(j / vertexCountX, i / vertexCountZ);
                    vertexCount++;
                }
            }
        }

        private void GenerateNormals() {
            //
            //Yo! Iz mah normalz!
            //pg 277.
            /*
             * Each vertex can be shared by multiple triangles.
             * foreach triangle, add the normal of the tringle to the normals of each vertex. then normalize
             */
            for (int i = 0; i < indices.Length; i += 3) {
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

            for (int i = 0; i < vertices.Length; i++) {
                vertices[i].Normal.Normalize();
            }
        }


        private void SetData(GraphicsDevice device) {
            vb = new VertexBuffer(device, numVertices * VertexPositionNormalTexture.SizeInBytes,
                BufferUsage.WriteOnly);
            vb.SetData<VertexPositionNormalTexture>(vertices);

            ib = new IndexBuffer(device, typeof(int), indices.Length, BufferUsage.WriteOnly);
            ib.SetData<int>(indices);
        }

        private void InitDefaultEffectVal() {

            effect = new ModelEffect();
            effect.Initialize(UserInterface3D.user.camera);
            //updateWorld();

            //effect.EnableDefaultLighting();
            //effect.PreferPerPixelLighting = true;
            effect.TextureEnabled = true;
            effect.Texture = tex;
            effect.SpecularColor = new Vector3(.4f, .4f, .4f);
            //effect.PreferPerPixelLighting = true;
            effect.SpecularPower = 30f;
            effect.CommitProperties();
            //effect.FogEnabled = true;
            //effect.FogColor = Vector3.Zero;
            //effect.FogStart = 50;
            //effect.FogEnd = 3000;
        }

        public void onDraw() {

            UserInterface3D.graphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace; //or null
            UserInterface3D.graphicsDevice.RenderState.DepthBufferEnable = true;

            UserInterface3D.graphicsDevice.VertexDeclaration = new VertexDeclaration(UserInterface3D.graphicsDevice, VertexPositionNormalTexture.VertexElements);
            UserInterface3D.graphicsDevice.Vertices[0].SetSource(vb, 0, VertexPositionNormalTexture.SizeInBytes);
            UserInterface3D.graphicsDevice.Indices = ib;

            effect.UpdateFromActiveCamera();
            BasicEffect actualEffect = effect.Effect;
            actualEffect.Begin();
            foreach (EffectPass pass in actualEffect.CurrentTechnique.Passes) {
                pass.Begin();
                // Draw the mesh
                UserInterface3D.graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleStrip, 0, 0, numVertices, 0, numTriangles);
                pass.End();
            }
            actualEffect.End();
        }

        private void loadContent() {

            Game.TestGame.initializeStuff();

            Texture2D texAsset = UserInterface3D.content.Load<Texture2D>("Amazonia"); //tex1.png //Amazonia.jpg
            Texture2D mapAsset = UserInterface3D.content.Load<Texture2D>("heightImage"); //heightImage

            this.LoadHeightmapFromImage(mapAsset);
            this.LoadTexture(texAsset);

            this.GenerateIndices();
            this.GenerateVertices();
            this.GenerateNormals();

            this.SetData(UserInterface3D.graphicsDevice);
            this.InitDefaultEffectVal();


            Console.WriteLine(this.getHeight(0, 0) + "");
        }

        public float getHeight(Vector3 pos) {
            return getHeight(pos.X, pos.Z);
        }
        public bool isOnTerrain(Vector3 pos) {
            return isOnTerrain(new Vector2(pos.X, pos.Z));
        }

        private Vector3? intersectionPoint(Ray ray) {

            Vector3 rayStep = ray.Direction * blockScale * .5f;
            Vector3 currPoint = ray.Position;
            Vector3 prevPoint = currPoint;

            currPoint += rayStep;

            while (currPoint.Y > this.getHeight(currPoint) && this.isOnTerrain(currPoint)) {
                prevPoint = currPoint;
                currPoint += rayStep;
            }
            //linear search over

            Vector3 topPoint = prevPoint;
            Vector3 botPoint = currPoint;
            for (int i = 0; i < 8; i++) {
                Vector3 midPoint = (topPoint + botPoint) / 2;
                if (midPoint.Y < this.getHeight(midPoint))
                    botPoint = midPoint;
                else
                    topPoint = midPoint;
            }
            Vector3 finalPoint = (topPoint + botPoint) / 2;
            return finalPoint; //the last point seen above the terrain.
        }

        public float? intersects(Ray ray) {
            Vector3? intersectionPoint = this.intersectionPoint(ray);
            if (intersectionPoint == null) return null;
            else return (intersectionPoint.Value - ray.Position).Length();
        }


        class TerrainGraphics : Graphics3DModel {

            private Terrain gameObject;

            public TerrainGraphics(Terrain gameObject)
                : base(gameObject, new ModelEffect(), null) {
                this.gameObject = gameObject;
            }

            public override void onDraw() {
                gameObject.onDraw();
            }

            public override void loadContent() {
                // TODO: this.
                gameObject.loadContent();
                //base.loadContent();
            }

            public override float? intersects(Ray ray) {
                return gameObject.intersects(ray);
            }

            public override Vector3? intersectionPoint(Ray ray) {
                return gameObject.intersectionPoint(ray);
            }
        }

    }

}