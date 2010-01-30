using InteractionEngine.Constructs;
using InteractionEngine.UserInterface;
using Microsoft.Xna.Framework.Input;
using GlobalGameJam.Graphics;
using InteractionEngine.UserInterface.TwoDimensional;
using System.Collections.Generic;
using InteractionEngine;
using System.IO;
using System;
using Microsoft.Xna.Framework;
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
            width = 25;
            height = 16;
            location.Position = new Vector3(0, 88, 0);
            mapLoadRegion = this.getLoadRegion();
            this.addEventMethod("tick", new InteractionEngine.EventHandling.EventMethod(this.update));
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

        public void LoadMap(string mapFile) {
            List<string> lines = new List<string>();
            using (StreamReader reader = new StreamReader(mapFile)) {
                string line = reader.ReadLine();
                while (line != null) {
                    lines.Add(line);
                    if (line.Length != Width)
                        throw new Exception(String.Format("The length of line {0} is different from all preceeding lines.", lines.Count));
                    line = reader.ReadLine();
                }
            }

            // Allocate the tile grid.
            entityArray = new Entity[Width, Height];

            // Loop over every tile position,
            for (int y = 0; y < Height; ++y) {
                for (int x = 0; x < Width; ++x) {
                    // to load each tile.
                    char tileType = lines[y][x];
                    entityArray[x, y] = LoadTile(tileType, x, y);
                }
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
                    returnEntity.getLocation().Heading = new Microsoft.Xna.Framework.Vector3(0, 1, 0);
                    break;
                case '-':
                    returnEntity = GameObject.createGameObject<Wall>(mapLoadRegion);
                    returnEntity.getLocation().Heading = new Microsoft.Xna.Framework.Vector3(1, 0, 0);
                    break;
                case 'S':
                    returnEntity = GameObject.createGameObject<Skunk>(mapLoadRegion);
                    break;
                case '!':
                    returnEntity = GameObject.createGameObject<Player>(mapLoadRegion);
                    break;
                default:
                    returnEntity = null;
                    break;
            }
            if (returnEntity != null) {
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
            return null;
        }

        /// <summary>
        /// Gets whether the given location in the game world is empty (i.e. is a valid square to move onto)
        /// </summary>
        /// <param name="location">The location to check</param>
        /// <returns>true if it is empty, false if it isn't</returns>
        public bool isEmpty(Point location) {
            return true;
        }

        /// <summary>
        /// Sets the character on the map. Used to set the location of the character on the 
        /// map's internal representation of the game board
        /// </summary>
        /// <param name="character">The character object to update the map with</param>
        public void setCharacter(Character character) {
        }

        public void update(InteractionEngine.Networking.Client client, object ob) {
            // In here I will call all the update events/methods on all of the characters
            
            Engine.addEvent(new InteractionEngine.EventHandling.Event(this.id, "tick", null));
        }
    }

}