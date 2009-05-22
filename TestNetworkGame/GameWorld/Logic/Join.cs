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

    public class Join : GameObject, Interactable {

        #region FACTORY

        /// <summary>
        /// All GameObjects need a parameterless constructor for calling by GameObject.createGameObject() and GameObject.createFromUpdate().
        /// NEVER CALL THIS! This constructor is exclusively for use by the InteractionEngine. If anyone else calls it things will break.
        /// If you want to construct this object, use GameObject.createGameObject(LoadRegion).
        /// </summary>
        public Join() {
        }

        // The classHash, a unique identifying string for the class. Hmm, wow, that's kind of redundant, isn't that? C# already provides such a function through reflection. Oh well.
        // Used for the factory methods called when the client receives a CREATE_NEW_OBJECT update from the server computer.
        public const string realHash = "Join";
        public override string classHash {
            get { return realHash; }
        }

        /// <summary>
        /// The static constructor. Adds the class's factory method to the GameObject factoryList when the class is first loaded.
        /// </summary>
        static Join() {
            GameObject.factoryList.Add(realHash, new GameObjectFactory(GameObject.createFromUpdate<Join>));
        }

        #endregion

        /// <summary>
        /// This method creates all of this GameObject's fields in constant order. 
        /// Instantiate modules and their fields here too.
        /// Pretty much the constructor. It'll be called every time this object is instantiated.
        /// </summary>
        public override void construct() {
            location = new Location(this);
            graphics = new JoinGraphics2D(this);
            this.addEventMethod("handleClick", this.handleClick);
            this.addEventMethod("returnAfterClick", ((JoinGraphics2D)this.graphics).returnAfterClick);
            this.addEventMethod("getHostedRegion", this.getHostedRegion);
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

        private Host host;

        public void associateHost(Host h) {
            host = h;
        }

        public void handleClick(Client client, object parameter) {
            // Graphics!
            this.graphics.onClick();
            // Kill any current regions.
            disconnect();
            if (host != null) {
                host.disconnect();
            }
            // Do the real stuff.
            Engine.status = Engine.Status.MULTIPLAYER_CLIENT;
            Engine.server = new Server("127.0.0.1");
            // Make sure we know what region is the hosted region so we can kill it later
            CreateRegion.onCreateRegion.Add(new Event(this.id, "getHostedRegion", null)); 
        }

        private LoadRegion hostedRegion;

        public void getHostedRegion(Client client, object parameter) {
            if (parameter is LoadRegion) hostedRegion = (LoadRegion)parameter;
        }

        public void disconnect() {
            if (hostedRegion == null) return;
            hostedRegion.deconstruct();
            hostedRegion = null;
            CreateRegion.onCreateRegion.Clear();
        }

    }

}