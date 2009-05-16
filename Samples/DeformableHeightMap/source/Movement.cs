using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Terrain;

namespace Bullshoot.Code
{
    public class Movement
    {
        protected Avatar avatar;
        protected Location location = Location.Air;
        protected Vector3 defaultGravity = new Vector3(0, -10*100, 0);
        public Matrix rotMtx = Matrix.Identity;
        public Boolean invertY = true;

        public enum Location
        {
            Ground,
            Air,
            Water,
        }

        public Movement(Avatar avatar)
        {
            this.avatar = avatar;
            avatar.movement = this;
		}


        public virtual void Tick(TickContext tickContext)
        {
            rotMtx = Matrix.CreateFromQuaternion(avatar.orientation.rotationQ);
        }
    }

	public class MovementAvatar : Movement
	{
		Vector3					velocity;

		public MovementAvatar( Avatar avatar, Vector3 velocity ) : base( avatar )
		{
			this.velocity = velocity;
		}

		public void SetVelocity( Vector3 velocity )
		{
			this.velocity = velocity;
		}

		public override void Tick(TickContext tickContext)
        {
			// Update Avatar
            avatar.orientation.position += this.velocity;
		}
	}

    public class MovementFly : Movement
    {
        public Rotation speed;
        public Rotation rot;

        public MovementFly(Avatar avatar)
            : base(avatar)
        {
            speed = new Rotation(3.0f, 3.0f, 2.0f);
        }

        public override void Tick(TickContext tickContext)
        {
            Vector3 velocity = new Vector3();
            float yawDelta;
            float pitchDelta;

            // Update Rotation
            yawDelta = -tickContext.padState.ThumbSticks.Right.X * 0.01f * speed.yaw;

            if (invertY)
            {
                pitchDelta = -tickContext.padState.ThumbSticks.Right.Y * 0.01f * speed.pitch;
            }
            else
            {
                pitchDelta = tickContext.padState.ThumbSticks.Right.Y * 0.01f * speed.pitch;
            }

            rot.yaw += yawDelta;
            rot.pitch += pitchDelta;

            rotMtx = Matrix.CreateRotationX(rot.pitch) * Matrix.CreateRotationY(rot.yaw);
            //rotMtx = rot.CalculateMatrix();

            // Update Position
            velocity.X = tickContext.padState.ThumbSticks.Left.X;
            velocity.Y = tickContext.padState.Buttons.RightShoulder - tickContext.padState.Buttons.LeftShoulder;
            velocity.Z = -tickContext.padState.ThumbSticks.Left.Y;
            velocity = Vector3.TransformNormal(velocity, rotMtx);

            // Update Avatar
            avatar.orientation.position += velocity;
            avatar.orientation.rotationQ = Quaternion.CreateFromRotationMatrix(rotMtx);
        }
    }

    public class MovementFPS : Movement
    {
        public Rotation rot;
        public Rotation speed;
        public Vector3 velocityFree;
        public Vector3 velocityWanted;
        public Vector3 forceLegs;
        public Vector3 forceGravity;
        public Boolean isJumping = false;
        public Boolean isOnGround = false;
        public float hitSphereRadius = 10.0f;
        Vector3 newPosition;
        float wantedHeight;

        public MovementFPS(Avatar avatar)
            : base(avatar)
        {
            speed = new Rotation(4.0f, 4.0f, 10.0f);
        }

        override public void Tick( TickContext tickContext )
        {
            UpdatePosition(tickContext);
            CheckHits();
            UpdateLocation(tickContext);
            UpdateRotation(tickContext);

            avatar.orientation.position = newPosition;
        }

        void UpdateRotation( TickContext tickContext )
        {
            float yawDelta;
            float pitchDelta;

            // Update Rotation
            yawDelta = -tickContext.padState.ThumbSticks.Right.X * 0.01f * speed.yaw;

            if (invertY)
            {
                pitchDelta = -tickContext.padState.ThumbSticks.Right.Y * 0.01f * speed.pitch;
            }
            else
            {
                pitchDelta = tickContext.padState.ThumbSticks.Right.Y * 0.01f * speed.pitch;
            }

            rot.yaw += yawDelta;
            rot.pitch = MathHelper.Clamp(rot.pitch+pitchDelta, -RMath.constPI, RMath.constPI);

            rotMtx = Matrix.CreateRotationX(rot.pitch)*Matrix.CreateRotationY(rot.yaw);
            //rotMtx = rot.CalculateMatrix();

            avatar.orientation.rotationQ = Quaternion.CreateFromRotationMatrix(rotMtx);
        }

        void UpdatePosition(TickContext tickContext)
        {
            const float movementSpeed = 100.0f;
            // Update forces
            forceLegs.X = movementSpeed * tickContext.padState.ThumbSticks.Left.X;
            forceLegs.Z = movementSpeed * -tickContext.padState.ThumbSticks.Left.Y;
            forceLegs.Y = 0.0f;

            forceLegs = Vector3.TransformNormal(forceLegs, rotMtx);

            // jump
            if (location == Location.Ground)
            {
                forceGravity = defaultGravity;

                isJumping = tickContext.padState.Buttons.A.Equals(ButtonState.Pressed);

                if (isJumping)
                {
                    forceGravity.Y = 200.0f * movementSpeed;
                }
            }
            else
            {
                forceGravity = defaultGravity;
            }

            // drag
            //velocityFree = 0.3f;

            // integrate
            velocityFree += tickContext.secs * (forceGravity);
            velocityWanted = forceLegs;

            newPosition = avatar.orientation.position + (velocityFree + velocityWanted) * tickContext.secs;
        }

        void CheckHits()
        {
            Vector3  collisionPoint;

            if (Program.GetActiveScene().background.CollideStaticSphere(newPosition, hitSphereRadius, out collisionPoint))
            {
                // push upwards
                Vector3 position = newPosition;
                Vector3 delta = position - collisionPoint;
                float rSqr;

                {
                    delta.Y = 0;
                    rSqr = hitSphereRadius * hitSphereRadius - delta.LengthSquared();
                }

                if (rSqr <= 0.0f)
                {
                    rSqr = 0.0001f;
                }

                wantedHeight = collisionPoint.Y + (float)Math.Sqrt(rSqr);
                newPosition.Y = 0.5f * (newPosition.Y + wantedHeight);

                // landed
                velocityFree = Vector3.Zero;
                location = Location.Ground;

                //Game.engine.text.Print("collision at " + collisionPoint);
            }
            else
            {
                //Game.engine.text.Print("No collision");
                location = Location.Air;
            }
        }

        void UpdateLocation(TickContext context)
        {
            //Game.engine.text.Print("Location : " + location);
        }
    }

}
