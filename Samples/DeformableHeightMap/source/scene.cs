using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Bullshoot.Code
{
    public class Scene
    {
        List<Avatar>    avatarList = new List<Avatar>(100);
		List<Avatar>    removeList = new List<Avatar>(10);
        Player          player;
        GamePadState    prevState;
        internal Background      background;

        public Scene()
        {
        }
		
        public Avatar CreateAvatar( AvatarParams avParams )
        {
            Avatar avatar = new Avatar(avParams);
			//Avatar avatar = new typeof( T )( avParams );

            avatar.Load( avParams.modelName );
            avatar.orientation.position = avParams.position;
            
            avatarList.Add(avatar);

            return avatar;
        }

		public void DestoryAvatar( Avatar avatar )
		{
			//avatarList.Remove( avatar );
			removeList.Add( avatar );
		}

		public void AddAvatarToAvatarList( Avatar avatar )
		{
			avatarList.Add(avatar);
		}

        public void Load()
        {
            // Create Player
            PlayerParams playerParams = new PlayerParams();
            playerParams.position = new Vector3(0, 0, 50);
            player = new Player(playerParams);

            background = new Background();
        }

        public void Draw()
        {
            DrawContext drawContext;

            drawContext = new DrawContext();
            drawContext.camera = player.camera;
            drawContext.camera.Update();

            background.Draw(drawContext);

            foreach (Avatar avatar in avatarList)
            {
                avatar.Draw(drawContext);
            }

			player.Draw(drawContext);
        }

        public void Tick( GameTime time )
        {
            GamePadState state = GamePad.GetState(PlayerIndex.One);
            TickContext tickContext = new TickContext(time, state, prevState);

			background.Tick(tickContext);

            player.Tick(tickContext);

            foreach (Avatar avatar in avatarList)
            {
                avatar.Tick(tickContext);
            }

			foreach (Avatar avatar in removeList )
			{
				avatarList.Remove( avatar );
			}
			removeList.Clear();

            prevState = state;
        }
    }
}
