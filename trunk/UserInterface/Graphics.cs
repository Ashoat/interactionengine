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
| * Graphics                     Interface |
| * Graphable                    Interface |
\*••••••••••••••••••••••••••••••••••••••••*/

namespace InteractionEngine.UserInterface {



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