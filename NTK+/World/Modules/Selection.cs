/*••••••••••••••••••••••••••••••••••••••••*\
| NTK+ Game                                |
| (C) Copyright Bluestone Coding 2009      |
|••••••••••••••••••••••••••••••••••••••••••|
|           __    ___ ___  ___             |
|          /++\  | _ ) __|/ __|            |
|          \++/  | _ \__ \ (__             |
|           \/   |___/___/\___|            |
|                                          |
|••••••••••••••••••••••••••••••••••••••••••|
| MODULE                                   |
| * Selection                    Class     |
| * Selectable                   Interface |
\*••••••••••••••••••••••••••••••••••••••••*/

using InteractionEngine.Constructs.Datatypes;
using InteractionEngine.Constructs;
using InteractionEngine;
using Microsoft.Xna.Framework;
using InteractionEngine.UserInterface;
using Microsoft.Xna.Framework.Graphics;
using InteractionEngine.Networking;

namespace NTKPlusGame.World.Modules {

    /**
     * Holds all information and methods regarding Selection.
     * That is, all information necessary for the GameObject to be selected by mouse out of the 3D scene.
     * This includes:
     * -- Bounding box/sphere
     */

    /**
     * Implemented by GameObjects that have the Selection module.
     */
    public interface Selectable : InteractionEngine.UserInterface.ThreeDimensional.Interactable3D {

        /// <summary>
        /// Handles the event where another GameObject is selected while this one possesses the SelectionFocus.
        /// </summary>
        /// <param name="second">The other GameObject that was selected.</param>
        /// <param name="param">Any additional information provided by the second selection.</param>
        /// <returns>True if the second selection indicates an action-triggering option,
        /// false if the SelectionFocus should be transferred to the new selection.</returns>
        bool acceptSecondSelection(GameObjectable second, Client client, object param);


        /// <summary>
        /// We'll probably want the cursor to change icon indicating that an action is
        /// possible for a certain selection, or something.
        /// Also, probably highlight the currently moused-over Selectable.
        /// Implementation details to follow.
        /// TODO
        /// </summary>
        /// <param name="selection">The new Selectable that had been moused-over.</param>
        //Texture2D getSecondSelectionMousedOverIcon(Selectable second);

    }

}