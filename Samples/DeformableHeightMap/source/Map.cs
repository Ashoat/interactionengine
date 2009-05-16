using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Bullshoot.Code;

namespace Terrain
{
    class Map
    {
        VertexPositionNormalTexture[]   terrainBuffer;
        VertexBuffer                    terrainVertexBuffer;             
        VertexDeclaration               basicEffectVertexDeclaration;
		IndexBuffer						triangleListIndexBuffer;

        Matrix                          worldMatrix;
        Matrix                          viewMatrix;
        Matrix                          projectionMatrix;
        BasicEffect                     basicEffect;

        GraphicsDeviceManager           graphics;

        int                             width;
        int                             height;

		PrimitiveType					primType;

		Array							heightMap;

		public Orientation				orientation = new Orientation();

        public Map( GraphicsDeviceManager graphics )
        {
            this.graphics = graphics;

			this.heightMap = null;

            // Generate the effects class
            worldMatrix = Matrix.CreateScale( 1.0f );

            viewMatrix = Matrix.CreateLookAt( new Vector3( 0, 5.0f, 5.0f ), Vector3.Zero, Vector3.Up );
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView( 
                                MathHelper.ToRadians( 45 ), 
                                ( float )graphics.GraphicsDevice.Viewport.Width / ( float )graphics.GraphicsDevice.Viewport.Height, 
                                1.0f, 100.0f
                            );
            basicEffect = new BasicEffect(graphics.GraphicsDevice, null);
            basicEffect.Alpha = 1.0f;
            basicEffect.DiffuseColor = new Vector3(1.0f, 0.0f, 1.0f);
            basicEffect.SpecularColor = new Vector3(0.25f, 0.25f, 0.25f);
            basicEffect.SpecularPower = 5.0f;
            basicEffect.AmbientLightColor = new Vector3(0.75f, 0.75f, 0.75f);

            basicEffect.DirectionalLight0.Enabled = true;
            basicEffect.DirectionalLight0.DiffuseColor = Vector3.One;
            basicEffect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(1.0f, -1.0f, -1.0f));
            basicEffect.DirectionalLight0.SpecularColor = Vector3.One;

            basicEffect.DirectionalLight1.Enabled = true;
            basicEffect.DirectionalLight1.DiffuseColor = new Vector3(0.5f, 0.5f, 0.5f);
            basicEffect.DirectionalLight1.Direction = Vector3.Normalize(new Vector3(-1.0f, -1.0f, 1.0f));
            basicEffect.DirectionalLight1.SpecularColor = new Vector3(0.5f, 0.5f, 0.5f);

            basicEffect.LightingEnabled = true;

            basicEffect.World = worldMatrix;
            basicEffect.View = viewMatrix;
            basicEffect.Projection = projectionMatrix;

			primType = PrimitiveType.TriangleList;
        }

		public void SetHitsHeightMap( Array heightMap )
		{
			this.heightMap = heightMap;
		}

