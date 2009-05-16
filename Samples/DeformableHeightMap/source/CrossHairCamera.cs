using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Bullshoot.Code;

using Terrain;

namespace CrossHair
{
	class CrossHairCamera : Camera
	{
		GraphicsDeviceManager			graphics;
		Texture2D						crosshairTexture;
		Rectangle						rect;

		public CrossHairCamera() : base()
		{
			ContentManager			content;
			Point					screenCentre;
			int						textureWidth;
			int						textureHeight;
			

			Program.GetContentManager( out content );
			Program.GetGraphicsManager( out this.graphics );

			this.crosshairTexture = content.Load< Texture2D >( "textures/crosshair" );

			textureHeight = this.crosshairTexture.Height;
			textureWidth = this.crosshairTexture.Width;

			screenCentre = new Point( graphics.GraphicsDevice.Viewport.Width / 2, graphics.GraphicsDevice.Viewport.Height / 2 );
			
			this.rect = new Rectangle( screenCentre.X - ( textureWidth / 2 ), screenCentre.Y - ( textureHeight / 2 ), textureWidth, textureHeight );
		}

		public override void Draw( DrawContext context )
		{
			SpriteBatch				sprite;

			sprite = new SpriteBatch( this.graphics.GraphicsDevice );

			sprite.Begin( SpriteBlendMode.AlphaBlend, SpriteSortMode.BackToFront, SaveStateMode.SaveState );

			sprite.Draw( this.crosshairTexture, this.rect, Color.White );

			sprite.End();
		}

		//////////////////////////////////////////////////////
        //  Private
        //////////////////////////////////////////////////////

	}
}
