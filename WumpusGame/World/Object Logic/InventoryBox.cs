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
| * InventoryBox                     Class |
| * InventoryIcon                    Class |
\*••••••••••••••••••••••••••••••••••••••••*/

using InteractionEngine.Constructs.Datatypes;
using InteractionEngine.Constructs;
using InteractionEngine.Client;
using InteractionEngine.GameWorld;
using WumpusGame.World.Graphics;
using WumpusGame.World.Modules;

namespace WumpusGame.World {
    /**
     * The InventoryBox object is used to display conversations between NPCs and PCs. It's loaded in the User's Personal LoadRegion (henceforce "UPLR"), where ketchup is served 24/7.
     * This should probably only be accessed by the Conversation module. If you want to display a dialog box, do it through a GameObject with a Conversation module.
     */
    public class InventoryBox : GameObject, Graphable, Interactable {

        #region FACTORY

        // The classHash, a unique identifying string for the class. Hmm, wow, that's kind of redundant, isn't that? C# already provides such a function through reflection. Oh well.
        // Used for the factory methods called when the client receives a CREATE_NEW_OBJECT update from the server computer.
        internal const string classHash = "InventoryBox";

        /// <summary>
        /// The static constructor. Adds the class's factory method to the GameObject factoryList when the class is first loaded.
        /// </summary>
        static InventoryBox() {
            GameObject.factoryList.Add(classHash, new GameObjectFactory(makeInventoryBox));
        }

        /// <summary>
        /// A factory method that creates and returns a new instance of InventoryBox. Used by the client when the server requests it to make a new GameObject.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        /// <param name="id">This GameObject's ID.</param>
        /// <param name="reader">The PacketReader from which we will read the fields of the newly constructed GameObject.</param>
        /// <returns>A new instance of InventoryBox.</returns>
        static InventoryBox makeInventoryBox(LoadRegion loadRegion, int id, Microsoft.Xna.Framework.Net.PacketReader reader) {
            if (GameWorld.status != GameWorld.Status.MULTIPLAYER_CLIENT)
                throw new System.Exception("You're not a client, so why are you calling the GameObject factory method?");
            InventoryBox dialogBox = new InventoryBox(loadRegion, id);
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
        private InventoryBox(LoadRegion loadRegion, int id)
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

        internal readonly UpdatableGameObject<Player> inventory;
        internal Door[] doorsSet = new Door[6];
        bool selected = false;

        /// <summary>
        /// Returns the Graphics module of this GameObject.
        /// </summary>
        /// <returns>The Graphics module associated with this GameObject.
        private InventoryBox2DGraphics graphics;
        public InteractionEngine.Client.Graphics getGraphics() {
            return graphics;
        }

        /// <summary>
        /// Constructs a new InventoryBox along with its buttons.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which the </param>
        public InventoryBox(LoadRegion loadRegion, Player inventory)
            : base(loadRegion) {
            this.inventory = new UpdatableGameObject<Player>(this, inventory);
            this.graphics = new InventoryBox2DGraphics(this, inventory.getInventory());
            this.addEvent("Button pressed", buttonPress);
        }

        public Event getEvent(int invoker) {
            if (invoker == InteractionEngine.Client.TwoDimensional.UserInterface2D.MOUSE_LEFT_CLICK)
                return new Event(this.id, "Button pressed", new object());
            return null;
        }


        public bool arrowSelected;

        public void buttonPress(object Object) {
	arrowSelected = !arrowSelected;
	if (inventory.value.getInventory().getItem<Arrow>() != null) arrowSelected = false;
}

    }

}