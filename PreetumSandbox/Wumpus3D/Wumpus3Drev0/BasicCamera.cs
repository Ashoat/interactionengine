using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace Wumpus3Drev0
{
    class BasicCamera
    {
        float fovy = (float)Math.PI / 4f; //45 degrees
        float aspectRatio;
        float nearPlane;
        float farPlane;

        Matrix projectionMatrix;
        Matrix viewMatrix;

        Vector3 position, target;
        Vector3 heading, strafe, up;

        BoundingFrustum frustum;

        public Vector3 Heading
        {
            get
            {
                return heading;
            }
            /*set
            {
                heading = value;
                heading.Normalize();
                updateCamera();
            }*/
        }

        public Matrix Projection
        {   
            get
            {
                return projectionMatrix;
            }
            set
            {
                projectionMatrix = value;
                updateCamera();
            }
        }
        public Matrix View
        {
            get
            {
                return viewMatrix;
            }
            set
            {
                viewMatrix = value;
                updateCamera();
            }
        }
        public BoundingFrustum Frustrum
        {
            get
            {
                updateCamera();
                return frustum;
            }
        }

        public Vector3 Position
        {
            get { return position; }
        }
        public Vector3 Strafe
        {
            get { return Vector3.Cross(position-target, up);}
        }

        public void SetPerspectiveFov(float fovy, float aspectRatio, float nearPlane, float farPlane)
        {
            this.fovy = fovy;
            this.aspectRatio = aspectRatio;
            this.farPlane = farPlane;
            this.nearPlane = nearPlane;
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(fovy), aspectRatio, nearPlane, farPlane);
            updateCamera();
        }
        public void SetLookAt(Vector3 cameraPos, Vector3 cameraTarget, Vector3 cameraUp)
        {
            this.position = cameraPos;
            this.target = cameraTarget;
            this.heading = this.target - this.position;
            heading.Normalize();
            this.up = cameraUp;
            this.strafe = Vector3.Cross(heading, up);
            updateCamera();
        }
        /// <summary>
        /// Moves the camera + displaces the camera target accordingly
        /// </summary>
        /// <param name="cameraPos"></param>
        public void SetPosition(Vector3 cameraPos)
        {
            target += cameraPos - position;
            position = cameraPos;
            updateCamera();
        }
        /// <summary>
        /// Moves the camera, without displacing the target.
        /// </summary>
        /// <param name="cameraPos"></param>
        public void SetPositionLockTarget(Vector3 cameraPos)
        {
            position = cameraPos;
            updateCamera();
        }

        public void SetTargetDisplacePosition(Vector3 tar)
        {
            position += tar - target;
            target = tar;
            updateCamera();
        }
        public void SetTargetLockPosition(Vector3 tar)
        {
            target = tar;
            updateCamera();
        }
        /// <summary>
        /// Rotates camera (orbit) around position (axis: y)
        /// </summary>
        /// <param name="posRot"></param>
        /// <param name="rot"></param>
        public void RotateUponPosition(Vector3 posRot, float rot)
        {
            position = Vector3.Transform((posRot - position), Matrix.CreateRotationY(MathHelper.ToRadians(rot))) + position;
            updateCamera();
        }

        public void Zoom(float dist)
        {
            SetPositionLockTarget(this.Position + dist * this.Heading);
        }

        /// <summary>
        /// DIE!
        /// </summary>
        /// <param name="posRot"></param>
        /// <param name="axis"></param>
        /// <param name="rot"></param>
        public void RotateUponAxis(Vector3 posRot, Vector3 axis, float rot)
        {
            Vector3 relPosition = position - posRot;
            position = Vector3.Transform(relPosition, Matrix.CreateFromAxisAngle(axis, MathHelper.ToRadians(rot))) + posRot;
            updateCamera();
        }
        public void ChangeAzimuth(Vector3 posRot, Vector3 axis, float amount)
        {
            Vector3 relPosition = position - posRot;
            Vector3 azimuthAxis = Vector3.Cross(relPosition, axis);
            azimuthAxis.Normalize();
            position = Vector3.Transform(relPosition, Matrix.CreateFromAxisAngle(azimuthAxis, MathHelper.ToRadians(amount))) + posRot;
            updateCamera();
        }
        private void updateCamera()
        {
            
            viewMatrix = Matrix.CreateLookAt(position, target, up);
            frustum = new BoundingFrustum(viewMatrix * projectionMatrix);
            heading = Vector3.Normalize(target - position);
        }

        public BasicCamera()
        {
        }
    }
}
