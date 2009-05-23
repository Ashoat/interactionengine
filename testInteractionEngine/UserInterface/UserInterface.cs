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
| CLIENT                                   |
| * Graphics                    Class      |
| * UserInterface               Interface  |
| * KeyboardFocus               GameObject |
| * Keyboardable                Interface  |
\*••••••••••••••••••••••••••••••••••••••••*/

// TODO: "inputEnabled?"

namespace InteractionEngine.Client {

    /**
     * This class holds a GameObject ID and an event hash.
     */
    public class Event {

        static int nextId = 0;

        public int id = nextId++;

        // Contains a reference to a GameObject.
        // Used for knowing where to find the EventHashlist for this Event.
        public int gameObjectID;
        // Contains a string referencing this particular event.
        // Used for figuring out which method to use on the GameObject's EventHashlist.
        public string eventHash;
        // Contains extra information for the event.
        // Used when extra information needs to be passed with the event.
        public object parameter;

        /// <summary>
        /// Constructs the event.
        /// </summary>
        /// <param name="id">The ID of the GameObject this Event is associated with.</param>
        /// <param name="hash">The eventHash of this Event.</param>
        /// <param name="parameter">Any extra information that needs to be passed with this Event.</param>
        public Event(int gameObjectID, string hash, object parameter) {
            this.gameObjectID = gameObjectID;
            this.eventHash = hash;
            this.parameter = parameter;
            System.Console.WriteLine("Event ID " + id + " created.");
        }

    }

    /// <summary>
    /// Interface for the user interface. Can do anything it wants, as long as it can input and output from and to the user.
    /// </summary>

    public abstract class UserInterface {

        // Contains delay between iterations of the user input loop, in milliseconds.
        // Used for delaying the loop in runRetrieveInput() so that it doesn't hog CPU resources.
        protected int inputDelay = 100;

        // Contains a list of user input events triggered since the last call to this.input().
        // Used for safely shuttling events from the user input thread into the main GameWorld thread.
        // It would be inadvisable to modify this list without first obtaining its lock.
        private System.Collections.Generic.List<Event> eventList = new System.Collections.Generic.List<Event>();

        // Contains the thread that handles detection of user input events by running runRetrieveInput().
        // Used for concurrently collecting user input events while the main thread is still running.
        private System.Threading.Thread inputDetector;

        /// <summary>
        /// Constructor... sets up input-event-detecting thread.
        /// </summary>
        public UserInterface() {
        }

        /// <summary>
        /// Continuous loop that calls the retrieveInput(System.Collections.Generic.List<Event>) method repeatedly,
        /// and stuffs the new events into the eventList, for the GameWorld to later fetch via the input() method.
        /// This method should run in its own thread, if that isn't clear already.
        /// </summary>
        private void runRetrieveInput() {
            System.Collections.Generic.List<Event> newEvents = new System.Collections.Generic.List<Event>();
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
        public virtual System.Collections.Generic.List<Event> input() {
            System.Collections.Generic.List<Event> eventExport;
            lock (eventList) {
                eventExport = new System.Collections.Generic.List<Event>(eventList);
                eventList.Clear();
            }
            return eventExport;
        }

        /// <summary>
        /// Checks state of user input devices to see if an input event should be triggered.
        /// If so, collects the Event objects and inserts them into the given list.
        /// Make it quick!
        /// </summary>
        /// <param name="newEventList">The list into which newly detected events are to be inserted.</param>
        protected abstract void retrieveInput(System.Collections.Generic.List<Event> newEventList);

        /// <summary>
        /// Output stuff into the user interface.
        /// </summary>
        public abstract void output();

        /// <summary>
        /// Initialize stuff.
        /// </summary>
        public virtual void initialize() {
        }

        public virtual void start() {
            inputDetector = new System.Threading.Thread(new System.Threading.ThreadStart(runRetrieveInput));
            inputDetector.Start();
        }

    }



