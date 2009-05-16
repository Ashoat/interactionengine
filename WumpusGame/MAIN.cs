// blah blah comments go here.

using WumpusGame;
using WumpusGame.World;
using WumpusGame.World.Modules;
using InteractionEngine.Constructs;
using InteractionEngine.Server;
using InteractionEngine.GameWorld;
using System;
// todo:
// make gold work
// make trivia work
// make random cave hobos
// allow the wumpus to leave you alive
// add a method for going to a random room so both bat and wumpus have it
// get rid of all the manual eventHashlist modification, replace by making the UI handle that
// make a method for creating a world
// make 3D work
// make multiplayer work
namespace WumpusGame {

    public class main {

        public static void Main() {
            // Initialize the GameWorld.
            GameWorld.game = new InteractionGame();
            GameWorld.status = GameWorld.Status.SINGLE_PLAYER;
            GameWorld.userInterface = new InteractionEngine.Client.TwoDimensional.UserInterface2D();
            GameWorld.game.setWindowSize(1000, 1100);
            GameWorld.game.setBackgroundColor(Microsoft.Xna.Framework.Graphics.Color.AliceBlue);
            // Initialize the user and their personal LoadRegion.
            User user = new User();
            user.addLoadRegion(new LoadRegion());
            GameWorld.user = user;
            // Create the LoadRegions, as well as their Rooms and RCHWDs
            LoadRegion[] loadRegions = new LoadRegion[30];
            for (int i = 0; i < loadRegions.Length; i++) {
                loadRegions[i] = new LoadRegion();
            }
            Room[] rooms = new Room[30];
            for (int i = 0; i < rooms.Length; i++) {
                rooms[i] = new Room(loadRegions[i]);
                loadRegions[i].addObject(rooms[i].id);
                new RandomCaveHoboWitchDoctor(rooms[i]).getLocation().move(rooms[i]);
            }
            // Set adjacent rooms
            for (int i = 0; i < rooms.Length; i++) {
                int borderRoom = (i + 6) % 30;
                rooms[i].adjacentRooms[Room.SOUTH] = rooms[borderRoom];
                rooms[borderRoom].adjacentRooms[Room.NORTH] = rooms[i];
                borderRoom = (i % 2 == 0 || i % 6 == 5) ? (i + 1) % 30 : (i + 7) % 30;
                rooms[i].adjacentRooms[Room.SOUTHEAST] = rooms[borderRoom];
                rooms[borderRoom].adjacentRooms[Room.NORTHWEST] = rooms[i];
                borderRoom = (i % 2 == 1 || i % 6 == 0) ? (i + 5) % 30 : (i - 1) % 30;
                rooms[i].adjacentRooms[Room.SOUTHWEST] = rooms[borderRoom];
                rooms[borderRoom].adjacentRooms[Room.NORTHEAST] = rooms[i];
            }
            // Create the Player. Also, assign the User a local LoadRegion.
            WumpusGame.World.Player player = new WumpusGame.World.Player(loadRegions[0], user);
            user.addPermission(player);
            // Create Doors
            for (int i = 0; i < rooms.Length; i++) {
                if ((i / 6) % 2 == 0) {
                    if (i % 2 == 0) {
                        Door door = new Door(rooms[i], rooms[i].adjacentRooms[Room.NORTHWEST], Room.NORTHWEST);
                        door.getLocation().move(rooms[i]);
                        door.addEvent("Move northwest", door.movePlayerToTarget);
                        door = new Door(rooms[i].adjacentRooms[Room.NORTHWEST], rooms[i], Room.SOUTHEAST);
                        door.getLocation().move(rooms[i].adjacentRooms[Room.NORTHWEST]);
                        door.addEvent("Move southeast", door.movePlayerToTarget);
                        door = new Door(rooms[i], rooms[i].adjacentRooms[Room.NORTH], Room.NORTH);
                        door.getLocation().move(rooms[i]);
                        door.addEvent("Move north", door.movePlayerToTarget);
                        door = new Door(rooms[i].adjacentRooms[Room.NORTH], rooms[i], Room.SOUTH);
                        door.getLocation().move(rooms[i].adjacentRooms[Room.NORTH]);
                        door.addEvent("Move south", door.movePlayerToTarget);
                        door = new Door(rooms[i], rooms[i].adjacentRooms[Room.NORTHEAST], Room.NORTHEAST);
                        door.getLocation().move(rooms[i]);
                        door.addEvent("Move northeast", door.movePlayerToTarget);
                        door = new Door(rooms[i].adjacentRooms[Room.NORTHEAST], rooms[i], Room.SOUTHWEST);
                        door.getLocation().move(rooms[i].adjacentRooms[Room.NORTHEAST]);
                        door.addEvent("Move southwest", door.movePlayerToTarget);
                    } else {
                        Door door = new Door(rooms[i], rooms[i].adjacentRooms[Room.SOUTHWEST], Room.SOUTHWEST);
                        door.getLocation().move(rooms[i]);
                        door.addEvent("Move southwest", door.movePlayerToTarget);
                        door = new Door(rooms[i].adjacentRooms[Room.SOUTHWEST], rooms[i], Room.NORTHEAST);
                        door.getLocation().move(rooms[i].adjacentRooms[Room.SOUTHWEST]);
                        door.addEvent("Move northeast", door.movePlayerToTarget);
                        door = new Door(rooms[i], rooms[i].adjacentRooms[Room.SOUTH], Room.SOUTH);
                        door.getLocation().move(rooms[i]);
                        door.addEvent("Move south", door.movePlayerToTarget);
                        door = new Door(rooms[i].adjacentRooms[Room.SOUTH], rooms[i], Room.NORTH);
                        door.getLocation().move(rooms[i].adjacentRooms[Room.SOUTH]);
                        door.addEvent("Move north", door.movePlayerToTarget);
                        door = new Door(rooms[i], rooms[i].adjacentRooms[Room.SOUTHEAST], Room.SOUTHEAST);
                        door.getLocation().move(rooms[i]);
                        door.addEvent("Move southeast", door.movePlayerToTarget);
                        door = new Door(rooms[i].adjacentRooms[Room.SOUTHEAST], rooms[i], Room.NORTHWEST);
                        door.getLocation().move(rooms[i].adjacentRooms[Room.SOUTHEAST]);
                        door.addEvent("Move northwest", door.movePlayerToTarget);
                    }
                }
            }
            // Focus on the Player's current LoadRegion.
            user.addLoadRegion(loadRegions[0]);
            // Add some Pits, Bats, and a Wumpus around randomly.
            System.Random randy = new Random((int)(0xC01DBEEF - 0xBAB1E555));
            Pit pit = new Pit(rooms[6]);
            pit.getLocation().move(rooms[6]);
            rooms[6].roomEntered += new RoomEnteredListener(pit.objectEntersRoom);
            Pit pit2 = new Pit(rooms[25]);
            pit2.getLocation().move(rooms[25]);
            rooms[25].roomEntered += new RoomEnteredListener(pit2.objectEntersRoom);
            Bat bat = new Bat(rooms[2]);
            bat.getLocation().move(rooms[2]);
            rooms[2].roomEntered += new RoomEnteredListener(bat.objectEnteredRoom);
            Bat bat2 = new Bat(rooms[15]);
            bat2.getLocation().move(rooms[15]);
            rooms[15].roomEntered += new RoomEnteredListener(bat2.objectEnteredRoom);
            Wumpus wumpus = new Wumpus(rooms[9].loadRegion);
            wumpus.getLocation().move(rooms[9]);
            rooms[9].roomEntered += new RoomEnteredListener(wumpus.playerEntersRoom);
            // Make the game run.
            GameWorld.game.Run();
        }

    }

}