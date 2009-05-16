using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Bullshoot.Code
{
    public class Camera
    {
        public Vector3 position = Vector3.Zero;
        //public Quaternion rotationQ = Quaternion.Identity;
        public Matrix viewMtx = Matrix.Identity;
        public Matrix projMtx = Matrix.Identity;
        public Matrix viewProj = Matrix.Identity;
        public Matrix rotMtx = Matrix.Identity;

        public Camera()
        {
            float aspectRatio = (float)GraphicsDeviceManager.DefaultBackBufferWidth / (float)GraphicsDeviceManager.DefaultBackBufferHeight;
            SetProjection(aspectRatio, 90.0f);
        }

        public void Update()
        {
            // View Matrix
            viewMtx = Matrix.CreateTranslation(-position) * rotMtx;

            viewProj = viewMtx * projMtx;

            //Vector3 direction = Vector3.TransformNormal(Vector3.UnitZ, Matrix.CreateFromQuaternion(rotationQ));
            //viewMtx = Matrix.CreateLookAt(position, position + direction, Vector3.UnitY);

            //viewProj = viewMtx * projMtx;
        }

        public void SetProjection(float aspectRatio, float fieldOfView)
        {
            float nearPlane = 0.1f;
            float farPlane = 100000.0f;
            projMtx = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(fieldOfView/aspectRatio), aspectRatio, nearPlane, farPlane);
        }

        public void Tick( TickContext tickContext, Vector3 position, Matrix rotMtx )
        {
            float aspectRatio = (float)GraphicsDeviceManager.DefaultBackBufferWidth / (float)GraphicsDeviceManager.DefaultBackBufferHeight;
            SetProjection(aspectRatio, 90.0f);
            this.position = position;
            this.rotMtx = Matrix.Transpose(rotMtx);
        }

		public virtual void Draw( DrawContext context )
		{

		}
    }
}
