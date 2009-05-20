using InteractionEngine;
using InteractionEngine.Constructs;
using InteractionEngine.EventHandling;
using InteractionEngine.Networking;
using InteractionEngine.UserInterface;
using InteractionEngine.UserInterface.TwoDimensional;

namespace TestNetworkGame.Logic {

    public class GameField : GameObject, Graphable {

        #region Constructors

        /// <summary>
        /// This helper methods creates all of this GameObject's fields in constant order.
        /// </summary>
        private void makeFields() {

        }

        #region Client

        // The classHash, a unique identifying string for the class. 
        // Used for the factory methods called when the client receives a CREATE_OBJECT update from the server computer.
        internal const string classHash = "GameField";

        /// <summary>
        /// Returns the class hash. 
        /// </summary>
        /// <returns>The class hash. Do we really have to tell you everything twice?</returns>
        public override string getClassHash() {
            return classHash;
        }

        /// <summary>
        /// The static constructor. Adds the class's factory method to the GameObject factoryList when the class is first loaded.
        /// </summary>
        static GameField() {
            GameObject.factoryList.Add(classHash, new InteractionEngine.Constructs.GameObjectFactory(makeGameField));
        }
        
        /// <summary>
        /// A factory method that creates and returns a new instance of this GameObject. Used by the client when the server requests it to make a new GameObject.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        /// <param name="id">This GameObject's ID.</param>
        /// <returns>A new instance of this LoadRegion.</returns>
        static GameField makeGameField(InteractionEngine.Constructs.LoadRegion loadRegion, int id) {
            if (InteractionEngine.Engine.status != InteractionEngine.Engine.Status.MULTIPLAYER_CLIENT)
                throw new System.Exception("You're not a client, so why are you calling the GameObject factory method?");
            GameField field = new GameField(loadRegion, id);
            return field;
        }

        /// <summary>
        /// This method is meant to be called by a subclass constructor, which in turn was called by a factory method on that subclass.
        /// It instantiates a GameObject from the information that was sent to a MULTIPLAYER_CLIENT by a CREATE_OBJECT packet.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion this GameObject belongs to.</param>
        /// <param name="id">The ID of this GameObject.</param>
        private GameField(LoadRegion loadRegion, int id) 
            : base(loadRegion, id) {
            makeFields();
        }

        #endregion

        #region GameWorld

        /// <summary>
        /// All GameObjects need a parameterless constructor for calling by GameObject.createGameObject().
        /// NEVER CALL THIS! This constructor is exclusively for use by GameObject.createGameObject(). If anyone else calls it things will break.
        /// </summary>
        public GameField() {
        }

        /// <summary>
        /// Constructs the GameObject from the GameWorld.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        /// <returns>The constructed GameObject.</returns>
        public static GameField createGameField(LoadRegion loadRegion) {
            GameField newField = GameObject.createGameObject<GameField>(loadRegion);
            if (newField == null) return newField;
            newField.makeFields();
            return newField;
        }

        #endregion

        #endregion

        /// <summary>
        /// Returns the Location module of this GameObject.
        /// </summary>
        /// <returns>The Location module associated with this GameObject.
        private Location location;
        public Location getLocation() {
            return location;
        }

        /// <summary>
        /// Returns the Graphics module of this GameObject.
        /// </summary>
        /// <returns>The Graphics module associated with this GameObject.
        private Graphics graphics;
        public Graphics getGraphics() {
            return graphics;
        }

    }

}