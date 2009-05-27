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

namespace NTKPlusGame.World.Modules {

    /**
     * Holds all information and methods regarding Attack.
     * That is, all information necessary for the GameObject to inflict damage in conflict.
     * This includes:
     * -- Attack strength
     */

    public class Attack {

        public const string STRENGTH_STRING = "Strength";
        public static readonly Stats.StatType STRENGTH_STAT = new Stats.StatType(STRENGTH_STRING);
        public const string RANGE_STRING = "Range";
        public static readonly Stats.StatType RANGE_STAT = new Stats.StatType(RANGE_STRING);
        public const string DISTANCE_STRING = "Distance";
        public static readonly Stats.StatType DISTANCE_STAT = new Stats.StatType(DISTANCE_STRING);
        public const string DURATION_STRING = "Duration";
        public static readonly Stats.StatType DURATION_STAT = new Stats.StatType(DURATION_STRING);

        // Contains a reference to the GameObject this Attack module is associated with.
        // Used for constructing Updatables.
        private readonly Attackable gameObject;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="gameObject">The GameObject whose Attack this is.</param>
        public Attack(Attackable gameObject) {
            this.gameObject = gameObject;
            gameObject.getStats().registerStatType(STRENGTH_STAT);
            gameObject.getStats().registerStatType(RANGE_STAT);
            gameObject.getStats().registerStatType(DISTANCE_STAT);
            gameObject.getStats().registerStatType(DURATION_STAT);
        }

        //Devour attack
        public class Devour {

            /// <summary>
            /// Constructor.
            /// </summary>
            public Devour() {
            }
        }

        //Scream attack.
        public class Scream {

            /// <summary>
            /// Constructor.
            /// </summary>
            public Scream() {
            }
        }

        //Ranged attack.
        public class Ranged {

            /// <summary>
            /// Constructor.
            /// </summary>
            public Ranged() {
            }
        }

        //Insult attack.
        public class Insult {

            /// <summary>
            /// Constructor.
            /// </summary>
            public Insult() {
            }
        }

        public int getAttackStrength() {
            return gameObject.getStats().getStat(Attack.STRENGTH_STAT);
        }

        public int getAttackRange() {
            return gameObject.getStats().getStat(Attack.RANGE_STAT);
        }

        public int getAttackDistance() {
            return gameObject.getStats().getStat(Attack.DISTANCE_STAT);
        }

        public int getAttackDuration() {
            return gameObject.getStats().getStat(Attack.DURATION_STAT);
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