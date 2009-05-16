/*••••••••••••••••••••••••••••••••••••••••*\
| Wumpus Game                              |
| (C) Copyright Bluestone Coding 2008      |
|••••••••••••••••••••••••••••••••••••••••••|
| Built using the Interaction Engine       |
| (C) Copyright Bluestone Coding 2008      |
|••••••••••••••••••••••••••••••••••••••••••|
|           __    ___ ___  ___             |
|          /++\  | _ ) __|/ __|            |
|          \++/  | _ \__ \ (__             |
|           \/   |___/___/\___|            |
|                                          |
|••••••••••••••••••••••••••••••••••••••••••|
| GAME OBJECTS                             |
| * DialogBox                        Class |
| * DialogButton                     Class |
\*••••••••••••••••••••••••••••••••••••••••*/

using InteractionEngine.Constructs.Datatypes;
using InteractionEngine.Constructs;
using InteractionEngine.Client;
using InteractionEngine.GameWorld;
using WumpusGame.World.Graphics;

namespace WumpusGame.World
{
    /**
     * The DialogBox object is used to display conversations between NPCs and PCs. It's loaded in the User's Personal LoadRegion (henceforce "UPLR"), where ketchup is served 24/7.
     * This should probably only be accessed by the Conversation module. If you want to display a dialog box, do it through a GameObject with a Conversation module.
     */
    public class DialogBox : GameObject, Graphable {

#region FACTORY

        // The classHash, a unique identifying string for the class. Hmm, wow, that's kind of redundant, isn't that? C# already provides such a function through reflection. Oh well.
        // Used for the factory methods called when the client receives a CREATE_NEW_OBJECT update from the server computer.
        internal const string classHash = "DialogBox";

        /// <summary>
        /// The static constructor. Adds the class's factory method to the GameObject factoryList when the class is first loaded.
        /// </summary>
        static DialogBox() {
            GameObject.factoryList.Add(classHash, new GameObjectFactory(makeDialogBox));
        }

        /// <summary>
        /// A factory method that creates and returns a new instance of DialogBox. Used by the client when the server requests it to make a new GameObject.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        /// <param name="id">This GameObject's ID.</param>
        /// <param name="reader">The PacketReader from which we will read the fields of the newly constructed GameObject.</param>
        /// <returns>A new instance of DialogBox.</returns>
        static DialogBox makeDialogBox(LoadRegion loadRegion, int id, Microsoft.Xna.Framework.Net.PacketReader reader) {
            if (GameWorld.status != GameWorld.Status.MULTIPLAYER_CLIENT)
                throw new System.Exception("You're not a client, so why are you calling the GameObject factory method?");
            DialogBox dialogBox = new DialogBox(loadRegion, id);
            // ORDER OF STUFF (where you used the reader to construct datatypes, used factory methods exclusively. also, construct modules and their datatypes here too.)
            return dialogBox;
        }

        /// <summary>
        /// Constructs a GameObject and assigns it an ID.
        /// This is the constructor that should be used if and only if you are a MULTIPLAYER_CLIENT.
        /// Furthermore, it is only called by the GameObjectFactory method.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        /// <param name="id">This GameObject's ID.</param>
        private DialogBox(LoadRegion loadRegion, int id) : base(loadRegion, id) {
        }
 
        /// <summary>
        /// Returns the class hash. 
        /// </summary>
        /// <returns>The class hash. Do we really have to tell you everything twice?</returns>
        public override string getClassHash() {
            return classHash;
        }

#endregion

        // The maximum number of buttons that this DialogBox supports.
        // Used for when stuff needs to know the maximum number of buttons that this DialogBox supports.
        // I'm having doubts about this style of documentation.
        public const int MAX_BUTTONS = 5;
        // The discussion from which this DialogBox is currently pulling DialogBox messages.
        // Used to pull messages.
        internal readonly UpdatableGameObject<Discussion> discussion;
        // The set of buttons that are held by this DialogBox.
        // Used to display to the user the options specified by the current Discussion.
        private readonly UpdatableArray buttons;

        /// <summary>
        /// Returns the Graphics module of this GameObject.
        /// </summary>
        /// <returns>The Graphics module associated with this GameObject.
        private DialogBox2DGraphics graphics;
        public InteractionEngine.Client.Graphics getGraphics() {
            return graphics;
        }

        /// <summary>
        /// Constructs a new DialogBox along with its buttons.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which the </param>
        public DialogBox(LoadRegion loadRegion) : base(loadRegion) {
            this.discussion = new UpdatableGameObject<Discussion>(this);
            UpdatableInteger[] buttonIds = new UpdatableInteger[MAX_BUTTONS];
            for (int i = 0; i < buttonIds.Length; i++) {
                buttonIds[i] = new UpdatableInteger(this.loadRegion, new DialogButton(loadRegion, this, (short) i).id);
            }
            this.graphics = new DialogBox2DGraphics(this);
            buttons = new UpdatableArray(this.loadRegion, buttonIds);
        }

        /// <summary>
        /// Starts displaying the messages specified by the given Discussion.
        /// </summary>
        /// <param name="discussion">The Discussion to display.</param>
        public void startConversation(Discussion discussion) {
            this.discussion.value = discussion;
            discussion.startDiscussion();
            updateButtons();
        }

