using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

using Bullshoot.Code;

namespace Terrain
{
    static class Program
    {
        static Game1       game;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main( string[] args )
        {
            using ( game = new Game1() )
            {
                game.Run();
            }
        }

        static public void GetContentManager( out ContentManager content )
        {
            game.GetContentManager( out content );
        }

        static public void GetGraphicsManager( out GraphicsDeviceManager graphics )
        {
            game.GetGraphicsManager( out graphics );
        }

		static public Scene GetActiveScene()
		{
			return game.GetActiveScene();
		}
    }
}

