/*••••••••••••••••••••••••••••••••••••••••*\
| Interaction Engine                       |
| (C) Copyright Bluestone Coding 2009      |
|••••••••••••••••••••••••••••••••••••••••••|
|           __    ___ ___  ___             |
|          /++\  | _ ) __|/ __|            |
|          \++/  | _ \__ \ (__             |
|           \/   |___/___/\___|            |
|                                          |
|••••••••••••••••••••••••••••••••••••••••••|
| CLIENT                                   |
| * Interactable                 Interface |
| * Graphics                     Interface |
| * Graphable                    Interface |
\*••••••••••••••••••••••••••••••••••••••••*/

namespace InteractionEngine.Client {

    /**
     * Implemented by GameObjects that can be interacted with.
     */
    public interface Interactable : Graphable {

        /// <summary>
        /// Gets an Event from this Interactable module.
        /// </summary>
        /// <param name="invoker">The invoker of this Event. If you have multiple possible invokers (ie. mouse click and mouse over) then we recommend you define constants for them.</param>
        /// <param name="user">The User that invokes this Event. Needed often for associating User invokers with GameObject invokers.</param>
        /// <returns>An Event.</returns>
        Event getEvent(int invoker);

    }

    /**
     * Holds all information and methods regarding how a GameObject looks and is displayed.
     */
    public interface Graphics {

        /// <summary>
        /// Draws the GameObject. Could be just outputting text to a console, or could be advanced 3D drawing.
        /// </summary>
        void onDraw();

        /// <summary>
        /// Loads contents!
        /// </summary>
        void loadContent();

    }

    /**
     * Implemented by GameObjects that can be displayed.
     */
    public interface Graphable : InteractionEngine.Constructs.Locatable {

        /// <summary>
        /// Gets the Graphics class from GameObject.
        /// </summary>
        /// <returns>The Graphics class.</returns>
        Graphics getGraphics();

    }

}