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
| MODULE                                   |
| * Conversation                 Class     |
| * Conversable                  Interface |
| * Discussion                   Class     |
\*••••••••••••••••••••••••••••••••••••••••*/

using WumpusGame.World.Modules;
using WumpusGame.World;
using InteractionEngine.Constructs;
using InteractionEngine.Constructs.Datatypes;
using InteractionEngine.GameWorld;
namespace WumpusGame.Modules {

    /**
     * The Conversation module. Umm... it's not really that useful, is it?
     * Well any GameObject that plans to open dialog boxes to display to the user should have one of these anyways.
     */
    public class Conversation {

        // The name of the resource to use when displaying this GameObject's face.
        // Used in the DialogBoxes used to display all Conversations originating from this GameObject.
        private readonly UpdatableString faceIcon;
        /// <summary>
        /// The GameObject to which this module belongs.
        /// </summary>
        public GameObject gameObject;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="gameObject">The GameObject whose module this is.</param>
        /// <param name="faceIcon">The graphics resource used to draw this thing's icon on the dialog box.</param>
        public Conversation(GameObject gameObject, string faceIcon) {
            this.faceIcon = new UpdatableString(gameObject, faceIcon);
            this.gameObject = gameObject;
        }

        /// <summary>
        /// Constructor for the client-side.
        /// </summary>
        /// <param name="gameObject">The GameObject whose module this is.</param>
        /// <param name="reader">The reader from which to read teh field datas.</param>
        public Conversation(GameObject gameObject, Microsoft.Xna.Framework.Net.PacketReader reader) {
            byte transferCode = reader.ReadByte();
            this.faceIcon = (UpdatableString)GameWorld.createField(reader);
            if (reader.ReadByte() == GameWorld.UPDATE_FIELD) GameWorld.updateField(reader);
            else reader.Position--;
            this.gameObject = gameObject;
        }
        

        /// <summary>
        /// Fetches the name of the resource used for the face icon in the dialog box when this object starts a Conversation.
        /// </summary>
        /// <returns>The identifier of the resource used for the face icon.</returns>
        public string getFaceIcon() {
            return faceIcon.value;
        }

        /// <summary>
        /// Starts a Conversation, consisting of a series of dialog boxes displayed to the player.
        /// </summary>
        /// <param name="player">The player that should be receiving the dialog boxes.</param>
        /// <param name="discussion">The content of the Conversation.</param>
        public void startConversation(Playable player, Discussion discussion) {
            player.getPlayer().getDialogBox().startConversation(discussion);
        }

    }

    /**
     * The interface for all GameObjects that have Conversation modules. Umm... yeah.
     */
    public interface Conversable : GameObjectable {

        /// <summary>
        /// Fetches the Conversation module of this GameObject.
        /// </summary>
        /// <returns>The Conversation module.</returns>
        Conversation getConversation();

    }

}

namespace WumpusGame.World {

    /**
     * Discussion controls the progress of a conversation--that is, a series of dialog boxes in the game.
     * Discussion should be subclassed, and the constructor of the subclass should set the non-Updatable fields.
     * Each array index in the non-Updatable fields represent a single dialog box of the conversation. 
     * Then advanceDiscussion should be overriden to control the conversation flow.
     * Hopefully someday we'll have a way to read stuff like this from file. Until then, good luck.
     * @author Help I'm Trapped In A Driver's License Factory Elaine Roberts
     */
    public abstract class Discussion : GameObject {

        // The array of all messages this Discussion could potentially display.
        // Usage is self-explanatory, but it involves a goat's tongue.
        protected string[] messages;
        // The array of all sets of options this Discussion could potentially present to the user.
        // Each item of each set in the array should be displayed as a button below the message, for the user to click.
        protected string[][] options;
        // The array of all booleans specifying whether or not to display the face of the Conversable GameObject when displaying the message.
        // Used when the DialogBox is displaying the message and needs to know whether or not to display the face of the Conversable GameObject. 
        protected bool[] usingFaces;
        // The index of current message specified by this Discussion, or a negative number if the Discussion is inactive.
        // This index is used on each of the three arrays specified above.
        protected readonly UpdatableInteger curMessage;

        /// <summary>
        /// Constructs a Discussion.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        public Discussion(LoadRegion loadRegion) : base(loadRegion) {
            curMessage = new UpdatableInteger(this, -1);
        }

        /// <summary>
        /// Constructs a Discussion.
        /// The client-side constructor.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        /// <param name="id">The id of this new GameObject.</param>
        public Discussion(LoadRegion loadRegion, int id) : base(loadRegion, id) {
        }

        /// <summary>
        /// Tells the DialogBox whether or not there is currently a message to display.
        /// </summary>
        /// <returns>True iff there's a message to display.</returns>
        public bool getDisplaying() {
            return curMessage.value >= 0;
        }

        /// <summary>
        /// Fetches the current message in the Conversation.
        /// </summary>
        /// <returns>The message to display in a DialogBox.</returns>
        public string getMessage() {
            return messages[curMessage.value];
        }

        /// <summary>
        /// Fetches the options to present to the player when displaying the current message,
        /// or null or an empty array if the only option should be "Okay."
        /// </summary>
        /// <returns>The options associated with the current message.</returns>
        public string[] getOptions() {
            return options[curMessage.value];
        }

        /// <summary>
        /// Tells whether or not the DialogBox should use the resource specified by Conversation.getFaceIcon()
        /// when displaying the current message.
        /// </summary>
        /// <returns>True if the face resource should be used, false otherwise.</returns>
        public bool getUsingFace() {
            return usingFaces[curMessage.value];
        }

        /// <summary>
        /// Stops the Conversation from displaying further dialog boxes.
        /// </summary>
        public void killDiscussion() {
            curMessage.value = -1;
        }

        /// <summary>
        /// Sets this Discussion to start specifying the first message in the Conversation.
        /// </summary>
        public virtual void startDiscussion() {
            curMessage.value = 0;
        }

        /// <summary>
        /// Sets this Discussion to start specifying the next message in the Conversation.
        /// Or to close if no more messages remain.
        /// </summary>
        /// <param name="option">The option chosen by the user.</param>
        public abstract void advanceDiscussion(string option);

    }

}