        private void updateButtons() {
            string[] options = discussion.value.getOptions();
            if (options == null || options.Length == 0) {
                options = new string[] { "Okay" };
            }
            for (int i = 0; i < buttons.getLength(); i++) {
                DialogButton button = (DialogButton) GameWorld.getObject( ((UpdatableInteger) buttons.get(i)).value);
                if (i < options.Length) button.setName(options[i]);
                else button.setName(null);
            }
        }

        private void killButtons() {
            for (int i = 0; i < buttons.getLength(); i++) {
                DialogButton button = (DialogButton)GameWorld.getObject(((UpdatableInteger)buttons.get(i)).value);
                button.setName(null);
            }
        }

        /// <summary>
        /// Handles the event when one of this box's DialogButtons have been pressed.
        /// Advances the Discussion to the next message and then refreshes the DialogButtons.
        /// </summary>
        /// <param name="option">The option represented by the button that was pressed.</param>
        public void buttonPressed(string option) {
            GameObject gameObject = GameWorld.getObject(discussion.value.id);
            if(!(gameObject is Discussion)) return;
            discussion.value.advanceDiscussion(option);
            if (discussion.value.getDisplaying()) updateButtons();
            else killButtons();
        }

    }

    /**
     * A button representing an option for the dialog boxes that display conversations between NPCs and PCs. Cool, no?
     */
    public class DialogButton : GameObject, Graphable, Interactable {
        
#region FACTORY

        // The classHash, a unique identifying string for the class. Hmm, wow, that's kind of redundant, isn't that? C# already provides such a function through reflection. Oh well.
        // Used for the factory methods called when the client receives a CREATE_NEW_OBJECT update from the server computer.
        internal const string classHash = "DialogButton";

        /// <summary>
        /// The static constructor. Adds the class's factory method to the GameObject factoryList when the class is first loaded.
        /// </summary>
        static DialogButton() {
            GameObject.factoryList.Add(classHash, new GameObjectFactory(makeDialogButton));
        }

        /// <summary>
        /// A factory method that creates and returns a new instance of DialogButton. Used by the client when the server requests it to make a new GameObject.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        /// <param name="id">This GameObject's ID.</param>
        /// <param name="reader">The PacketReader from which we will read the fields of the newly constructed GameObject.</param>
        /// <returns>A new instance of DialogButton.</returns>
        static DialogButton makeDialogButton(LoadRegion loadRegion, int id, Microsoft.Xna.Framework.Net.PacketReader reader) {
            if (GameWorld.status != GameWorld.Status.MULTIPLAYER_CLIENT)
                throw new System.Exception("You're not a client, so why are you calling the GameObject factory method?");
            DialogButton dialogButton = new DialogButton(loadRegion, id);
            // ORDER OF STUFF (where you used the reader to construct datatypes, used factory methods exclusively. also, construct modules and their datatypes here too.)
            return dialogButton;
        }

        /// <summary>
        /// Constructs a GameObject and assigns it an ID.
        /// This is the constructor that should be used if and only if you are a MULTIPLAYER_CLIENT.
        /// Furthermore, it is only called by the GameObjectFactory method.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        /// <param name="id">This GameObject's ID.</param>
        private DialogButton(LoadRegion loadRegion, int id) : base(loadRegion, id) {
        }

        /// <summary>
        /// Returns the class hash. 
        /// </summary>
        /// <returns>The class hash. Do we really have to tell you everything twice?</returns>
        public override string getClassHash() {
            return classHash;
        }

#endregion

        // The DialogBox to which this DialogButton belongs.
        // Used to alert the DialogBox to when this button has been pressed.
        internal readonly UpdatableGameObject<DialogBox> dialogBox;
        // The value of this button. The option that it represents.
        // Used as a label displayed to the user.
        internal readonly UpdatableString name;

        /// <summary>
        /// Returns the Graphics module of this GameObject.
        /// </summary>
        /// <returns>The Graphics module associated with this GameObject.
        private DialogButton2DGraphics graphics;
        public InteractionEngine.Client.Graphics getGraphics() {
            return graphics;
        }

        public Event getEvent(int invoker) {
            if (name.value == null) return null;
            if (invoker == InteractionEngine.Client.TwoDimensional.UserInterface2D.MOUSE_LEFT_CLICK)
                return new Event(this.id, "Button click", new object());
            return null;
        }

        /// <summary>
        /// Constructs a new DialogButton. This bare constructor is intended for use in the client-side factory methods.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        public DialogButton(LoadRegion loadRegion, DialogBox dialogBox, short number) : base(loadRegion) {
            this.dialogBox = new UpdatableGameObject<DialogBox>(this, dialogBox);
            loadRegion.addObject(this.id); // add itself to local load region
            name = new UpdatableString(this);
            this.graphics = new Graphics.DialogButton2DGraphics(this, (int) number);
            this.addEvent("Button click", this.buttonClicked);
        }

        /// <summary>
        /// Set the Conversation option represented by this button.
        /// </summary>
        /// <param name="moniker">The option to display to the user.</param>
        internal void setName(string moniker) {
            name.value = moniker;
        }

        /// <summary>
        /// Handles the event when this button is clicked.
        /// </summary>
        /// <param name="player">The Player who interacted with this button.</param>
        public void buttonClicked(object parameter) {
            ((DialogBox) dialogBox.value).buttonPressed(name.value);
        }

    }
}