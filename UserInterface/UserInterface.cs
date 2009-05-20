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
| * Graphics                    Class      |
| * UserInterface               Interface  |
| * KeyboardFocus               GameObject |
| * Keyboardable                Interface  |
\*••••••••••••••••••••••••••••••••••••••••*/

// TODO: "inputEnabled?"

namespace InteractionEngine.UserInterface {

    /// <summary>
    /// Interface for the user interface. Can do anything it wants, as long as it can input and output from and to the user.
    /// </summary>

    public abstract class UserInterface {

        // Contains delay between iterations of the user input loop, in milliseconds.
        // Used for delaying the loop in runRetrieveInput() so that it doesn't hog CPU resources.
        protected int inputDelay = 10;

        // Contains a list of user input events triggered since the last call to this.input().
        // Used for safely shuttling events from the user input thread into the main GameWorld thread.
        // It would be inadvisable to modify this list without first obtaining its lock.
        private System.Collections.Generic.List<EventHandling.Event> eventList = new System.Collections.Generic.List<EventHandling.Event>();

        // Contains the thread that handles detection of user input events by running runRetrieveInput().
        // Used for concurrently collecting user input events while the main thread is still running.
        private System.Threading.Thread inputDetector;

        /// <summary>
        /// Constructor... sets up input-event-detecting thread.
        /// </summary>
        public UserInterface() {
            inputDetector = new System.Threading.Thread(new System.Threading.ThreadStart(runRetrieveInput));
            inputDetector.Start();
        }

        /// <summary>
        /// Continuous loop that calls the retrieveInput(System.Collections.Generic.List<EventHandling.Event>) method repeatedly,
        /// and stuffs the new events into the eventList, for the GameWorld to later fetch via the input() method.
        /// This method should run in its own thread, if that isn't clear already.
        /// </summary>
        private void runRetrieveInput() {
            System.Collections.Generic.List<EventHandling.Event> newEvents = new System.Collections.Generic.List<EventHandling.Event>();
            while (true) {
                newEvents.Clear();
                retrieveInput(newEvents);
                lock (eventList) {
                    eventList.AddRange(newEvents);
                }
                System.Threading.Thread.Sleep(inputDelay);
            }
        }

        /// <summary>
        /// Gets input from the user. Figures out what GameObject is being called, checks its Interaction module, and passes the Events on to the caller.
        /// Also, clears eventList of all events.
        /// </summary>
        /// <returns>The events that are being called. Each includes a GameObject ID and an event hash.</returns>
        public System.Collections.Generic.List<EventHandling.Event> input() {
            System.Collections.Generic.List<EventHandling.Event> eventExport;
            lock (eventList) {
                eventExport = new System.Collections.Generic.List<EventHandling.Event>(eventList);
                eventList.Clear();
            }
            return eventExport;
        }

        /// <summary>
        /// Checks state of user input devices to see if an input event should be triggered.
        /// If so, collects the EventHandling.Event objects and inserts them into the given list.
        /// Make it quick!
        /// </summary>
        /// <param name="newEventList">The list into which newly detected events are to be inserted.</param>
        protected abstract void retrieveInput(System.Collections.Generic.List<EventHandling.Event> newEventList);

        /// <summary>
        /// Output stuff into the user interface.
        /// </summary>
        public abstract void output();

        /// <summary>
        /// Initialize stuff.
        /// </summary>
        public virtual void initialize() {
            // Default: do nothing!
        }

    }

    /// <summary>
    /// The GameObject that receives keyboard input and passes it on to whichever GameObject has the focus.
    /// This belongs in the user's local LoadRegion.
    /// TODO: Add support for multiple focus-holders? Looks unpleasant since it'll involve an UpdatableList or something.
    /// </summary>
    public class KeyboardFocus : InteractionEngine.Constructs.GameObject {

        #region FACTORY

        /// <summary>
        /// All GameObjects need a parameterless constructor for calling by GameObject.createGameObject() and GameObject.createFromUpdate().
        /// NEVER CALL THIS! This constructor is exclusively for use by the InteractionEngine. If anyone else calls it things will break.
        /// If you want to construct this object, use GameObject.createGameObject(LoadRegion).
        /// </summary>
        public KeyboardFocus() {
        }

