﻿/*••••••••••••••••••••••••••••••••••••••••*\
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
| * Attackable                   Interface |
\*••••••••••••••••••••••••••••••••••••••••*/

using InteractionEngine.Constructs.Datatypes;
using InteractionEngine.Constructs;
using InteractionEngine.GameWorld;
using Microsoft.Xna.Framework;

namespace NTKPlusGame.World.Modules {

    /**
     * Holds all information and methods regarding Attack.
     * That is, all information necessary for the GameObject to inflict damage in conflict.
     * This includes:
     * -- Attack strength
     */

    public class Attack {

        public const string ATTACK_STRING = "Attack";
        public static readonly Stats.StatType ATTACK_STAT = new Stats.StatType(ATTACK_STRING);
        public const string RANGE_STRING = "Range";
        public static readonly Stats.StatType RANGE_STAT = new Stats.StatType(RANGE_STRING);

        // Contains a reference to the GameObject this Attack module is associated with.
        // Used for constructing Updatables.
        private readonly Attackable gameObject;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="gameObject">The GameObject whose Attack this is.</param>
        public Attack(Attackable gameObject) {
            this.gameObject = gameObject;
            gameObject.getStats().registerStatType(ATTACK_STAT);
            gameObject.getStats().registerStatType(RANGE_STAT);
        }

        /// <summary>
        /// Constructor for the client-side.
        /// </summary>
        /// <param name="gameObject">The GameObject of whose Attack this is.</param>
        /// <param name="reader">The reader from which to read teh field datas.</param>
        internal Attack(GameObject gameObject, Microsoft.Xna.Framework.Net.PacketReader reader) {
            /*byte transferCode = reader.ReadByte();
            UpdatableInteger intty = (UpdatableInteger)GameWorld.createField(reader);
            roomAttack = new UpdatableGameObject<Room>(intty);
            if (reader.ReadByte() == GameWorld.UPDATE_FIELD) GameWorld.updateField(reader);
            else reader.Position--;
            this.gameObject = gameObject;*/
        }

        public int getAttackStrength() {
            return gameObject.getStats().getStat(Attack.ATTACK_STAT);
        }

        public int getAttackRange() {
            return gameObject.getStats().getStat(Attack.RANGE_STAT);
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