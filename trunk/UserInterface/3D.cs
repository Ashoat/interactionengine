/*••••••••••••••••••••••••••••••••••••••••*\
| Interaction Engine                       |
| (C) Copyright Bluestone Coding 2009      |
|••••••••••••••••••••••••••••••••••••••••••|
|           __    ___ ___  ___             |
|          /++\  | _ ) __|/ __|            |
|          \++/  | _ \__ \ (__             |
|           \/   |___/___/\___|            |
|                                          |
|••••••••••••••••••••••••••••••••••••••••••|
| CLIENT                                   |
| * UserInterface3D                  Class |
| * Graphics3D              Abstract Class |
\*••••••••••••••••••••••••••••••••••••••••*/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using InteractionEngine.EventHandling;

namespace InteractionEngine.UserInterface.ThreeDimensional {

    /// <summary>
    /// Coolio.
    /// </summary>
    public class UserInterface3D : UserInterface {

        // Contains the SpriteBatch this UserInterface uses to draw its GameObjects.
        // Used for drawing GameObjects to the screen.
        public Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch;
        // Contain lists of Interactables that had been MOUSEMASK_OVER'd and MOUSEMASK_CLICK'd in the last iteration of input().
        // Used for knowing when to invoke MOUSEMASK_OUT and MOUSEMASK_RELEASE events.
        private System.Collections.Generic.Dictionary<Interactable, int> eventsAwaitingReset = new System.Collections.Generic.Dictionary<Interactable, int>();
        
        // This specifies the bits that represent unique positive mouse buttons or actions (right-click, mouse-over, etc)
        public const int MOUSEMASK_ACTION = (1 << 4) - 1;
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
        public const int MOUSEMASK_RIGHT_PRESS = 1 << 0;
        public const int MOUSEMASK_LEFT_PRESS = 1 << 1;
        public const int MOUSEMASK_OVER = 1 << 2;
        // Currently unused
        public const int MOUSEMASK_OTHER = 1 << 3;

        // Negative mouse actions!
        public const int MOUSEMASK_RIGHT_RELEASE = MOUSEMASK_RIGHT_PRESS + MOUSEMASK_RESET;
        public const int MOUSEMASK_LEFT_RELEASE = MOUSEMASK_LEFT_PRESS + MOUSEMASK_RESET;
        public const int MOUSEMASK_OUT = MOUSEMASK_OVER + MOUSEMASK_RESET;


