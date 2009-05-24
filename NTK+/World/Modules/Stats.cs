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
| * Stats                        Class     |
| * Statsable                    Interface |
\*••••••••••••••••••••••••••••••••••••••••*/

using InteractionEngine.Constructs.Datatypes;
using InteractionEngine.Constructs;
using InteractionEngine;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;

namespace NTKPlusGame.World.Modules {

    /**
     * Holds all information and methods regarding modules that handle buffable stats.
     */
    public class Stats {

        // Contains a reference to the GameObject this Stats module is associated with.
        // Used for constructing Updatables.
        private readonly Statsable gameObject;
        private readonly Dictionary<StatType, UpdatableInteger> stats = new Dictionary<StatType, UpdatableInteger>();
        private readonly Dictionary<StatType, StatModification> modifications = new Dictionary<StatType, StatModification>();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="gameObject">The GameObject whose Stats this is.</param>
        public Stats(Statsable gameObject) {
            this.gameObject = gameObject;
        }

        /// <summary>
        /// Registers a StatType on this GameObject so that this GameObject has a stat corresponding
        /// to that StatType.
        /// Must be done upon GameObject construction... dynamically adding stats isn't going to work!
        /// </summary>
        /// <param name="stat">The type of the stat.</param>
        public void registerStatType(StatType stat) {
            this.stats.Add(stat, new UpdatableInteger(gameObject));
        }

        /// <summary>
        /// Returns the base value of the specified stat before modifications.
        /// </summary>
        /// <param name="stat">The type of the stat.</param>
        /// <returns>The base value of the specified stat before modifications.</returns>
        public int getBaseStat(StatType stat) {
            return this.stats[stat].value;
        }

        /// <summary>
        /// Gets the value of the specified stat, after buffs and such.
        /// </summary>
        /// <param name="stat">The type of the stat.</param>
        /// <returns>The value of the specified stat, after buffs and such.</returns>
        public int getStat(StatType stat) {
            if (!this.modifications.ContainsKey(stat)) return this.getBaseStat(stat);
            StatModification buff = this.modifications[stat];
            bool expired;
            int modification = this.modifications[stat].getChainStatModification(this, out expired);
            if (expired) this.modifications[stat] = this.modifications[stat].getNextModification();
            return this.getBaseStat(stat) + modification;
        }

        /// <summary>
        /// Adds a "buff" to the stat indicated.
        /// </summary>
        /// <param name="stat">The type of the stat.</param>
        /// <param name="buff">The buff.</param>
        public void addStatBuff(StatType stat, StatModification buff) {
            StatModification preexisting = this.modifications[stat];
            if (preexisting == null) this.modifications[stat] = buff;
            else preexisting.appendNewModification(buff);
        }

        /// <summary>
        /// Permanently sets the base value of the stat.
        /// </summary>
        /// <param name="stat">The type of the stat.</param>
        /// <param name="newValue">The new base value.</param>
        public void setBaseStat(StatType stat, int newValue) {
            this.stats[stat].value = newValue;
        }

        /// <summary>
        /// Permanentlty augments/weakens the specified base stat by the specified amount.
        /// </summary>
        /// <param name="stat">The type of the stat.</param>
        /// <param name="modification">The amount to modify the stat.</param>
        public void modifyBaseStat(StatType stat, int modification) {
            this.stats[stat].value += modification;
        }

        /**
         * Class representing type of stat... basically encapsulates a string, but with stricter type-safety.
         * Meant to be used as a static readonly variable.
         */
        [Serializable]
        public sealed class StatType {
            private readonly string type;
            public StatType(string type) {
                this.type = type;
            }
            public override bool Equals(object o) {
                return o is StatType && ((StatType)o).type.Equals(this.type);
            }
            public override int GetHashCode() {
                return type.GetHashCode();
            }
            public override string ToString() {
                return type;
            }
        }

        /**
         * Basically represents a "buff."
         * Implemented in linked-list style!
         * This allows us to chain multiple StatModifications together.
         */ 
        public abstract class StatModification : GameObject {

            // Contains the next StatModification
            // Used for linked-list implementation
            private UpdatableGameObject<StatModification> nextModification;

            /// <summary>
            /// Server-side constructor.
            /// </summary>
            /// <param name="loadRegion"></param>
            public override void  construct() {
                this.nextModification = new UpdatableGameObject<StatModification>(this);
            }

            protected StatModification() {
            }

