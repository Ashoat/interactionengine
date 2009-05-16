/*••••••••••••••••••••••••••••••••••••••••*\
| Wumpus Game                              |
| (C) Copyright Bluestone Coding 2008      |
|••••••••••••••••••••••••••••••••••••••••••|
| Built using the Interaction Engine       |
| (C) Copyright Bluestone Coding 2008      |
|••••••••••••••••••••••••••••••••••••••••••|
|           __    ___ ___  ___             |
|          /++\  | _ ) __|/ __|            |
|          \++/  | _ \__ \ (__             |
|           \/   |___/___/\___|            |
|                                          |
|••••••••••••••••••••••••••••••••••••••••••|
| GRAPHICS                                 |
| * DoorTextGraphics                 Class |
\*••••••••••••••••••••••••••••••••••••••••*/

using WumpusGame.World.Modules;
using InteractionEngine.Constructs;
using System.Collections.Generic;
using InteractionEngine.GameWorld;
using InteractionEngine.Client.Text;
using InteractionEngine.Client.TwoDimensional;

namespace WumpusGame.World.Graphics
{

    public class Door2DGraphics : Graphics2D, DoorGraphics
    {

        public const int NORTH_X = 362;
        public const int NORTH_Y = 2;
        public const int NORTHWEST_X = 70;
        public const int NORTHWEST_Y = 290;
        public const int SOUTHWEST_X = 85;
        public const int SOUTHWEST_Y = 505;
        public const int SOUTH_X = NORTH_X;
        public const int SOUTH_Y = 800;
        public const int SOUTHEAST_X = 760;
        public const int SOUTHEAST_Y = 710;
        public const int NORTHEAST_X = 795;
        public const int NORTHEAST_Y = 125;

        private int direction;

        private const float PIXELS_PER_ITERATION = 2.5f;

        public Door2DGraphics(InteractionEngine.Constructs.GameObject gameObject, int direction) : base(gameObject){
            this.direction = direction;
        }

        public override void onDraw() {
 //           base.applyRotation(60);
            base.onDraw();
            base.addExtremities();
        }

        public override void loadContent() {
            base.loadTexture("Door");
            
            switch (this.direction) {
                case Room.NORTH:
                    this.changePosition(NORTH_X, NORTH_Y);
                    break;
                case Room.SOUTH:
                    this.changePosition(SOUTH_X, SOUTH_Y);
                    break;
                case Room.NORTHEAST:
                    this.changePosition(NORTHEAST_X, NORTHEAST_Y);
                    this.applyRotation(60);
                    break;
                case Room.SOUTHEAST:
                    this.changePosition(SOUTHEAST_X, SOUTHEAST_Y);
                    this.applyRotation(-60);
                    break;
                case Room.NORTHWEST:
                    this.changePosition(NORTHWEST_X, NORTHWEST_Y);
                    this.applyRotation(-60);
                    break;
                case Room.SOUTHWEST:
                    this.changePosition(SOUTHWEST_X, SOUTHWEST_Y);
                    this.applyRotation(60);
                    break;
            }
        }

        /// <summary>
        /// Leave the current Room and enter a new one.
        /// </summary>
        /// <param name="direction">The corner of the Room where the door is located at. This is neccessary to know where the Player needs to be made to walk to.</param>
        /// <param name="room">The new Room that is being walked to. Neccessary as a parameter to Playable.getLocation().move().</param>
        /// <param name="player">The Player that is moving. Neccessary to call PlayerGraphics.onMove() and also to call Playable.getLocation().move() when neccessary.</param>
        public void onDoorOpen(Player player, Room room, int direction) {
            //this.animation = true;
            //this.direction = direction;
        }

    }

}