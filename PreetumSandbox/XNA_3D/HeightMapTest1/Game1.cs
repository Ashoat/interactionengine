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
//using Microsoft.Xna.Framework.Content.Pipeline;
//using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
//using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using System.IO;

namespace HeightMapTest1
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;


        Matrix scale;

        Model cube;

        Texture2D tex;

        BasicCamera camera;

        Terrain terrain;

        GameModel model;

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
            this.IsMouseVisible = true;
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

            cube = Content.Load<Model>("Models\\sphere180");
            tex = Content.Load<Texture2D>("Amazonia"); //tex1.png //Amazonia.jpg
            Texture2D texImage = Content.Load<Texture2D>("heightImage"); //heightImage
            //FileStream fileStream = File.OpenRead(this.Content.RootDirectory + "/map.raw");
            
            Vector3 cameraPos = new Vector3(75, 40, 75); //30

            camera = new BasicCamera();
            camera.SetLookAt(cameraPos, Vector3.Zero, Vector3.Up);
            camera.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), GraphicsDevice.Viewport.AspectRatio, 1.0f, 1000.0f);      

            terrain = new Terrain(GraphicsDevice, camera, texImage, tex, 2f, .1f);
            terrain.Effect.SpecularPower = 2f;

            graphics.GraphicsDevice.RenderState.CullMode = CullMode.None;

            //ModelEffect modelEffect = new ModelEffect(GraphicsDevice, null, terrain.Effect);
            ModelEffect modelEffect = new ModelEffect(GraphicsDevice, null);
            //modelEffect.SpecularColor = new Vector3(.1f, .3f, .6f);
            //modelEffect.SpecularPower = 10f;
            modelEffect.Texture = Content.Load<Texture2D>("Images\\human_texture");
            modelEffect.TextureEnabled = true;
            //modelEffect.Texture = null;
            modelEffect.SpecularPower = 1f;
            //modelEffect.EnableDefaultLighting(); //doesn't do anything. draw method does this automatically
            
            //model = new GameModel(Content.Load<Model>("human4"), modelEffect, terrain, GraphicsDevice);
            model = new GameModel(Content.Load<Model>("Models\\Borat"), modelEffect, terrain, GraphicsDevice);
            model.SetScale(3f);
            model.RotationOffset = MathHelper.Pi;

            //
            //
            //
            
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
                if (Keyboard.GetState().IsKeyDown(Keys.E))
                    camera.RotateUponAxis(model.Position3, Vector3.Up, 1f); //rotY += 1f;
                if (Keyboard.GetState().IsKeyDown(Keys.Q))
                    camera.RotateUponAxis(model.Position3, Vector3.Up, -1f); //rotY -= 1f;
                if (Keyboard.GetState().IsKeyDown(Keys.Up))
                    camera.RotateUponAxis(model.Position3, Vector3.Right, -1f); //camera.ChangeAzimuth(Vector3.Zero, Vector3.Up, 1f);  //rotX += 1f;
                if (Keyboard.GetState().IsKeyDown(Keys.Down))
                    camera.ChangeAzimuth(model.Position3, Vector3.Up, -1f); //rotX -= 1f;
                if (Keyboard.GetState().IsKeyDown(Keys.Left))
                    model.Rotation += .05f;
                if (Keyboard.GetState().IsKeyDown(Keys.Right))
                    model.Rotation -= .05f;
               
                if (Keyboard.GetState().IsKeyDown(Keys.PageDown)) { }
                    //terrain.Scale += .01f;
                if (Keyboard.GetState().IsKeyDown(Keys.PageUp)) { }
                    //terrain.Scale -= .01f;
                if (Keyboard.GetState().IsKeyDown(Keys.A))
                    model.Move(-Vector2.UnitX / 3);  //model.Position2 -= Vector2.UnitX / 3;
                if (Keyboard.GetState().IsKeyDown(Keys.D))
                    model.Move(Vector2.UnitX / 3); //model.Position2 += Vector2.UnitX / 3;
                if (Keyboard.GetState().IsKeyDown(Keys.W))
                    model.Move(-Vector2.UnitY / 3); //model.Position2 -= Vector2.UnitY / 3;
                if (Keyboard.GetState().IsKeyDown(Keys.S))
                    model.Move(Vector2.UnitY / 3);  //model.Position2 += Vector2.UnitY / 3;           
                
            }

            model.Move(Vector2.Reflect(GamePad.GetState(PlayerIndex.One).ThumbSticks.Left,Vector2.UnitY));
            model.Rotation += -GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.X / 20;
            //camera.SetTargetDisplacePosition(model.Position3);
            //camera.SetTargetLockPosition(model.Position3);
            //

            Vector2 mouse = new Vector2(Mouse.GetState().X,Mouse.GetState().Y);

            Vector3 near = new Vector3(mouse.X, mouse.Y, 0f);
            Vector3 far = new Vector3(mouse.X, mouse.Y, 1f);
            Matrix world = terrain.WorldMatrix;

            Vector3 nearPt = GraphicsDevice.Viewport.Unproject(near, camera.Projection, camera.View, world);
            Vector3 farPt = GraphicsDevice.Viewport.Unproject(far, camera.Projection, camera.View, world);

            Vector3 dir = farPt - nearPt;
            dir.Normalize();

            Ray rayOut = new Ray(camera.Position, dir);

            /*if (model.RayIntersects(rayOut))
                model.Move(-Vector2.UnitY / 3);*/

            Vector3? point = terrain.RayIntersects(rayOut);
            point = point ?? Vector3.Zero;
            model.SetPosition(new Vector2(point.Value.X, point.Value.Z));

            //
            base.Update(gameTime);
        }
        
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            terrain.Draw();
            model.Draw();

            Matrix cubeWorld = Matrix.CreateScale(model.BoundingSphere.Radius) * Matrix.CreateTranslation(model.BoundingSphere.Center);
            foreach (ModelMesh mesh in cube.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.View = terrain.Camera.View;
                    effect.Projection = terrain.Camera.Projection;
                    effect.World = cubeWorld;
                }
                //mesh.Draw();
            }

            base.Draw(gameTime);
        }
    }
}
