
#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

using Bullshoot;
using Bullshoot.Code;

#endregion

namespace Terrain
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager           graphics;
        ContentManager                  content;
        //Matrix                          worldMatrix;
        //Matrix                          viewMatrix;
        //Matrix                          projectionMatrix;
        //BasicEffect                     basicEffect;
        //FractalTerrain3D                  terrain;
        Scene                           scene;

        public Game1()
        {
            graphics = new GraphicsDeviceManager( this );
            content = new ContentManager( Services );
            scene = new Scene();
        }

		public Scene GetActiveScene()
		{
			return this.scene;
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

            //worldMatrix = Matrix.Identity;
            //viewMatrix = Matrix.CreateLookAt( new Vector3( 0, 5.0f, 5.0f ), Vector3.Zero, Vector3.Up );
            //projectionMatrix = Matrix.CreatePerspectiveFieldOfView( 
            //                    MathHelper.ToRadians( 45 ), 
            //                    ( float )graphics.GraphicsDevice.Viewport.Width / ( float )graphics.GraphicsDevice.Viewport.Height, 
            //                    1.0f, 100.0f
            //                );
            //basicEffect = new BasicEffect(graphics.GraphicsDevice, null);
            //basicEffect.Alpha = 1.0f;
            //basicEffect.DiffuseColor = new Vector3(1.0f, 0.0f, 1.0f);
            //basicEffect.SpecularColor = new Vector3(0.25f, 0.25f, 0.25f);
            //basicEffect.SpecularPower = 5.0f;
            //basicEffect.AmbientLightColor = new Vector3(0.75f, 0.75f, 0.75f);

            //basicEffect.DirectionalLight0.Enabled = true;
            //basicEffect.DirectionalLight0.DiffuseColor = Vector3.One;
            //basicEffect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(1.0f, -1.0f, -1.0f));
            //basicEffect.DirectionalLight0.SpecularColor = Vector3.One;

            //basicEffect.DirectionalLight1.Enabled = true;
            //basicEffect.DirectionalLight1.DiffuseColor = new Vector3(0.5f, 0.5f, 0.5f);
            //basicEffect.DirectionalLight1.Direction = Vector3.Normalize(new Vector3(-1.0f, -1.0f, 1.0f));
            //basicEffect.DirectionalLight1.SpecularColor = new Vector3(0.5f, 0.5f, 0.5f);

            //basicEffect.LightingEnabled = true;

            //basicEffect.World = worldMatrix;
            //basicEffect.View = viewMatrix;
            //basicEffect.Projection = projectionMatrix;

            //terrain = new FractalTerrain( this.graphics );

            base.Initialize();
        }


        /// <summary>
        /// Load your graphics content.  If loadAllContent is true, you should
        /// load content from both ResourceManagementMode pools.  Otherwise, just
        /// load ResourceManagementMode.Manual content.
        /// </summary>
        /// <param name="loadAllContent">Which type of content to load.</param>
        protected override void LoadGraphicsContent( bool loadAllContent )
        {
            if ( loadAllContent )
            {
                // TODO: Load any ResourceManagementMode.Automatic content
                scene.Load();
            }

            // TODO: Load any ResourceManagementMode.Manual content
        }

        public void GetContentManager( out ContentManager content )
        {
            content = this.content;
        }
        
        public void GetGraphicsManager( out GraphicsDeviceManager graphics )
        {
            graphics = this.graphics;
        }
        /// <summary>
        /// Unload your graphics content.  If unloadAllContent is true, you should
        /// unload content from both ResourceManagementMode pools.  Otherwise, just
        /// unload ResourceManagementMode.Manual content.  Manual content will get
        /// Disposed by the GraphicsDevice during a Reset.
        /// </summary>
        /// <param name="unloadAllContent">Which type of content to unload.</param>
        protected override void UnloadGraphicsContent( bool unloadAllContent )
        {
            if ( unloadAllContent == true )
            {
                content.Unload();
            }
        }


        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update( GameTime gameTime )
        {
            // Allows the default game to exit on Xbox 360 and Windows
            if ( GamePad.GetState( PlayerIndex.One ).Buttons.Back == ButtonState.Pressed )
                this.Exit();

            // TODO: Add your update logic here

            scene.Tick(gameTime);

            base.Update( gameTime );
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw( GameTime gameTime )
        {
            graphics.GraphicsDevice.Clear( Color.CornflowerBlue );

            // TODO: Add your drawing code here

            scene.Draw();

            //graphics.GraphicsDevice.RenderState.FillMode = FillMode.WireFrame;

            //this.basicEffect.Begin();

            //foreach ( EffectPass pass in this.basicEffect.CurrentTechnique.Passes )
            //{
            //    pass.Begin();

                
            //    this.terrain.Draw();

            //    base.Draw( gameTime );

            //    pass.End();
            //}

            //this.basicEffect.End();
        }
    }
}