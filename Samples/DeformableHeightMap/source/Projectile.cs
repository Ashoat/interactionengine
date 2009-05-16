using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Terrain;
using Bullshoot.Code;

namespace HitsGame
{
	class Projectile : Avatar
	{
		GraphicsDeviceManager			graphics;
		Vector3							velocity;
					
		public Projectile( AvatarParams avParams ) : base( avParams )
		{
			Program.GetGraphicsManager( out this.graphics );				

			this.velocity = avParams.velocity;

			this.movement = new MovementAvatar( this, avParams.velocity );
		}

		public override void Tick( TickContext tickContext )
		{
			BoundingSphere				hitSphere;
			Vector3						hitPoint;

			base.GetBoundingSphere( out hitSphere );

			if ( Program.GetActiveScene().background.CollideSweepingSphereBackground( hitSphere.Center, hitSphere.Radius, this.velocity, out hitPoint ) )
			{
			    // need to destory this avatar and puncture a hole in the terrain.
			    Program.GetActiveScene().DestoryAvatar( this );

			    Program.GetActiveScene().background.AddExplodeHole( hitPoint );
			}
			
			base.Tick( tickContext );
		}
	}
}
