using InteractionEngine;
using InteractionEngine.Constructs;
using InteractionEngine.EventHandling;
using InteractionEngine.Networking;
using InteractionEngine.UserInterface;
using TestNetworkGame.Graphics;
using TestNetworkGame.Graphics.TwoDimensional;
using TestNetworkGame.Modules;
using InteractionEngine.UserInterface.TwoDimensional;
using InteractionEngine.Constructs.Datatypes;

namespace TestNetworkGame.Logic {

    public class Host : GameObject, Interactable {

        #region FACTORY

        /// <summary>
        /// All GameObjects need a parameterless constructor for calling by GameObject.createGameObject() and GameObject.createFromUpdate().
        /// NEVER CALL THIS! This constructor is exclusively for use by the InteractionEngine. If anyone else calls it things will break.
        /// If you want to construct this object, use GameObject.createGameObject(LoadRegion).
        /// </summary>
        public Host() {
        }

        // The classHash, a unique identifying string for the class. Hmm, wow, that's kind of redundant, isn't that? C# already provides such a function through reflection. Oh well.
        // Used for the factory methods called when the client receives a CREATE_NEW_OBJECT update from the server computer.
        public const string realHash = "Host";
        public override string classHash {
            get { return realHash; }
        }

        /// <summary>
        /// The static constructor. Adds the class's factory method to the GameObject factoryList when the class is first loaded.
        /// </summary>
        static Host() {
            GameObject.factoryList.Add(realHash, new GameObjectFactory(GameObject.createFromUpdate<Host>));
        }

        #endregion

        /// <summary>
        /// This method creates all of this GameObject's fields in constant order. 
        /// Instantiate modules and their fields here too.
        /// Pretty much the constructor. It'll be called every time this object is instantiated.
        /// </summary>
        public override void construct() {
            location = new Location(this);
            graphics = new HostGraphics2D(this);
            this.addEventMethod("handleClick", this.handleClick);
            this.addEventMethod("returnAfterClick", ((HostGraphics2D)this.graphics).returnAfterClick);
            this.addEventMethod("handleNewPlayer", this.handleNewPlayer);
        }

        public void setPosition(int startingXPos, int startingYPos) {
            graphics.setPosition(startingXPos, startingYPos);
        }

        /// <summary>
        /// Returns the Location module of this GameObject.
        /// </summary>
        /// <returns>The Location module associated with this GameObject.</returns>
        private Location location;
        public Location getLocation() {
            return location;
        }

        /// <summary>
        /// Returns the Graphics module of this GameObject.
        /// </summary>
        /// <returns>The Graphics module associated with this GameObject.</returns>
        private ButtonGraphics graphics;
        public InteractionEngine.UserInterface.Graphics getGraphics() {
            return graphics;
        }

        public Event getEvent(int invoker) {
            if (invoker == UserInterface2D.MOUSE_LEFT_CLICK) return new Event(this.id, "handleClick", null);
            else return null;
        }

        private Join join;

        public void associateJoin(Join j) {
            join = j;
        }

        private LoadRegion hostedRegion;

        public void handleClick(Client client, object parameter) {
            // Graphics!
            this.graphics.onClick();
            // Kill any current regions.
            disconnect();
            if (join != null) {
                join.disconnect();
            }
            // Do the real stuff.
            Engine.status = Engine.Status.MULTIPLAYER_SERVERCLIENT;
            Client.startListening();
            // Set up the LoadRegion.
            hostedRegion = LoadRegion.createLoadRegion();
            // Set up the GameField.
            GameField gameField = GameObject.createGameObject<GameField>(hostedRegion);
            const int offset = 56;
            const int padding = 108;
            for (int i = 0; i < 3; i++) {
                for (int j = 0; j < 3; j++) {
                    // Give us an O!
                    O firstO = GameObject.createGameObject<O>(hostedRegion);
                    firstO.setPosition(offset + padding * i, offset + padding * j);
                    firstO.getGamePiece().display.value = false;
                    // Give us an X!
                    X firstX = GameObject.createGameObject<X>(hostedRegion);
                    firstX.setPosition(offset + padding * i, offset + padding * j);
                    firstX.getGamePiece().display.value = false;
                    // Give us a ClickySpot!
                    ClickySpot firstClicky = GameObject.createGameObject<ClickySpot>(hostedRegion);
                    firstClicky.setPosition(offset + padding * i, offset + padding * j);
                    firstClicky.relateToGamePieces(firstO, firstX);
                }
            }
            // Send the hosted region to anybody who joins the game
            Client.onJoin.Add(new Event(id, "handleNewPlayer", null));
        }

        public void handleNewPlayer(Client client, object parameter) {
            if (parameter is Client) {
                Client realClient = (Client)parameter;
                hostedRegion.sentToClient(realClient);
                foreach (GameObjectable gameObject in Engine.getGameObjectList()) {
                    if (hostedRegion.containsObject(gameObject.id))
                        realClient.addPermission(gameObject.id);
                }
            }
        }

        public void disconnect() {
            Client.stopListening();
            if (hostedRegion == null) return;
            hostedRegion.deconstruct();
            hostedRegion = null;
            Client.onJoin.Clear();
        }

    }

}