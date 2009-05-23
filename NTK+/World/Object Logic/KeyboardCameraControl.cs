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
using InteractionEngine.GameWorld;
using NTKPlusGame.World.Modules;
using InteractionEngine.Client;
using InteractionEngine.Client.ThreeDimensional;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace NTKPlusGame.World {

    /**
     * Template
     */
    public class KeyboardCameraControl : GameObject, Keyboardable {

        #region FACTORY

        // The classHash, a unique identifying string for the class. Hmm, wow, that's kind of redundant, isn't that? C# already provides such a function through reflection. Oh well.
        // Used for the factory methods called when the client receives a CREATE_NEW_OBJECT update from the server computer.
        internal const string classHash = "KeyboardCameraControl";

        /// <summary>
        /// The static constructor. Adds the class's factory method to the GameObject factoryList when the class is first loaded.
        /// </summary>
        static KeyboardCameraControl() {
            GameObject.factoryList.Add(classHash, new GameObjectFactory(makeKeyboardCameraControl));
        }

        /// <summary>
        /// A factory method that creates and returns a new instance of KeyboardCameraControl. Used by the client when the server requests it to make a new GameObject.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        /// <param name="id">This GameObject's ID.</param>
        /// <param name="reader">The PacketReader from which we will read the fields of the newly constructed GameObject.</param>
        /// <returns>A new instance of KeyboardCameraControl.</returns>
        static KeyboardCameraControl makeKeyboardCameraControl(LoadRegion loadRegion, int id, Microsoft.Xna.Framework.Net.PacketReader reader) {
            if (GameWorld.status != GameWorld.Status.MULTIPLAYER_CLIENT)
                throw new System.Exception("You're not a client, so why are you calling the GameObject factory method?");
            KeyboardCameraControl gameObject = new KeyboardCameraControl(loadRegion, id);
            // ORDER OF STUFF (where you used the reader to construct datatypes, used factory methods exclusively. also, construct modules and their datatypes here too.)
            return gameObject;
        }

        /// <summary>
        /// Constructs a GameObject and assigns it an ID.
        /// This is the constructor that should be used if and only if you are a MULTIPLAYER_CLIENT.
        /// Furthermore, it is only called by the GameObjectFactory method.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        /// <param name="id">This GameObject's ID.</param>
        private KeyboardCameraControl(LoadRegion loadRegion, int id)
            : base(loadRegion, id) {
        }

        /// <summary>
        /// Returns the class hash. 
        /// </summary>
        /// <returns>The class hash. Do we really have to tell you everything twice?</returns>
        public override string getClassHash() {
            return classHash;
        }

        #endregion


        /// <summary>
        /// Constructs a new KeyboardCameraControl.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        public KeyboardCameraControl(LoadRegion loadRegion)
            : base(loadRegion) {
        }



        #region Keyboardable Members

        public void keyPressed(Microsoft.Xna.Framework.Input.Keys key) {
            Camera camera = NTKPlusUser.localUser.camera;
            if (key == Keys.Up) camera.ChangeAzimuth(camera.Target, Vector3.Up, 1f);
            if (key == Keys.Down) camera.ChangeAzimuth(camera.Target, Vector3.Up, 1f);
            if (key == Keys.Left) camera.RotateUponAxis(camera.Target, Vector3.Up, 1f);
            if (key == Keys.Right) camera.RotateUponAxis(camera.Target, Vector3.Up, 1f);
        }

        public void focusLost(Keyboardable newFocusHolder) {
            throw new System.NotImplementedException();
        }

        #endregion
    }

}