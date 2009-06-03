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
using InteractionEngine;
using InteractionEngine.Constructs.Datatypes;
using NTKPlusGame.World.Modules;
using WumpusGame.World;
using InteractionEngine.Networking;

namespace NTKPlusGame.World {

    /// <summary>
    /// SelectionFocus belongs to a single Player in his/her LocalLoadRegion.
    /// It keeps track of what's been selected by each Player.
    /// </summary>
    public class SelectionFocus : GameObject {

        #region FACTORY

        /// <summary>
        /// All GameObjects need a parameterless constructor for calling by GameObject.createGameObject() and GameObject.createFromUpdate().
        /// NEVER CALL THIS! This constructor is exclusively for use by the InteractionEngine. If anyone else calls it things will break.
        /// If you want to construct this object, use GameObject.createGameObject(LoadRegion).
        /// </summary>
        public SelectionFocus() {
        }

        // The classHash, a unique identifying string for the class. Hmm, wow, that's kind of redundant, isn't that? C# already provides such a function through reflection. Oh well.
        // Used for the factory methods called when the client receives a CREATE_NEW_OBJECT update from the server computer.
        public const string realHash = "SelectionFocus";
        public override string classHash {
            get { return realHash; }
        }

        /// <summary>
        /// The static constructor. Adds the class's factory method to the GameObject factoryList when the class is first loaded.
        /// </summary>
        static SelectionFocus() {
            GameObject.factoryList.Add(realHash, new GameObjectFactory(GameObject.createFromUpdate<SelectionFocus>));
        }

        #endregion


        private const int maxSelections = 50;
        private UpdatableInteger numberSelected;
        private UpdatableGameObject<Selectable>[] currentlySelected = new UpdatableGameObject<Selectable>[maxSelections];

        /*••••••••••••••••••••••••••••••••••••••••*\
          MEMBERS
        \*••••••••••••••••••••••••••••••••••••••••*/

        /// <summary>
        /// Constructs a SelectionFocus.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        public override void construct() {
            for (int i = 0; i < maxSelections; i++) this.currentlySelected[i] = new UpdatableGameObject<Selectable>(this);
            this.numberSelected = new UpdatableInteger(this);
        }

        /// <summary>
        /// Registers the given Selectable as having been selected.
        /// If no other Selectable is currently selected, makes the new Selectable the active selection.
        /// If there already is an active selection, notifies the active selection that a second selection had been made.
        /// If the active selection does not "accept" the new Selectable, makes the new Selectable the active selection.
        /// </summary>
        /// <param name="newSelection">The new Selectable that had been clicked on.</param>
        public void setSelection(Selectable newSelection, Client client, object param) {
            Selectable previous = currentlySelected[0].value;
            if (previous == null) {
                this.currentlySelected[0].value = newSelection;
                this.numberSelected.value = 1;
                if (newSelection is InfoDisplayable) NTKPlusUser.localUser.infoDisplayBox.setDisplayedObject((InfoDisplayable)newSelection);
            } else {
                bool actionAccepted = false;
                for (int i = 0; i < numberSelected.value; i++) actionAccepted |= currentlySelected[i].value.acceptSecondSelection(newSelection, client, param);
                if (!actionAccepted) {
                    this.currentlySelected[0].value = newSelection;
                    this.numberSelected.value = 1;
                    if (newSelection is InfoDisplayable) NTKPlusUser.localUser.infoDisplayBox.setDisplayedObject((InfoDisplayable)newSelection);
                }
            }
        }

        // no nulls allowed
        public void setMultipleSelections(Selectable[] selections, Client client) {
            numberSelected.value = selections.Length;
            for (int i = 0; i < selections.Length; i++) currentlySelected[i].value = selections[i];
        }

        public void addOnlyAsSecondSelection(GameObject secondSelection, Client client, object param) {
            for (int i = 0; i < numberSelected.value; i++) currentlySelected[i].value.acceptSecondSelection(secondSelection, client, param);
        }

        public void clearSelections() {
            numberSelected.value = 0;
        }

        public void addAsMultipleSelection(Selectable selection) {
            currentlySelected[numberSelected.value++].value = selection;
        }

        /// <summary>
        /// TODO!!! HAHAHA!!! GAY NETWORKING!
        /// </summary>
        /// <param name="position"></param>
        public void swarmTo(Vector3 position) {
            for (int j = 0; j < numberSelected.value; j++)
            {
                ((TerrainMovable)currentlySelected[j].value).getTerrainMovement().swarm.value.Units.Remove(((TerrainMovable)currentlySelected[j].value).getTerrainMovement().unit);
            }
            Swarm swarm = GameObject.createGameObject<Swarm>(this.getLoadRegion());
            for (int i = 0; i < numberSelected.value; i++)
            {
                ((TerrainMovable)currentlySelected[i].value).getTerrainMovement().swarm.value = swarm;
                Unit unit = new Unit();
                unit.Position = new Vector2((currentlySelected[i].value).getLocation().Position.X, (currentlySelected[i].value).getLocation().Position.Z);
                swarm.Units.Add(unit);
                ((TerrainMovable)currentlySelected[i].value).getTerrainMovement().unit = unit;
            }
        }

        /// <summary>
        /// We'll probably want the cursor to change icon indicating that an action is
        /// possible for a certain selection, or something.
        /// Also, probably highlight the currently moused-over Selectable.
        /// Implementation details to follow.
        /// </summary>
        /// <param name="selection">The new Selectable that had been moused-over.</param>
        public void selectionMousedOver(Selectable selection)
        {
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