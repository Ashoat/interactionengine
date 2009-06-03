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
| * Attack                       Class     |
| * Devour                       Class     |
| * Scream                       Class     |
| * Ranged                       Class     |
| * Insult                       Class     |
| * Attackable                   Interface |
\*••••••••••••••••••••••••••••••••••••••••*/

using InteractionEngine.Constructs.Datatypes;
using InteractionEngine.Constructs;
using InteractionEngine;
using Microsoft.Xna.Framework;
using InteractionEngine.UserInterface.ThreeDimensional;

namespace NTKPlusGame.World.Modules {

    /**
     * Holds all information and methods regarding Attack.
     * That is, all information necessary for the GameObject to inflict damage in conflict.
     * This includes:
     * -- Attack strength
     */

    public class Attack {
        
        //Attack variables
        private static UpdatableInteger Strength;
        private static UpdatableInteger Range;
        private static UpdatableInteger Distance;
        private static UpdatableInteger Duration;
        private static UpdatableString AttackType;

        // Contains a reference to the GameObject this Attack module is associated with.
        // Used for constructing Updatables.
        private readonly Attackable gameObject;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="gameObject">The GameObject whose Attack this is.</param>
        public Attack(Attackable gameObject) {
            this.gameObject = gameObject;
        }

        //Devour attack
        public void Devour(Combatable target) {
            AttackType.value = "Devour";
            Strength.value = 20;
            Range.value = 1;
            Distance.value = 100;
            ((Graphics3DModel)gameObject.getGraphics3D()).StartAnimation("Animations\\");
            target.onBeingAttacked(gameObject);
        }

        //Scream attack.
        public void Scream(Combatable target) {
            AttackType.value = "Scream";
            Strength.value = 20;
            Range.value = 1;
            Distance.value = 100;
            ((Graphics3DModel)gameObject.getGraphics3D()).StartAnimation("Animations\\");
            target.onBeingAttacked(gameObject);
        }

        //Ranged attack.  
        public void Ranged(Combatable target) {
            AttackType.value = "Ranged";
            Strength.value = 20;
            Range.value = 1;
            Distance.value = 100;
            ((Graphics3DModel)gameObject.getGraphics3D()).StartAnimation("Animations\\");
            target.onBeingAttacked(gameObject);
        }

        //Insult attack.
        public void Insult(Combatable target) {
            AttackType.value = "Insult";
            Strength.value = 20;
            Range.value = 1;
            Distance.value = 100;
            ((Graphics3DModel)gameObject.getGraphics3D()).StartAnimation("Animations\\");
            target.onBeingAttacked(gameObject);
        }

        public int getAttackStrength() {
            return Strength.value;
        }

        public int getAttackRange() {
            return Range.value;
        }

        public int getAttackDistance() {
            return Distance.value;
        }

        public int getAttackDuration() {
            return Duration.value;
        }

        public string getAttackType() {
            return AttackType.value;
        }

        // TODO

    }

    /**
     * Implemented by GameObjects that have the Attack module.
     */
    public interface Attackable : Combatable {

        /// <summary>
        /// Returns the Attack module of this GameObject.
        /// </summary>
        /// <returns>The Attack module associated with this GameObject.
        Attack getAttack();

    }

}