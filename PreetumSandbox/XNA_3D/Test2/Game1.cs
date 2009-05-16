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

namespace Test2
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        VertexBuffer vertexBuffer;
        BasicEffect basicEffect;

        VertexPositionColor[] vertices;

        float rotY = 45f;
        float rotX = 45f;

        Matrix scale;

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
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Color color = Color.YellowGreen;

            Vector3 cameraPos = new Vector3(0, 0, 3);
            scale = Matrix.CreateScale(.03f);

            basicEffect = new BasicEffect(GraphicsDevice, null);
            basicEffect.View = Matrix.CreateLookAt(cameraPos, Vector3.Zero, Vector3.Up);
            basicEffect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), GraphicsDevice.Viewport.AspectRatio, 1.0f, 100.0f);
            basicEffect.View = Matrix.CreateLookAt(cameraPos, Vector3.Zero, Vector3.Up);
            basicEffect.EnableDefaultLighting();

            vertices = new VertexPositionColor[2];
            vertices[0] = new VertexPositionColor(new Vector3(1, 0, 0), color);
            vertices[1] = new VertexPositionColor(new Vector3(0, 1, 0), color);

            vertexBuffer = new VertexBuffer(GraphicsDevice, vertices.Length * VertexPositionColor.SizeInBytes, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionColor>(vertices);
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

            GraphicsDevice.VertexDeclaration = new VertexDeclaration(GraphicsDevice, VertexPositionColor.VertexElements);
            GraphicsDevice.Vertices[0].SetSource(vertexBuffer, 0, VertexPositionColor.SizeInBytes);
            //GraphicsDevice.RenderState...

            basicEffect.Begin();
            foreach (EffectPass currPass in basicEffect.CurrentTechnique.Passes)
            {
                currPass.Begin();
                GraphicsDevice.DrawPrimitives(PrimitiveType.LineList, 0, 1);
                currPass.End();
            }
            basicEffect.End();

            base.Draw(gameTime);
        }
    }
}
