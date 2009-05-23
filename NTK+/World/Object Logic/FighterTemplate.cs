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
| * FighterTemplate                  Class |
\*••••••••••••••••••••••••••••••••••••••••*/

using InteractionEngine.Constructs;
using InteractionEngine;
using InteractionEngine.UserInterface;
using NTKPlusGame.World.Modules;
using InteractionEngine.UserInterface.ThreeDimensional;
using System;
using InteractionEngine.Constructs.Datatypes;

namespace NTKPlusGame.World {

    /// <summary>
    /// A template for 'em fightin' types.
    /// Note: before we make much progress, we need to make a program to generate these templates for us.
    /// </summary>
    public abstract class FighterTemplate : WalkerTemplate, Attackable {

        // Specifies this GameObject's current target of attack.
        protected UpdatableGameObject<Combatable> target;

        /// <summary>
        /// Returns the Attack module of this GameObject.
        /// </summary>
        /// <returns>The Attack module associated with this GameObject.
        private Attack attack;
        public Attack getAttack() {
            return attack;
        }

        /// <summary>
        /// Returns the Combat module of this GameObject.
        /// </summary>
        /// <returns>The Combat module associated with this GameObject.
        private Combat combat;
        public Combat getCombat() {
            return combat;
        }

        public FighterTemplate() {

        }

        /// <summary>
        /// Constructs a new FighterTemplate.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        public override void construct() {
            combat = new Combat(this);
            attack = new Attack(this);
            this.target = new UpdatableGameObject<Combatable>(this);
        }

        /// <summary>
        /// Handles the event where another GameObject is selected while this one possesses the SelectionFocus.
        /// </summary>
        /// <param name="second">The other GameObject that was selected.</param>
        /// <param name="param">Any additional information provided by the second selection.</param>
        /// <returns>True if the second selection indicates an action-triggering option,
        /// false if the SelectionFocus should be transferred to the new selection.</returns>
        public override bool acceptSecondSelection(GameObjectable second, object param) {
            if (base.acceptSecondSelection(second, param)) return true;

            const bool OTHER_OBJECT_BELONGS_TO_ENEMY = false;
            if (OTHER_OBJECT_BELONGS_TO_ENEMY && second is Combatable) {
                onCommandedToAttackSomethingElse((Combatable)second);
                return true;
            }

            return false;

        }

        /// <summary>
        /// Handler for when this GameObject has been ordered to attack something else.
        /// </summary>
        /// <param name="somethingElse">The thing that this GameObject has been ordered to attack.</param>
        public virtual void onCommandedToAttackSomethingElse(Combatable somethingElse) {
            this.target.value = somethingElse;
            this.getTerrainMovement().startTracking(somethingElse, this.getAttack().getAttackRange());
            this.getTerrainMovement().destinationArrived += new EventHandler(onArrivedAtAttackTargetLocation);
        }

        /// <summary>
        /// Handler for when we have finished walking toward the attack target and now desire to hurt it!
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        public virtual void onArrivedAtAttackTargetLocation(object sender, EventArgs e) {
            this.attacking();
            this.target.value.onBeingAttacked(this);
        }

        /// <summary>
        /// Specifically execute attack maneuvers, including animations and damage-doing.
        /// </summary>
        protected virtual void attacking() {
            // TODO: attack animations/code!
        }

        /// <summary>
        /// Handler for when this GameObject is being attacked.
        /// Default behavior is to attack back if not already attacking something else.
        /// </summary>
        /// <param name="attacker">The one attacking this GameObject.</param>
        public virtual void onBeingAttacked(Attackable attacker) {
            if (this.target.value == null) onCommandedToAttackSomethingElse(attacker);
        }

    }

}