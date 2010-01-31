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
using InteractionEngine.Networking;
using InteractionEngine.EventHandling;
using InteractionEngine.Constructs.Datatypes;

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
        
        public override void construct() {
            //graphics.LayerDepth = 0;
            //graphics.TextureName = "menu";
            //location.Position = new Microsoft.Xna.Framework.Vector3(0, 0, 0);
            menuStrings = new List<string>();
            menuStrings.Add("Join Multiplayer");
            menuStrings.Add("Play Level 1");
            menuStrings.Add("Play Level 2");
            menuStrings.Add("Play Level 3");
            menuStrings.Add("Play Level 4");
            menuStrings.Add("Play Level 5");
            graphics.menuStrings = menuStrings;
            graphics.Visible = false;
            graphics.activeMenuItemIndex = 0;

            this.addEventMethod("getHostedRegion", this.getHostedRegion);
            this.addEventMethod("handleDroppedConnection", this.handleDroppedConnection);
            this.addEventMethod("handleNewPlayer", this.handleNewPlayer);
            this.addEventMethod("getGameObject", this.getGameObject);
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
                            Client.stopListening();
                            Client.onJoin.Clear();
                            CreateRegion.onCreateRegion.Clear();
                            Engine.status = Engine.Status.MULTIPLAYER_CLIENT;
                            if (map != null) map.getLoadRegion().deconstruct();
                            LoadRegion syncedRegion = createGame();
                            try {
                                Engine.server = new Server("127.0.0.1");
                                // Make sure we know what region is the hosted region so we can kill it later
                                CreateRegion.onRegionTransfer.Add(new Event(this.id, "getHostedRegion", null));
                                Server.onDisconnect.Add(new Event(this.id, "handleDroppedConnection", null));
                                CreateObject.onCreateObject.Add(new Event(this.id, "getGameObject", null));
                            } catch (GameWorldException) {
                                Engine.status = Engine.Status.SINGLE_PLAYER;
                            }
                        } else if (graphics.activeMenuItemIndex == 1) {
                            if(map != null) map.getLoadRegion().deconstruct();
                            LoadRegion syncedRegion = createGame();
                            map.LoadMap("levels/campaign1.ani");
                            exitMenu();
                        } else if (graphics.activeMenuItemIndex == 2) {
                            if (map != null) map.getLoadRegion().deconstruct();
                            LoadRegion syncedRegion = createGame();
                            map.LoadMap("levels/campaign2.ani");//level2.ani");
                            exitMenu();
                        } else if (graphics.activeMenuItemIndex == 3) {
                            if (map != null) map.getLoadRegion().deconstruct();
                            LoadRegion syncedRegion = createGame();
                            map.LoadMap("levels/campaign3.ani");
                            exitMenu();
                        }
                        else if (graphics.activeMenuItemIndex == 4) {
                            if (map != null) map.getLoadRegion().deconstruct();
                            LoadRegion syncedRegion = createGame();
                            map.LoadMap("levels/campaign4.ani");
                            exitMenu();
                        } else if (graphics.activeMenuItemIndex == 5) {
                            if (map != null) map.getLoadRegion().deconstruct();
                            LoadRegion syncedRegion = createGame();
                            map.LoadMap("levels/campaign5.ani");
                            exitMenu();
                        }
                    /*
                            } else if (graphics.activeMenuItemIndex == 2) {
                                map.LoadMap("levels/level2.ani");
                            } else if (graphics.activeMenuItemIndex == 3) {
                                map.LoadMap("levels/level3.ani");
                            }
                        }*/
                        break;
                    case Keys.M:
                        if (graphics.activeMenuItemIndex == 0) {
                            Client.stopListening();
                            Client.onJoin.Clear();
                            CreateRegion.onCreateRegion.Clear();
                            Engine.status = Engine.Status.MULTIPLAYER_CLIENT;
                            if (map != null) map.getLoadRegion().deconstruct();
                            LoadRegion syncedRegion = createGame();
                            try {
                                Engine.server = new Server("127.0.0.1");
                                // Make sure we know what region is the hosted region so we can kill it later
                                CreateRegion.onRegionTransfer.Add(new Event(this.id, "getHostedRegion", null));
                                Server.onDisconnect.Add(new Event(this.id, "handleDroppedConnection", null));
                                CreateObject.onCreateObject.Add(new Event(this.id, "getGameObject", null));
                            } catch (GameWorldException) {
                                Engine.status = Engine.Status.SINGLE_PLAYER;
                            }
                        } else {
                            Client.stopListening();
                            Client.onJoin.Clear();
                            CreateRegion.onCreateRegion.Clear();
                            Engine.status = Engine.Status.MULTIPLAYER_SERVERCLIENT;
                            Client.startListening();
                            if (map != null) map.getLoadRegion().deconstruct();
                            LoadRegion syncedRegion = createGame();
                            if (graphics.activeMenuItemIndex == 1) {
                                map.LoadMap("levels/campaign1.ani");
                            } else if (graphics.activeMenuItemIndex == 2) {
                                map.LoadMap("levels/campaign2.ani");
                            } else if (graphics.activeMenuItemIndex == 3) {
                                map.LoadMap("levels/campaign3.ani");
                            } else if (graphics.activeMenuItemIndex == 4) {
                                map.LoadMap("levels/campaign4.ani");
                            } else if (graphics.activeMenuItemIndex == 5) {
                                map.LoadMap("levels/campaign5.ani");
                            } 
                            Client.onJoin.Add(new Event(id, "handleNewPlayer", null));
                            exitMenu();
                        }
                        break;
                }
            }
        }

        private LoadRegion createGame() {
            LoadRegion syncedRegion = LoadRegion.createLoadRegion();
            map = GameObject.createGameObject<Map>(syncedRegion);
            HealthBar health = GameObject.createGameObject<HealthBar>(syncedRegion);
            if (health != null) health.setLocationAndMap(new Vector3(15, 29, 0), map);
            game.addMap(map);
            return syncedRegion;
        }

        public void handleNewPlayer(Client client, object parameter) {
            if (parameter is Client) {
                Client realClient = (Client)parameter;
                map.getLoadRegion().sentToClient(realClient);
                realClient.addPermission(map.id);
            }
        }

        public void getHostedRegion(Client client, object parameter) {
            if (parameter is LoadRegion) {
                LoadRegion lr = (LoadRegion)parameter;
                foreach (GameObject go in lr.getGameObjectArray()) {
                    if (go is Map) {
                        exitMenu();
                        map = (Map)go;
                        //map.LoadMap(map.mapFile.value);
                        map.Active = true;
                        game.addMap(map);
                        map.characterList = new List<UpdatableGameObject<Character>>();
                        map.entityArray = new UpdatableGameObject<Entity>[25,16];
                        foreach (Updatable updatable in map.getFields().Values) {
                            if (updatable is UpdatableInteger) {
                                GameObjectable go2 = Engine.getGameObject(((UpdatableInteger)updatable).value);
                                if (go != null && go2 is Character) {
                                    map.characterList.Add(new UpdatableGameObject<Character>(((UpdatableInteger)updatable)));
                                }
                                if (go != null && go2 is Entity) {
                                    map.entityArray[((Entity)go2).Position.X, ((Entity)go2).Position.Y] = new UpdatableGameObject<Entity>((UpdatableInteger)updatable);
                                }
                                if (go2 != null && go2 is Player) {
                                    if (((Player)go2).isClient.value == true) map.clientPlayer.value = (Player)go2;
                                    else map.serverPlayer.value = (Player)go2;
                                }
                            }
                        }
                        foreach (Updatable updatable in map.getFields().Values) {
                            if (updatable is UpdatableInteger) {
                                GameObjectable go2 = Engine.getGameObject(((UpdatableInteger)updatable).value);
                                if (go != null && go2 is Character) {
                                    map.characterList.Add(new UpdatableGameObject<Character>(((UpdatableInteger)updatable)));
                                }
                            }
                        }
                        Engine.addEvent(new InteractionEngine.EventHandling.Event(map.id, "tick", null));
                        return;
                    }
                }
            }
        }

        public void getGameObject(Client client, object parameter) {
            /*if (parameter is Character) {
                foreach (Updatable updatable in ((Character)parameter).getFields().Values) {
                    if (updatable is UpdatableInteger) {
                        GameObjectable go = Engine.getGameObject(((UpdatableInteger)updatable).value);
                        if (go != null && go is Character) {
                            map.characterList.Add(new UpdatableGameObject<Character>((UpdatableInteger)updatable));
                        }
                    }
                }
            }
            if (parameter is Entity) {
                foreach (Updatable updatable in ((Entity)parameter).getFields().Values) {
                    if (updatable is UpdatableInteger) {
                        GameObjectable go = Engine.getGameObject(((UpdatableInteger)updatable).value);
                        if (go != null && go is Entity) {
                            map.entityArray[((Entity)parameter).Position.X, ((Entity)parameter).Position.Y] = new UpdatableGameObject<Entity>((UpdatableInteger)updatable);
                        }
                    }
                }
            }*/
        }

        public void handleDroppedConnection(Client client, object parameter) {
            Client.stopListening();
            Client.onJoin.Clear();
            CreateRegion.onCreateRegion.Clear();
            Engine.status = Engine.Status.SINGLE_PLAYER;
            if (map != null) map.getLoadRegion().deconstruct();
        }

        private void exitMenu() {
            Displayed = false;
            if (map != null) {
                map.Active = true;
                Engine.addEvent(new InteractionEngine.EventHandling.Event(map.id, "tick", null));
            }
            graphics.Visible = false;
        }

        internal void show() {
            graphics.Visible = true;
            if (map != null) map.Active = false;
            Displayed = true;
        }
    }
}
