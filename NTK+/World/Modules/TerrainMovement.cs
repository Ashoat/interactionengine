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
| * TerrainMovement              Class     |
| * TerrainMovable               Interface |
\*••••••••••••••••••••••••••••••••••••••••*/

using InteractionEngine.Constructs.Datatypes;
using InteractionEngine.Constructs;
using InteractionEngine;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;

namespace NTKPlusGame.World.Modules {

    /**
     * Holds all information and methods regarding modules that handle Movement on terrain.
     */
    public class TerrainMovement : TerrainLocation {
		
		public const string SPEED_STRING = "Movement speed";
		public readonly Stats.StatType SPEED_STAT = new Stats.StatType(SPEED_STRING);
		private const float speedRatio = 0.001f;

        /// <summary>
        /// Triggered when this GameObject arrives at the destination towards which it was headed.
        /// </summary>
        public event EventHandler destinationArrived;

        // Contains a reference to the GameObject this TerrainMovement module is associated with.
        // Used for constructing Updatables.
        private readonly TerrainMovable gameObject;
		// Desired location
        private readonly UpdatableVector targetPosition;
        // Desired location
        private readonly UpdatableGameObject<Locatable> targetGameObject;
        // Desired distance away from targetGameObject
        private readonly UpdatableDouble targetTrackingDistance;
		// is moving?
		private readonly UpdatableBoolean isMoving;
		// Time of last update or whatever
		private readonly UpdatableDouble lastUpdate;
		
		/// <summary>
        /// Constructor.
		/// Stats module must be initialized first.
        /// </summary>
        /// <param name="gameObject">The GameObject whose TerrainMovement this is.</param>
        public TerrainMovement(TerrainMovable gameObject) : base(gameObject) {
            this.gameObject = gameObject;
			this.targetPosition = new UpdatableVector(gameObject);
            this.targetGameObject = new UpdatableGameObject<Locatable>(gameObject);
            this.targetTrackingDistance = new UpdatableDouble(gameObject);
			this.lastUpdate = new UpdatableDouble(gameObject);
			this.isMoving = new UpdatableBoolean(gameObject);
			this.gameObject.getStats().registerStatType(SPEED_STAT);
            this.gameObject.getStats().setBaseStat(SPEED_STAT, 10);
        }
		

		/// <summary>
        /// Starts moving to the specified location.
        /// </summary>
        /// <param name="target">The location to go to.</param>
		public void startWalking(Vector3 target) {
            Vector3 direction = target - base.Position;
            this.yaw = (float)Math.Atan2(direction.X, direction.Z);
			this.targetPosition.value = target;
            this.isMoving.value = true;
			this.lastUpdate.value = Engine.gameTime.TotalRealTime.TotalMilliseconds;
		}

        /// <summary>
        /// Starts tracking the specified GameObject.
        /// That is, follow the specified GameObject until you get within a specified distance.
        /// </summary>
        /// <param name="target">The GameObject to track.</param>
        /// <param name="followingDistance">The distance from the target at which to stop.</param>
        public void startTracking(Locatable target, float trackingDistance) {
            this.targetGameObject.value = target;
            this.isMoving.value = true;
            this.lastUpdate.value = Engine.gameTime.TotalRealTime.TotalMilliseconds;
        }
		
        /// <summary>
        /// Returns the point represented by this Location.
        /// </summary>
        /// <returns>The point represented by this Location.</returns>
		public override Vector3 Position {
            get {
                if (this.isMoving.value) {
                    updateTerrainMovement();
                }
                return base.Position;
            }
            set { base.Position = value; }
		}

        private Vector3 getVectorToTarget() {
            if (this.targetGameObject.value == null) return this.targetPosition.value - base.Position;
            else {
                Vector3 toTarget = this.targetGameObject.value.getLocation().Position - base.Position;
                if (this.targetTrackingDistance.value != 0) {
                    double originalLength = toTarget.Length();
                    double desiredLength = originalLength - this.targetTrackingDistance.value;
                    Vector3.Multiply(toTarget, (float)(desiredLength / originalLength));
                }
                return toTarget;
            }
        }

		private void updateTerrainMovement() {
			double currentTime = Engine.gameTime.TotalRealTime.TotalMilliseconds;
			double timeElapsed = currentTime - this.lastUpdate.value;
			if (timeElapsed != 0) {
				// Calculate the displacement actually traveled
				float speed = speedRatio * this.gameObject.getStats().getStat(SPEED_STAT);
				float displacement = speed * (float)timeElapsed;
				// Calculate the displacement needed to get to the target
                Vector3 toTarget = this.getVectorToTarget();
				// If we overshot it, just go directly to target
				if (displacement >= toTarget.Length()) {
					base.move(toTarget);
					isMoving.value = false;
                    if (destinationArrived != null) destinationArrived.Invoke(this.gameObject, null);
				} else {  // Travel!
                    toTarget.Normalize();
                    base.move(Vector3.Multiply(toTarget, displacement));
				}
				// Update time
				this.lastUpdate.value = currentTime;
			}
		}

    }


    /**
     * Implemented by GameObjects that have TerrainMovement.
     */
    public interface TerrainMovable : Statsable, TerrainLocatable {

        /// <summary>
        /// Returns the TerrainMovement module of this GameObject.
        /// </summary>
        /// <returns>The TerrainMovement module associated with this GameObject.
        TerrainMovement getTerrainMovement();

    }


}