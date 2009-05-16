using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Bullshoot.Code
{
    public struct Rotation
    {
        public float pitch;
        public float yaw;
        public float roll;

        public Rotation(float pitch, float yaw, float roll)
        {
            this.pitch = pitch;
            this.yaw = yaw;
            this.roll = roll;
        }

        public static Rotation ModulusPI(Rotation rot)
        {
            rot.pitch = RMath.ModulusF(rot.pitch + RMath.constPI, RMath.const2PI, RMath.const1Div2PI) - RMath.constPI;
            rot.yaw = RMath.ModulusF(rot.yaw + RMath.constPI, RMath.const2PI, RMath.const1Div2PI) - RMath.constPI;
            rot.roll = RMath.ModulusF(rot.roll + RMath.constPI, RMath.const2PI, RMath.const1Div2PI) - RMath.constPI;

            return rot;
        }

        public Vector3 CalculateDirectionVector()
        {
            Vector3 direction = new Vector3();
            float   cosYaw = (float)Math.Cos(yaw);
            float   sinYaw = (float)Math.Sin(yaw);
            float   cosPitch = (float)Math.Cos(pitch);
            float   sinPitch = (float)Math.Sin(pitch);

            direction.X = cosYaw * cosPitch;
            direction.Z = sinYaw * cosPitch;
            direction.Y = sinPitch;

            return direction;
        }

        public Matrix CalculateMatrix()
        {
            float cy = (float)Math.Cos(yaw);
            float sy = (float)Math.Sin(yaw);
            float cx = (float)Math.Cos(pitch);
            float sx = (float)Math.Sin(pitch);
            float cz = (float)Math.Cos(roll);
            float sz = (float)Math.Sin(roll);
            float sxsy = sx * sy;
            float cycz = cy * cz;
            Matrix m = new Matrix();

            m.M11 = cycz + sxsy * sz;
            m.M12 = cx * sz;
            m.M13 = cx * sx;
            m.M14 = 0.0f;

            m.M11 = cycz + sxsy * sz;
            m.M12 = cx * sz;
            m.M13 = cy * sx * sz - sy * cz;
            m.M14 = 0.0f;

            m.M21 = sxsy * cz - cy * sz;
            m.M22 = cx * cz;
            m.M23 = sy * sz + cycz * sx;
            m.M24 = 0.0f;

            m.M31 = sy * cx;
            m.M32 = -sx;
            m.M33 = cy * cx;
            m.M34 = 0.0f;

            m.M41 = 0.0f;
            m.M42 = 0.0f;
            m.M43 = 0.0f;
            m.M44 = 1.0f;

            return m;
        }
    }
}
