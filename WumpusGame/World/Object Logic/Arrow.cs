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
| * Arrow                            Class |
\*••••••••••••••••••••••••••••••••••••••••*/

using InteractionEngine.Constructs;
using WumpusGame.World.Modules;
using InteractionEngine.GameWorld;
using InteractionEngine.Client;
using WumpusGame.World.Graphics;

namespace WumpusGame.World {

    /// <summary>
    /// The Arrow item, representing... arrows. In the player's inventory.
    /// Used to shoot the Wumpus. Bought from RandomCaveHobos.
    /// </summary>
    public class Arrow : GameObject, Itemable, Graphable {

#region FACTORY

        // The classHash, a unique identifying string for the class. Hmm, wow, that's kind of redundant, isn't that? C# already provides such a function through reflection. Oh well.
        // Used for the factory methods called when the client receives a CREATE_NEW_OBJECT update from the server computer.
        internal const string classHash = "Arrow";

        /// <summary>
        /// The static constructor. Adds the class's factory method to the GameObject factoryList when the class is first loaded.
        /// </summary>
        static Arrow() {
            GameObject.factoryList.Add(classHash, new GameObjectFactory(makeArrow));
        }

        /// <summary>
        /// A factory method that creates and returns a new instance of Arrow. Used by the client when the server requests it to make a new GameObject.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        /// <param name="id">This GameObject's ID.</param>
        /// <param name="reader">The PacketReader from which we will read the fields of the newly constructed GameObject.</param>
        /// <returns>A new instance of Arrow.</returns>
        static Arrow makeArrow(LoadRegion loadRegion, int id, Microsoft.Xna.Framework.Net.PacketReader reader) {
            if (GameWorld.status != GameWorld.Status.MULTIPLAYER_CLIENT)
                throw new System.Exception("You're not a client, so why are you calling the GameObject factory method?");
            Arrow arrow = new Arrow(loadRegion, id);
            // ORDER OF STUFF (where you used the reader to construct datatypes, used factory methods exclusively. also, construct modules and their datatypes here too.)
            return arrow;
        }

        /// <summary>
        /// Constructs a GameObject and assigns it an ID.
        /// This is the constructor that should be used if and only if you are a MULTIPLAYER_CLIENT.
        /// Furthermore, it is only called by the GameObjectFactory method.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        /// <param name="id">This GameObject's ID.</param>
        private Arrow(LoadRegion loadRegion, int id) : base(loadRegion, id) {
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
        /// Returns the Item module of this GameObject.
        /// </summary>
        /// <returns>The Item module associated with this GameObject.
        private Item item;
        public Item getItem() {
            return item;
        }

        /// <summary>
        /// Returns the Graphics module of this GameObject.
        /// </summary>
        /// <returns>The Graphics module associated with this GameObject.
        private ArrowGraphics graphics;
        public InteractionEngine.Client.Graphics getGraphics() {
            return graphics;
        }

        /// <summary>
        /// Constructs a new Arrow object representing a single arrow.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        public Arrow(LoadRegion loadRegion) : base(loadRegion) {
            item = new Item(this, "Arrow", 1);
        }

        /// <summary>
        /// Shoot this arrow.
        /// </summary>
        /// <param name="direction">The direction that you want to shoot into. See Room for more information.</param>
        public void onSelectArrow(int direction){
            this.graphics.onArrowShoot(direction);
            //if(this.item.
        }

    }
}