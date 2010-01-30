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
using Microsoft.Xna.Framework.Input;
using InteractionEngine.EventHandling;
using System.Collections.Generic;

namespace InteractionEngine.UserInterface.TwoDimensional {

    public class UserInterface2D : UserInterface {

        public static GraphicsDevice graphicsDevice;
        public SpriteBatch spriteBatch;

        // Contain lists of Interactables that had been MOUSEMASK_OVER'd and MOUSEMASK_CLICK'd in the last iteration of input().
        // Used for knowing when to invoke MOUSEMASK_OUT and MOUSEMASK_RELEASE events.
        private Interactable currentlyMousedOver;
        private MouseState? leftClickPosition;
        private MouseState? rightClickPosition;

        // This specifies the bits that represent unique positive mouse buttons or actions (right-click, mouse-over, etc)
        public const int MOUSEMASK_ACTION = (1 << 3) - 1;
        // This specifies the bit that represents a reversal of the positive mouse actions (right-release, mouse-out, etc)
        // It might also represent the release of a modifier key, although that probably won't be necessary.
        public const int MOUSEMASK_RESET = 1 << 4;

        // These next three bits represent modifier keys. If a key is pressed while a positive mouse action is
        // performed, the respective bit should be masked on top of the action mask.
        // These might possibly find some usage in the mouse-action-resetting portion of the code...
        // say if you wanted to detect when the CTRL key was pressed of released during a mouseover...
        // though I doubt it so I recommend we not implement that until we find it necessary.
        // However, if we do implement that, we would need to apply the RESET mask for key releases.
        public const int MOUSEMASK_CTRL = 1 << 5;
        public const int MOUSEMASK_ALT = 1 << 6;
        public const int MOUSEMASK_SHIFT = 1 << 7;

        // Positive mouse actions (except the first one, to be used when sending modifier-key-changed-events, if we want to do that)
        public const int MOUSEMASK_NULL = 0;
        public const int MOUSEMASK_RIGHT_CLICK = 1 << 0;
        public const int MOUSEMASK_LEFT_CLICK = 1 << 1;
        public const int MOUSEMASK_OVER = 1 << 2;
        // Currently unused
        public const int MOUSEMASK_DRAG_SELECT = 1 << 3;

        // Negative mouse actions!
        //    public const int MOUSEMASK_RIGHT_RELEASE = MOUSEMASK_RIGHT_PRESS + MOUSEMASK_RESET;
        //    public const int MOUSEMASK_LEFT_RELEASE = MOUSEMASK_LEFT_PRESS + MOUSEMASK_RESET;
        public const int MOUSEMASK_OUT = MOUSEMASK_OVER + MOUSEMASK_RESET;
        public const int MOUSEMASK_LEFT_DRAG = MOUSEMASK_DRAG_SELECT + MOUSEMASK_LEFT_CLICK;
        public const int MOUSEMASK_RIGHT_DRAG = MOUSEMASK_DRAG_SELECT + MOUSEMASK_RIGHT_CLICK;

        public static bool testMask(int val, int mask) {
            return (val & mask) != 0;
        }

        public static int toggleMask(int val, int mask) {
            return val ^ mask;
        }

        public static int unsetMask(int val, int mask) {
            return val & (~mask);
        }

        public static int setMask(int val, int mask) {
            return val | mask;
        }