            /// <summary>
            /// Returns the total modification to the stat given by this StatModification and all the others
            /// below it in the list.
            /// </summary>
            /// <param name="stats">The Stats module of the GameObject that this StatModification applies to.</param>
            /// <param name="expired">Whether or not this StatModification has expired and may now be removed.</param>
            /// <returns>The total modification to the stat given by this StatModification and all the others
            /// below it in the list.</returns>
            public int getChainStatModification(Stats stats, out bool expired) {
                int cumulativeModification = 0;
                StatModification nextOne = nextModification.value;
                if (nextOne != null) {
                    bool nextOneExpired;
                    cumulativeModification = nextOne.getChainStatModification(stats, out nextOneExpired);
                    if (nextOneExpired) this.nextModification.value = nextOne.getNextModification();
                }
                expired = this.isExpired(stats);
                return cumulativeModification + this.getStatModification(stats);
            }

            /// <summary>
            /// Returns the next StatModification in the linked list.
            /// </summary>
            /// <returns>The next StatModification in the linked list.</returns>
            public StatModification getNextModification() {
                return nextModification.value;
            }

            /// <summary>
            /// Appends a new StatModification to the end of the linked list.
            /// </summary>
            /// <param name="newModification">The new StatModification to append.</param>
            public void appendNewModification(StatModification newModification) {
                StatModification nextOne = nextModification.value;
                if (nextOne == null) nextModification.value = newModification;
                else nextOne.appendNewModification(newModification);
            }

            /// <summary>
            /// Returns the amount of the stat modification: positive for a buff, negative for a debuff.
            /// </summary>
            /// <param name="stats">The Stats module of the GameObject that this StatModification applies to.</param>
            /// <returns>The amount of the stat modification: positive for a buff, negative for a debuff.</returns>
            protected abstract int getStatModification(Stats stats);

            /// <summary>
            /// Returns whether or not this stat modification has expired.
            /// </summary>
            /// <param name="stats">The Stats module of the GameObject that this StatModification applies to.</param>
            /// <returns>True if  this stat modification has expired, false otherwise.</returns>
            protected abstract bool isExpired(Stats stats);

            /// <summary>
            /// Returns the description of the stat modification.
            /// </summary>
            /// <returns>The description of the stat modification.</returns>
            public abstract string getDescription();

        }

        public class SimpleTimedStatModification : StatModification {


            #region FACTORY

            /// <summary>
            /// All GameObjects need a parameterless constructor for calling by GameObject.createGameObject() and GameObject.createFromUpdate().
            /// NEVER CALL THIS! This constructor is exclusively for use by the InteractionEngine. If anyone else calls it things will break.
            /// If you want to construct this object, use GameObject.createGameObject(LoadRegion).
            /// </summary>
            public SimpleTimedStatModification() {
            }

            // The classHash, a unique identifying string for the class. Hmm, wow, that's kind of redundant, isn't that? C# already provides such a function through reflection. Oh well.
            // Used for the factory methods called when the client receives a CREATE_NEW_OBJECT update from the server computer.
            public const string realHash = "SimpleTimedStatModification";
            public override string classHash {
                get { return realHash; }
            }

            /// <summary>
            /// The static constructor. Adds the class's factory method to the GameObject factoryList when the class is first loaded.
            /// </summary>
            static SimpleTimedStatModification() {
                GameObject.factoryList.Add(realHash, new GameObjectFactory(GameObject.createFromUpdate<SimpleTimedStatModification>));
            }

            #endregion

            private UpdatableDouble expirationTime;
            private UpdatableInteger amount;
            private UpdatableString statType;
            private UpdatableString description;

            public override void construct() {
                base.construct();
                this.statType = new UpdatableString(this);
                this.amount = new UpdatableInteger(this);
                this.expirationTime = new UpdatableDouble(this);
                this.description = new UpdatableString(this);
            }

            public void initialize(LoadRegion loadRegion, StatType statType, int amount, double duration, string description) {
                this.statType.value = statType.ToString();
                this.amount.value = amount;
                this.expirationTime.value = Engine.gameTime.TotalRealTime.TotalMilliseconds + duration * 1000;
                this.description.value = description;
            }

            protected override int getStatModification(Stats stats) {
                return this.amount.value;
            }

            protected override bool isExpired(Stats stats) {
                return Engine.gameTime.TotalRealTime.TotalMilliseconds >= this.expirationTime.value;
            }

            public override string getDescription() {
                return this.description.value;
            }

        }

    }


    /**
     * Implemented by GameObjects that have Stats.
     * Not sure about the "Selectable" part, but we want some way of displaying buffs and stats here.
     */
    public interface Statsable : Selectable {

        /// <summary>
        /// Returns the Stats module of this GameObject.
        /// </summary>
        /// <returns>The Stats module associated with this GameObject.
        Stats getStats();

    }


}