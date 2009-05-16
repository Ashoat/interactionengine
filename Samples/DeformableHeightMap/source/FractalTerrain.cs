using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Bullshoot.Code;
using RandomNumber;

namespace Terrain 
{
    class FractalTerrain3D : Map
    {
        Array                           heightMap;
        static int                      xLength = 129;
        static int                      yLength = 129;

        RandomNumGen                    rndGen;

        public FractalTerrain3D( GraphicsDeviceManager graphics, float initialRandomNumberRange, float roughnessConstant ) : base( graphics )
        {
            this.heightMap = Array.CreateInstance( typeof( float ), FractalTerrain3D.xLength, FractalTerrain3D.yLength );
            
            this.rndGen = new RandomNumGen();

            // artificially set the height of the centre point
            //this.heightMap.SetValue( 3.0f, FractalTerrain3D.xLength / 3, FractalTerrain3D.yLength / 4 );

            this.meGenerateFractalHeightMap( initialRandomNumberRange, roughnessConstant );

            base.SetVertexBuffer( this.heightMap, FractalTerrain3D.xLength, FractalTerrain3D.yLength );
        }

        //////////////////////////////////////////////////////
        //  Private
        //////////////////////////////////////////////////////

        private void meGenerateFractalHeightMap( float initialRandomNumberRange, float roughnessConstant ) 
        {
            Point                   minPoint;
            Point                   maxPoint;

            minPoint = new Point( 0, 0 );
            maxPoint = new Point( FractalTerrain3D.xLength - 1, FractalTerrain3D.yLength - 1 );

            meOffsetCentrePoint( minPoint, maxPoint, initialRandomNumberRange, roughnessConstant );  
        }

        private void meOffsetCentrePoint( Point minPoint, Point maxPoint, float initialRandomNumberRange, float roughnessConstant )
        {
            Point                   bottomLeft = new Point( minPoint.X, maxPoint.Y );
            Point                   topRight = new Point( maxPoint.X, minPoint.Y );
            Point                   midLeft;
            Point                   midTop;
            Point                   midRight;
            Point                   midBottom;
            Point                   centrePoint;
            float                   average;
            
            centrePoint = new Point( ( maxPoint.X - minPoint.X ) / 2 , ( maxPoint.Y - minPoint.Y ) / 2 );
            centrePoint.X += minPoint.X;
            centrePoint.Y += minPoint.Y;

            // check that the centre point is different from the other 2 points
            if ( !centrePoint.Equals( minPoint ) && !centrePoint.Equals( maxPoint ) )
            {
                // take average height of each corner and change by a random offset
                average = ( float )this.heightMap.GetValue( minPoint.X, minPoint.Y )
                        + ( float )this.heightMap.GetValue( maxPoint.X, maxPoint.Y )
                        + ( float )this.heightMap.GetValue( bottomLeft.X, bottomLeft.Y )
                        + ( float )this.heightMap.GetValue( topRight.X, topRight.Y );
                average /= 4.0f;

                average += rndGen.GetRandomFloatRange( 0.0f, initialRandomNumberRange );

                // add the average to the previous value and store it.
                average += ( float )this.heightMap.GetValue( centrePoint.X, centrePoint.Y );

                this.heightMap.SetValue( average, centrePoint.X, centrePoint.Y );

                midLeft     = new Point( minPoint.X, centrePoint.Y );
                midTop      = new Point( centrePoint.X, minPoint.Y );
                midRight    = new Point( maxPoint.X, centrePoint.Y );
                midBottom   = new Point( centrePoint.X, maxPoint.Y );

                // times the random number range by H.2^(-H) ( H = roughness constant )

                initialRandomNumberRange = initialRandomNumberRange * ( roughnessConstant * ( float )Math.Pow( 2.0, -roughnessConstant ) );
                // recurse for each square...
                // top left square
                meOffsetCentrePoint( minPoint, centrePoint, initialRandomNumberRange, roughnessConstant );
                // top right square
                meOffsetCentrePoint( midTop, midRight, initialRandomNumberRange, roughnessConstant );
                // bottom right square
                meOffsetCentrePoint( centrePoint, maxPoint, initialRandomNumberRange, roughnessConstant );
                // bottom left square
                meOffsetCentrePoint( midLeft, midBottom, initialRandomNumberRange, roughnessConstant );

                // need to do a diamond step...
                // this calculates a value for each midPoint
                // midleft point
                average = ( float )this.heightMap.GetValue( minPoint.X, minPoint.Y )
                        + ( float )this.heightMap.GetValue( bottomLeft.X, bottomLeft.Y )
                        + ( float )this.heightMap.GetValue( centrePoint.X, centrePoint.Y );
                average /= 3.0f;
                this.heightMap.SetValue( average, midLeft.X, midLeft.Y );
                // midTop point
                average = ( float )this.heightMap.GetValue( minPoint.X, minPoint.Y )
                        + ( float )this.heightMap.GetValue( topRight.X, topRight.Y )
                        + ( float )this.heightMap.GetValue( centrePoint.X, centrePoint.Y );
                average /= 3.0f;
                this.heightMap.SetValue( average, midTop.X, midTop.Y );
                // midRight point
                average = ( float )this.heightMap.GetValue( topRight.X, topRight.Y )
                        + ( float )this.heightMap.GetValue( maxPoint.X, maxPoint.Y )
                        + ( float )this.heightMap.GetValue( centrePoint.X, centrePoint.Y );
                average /= 3.0f;
                this.heightMap.SetValue( average, midRight.X, midRight.Y );
                // mideBottom point
                average = ( float )this.heightMap.GetValue( bottomLeft.X, bottomLeft.Y )
                        + ( float )this.heightMap.GetValue( maxPoint.X, maxPoint.Y )
                        + ( float )this.heightMap.GetValue( centrePoint.X, centrePoint.Y );
                average /= 3.0f;
                this.heightMap.SetValue( average, midBottom.X, midBottom.Y );
            }
        }
    }
}
