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
| * Combat                       Class     |
| * Combatable                   Interface |
\*••••••••••••••••••••••••••••••••••••••••*/

using InteractionEngine.Constructs.Datatypes;
using InteractionEngine.Constructs;
using InteractionEngine;
using Microsoft.Xna.Framework;
using InteractionEngine.Networking;

namespace NTKPlusGame.World.Modules {

    /**
     * Holds all information and methods regarding Combat.
     * That is, all information necessary for the GameObject to be targetted in combat.
     * This includes:
     * -- Health
     * -- Defense stuff ?
     */

    public class Combat {

        public const string HEALTH_STRING = "Health";
        public static readonly Stats.StatType HEALTH_STAT = new Stats.StatType(HEALTH_STRING);

        // Contains a reference to the GameObject this Combat module is associated with.
        // Used for constructing Updatables.
        private readonly Combatable gameObject;
        private readonly UpdatableBoolean isOnNullTeam;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="gameObject">The GameObject whose Combat this is.</param>
        public Combat(Combatable gameObject) {
            this.gameObject = gameObject;
            this.isOnNullTeam = new UpdatableBoolean(gameObject);
            gameObject.getStats().registerStatType(Combat.HEALTH_STAT);
            gameObject.getStats().setBaseStat(HEALTH_STAT, 100);
        }

        public void setOnNullTeam(bool nullTeam) {
            isOnNullTeam.value = nullTeam;
        }

        public bool isOnOurTeam(Client client) {
            return isOnNullTeam.value == (client == null);
        }

        public int getHealth() {
            return this.gameObject.getStats().getStat(Combat.HEALTH_STAT);
        }

        void onBeingAttacked(Attackable attacker) {
            UpdateStats(attacker);
        }

        void UpdateStats(Attackable attacker) {
            if ((int)getDistance(attacker, gameObject) < attacker.getAttack().getAttackRange()) {
                if (gameObject.getStats().getBaseStat(HEALTH_STAT) > 0) {
                    gameObject.getStats().setBaseStat(HEALTH_STAT, (gameObject.getStats().getBaseStat(HEALTH_STAT) - attacker.getAttack().getAttackStrength()));
                }
                else {
                    gameObject.deconstruct();
                }
            }
        }

        float getDistance(Locatable first, Locatable second) { 
            return (first.getLocation().Position - second.getLocation().Position).Length();
        }


    }

    /**
     * Implemented by GameObjects that have the Combat module.
     */
    public interface Combatable : Statsable {

        /// <summary>
        /// Returns the Combat module of this GameObject.
        /// </summary>
        /// <returns>The Combat module associated with this GameObject.</returns>
        Combat getCombat();

        /// <summary>
        /// Handler for when this GameObject is being attacked.
        /// </summary>
        /// <param name="attacker">The one attacking this GameObject.</param>
        void onBeingAttacked(Attackable attacker);

    }

}