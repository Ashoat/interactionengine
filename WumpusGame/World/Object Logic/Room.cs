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
| * Room                             Class |
\*••••••••••••••••••••••••••••••••••••••••*/

using InteractionEngine.Constructs;
using InteractionEngine.GameWorld;
using WumpusGame.World.Modules;
using System.Collections.Generic;
using System;
using InteractionEngine.Constructs.Datatypes;
using WumpusGame.World.Graphics;

namespace WumpusGame.World {

    /**
     * An event for a GameObject entering a Room.
     */
    public delegate void RoomEnteredListener(Locatable enterer);

    /**
     * The room. It's basically a tile. The user is present in one room at a time.
     */

    public class Room : GameObject, InteractionEngine.Client.Graphable {

#region FACTORY

		// The classHash, a unique identifying string for the class. Hmm, wow, that's kind of redundant, isn't that? C# already provides such a function through reflection. Oh well.
		// Used for the factory methods called when the client receives a CREATE_NEW_OBJECT update from the server computer.
        internal const string classHash = "Room";

        /// <summary>
        /// The static constructor. Adds the class's factory method to the GameObject factoryList when the class is first loaded.
        /// </summary>
        static Room() {
            GameObject.factoryList.Add(classHash, new GameObjectFactory(makeRoom));
        }

		/// <summary>
		/// A factory method that creates and returns a new instance of Room. Used by the client when the server requests it to make a new GameObject.
		/// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        /// <param name="id">This GameObject's ID.</param>
        /// <param name="reader">The PacketReader from which we will read the fields of the newly constructed GameObject.</param>
        /// <returns>A new instance of Room.</returns>
        static Room makeRoom(LoadRegion loadRegion, int id, Microsoft.Xna.Framework.Net.PacketReader reader) {
            if (GameWorld.status != GameWorld.Status.MULTIPLAYER_CLIENT)
                throw new System.Exception("You're not a client, so why are you calling the GameObject factory method?");
            Room room = new Room(loadRegion, id);
            // ORDER OF STUFF (where you used the reader to construct datatypes, used factory methods exclusively. also, construct modules and their datatypes here too.)
            return room;
        }

        /// <summary>
        /// Constructs a GameObject and assigns it an ID.
        /// This is the constructor that should be used if and only if you are a MULTIPLAYER_CLIENT.
        /// Furthermore, it is only called by the GameObjectFactory method.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        /// <param name="id">This GameObject's ID.</param>
        private Room(LoadRegion loadRegion, int id) : base(loadRegion, id) {
        }

        /// <summary>
        /// Returns the class hash. 
        /// </summary>
        /// <returns>The class hash. Do we really have to tell you everything twice?</returns>
        public override string getClassHash() {
            return classHash;
        }

#endregion
        
        static readonly Random randy = new Random();

        // A list of all the rooms, by their GameObject ids.
        public static List<Room> allRooms = new List<Room>();
        // The list of adjacent Rooms.
        // Because it is not Updatable, be sure that this field is identical on the server and client sides.
        public Room[] adjacentRooms = new Room[6];
        // Some directional constants.
        public const int NORTH = 0;
        public const int NORTHEAST = 1;
        public const int SOUTHEAST = 2;
        public const int SOUTH = 3;
        public const int SOUTHWEST = 4;
        public const int NORTHWEST = 5; // The best one.
        // Event
        public event RoomEnteredListener roomEntered;

        /*••••••••••••••••••••••••••••••••••••••••*\
          MODULES
        \*••••••••••••••••••••••••••••••••••••••••*/

        /// <summary>
        /// Returns the Graphics module of this GameObject.
        /// </summary>
        /// <returns>The Graphics module associated with this GameObject.
        private Room2DGraphics graphics;
        public InteractionEngine.Client.Graphics getGraphics() {
            return graphics;
        }

        /*••••••••••••••••••••••••••••••••••••••••*\
          MEMBERS
        \*••••••••••••••••••••••••••••••••••••••••*/

        /// <summary>
        /// Add this Room to the static list of all rooms.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        public Room(LoadRegion loadRegion) : base(loadRegion) {
            allRooms.Add(this);
            this.graphics = new Room2DGraphics(this);
        }

        /// <summary>
        /// This method is triggered when a player enters the Room.
        /// </summary>
        /// <param name="gameObject">The GameObject that enters the room.</param>
        /// <returns>True if object successfully enteres, false if fails.</returns>
        public void objectEnteredRoom(Locatable locatable) {
            if(roomEntered != null) roomEntered(locatable);
        }

        /// <summary>
        /// Get a random Room on the map, that is not the current one.
        /// </summary>
        /// <returns>The Room we got.</returns>
        public Room getRandomRoom() {
            Room randomRoom;
            do {
                Random random = new Random();
                randomRoom = allRooms[random.Next(allRooms.Count)];
            } while (randomRoom == this);
            return randomRoom;
        }

        /// <summary>
        /// Get one of the rooms that lie a specific distance away from this one.
        /// </summary>
        /// <param name="distance">The distance from this room.</param>
        /// <returns>A room that is the given distance away from this one.</returns>
        public Room getRandomRoomByDistance(int distance) {
            return getRoom(distance, randy.NextDouble() * 2 * Math.PI);
        }

        /// <summary>
        /// Get the room whose center is closest to the point specified.
        /// </summary>
        /// <param name="distance">The distance, in rooms, from this room.</param>
        /// <param name="direction">The clockwise direction, in radians, from this room, where 0 is north.</param>
        /// <returns>The room closest to the point given by the given distance and direction from this one.</returns>
        public Room getRoom(int distance, double direction) {
            // Change direction from radians to a scale of 0 to 6.
            direction = (direction / Math.PI * 3) % 6;
            Room currentRoom = this;
            while (distance > 0) {
                int quantized = (int)Math.Round(direction);
                // apply Law of Cosines
                int a = distance;
                int b = distance-1;
                int c = 1;
                double deltaDirection = Math.Acos(a*a +b*b - c*c / 2*a*b);
                if (quantized > direction) deltaDirection *= -1;
                direction = direction - deltaDirection;
                quantized = quantized % 6;
                currentRoom = currentRoom.adjacentRooms[quantized];  // meh, bug
                distance--;
            }
            return currentRoom;
        }
        
        /// <summary>
        /// Checks to see if an object of the specified type is in the room.
        /// </summary>
        /// <typeparam name="T">The type of object to look for.</typeparam>
        /// <returns>True if there is an object of the specified type in the room, false otherwise.</returns>
        public bool contains<T>() {
            for (int i = 0; i < loadRegion.getObjectCount(); i++) {
                GameObject gameObject = loadRegion.getObject(i);
                if (gameObject is T) return true;
            }
            return false;
        }

        /// <summary>
        /// Gets all nonempty adjacent Rooms.
        /// </summary>
        /// <returns>A list of all nonempty adjacent Rooms.</returns>
        public List<Room> getAdjacentRooms() {
            List<Room> list = new List<Room>();
            foreach (Room room in adjacentRooms) {
                if (room != null) list.Add(room);
            }
            return list;
        }

        /// <summary>
        /// Gets a random nonempty adjacent Room.
        /// </summary>
        /// <returns>Make a guess.</returns>
        public Room getRandomAdjacentRoom() {
            Random random = new Random();
            List<Room> rooms = getAdjacentRooms();
            return rooms[random.Next(rooms.Count)];
        }
    
    }

}