        public static bool testMask(int val, int mask) {
            return (val & mask) == mask;
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

        /// <summary>
        /// Goes through old "held" events that need resetting (such as a "mouseOver" awaiting a "mouseOut")
        /// and checks to see if they are ready to reset, and if so, sends their corresponding release events
        /// and removes from the list of held events.
        /// </summary>
        /// <param name="newEvents">The list into which newly detected events are to be inserted.</param>
        private void resetEvents(System.Collections.Generic.List<EventHandling.Event> newEvents, Ray ray) {
            // Check states of mouse input
            Microsoft.Xna.Framework.Input.MouseState mouse = Microsoft.Xna.Framework.Input.Mouse.GetState();
            bool leftReleased = mouse.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released;
            bool rightReleased = mouse.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Released;
            // Loop through list and test to see if any are ready to receive a RELEASE or OUT event
            System.Collections.Generic.LinkedList<Interactable> removals = new System.Collections.Generic.LinkedList<Interactable>();
            foreach (Interactable interactable in eventsAwaitingReset.Keys) {
                int eveCode = eventsAwaitingReset[interactable];
                if (testMask(eveCode, MOUSEMASK_LEFT_PRESS)) {
                    if (leftReleased) {
                        EventHandling.Event evvie = interactable.getEvent(MOUSEMASK_LEFT_RELEASE);
                        newEvents.Add(evvie);
                        eventsAwaitingReset[interactable] = unsetMask(eveCode, MOUSEMASK_LEFT_PRESS);
                    }
                }
                if (testMask(eveCode, MOUSEMASK_RIGHT_PRESS)) {
                    if (rightReleased) {
                        EventHandling.Event evvie = interactable.getEvent(MOUSEMASK_RIGHT_RELEASE);
                        newEvents.Add(evvie);
                        eventsAwaitingReset[interactable] = unsetMask(eveCode, MOUSEMASK_RIGHT_PRESS);
                    }
                }
                if (testMask(eveCode, MOUSEMASK_OVER)) {
                    Graphics3D graphics = (Graphics3D)interactable.getGraphics();
                    if (graphics.intersects(ray) == -1) {
                        EventHandling.Event evvie = interactable.getEvent(MOUSEMASK_OUT);
                        newEvents.Add(evvie);
                        eventsAwaitingReset[interactable] = unsetMask(eveCode, MOUSEMASK_OVER);
                    }
                }
                if ((eventsAwaitingReset[interactable] & MOUSEMASK_ACTION) == 0) removals.AddLast(interactable);
            }
            // Remove ones that no longer have any mouse events to resetS
            foreach (Interactable removal in removals) {
                eventsAwaitingReset.Remove(removal);
            }

        }

        private void checkInteractableForInteraction(System.Collections.Generic.List<EventHandling.Event> newEvents, Ray ray, Interactable interaction) {
            Microsoft.Xna.Framework.Input.MouseState mouse = Microsoft.Xna.Framework.Input.Mouse.GetState();
            Graphics3D graphics = (Graphics3D)interaction.getGraphics();
            // Check to see if the mouse is intersecting the GameObject.
            if (graphics.intersects(ray) >= 0) {
                int alreadyEvented = eventsAwaitingReset.ContainsKey(interaction) ? eventsAwaitingReset[interaction] : 0;
                // MOUSEMASK_OVER?
                if (!testMask(alreadyEvented, MOUSEMASK_OVER)) {
                    eventsAwaitingReset[interaction] = setMask(alreadyEvented, MOUSEMASK_OVER);
                    EventHandling.Event evvie = interaction.getEvent(MOUSEMASK_OVER);
                    if (evvie != null) newEvents.Add(evvie);
                }
                // MOUSEMASK_LEFT_CLICK?
                if (!testMask(alreadyEvented, MOUSEMASK_LEFT_PRESS) && mouse.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed) {
                    eventsAwaitingReset[interaction] = setMask(alreadyEvented, MOUSEMASK_LEFT_PRESS);
                    EventHandling.Event evvie = interaction.getEvent(MOUSEMASK_LEFT_PRESS);
                    if (evvie != null) newEvents.Add(evvie);
                }
                // MOUSEMASK_RIGHT_CLICK?
                if (!testMask(alreadyEvented, MOUSEMASK_LEFT_PRESS) && mouse.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed) {
                    eventsAwaitingReset[interaction] = setMask(alreadyEvented, MOUSEMASK_LEFT_PRESS);
                    EventHandling.Event evvie = interaction.getEvent(MOUSEMASK_LEFT_PRESS);
                    if (evvie != null) newEvents.Add(evvie);
                }
            }
        }

        /// <summary>
        /// Checks state of user input devices to see if an input event should be triggered.
        /// If so, collects the EventHandling.Event objects and inserts them into the given list.
        /// Make it quick!
        /// </summary>
        /// <param name="newEventList">The list into which newly detected events are to be inserted.</param>
        protected override void retrieveInput(System.Collections.Generic.List<EventHandling.Event> newEvents) {
            // Get mouse state
            Microsoft.Xna.Framework.Input.MouseState mouse = Microsoft.Xna.Framework.Input.Mouse.GetState();
            Ray ray = new Ray(); // TODO: PREETUM figure out how to get the useful ray here.
            // Reset old events
            resetEvents(newEvents, ray);
            // Loop through all of the User's LoadRegions
            foreach (InteractionEngine.Constructs.LoadRegion loadRegion in InteractionEngine.Engine.getLoadRegionList()) {
                // Loop through all the LoadRegion's GameObjects
                for (int i = 0; i < loadRegion.getObjectCount(); i++) {
                    Constructs.GameObjectable gameObject = InteractionEngine.Engine.getGameObject(i);
                    // See if this GameObject can be interacted with.
                    if (gameObject is Interactable) {
                        checkInteractableForInteraction(newEvents, ray, (Interactable)gameObject);
                    }
                }
            }
        }

        /// <summary>
        /// Output stuff.
        /// </summary>
        public override void output() {
            this.spriteBatch.Begin();
            // Loop through the user's LoadRegions
            foreach (Constructs.LoadRegion loadRegion in InteractionEngine.Engine.getLoadRegionList()) {
                // Loop through the GameObjects within those LoadRegions
                for (int i = 0; i < loadRegion.getObjectCount(); i++) {
                    Constructs.GameObjectable gameObject = InteractionEngine.Engine.getGameObject(i);
                    // Go through every GameObject and see if they have something to output
                    if (gameObject is Graphable)
                        ((Graphable)gameObject).getGraphics().onDraw();
                }
            }
            this.spriteBatch.End();
        }

        /// <summary>
        /// Initialize stuff.
        /// </summary>
        public override void initialize() {
            this.spriteBatch = new Microsoft.Xna.Framework.Graphics.SpriteBatch(InteractionEngine.Engine.game.GraphicsDevice);
        }

    }

