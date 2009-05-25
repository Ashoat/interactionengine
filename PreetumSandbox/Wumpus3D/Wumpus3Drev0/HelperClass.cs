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
using System.ComponentModel;
using System.IO;

namespace Wumpus3Drev0
{
    class HelperClass
    {
        /// <summary>
        /// incomplete
        /// </summary>
        /// <param name="source"></param>
        /// <param name="cloned"></param>
        public static void CloneModelEffect(ModelEffect source, ref ModelEffect cloned)
        {
            cloned.ActiveCamera = source.ActiveCamera;

            cloned.Alpha = source.Alpha;
            cloned.AmbientLightColor = source.AmbientLightColor;
            cloned.CurrentTechnique = source.CurrentTechnique;
            cloned.DiffuseColor = source.DiffuseColor;

        }

        public static Vector2 RotateVector(Vector2 source, float angle)
        {
            float finalAngle = (float)Math.Atan2(source.X, source.Y) + angle;
            float d = source.Length();
            return new Vector2(d * (float)Math.Sin(finalAngle), d * (float)Math.Cos(finalAngle));
        }

        public static float GetAngle(Vector2 source) //angle as heading
        {
            //source = Vector2.Reflect(source, Vector2.UnitY);
            return (float)Math.Atan2(source.X, source.Y);
        }

    }
}
