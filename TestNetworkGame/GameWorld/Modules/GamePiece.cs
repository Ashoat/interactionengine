using InteractionEngine.UserInterface;
using InteractionEngine.Constructs.Datatypes;

namespace TestNetworkGame.Modules {

    public class GamePiece {

        public UpdatableBoolean display;

    }

    /**
     * Implemented by GameObjects that have the Player module.
     */
    public interface GamePieceable : Graphable {

        /// <summary>
        /// Returns the GamePiece module.
        /// </summary>
        /// <returns>The GamePiece module.</returns>
        GamePiece getGamePiece();

    }

}