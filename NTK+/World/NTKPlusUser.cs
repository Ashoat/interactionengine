/*••••••••••••••••••••••••••••••••••••••••*\
| NTK+                                     |
| (C) Copyright Bluestone Coding 2009      |
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
| USER                                     |
| * WalkerTemplate                   Class |
\*••••••••••••••••••••••••••••••••••••••••*/

using InteractionEngine.Constructs;
using InteractionEngine;
using InteractionEngine.UserInterface;
using NTKPlusGame.World.Modules;
using InteractionEngine.UserInterface.ThreeDimensional;
using Microsoft.Xna.Framework;
using InteractionEngine.Networking;
using InteractionEngine.UserInterface.Audio;

namespace NTKPlusGame.World {

    /// <summary>
    /// A user specific to uh this thing.
    /// </summary>
    public class NTKPlusUser : User3D {

		public static NTKPlusUser localUser;

        public readonly Team team;

        public readonly SelectionFocus selectionFocus;

        public readonly KeyboardFocus keyboardFocus;

        public readonly KeyboardCameraControl keyboardCameraControl;

        public readonly InfoDisplayBox infoDisplayBox;

        public NTKPlusUser()
            : base() {
            this.selectionFocus = GameObject.createGameObject<SelectionFocus>(this.localLoadRegion);
            this.keyboardFocus = GameObject.createGameObject<KeyboardFocus>(this.localLoadRegion);
            this.infoDisplayBox = GameObject.createGameObject<InfoDisplayBox>(this.localLoadRegion);
            this.keyboardCameraControl = GameObject.createGameObject<KeyboardCameraControl>(this.localLoadRegion);
            this.keyboardFocus.setFocus(keyboardCameraControl);
            ((UserInterface3D)Engine.userInterface).registerKeyboardFocus(keyboardFocus);
        }

        public static bool isOnOurTeam(Client client) {
            return Engine.status == Engine.Status.MULTIPLAYER_CLIENT ^ client == null;
        }

    }

    public enum Team : int {
        Team1,
        Team2,
        Team3
    }

}