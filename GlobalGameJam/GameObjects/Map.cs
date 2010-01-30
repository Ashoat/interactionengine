using InteractionEngine.Constructs;
using InteractionEngine.UserInterface;
using Microsoft.Xna.Framework.Input;
using GlobalGameJam.Graphics;
using System.Linq;
using InteractionEngine.UserInterface.TwoDimensional;
using System.Collections.Generic;
using InteractionEngine;
using System.IO;
using System;
using Microsoft.Xna.Framework;
using System.Threading;
namespace GlobalGameJam.GameObjects {

    public class Map : GameObject, Graphable2D {

        #region FACTORY

        /// <summary>
        /// All GameObjects need a parameterless constructor for calling by GameObject.createGameObject() and GameObject.createFromUpdate().
        /// NEVER CALL THIS! This constructor is exclusively for use by the InteractionEngine. If anyone else calls it things will break.
        /// If you want to construct this object, use GameObject.createGameObject(LoadRegion).
        /// </summary>
        public Map() {
        }

        // The classHash, a unique identifying string for the class. Hmm, wow, that's kind of redundant, isn't that? C# already provides such a function through reflection. Oh well.
        // Used for the factory methods called when the client receives a CREATE_NEW_OBJECT update from the server computer.
        public const string realHash = "Map";
        public override string classHash {
            get { return realHash; }
        }

        /// <summary>
        /// The static constructor. Adds the class's factory method to the GameObject factoryList when the class is first loaded.
        /// </summary>
        static Map() {
            GameObject.factoryList.Add(realHash, new GameObjectFactory(GameObject.createFromUpdate<Map>));
        }

        #endregion

        protected Graphics2DTexture graphics;
        public InteractionEngine.UserInterface.Graphics getGraphics() {
            return graphics;
        }

        public InteractionEngine.UserInterface.TwoDimensional.Graphics2D getGraphics2D() {
            return graphics;
        }

        protected Location location;
        public Location getLocation() {
            return location;
        }

        LoadRegion mapLoadRegion;
        public override void construct() {
            location = new Location(this);
            graphics = new Graphics2DTexture(this);
            graphics.LayerDepth = 1;
            graphics.TextureName = "floor2";
            location.Position = new Vector3(0, 88, 0);
            mapLoadRegion = this.getLoadRegion();
            this.addEventMethod("tick", new InteractionEngine.EventHandling.EventMethod(this.update));
            Engine.addEvent(new InteractionEngine.EventHandling.Event(this.id, "tick", null));
        }

        private int height;
        private int width;

        public int Height {
            get { return height; }
        }

        public int Width {
            get { return width; }
        }

        Entity[,] entityArray;

        Player player;
        List<Character> characterList;

        public void LoadMap(string mapFile)
        {
            characterList = new List<Character>();
            try
            {
                FileStream file = new FileStream(mapFile, FileMode.Open, FileAccess.Read);

                width = (byte)file.ReadByte();
                height = (byte)file.ReadByte();
                file.ReadByte();

                char[,] characters = new char[width, height];

                try
                {
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            byte[] buffer = new byte[4];
                            file.Read(buffer, 0, 4);

                            characters[x, y] = (char)buffer[0];  
                        }
                    }
                
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }
                file.Close();

                // Allocate the tile grid.
                entityArray = new Entity[Width, Height];

                // Loop over every tile position,
                for (int y = 0; y < Height; ++y)
                {
                    for (int x = 0; x < Width; ++x)
                    {
                        // to load each tile.
                        char tileType = characters[x,y];
                        entityArray[x, y] = LoadTile(tileType, x, y);
                        if (entityArray[x, y] != null)
                        {
                            entityArray[x, y].position = new Point(x, y);
                        }
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("File Error: " + e.ToString());
            }
        }

        private Entity LoadTile(char tileType, int x, int y) {
            Entity returnEntity;
            switch (tileType) {
                case 'M':
                    returnEntity = GameObject.createGameObject<Monk>(mapLoadRegion);
                    break;
                case 'P':
                    returnEntity = GameObject.createGameObject<Punk>(mapLoadRegion);
                    break;
                case '|':
                    returnEntity = GameObject.createGameObject<Wall>(mapLoadRegion);
                    Vector3 heading = new Vector3(0, 1, 0);
                    Vector3 strafe = new Microsoft.Xna.Framework.Vector3(-1, 0, 0);
                    returnEntity.getLocation().setHeadingAndStrafe(heading, strafe);
                    break;
                case '-':
                    returnEntity = GameObject.createGameObject<Wall>(mapLoadRegion);
                    Vector3 heading1 = new Vector3(0, 1, 0);
                    Vector3 strafe1 = new Microsoft.Xna.Framework.Vector3(-1, 0, 0);
                    returnEntity.getLocation().setHeadingAndStrafe(heading1, strafe1);
                    break;
                case 'S':
                    returnEntity = GameObject.createGameObject<Skunk>(mapLoadRegion);
                    break;
                case '!':
                    returnEntity = GameObject.createGameObject<Player>(mapLoadRegion);
                    player = (Player)returnEntity;
                    break;
                default:
                    returnEntity = null;
                    break;
            }
            if (returnEntity != null) {
                if (returnEntity is Character) {
                    ((Character)returnEntity).Map = this;
                    characterList.Add((Character)returnEntity);
                }
                returnEntity.getLocation().Position = new Microsoft.Xna.Framework.Vector3(x, y, 0);
            }
            return returnEntity;
        }

        /// <summary>
        /// Gets the characters that are within a given radius of the location
        /// </summary>
        /// <param name="location">The location to center the circle on</param>
        /// <param name="radius">The radius of the circle to check inside</param>
        /// <returns>A list of characters that are visisble</returns>
        public List<Character> getVisibleCharacters(Point location, float radius) {
            List<Character> charList = new List<Character>();
            for (int x = (int)Math.Max(location.X-radius,0); x <= Math.Min(location.X+radius,Width); x++) {
                for (int y = (int)Math.Max(location.Y-radius,0); y <= Math.Min(location.Y+radius,Height); y++) {
                    float val = (float)Math.Sqrt(Math.Pow(location.X - x, 2) + Math.Pow(location.Y - y, 2));
                    if (0 < val && val <= radius) {
                        Entity e = entityArray[x, y];
                        if (e != null && e is Character) {
                            charList.Add((Character)e);
                        }
                    }
                }
            }
            return charList;
        }

        /// <summary>
        /// Gets whether the given location in the game world is empty (i.e. is a valid square to move onto)
        /// </summary>
        /// <param name="location">The location to check</param>
        /// <returns>true if it is empty, false if it isn't</returns>
        public bool isEmpty(Point location) {
            if (location.X >= Width || location.Y >= Height || location.X < 0 || location.Y < 0) return false;
            return entityArray[location.X,location.Y]==null;
        }

        /// <summary>
        /// Sets the character on the map. Used to set the location of the character on the 
        /// map's internal representation of the game board
        /// </summary>
        /// <param name="character">The character object to update the map with</param>
        public void setCharacter(Point oldLocation, Character character) {
            if (character != null) {
                entityArray[character.position.X, character.position.Y] = character;
            }
            entityArray[oldLocation.X, oldLocation.Y] = null;
        }

        public void update(InteractionEngine.Networking.Client client, object ob) {
            foreach (Character c in characterList) {
                c.update();
            }
            Thread.Sleep(16);
            Engine.addEvent(new InteractionEngine.EventHandling.Event(this.id, "tick", null));
        }

        public Player getPlayer() {
            return player;
        }
    }

    public enum Direction {
        NORTH,SOUTH,EAST,WEST
    }
}