        // The classHash, a unique identifying string for the class. Hmm, wow, that's kind of redundant, isn't that? C# already provides such a function through reflection. Oh well.
        // Used for the factory methods called when the client receives a CREATE_NEW_OBJECT update from the server computer.
        public const string realHash = "KeyboardFocus";
        public override string classHash {
            get { return realHash; }
        }

        /// <summary>
        /// The static constructor. Adds the class's factory method to the GameObject factoryList when the class is first loaded.
        /// </summary>
        static KeyboardFocus() {
            Constructs.GameObject.factoryList.Add(realHash, new InteractionEngine.Constructs.GameObjectFactory(Constructs.GameObject.createFromUpdate<KeyboardFocus>));
        }

        #endregion

        /// <summary>
        /// This method creates all of this GameObject's fields in constant order. 
        /// Instantiate modules and their fields here too.
        /// Pretty much the constructor. It'll be called every time this object is instantiated.
        /// </summary>
        public override void construct() {
        }

        private const string EVENT_HASH = "Key";
        // Contains the delay before the first key press events if the key is held down, in milliseconds.
        // Used for sending multiple key press events if the key is held down.
        private int repeatDelay = 600;
        // Contains the delay between key press events if the key is held down, in milliseconds.
        // Used for sending multiple key press events if the key is held down.
        private int repeatRate = 200;
        // Contains the GameObject that currently holds the keyboard focus.
        // Used for knowing where to send keyboard events.
        private Constructs.Datatypes.UpdatableGameObject<Keyboardable> focusHolder;

        // You can't use the following method. You have to use a factory like in GameObject.

        /// <summary>
        /// Constructs a new KeyboardFocus object.
        /// Should be assigned to a user's local LoadRegion.
        /// </summary>
        /// <param name="loadRegion">The user's local LoadRegion to which this GameObject belongs.</param>
        /*public KeyboardFocus(InteractionEngine.Constructs.LoadRegion loadRegion) {
            InteractionEngine.Constructs.GameObject.createGameObject<KeyboardFocus>(loadRegion);
            focusHolder = new InteractionEngine.Constructs.Datatypes.UpdatableGameObject<Keyboardable>(this);
            this.addEvent(EVENT_HASH, new EventHandlingEventMethod(keyPressed));
        }*/

        /// <summary>
        /// Processes the event where a key press is received.
        /// By dispatching it to the focus-holder, basically.
        /// </summary>
        /// <param name="keyParam">The Microsoft.Xna.Framework.Input.Keys key pressed.</param>
        public void keyPressed(InteractionEngine.Networking.Client client, object keyParam) {
            Microsoft.Xna.Framework.Input.Keys key = (Microsoft.Xna.Framework.Input.Keys)keyParam;
            focusHolder.value.keyPressed(key);
        }

        /// <summary>
        /// Yields the keyboard focus to the specified Keyboardable object.
        /// </summary>
        /// <param name="newFocusHolder">The new holder of the keyboard focus.</param>
        public void getFocus(Keyboardable newFocusHolder) {
            focusHolder.value.focusLost(newFocusHolder);
            this.focusHolder.value = newFocusHolder;
        }

        /// <summary>
        /// Gets an event from the eventHashlist.
        /// </summary>
        /// <param name="invoker">The type of event (ie. left-click, right-click, etc).</param>
        /// <returns>The method that is called by the event.</returns>
        public EventHandling.Event getEvent(int invoker) {
            return new EventHandling.Event(this.id, EVENT_HASH, (object)invoker);
        }

    }

    /**
     * Implemented by GameObjects that can be receive keyboard input from the KeyboardFocus object.
     */
    public interface Keyboardable : InteractionEngine.Constructs.GameObjectable {

        /// <summary>
        /// Processes keys when they are pressed.
        /// </summary>
        void keyPressed(Microsoft.Xna.Framework.Input.Keys key);

        /// <summary>
        /// Processes the event where the keyboard focus is taken away by another Keyboardable object.
        /// </summary>
        void focusLost(Keyboardable newFocusHolder);

    }

}