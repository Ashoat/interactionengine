/*••••••••••••••••••••••••••••••••••••••••*\
| Interaction Engine                       |
| (C) Copyright Bluestone Coding 2008      |
|••••••••••••••••••••••••••••••••••••••••••|
|           __    ___ ___  ___             |
|          /++\  | _ ) __|/ __|            |
|          \++/  | _ \__ \ (__             |
|           \/   |___/___/\___|            |
|                                          |
|••••••••••••••••••••••••••••••••••••••••••|
| XNA INTEGRATION                          |
| * InteractionGame           Class        |
| * InteractionSprite         Class        |
\*••••••••••••••••••••••••••••••••••••••••*/

namespace InteractionEngine.GameWorld {

    /**
     * This class specifies an InteractionEngine-integrated XNA Game.
     */
    public class InteractionGame : Microsoft.Xna.Framework.Game {

        // Contains a reference to the display screen.
        // Used for outputting graphics.
        public Microsoft.Xna.Framework.GraphicsDeviceManager graphics;
        // Contains a reference to a multiplayer game's NetworkSession.
        // Used for sending and recieving data from other gamers.
        public Microsoft.Xna.Framework.Net.NetworkSession session;

        /// <summary>
        /// Constructs the InteractionGame.
        /// </summary>
        public InteractionGame()
            : base() {
            // Create the XNA graphics manager.
            graphics = new Microsoft.Xna.Framework.GraphicsDeviceManager(this);
            // Specify the folder in the project where sprites, models, etc. are stored.
            Content.RootDirectory = "Content";
            // Add the GamerServciesComponent to the Game class. Check MSDN's Programming Guide for XNA Networking for more information.
            Components.Add(new Microsoft.Xna.Framework.GamerServices.GamerServicesComponent(this));
            // Create the network session.
            if (InteractionEngine.GameWorld.GameWorld.status != InteractionEngine.GameWorld.GameWorld.Status.SINGLE_PLAYER) {
                session = Microsoft.Xna.Framework.Net.NetworkSession.Create(Microsoft.Xna.Framework.Net.NetworkSessionType.SystemLink, 1, 4);
                session.AllowHostMigration = false;
                session.AllowJoinInProgress = false;
                /*session.GameEnded += new System.EventHandler<Microsoft.Xna.Framework.Net.GameEndedEventArgs>();
                session.GamerJoined += new System.EventHandler<Microsoft.Xna.Framework.Net.GamerJoinedEventArgs>();
                session.GamerLeft += new System.EventHandler<Microsoft.Xna.Framework.Net.GamerLeftEventArgs>();
                session.GameStarted += new System.EventHandler<Microsoft.Xna.Framework.Net.GameStartedEventArgs>();
                session.SessionEnded += new System.EventHandler<Microsoft.Xna.Framework.Net.NetworkSessionEndedEventArgs>();*/
            }
            this.IsMouseVisible = true;
        }

        /// <summary>
        /// Instantiate all the LoadRegions, etc. that MUST HAVE GUARANTEED ORDER!!!
        /// </summary>
        protected override void Initialize() {
            base.Initialize();
        }

        /// <summary>
        /// WTF?!?!?!? NOTHING!!!
        /// </summary>
        protected override void LoadContent() {
            GameWorld.userInterface.initialize();
            this.graphics.PreferredBackBufferWidth = width;
            this.graphics.PreferredBackBufferHeight = height;
            this.graphics.ApplyChanges();
            this.graphics.GraphicsDevice.Clear(this.color);
            // TODO: i = 1?
            for (int i = 1; i < GameWorld.getFieldContainerCount()+1; i++) {
                InteractionEngine.Constructs.FieldContainer oneOfThem = GameWorld.getFieldContainer(i);
                if (oneOfThem is Client.Graphable) {
                    ((Client.Graphable)oneOfThem).getGraphics().loadContent();
                }
            }
            GameWorld.userInterface.start();
        }

        private int height, width;
        /// <summary>
        /// Must be called before this.Run();
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void setWindowSize(int width, int height) {
            this.height = height; this.width = width;
        }

        Microsoft.Xna.Framework.Graphics.Color color;
        public void setBackgroundColor(Microsoft.Xna.Framework.Graphics.Color color) {
            this.color = color;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent() {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(Microsoft.Xna.Framework.GameTime gameTime) {
            this.graphics.GraphicsDevice.Clear(this.color);
            GameWorld.run(gameTime);
            base.Update(gameTime);
        }

        /// <summary>
        /// We override it so that it doesn't keep clearing the screen in its almighty annoyance.
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Draw(Microsoft.Xna.Framework.GameTime gameTime) {
        }

    }

}