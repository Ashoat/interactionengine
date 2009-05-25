/*••••••••••••••••••••••••••••••••••••••••*\
| Interaction Engine                       |
| (C) Copyright Bluestone Coding 2008-2009 |
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

namespace InteractionEngine {

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
            this.IsMouseVisible = true;
        }

        /// <summary>
        /// Initialize stuff, I guess?
        /// </summary>
        protected override void Initialize() {
            base.Initialize();
            Engine.userInterface.initialize();
        }

        /// <summary>
        /// WTF?!?!?!? NOTHING!!!
        /// </summary>
        protected override void LoadContent() {
            if (width > 0) this.graphics.PreferredBackBufferWidth = width;
            if (height > 0) this.graphics.PreferredBackBufferHeight = height;
            this.graphics.ApplyChanges();
            this.graphics.GraphicsDevice.Clear(this.color);
            foreach (Constructs.GameObjectable gameObject in Engine.getGameObjectList()) { 
                if (gameObject is UserInterface.Graphable) {
                    ((UserInterface.Graphable)gameObject).getGraphics().loadContent();
                }
            }
        }

        private int height, width;
        /// <summary>
        /// Must be called before this.Run();
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void setWindowSize(int width, int height) {
            this.height = height;
            this.width = width;
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
            if (Microsoft.Xna.Framework.Input.GamePad.GetState(Microsoft.Xna.Framework.PlayerIndex.One).Buttons.Back == Microsoft.Xna.Framework.Input.ButtonState.Pressed) this.Exit();
            this.graphics.GraphicsDevice.Clear(this.color);
            Engine.run(gameTime);
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