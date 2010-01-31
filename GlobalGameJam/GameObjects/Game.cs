using InteractionEngine.Constructs;
using InteractionEngine.UserInterface;
using Microsoft.Xna.Framework.Input;
using System.Threading;
using System;
using InteractionEngine.Networking;
using InteractionEngine;
using Microsoft.Xna.Framework;

namespace GlobalGameJam.GameObjects {

    public class GalactazoidsGame : GameObject, Keyboardable {

        Menu menu;

        #region FACTORY

        /// <summary>
        /// All GameObjects need a parameterless constructor for calling by GameObject.createGameObject() and GameObject.createFromUpdate().
        /// NEVER CALL THIS! This constructor is exclusively for use by the InteractionEngine. If anyone else calls it things will break.
        /// If you want to construct this object, use GameObject.createGameObject(LoadRegion).
        /// </summary>
        public GalactazoidsGame() {
        }

        // The classHash, a unique identifying string for the class. Hmm, wow, that's kind of redundant, isn't that? C# already provides such a function through reflection. Oh well.
        // Used for the factory methods called when the client receives a CREATE_NEW_OBJECT update from the server computer.
        public const string realHash = "Game";
        public override string classHash {
            get { return realHash; }
        }

        /// <summary>
        /// The static constructor. Adds the class's factory method to the GameObject factoryList when the class is first loaded.
        /// </summary>
        static GalactazoidsGame() {
            GameObject.factoryList.Add(realHash, new GameObjectFactory(GameObject.createFromUpdate<GalactazoidsGame>));
        }

        #endregion

        private TimeSpan start;

        public override void construct() {
            
            //this.addEventMethod("timetick", new InteractionEngine.EventHandling.EventMethod(this.tick));
            //Engine.addEvent(new InteractionEngine.EventHandling.Event(this.id, "timetick", null));
        }

        public Player player;

        private KeyboardState oldState;
        public void focusLost(Keyboardable blah) {
        }

        public void keyEvent(Keys key, KeyEvent keyEvent) {
            //Thread.Sleep(100);
            //Console.WriteLine("Key: " + key + ", " + keyEvent.ToString());
            //if (oldState == null) oldState = Keyboard.GetState();
            //KeyboardState state = Keyboard.GetState();
            if (menu.Displayed) {
                if (keyEvent == KeyEvent.KEY_PRESSED) menu.handleKey(key);
            } else {
                if (key==Keys.Escape) {
                    if (keyEvent == KeyEvent.KEY_PRESSED) {
                        menu.show();
                    }
                } else if (keyEvent == KeyEvent.IS_DOWN){
                    if (map != null && map.getPlayer() != null) map.getPlayer().handleKey(key);
                }
            }
        }

        Map map;
        internal void addMap(Map map) {
            this.map = map;
        }

        internal void setMenu(Menu menu) {
            this.menu = menu;
            this.menu.map = map;
        }

        private int counter = 0;

        private void tick(Client c, object o) {
            if (start == null) {
                start = Engine.gameTime.TotalRealTime;
            }
            TimeSpan delta = Engine.gameTime.TotalRealTime - start;
            //Console.WriteLine(delta);
            if (delta.Seconds >= 1) {
                start = Engine.gameTime.TotalRealTime;
                //Console.WriteLine("ticks/sec: " + counter);
                counter = 0;
            }
            counter++;            
            //Engine.addEvent(new InteractionEngine.EventHandling.Event(this.id, "timetick", null));
        }
    }

}
