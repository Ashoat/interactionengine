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

        SimplePlane plane;

        int frameCount;
        TimeSpan elapsedTime = TimeSpan.Zero;

        Matrix scale;

        GameModel lab;

        List<GameModel> trees = new List<GameModel>();

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

            Vector3 cameraPos = new Vector3(400, 400, 400); //(75, 40, 75)

            camera = new BasicCamera();
            camera.SetLookAt(cameraPos, Vector3.Zero, Vector3.Up);
            //camera.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), GraphicsDevice.Viewport.AspectRatio, 1.0f, 1000.0f);
            camera.SetPerspectiveFov(45f, GraphicsDevice.Viewport.AspectRatio, 1.0f, 10000f);

            //terrain = new Terrain(GraphicsDevice, camera, Content.Load<Texture2D>("Images\\texMulti3"), 10f, 1f); //3f, .2
            terrain = new Terrain(GraphicsDevice, camera, Content.Load<Texture2D>("Images\\texMulti3"), Terrain.TerrainSize.Default);

            //terrain.Effect.SpecularPower = 40f;
            //terrain.Effect.AmbientLightColor = Color.Black.ToVector3();
            

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

            //model = new GameModel(Content.Load<Model>("Models\\human"), new List<Animation>(), modelEffect, terrain, GraphicsDevice);
            model = new GameModel(Content.Load<Model>("Models\\BoratWalk\\0"), new List<Animation>(), modelEffect, terrain, GraphicsDevice);

            model.SetScale(10f); //3
            model.RotationOffset = MathHelper.Pi;

            Animation anim = new Animation("walk");
            for (int i = 1; i <= 35; i++)
            {
                anim.Frames.Add(Content.Load<Model>("Models\\BoratWalk\\" + i.ToString()));
            }
            model.Animations.Add(anim);
            anim = new Animation("attack");
            for (int i = 0; i <= 40; i++)
            {
                anim.Frames.Add(Content.Load<Model>("Models\\BoratAttack\\" + i.ToString()));
            }
            model.Animations.Add(anim);


            //
            //
            //
            
            UI = new UserInterface(camera,terrain, GraphicsDevice);
            //UI.Add(this.model);
            Random rand = new Random();
            ModelEffect raptorEffect = new ModelEffect(GraphicsDevice, null);
            raptorEffect.Texture = Content.Load<Texture2D>("Images\\raptorTex");
            raptorEffect.ActiveCamera = camera;
            raptorEffect.TextureEnabled = true;
            raptorEffect.EnableDefaultLighting();

            for (int i = 0; i < 30; i++)
            {
                GameModel m = new GameModel(Content.Load<Model>("Models\\RaptorWalk\\0"), raptorEffect, terrain, GraphicsDevice);
                m.Position2 = new Vector2((float)rand.NextDouble() * terrain.Size.X - terrain.Size.X / 2, (float)rand.NextDouble() * terrain.Size.Y - terrain.Size.Y / 2);
                m.SetScale(2f); //3
                m.RotationOffset = MathHelper.Pi;
                Animation anim1 = new Animation("attack");
                for (int j = 0; j <= 30; j++)
                {
                    anim1.Frames.Add(Content.Load<Model>("Models\\RaptorAttack\\" + j.ToString()));
                }
                m.Animations.Add(anim1);
                anim1 = new Animation("walk");
                for (int j = 1; j <= 40; j++)
                {
                    anim1.Frames.Add(Content.Load<Model>("Models\\RaptorWalk\\" + j.ToString()));
                }
                m.Animations.Add(anim1);

                UI.Add(m);
            }
            //

            ModelEffect treeEffect = new ModelEffect(GraphicsDevice, null);
            treeEffect.TextureEnabled = true;
            treeEffect.Texture = Content.Load<Texture2D>("treemap");
            treeEffect.ActiveCamera = camera;
            Model treeM = Content.Load<Model>("tree");
            for (int i = 0; i < 30; i++)
            {
                GameModel tree = new GameModel(treeM, treeEffect, terrain, GraphicsDevice);
                tree.Position2 = new Vector2((float)rand.NextDouble() * terrain.Size.X - terrain.Size.X / 2, (float)rand.NextDouble() * terrain.Size.Y - terrain.Size.Y / 2);
                tree.SetScale(1f);
                trees.Add(tree);
            }
            
            //
            ModelEffect sky = new ModelEffect(GraphicsDevice, null);
            sky.ActiveCamera = camera;
            sky.TextureEnabled = true;
            sky.AmbientLightColor = new Vector3(.6f, .6f, .6f);
            sky.Texture = Content.Load<Texture2D>("Models\\cloudMap");
            skySphere = new SkySphere(Content.Load<Model>("Models\\dome"), sky);
            skySphere.Orgin = new Vector3(0, -1500, 0);
            skySphere.SetScale(7000f); //5000

            //
           
            //
            //lab = new GameModel(Content.Load<Model>("Models\\Lab"), modelEffect, terrain, GraphicsDevice);
            //lab.SetScale(10f);


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
                    camera.Zoom(-5.5f);
                if (Keyboard.GetState().IsKeyDown(Keys.PageUp))
                    camera.Zoom(5.5f);

                /*if (Keyboard.GetState().IsKeyDown(Keys.A))
                    model.Move(-Vector2.UnitX / 2);  
                if (Keyboard.GetState().IsKeyDown(Keys.D))
                    model.Move(Vector2.UnitX / 2);*/

                if (Keyboard.GetState().IsKeyDown(Keys.W) || Keyboard.GetState().IsKeyDown(Keys.S))
                {
                    model.StartAnimation("walk");
                    if (Keyboard.GetState().IsKeyDown(Keys.W))
                        model.Move(-Vector2.UnitY /1);
                    if (Keyboard.GetState().IsKeyDown(Keys.S))
                        model.Move(Vector2.UnitY /1);
                    //System.Diagnostics.Debug.WriteLine(model.Position3.Y);
                }
                else
                    model.StopAnimation();


                if (Keyboard.GetState().IsKeyDown(Keys.X))
                {
                    model.StartAnimation("walk");
                    camera.SetTargetLockPosition(model.Position3);
                }
                if (Keyboard.GetState().IsKeyDown(Keys.R))
                {
                    model.StartAnimation("attack");
                }

                if (Keyboard.GetState().IsKeyDown(Keys.D)) //dump debug info
                {
                    float height = terrain.getHeight(Vector2.Zero);
                    System.Diagnostics.Debug.WriteLine(height);
                }

                if (Keyboard.GetState().IsKeyDown(Keys.G))
                    terrain.ReGenerateTerrain();
                if (Keyboard.GetState().IsKeyDown(Keys.F))
                    terrain.FliterCurrentTerrain(.5f);
            }

            model.Move(Vector2.Reflect(GamePad.GetState(PlayerIndex.One).ThumbSticks.Left, Vector2.UnitY));
            model.Rotation += -GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.X / 20;
            camera.SetTargetDisplacePosition(model.Position3);
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
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1, 0);
            //GraphicsDevice.Clear(Color.CornflowerBlue);

            skySphere.Draw();


            terrain.Draw();
            

            //GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
            //GraphicsDevice.RenderState.DepthBufferEnable = true;
            model.Draw();
            //lab.Draw();

            foreach (GameModel gm in trees)
            {
                gm.Draw();
            }
            UI.Draw();

            //plane.Draw();

            frc.Draw();
           
            

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
