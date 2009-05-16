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
| * Inventory                    Class     |
| * Inventorable                 Interface |
\*••••••••••••••••••••••••••••••••••••••••*/

using InteractionEngine.Constructs.Datatypes;
using System.Collections.Generic;
using InteractionEngine.GameWorld;
using InteractionEngine.Constructs;

namespace WumpusGame.World.Modules {

    /**
     * Holds all information and methods regarding inventory.
     */
    public class Inventory {

        /// <summary>
        /// The GameObject to which this module belongs.
        /// </summary>
        public GameObject gameObject;

        private readonly UpdatableList items;
        private readonly UpdatableInteger selectedItem;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="gameObject">The GameObject whose Location this is.</param>
        public Inventory(GameObject gameObject) {
            items = new UpdatableList(gameObject);
            selectedItem = new UpdatableInteger(gameObject, -1);
            this.gameObject = gameObject;
        }

        /// <summary>
        /// Constructor for the client-side.
        /// </summary>
        /// <param name="gameObject">The GameObject whose module this is.</param>
        /// <param name="reader">The reader from which to read teh field datas.</param>
        public Inventory(GameObject gameObject, Microsoft.Xna.Framework.Net.PacketReader reader) {
            byte transferCode = reader.ReadByte();
            this.items = (UpdatableList)GameWorld.createField(reader);
            if (reader.ReadByte() == GameWorld.UPDATE_FIELD) GameWorld.updateField(reader);

            this.selectedItem = (UpdatableInteger)GameWorld.createField(reader);
            if (reader.ReadByte() == GameWorld.UPDATE_FIELD) GameWorld.updateField(reader);
            else reader.Position--;

            this.gameObject = gameObject;
        }

        /// <summary>
        /// Appoints the item at the given index as the "selected" item.
        /// </summary>
        /// <param name="index">The index of the item to select.</param>
        public void selectItem(int index) {
            selectedItem.value = index;
        }

        /// <summary>
        /// Sets the selected item to null.
        /// </summary>
        public void unselectItem() {
            selectedItem.value = -1;
        }

        /// <summary>
        /// Get the item that is currently selected in this inventory, or null if none are selected.
        /// </summary>
        /// <returns>The item that is currently selected in this inventory, or null if none exists.</returns>
        public Itemable getSelectedItem() {
            if (selectedItem.value == -1) return null;
            return getItem(selectedItem.value);
        }

        /// <summary>
        /// Adds the given item to this inventory.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void addItem(Itemable item) {
            Itemable existing = getItemOfSameType(item);
            if (existing != null) {
                if (existing.getItem().isCollapsable() && item.getItem().isCollapsable()) {
                    existing.getItem().add(item);
                    return;
                }
            }
            items.add(new UpdatableInteger(gameObject, ((GameObject)item).id));
        }

        /// <summary>
        /// Gets an item of the same type as the input item if it exists in this inventory, or null if it doesn't.
        /// </summary>
        /// <param name="item">The item whose type we are looking for.</param>
        /// <returns>An item of the same type if it exists in this inventory, or null otherwise.</returns>
        private Itemable getItemOfSameType(Itemable item) {
            foreach (Updatable value in items.value) {
                Itemable another = (Itemable)GameWorld.getObject(((UpdatableInteger)value).value);
                if (item.GetType() == another.GetType()) return another;
            }
            return null;
        }

        /// <summary>
        /// Determines whether or not the given item is in this inventory.
        /// </summary>
        /// <param name="item">The item to look for.</param>
        /// <returns>True if the item is in this inventory, false otherwise.</returns>
        public bool haveItem(Itemable item) {
            return items.contains(((GameObject)item).id);
        }
        
        /// <summary>
        /// Searches for the given item in this inventory.
        /// </summary>
        /// <param name="item">The item to look for.</param>
        /// <returns>The index of the item in this inventory, or -1 if it was not found.</returns>
        public int indexOf(Itemable item) {
            return items.indexOf(((GameObject)item).id);
        }

        /// <summary>
        /// Searches the inventory for the specified item and removes the specified amount of them if it is found.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <param name="amount">The amount to remove.</param>
        public void removeItem(Itemable item, int amount) {
            int index = indexOf(item);
            if (index < 0) return;
            if (item.getItem().remove(amount)) items.remove(index);
        }

        /// <summary>
        /// Searches the inventory for an item of the specified type, and returns it if found.
        /// Otherwise returns null.
        /// </summary>
        /// <typeparam name="T">The type of the item to look for.</typeparam>
        /// <returns>An item of the specified type if it exists in this inventory, or null if it does not.</returns>
        public T getItem<T>() where T : Itemable {
            foreach (Updatable value in items.value) {
                Itemable item = (Itemable) GameWorld.getObject(((UpdatableInteger) value).value);
                if ( item is T ) return (T) item;
            }
            return default(T);
        }

        /// <summary>
        /// Searches the inventory for an item of the specified type, and returns the index of its first occurance if it is found.
        /// Otherwise returns -1.
        /// </summary>
        /// <typeparam name="T">The type of the item to look for.</typeparam>
        /// <returns>The index of the first occurance of an item of the specified type if it exists in this inventory, or -1 if it does not.</returns>
        public int indexOf<T>() where T : Itemable {
            int index = 0;
            foreach (Updatable value in items.value) {  // More efficient to use iterator.
                GameObject gameObject = GameWorld.getObject( ((UpdatableInteger) value).value );
                if (!(gameObject is Itemable)) continue;
                Item item = ((Itemable) GameWorld.getObject(((UpdatableInteger) value).value)).getItem();
                if (item is T) return index;
                index++;
            }
            return -1;
        }

        /// <summary>
        /// Searches the inventory for an item of the specified type, and removes the specified amount of them if it is found.
        /// </summary>
        /// <typeparam name="T">The type of the item to remove.</typeparam>
        /// <param name="amount">The amount of the item to remove.</param>
        public void removeItem<T>(int amount) where T : Itemable {
            T item = getItem<T>();
            if (item == null) return;
            if (item.getItem().remove(amount) && item is GameObject) items.remove(((GameObjectable) item).getID());
        }

        /// <summary>
        /// Get the item at the given index.
        /// </summary>
        /// <param name="index">The index of the desired item.</param>
        /// <returns>The item located at the given index in this inventory.</returns>
        public Itemable getItem(int index) {
            return (Itemable) GameWorld.getObject(((UpdatableInteger)(items.get(index))).value);
        }

        /// <summary>
        /// Returns the number of distinct Itemable objects in this inventory.
        /// </summary>
        /// <returns>The number of distinct Itemable objects in this inventory.</returns>
        public int getCount() {
            return items.getLength();
        }

    }

    /**
     * Implemented by GameObjects that have the Inventory module.
     */
    public interface Inventorable : GameObjectable {

        /// <summary>
        /// Returns the Inventory module of this GameObject.
        /// </summary>
        /// <returns>The Inventory module associated with this GameObject.
        Inventory getInventory();

    }

}