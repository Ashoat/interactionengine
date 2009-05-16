using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Bullshoot.Code
{
    static public class RMath
    {
        public static float ModulusF(float value, float modulus, float invModulus)
        {
            value -= (float)((int)(value * invModulus)) * modulus;

            return value;
        }

        public static int Clamp(int value, int min, int max)
        {
            return (value < min) ? min : ((value > max) ? max : value);
        }

        static public float epsilon = 0.0000001f;
        static public float constPI = MathHelper.Pi;
        static public float const2PI = constPI * 2.0f;
        static public float const1DivPI = 1.0f / constPI;
        static public float const1Div2PI = 1.0f / (2.0f * constPI);
        static public float constPIDiv2 = constPI / 2.0f;
        static public float constPIDiv3 = constPI / 3.0f;
        static public float constPIDiv4 = constPI / 4.0f;

        public class Random : System.Random
        {
            public float GetRandomFloatRange(float min, float max)
            {
                double rnd = base.NextDouble();
                double rndRange = (max - min) * rnd;
                rndRange += min;

                return (float)rndRange;
            }

            public int GetRandomIntRange(int min, int max)
            {
                double rnd = base.NextDouble();
                double rndRange = (max - min) * rnd;
                rndRange += min;

                return (int)rndRange;
            }
        }

        // oriented square
        public class Square
        {
            //Vector3 normal;
			//Vector3 centre;
            //Vector2 extents;
        }
    }
}
