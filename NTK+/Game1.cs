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
using System.IO;

namespace HeightMapTest1
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        BasicEffect basicEffect;

        VertexPositionNormalTexture[] vertices;

        float rotY = 45f;
        float rotX = 45f;

        Matrix scale;

        byte[] heightMap;

        VertexBuffer vb;
        IndexBuffer ib;
        int numVertices;
        int numTriangles;

        int vertexCountX;
        int vertexCountZ;
        float blockScale;
        float heightScale;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            //spriteBatch = new SpriteBatch(GraphicsDevice);

            //
            //effectz
            //
            Vector3 cameraPos = new Vector3(0, 20, 10);
            scale = Matrix.CreateScale(.5f);

            basicEffect = new BasicEffect(GraphicsDevice, null);
            basicEffect.View = Matrix.CreateLookAt(cameraPos, Vector3.Zero, Vector3.Up);
            basicEffect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), GraphicsDevice.Viewport.AspectRatio, 1.0f, 1000.0f);
            basicEffect.World = scale * Matrix.CreateRotationY(MathHelper.ToRadians(rotY)) * Matrix.CreateRotationX(MathHelper.ToRadians(rotX));
            basicEffect.EnableDefaultLighting();


            //
            //load da heigh map 
            //
            
            heightMap = new byte[fileStream.Length];
            fileStream.Read(heightMap, 0, (int)fileStream.Length);
            fileStream.Close();

            numVertices = heightMap.Length;
            vertexCountX = (int)Math.Sqrt(numVertices);
            vertexCountZ = heightMap.Length / vertexCountX;

            numTriangles = (vertexCountX - 1) * (vertexCountZ - 1) * 2;


            //
            //Index!
            //
            int numIndices = numTriangles * 3;
            int[] indices = new int[numIndices];

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
            
            //
            //gen those vertices!
            //
            blockScale = .10f;
            heightScale = .05f;

            float terrainWidth = (vertexCountX - 1) * blockScale;
            float terrainDepth = (vertexCountZ - 1) * blockScale;
            float halfTerrainWidth = terrainWidth * 0.5f;
            float halfTerrainDepth = terrainDepth * 0.5f;

            vertices = new VertexPositionNormalTexture[numVertices];
 
           
            int vertexCount = 0;
            for (float i = -halfTerrainDepth; i <= halfTerrainDepth; i += blockScale)
            {
                for (float j = -halfTerrainWidth; j <= halfTerrainWidth; j += blockScale)
                {
                    vertices[vertexCount].Position = new Vector3(j, heightMap[vertexCount] * heightScale, i);
                    vertices[vertexCount].TextureCoordinate = new Vector2(0, 0);
                    vertexCount++;
                }
            }


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


            //
            //Set data
            //
            vb = new VertexBuffer(GraphicsDevice, numVertices * VertexPositionNormalTexture.SizeInBytes,
                BufferUsage.WriteOnly);
            vb.SetData<VertexPositionNormalTexture>(vertices);

            ib = new IndexBuffer(GraphicsDevice, numTriangles * 3 * sizeof(int), BufferUsage.WriteOnly,
                IndexElementSize.ThirtyTwoBits);
            ib.SetData<int>(indices);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            if (Keyboard.GetState().GetPressedKeys() != null)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Right))
                    rotY += 1f;
                if (Keyboard.GetState().IsKeyDown(Keys.Left))
                    rotY -= 1f;
                if (Keyboard.GetState().IsKeyDown(Keys.Up))
                    rotX += 1f;
                if (Keyboard.GetState().IsKeyDown(Keys.Down))
                    rotX -= 1f;

                basicEffect.World = scale * Matrix.CreateRotationY(MathHelper.ToRadians(rotY)) * Matrix.CreateRotationX(MathHelper.ToRadians(rotX));
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            GraphicsDevice.VertexDeclaration = new VertexDeclaration(GraphicsDevice, VertexPositionNormalTexture.VertexElements);
            GraphicsDevice.Vertices[0].SetSource(vb, 0, VertexPositionNormalTexture.SizeInBytes);
            GraphicsDevice.Indices = ib;

            basicEffect.Begin();
            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Begin();
                // Draw the mesh
                GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, numVertices, 0, numTriangles);
                pass.End();
            }
            basicEffect.End();

            base.Draw(gameTime);
        }
    }
}
