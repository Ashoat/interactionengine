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
        private UpdatableInteger Strength;
        private UpdatableInteger Range;
        private UpdatableInteger Distance;
        private UpdatableInteger Duration;
        private UpdatableString AttackType;

        // Contains a reference to the GameObject this Attack module is associated with.
        // Used for constructing Updatables.
        private readonly Attackable gameObject;
        private Combatable target;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="gameObject">The GameObject whose Attack this is.</param>
        public Attack(Attackable gameObject) {
            this.gameObject = gameObject;
        }

        //Devour attack
        public void Devour(Combatable target) {
            this.target = target;
            AttackType.value = "Devour";
            Strength.value = 20;
            Range.value = 1;
            Distance.value = 100;
            ((Graphics3DModel)gameObject.getGraphics3D()).StartAnimation("Animations\\");
            DoAttack(target);
        }

        //Scream attack.
        public void Scream(Combatable target) {
            this.target = target;
            AttackType.value = "Scream";
            Strength.value = 20;
            Range.value = 1;
            Distance.value = 100;
            ((Graphics3DModel)gameObject.getGraphics3D()).StartAnimation("Animations\\");
            DoAttack(target);
        }

        //Ranged attack.  
        public void Ranged(Combatable target) {
            this.target = target;
            AttackType.value = "Ranged";
            Strength.value = 20;
            Range.value = 1;
            Distance.value = 100;
            ((Graphics3DModel)gameObject.getGraphics3D()).StartAnimation("Animations\\");
            DoAttack(target);
        }

        //Insult attack.
        public void Insult(Combatable target) {
            this.target = target;
            AttackType.value = "Insult";
            Strength.value = 20;
            Range.value = 1;
            Distance.value = 100;
            ((Graphics3DModel)gameObject.getGraphics3D()).StartAnimation("Animations\\");
            DoAttack(target);
        }

        void DoAttack(Combatable target) {
            if (gameObject is TerrainMovable) {
                ((TerrainMovable)gameObject).getTerrainMovement().startTracking(target, gameObject.getAttack().getAttackRange());
                ((TerrainMovable)gameObject).getTerrainMovement().destinationArrived += new System.EventHandler(onArrived);
            }
            else {
                if (getDistance(gameObject, target) <= gameObject.getAttack().getAttackDistance()) {
                    this.target.onBeingAttacked(gameObject);
                }
            }
        }

        void onArrived(object sender, System.EventArgs e) {
            this.target.onBeingAttacked(gameObject);
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

        float getDistance(Locatable first, Locatable second) {
            return (first.getLocation().Position - second.getLocation().Position).Length();
        }

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