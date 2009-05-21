using InteractionEngine;
using InteractionEngine.Constructs;
using InteractionEngine.EventHandling;
using InteractionEngine.Networking;
using InteractionEngine.UserInterface;
using TestNetworkGame.Graphics;
using TestNetworkGame.Graphics.TwoDimensional;
using TestNetworkGame.Modules;

namespace TestNetworkGame.Logic {

    public class X : GameObject, GamePieceable {

        #region FACTORY

        /// <summary>
        /// All GameObjects need a parameterless constructor for calling by GameObject.createGameObject() and GameObject.createFromUpdate().
        /// NEVER CALL THIS! This constructor is exclusively for use by the InteractionEngine. If anyone else calls it things will break.
        /// If you want to construct this object, use GameObject.createGameObject(LoadRegion).
        /// </summary>
        public X() {
        }

        // The classHash, a unique identifying string for the class. Hmm, wow, that's kind of redundant, isn't that? C# already provides such a function through reflection. Oh well.
        // Used for the factory methods called when the client receives a CREATE_NEW_OBJECT update from the server computer.
        public const string realHash = "X";
        public override string classHash {
            get { return realHash; }
        }

        /// <summary>
        /// The static constructor. Adds the class's factory method to the GameObject factoryList when the class is first loaded.
        /// </summary>
        static X() {
            GameObject.factoryList.Add(realHash, new GameObjectFactory(GameObject.createFromUpdate<X>));
        }

        #endregion

        /// <summary>
        /// This method creates all of this GameObject's fields in constant order. 
        /// Instantiate modules and their fields here too.
        /// Pretty much the constructor. It'll be called every time this object is instantiated.
        /// </summary>
        public override void construct() {
            location = new Location(this);
            graphics = new XGraphics2D(this);
            gamePiece = new GamePiece();
            gamePiece.display = new InteractionEngine.Constructs.Datatypes.UpdatableBoolean(this);
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
        private GamePieceGraphics graphics;
        public InteractionEngine.UserInterface.Graphics getGraphics() {
            return graphics;
        }

        /// <summary>
        /// Returns the GamePiece module of this GameObject.
        /// </summary>
        /// <returns>The GamePiece module associated with this GameObject.</returns>
        private GamePiece gamePiece;
        public GamePiece getGamePiece() {
            return gamePiece;
        }

        public void setPosition(int startingXPos, int startingYPos) {
            graphics.setPosition(startingXPos, startingYPos);
        }

    }

}