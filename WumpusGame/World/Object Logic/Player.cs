﻿/*••••••••••••••••••••••••••••••••••••••••*\
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
| WORLD                                    |
| * Player                           Class |
\*••••••••••••••••••••••••••••••••••••••••*/

using InteractionEngine.Constructs;
using InteractionEngine.GameWorld;
using InteractionEngine.Server;
using WumpusGame.World.Modules;
using WumpusGame.World.Graphics;

namespace WumpusGame.World {


    public class Player : GameObject, Playable {

        #region FACTORY

        // The classHash, a unique identifying string for the class. Hmm, wow, that's kind of redundant, isn't that? C# already provides such a function through reflection. Oh well.
        // Used for the factory methods called when the client receives a CREATE_NEW_OBJECT update from the server computer.
        internal const string classHash = "Player";

        /// <summary>
        /// The static constructor. Adds the class's factory method to the GameObject factoryList when the class is first loaded.
        /// </summary>
        static Player() {
            GameObject.factoryList.Add(classHash, new GameObjectFactory(makePlayer));
        }

        /// <summary>
        /// A factory method that creates and returns a new instance of Player. Used by the client when the server requests it to make a new GameObject.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        /// <param name="id">This GameObject's ID.</param>
        /// <param name="reader">The PacketReader from which we will read the fields of the newly constructed GameObject.</param>
        /// <returns>A new instance of Player.</returns>
        static Player makePlayer(LoadRegion loadRegion, int id, Microsoft.Xna.Framework.Net.PacketReader reader) {
            if (GameWorld.status != GameWorld.Status.MULTIPLAYER_CLIENT)
                throw new System.Exception("You're not a client, so why are you calling the GameObject factory method?");
            Player player = new Player(loadRegion, id);
            // ORDER OF STUFF (where you used the reader to construct datatypes, used factory methods exclusively. also, construct modules and their datatypes here too.)
            return player;
        }

        /// <summary>
        /// Constructs a GameObject and assigns it an ID.
        /// This is the constructor that should be used if and only if you are a MULTIPLAYER_CLIENT.
        /// Furthermore, it is only called by the GameObjectFactory method.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        /// <param name="id">This GameObject's ID.</param>
        private Player(LoadRegion loadRegion, int id)
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

        /*••••••••••••••••••••••••••••••••••••••••*\
          MODULES
        \*••••••••••••••••••••••••••••••••••••••••*/

        /**
         * Return the Location module.
         * @return Location The Location module.
         */
        private Location location;
        public Location getLocation() {
            return location;
        }

        /**
         * Return the Graphics module.
         * @return Graphics The Graphics module.
         */
//        private PlayerGraphics graphics;
//        public InteractionEngine.Client.Graphics getGraphics() {
//            return graphics;
//        }

        /**
         * Return the Inventory module.
         * @return Inventory The Inventory module.
         */
        private Inventory inventory;
        public Inventory getInventory() {
            return inventory;
        }

        /**
         * Return the Player module.
         * @return Player The Player module.
         */
        private Modules.Player player;
        public Modules.Player getPlayer() {
            return player;
        }

        public InventoryBox inventoryBox;
        public bool isArrowSelected() {
            return inventoryBox.arrowSelected;
        }

        /*••••••••••••••••••••••••••••••••••••••••*\
          MEMBERS
        \*••••••••••••••••••••••••••••••••••••••••*/

        /// <summary>
        /// Constructs the Player.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        public Player(LoadRegion loadRegion, User user) : base(loadRegion) {
            location = new Location(this);
            inventory = new Inventory(this);
            player = new Modules.Player(this, user);

            inventoryBox = new InventoryBox(player.getLocalLoadRegion(), this);
            player.getLocalLoadRegion().addObject(inventoryBox.id);
            // TODO: how do we pass User across the network?
        }

    }

}