		public Boolean CollideSweepingSphere( Vector3 spherePosition, float sphereRadius, Vector3 velocity, out Vector3 collisionPoint )
		{
			int					interval = 10;
			Vector3				intervalDisplacement = velocity / interval;
			int					xStart;
			int					xEnd;
			int					yStart;
			int					yEnd;

			this.GetIndexForRange(spherePosition, out xStart, out xEnd, out yStart, out yEnd, (int)sphereRadius);

			for ( int intervalIndex = 0; intervalIndex < interval; intervalIndex++ )
			{
				BoundingSphere		hitSphere = new BoundingSphere( spherePosition + ( intervalDisplacement * intervalIndex ), sphereRadius );

				for ( int xIndex = xStart; xIndex < xEnd - 1; xIndex++ )
				{
					for ( int yIndex = yStart; yIndex < yEnd - 1; yIndex++ )
					{
						Plane					hitPlane;
						Vector3					point1;
						Vector3					point2;
						Vector3					point3;

						// check top triangle

						point1 = new Vector3( xIndex, ( float )this.heightMap.GetValue( xIndex, yIndex ), yIndex );
						point2 = new Vector3( xIndex + 1, ( float )this.heightMap.GetValue( xIndex + 1, yIndex ), yIndex );
						point3 = new Vector3( xIndex, ( float )this.heightMap.GetValue( xIndex, yIndex + 1 ), yIndex + 1 );

						hitPlane = new Plane( point1, point2, point3 );

						if ( hitPlane.Intersects( hitSphere ) == PlaneIntersectionType.Intersecting )
						{
							collisionPoint = new Vector3( xIndex, ( float )this.heightMap.GetValue( xIndex, yIndex ), yIndex );
							return true;
						}
						// check bottom triangle

						point1 = new Vector3( xIndex + 1, ( float )this.heightMap.GetValue( xIndex + 1, yIndex ), yIndex );
						point2 = new Vector3( xIndex + 1, ( float )this.heightMap.GetValue( xIndex + 1, yIndex + 1 ), yIndex + 1 );
						point3 = new Vector3( xIndex, ( float )this.heightMap.GetValue( xIndex, yIndex + 1 ), yIndex + 1 );

						hitPlane = new Plane( point1, point2, point3 );

						if ( hitPlane.Intersects( hitSphere ) == PlaneIntersectionType.Intersecting )
						{
							collisionPoint = new Vector3( xIndex + 1, ( float )this.heightMap.GetValue( xIndex + 1, yIndex + 1 ), yIndex + 1 );
							return true;
						}
					}
				}
			}

			collisionPoint = Vector3.Zero;

			return false;
		}

		public Boolean GetIndexForPosition(Vector3 position, out int xIndex, out int yIndex)
        {
            Vector3 modelPos = Vector3.Transform( position, orientation.InvBasisMtx );

            int x = ( int )modelPos.X;
            int y = ( int )modelPos.Z;

            xIndex = RMath.Clamp( x, 0, this.GetWidth() );
            yIndex = RMath.Clamp( y, 0, this.GetHeight() );

            return ( x == xIndex && y == yIndex );
        }


        public void GetIndexForRange(Vector3 position, out int xStart, out int xEnd, out int yStart, out int yEnd, int sphereRadius)
        {
            Vector3 modelPos = Vector3.Transform( position, orientation.InvBasisMtx );

            int x = ( int )modelPos.X;
            int y = ( int )modelPos.Z;

            xStart = RMath.Clamp( x - ( ( int )sphereRadius + 1 ), 0, this.GetWidth() );
            yStart = RMath.Clamp( y - ( ( int )sphereRadius + 1 ), 0, this.GetHeight() );
            xEnd = RMath.Clamp( x + ( ( int )sphereRadius + 1 ), 0, this.GetWidth() );
            yEnd = RMath.Clamp( y+( ( int )sphereRadius + 1 ), 0, this.GetHeight() );
        }

		public void SetTexture( Texture2D texture )
		{
			this.basicEffect.Texture = texture;
		}

        public virtual void Draw( DrawContext context )
        {
			switch ( this.primType )
			{
				case PrimitiveType.TriangleList:
				{
					this.meRenderTriangleList( context );
				}
				break;

				case PrimitiveType.TriangleStrip:
				{
					this.meRenderTriangleStrips( context );
				}
				break;
			}
        }

		// height map is used for culling, i.e if is an overlapping surface and if it is above
		//  this surface then don't update this position
		public void ChangeVertexBuffer( Array map, int offsetX, int offsetY, int width, int height, HeightMap heightMap )
		{
			for ( int xIndex = 0; xIndex < width; xIndex++ )
			{
				for ( int yIndex = 0; yIndex < height; yIndex++ )
				{
					int					actualYIndex		= yIndex + offsetY;
					int					actualXIndex		= xIndex + offsetX;
					int					actualVertexIndex	= ( actualYIndex * this.width ) + actualXIndex;
					Vector3				position;
					Vector3				normal;

					if ( ( heightMap == null ) || ( heightMap.GetHeightMapValue( xIndex, yIndex ) <= ( float )map.GetValue( xIndex, yIndex ) ) )
					{
						if ( ( actualXIndex >= 0 ) && ( actualXIndex <= this.GetWidth() ) )
						{
							if ( ( actualYIndex >= 0 ) && ( actualYIndex <= this.GetHeight() ) )
							{
								position = new Vector3( ( float )actualXIndex, ( float )map.GetValue( actualXIndex, actualYIndex ), ( float )actualYIndex );
								this.meGetNormalForVertex( ref map, actualXIndex, actualYIndex, out normal );

								//terrainBuffer[ actualVertexIndex ].Normal = normal;
								terrainBuffer[ actualVertexIndex ].Position = position;
								terrainBuffer[ actualVertexIndex ].TextureCoordinate.X = actualXIndex;
								terrainBuffer[ actualVertexIndex ].TextureCoordinate.Y = actualYIndex;
								//terrainBuffer[ actualVertexIndex ] = new VertexPositionNormalTexture( position, normal, new Vector2( actualXIndex, actualYIndex ) );
							}
						}

					}
				}
			}

		    this.terrainVertexBuffer.SetData< VertexPositionNormalTexture >( this.terrainBuffer );
		}

