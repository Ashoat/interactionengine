/*••••••••••••••••••••••••••••••••••••••••*\
| NTK+                                     |
| (C) Copyright Bluestone Coding 2009      |
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
| GAME OBJECTS                             |
| * SelectionFocus                   Class |
\*••••••••••••••••••••••••••••••••••••••••*/

using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework;
using InteractionEngine.Constructs;
using InteractionEngine.GameWorld;
using InteractionEngine.Constructs.Datatypes;
using NTKPlusGame.World.Modules;

namespace NTKPlusGame.World {

    /// <summary>
    /// SelectionFocus belongs to a single Player in his/her LocalLoadRegion.
    /// It keeps track of what's been selected by each Player.
    /// </summary>
    public class SelectionFocus : GameObject {

        #region FACTORY

        // The classHash, a unique identifying string for the class. Hmm, wow, that's kind of redundant, isn't that? C# already provides such a function through reflection. Oh well.
        // Used for the factory methods called when the client receives a CREATE_NEW_OBJECT update from the server computer.
        internal const string classHash = "SelectionFocus";

        /// <summary>
        /// The static constructor. Adds the class's factory method to the GameObject factoryList when the class is first loaded.
        /// </summary>
        static SelectionFocus() {
            GameObject.factoryList.Add(classHash, new GameObjectFactory(makeSelectionFocus));
        }

        /// <summary>
        /// A factory method that creates and returns a new instance of SelectionFocus. Used by the client when the server requests it to make a new GameObject.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        /// <param name="id">This GameObject's ID.</param>
        /// <param name="reader">The PacketReader from which we will read the fields of the newly constructed GameObject.</param>
        /// <returns>A new instance of SelectionFocus.</returns>
        static SelectionFocus makeSelectionFocus(LoadRegion loadRegion, int id, Microsoft.Xna.Framework.Net.PacketReader reader) {
            if (GameWorld.status != GameWorld.Status.MULTIPLAYER_CLIENT)
                throw new System.Exception("You're not a client, so why are you calling the GameObject factory method?");
            SelectionFocus SelectionFocus = new SelectionFocus(loadRegion, id);
            // ORDER OF STUFF (where you used the reader to construct datatypes, used factory methods exclusively. also, construct modules and their datatypes here too.)
            return SelectionFocus;
        }

        /// <summary>
        /// Constructs a GameObject and assigns it an ID.
        /// This is the constructor that should be used if and only if you are a MULTIPLAYER_CLIENT.
        /// Furthermore, it is only called by the GameObjectFactory method.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        /// <param name="id">This GameObject's ID.</param>
        private SelectionFocus(LoadRegion loadRegion, int id)
            : base(loadRegion, id) {
        }

        /// <summary>
        /// Returns the class hash. 
        /// </summary>
        /// <returns>The class hash. Do we really have to tell you everything twice?</returns>
        public override string getClassHash() {
            return classHash;
        }

        #endregion


        private readonly UpdatableGameObject<Selectable> currentlySelected;

        /*••••••••••••••••••••••••••••••••••••••••*\
          MODULES
        \*••••••••••••••••••••••••••••••••••••••••*/

        /// <summary>
        /// Returns the Graphics module of this GameObject.
        /// </summary>
        /// <returns>The Graphics module associated with this GameObject.
        //        private SelectionFocus2DGraphics graphics;
        //        public InteractionEngine.Client.Graphics getGraphics() {
        //            return graphics;
        //        }

        /*••••••••••••••••••••••••••••••••••••••••*\
          MEMBERS
        \*••••••••••••••••••••••••••••••••••••••••*/

        /// <summary>
        /// Constructs a SelectionFocus.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        public SelectionFocus(LoadRegion loadRegion)
            : base(loadRegion) {
            currentlySelected = new UpdatableGameObject<Selectable>(this);
            //            this.graphics = new SelectionFocus2DGraphics(this);
        }

        /// <summary>
        /// Registers the given Selectable as having been selected.
        /// If no other Selectable is currently selected, makes the new Selectable the active selection.
        /// If there already is an active selection, notifies the active selection that a second selection had been made.
        /// If the active selection does not "accept" the new Selectable, makes the new Selectable the active selection.
        /// </summary>
        /// <param name="newSelection">The new Selectable that had been clicked on.</param>
        public void addSelection(Selectable newSelection, object param) {
            Selectable previous = currentlySelected.value;
            if (previous == null) {
                this.currentlySelected.value = newSelection;
            } else {
                bool actionAccepted = previous.acceptSecondSelection(newSelection, param);
                if (!actionAccepted) this.currentlySelected.value = newSelection;
            }
        }

        public void addOnlyAsSecondSelection(GameObject secondSelection, object param) {
            Selectable previous = currentlySelected.value;
            if (previous != null) previous.acceptSecondSelection(secondSelection, param);
        }

        /// <summary>
        /// We'll probably want the cursor to change icon indicating that an action is
        /// possible for a certain selection, or something.
        /// Also, probably highlight the currently moused-over Selectable.
        /// Implementation details to follow.
        /// </summary>
        /// <param name="selection">The new Selectable that had been moused-over.</param>
        public void selectionMousedOver(Selectable selection) {
            // TODO
        }

        /// <summary>
        /// We'll probably want the cursor to change icon indicating that an action is
        /// possible for a certain selection, or something.
        /// Also, probably highlight the currently moused-over Selectable.
        /// Implementation details to follow.
        /// </summary>
        /// <param name="selection">The new Selectable that had been moused-over.</param>
        public void selectionMousedOut(Selectable selection) {
            // TODO
        }

    }

}