    public abstract class Graphics3D : Graphics {

        // Contains a reference to this Graphics module's GameObject.
        // Used for proper Updatable construction.
        private Graphable gameObject;

        /// <summary>
        /// Constructs the Graphics3D
        /// </summary>
        /// <param name="textureFileName">The filename of this GameObject's texture.</param>
        public Graphics3D(Graphable gameObject) {
            this.gameObject = gameObject;
        }

        /// <summary>
        /// Draw this Graphics3D onto the screen
        /// </summary>
        public virtual void onDraw() {
            // This is where you draw stuff onto the screen!
            // Get the necessary information out of the ContentPipeline and by having a few fields if necessary
            // Make sure that all the fields are readonly. If you have to be able to change a field, make it an Updatable.
        }

        /// <summary>
        /// If the given ray intersects this GameObject, returns the distance at which it intersects.
        /// Otherwise, returns negative one.
        /// </summary>
        /// <param name="ray">The ray</param>
        /// <returns>The distance at which the ray intersects this GameObject, or -1 if it does not intersect.</returns>
        public int intersects(Ray ray) {
            // TODO
            return -1;
        }

        /// <summary>
        /// Called during InteractionGame's LoadContent loop.
        /// </summary>
        public virtual void loadContent() {
            // TODO
            // Also, remove "virtual" unless the plan is to subclass this.
        }

    }

    public class Camera : InteractionEngine.Constructs.GameObject, InteractionEngine.Constructs.Locatable {

        #region FACTORY

        // The classHash, a unique identifying string for the class. Hmm, wow, that's kind of redundant, isn't that? C# already provides such a function through reflection. Oh well.
        // Used for the factory methods called when the client receives a CREATE_NEW_OBJECT update from the server computer.
        internal const string classHash = "Camera";

        /// <summary>
        /// The static constructor. Adds the class's factory method to the GameObject factoryList when the class is first loaded.
        /// </summary>
        static Camera() {
            InteractionEngine.Constructs.GameObject.factoryList.Add(classHash, new InteractionEngine.Constructs.GameObjectFactory(makeCamera));
        }

        /// <summary>
        /// A factory method that creates and returns a new instance of Camera. Used by the client when the server requests it to make a new GameObject.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        /// <param name="id">This GameObject's ID.</param>
        /// <param name="reader">The PacketReader from which we will read the fields of the newly constructed GameObject.</param>
        /// <returns>A new instance of Camera.</returns>
        static Camera makeCamera(InteractionEngine.Constructs.LoadRegion loadRegion, int id) {
            if (InteractionEngine.Engine.status != InteractionEngine.Engine.Status.MULTIPLAYER_CLIENT)
                throw new System.Exception("You're not a client, so why are you calling the GameObject factory method?");
            Camera camera = new Camera(loadRegion, id);
            // ORDER OF STUFF (where you used the reader to construct datatypes, used factory methods exclusively. also, construct modules and their datatypes here too.)
            return camera;
        }

        /// <summary>
        /// Constructs a GameObject and assigns it an ID.
        /// This is the constructor that should be used if and only if you are a MULTIPLAYER_CLIENT.
        /// Furthermore, it is only called by the GameObjectFactory method.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        /// <param name="id">This GameObject's ID.</param>
        private Camera(InteractionEngine.Constructs.LoadRegion loadRegion, int id)
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
        /// Returns the Location module of this GameObject.
        /// </summary>
        /// <returns>The Location module associated with this GameObject.
        private InteractionEngine.Constructs.Location location;
        public InteractionEngine.Constructs.Location getLocation() {
            return location;
        }

        // You can't use a constructor anymore. Use a factory, like in GameObject.

        /// <summary>
        /// Constructs a new Camera.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        /*public Camera(Constructs.LoadRegion loadRegion)
            : base(loadRegion) {
            location = new Constructs.Location(this);
        }*/


        // TODO: do stuff


    }

    /*public class User3D : InteractionEngine.Server.User {

        public readonly Constructs.LoadRegion localLoadRegion;
        public readonly Camera camera;

        public User3D()
            : base() {
            this.localLoadRegion = new Constructs.LoadRegion();
            this.camera = new Camera(this.localLoadRegion);
            this.addLoadRegion(this.localLoadRegion);
            this.addPermission(this.camera);
        }


    }*/


}