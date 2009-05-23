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
| CLIENT                                   |
| * UserInterface2D                  Class |
| * Graphics2D              Abstract Class |
\*••••••••••••••••••••••••••••••••••••••••*/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace InteractionEngine.UserInterface.TwoDimensional {

    public class UserInterface2D : UserInterface {

        // Contains the SpriteBatch this UserInterface uses to draw its GameObjects.
        // Used for drawing GameObjects to the screen.
        public Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch;
        // Contain lists of EventHandling.Interactables that had been MOUSE_OVER'd and MOUSE_CLICK'd in the last iteration of input().
        // Used for knowing when to invoke MOUSE_OUT and MOUSE_RELEASE events.
        private System.Collections.Generic.List<EventHandling.Interactable> mouseOvered = new System.Collections.Generic.List<EventHandling.Interactable>();
        private System.Collections.Generic.List<EventHandling.Interactable> mouseLeftClicked = new System.Collections.Generic.List<EventHandling.Interactable>();
        private System.Collections.Generic.List<EventHandling.Interactable> mouseRightClicked = new System.Collections.Generic.List<EventHandling.Interactable>();
        // Contains an event invoker constant.
        // Used for figuring out what EventHandling.EventHandling.Event an EventHandling.Interactable should return.
        public const int MOUSE_OVER = 0;
        // Contains an event invoker constant.
        // Used for figuring out what EventHandling.EventHandling.Event an EventHandling.Interactable should return.
        public const int MOUSE_OUT = 1;
        // Contains an event invoker constant.
        // Used for figuring out what EventHandling.EventHandling.Event an EventHandling.Interactable should return.
        public const int MOUSE_RIGHT_CLICK = 2;
        // Contains an event invoker constant.
        // Used for figuring out what EventHandling.EventHandling.Event an EventHandling.Interactable should return.
        public const int MOUSE_RIGHT_RELEASE = 3;
        // Contains an event invoker constant.
        // Used for figuring out what EventHandling.EventHandling.Event an EventHandling.Interactable should return.
        public const int MOUSE_LEFT_CLICK = 4;
        // Contains an event invoker constant.
        // Used for figuring out what EventHandling.EventHandling.Event an EventHandling.Interactable should return.
        public const int MOUSE_LEFT_RELEASE = 5;

        /// <summary>
        /// Checks state of user input devices to see if an input event should be triggered.
        /// If so, collects the EventHandling.EventHandling.Event objects and inserts them into the given list.
        /// Make it quick!
        /// </summary>
        /// <param name="newEventList">The list into which newly detected events are to be inserted.</param>
        protected override void retrieveInput(System.Collections.Generic.List<EventHandling.Event> newEvents) {
            // Get mouse /*and keyboard*/ states
            Microsoft.Xna.Framework.Input.MouseState mouse = Microsoft.Xna.Framework.Input.Mouse.GetState();
            // Loop through previously clicked/over'd GameObjects to see if they have been released/out'd.
            for (int i = mouseOvered.Count - 1; i >= 0; i--) {
                EventHandling.Interactable interactable = mouseOvered[i];
                if (!(((Graphable)interactable).getGraphics() is Graphics2D)) continue;
                Graphics2D graphics = (Graphics2D)((Graphable)interactable).getGraphics();
                // MOUSE_OUT?
                if (!graphics.contains(mouse.X, mouse.Y)) {
                    EventHandling.Event evvie = interactable.getEvent(MOUSE_OUT);
                    if (evvie != null) newEvents.Add(evvie);
                    mouseOvered.RemoveAt(i);
                }
            }
            for (int i = mouseLeftClicked.Count - 1; i >= 0; i--) {
                EventHandling.Interactable interactable = mouseLeftClicked[i];
                if (!(((Graphable)interactable).getGraphics() is Graphics2D)) continue;
                Graphics2D graphics = (Graphics2D)((Graphable)interactable).getGraphics();
                // MOUSE_LEFT_RELEASE?
                if (!graphics.contains(mouse.X, mouse.Y) || mouse.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released) {
                    EventHandling.Event evvie = interactable.getEvent(MOUSE_LEFT_RELEASE);
                    if (evvie != null) newEvents.Add(evvie);
                    mouseLeftClicked.RemoveAt(i);
                }
            }
            for (int i = mouseRightClicked.Count - 1; i >= 0; i--) {
                EventHandling.Interactable interactable = mouseRightClicked[i];
                if (!(((Graphable)interactable).getGraphics() is Graphics2D)) continue;
                Graphics2D graphics = (Graphics2D)((Graphable)interactable).getGraphics();
                // MOUSE_RIGHT_RELEASE?
                if (!graphics.contains(mouse.X, mouse.Y) || mouse.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Released) {
                    EventHandling.Event evvie = interactable.getEvent(MOUSE_RIGHT_RELEASE);
                    if (evvie != null) newEvents.Add(evvie);
                    mouseRightClicked.RemoveAt(i);
                }
            }
            // Loop through all the GameObjects
            foreach (Constructs.GameObjectable gameObject in Engine.getGameObjectList()) {
                // See if this GameObject can be interacted with.
                if (gameObject is Graphable) {
                    if (!(((Graphable)gameObject).getGraphics() is Graphics2D)) continue;
                    Graphics2D graphics = (Graphics2D)((Graphable)gameObject).getGraphics();
                    if (gameObject is EventHandling.Interactable) {
                        EventHandling.Interactable interaction = (EventHandling.Interactable)gameObject;
                        // Check to see if the mouse is intersecting the GameObject.
                        if (graphics.contains(mouse.X, mouse.Y)) {
                            // MOUSE_OVER?
                            if (!mouseOvered.Contains(interaction)) {
                                mouseOvered.Add(interaction);
                                EventHandling.Event evvie = interaction.getEvent(MOUSE_OVER);
                                if (evvie != null) newEvents.Add(evvie);
                            }
                            // MOUSE_LEFT_CLICK?
                            if (mouse.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed && !mouseLeftClicked.Contains(interaction)) {
                                mouseLeftClicked.Add(interaction);
                                EventHandling.Event evvie = interaction.getEvent(MOUSE_LEFT_CLICK);
                                if (evvie != null) newEvents.Add(evvie);
                            }
                            // MOUSE_RIGHT_CLICK?
                            if (mouse.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed && !mouseRightClicked.Contains(interaction)) {
                                mouseRightClicked.Add(interaction);
                                EventHandling.Event evvie = interaction.getEvent(MOUSE_RIGHT_CLICK);
                                if (evvie != null) newEvents.Add(evvie);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Output stuff.
        /// </summary>
        public override void output() {
            this.spriteBatch.Begin();
            // Go through every GameObject and see if they have something to output
            foreach (Constructs.GameObjectable gameObject in Engine.getGameObjectList()){
                if (gameObject is Graphable)
                    ((Graphable)gameObject).getGraphics().onDraw();
            }
            this.spriteBatch.End();
        }

        /// <summary>
        /// Set the window size.
        /// </summary>
        /// <param name="width">The width of the window.</param>
        /// <param name="height">The height of the window</param>
        public void setWindowSize(int width, int height) {
            Engine.game.setWindowSize(width, height);
        }

        /// <summary>
        /// Initialize stuff.
        /// </summary>
        public override void initialize() {
            Engine.game.setBackgroundColor(Microsoft.Xna.Framework.Graphics.Color.White);
            this.spriteBatch = new Microsoft.Xna.Framework.Graphics.SpriteBatch(InteractionEngine.Engine.game.GraphicsDevice);
        }

    }

    /**
     * Contains all graphics information and methods neccessary to drawing a GameObject in a 2D environment.
     * Could effectively be called a "Sprite" class.
     */
    public abstract class Graphics2D : Graphics {

        // Contains a reference to this Graphics module's GameObject.
        // Used for proper Updatable construction.
        public readonly Constructs.GameObject gameObject;
        // Contains the Updatable X position of this object.
        // Used for updating position vector information.
        public Constructs.Datatypes.UpdatableDouble xPos;
        // Contains the Updatable Y position of this object.
        // Used for updating position vector information.
        public Constructs.Datatypes.UpdatableDouble yPos;
        // Contains the rotational angle of this sprite
        // Used for calculating corners to know if a point is within boundaries.
        private Constructs.Datatypes.UpdatableDouble rotation;
        // Contains texture information for the sprite.
        // Used for drawing the sprite.
        private Microsoft.Xna.Framework.Graphics.Texture2D texture;
        // Contains the bounds of this sprite.
        // Used for figuring out if a mouse is pointing at the sprite.
        // [0]: upper left corner, [1]: upper right corner, [2]: lower left corner, [3]: lower right corner
        // [x][0]: x-coordinate, [x][1]: y-coordinate
        public Vector3[] corners;
        // Contains the width of this GameObject.
        // Used primarily for calculating Interactables.
        public Constructs.Datatypes.UpdatableDouble width;
        // Contains the height of this GameObject.
        // Used primarily for calculating Interactables.
        public Constructs.Datatypes.UpdatableDouble height;


        /// <summary>
        /// Loads the sprite from XNA's ContentPipeline.
        /// </summary>
        /// <param name="textureFileName">The filename of this GameObject's texture.</param>
        public Graphics2D(Constructs.GameObject gameObject) {
            this.gameObject = gameObject;
            xPos = new InteractionEngine.Constructs.Datatypes.UpdatableDouble(gameObject);
            xPos.value = 0;
            yPos = new InteractionEngine.Constructs.Datatypes.UpdatableDouble(gameObject);
            yPos.value = 0;
            rotation = new InteractionEngine.Constructs.Datatypes.UpdatableDouble(gameObject);
            rotation.value = 0;
            width = new InteractionEngine.Constructs.Datatypes.UpdatableDouble(gameObject);
            width.value = 0;
            height = new InteractionEngine.Constructs.Datatypes.UpdatableDouble(gameObject);
            height.value = 0;
            loadBounds();
        }


        /// <summary>
        /// Reload the bounds after changing the texture or the position.
        /// </summary>
        public void loadBounds() {
            corners = new Vector3[]{ new Vector3((float) xPos.value, (float) yPos.value, 0f),
                                     new Vector3((float) xPos.value, (float) (yPos.value + height.value), 0f),
                                     new Vector3((float) (xPos.value + width.value), (float) yPos.value, 0f),
                                     new Vector3((float) (xPos.value + width.value), (float) (yPos.value + height.value), 0f) };
        }

        /// <summary>
        /// Returns true if this sprite has a texture.
        /// </summary>
        /// <returns>True if this sprite has a texture.</returns>
        public bool hasTexture() {
            if (texture != null) return true;
            return false;
        }

        /// <summary>
        /// Load the texture. Must be done after calling InteractionGame.Run().
        /// </summary>
        /// <param name="textureFileName">The file name of the texture to load.</param>
        public void loadTexture(string textureFileName) {
            texture = InteractionEngine.Engine.game.Content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>(textureFileName);
        }

        /// <summary>
        /// Calculate the bounds from the height and width of the texture.
        /// </summary>
        public void calculateBoundsFromTexture() {
            if (!hasTexture()) return;
            height.value = texture.Height;
            width.value = texture.Width;
            loadBounds();
        }

        /// <summary>
        /// Change the position of this GameObject in the 2D environment.
        /// </summary>
        /// <param name="x">The x-coordinate of the GameObject's new position.</param>
        /// <param name="y">The y-coordinate of the GameObject's new position.</param>
        public void changePosition(double x, double y) {
            xPos.value = x;
            yPos.value = y;
            if (this.hasTexture()) this.loadBounds();
        }

        /// <summary>
        /// Apply the specified rotaton to the sprite.
        /// </summary>
        /// <param name="degrees">The angle, in degrees, to rotate.</param>
        public void applyRotation(float degrees) {
            this.rotation.value = (double)degrees;
        }

        /// <summary>
        /// Draw this Graphics2D onto the SpriteBatch.
        /// </summary>
        public virtual void onDraw() {
            ((UserInterface2D)InteractionEngine.Engine.userInterface).spriteBatch.Draw(texture,
                new Vector2((float)xPos.value, (float)yPos.value),
                null,
                Color.White,
                ((float)rotation.value) * (MathHelper.Pi / 180.0f),
                new Vector2(0f, 0f),
                1f,
                new Microsoft.Xna.Framework.Graphics.SpriteEffects(),
                0);
        }

        public void addExtremities() {
            /*foreach (Vector3 vector in corners) {
                test test = new test(this.gameObject);
                test.loadTexture("pixelcheck");
                test.changePosition((double)vector.X, (double)vector.Y);
                test.applyRotation((float)this.rotation.value);
                test.onDraw();
            }*/
        }

        /// <summary>
        /// Returns true if a point is contained within this Graphics2D.
        /// </summary>
        /// <param name="x">The x-coordinate of the point.</param>
        /// <param name="y">The y-coordinate of the point.</param>
        /// <returns>True if the point is within the Graphics2D's boundaries; false otherwise.</returns>
        public bool contains(double x, double y) {
            Matrix matrix = Matrix.CreateRotationZ((-1f) * (float)this.rotation.value * (MathHelper.Pi / 180.0f));
            Vector3 vector = new Vector3((float)x, (float)y, 0f);
            vector -= corners[0];
            vector = Vector3.Transform(vector, matrix);
            vector += corners[0];
            return (vector.X > corners[0].X && vector.X < corners[3].X && vector.Y > corners[0].Y && vector.Y < corners[3].Y);
        }

        /// <summary>
        /// Called during InteractionGame's LoadContent loop.
        /// </summary>
        public abstract void loadContent();

    }


}