        private class MouseMaskTest {
            public delegate bool IsActivated(MouseState mouse);
            int mask;
            IsActivated isActivated;
            public MouseMaskTest(int mask, IsActivated isActivated) {
                this.mask = mask;
                this.isActivated = isActivated;
            }
            public bool isStillMousedOver(Interactable interaction, Ray ray, MouseState mouse) {
                if (interaction is Interactable2D) return ((Interactable2D)interaction).getGraphics2D().intersectionPoint(mouse.X, mouse.Y).HasValue;
                return false;
            }
            public void testAndRetrievePositiveEvent(Dictionary<Interactable, int> eventsAwaitingReset, List<Event> newEvents, Interactable interaction, MouseState mouse, Ray ray, Vector3 point) {
                int alreadyEvented = eventsAwaitingReset.ContainsKey(interaction) ? eventsAwaitingReset[interaction] : 0;
                if (!testMask(alreadyEvented, mask) && isActivated(mouse)) {
                    eventsAwaitingReset[interaction] = setMask(alreadyEvented, mask);
                    Event evvie = interaction.getEvent(mask, point);
                    if (evvie != null) newEvents.Add(evvie);
                }
            }
            public void testAndRetrieveNegativeEvent(Dictionary<Interactable, int> removals, List<Event> newEvents, Interactable interaction, Ray ray, MouseState mouse, int alreadyEvented) {
                if (testMask(alreadyEvented, mask) && (!isActivated(mouse) || !isStillMousedOver(interaction, ray, mouse))) {
                    Event evvie = interaction.getEvent(setMask(mask, MOUSEMASK_RESET), Vector3.Zero);
                    if (evvie != null) newEvents.Add(evvie);
                    //if (!removals.ContainsKey(interaction)) removals.Add(interaction, unsetMask(alreadyEvented, mask));
                    removals[interaction] = unsetMask(alreadyEvented, mask);
                }
            }
        }

        private MouseMaskTest[] maskTests = new MouseMaskTest[] {
            new MouseMaskTest(MOUSEMASK_OVER, (MouseState mouse) => true),
            new MouseMaskTest(MOUSEMASK_LEFT_CLICK, (MouseState mouse) => mouse.LeftButton == ButtonState.Pressed),
            new MouseMaskTest(MOUSEMASK_RIGHT_CLICK, (MouseState mouse) => mouse.RightButton == ButtonState.Pressed),
        };

        #region Keyboard Stuff

        private KeyboardFocus kf;
        private const int repeatDelay = 20;
        private Dictionary<Microsoft.Xna.Framework.Input.Keys, double> repeatTimes = new Dictionary<Microsoft.Xna.Framework.Input.Keys, double>();
        public void registerKeyboardFocus(KeyboardFocus kf) {
            this.kf = kf;
        }
        private void checkKeyboard(System.Collections.Generic.List<InteractionEngine.EventHandling.Event> newEvents) {
            if (this.kf == null || !Engine.game.IsActive) return;
            Microsoft.Xna.Framework.Input.KeyboardState keyboard = Microsoft.Xna.Framework.Input.Keyboard.GetState();
            foreach (Microsoft.Xna.Framework.Input.Keys key in keyboard.GetPressedKeys()) {
                if (!repeatTimes.ContainsKey(key) || repeatTimes[key] < InteractionEngine.Engine.gameTime.TotalRealTime.TotalMilliseconds) {
                    // newEvents.Add(this.kf.getEvent((int)key));
                    kf.keyEvent(null, key, KeyEvent.KEY_TYPED);
                    repeatTimes[key] = InteractionEngine.Engine.gameTime.TotalRealTime.TotalMilliseconds + repeatDelay;
                }
            }
        }

        #endregion

        /// <summary>
        /// Checks state of user input devices to see if an input event should be triggered.
        /// If so, collects the EventHandling.EventHandling.Event objects and inserts them into the given list.
        /// Make it quick!
        /// </summary>
        /// <param name="newEventList">The list into which newly detected events are to be inserted.</param>
        protected override void retrieveInput(System.Collections.Generic.List<EventHandling.Event> newEvents) {

            if (!Engine.game.IsActive) return;

            // Get mouse state
            Microsoft.Xna.Framework.Input.MouseState mouse = Microsoft.Xna.Framework.Input.Mouse.GetState();
            Vector3 point;
            Interactable interaction = this.findClosestIntersectedInteractable(mouse, out point);
            if (interaction != this.currentlyMousedOver) {
                if (currentlyMousedOver != null) addIfNotNull(newEvents, this.currentlyMousedOver.getEvent(MOUSEMASK_OUT, point));
                if (interaction != null) addIfNotNull(newEvents, interaction.getEvent(MOUSEMASK_OVER, point));
                this.currentlyMousedOver = interaction;
            }
            if (interaction == null) return;
            if (mouse.LeftButton == ButtonState.Pressed) {
                if (this.leftClickPosition == null) this.leftClickPosition = mouse;
            } else {
                if (this.leftClickPosition != null) {
                    // If not a drag...
                    if (isDrag(leftClickPosition.Value, mouse)) {
                        addIfNotNull(newEvents, interaction.getEvent(MOUSEMASK_LEFT_CLICK, point));
                    } //else frustrumSelect(newEvents, this.calculateMouseRay(leftClickPosition.Value), ray, true);
                }
                this.leftClickPosition = null;
            }
            if (mouse.RightButton == ButtonState.Pressed) {
                if (this.rightClickPosition == null) this.rightClickPosition = mouse;
            } else {
                if (this.rightClickPosition != null) {
                    if (isDrag(rightClickPosition.Value, mouse)) {
                        addIfNotNull(newEvents, interaction.getEvent(MOUSEMASK_RIGHT_CLICK, point));
                    } //else frustrumSelect(newEvents, this.calculateMouseRay(rightClickPosition.Value), ray, false);
                }
                this.rightClickPosition = null;
            }
        }

