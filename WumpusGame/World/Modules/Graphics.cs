/*••••••••••••••••••••••••••••••••••••••••*\
| Interaction Engine                       |
| (C) Copyright Bluestone Coding 2008      |
|••••••••••••••••••••••••••••••••••••••••••|
|           __    ___ ___  ___             |
|          /++\  | _ ) __|/ __|            |
|          \++/  | _ \__ \ (__             |
|           \/   |___/___/\___|            |
|                                          |
|••••••••••••••••••••••••••••••••••••••••••|
| MODULE                                   |
| * Graphics                     Class     |
| * Graphable                    Interface |
\*••••••••••••••••••••••••••••••••••••••••*/

using InteractionEngine.Constructs.Datatypes;
using InteractionEngine.Constructs;
using InteractionEngine.Client;

namespace WumpusGame.World.Modules {

    /**
     * Implemented by the Graphics module on GameObjects that can be interacted with.
     */
    public interface Interactable {

        /// <summary>
        /// Gets an Event from this Interactable module.
        /// </summary>
        /// <param name="invoker">The invoker of this Event. If you have multiple possible invokers (ie. mouse click and mouse over) then we recommend you define constants for them.</param>
        /// <returns></returns>
        Event getEvent(int invoker);

    }

    /**
     * Holds all information and methods regarding graphics.
     */
    public class Graphics {

        /// <summary>
        /// The GameObject to which this module belongs.
        /// </summary>
        public GameObject gameObject;
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="gameObject">The GameObject whose Location this is.</param>
        public Graphics(GameObject gameObject) {
            this.gameObject = gameObject;
        }

        /// <summary>
        /// Constructor for the client-side, maybe I guess.
        /// </summary>
        /// <param name="gameObject">The GameObject whose module this is.</param>
        /// <param name="reader">The reader from which to read teh field datas.</param>
        public Graphics(GameObject gameObject, Microsoft.Xna.Framework.Net.PacketReader reader) {
            this.gameObject = gameObject;
        }

        /// <summary>
        /// Output a string.
        /// </summary>
        /// <param name="output">The string to output.</param>
        public void output(string output) {
            ((UI) InteractionEngine.Server.GameWorld.GameWorld.ui).addOutputToBuffer(output);
        }

    }

    /**
     * Implemented by GameObjects that have the Graphics module.
     */
    public interface Graphable : Locatable {

        /// <summary>
        /// Returns the Graphics module of this GameObject.
        /// </summary>
        /// <returns>The Graphics module associated with this GameObject.
        Graphics getGraphics();

    }

}