    /// <summary>
    /// The GameObject that receives keyboard input and passes it on to whichever GameObject has the focus.
    /// This belongs in the user's local LoadRegion.
    /// TODO: Add support for multiple focus-holders? Looks unpleasant since it'll involve an UpdatableList or something.
    /// </summary>
    public class KeyboardFocus : InteractionEngine.Constructs.GameObject {

        #region FACTORY

        // The classHash, a unique identifying string for the class. Hmm, wow, that's kind of redundant, isn't that? C# already provides such a function through reflection. Oh well.
        // Used for the factory methods called when the client receives a CREATE_NEW_OBJECT update from the server computer.
        internal const string classHash = "KeyboardFocus";

        /// <summary>
        /// The static constructor. Adds the class's factory method to the GameObject factoryList when the class is first loaded.
        /// </summary>
        static KeyboardFocus() {
            InteractionEngine.Constructs.GameObject.factoryList.Add(classHash, new InteractionEngine.Constructs.GameObjectFactory(makeKeyboardFocus));
        }

        /// <summary>
        /// A factory method that creates and returns a new instance of KeyboardFocus. Used by the client when the server requests it to make a new GameObject.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        /// <param name="id">This GameObject's ID.</param>
        /// <param name="reader">The PacketReader from which we will read the fields of the newly constructed GameObject.</param>
        /// <returns>A new instance of KeyboardFocus.</returns>
        static KeyboardFocus makeKeyboardFocus(InteractionEngine.Constructs.LoadRegion loadRegion, int id, Microsoft.Xna.Framework.Net.PacketReader reader) {
            if (GameWorld.GameWorld.status != GameWorld.GameWorld.Status.MULTIPLAYER_CLIENT)
                throw new System.Exception("You're not a client, so why are you calling the GameObject factory method?");
            KeyboardFocus keyboardFocus = new KeyboardFocus(loadRegion, id);
            // ORDER OF STUFF (where you used the reader to construct datatypes, used factory methods exclusively. also, construct modules and their datatypes here too.)
            return keyboardFocus;
        }

        /// <summary>
        /// Constructs a GameObject and assigns it an ID.
        /// This is the constructor that should be used if and only if you are a MULTIPLAYER_CLIENT.
        /// Furthermore, it is only called by the GameObjectFactory method.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        /// <param name="id">This GameObject's ID.</param>
        private KeyboardFocus(InteractionEngine.Constructs.LoadRegion loadRegion, int id)
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

        /// <summary>
        /// Constructs a new KeyboardFocus object.
        /// Should be assigned to a user's local LoadRegion.
        /// </summary>
        /// <param name="loadRegion">The user's local LoadRegion to which this GameObject belongs.</param>
        public KeyboardFocus(InteractionEngine.Constructs.LoadRegion loadRegion)
            : base(loadRegion) {
            focusHolder = new InteractionEngine.Constructs.Datatypes.UpdatableGameObject<Keyboardable>(this);
            this.addEvent(EVENT_HASH, new InteractionEngine.Constructs.EventMethod(keyPressed));
        }

        /// <summary>
        /// Processes the event where a key press is received.
        /// By dispatching it to the focus-holder, basically.
        /// </summary>
        /// <param name="keyParam">The Microsoft.Xna.Framework.Input.Keys key pressed.</param>
        public void keyPressed(object keyParam) {
            Microsoft.Xna.Framework.Input.Keys key = (Microsoft.Xna.Framework.Input.Keys)keyParam;
            focusHolder.value.keyPressed(key);
        }

        /// <summary>
        /// Yields the keyboard focus to the specified Keyboardable object.
        /// </summary>
        /// <param name="newFocusHolder">The new holder of the keyboard focus.</param>
        public void getFocus(Keyboardable newFocusHolder) {
            if (focusHolder.value != null) focusHolder.value.focusLost(newFocusHolder);
            this.focusHolder.value = newFocusHolder;
        }

        /// <summary>
        /// Gets an event from the eventHashlist.
        /// </summary>
        /// <param name="invoker">The type of event (ie. left-click, right-click, etc).</param>
        /// <returns>The method that is called by the event.</returns>
        public Event getEvent(int invoker) {
            return new Event(this.getID(), EVENT_HASH, (object)invoker);
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