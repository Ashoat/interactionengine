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
| * DebugSphere                   Class |
\*••••••••••••••••••••••••••••••••••••••••*/

using InteractionEngine.Constructs;
using InteractionEngine;
using NTKPlusGame.World.Modules;
using InteractionEngine.UserInterface.ThreeDimensional;
using InteractionEngine.UserInterface;
using Microsoft.Xna.Framework;

namespace NTKPlusGame.World {

    /**
     * Template
     */
    public class DebugSphere : GameObject, InteractionEngine.UserInterface.ThreeDimensional.Graphable3D {

        #region FACTORY

        /// <summary>
        /// All GameObjects need a parameterless constructor for calling by GameObject.createGameObject() and GameObject.createFromUpdate().
        /// NEVER CALL THIS! This constructor is exclusively for use by the InteractionEngine. If anyone else calls it things will break.
        /// If you want to construct this object, use GameObject.createGameObject(LoadRegion).
        /// </summary>
        public DebugSphere() {
        }

        // The classHash, a unique identifying string for the class. Hmm, wow, that's kind of redundant, isn't that? C# already provides such a function through reflection. Oh well.
        // Used for the factory methods called when the client receives a CREATE_NEW_OBJECT update from the server computer.
        public const string realHash = "DebugSphere";
        public override string classHash {
            get { return realHash; }
        }

        /// <summary>
        /// The static constructor. Adds the class's factory method to the GameObject factoryList when the class is first loaded.
        /// </summary>
        static DebugSphere() {
            GameObject.factoryList.Add(realHash, new GameObjectFactory(GameObject.createFromUpdate<DebugSphere>));
        }

        #endregion

        private Location location;
        public Location getLocation() {
            return location;
        }

        private Graphics3D graphics3D;
        public Graphics3D getGraphics3D() {
            return graphics3D;
        }
        public Graphics getGraphics() {
            return graphics3D;
        }

        /// <summary>
        /// Constructs a new DebugSphere.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        public override void construct() {
            this.location = new Location(this);
            ModelEffect modelEffect = new ModelEffect();
            modelEffect.SpecularColor = new Vector3(.1f, .3f, .3f);
            modelEffect.AmbientLightColor = new Vector3(.6f, .3f, .1f);
            modelEffect.Alpha = 0.5f;
            modelEffect.SpecularPower = 30f;
            modelEffect.CommitProperties();
            this.graphics3D = new Graphics3D(this, modelEffect, "Models\\sphere180");
        }

        public void setPosition(Vector3 position, float radius) {
            this.location.moveTo(position);
            this.graphics3D.SetScale(radius);
        }

    }

}