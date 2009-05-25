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
            Engine.game = new InteractionGame();
            Engine.status = Engine.Status.SINGLE_PLAYER;
            //Engine.game.setWindowSize(1000, 1100);
            Engine.game.setBackgroundColor(Microsoft.Xna.Framework.Graphics.Color.AliceBlue);
            // Initialize the user and their personal LoadRegion.
            UserInterface3D interface3D = new UserInterface3D();
            Engine.userInterface = interface3D;
            user = new NTKPlusUser();
            NTKPlusUser.localUser = user;
            UserInterface3D.user = user;
            loadRegion = LoadRegion.createLoadRegion();

            Vector3 cameraPos = new Vector3(75, 40, 75); //30

            user.camera.SetLookAt(cameraPos, Vector3.Zero, Vector3.Up);

            Terrain terrain = GameObject.createGameObject<Terrain>(loadRegion);
            terrain.initialize(2f, .1f, loadRegion);
            Human human = GameObject.createGameObject<Human>(loadRegion);
            human.initialize(terrain);

            //Engine.game.initializeMethod = new InitializeMethod(initializeStuff);

            Engine.game.Run();
        }

        public static void initializeStuff() {

            user.camera.SetPerspectiveFov(MathHelper.ToRadians(45f), Engine.game.GraphicsDevice.Viewport.AspectRatio, 1.0f, 1000.0f);
            Vector3 cameraPos = new Vector3(75, 40, 75); //30
            user.camera.SetLookAt(cameraPos, Vector3.Zero, Vector3.Up);
            user.camera.Zoom(1000);
            Engine.game.GraphicsDevice.RenderState.CullMode = CullMode.None;

            Engine.game.GraphicsDevice.RenderState.AlphaBlendEnable = true;
            Engine.game.GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha; // source rgb * source alpha
            Engine.game.GraphicsDevice.RenderState.AlphaSourceBlend = Blend.One; // don't modify source alpha
            Engine.game.GraphicsDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha; // dest rgb * (255 - source alpha)
            Engine.game.GraphicsDevice.RenderState.AlphaDestinationBlend = Blend.InverseSourceAlpha; // dest alpha * (255 - source alpha)
            Engine.game.GraphicsDevice.RenderState.BlendFunction = BlendFunction.Add; // add source and dest results

        }

    }

    public class Human : WalkerTemplate {


        #region FACTORY

        /// <summary>
        /// All GameObjects need a parameterless constructor for calling by GameObject.createGameObject() and GameObject.createFromUpdate().
        /// NEVER CALL THIS! This constructor is exclusively for use by the InteractionEngine. If anyone else calls it things will break.
        /// If you want to construct this object, use GameObject.createGameObject(LoadRegion).
        /// </summary>
        public Human() {
        }

        // The classHash, a unique identifying string for the class. Hmm, wow, that's kind of redundant, isn't that? C# already provides such a function through reflection. Oh well.
        // Used for the factory methods called when the client receives a CREATE_NEW_OBJECT update from the server computer.
        public const string realHash = "Human";
        public override string classHash {
            get { return realHash; }
        }

        /// <summary>
        /// The static constructor. Adds the class's factory method to the GameObject factoryList when the class is first loaded.
        /// </summary>
        static Human() {
            GameObject.factoryList.Add(realHash, new GameObjectFactory(GameObject.createFromUpdate<Human>));
        }

        #endregion


        private UpdatableGameObject<DebugSphere> debugSphere;

        /// <summary>
        /// Returns the Graphics module of this GameObject.
        /// </summary>
        /// <returns>The Selection module associated with this GameObject.
        private Graphics3D graphics;
        public override Graphics getGraphics() {
            return graphics;
        }
        public override Graphics3D getGraphics3D() {
            return graphics;
        }

        public override void construct() {
            base.construct();
            ModelEffect modelEffect = new ModelEffect(); // new Graphics3D.ModelEffect(Engine.game.GraphicsDevice, null);
            modelEffect.SpecularColor = new Vector3(.1f, .3f, .6f);
            modelEffect.SpecularPower = 10f;
            modelEffect.setTextureName("Images\\human_texture");
            modelEffect.TextureEnabled = true;
            //modelEffect.Texture = null;
            modelEffect.SpecularPower = 1f;
            modelEffect.CommitProperties();
            this.graphics = new Graphics3D(this, modelEffect, "Models\\Borat");
            this.graphics.SetScale(3f);
            this.getLocation().yaw = MathHelper.Pi;

            DebugSphere newDebugSphere = GameObject.createGameObject<DebugSphere>(this.getLoadRegion());

            this.debugSphere = new UpdatableGameObject<DebugSphere>(this, newDebugSphere);
        }

        public override Event getEvent(int invoker, Vector3 position) {
            return base.getEvent(invoker, position);
            //this.debugSphere.value.setPosition(this.graphics.BoundingSphere.Center, this.graphics.BoundingSphere.Radius);
        }

    }

}