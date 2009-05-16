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
| * Door                             Class |
\*••••••••••••••••••••••••••••••••••••••••*/

using WumpusGame.World.Modules;
using InteractionEngine.Constructs;
using System.Collections.Generic;
using InteractionEngine.Constructs.Datatypes;
using InteractionEngine.Client;
using InteractionEngine.GameWorld;
using WumpusGame.World.Graphics;

namespace WumpusGame.World {

    /**
     * The Door. It's really a tunnel in the cave system. When the user clicks on this, the door whisks the player away to another LoadRegion.
     */
    public class Door : GameObject, Graphable, Locatable, Interactable {

#region FACTORY

		// The classHash, a unique identifying string for the class. Hmm, wow, that's kind of redundant, isn't that? C# already provides such a function through reflection. Oh well.
		// Used for the factory methods called when the client receives a CREATE_NEW_OBJECT update from the server computer.
        internal const string classHash = "Door";

        /// <summary>
        /// The static constructor. Adds the class's factory method to the GameObject factoryList when the class is first loaded.
        /// </summary>
        static Door() {
            GameObject.factoryList.Add(classHash, new GameObjectFactory(makeDoor));
        }

        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        /// <param name="id">This GameObject's ID.</param>
        /// <param name="reader">The PacketReader from which we will read the fields of the newly constructed GameObject.</param>
        /// <returns>A new instance of Door.</returns>
        static Door makeDoor(LoadRegion loadRegion, int id, Microsoft.Xna.Framework.Net.PacketReader reader) {
            if (GameWorld.status != GameWorld.Status.MULTIPLAYER_CLIENT)
                throw new System.Exception("You're not a client, so why are you calling the GameObject factory method?");
            Door door = new Door(loadRegion, id);
            // ORDER OF STUFF (where you used the reader to construct datatypes, used factory methods exclusively. also, construct modules and their datatypes here too.)
            return door;
        }

        /// <summary>
        /// Constructs a GameObject and assigns it an ID.
        /// This is the constructor that should be used if and only if you are a MULTIPLAYER_CLIENT.
        /// Furthermore, it is only called by the GameObjectFactory method.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        /// <param name="id">This GameObject's ID.</param>
        private Door(LoadRegion loadRegion, int id) : base(loadRegion, id) {
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

        /// <summary>
        /// Returns the Location module of this GameObject.
        /// </summary>
        /// <returns>The Location module associated with this GameObject.
        private Location location;
        public Location getLocation() {
            return location;
        }
            
        /// <summary>
        /// Returns the Graphics module of this GameObject.
        /// </summary>
        /// <returns>The Graphics module associated with this GameObject.
        private DoorGraphics graphics;
        public InteractionEngine.Client.Graphics getGraphics() {
            return graphics;
        }

        /*••••••••••••••••••••••••••••••••••••••••*\
          MEMBERS
        \*••••••••••••••••••••••••••••••••••••••••*/

        // Contains a reference to the Room this Door points this.
        // Used for moving users to that Room.
        private readonly UpdatableGameObject<Room> target;
        // Contains an int signifying which direction this Door is facing.
        // Used for determing what graphics to output and what events to return.
        private readonly UpdatableInteger direction;

        /// <summary>
        /// Constructs a new Door object.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion that this door is in.</param>
        /// <param name="target">The room that this door leads to.</param>
        public Door(Room roomLocation, Room target, int direction) : base(roomLocation.loadRegion) {
            location = new Location(this);
            graphics = new Door2DGraphics(this, direction);
            this.target = new UpdatableGameObject<Room>(this);
            this.target.value = target;
            this.direction = new UpdatableInteger(this, direction);
        }

        /// <summary>
        /// Get an event. This method is called when an invoker invokes it.
        /// </summary>
        /// <param name="invoker">The caller of the event. Could be a mouse click, etc.</param>
        /// <returns>An event.</returns>
        public Event getEvent(int invoker) {
            Player param = (Player) GameWorld.user.getPermission(0); // Sort of hacky, but there was no alternative. Actually... there was, but it was even lamer.
            if (invoker == InteractionEngine.Client.Text.UserInterfaceText.TEXT_EVENT_CHOSEN || invoker == InteractionEngine.Client.TwoDimensional.UserInterface2D.MOUSE_LEFT_CLICK) {
                switch (direction.value) {
                    case Room.NORTH:
                        return new Event(this.id, "Move north", param);
                    case Room.NORTHEAST:
                        return new Event(this.id, "Move northeast", param);
                    case Room.NORTHWEST:
                        return new Event(this.id, "Move northwest", param);
                    case Room.SOUTH:
                        return new Event(this.id, "Move south", param);
                    case Room.SOUTHEAST:
                        return new Event(this.id, "Move southeast", param);
                    case Room.SOUTHWEST:
                        return new Event(this.id, "Move southwest", param);
                    default:
                        return new Event(this.id, "Move", param);
                }
            } else return null;
        }

        /// <summary>
        /// Move a player to the specified target.
        /// </summary>
        /// <param name="parameter"></param>
        public void movePlayerToTarget(object parameter) {
            Player player = (Player)parameter;
            if (player.isArrowSelected()) {
                if (target.value.contains<Wumpus>()) {
                    //graphics.win();
                } else {
                    player.getInventory().removeItem<Arrow>(1);
                }
            } else {
            
                // Literal move...
                player.getLocation().move(target.value);
                player.getPlayer().user.removeLoadRegion(this.loadRegion);
                player.getPlayer().user.addLoadRegion(this.target.value.loadRegion);
            }
        }

    }

}