        private static bool isDrag(MouseState current, MouseState previous) {
            return current.X == previous.X && current.Y == previous.Y;
        }

        private static void addIfNotNull(List<Event> newEvents, Event evvie) {
            if (evvie != null) newEvents.Add(evvie);
        }

        private Interactable findClosestIntersectedInteractable(MouseState mouse, out Vector3 intersectionPoint) {
            Interactable2D closest2DInteractable = null;
            float closest2DLayerDepth = float.PositiveInfinity;
            Vector3 intersection2D = Vector3.Zero;
            // Loop through all of the User's LoadRegions
            foreach (InteractionEngine.Constructs.LoadRegion loadRegion in InteractionEngine.Engine.getGraphableLoadRegions()) {
                // Loop through all the LoadRegion's GameObjects
                foreach (Constructs.GameObjectable gameObject in loadRegion.getGameObjectArray()) {
                    // See if this GameObject can be interacted with.
                    if (gameObject is Interactable2D) {
                        Interactable2D interaction = (Interactable2D)gameObject;
                        if (interaction.getGraphics2D().Visible && interaction.getGraphics2D().LayerDepth < closest2DLayerDepth) {
                            Vector3? pointClicked = interaction.getGraphics2D().intersectionPoint(mouse.X, mouse.Y);
                            if (pointClicked.HasValue) {
                                closest2DInteractable = interaction;
                                closest2DLayerDepth = interaction.getGraphics2D().LayerDepth;
                                intersection2D = pointClicked.Value;
                            }
                        }
                    }
                    // End checking gameObject
                }
            }
            if (closest2DInteractable != null) {
                intersectionPoint = intersection2D;
                return closest2DInteractable;
            }
            intersectionPoint = Vector3.Zero;
            return null;
        }

        public override List<InteractionEngine.EventHandling.Event> input() {
            // Get mouse events
            List<InteractionEngine.EventHandling.Event> events = base.input();
            // Get keyboard events
            List<InteractionEngine.EventHandling.Event> keyboardEvents = new List<InteractionEngine.EventHandling.Event>();
            checkKeyboard(keyboardEvents);
            events.AddRange(keyboardEvents);
            return events;
        }