		public void SetVertexBuffer( Array map, int width, int height )
		{
			int vertexIndex = 0;

		    this.width = width;
			this.height = height;

			terrainBuffer = new VertexPositionNormalTexture[ this.width * this.height ];
            
			for ( int yIndex = 0; yIndex < this.height; yIndex++ )
			{
				for ( int xIndex = 0; xIndex < this.width; xIndex++ )
				{
					Vector3						position;
					Vector3						normal;

					position = new Vector3( ( float )xIndex, ( float )map.GetValue( xIndex, yIndex ), ( float )yIndex );
					this.meGetNormalForVertex( ref map, xIndex, yIndex, out normal );

					terrainBuffer[ vertexIndex ] = new VertexPositionNormalTexture( position, normal, new Vector2( xIndex, yIndex ) );
		            vertexIndex++;
				}
			}

			terrainVertexBuffer = new VertexBuffer( 
		            this.graphics.GraphicsDevice, 
		            VertexPositionNormalTexture.SizeInBytes * terrainBuffer.Length,
		            ResourceUsage.None,
		            ResourceManagementMode.Automatic
		        );

		    this.terrainVertexBuffer.SetData< VertexPositionNormalTexture >( this.terrainBuffer );

		    this.basicEffectVertexDeclaration = new VertexDeclaration(
		            this.graphics.GraphicsDevice, VertexPositionNormalTexture.VertexElements );

		}
		
		// must be called after SetVertexBuffer
		public void SetVertexIndices()
		{
			switch ( this.primType )
			{
				case PrimitiveType.TriangleList:
				{
					this.meSetVertexIndicesForTriangleList();
				}
				break;

				case PrimitiveType.TriangleStrip:
				{
					this.meSetVertexIndicesForTriangleStrips();
				}
				break;
			}
		}

		public int GetWidth()
		{
			return this.width;
		}

		public int GetHeight()
		{
			return this.height;
		}

		private void meSetVertexIndicesForTriangleList()
		{
			int					triangleCount		= ( this.width - 1 ) * ( this.height - 1 ) * 2 * 3;
			int[]				triangleListIndices = new int[ triangleCount ];

			int					triangleIndex		= 0;

			for ( int xIndex = 0; xIndex < this.width - 1; xIndex++ )
			{
				for ( int yIndex = 0; yIndex < this.height - 1; yIndex++ )
				{
					int								triangleMinIndex = ( int )( ( this.width * yIndex ) + xIndex );

					// top triangle
					triangleListIndices[ triangleIndex * 3 ]			= triangleMinIndex;
					triangleListIndices[ ( triangleIndex * 3 ) + 1 ]	= ( int )( triangleMinIndex + 1 );
					triangleListIndices[ ( triangleIndex * 3 ) + 2 ]	= ( int )( triangleMinIndex + this.width + 1 );
					triangleIndex++;

					// bottom triangle
					triangleListIndices[ triangleIndex * 3 ]			= triangleMinIndex;
					triangleListIndices[ ( triangleIndex * 3 ) + 1 ]	= ( int )( triangleMinIndex + this.width + 1 );
					triangleListIndices[ ( triangleIndex * 3 ) + 2 ]	= ( int )( triangleMinIndex + this.width );
					triangleIndex++;
				}
			}

			triangleListIndexBuffer = new IndexBuffer(
					this.graphics.GraphicsDevice,
					sizeof( int ) * triangleListIndices.Length,
					ResourceUsage.None,
					ResourceManagementMode.Automatic,
					IndexElementSize.ThirtyTwoBits
				);

			// Set the data in the index buffer to our array
			triangleListIndexBuffer.SetData< int >(
					triangleListIndices
				);
		}

