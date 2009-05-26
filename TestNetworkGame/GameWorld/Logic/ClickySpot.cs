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

    public class ClickySpot : GameObject, Interactable {

        #region FACTORY

        /// <summary>
        /// All GameObjects need a parameterless constructor for calling by GameObject.createGameObject() and GameObject.createFromUpdate().
        /// NEVER CALL THIS! This constructor is exclusively for use by the InteractionEngine. If anyone else calls it things will break.
        /// If you want to construct this object, use GameObject.createGameObject(LoadRegion).
        /// </summary>
        public ClickySpot() {
        }

        // The classHash, a unique identifying string for the class. Hmm, wow, that's kind of redundant, isn't that? C# already provides such a function through reflection. Oh well.
        // Used for the factory methods called when the client receives a CREATE_NEW_OBJECT update from the server computer.
        public const string realHash = "ClickySpot";
        public override string classHash {
            get { return realHash; }
        }

        /// <summary>
        /// The static constructor. Adds the class's factory method to the GameObject factoryList when the class is first loaded.
        /// </summary>
        static ClickySpot() {
            GameObject.factoryList.Add(realHash, new GameObjectFactory(GameObject.createFromUpdate<ClickySpot>));
        }

        #endregion

        /// <summary>
        /// This method creates all of this GameObject's fields in constant order. 
        /// Instantiate modules and their fields here too.
        /// Pretty much the constructor. It'll be called every time this object is instantiated.
        /// </summary>
        public override void construct() {
            location = new Location(this);
            this.addEventMethod("handleClick", this.handleClick);
            graphics = new ClickySpotGraphics2D(this);
            this.currentO = new UpdatableBoolean(this);
            currentO.value = false;
        }

        public void setPosition(int startingXPos, int startingYPos) {
            graphics.setPosition(startingXPos, startingYPos);
        }

        private O oPiece;
        private X xPiece;

        public void relateToGamePieces(O o, X x) {
            oPiece = o;
            xPiece = x;
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
        private ClickySpotGraphics graphics;
        public InteractionEngine.UserInterface.Graphics getGraphics() {
            return graphics;
        }

        public Event getEvent(int invoker, Microsoft.Xna.Framework.Vector3 blah) {
            if (invoker == UserInterface2D.MOUSE_LEFT_CLICK) return new Event(this.id, "handleClick", null);
            else return null;
        }

        private UpdatableBoolean currentO;

        public void handleClick(Client client, object parameter) {
            if (!currentO.value && oPiece != null) {
                oPiece.getGamePiece().display.value = true;
                xPiece.getGamePiece().display.value = false;
                currentO.value = true;
            } else if (xPiece != null) {
                oPiece.getGamePiece().display.value = false;
                xPiece.getGamePiece().display.value = true;
                currentO.value = false;
            }
        }

    }

}