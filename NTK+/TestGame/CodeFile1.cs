// blah blah comments go here.

using InteractionEngine.Constructs;
using InteractionEngine.Server;
using InteractionEngine.Client.ThreeDimensional;
using InteractionEngine.GameWorld;
using System;
using NTKPlusGame.World;
using InteractionEngine.Client;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
// todo:
// make gold work
// make trivia work
// make random cave hobos
// allow the wumpus to leave you alive
// add a method for going to a random room so both bat and wumpus have it
// get rid of all the manual eventHashlist modification, replace by making the UI handle that
// make a method for creating a world
// make 3D work
// make multiplayer work
namespace Game
{

    public class main
    {

        static User3D user;
        static TerrainedLoadRegion terrainedLoadRegion;

        public static void Main()
        {
            // Initialize the GameWorld.
            GameWorld.game = new InteractionGame();
            GameWorld.status = GameWorld.Status.SINGLE_PLAYER;
            GameWorld.game.setWindowSize(1000, 1100);
            GameWorld.game.setBackgroundColor(Microsoft.Xna.Framework.Graphics.Color.AliceBlue);
            // Initialize the user and their personal LoadRegion.
             user = new User3D();
            GameWorld.user = user;
            GameWorld.userInterface = new UserInterface3D();
            GameWorld.game.initializeMethod = new InitializeMethod(initializeStuff);
            terrainedLoadRegion = new TerrainedLoadRegion();
            GameWorld.addLoadRegion(terrainedLoadRegion);
            user.addLoadRegion(terrainedLoadRegion);

            GameWorld.game.Run();
        }

        public static void initializeStuff()
        {

            Model sphere = GameWorld.game.Content.Load<Model>("Models\\sphere180");
            Texture2D tex = GameWorld.game.Content.Load<Texture2D>("Amazonia"); //tex1.png //Amazonia.jpg
            Texture2D texImage = GameWorld.game.Content.Load<Texture2D>("heightImage"); //heightImage
            //FileStream fileStream = File.OpenRead(this.Content.RootDirectory + "/map.raw");

            Vector3 cameraPos = new Vector3(75, 40, 75); //30


            user.camera.SetLookAt(cameraPos, Vector3.Zero, Vector3.Up);
            user.camera.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), GameWorld.game.GraphicsDevice.Viewport.AspectRatio, 1.0f, 1000.0f);

            GameWorld.game.GraphicsDevice.RenderState.CullMode = CullMode.None;

            Terrain terrain = new Terrain(terrainedLoadRegion, texImage, tex, 2f, .1f);

            new Human(terrainedLoadRegion);
        }

    }

    public class Human : WalkerTemplate {

        
        #region FACTORY

        // The classHash, a unique identifying string for the class. Hmm, wow, that's kind of redundant, isn't that? C# already provides such a function through reflection. Oh well.
        // Used for the factory methods called when the client receives a CREATE_NEW_OBJECT update from the server computer.
        internal const string classHash = "Human";

        /// <summary>
        /// The static constructor. Adds the class's factory method to the GameObject factoryList when the class is first loaded.
        /// </summary>
        static Human() {
            GameObject.factoryList.Add(classHash, new GameObjectFactory(makeHuman));
        }

        /// <summary>
        /// A factory method that creates and returns a new instance of Human. Used by the client when the server requests it to make a new GameObject.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        /// <param name="id">This GameObject's ID.</param>
        /// <param name="reader">The PacketReader from which we will read the fields of the newly constructed GameObject.</param>
        /// <returns>A new instance of Human.</returns>
        static Human makeHuman(LoadRegion loadRegion, int id, Microsoft.Xna.Framework.Net.PacketReader reader) {
            if (GameWorld.status != GameWorld.Status.MULTIPLAYER_CLIENT)
                throw new System.Exception("You're not a client, so why are you calling the GameObject factory method?");
            Human human = new Human(loadRegion, id);
            // ORDER OF STUFF (where you used the reader to construct datatypes, used factory methods exclusively. also, construct modules and their datatypes here too.)
            return human;
        }

        /// <summary>
        /// Constructs a GameObject and assigns it an ID.
        /// This is the constructor that should be used if and only if you are a MULTIHuman_CLIENT.
        /// Furthermore, it is only called by the GameObjectFactory method.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        /// <param name="id">This GameObject's ID.</param>
        private Human(LoadRegion loadRegion, int id)
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
        /// Returns the Graphics module of this GameObject.
        /// </summary>
        /// <returns>The Selection module associated with this GameObject.
        private readonly Graphics3D graphics;
        public override Graphics getGraphics() {
            return graphics;
        }
        public override Graphics3D getGraphics3D() {
            return graphics;
        }

        public Human(TerrainedLoadRegion loadRegion) : base(loadRegion)
        {
            Graphics3D.ModelEffect modelEffect = new Graphics3D.ModelEffect(GameWorld.game.GraphicsDevice, null);
            //modelEffect.SpecularColor = new Vector3(.1f, .3f, .6f);
            //modelEffect.SpecularPower = 10f;
            modelEffect.Texture = GameWorld.game.Content.Load<Texture2D>("Images\\human_texture");
            modelEffect.TextureEnabled = true;
            //modelEffect.Texture = null;
            modelEffect.SpecularPower = 1f;
            Model model = GameWorld.game.Content.Load<Model>("Models\\Borat");
            this.graphics = new Graphics3D(this, model, modelEffect);
            this.graphics.SetScale(3f);
            this.getLocation().yaw = MathHelper.Pi;
        }

    }

}