        /// <summary>
        /// Output stuff.
        /// </summary>
        public override void output() {
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.BackToFront, SaveStateMode.SaveState);
            // Go through every GameObject and see if they have something to output
            foreach (Constructs.GameObjectable gameObject in Engine.getGameObjectArray()) {
                if (gameObject is Graphable2D)
                    ((Graphable)gameObject).getGraphics().onDraw();
            }
            spriteBatch.End();
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
            graphicsDevice = InteractionEngine.Engine.game.GraphicsDevice;
            spriteBatch = new SpriteBatch(graphicsDevice);
            base.initialize();
        }

    }

    /**
     * This interface is more of an annotation than anything else, but it's a crucial annotation.
     * This interface flags GameObjects out for event-checking.
     */
    public interface Interactable2D : Interactable, Graphable2D {
    }

    public interface Graphics2D : Graphics {
        Vector3? intersectionPoint(double x, double y);
        float LayerDepth { get; }
        bool Visible { get; set; }
    }

    public interface Graphable2D : Graphable {
        Graphics2D getGraphics2D();
    }

    /**
     * Contains all graphics information and methods neccessary to drawing a GameObject in a 2D environment.
     * Could effectively be called a "Sprite" class.
     */
    public class Graphics2DTexture : Graphics2D {

        // Contains a reference to this Graphics module's GameObject.
        // Used for proper Updatable construction.
        public readonly Graphable2D gameObject;
        // Contains texture information for the sprite.
        // Used for drawing the sprite.
        private Microsoft.Xna.Framework.Graphics.Texture2D texture;
        private string textureName;
        private Color[] pixels;
        private bool visible = true;
        private float scale = 1;
        private float layerDepth = 0;

        /// <summary>
        /// Loads the sprite from XNA's ContentPipeline.
        /// </summary>
        /// <param name="textureFileName">The filename of this GameObject's texture.</param>
        public Graphics2DTexture(Graphable2D gameObject, string textureName) {
            this.gameObject = gameObject;
            this.textureName = textureName;
            if (Engine.game.GraphicsDevice != null) this.loadContent();
        }

        public Graphics2DTexture(Graphable2D gameObject) {
            this.gameObject = gameObject;
        }

        public string TextureName {
            get { return textureName; }
            set {
                this.textureName = value;
                if (Engine.game.GraphicsDevice != null) this.loadContent();
            }
        }

        public bool Visible {
            get { return visible; }
            set { this.visible = value; }
        }

        public float Scale {
            get { return this.scale; }
            set { this.scale = value; }
        }

        /// <summary>
        /// One is back, zero is front.
        /// </summary>
        public float LayerDepth {
            get { return this.layerDepth; }
            set { this.layerDepth = value; }
        }

        /// <summary>
        /// Draw this Graphics2D onto the SpriteBatch.
        /// </summary>
        public virtual void onDraw() {
            if (this.texture == null || !this.visible) return;
            SpriteBatch spriteBatch = ((UserInterface2D)InteractionEngine.Engine.userInterface).spriteBatch;
            Vector3 position3 = this.gameObject.getLocation().Position;
            Vector2 position = new Vector2(position3.X, position3.Y);
            float rotationRadians = this.gameObject.getLocation().yaw;
            Vector2 origin = Vector2.Zero;
            spriteBatch.Draw(texture, position, (Rectangle?)null, Color.White, rotationRadians,
                origin, scale, SpriteEffects.None, layerDepth);
        }

        /// <summary>
        /// Returns true if a point is contained within this Graphics2D.
        /// </summary>
        /// <param name="x">The x-coordinate of the point.</param>
        /// <param name="y">The y-coordinate of the point.</param>
        /// <returns>True if the point is within the Graphics2D's boundaries; false otherwise.</returns>
        public Vector3? intersectionPoint(double x, double y) {
            if (this.texture == null || !this.visible) return null;
            Vector3 location3 = this.gameObject.getLocation().Position;
            Vector3 inversePosition = Vector3.Multiply(location3, -1);
            Matrix inverseRotation = Matrix.CreateRotationZ(-this.gameObject.getLocation().yaw * MathHelper.Pi / 180);
            Vector3 vector = new Vector3((float)x, (float)y, 0f);
            vector = Vector3.Transform(vector, inverseRotation) + inversePosition;
            if (vector.X < 0 || vector.Y < 0 || (vector.X >= this.texture.Width) || (vector.Y >= this.texture.Height)) return null;
            int index = (int)vector.X * this.texture.Height + (int)vector.Y;
            const int alphaLimit = 10;
            if (this.pixels[index].A < alphaLimit) return null;
            return vector;
        }

        /// <summary>
        /// Called during InteractionGame's LoadContent loop.
        /// </summary>
        public virtual void loadContent() {
            if (textureName == null || textureName.Length == 0) return;
            texture = InteractionEngine.Engine.game.Content.Load<Texture2D>(textureName);
            pixels = new Color[texture.Width * texture.Height];
            texture.GetData<Color>(pixels);
        }

    }

}