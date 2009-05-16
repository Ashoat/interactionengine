using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Terrain;

namespace Bullshoot.Code
{
    public class AvatarParams
    {
        public Vector3      position;
        public Quaternion   rotationQ;
        public Vector3      scale;
        public String       modelName;

		public Vector3		velocity;
    }

    public class Avatar : Object
    {
        Model model;
        public Movement movement;
		BoundingSphere	hitSphere;

        public Avatar(AvatarParams avParams)
        {
            this.orientation = new Orientation(avParams.position, avParams.rotationQ);
            this.orientation.scale = avParams.scale;
            this.orientation.UpdateMtx();

            this.movement = new Movement(this);
        }

        public void Load(String filename)
        {
			ContentManager			content;

			Program.GetContentManager( out content );

            model = content.Load<Model>("models/" + filename);

			hitSphere = new BoundingSphere();

			foreach ( ModelMesh mesh in model.Meshes )
			{
				hitSphere = BoundingSphere.CreateMerged( hitSphere, mesh.BoundingSphere );
			}
        }

		public void GetBoundingSphere( out BoundingSphere hitSphere )
		{
			Vector3				radius;
			Vector3				position;
			Matrix				transformation;

			radius = new Vector3( this.hitSphere.Radius );
			position = this.hitSphere.Center;

			// scale the radius
			radius = Vector3.Transform( radius, Matrix.CreateScale( this.orientation.scale ) );
			
			transformation = Matrix.CreateScale( this.orientation.scale ) * Matrix.CreateTranslation( this.orientation.position );
			position = Vector3.Transform( position, transformation );

			hitSphere = new BoundingSphere( position, radius.X );
		}

        override public void Draw(DrawContext context)
        {
            Matrix[] transforms = new Matrix[model.Bones.Count];

            model.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();

                    Matrix worldMtx = transforms[mesh.ParentBone.Index] * this.orientation.BasisMtx;

                    effect.View = context.camera.viewMtx;
                    effect.Projection = context.camera.projMtx;
                    effect.World = worldMtx;
                }
                mesh.Draw();
            }
        }

        override public void Tick(TickContext tickContext)
        {
            movement.Tick(tickContext);

            this.orientation.UpdateMtx();
        }
    }
}
