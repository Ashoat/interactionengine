using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using Bullshoot;
using Bullshoot.Code;

namespace Terrain
{
    class HeightMap : Map
    {
        Array                   heightMap;

        public HeightMap( GraphicsDeviceManager graphics, ContentManager content, string heightMapTexture ) : base( graphics )
        {
			Color[]					colour;
			Texture2D               texture;
			
			texture = content.Load< Texture2D >( "textures/" + heightMapTexture );
            
			colour = new Color[ texture.Height * texture.Width ];

            texture.GetData< Color >( colour );

            this.meGenerateHeightMap( colour, texture.Width, texture.Height );

            base.SetVertexBuffer( this.heightMap, texture.Width, texture.Height );
			//base.SetVertexBuffer( this.heightMap, 256, 257 );
			this.SetVertexIndices();

			base.SetHitsHeightMap( this.heightMap );
        }

		// position if the place the middle of the deformation map will try and deform
		public void DeformHeightMap( Vector2 position, ContentManager content, string deformationMap )
		{
			Texture2D				texture;
			Color[]					colour;
			Point					deformPosition;

			texture = content.Load< Texture2D >( "textures/" + deformationMap );

			colour = new Color[ texture.Height * texture.Width ];

            texture.GetData< Color >( colour );

			deformPosition = new Point( ( int )( position.X - ( texture.Width / 2 ) ), ( int )( position.Y - ( texture.Height / 2 ) ) );
			
			for ( int xIndex = 0; xIndex < texture.Width; xIndex++ )
			{
				for ( int yIndex = 0; yIndex < texture.Height; yIndex++ )
				{
					float					value;
					int                     arrayIndex = ( ( yIndex * texture.Width ) + xIndex );
					int						actualXIndex = xIndex + deformPosition.X;
					int						actualYIndex = yIndex + deformPosition.Y;

					if ( ( actualXIndex >= 0 ) && ( actualXIndex <= base.GetWidth() ) )
					{
						if ( ( actualYIndex >= 0 ) && ( actualYIndex <= base.GetHeight() ) )
						{
							value = ( float )this.heightMap.GetValue( actualXIndex, actualYIndex );
							value -= meConvertColourToHeight( colour[ arrayIndex ], 6.0f );
							this.heightMap.SetValue( value, actualXIndex, actualYIndex );
						}
					}
				}
			}

			// redo the heighmap
			base.ChangeVertexBuffer( this.heightMap, deformPosition.X, deformPosition.Y, texture.Width, texture.Height, null );
		}

		public float GetHeightMapValue( int x, int y )
		{
			return ( float )this.heightMap.GetValue( x, y );
		}

        //////////////////////////////////////////////////////
        //  Private
        //////////////////////////////////////////////////////

        private void meGenerateHeightMap( Color[] colour, int height, int width )
        {
            this.heightMap = Array.CreateInstance( typeof( float ), width, height );
			
            for ( int yIndex = 0; yIndex < height; yIndex++ )
            {
                for ( int xIndex = 0; xIndex < width; xIndex++ )
                {
                    float                   value;
                    int                     arrayIndex = ( ( yIndex * width ) + xIndex );

					value = this.meConvertColourToHeight( colour[ arrayIndex ], 50.0f );
					this.heightMap.SetValue( value, xIndex, yIndex );
                }
            }
        }

        private float meConvertColourToHeight( Color colour, float scale )
        {
            float                   height;

            // cheat for the moment and assume all the RGB are the same.
			//unsafe 
			//{
				//byte *				colourChannel = ( byte * )&colour;

				height = ( float )colour.R / ( float )255;
			//}
            return ( height * scale );
        }
    }
}
