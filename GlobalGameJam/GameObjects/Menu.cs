using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InteractionEngine.Constructs;
using InteractionEngine.UserInterface.TwoDimensional;
using GlobalGameJam.Graphics;
using Microsoft.Xna.Framework.Input;
using InteractionEngine;
using Microsoft.Xna.Framework;

namespace GlobalGameJam.GameObjects {
    class Menu: GameObject, Graphable2D {
        
        private bool displayed = false;
        public Map map;
        public bool Displayed {
            get { return displayed; }
            set { displayed = value; }
        }
        public GlobalGameJam.GameObjects.GalactazoidsGame game;

        #region FACTORY

        /// <summary>
        /// All GameObjects need a parameterless constructor for calling by GameObject.createGameObject() and GameObject.createFromUpdate().
        /// NEVER CALL THIS! This constructor is exclusively for use by the InteractionEngine. If anyone else calls it things will break.
        /// If you want to construct this object, use GameObject.createGameObject(LoadRegion).
        /// </summary>
        public Menu() {
        }

        // The classHash, a unique identifying string for the class. Hmm, wow, that's kind of redundant, isn't that? C# already provides such a function through reflection. Oh well.
        // Used for the factory methods called when the client receives a CREATE_NEW_OBJECT update from the server computer.
        public const string realHash = "Menu";
        public override string classHash {
            get { return realHash; }
        }

        /// <summary>
        /// The static constructor. Adds the class's factory method to the GameObject factoryList when the class is first loaded.
        /// </summary>
        static Menu() {
            GameObject.factoryList.Add(realHash, new GameObjectFactory(GameObject.createFromUpdate<Menu>));
        }

        #endregion

        protected MenuGraphics graphics = new MenuGraphics();
        List<String> menuStrings;
        public int activeMenuItemIndex;
        public override void construct() {
            //graphics.LayerDepth = 0;
            //graphics.TextureName = "menu";
            //location.Position = new Microsoft.Xna.Framework.Vector3(0, 0, 0);
            menuStrings = new List<string>();
            menuStrings.Add("Join Multiplayer");
            menuStrings.Add("Play Level 1");
            menuStrings.Add("Play Level 2");
            menuStrings.Add("Play Level 3");
            graphics.menuStrings = menuStrings;
            graphics.Visible = false;
            graphics.activeMenuItemIndex = 0;
            
        }

        #region Graphable2D Members

        public Graphics2D getGraphics2D() {
            return graphics;
        }

        #endregion

        #region Graphable Members

        public InteractionEngine.UserInterface.Graphics getGraphics() {
            return graphics;
        }

        #endregion

        #region Locatable Members

        protected Location location;

        public Location getLocation() {
            return location;
        }

        #endregion

        internal void handleKey(Microsoft.Xna.Framework.Input.Keys key) {
            if (key.Equals(Keys.Escape)) {
                exitMenu();
            } else {
                switch (key) {
                    case Keys.Down:
                        graphics.activeMenuItemIndex = (graphics.activeMenuItemIndex + 1) % (menuStrings.Count);
                        break;
                    case Keys.Up:
                        graphics.activeMenuItemIndex--;
                        if (graphics.activeMenuItemIndex < 0)
                            graphics.activeMenuItemIndex = menuStrings.Count - 1;
                        break;
                    case Keys.Enter:
                        if (graphics.activeMenuItemIndex == 0) {
                            
                        } else if (graphics.activeMenuItemIndex == 1) {
                            if(map != null) map.getLoadRegion().deconstruct();
                            LoadRegion syncedRegion = createGame();
                            map.LoadMap("levels/level1.ani");
                        } else if (graphics.activeMenuItemIndex == 2) {
                            if (map != null) map.getLoadRegion().deconstruct();
                            LoadRegion syncedRegion = createGame();
                            map.LoadMap("levels/level2.ani");
                        } else if (graphics.activeMenuItemIndex == 3) {
                            if (map != null) map.getLoadRegion().deconstruct();
                            LoadRegion syncedRegion = createGame();
                            map.LoadMap("levels/level3.ani");
                        }
                        exitMenu();
                        break;
                    case Keys.M:
                        if (graphics.activeMenuItemIndex == 0) {

                        } else if (graphics.activeMenuItemIndex == 1) {
                            LoadRegion syncedRegion = createGame();

                            map.getLoadRegion().deconstruct();

                            map.LoadMap("levels/level1.ani");
                        } else if (graphics.activeMenuItemIndex == 2) {

                            map.getLoadRegion().deconstruct();

                            map.LoadMap("levels/level1.ani");
                        } else if (graphics.activeMenuItemIndex == 3) {
                            map.LoadMap("levels/level2.ani");
                        }
                        exitMenu();
                        break;
                }
            }
        }

        private LoadRegion createGame() {
            LoadRegion syncedRegion = LoadRegion.createLoadRegion();
            map = GameObject.createGameObject<Map>(syncedRegion);
            HealthBar health = GameObject.createGameObject<HealthBar>(syncedRegion);
            health.setLocationAndMap(new Vector3(15, 29, 0), map);
            game.addMap(map);
            return syncedRegion;
        }

        private void createMultiPlayerGame() {
            
        }

        private void exitMenu() {
            Displayed = false;
            map.Active = true;
            Engine.addEvent(new InteractionEngine.EventHandling.Event(map.id, "tick", null));
            graphics.Visible = false;
        }

        internal void show() {
            graphics.Visible = true;
            if (map != null) map.Active = false;
            Displayed = true;
        }
    }
}
