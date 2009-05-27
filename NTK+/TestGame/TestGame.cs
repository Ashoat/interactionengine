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


namespace Game
{

    public class TestGame
    {

        static NTKPlusUser user;
        static LoadRegion loadRegion;

        public static void Main()
        {
            // Initialize the Engine.
            // Engine.status = Engine.Status.SINGLE_PLAYER;
            // ^---- That's the default. No need to set it.
            // Initialize the user and their personal LoadRegion.
            // UserInterface3D interface3D = new UserInterface3D();
            // ^---- absolute waste of memory
            Engine.userInterface = new UserInterface3D();
            // Set up the user
            user = new NTKPlusUser();
            NTKPlusUser.localUser = user;
            UserInterface3D.user = user;
            loadRegion = LoadRegion.createLoadRegion(); // <--- are we planning on doing anything with this?
            // Set up the camera
            Vector3 cameraPos = new Vector3(75, 40, 75); //30
            user.camera.SetLookAt(cameraPos, Vector3.Zero, Vector3.Up);
            // Set up some basic GameObjects.
            Terrain terrain = GameObject.createGameObject<Terrain>(loadRegion);
            terrain.initialize(2f, .1f, loadRegion);
            Human human = GameObject.createGameObject<Human>(loadRegion);
            human.initialize(terrain);
            FrameRateCounter frameRateCounter = GameObject.createGameObject<FrameRateCounter>(loadRegion);
            Engine.run();
        }

        public static void initializeStuff() {

            user.camera.SetPerspectiveFov(45f, UserInterface3D.graphicsDevice.Viewport.AspectRatio, 1.0f, 1000.0f);
            Vector3 cameraPos = new Vector3(75, 40, 75); //30
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

    }

}