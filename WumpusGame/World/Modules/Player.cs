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
| * Player                       Class     |
| * Playable                     Interface |
\*••••••••••••••••••••••••••••••••••••••••*/

using InteractionEngine.Constructs.Datatypes;
using InteractionEngine.Constructs;
using InteractionEngine.Server;
using InteractionEngine.GameWorld;
using InteractionEngine.Client;

namespace WumpusGame.World.Modules {

    /**
     * Holds all information and methods regarding player.
     */
    public class Player {

        /// <summary>
        /// The GameObject to which this module belongs.
        /// </summary>
        public GameObject gameObject;
        private readonly UpdatableGameObject<DialogBox> dialogBox;
        private readonly UpdatableInteger localLoadRegion;
        public User user; //TODO: update this.

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="gameObject">The GameObject whose module this is.</param>
        public Player(GameObject gameObject, User user) {
            this.user = user;
            dialogBox = new UpdatableGameObject<DialogBox>(gameObject);
            this.localLoadRegion = new UpdatableInteger(user.getLoadRegion(0));
            dialogBox.value = new DialogBox(this.getLocalLoadRegion());
            ((LoadRegion)GameWorld.getLoadRegion(localLoadRegion.value)).addObject(dialogBox.value.id);
            this.gameObject = gameObject;
        }

        /// <summary>
        /// Constructor for the client-side.
        /// </summary>
        /// <param name="gameObject">The GameObject whose module this is.</param>
        /// <param name="reader">The reader from which to read teh field datas.</param>
        public Player(GameObject gameObject, Microsoft.Xna.Framework.Net.PacketReader reader) {
            // OK, this system is bullshit. We should just have a list that predefines the order, and follow that.
            // We aren't using the client-side constructor anyways... we'll do this tommorow or something.

            /*byte transferCode = reader.ReadByte();
            UpdatableInteger intty = (UpdatableInteger)GameWorld.createField(reader);
            dialogBox = new UpdatableGameObject<DialogBox>(intty);
            if (reader.ReadByte() == GameWorld.UPDATE_FIELD) GameWorld.updateField(reader);

            else transferCode = reader.ReadByte();
            dialogBox = (UpdatableInteger) GameWorld.createField(reader);
            if (reader.ReadByte() == GameWorld.UPDATE_FIELD) GameWorld.updateField(reader);
            else reader.Position--;

            this.gameObject = gameObject;*/
        }

        /// <summary>
        /// Get the local LoadRegion associated with the Player.
        /// This is where Player-specific GameObjects go, such as inventory items and dialog boxes.
        /// </summary>
        /// <returns>This player's local LoadRegion.</returns>
        public LoadRegion getLocalLoadRegion() {
            return GameWorld.getLoadRegion(this.localLoadRegion.value);
        }

        /// <summary>
        /// Get the DialogBox associated with the Player.
        /// This is where we display the communications that are handled by the Conversation module.
        /// </summary>
        /// <returns></returns>
        public DialogBox getDialogBox() {
            return dialogBox.value;
        }

    }

    /**
     * Implemented by GameObjects that have the Player module.
     */
    public interface Playable : Locatable, Inventorable {

        /// <summary>
        /// Returns the Player module.
        /// </summary>
        /// <returns>The Player module.</returns>
        Player getPlayer();

    }

}