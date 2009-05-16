using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Terrain;
using CrossHair;
using HitsGame;

namespace Bullshoot.Code
{
    class PlayerParams : AvatarParams
    {
    }

    class Player : Avatar
    {
        public Camera camera = new CrossHairCamera();
        Boolean useFlyCamera = true;
        Avatar sphere;
		
        public Player(PlayerParams playerParams) : base(playerParams)
        {
            movement = new MovementFly( this );
            useFlyCamera = true;

            AvatarParams avParams = new AvatarParams();
            avParams.modelName = "sphere";
            sphere = Program.GetActiveScene().CreateAvatar(avParams);
        }
        
        public override void Tick( TickContext tickContext)
        {
            //// hit sphere test
            //Vector3 spherePos;

            //{
            //    // cast forwards
            //    spherePos = orientation.position +
            //        Vector3.TransformNormal(new Vector3(0, 0, -1000), movement.rotMtx);

            //    // hit test
            //    CollisionParameters collisionParameters = new CollisionParameters();
            //    collisionParameters.sphere.Center = sphere.orientation.position;
            //    collisionParameters.sphere.Radius = sphere.orientation.scale.X;
            //    collisionParameters.wantedPos = spherePos;

            //    CollisionResult result = Game.collision.MovingSphere(collisionParameters);

            //    sphere.orientation.position = result.collisionPoint;
            //}

			if (tickContext.padState.Buttons.Start == ButtonState.Pressed && tickContext.prevPadState.Buttons.Start == ButtonState.Released)
            {
                useFlyCamera = !useFlyCamera;

                if (useFlyCamera)
                {
                    movement = new MovementFly(this);
                }
                else
                {
                    movement = new MovementFPS(this);
                }
            }

			if (useFlyCamera)
            {
				if ( ( tickContext.padState.Buttons.A == ButtonState.Pressed ) && tickContext.prevPadState.Buttons.A != ButtonState.Pressed )
				{
				    AvatarParams			avParams = new AvatarParams();
				    Projectile				proj;

				    avParams.modelName = "projectile";
				    avParams.position = this.orientation.position + new Vector3( 0.0f, 10.0f, 0.0f );
				    avParams.scale = new Vector3(0.5f);

				    avParams.velocity = Vector3.Transform( Vector3.Forward, Matrix.CreateFromQuaternion(this.orientation.rotationQ) );

				    // fire projectile.
				    proj = new Projectile( avParams );
				    proj.Load( avParams.modelName );
				    proj.orientation.position = avParams.position;

				    Program.GetActiveScene().AddAvatarToAvatarList( proj );
				    //Program.GetActiveScene().CreateAvatar( avParams );
				}
			}
            //Game.engine.text.Print("PlayerMovement : " + movement.GetType().Name);
            //if (this.useFlyCamera)
            //{
            //    Game.engine.text.Print("Fly Camera");
            //}
            //else
            //{
            //    Game.engine.text.Print("Player Camera");
            //}

            //Game.engine.text.Print("Pos : " + orientation.position);

            base.Tick(tickContext);

            camera.Tick(tickContext, orientation.position+new Vector3(0,10,0), movement.rotMtx);
        }

		public override void Draw( DrawContext context )
		{
			this.camera.Draw( context );
		}
    }
}