		private void meSetVertexIndicesForTriangleStrips()
		{
			int					stripLength		= 4 + ( this.height - 2 ) * 2;
			int					stripCount		= this.width - 1;

			short[]				triangleListIndices = new short[ stripLength * stripCount ];
			int					indicesIndex	= 0;
	
			for ( int stripIndex = 0; stripIndex < stripCount; stripIndex++ )
			{
				for ( int yIndex = 0; yIndex < this.height; yIndex++ )
				{
					triangleListIndices[ indicesIndex ] = ( short )( stripIndex + ( this.width * yIndex ) );
					indicesIndex++;

					triangleListIndices[ indicesIndex ] = ( short )( stripIndex + ( this.width * yIndex ) + 1);
					indicesIndex++;
				}
			}

			triangleListIndexBuffer = new IndexBuffer(
					this.graphics.GraphicsDevice,
					sizeof(short) * triangleListIndices.Length,
					ResourceUsage.None,
					ResourceManagementMode.Automatic,
					IndexElementSize.SixteenBits
				);

			// Set the data in the index buffer to our array
			triangleListIndexBuffer.SetData< short >(
					triangleListIndices
				);
		}

        private void meGetNormalForVertex( ref Array map, int xIndex, int yIndex, out Vector3 normal )
        {
            Vector3                 centre;
            Vector3                 point1;
            Vector3                 point2;
            Vector3                 averageNormal = Vector3.Zero;
            int                     averageCount = 0;
            bool                    spaceAbove  = false;
            bool                    spaceBelow  = false;
            bool                    spaceLeft   = false;
            bool                    spaceRight  = false;
            Vector3                 normalTemp;
            Vector3                 vector1;
            Vector3                 vector2;

            centre = new Vector3( ( float )xIndex, ( float )map.GetValue( xIndex, yIndex ), ( float )yIndex );

            if ( xIndex > 0 )
            {
                // can use left side of the centre point
                spaceLeft = true;
            }
            if ( xIndex < ( this.width - 1 ) )
            {
                // can use right side of the centre point
                spaceRight = true;
            }
            if ( yIndex > 0 )
            {
                // can use top side of the centre point
                spaceAbove = true;
            }
            if ( yIndex < ( this.height - 1 ) )
            {
                // can use bottom side of the centre point
                spaceBelow = true;
            }

            if ( spaceAbove && spaceLeft )
            {
                point1 = new Vector3( xIndex - 1, ( float )map.GetValue( xIndex - 1, yIndex ), yIndex );
                point2 = new Vector3( xIndex - 1, ( float )map.GetValue( xIndex - 1, yIndex - 1 ), yIndex - 1 );

                vector1 = point1 - centre;
                vector2 = point2 - point1;

                normalTemp = Vector3.Cross( vector1, vector2 );
                averageNormal += normalTemp;

                averageCount++;
            }

            if ( spaceAbove && spaceRight )
            {
                point1 = new Vector3( xIndex, ( float )map.GetValue( xIndex, yIndex - 1 ), yIndex - 1 );
                point2 = new Vector3( xIndex + 1, ( float )map.GetValue( xIndex + 1, yIndex - 1 ), yIndex - 1 );

                vector1 = point1 - centre;
                vector2 = point2 - point1;

                normalTemp = Vector3.Cross( vector1, vector2 );
                averageNormal += normalTemp;

                averageCount++;
            }

            if ( spaceBelow && spaceRight )
            {
                point1 = new Vector3( xIndex + 1, ( float )map.GetValue( xIndex + 1, yIndex ), yIndex );
                point2 = new Vector3( xIndex + 1, ( float )map.GetValue( xIndex + 1, yIndex + 1 ), yIndex + 1 );

                vector1 = point1 - centre;
                vector2 = point2 - point1;

                normalTemp = Vector3.Cross( vector1, vector2 );
                averageNormal += normalTemp;

                averageCount++;
            }

            if ( spaceBelow && spaceLeft )
            {
                point1 = new Vector3( xIndex, ( float )map.GetValue( xIndex, yIndex + 1 ), yIndex + 1 );
                point2 = new Vector3( xIndex - 1, ( float )map.GetValue( xIndex - 1, yIndex + 1 ), yIndex + 1 );

                vector1 = point1 - centre;
                vector2 = point2 - point1;

                normalTemp = Vector3.Cross( vector1, vector2 );
                averageNormal += normalTemp;

                averageCount++;
            }

            normal = averageNormal / averageCount;
        }

