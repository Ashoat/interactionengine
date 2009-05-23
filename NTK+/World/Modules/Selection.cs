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

namespace NTKPlusGame.World.Modules {

    /**
     * Holds all information and methods regarding Selection.
     * That is, all information necessary for the GameObject to be selected by mouse out of the 3D scene.
     * This includes:
     * -- Bounding box/sphere
     */

    public class Selection {

        // Contains a reference to the GameObject this Selection module is associated with.
        // Used for constructing Updatables.
        private readonly Selectable gameObject;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="gameObject">The GameObject whose Selection this is.</param>
        public Selection(Selectable gameObject) {
            this.gameObject = gameObject;
        }

        /// <summary>
        /// Constructor for the client-side.
        /// </summary>
        /// <param name="gameObject">The GameObject of whose Selection this is.</param>
        /// <param name="reader">The reader from which to read teh field datas.</param>
        internal Selection(GameObject gameObject, Microsoft.Xna.Framework.Net.PacketReader reader) {
            /*byte transferCode = reader.ReadByte();
            UpdatableInteger intty = (UpdatableInteger)Engine.createField(reader);
            roomSelection = new UpdatableGameObject<Room>(intty);
            if (reader.ReadByte() == Engine.UPDATE_FIELD) Engine.updateField(reader);
            else reader.Position--;
            this.gameObject = gameObject;*/
        }

    }

    /**
     * Implemented by GameObjects that have the Selection module.
     */
    public interface Selectable : InteractionEngine.UserInterface.ThreeDimensional.Interactable3D {

        /// <summary>
        /// Returns the Selection module of this GameObject.
        /// </summary>
        /// <returns>The Selection module associated with this GameObject.
        Selection getSelection();

        /// <summary>
        /// Handles the event where another GameObject is selected while this one possesses the SelectionFocus.
        /// </summary>
        /// <param name="second">The other GameObject that was selected.</param>
        /// <param name="param">Any additional information provided by the second selection.</param>
        /// <returns>True if the second selection indicates an action-triggering option,
        /// false if the SelectionFocus should be transferred to the new selection.</returns>
        bool acceptSecondSelection(GameObjectable second, object param);


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