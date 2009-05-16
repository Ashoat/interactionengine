using System;
using System.Collections.Generic;
using System.Text;

namespace RandomNumber
{
    class RandomNumGen : Random
    {
        public float GetRandomFloatRange( float min, float max )
        {
            double rnd = base.NextDouble();
            double rndRange = ( max - min ) * rnd;
            rndRange += min;
            
            return ( float )rndRange;
        }

        public int GetRandomIntRange( int min, int max )
        {
            double rnd = base.NextDouble();
            double rndRange = ( max - min ) * rnd;
            rndRange += min;
            
            return ( int )rndRange;
        }
    }
}
