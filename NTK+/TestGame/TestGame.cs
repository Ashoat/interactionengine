// blah blah comments go here.

using InteractionEngine.Constructs;
using InteractionEngine.Networking;
using InteractionEngine.UserInterface.ThreeDimensional;
using InteractionEngine;
using System;
using NTKPlusGame.World;
using InteractionEngine.UserInterface;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using InteractionEngine.Constructs.Datatypes;
using InteractionEngine.EventHandling;
using NTKPlusGame.World.Modules;
using WumpusGame.World;
using InteractionEngine.UserInterface.Audio;


namespace Game
{

    public class TestGame
    {

        static NTKPlusUser user;

        public static void Main()
        {
            // Initialize the Engine.
            // Engine.status = Engine.Status.SINGLE_PLAYER;
            // ^---- That's the default. No need to set it.
            // Initialize the user and their personal LoadRegion.
            // UserInterface3D interface3D = new UserInterface3D();
            // ^---- absolute waste of memory
            Engine.userInterface = new UserInterface3D();
            Audio.loadAudioSettings("Content\\NTK_music.xgs");
            // Set up the user
            user = new NTKPlusUser();
            NTKPlusUser.localUser = user;
            UserInterface3D.user = user;
            // Set up the camera
            Microsoft.Xna.Framework.Vector3 cameraPos = new Microsoft.Xna.Framework.Vector3(75, 40, 75); //30
            Game.TestGame.user.camera.SetLookAt(cameraPos, Microsoft.Xna.Framework.Vector3.Zero, Microsoft.Xna.Framework.Vector3.Up);
            // blah
            new Terrain();
            new Human();
            new FrameRateCounter();
            new DebugSphere();
            new SkyDome();
            new InfoTab();
            new InfoButton();
            KeyboardFocus keyboardFocus = new KeyboardFocus();
            ((UserInterface3D)Engine.userInterface).registerKeyboardFocus(keyboardFocus);
            keyboardFocus.setFocus(new KeyboardCameraControl());
            // Set up the LobbyButtons
            LobbyButtons buttons = GameObject.createGameObject<LobbyButtons>(user.localLoadRegion);
            GameObject.createGameObject<Initializer>(user.localLoadRegion);
            Engine.run();
        }

        public static void initializeStuff() {

            user.camera.SetPerspectiveFov(45f, UserInterface3D.graphicsDevice.Viewport.AspectRatio, 1.0f, 9000.0f);
            Vector3 cameraPos = new Vector3(600, 600, 600); //30
            user.camera.SetLookAt(cameraPos, Vector3.Zero, Vector3.Up);
            UserInterface3D.graphicsDevice.RenderState.CullMode = CullMode.None;

            //UserInterface3D.graphicsDevice.RenderState.MultiSampleAntiAlias = false;
            UserInterface3D.graphicsDevice.RenderState.DepthBufferEnable = true;
            UserInterface3D.graphicsDevice.RenderState.AlphaBlendEnable = true;
            UserInterface3D.graphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha; // source rgb * source alpha
            UserInterface3D.graphicsDevice.RenderState.AlphaSourceBlend = Blend.One; // don't modify source alpha
            UserInterface3D.graphicsDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha; // dest rgb * (255 - source alpha)
            UserInterface3D.graphicsDevice.RenderState.AlphaDestinationBlend = Blend.InverseSourceAlpha; // dest alpha * (255 - source alpha)
            UserInterface3D.graphicsDevice.RenderState.BlendFunction = BlendFunction.Add; // add source and dest results
        }

        public class Initializer : GameObject, Graphable {

            #region FACTORY

            /// <summary>
            /// All GameObjects need a parameterless constructor for calling by GameObject.createGameObject() and GameObject.createFromUpdate().
            /// NEVER CALL THIS! This constructor is exclusively for use by the InteractionEngine. If anyone else calls it things will break.
            /// If you want to construct this object, use GameObject.createGameObject(LoadRegion).
            /// </summary>
            public Initializer() {
            }

            // The classHash, a unique identifying string for the class. Hmm, wow, that's kind of redundant, isn't that? C# already provides such a function through reflection. Oh well.
            // Used for the factory methods called when the client receives a CREATE_NEW_OBJECT update from the server computer.
            public const string realHash = "Initializer";
            public override string classHash {
                get { return realHash; }
            }

            /// <summary>
            /// The static constructor. Adds the class's factory method to the GameObject factoryList when the class is first loaded.
            /// </summary>
            static Initializer() {
                GameObject.factoryList.Add(realHash, new GameObjectFactory(GameObject.createFromUpdate<Initializer>));
            }

            #endregion

            /*••••••••••••••••••••••••••••••••••••••••*\
              MODULES
            \*••••••••••••••••••••••••••••••••••••••••*/

            public Location getLocation() {
                return null;
            }

            InitializerGraphics graphics;
            public Graphics getGraphics() {
                return graphics;
            }


            /*••••••••••••••••••••••••••••••••••••••••*\
              MEMBERS
            \*••••••••••••••••••••••••••••••••••••••••*/

            /// <summary>
            /// Constructs the Initializer.
            /// </summary>
            /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
            public override void construct() {
                this.graphics = new InitializerGraphics(this);
            }

            class InitializerGraphics : Graphics {

                Initializer gameObject;

                public InitializerGraphics(Initializer gameObject) {
                    this.gameObject = gameObject;
                }

                public void onDraw() {

                }

                public void loadContent() {
                    initializeStuff();
                    gameObject.deconstruct();
                }

            }

        }

    }

}