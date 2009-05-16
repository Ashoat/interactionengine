using System;
using System.Collections.Generic;
using System.Text;
using Terrain;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Bullshoot.Code
{
    class Background
    {
        public HeightMap	heightMap;

        public Background()
        {
			ContentManager			content;
			GraphicsDeviceManager	graphics;

			Program.GetContentManager( out content );
			Program.GetGraphicsManager( out graphics );

            heightMap = new HeightMap( graphics, content, "heightMap" );
        }

        public void Tick(TickContext tickContext)
        {
        }

        public void Draw( DrawContext drawContext )
        {
            heightMap.Draw(drawContext);
        }

		public void AddExplodeHole( Vector3 position )
		{
			ContentManager			content;
			GraphicsDeviceManager	graphics;
			Vector2					mapPosition;
			int						xIndex;
			int						yIndex;

			Program.GetContentManager( out content );
			Program.GetGraphicsManager( out graphics );

			this.heightMap.GetIndexForPosition( position, out xIndex, out yIndex );
			mapPosition = new Vector2( xIndex, yIndex );
			this.heightMap.DeformHeightMap( mapPosition, content, "crater" );
		}

		public Boolean CollideSweepingSphereBackground( Vector3 spherePosition, float sphereRadius, Vector3 velocity, out Vector3 collisionPoint )
		{
			Boolean					result = false;

			if ( this.heightMap != null )
			{
				result = this.heightMap.CollideSweepingSphere( spherePosition, sphereRadius, velocity, out collisionPoint );
			}
			else
			{
				collisionPoint = Vector3.Zero;
			}

			return result;
		}

        public Boolean CollideStaticSphere(Vector3 spherePosition, float sphereRadius, out Vector3 collisionPoint)
        {
            float sphereRadiusSqr = sphereRadius * sphereRadius;

            int xStart, yStart, xEnd, yEnd;
            int x, y;

            heightMap.GetIndexForRange(spherePosition, out xStart, out xEnd, out yStart, out yEnd, (int)sphereRadius);

            for (x = xStart; x <= xEnd - 1; ++x)
            {
                for (y = yStart; y <= yEnd - 1; ++y)
                {
                    float vertHeight = (float)heightMap.GetHeightMapValue(x, y);

                    Vector3 vertPos = new Vector3(x, vertHeight, y);
                    Vector3 distance = -vertPos + spherePosition;

                    if ( distance.LengthSquared() < sphereRadiusSqr )
                    {
                        collisionPoint = vertPos;
                        return true;
                    }
                }
            }

            collisionPoint = Vector3.Zero;

            return false;
        }
    }
}