		private void meRenderTriangleList( DrawContext context )
		{
			int                         numberOfPrimitives;
            
            numberOfPrimitives = ( this.width - 1 ) * ( this.height - 1 ) * 2;

            this.graphics.GraphicsDevice.VertexDeclaration = this.basicEffectVertexDeclaration;
            
            this.basicEffect.View = context.camera.viewMtx;
            this.basicEffect.Projection = context.camera.projMtx;
            //this.basicEffect.World = worldMtx;

            this.basicEffect.Begin();
            this.basicEffect.EnableDefaultLighting();

            foreach ( EffectPass pass in this.basicEffect.CurrentTechnique.Passes )
            {
                pass.Begin();
               
				this.graphics.GraphicsDevice.Vertices[0].SetSource( terrainVertexBuffer, 0, VertexPositionNormalTexture.SizeInBytes );
				this.graphics.GraphicsDevice.Indices = this.triangleListIndexBuffer;

				graphics.GraphicsDevice.DrawIndexedPrimitives(
					PrimitiveType.TriangleList,
					0,  // vertex buffer offset to add to each element of the index buffer
					0,  // minimum vertex index
					terrainBuffer.Length, // number of vertices
					0,  // first index element to read
					numberOfPrimitives   // number of primitives to draw
				);

                //this.graphics.GraphicsDevice.Vertices[ 0 ].SetSource( this.terrainVertexBuffer, 0, VertexPositionNormalTexture.SizeInBytes );
                //this.graphics.GraphicsDevice.DrawPrimitives( PrimitiveType.TriangleList, 0, numberOfPrimitives );

                pass.End();
            }
            
            this.basicEffect.End();
		}

		private void meRenderTriangleStrips( DrawContext context )
		{
			int             numberOfPrimitivesPerStrip = ( this.height - 1 ) * 2;
            int				stripCount = this.width - 1;
			int				numberVertexPerStrip = this.height * 2;

            this.graphics.GraphicsDevice.VertexDeclaration = this.basicEffectVertexDeclaration;
            
            this.basicEffect.View = context.camera.viewMtx;
            this.basicEffect.Projection = context.camera.projMtx;
			this.basicEffect.FogEnabled = true;
			
            this.basicEffect.Begin();
            this.basicEffect.EnableDefaultLighting();

			for ( int stripIndex = 0; stripIndex < stripCount; stripIndex++ )
			{
				foreach ( EffectPass pass in this.basicEffect.CurrentTechnique.Passes )
				{
					pass.Begin();
	               
					this.graphics.GraphicsDevice.Vertices[0].SetSource( terrainVertexBuffer, 0, VertexPositionNormalTexture.SizeInBytes );
					this.graphics.GraphicsDevice.Indices = this.triangleListIndexBuffer;

					graphics.GraphicsDevice.DrawIndexedPrimitives(
						PrimitiveType.TriangleStrip,
						0,  // vertex buffer offset to add to each element of the index buffer
						0,  // minimum vertex index
						terrainBuffer.Length, // number of vertices
						numberVertexPerStrip * stripIndex,  // first index element to read
						numberOfPrimitivesPerStrip   // number of primitives to draw
					);

					//this.graphics.GraphicsDevice.Vertices[ 0 ].SetSource( this.terrainVertexBuffer, 0, VertexPositionNormalTexture.SizeInBytes );
					//this.graphics.GraphicsDevice.DrawPrimitives( PrimitiveType.TriangleList, 0, numberOfPrimitives );

					pass.End();
				}
			}

            this.basicEffect.End();
		}
    }
}
