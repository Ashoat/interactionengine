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
| MODULE                                   |
| * Item                         Class     |
| * Itemable                     Interface |
\*••••••••••••••••••••••••••••••••••••••••*/

using InteractionEngine.Constructs.Datatypes;
using InteractionEngine.Constructs;
using InteractionEngine.GameWorld;
using InteractionEngine.Client;

namespace WumpusGame.World.Modules {

    /**
     * Holds all information and methods regarding items.
     */
    public class Item {

        // Contains a reference to the GameObject to which this module belongs.
        // Used for instantiating Updatables.
        public GameObject gameObject;
        // Contains a reference to the texture in the TextureList of this item.
        // Used for specifying the graphics representation of this item when it's displayed in an inventory.
        readonly UpdatableString inventoryIconTexture;
        // Contains the amount of the item if multiple items may collapse into one GameObject in the inventory. Zero if not collapsable.
        // Used for using screen space more efficiently, basically.
        private readonly UpdatableInteger count;
        // Contains a reference to an Event that is called by 
        public Event interaction;

        /// <summary>
        /// Construct and initialize the state of this module.
        /// </summary>
        /// <param name="gameObject">The GameObject to which this module belongs.</param>
        /// <param name="inventoryIcon">The icon to use to display this Item when it's in an inventory.</param>
        /// <param name="count">The number of in-game items represented by this single Item object, or zero if this Item object may only represent a single in-game item.</param>
        public Item(GameObject gameObject, string inventoryIcon, int count) {
            this.gameObject = gameObject;
            inventoryIconTexture = new UpdatableString(gameObject, inventoryIcon);
            this.count = new UpdatableInteger(gameObject, count);
        }

        /// <summary>
        /// Constructor for the client-side.
        /// </summary>
        /// <param name="gameObject">The GameObject whose module this is.</param>
        /// <param name="reader">The reader from which to read teh field datas.</param>
        public Item(GameObject gameObject, Microsoft.Xna.Framework.Net.PacketReader reader) {

            byte transferCode = reader.ReadByte();
            this.inventoryIconTexture = (UpdatableString)GameWorld.createField(reader);
            if (reader.ReadByte() == GameWorld.UPDATE_FIELD) GameWorld.updateField(reader);

            transferCode = reader.ReadByte();
            this.count = (UpdatableInteger)GameWorld.createField(reader);
            if (reader.ReadByte() == GameWorld.UPDATE_FIELD) GameWorld.updateField(reader);

            this.gameObject = gameObject;
        }

        /// <summary>
        /// This method should only be called by Inventory, which will dispose of this object after this method is called.
        /// If there are more than <i>amount</i> game items in this Item object, removes the specified amount of them and returns false.
        /// Else removes all of them and returns true.
        /// </summary>
        /// <returns>True if this item has been used up after this removal, false if some of the item still exists.</returns>
        internal bool remove(int amount) {
            if (amount <= 0) return false;
            if (count.value == 1 || count.value == 0 || count.value - amount <= 0) {
                count.value = -1;
                return true;
            }
            count.value = count.value - amount;
            return false;
        }

        /// <summary>
        /// This method should only be called by Inventory, which will dispose of the parameter after this method is called.
        /// Merges or "collapses" two items together if they are the same type of item.
        /// </summary>
        /// <param name="addition">Another item object of the same type that's just been added to the inventory.</param>
        internal void add(Itemable addition) {
            if (addition.GetType() != this.gameObject.GetType()) throw new System.Exception("You cannot use the Item.add() method with a parameter of different type than Item.");
            this.count.value = this.count.value + addition.getItem().count.value;
        }

        /// <summary>
        /// Returns true if this item is collapsable, ie. multiple instances of it can be stored in one Item object.
        /// </summary>
        /// <returns>True if collapsable, false otherwise.</returns>
        public bool isCollapsable() {
            return count.value > 0;
        }

        /// <summary>
        /// Gets the actual amount of items represented by this GameObject.
        /// </summary>
        /// <returns>The amount of this item left.</returns>
        public int getAmount() {
            return count.value != 0 ? count.value : 1;
        }

    }

    /**
     * Implemented by GameObjects that have the Item module.
     */
    public interface Itemable : GameObjectable{

        /// <summary>
        /// Returns the Item module.
        /// </summary>
        /// <returns>The Item module.</returns>
        Item getItem();

    }

}