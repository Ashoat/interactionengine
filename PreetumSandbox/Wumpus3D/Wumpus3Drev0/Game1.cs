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
//using System.ComponentModel;
//using Microsoft.Xna.Framework.Content.Pipeline;
//using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
//using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using System.IO;

namespace Wumpus3Drev0
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteFont font;

        int frameCount;
        TimeSpan elapsedTime = TimeSpan.Zero;

        Matrix scale;

        Model cube;

        Texture2D tex;
        Texture2D humanTex;

        BasicCamera camera;

        Terrain terrain;

        GameModel model;

        UserInterface UI;

        SkySphere skySphere;
        FrameRateCounter frc;

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

            font = Content.Load<SpriteFont>("Fonts\\font1");
            frc = new FrameRateCounter(font, GraphicsDevice);

            cube = Content.Load<Model>("Models\\sphere180");
            tex = Content.Load<Texture2D>("Images\\Amazonia"); //tex1.png //Amazonia.jpg
            Texture2D texImage = Content.Load<Texture2D>("Images\\heightImage"); //heightImage
            humanTex = Content.Load<Texture2D>("Images\\human_texture");

            //FileStream fileStream = File.OpenRead(this.Content.RootDirectory + "/map.raw");

            Vector3 cameraPos = new Vector3(75, 40, 75); //30

            camera = new BasicCamera();
            camera.SetLookAt(cameraPos, Vector3.Zero, Vector3.Up);
            //camera.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), GraphicsDevice.Viewport.AspectRatio, 1.0f, 1000.0f);
            camera.SetPerspectiveFov(45f, GraphicsDevice.Viewport.AspectRatio, 1.0f, 1000f);

            terrain = new Terrain(GraphicsDevice, camera, texImage, tex, 3f, .13f); //2
            terrain.Effect.SpecularPower = 40f;
            terrain.Effect.AmbientLightColor = Color.Black.ToVector3();
            

            graphics.GraphicsDevice.RenderState.CullMode = CullMode.None;
            graphics.GraphicsDevice.RenderState.DepthBufferEnable = true;

            graphics.GraphicsDevice.RenderState.AlphaBlendEnable = true;
            graphics.GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha;
            graphics.GraphicsDevice.RenderState.AlphaSourceBlend = Blend.One;
            graphics.GraphicsDevice.RenderState.AlphaDestinationBlend = Blend.InverseSourceAlpha;
            graphics.GraphicsDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
            graphics.GraphicsDevice.RenderState.BlendFunction= BlendFunction.Add;

            //ModelEffect modelEffect = new ModelEffect(GraphicsDevice, null, terrain.Effect);
            ModelEffect modelEffect = new ModelEffect(GraphicsDevice, null);
            //modelEffect.SpecularColor = new Vector3(.1f, .3f, .6f);
            //modelEffect.SpecularPower = 10f;
            
            modelEffect.Texture = humanTex;
            
            modelEffect.TextureEnabled = true;
            //modelEffect.Texture = null;
            modelEffect.SpecularPower = 4f;
            modelEffect.Alpha = 1f;
            modelEffect.ActiveCamera = camera;

            modelEffect.EnableDefaultLighting(); //doesn't do anything. draw method does this automatically

            //model = new GameModel(Content.Load<Model>("Models\\human4"), modelEffect, terrain, GraphicsDevice);
            model = new GameModel(Content.Load<Model>("Models\\human"), new List<Animation>(), modelEffect, terrain, GraphicsDevice);
            
            model.SetScale(3f); //3
            model.RotationOffset = MathHelper.Pi;

            Animation anim = new Animation("walk");
            anim.Frames.Add(Content.Load<Model>("Models\\human4"));
            anim.Frames.Add(Content.Load<Model>("Models\\sphere180"));
            anim.Frames.Add(Content.Load<Model>("Models\\masterchief2"));
            model.Animations.Add(anim);

            //
            //
            //

            UI = new UserInterface(camera, GraphicsDevice);
            UI.Add(this.model);
            //
            skySphere = new SkySphere(Content.Load<Model>("Models\\sphereHP"), modelEffect);
            skySphere.Orgin = Vector3.Zero;
            skySphere.SetScale(100f);

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
                    camera.ChangeAzimuth(model.Position3, Vector3.Up, 1f); //camera.ChangeAzimuth(Vector3.Zero, Vector3.Up, 1f);  //rotX += 1f;
                if (Keyboard.GetState().IsKeyDown(Keys.Down))
                    camera.ChangeAzimuth(model.Position3, Vector3.Up, -1f); //rotX -= 1f;
                if (Keyboard.GetState().IsKeyDown(Keys.Left))
                    model.Rotation += .05f;
                if (Keyboard.GetState().IsKeyDown(Keys.Right))
                    model.Rotation -= .05f;

                if (Keyboard.GetState().IsKeyDown(Keys.PageDown))
                    camera.Zoom(-1.5f);
                if (Keyboard.GetState().IsKeyDown(Keys.PageUp))
                    camera.Zoom(1.5f);

                /*if (Keyboard.GetState().IsKeyDown(Keys.A))
                    model.Move(-Vector2.UnitX / 2);  
                if (Keyboard.GetState().IsKeyDown(Keys.D))
                    model.Move(Vector2.UnitX / 2);*/

                if (Keyboard.GetState().IsKeyDown(Keys.W) || Keyboard.GetState().IsKeyDown(Keys.S))
                {
                    model.StartAnimation("walk");
                    if (Keyboard.GetState().IsKeyDown(Keys.W))
                        model.Move(-Vector2.UnitY *2);
                    if (Keyboard.GetState().IsKeyDown(Keys.S))
                        model.Move(Vector2.UnitY *2);
                }
                else
                    model.StopAnimation();


                if (Keyboard.GetState().IsKeyDown(Keys.X))
                {
                    model.StartAnimation("walk");
                    camera.SetTargetLockPosition(model.Position3);
                }


                if (Keyboard.GetState().IsKeyDown(Keys.D)) //dump debug info
                {
                    float height = terrain.getHeight(Vector2.Zero);
                    System.Diagnostics.Debug.WriteLine(height);
                }

            }

            model.Move(Vector2.Reflect(GamePad.GetState(PlayerIndex.One).ThumbSticks.Left, Vector2.UnitY));
            model.Rotation += -GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.X / 20;
            //camera.SetTargetDisplacePosition(model.Position3);
            //camera.SetTargetLockPosition(model.Position3);
            
            //

            /*
            Vector2 mouse = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);

            Vector3 near = new Vector3(mouse.X, mouse.Y, 0f);
            Vector3 far = new Vector3(mouse.X, mouse.Y, 1f);
            Matrix world = terrain.WorldMatrix;

            Vector3 nearPt = GraphicsDevice.Viewport.Unproject(near, camera.Projection, camera.View, world);
            Vector3 farPt = GraphicsDevice.Viewport.Unproject(far, camera.Projection, camera.View, world);

            Vector3 dir = farPt - nearPt;
            dir.Normalize();

            Ray rayOut = new Ray(camera.Position, dir);

            Vector3? point = terrain.RayIntersects(rayOut);
            point = point ?? Vector3.Zero;
            model.SetPosition(new Vector2(point.Value.X, point.Value.Z));
            */


            //

            UI.Update();

            //
            frc.Update(gameTime);
            //
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.CornflowerBlue, 1, 0);
            //GraphicsDevice.Clear(Color.CornflowerBlue);

            


            terrain.Draw(CullMode.None);

            GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
            GraphicsDevice.RenderState.DepthBufferEnable = true;
            model.Draw();

            frc.Draw();
            //skySphere.Draw();

            /*GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
            GraphicsDevice.RenderState.DepthBufferEnable = true;
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
                mesh.Draw();
            }*/

            base.Draw(gameTime);
        }
    }
}
