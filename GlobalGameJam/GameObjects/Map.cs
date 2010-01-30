using InteractionEngine.Constructs;
using InteractionEngine.UserInterface;
using Microsoft.Xna.Framework.Input;
using GlobalGameJam.Graphics;
using InteractionEngine.UserInterface.TwoDimensional;
using System.Collections.Generic;
using InteractionEngine;
using System.IO;
using System;
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

        public override void construct() {
            location = new Location(this);
            graphics = new Graphics2DTexture(this);
            this.addEventMethod("tick", new InteractionEngine.EventHandling.EventMethod(this.update));
        }

        private int height;
        private int width;

        public readonly int Height {
            get { return height; }
        }

        public readonly int Width {
            get { return width; }
        }

        Entity[,] entityArray;

        public void LoadMap(string mapFile) {
            int width;
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
            switch (tileType) {
                case '.':
                    return null;
            }
        }

        /// <summary>
        /// Gets the characters that are within a given radius of the location
        /// </summary>
        /// <param name="location">The location to center the circle on</param>
        /// <param name="radius">The radius of the circle to check inside</param>
        /// <returns>A list of characters that are visisble</returns>
        public List<Character> getVisibleCharacters(Location location, float radius) {
            return null;
        }

        /// <summary>
        /// Gets whether the given location in the game world is empty (i.e. is a valid square to move onto)
        /// </summary>
        /// <param name="location">The location to check</param>
        /// <returns>true if it is empty, false if it isn't</returns>
        public bool isEmpty(Location location) {
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