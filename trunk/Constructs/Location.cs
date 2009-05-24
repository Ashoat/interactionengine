/*••••••••••••••••••••••••••••••••••••••••*\
| Interaction Engine                       |
| (C) Copyright Bluestone Coding 2009      |
|••••••••••••••••••••••••••••••••••••••••••|
|           __    ___ ___  ___             |
|          /++\  | _ ) __|/ __|            |
|          \++/  | _ \__ \ (__             |
|           \/   |___/___/\___|            |
|                                          |
|••••••••••••••••••••••••••••••••••••••••••|
| MODULE                                   |
| * Location                     Class     |
| * Locatable                    Interface |
\*••••••••••••••••••••••••••••••••••••••••*/
using System;
using Microsoft.Xna.Framework;
namespace InteractionEngine.Constructs {

    /**
     * Holds all information and methods regarding location.
     */
    public class Location {

        // Contains a reference to the GameObject this Location module is associated with.
        // Used for constructing Updatables.
        private Locatable gameObject;
        // Contains this location in the form of a three-dimensional point.
        private InteractionEngine.Constructs.Datatypes.UpdatableVector point;
        // Contains the rotation of this point. Yaw, pitch, and roll, in that order
        private InteractionEngine.Constructs.Datatypes.UpdatableVector rotation;
        private InteractionEngine.Constructs.Datatypes.UpdatableVector heading;
        private InteractionEngine.Constructs.Datatypes.UpdatableVector strafe;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="gameObject">The GameObject whose Location this is.</param>
        public Location(Locatable gameObject) {
            this.gameObject = gameObject;
            this.point = new InteractionEngine.Constructs.Datatypes.UpdatableVector(gameObject);
            this.rotation = new InteractionEngine.Constructs.Datatypes.UpdatableVector(gameObject);
            this.heading = new InteractionEngine.Constructs.Datatypes.UpdatableVector(gameObject);
            this.strafe = new InteractionEngine.Constructs.Datatypes.UpdatableVector(gameObject);
        }

        /// <summary>
        /// Returns the point represented by this Location.
        /// </summary>
        /// <returns>The point represented by this Location.</returns>
        public virtual Microsoft.Xna.Framework.Vector3 getPoint() {
            return point.value;
        }

        private void calculateHeadingAndStrafe() {
            Microsoft.Xna.Framework.Vector3 defaultHeading = Microsoft.Xna.Framework.Vector3.Forward;
            Microsoft.Xna.Framework.Matrix pitchRotation = Microsoft.Xna.Framework.Matrix.CreateFromAxisAngle(Microsoft.Xna.Framework.Vector3.UnitX, this.pitch);
            Microsoft.Xna.Framework.Matrix yawRotation = Microsoft.Xna.Framework.Matrix.CreateFromAxisAngle(Microsoft.Xna.Framework.Vector3.UnitY, this.yaw);
            this.heading.value = Microsoft.Xna.Framework.Vector3.Transform(defaultHeading, pitchRotation * yawRotation);

            Microsoft.Xna.Framework.Vector3 defaultStrafe = Microsoft.Xna.Framework.Vector3.Right;
            Microsoft.Xna.Framework.Matrix rollRotation = Microsoft.Xna.Framework.Matrix.CreateFromAxisAngle(Microsoft.Xna.Framework.Vector3.UnitZ, this.roll);
            this.strafe.value = Microsoft.Xna.Framework.Vector3.Transform(defaultStrafe, rollRotation * yawRotation);
        }

        private void calculateEulerRotation() {
            float yaw = (float)System.Math.Atan2(this.heading.value.X, this.heading.value.Z);
            float pitch = (float)System.Math.Atan2(this.heading.value.Y, new Microsoft.Xna.Framework.Vector2(this.heading.value.X, this.heading.value.Z).Length());
            float roll = (float)System.Math.Atan2(this.strafe.value.Y, new Microsoft.Xna.Framework.Vector2(this.strafe.value.X, this.strafe.value.Z).Length());
            this.rotation.value = new Microsoft.Xna.Framework.Vector3(yaw, pitch, roll);
        }

        private Vector2 getEulerRotation(Vector3 vect)
        {
            float yaw = (float)System.Math.Atan2(vect.X, vect.Z);
            float pitch = (float)System.Math.Atan2(vect.Y, new Microsoft.Xna.Framework.Vector2(vect.X, vect.Z).Length());
            //float roll = (float)System.Math.Atan2(this.strafe.value.Y, new Microsoft.Xna.Framework.Vector2(this.strafe.value.X, this.strafe.value.Z).Length());
            return new Vector2(yaw, pitch);
        }

        // In radians
        public virtual Microsoft.Xna.Framework.Vector3 EulerRotation {
            get { return rotation.value; }
            set
            {
                this.rotation.value = value;
                calculateHeadingAndStrafe();
            }
        }

        public virtual Microsoft.Xna.Framework.Vector3 Heading {
            get { return heading.value; }
            set 
            {
                Vector2 currRot = this.getEulerRotation(this.heading.value);
                Vector2 diffRot = this.getEulerRotation(value) - currRot;

                Vector2 strafeRot = this.getEulerRotation(this.strafe.value) + diffRot;
                Vector3 strafe = Vector3.Transform(Vector3.Forward, Matrix.CreateRotationX(strafeRot.Y) * Matrix.CreateRotationY(strafeRot.X));

                this.strafe.value = strafe;
                this.heading.value = value;
                calculateEulerRotation();
            }
        }


        public virtual Microsoft.Xna.Framework.Vector3 Strafe {
            get { return strafe.value; }
        }

        // In radians
        public virtual float yaw {
            get { return this.rotation.value.X; }
            set {
                this.rotation.value = new Microsoft.Xna.Framework.Vector3(value, this.rotation.value.Y, this.rotation.value.Z);
                calculateHeadingAndStrafe();
            }
        }
        public virtual float pitch {
            get { return this.rotation.value.Y; }
            set {
                this.rotation.value = new Microsoft.Xna.Framework.Vector3(this.rotation.value.X, value, this.rotation.value.Z);
                calculateHeadingAndStrafe();
            }
        }
        public virtual float roll {
            get { return this.rotation.value.Z; }
            set {
                this.rotation.value = new Microsoft.Xna.Framework.Vector3(this.rotation.value.X, this.rotation.value.Y, value);
                calculateHeadingAndStrafe();
            }
        }

        public virtual void moveTo(Microsoft.Xna.Framework.Vector3 position) {
            this.point.value = position;
        }

        /// <summary>
        /// Translates this Location.
        /// </summary>
        /// <param name="translation">The vector describing the translation by which to move.</param>
        public virtual void move(Microsoft.Xna.Framework.Vector3 translation) {
            // Loving the encapsulation!
            this.point.value += translation;
        }

    }

    /**
     * Implemented by GameObjects that have the Location module.
     */
    public interface Locatable : GameObjectable {

        /// <summary>
        /// Returns the Location module of this GameObject.
        /// </summary>
        /// <returns>The Location module associated with this GameObject.
        Location getLocation();

    }

}