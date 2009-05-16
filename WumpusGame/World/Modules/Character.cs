/*••••••••••••••••••••••••••••••••••••••••*\
| Wumpus Game                              |
| (C) Copyright Bluestone Coding 2008      |
|••••••••••••••••••••••••••••••••••••••••••|
| Built using the Interaction Engine       |
| (C) Copyright Bluestone Coding 2008      |
|••••••••••••••••••••••••••••••••••••••••••|
|           __    ___ ___  ___             |
|          /++\  | _ ) __|/ __|            |
|          \++/  | _ \__ \ (__             |
|           \/   |___/___/\___|            |
|                                          |
|••••••••••••••••••••••••••••••••••••••••••|
| MODULE                                   |
| * Character                    Class     |
| * Characterable                Interface |
\*••••••••••••••••••••••••••••••••••••••••*/

using InteractionEngine.Constructs.Datatypes;
using InteractionEngine.Constructs;

namespace WumpusGame.World.Modules {

    /**
     * Holds all information and methods regarding character.
     * NOT a very useful module, is it?
     */
    public class Character {

        /// <summary>
        /// The GameObject to which this module belongs.
        /// </summary>
        public GameObject gameObject;

    }

    /**
     * Implemented by GameObjects that have the Character module.
     */
    public interface Characterable : Graphable {

        /// <summary>
        /// Returns the Character module of this GameObject.
        /// </summary>
        /// <returns>The Character module associated with this GameObject.
        Character getCharacter();

    }

}