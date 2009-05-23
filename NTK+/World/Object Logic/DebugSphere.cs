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
using InteractionEngine.GameWorld;
using NTKPlusGame.World.Modules;
using InteractionEngine.Client.ThreeDimensional;
using InteractionEngine.Client;
using Microsoft.Xna.Framework;

namespace NTKPlusGame.World {

    /**
     * Template
     */
    public class DebugSphere : GameObject, Graphable3D {

        #region FACTORY

        // The classHash, a unique identifying string for the class. Hmm, wow, that's kind of redundant, isn't that? C# already provides such a function through reflection. Oh well.
        // Used for the factory methods called when the client receives a CREATE_NEW_OBJECT update from the server computer.
        internal const string classHash = "DebugSphere";

        /// <summary>
        /// The static constructor. Adds the class's factory method to the GameObject factoryList when the class is first loaded.
        /// </summary>
        static DebugSphere() {
            GameObject.factoryList.Add(classHash, new GameObjectFactory(makeDebugSphere));
        }

        /// <summary>
        /// A factory method that creates and returns a new instance of DebugSphere. Used by the client when the server requests it to make a new GameObject.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        /// <param name="id">This GameObject's ID.</param>
        /// <param name="reader">The PacketReader from which we will read the fields of the newly constructed GameObject.</param>
        /// <returns>A new instance of DebugSphere.</returns>
        static DebugSphere makeDebugSphere(LoadRegion loadRegion, int id, Microsoft.Xna.Framework.Net.PacketReader reader) {
            if (GameWorld.status != GameWorld.Status.MULTIPLAYER_CLIENT)
                throw new System.Exception("You're not a client, so why are you calling the GameObject factory method?");
            DebugSphere gameObject = new DebugSphere(loadRegion, id);
            // ORDER OF STUFF (where you used the reader to construct datatypes, used factory methods exclusively. also, construct modules and their datatypes here too.)
            return gameObject;
        }

        /// <summary>
        /// Constructs a GameObject and assigns it an ID.
        /// This is the constructor that should be used if and only if you are a MULTIPLAYER_CLIENT.
        /// Furthermore, it is only called by the GameObjectFactory method.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        /// <param name="id">This GameObject's ID.</param>
        private DebugSphere(LoadRegion loadRegion, int id)
            : base(loadRegion, id) {
        }

        /// <summary>
        /// Returns the class hash. 
        /// </summary>
        /// <returns>The class hash. Do we really have to tell you everything twice?</returns>
        public override string getClassHash() {
            return classHash;
        }

        #endregion

        private readonly Location location;
        public Location getLocation() {
            return location;
        }

        private readonly Graphics3D graphics3D;
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
        public DebugSphere(LoadRegion loadRegion, Vector3 position, float radius)
            : base(loadRegion) {
            this.location = new Location(this);
            ModelEffect modelEffect = new ModelEffect();
            modelEffect.SpecularColor = new Vector3(.1f, .3f, .6f);
            modelEffect.SpecularPower = 1f;
            modelEffect.CommitProperties();
            this.graphics3D = new Graphics3D(this, modelEffect, "Models\\sphere180");
            this.location.move(position);
            this.graphics3D.SetScale(radius);
        }

        public void setPosition(Vector3 position, float radius) {
            this.location.moveTo(position);
            this.graphics3D.SetScale(radius);
        }

    }

}