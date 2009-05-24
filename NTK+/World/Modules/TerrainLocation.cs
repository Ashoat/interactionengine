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
| * Location                     Class     |
| * Locatable                    Interface |
\*••••••••••••••••••••••••••••••••••••••••*/

using InteractionEngine.Constructs.Datatypes;
using InteractionEngine.Constructs;
using InteractionEngine;
using Microsoft.Xna.Framework;

namespace NTKPlusGame.World.Modules {

    /**
     * Holds all information and methods regarding location for terrain-grounded objects.
     */

    public class TerrainLocation : Location {

        // Contains a reference to the GameObject this Location module is associated with.
        // Used for constructing Updatables.
        private readonly TerrainLocatable gameObject;
        // Contains whether or not this location is fixed to the Terrain.
        // Used for specifying whether or not this location is fixed to the Terrain.
        private readonly UpdatableBoolean grounded;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="gameObject">The GameObject whose Location this is.</param>
        public TerrainLocation(TerrainLocatable gameObject) : base(gameObject) {
            this.gameObject = gameObject;
            grounded = new UpdatableBoolean(gameObject);
            grounded.value = true;
        }

        /// <summary>
        /// Translates this Location along the horizontal plane.
        /// Also adjusts to the altitude of the terrain if this Location is grounded.
        /// </summary>
        /// <param name="dx">The x-component of the horizontal translation by which to move.</param>
        /// <param name="dz">The z-component of the horizontal translation by which to move.</param>
        /// <returns>The change in height effected during the move.</returns>
        public float move(float dx, float dz) {
            if (grounded.value) {
                Vector3 position = base.Position;
                // Calculate the z-translation based on the terrain.
                float y = gameObject.getTerrain().getHeight(position.X + dx, position.Z + dz);
                float dy = y - position.Y;
                // Apply translation
                Vector3 translation = position; // recycling the Vector3 object
                translation.X = dx;
                translation.Y = dy;
                translation.Z = dz;
                base.move(translation);
                return dy;
            } else {
                base.move(new Vector3(dx, 0, dz));
                return 0;
            }
        }

        public override Vector3 Position {
            get { return base.Position; }
            set {
                if (grounded.value) {
                    float x = value.X;
                    float z = value.Z;
                    float y = this.gameObject.getTerrain().getHeight(value.X, value.Z);
                    base.Position = (new Vector3(x, y, z));
                } else {
                    base.Position = (value);
                }
            }
        }

        /// <summary>
        /// Translates this Location.
        /// Ignores the z-component of the translation if this Location is grounded,
        /// and also modifies the translation parameter so that its z-component is zero.
        /// </summary>
        /// <param name="translation">The vector describing the translation by which to move.</param>
        public override void move(Vector3 translation) {
            if (grounded.value) {
                translation.Y = 0;
                this.move(translation.X, translation.Z);
            } else {
                base.move(translation);
            }
        }

        /// <summary>
        /// Moves the specified distance along the horizontal direction specified by the
        /// "x" and "y" parameters. If grounded, also adjusts the height to fix it to the
        /// Terrain. This height adjustment is then included in the specified distance.
        /// </summary>
        /// <param name="dx">The x-component of the horizontal direction in which to move.</param>
        /// <param name="dz">The z-component of the horizontal direction in which to move.</param>
        /// <param name="distance">The total distance to move.</param>
        /// <returns>The change in height effected during the move.</returns>
        public float moveMagn(float dx, float dz, float distance) {
            // TODO
            return 0.0f;
        }

    }

    /**
     * Implemented by GameObjects that have the Location module.
     */
    public interface TerrainLocatable : Locatable {

        /// <summary>
        /// Returns the Location module of this GameObject.
        /// </summary>
        /// <returns>The Location module associated with this GameObject.
        TerrainLocation getTerrainLocation();

        Terrain getTerrain();

    }

}