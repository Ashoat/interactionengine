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
| GAME OBJECTS                             |
| * KeyboardCameraControl            Class |
\*••••••••••••••••••••••••••••••••••••••••*/

using InteractionEngine.Constructs;
using InteractionEngine;
using NTKPlusGame.World.Modules;
using InteractionEngine.UserInterface;
using InteractionEngine.UserInterface.ThreeDimensional;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace NTKPlusGame.World {

    /**
     * Template
     */
    public class KeyboardCameraControl : GameObject, Keyboardable {


        #region FACTORY

        /// <summary>
        /// All GameObjects need a parameterless constructor for calling by GameObject.createGameObject() and GameObject.createFromUpdate().
        /// NEVER CALL THIS! This constructor is exclusively for use by the InteractionEngine. If anyone else calls it things will break.
        /// If you want to construct this object, use GameObject.createGameObject(LoadRegion).
        /// </summary>
        public KeyboardCameraControl() {
        }

        // The classHash, a unique identifying string for the class. Hmm, wow, that's kind of redundant, isn't that? C# already provides such a function through reflection. Oh well.
        // Used for the factory methods called when the client receives a CREATE_NEW_OBJECT update from the server computer.
        public const string realHash = "KeyboardCameraControl";
        public override string classHash {
            get { return realHash; }
        }

        /// <summary>
        /// The static constructor. Adds the class's factory method to the GameObject factoryList when the class is first loaded.
        /// </summary>
        static KeyboardCameraControl() {
            GameObject.factoryList.Add(realHash, new GameObjectFactory(GameObject.createFromUpdate<KeyboardCameraControl>));
        }

        #endregion


        /// <summary>
        /// Constructs a new KeyboardCameraControl.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        public override void construct() {
        }

        #region Keyboardable Members

        public void keyPressed(Microsoft.Xna.Framework.Input.Keys key) {
            Camera camera = NTKPlusUser.localUser.camera;
            if (key == Keys.Up) camera.ChangeAzimuth(camera.Target, Vector3.Up, 1f);
            if (key == Keys.Down) camera.ChangeAzimuth(camera.Target, Vector3.Up, -1f);
            if (key == Keys.Right) camera.RotateUponAxis(camera.Target, Vector3.Up, 3);
            if (key == Keys.Left) camera.RotateUponAxis(camera.Target, Vector3.Up, -3f);
            if (key == Keys.W) camera.SetPosition(camera.getLocation().Position + 3 * planeProjection(camera.getLocation().Heading));
            if (key == Keys.S) camera.SetPosition(camera.getLocation().Position - 3 * planeProjection(camera.getLocation().Heading));
            if (key == Keys.D) camera.SetPosition(camera.getLocation().Position + 3 * planeProjection(camera.getLocation().Strafe));
            if (key == Keys.A) camera.SetPosition(camera.getLocation().Position - 3 * planeProjection(camera.getLocation().Strafe));
            if (key == Keys.PageUp) camera.SetPositionLockTarget(camera.getLocation().Position + 3 * camera.getLocation().Heading);
            if (key == Keys.PageDown) camera.SetPositionLockTarget(camera.getLocation().Position - 3 * camera.getLocation().Heading);
        }

        private Vector3 planeProjection(Vector3 input){
            input.Y = 0;
            return input;
        }

        public void focusLost(Keyboardable newFocusHolder) {
        }

        #endregion
    }

}