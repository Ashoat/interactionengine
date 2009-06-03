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
| * Human                            Class |
\*••••••••••••••••••••••••••••••••••••••••*/

using InteractionEngine.Constructs;
using NTKPlusGame.World.Modules;
using InteractionEngine.UserInterface.ThreeDimensional;
using Microsoft.Xna.Framework;
using InteractionEngine.Networking;
using InteractionEngine.UserInterface;
using InteractionEngine.EventHandling;
using Microsoft.Xna.Framework.Graphics;
using InteractionEngine;

namespace NTKPlusGame.World {


    public class SkyDome : GameObject, Graphable3D {


        #region FACTORY

        /// <summary>
        /// All GameObjects need a parameterless constructor for calling by GameObject.createGameObject() and GameObject.createFromUpdate().
        /// NEVER CALL THIS! This constructor is exclusively for use by the InteractionEngine. If anyone else calls it things will break.
        /// If you want to construct this object, use GameObject.createGameObject(LoadRegion).
        /// </summary>
        public SkyDome() {
        }

        // The classHash, a unique identifying string for the class. Hmm, wow, that's kind of redundant, isn't that? C# already provides such a function through reflection. Oh well.
        // Used for the factory methods called when the client receives a CREATE_NEW_OBJECT update from the server computer.
        public const string realHash = "SkyDome";
        public override string classHash {
            get { return realHash; }
        }

        /// <summary>
        /// The static constructor. Adds the class's factory method to the GameObject factoryList when the class is first loaded.
        /// </summary>
        static SkyDome() {
            GameObject.factoryList.Add(realHash, new GameObjectFactory(GameObject.createFromUpdate<SkyDome>));
        }

        #endregion

        //   private UpdatableGameObject<DebugSphere> debugSphere;

        private Location location;
        public Location getLocation() {
            return location;
        }

        /// <summary>
        /// Returns the Graphics module of this GameObject.
        /// </summary>
        /// <returns>The Selection module associated with this GameObject.
        private Graphics3DModel graphics;
        public Graphics getGraphics() {
            return graphics;
        }
        public Graphics3D getGraphics3D() {
            return graphics;
        }

        private InfoDisplay infoDisplay;
        public InfoDisplay getInfoDisplay() {
            return infoDisplay;
        }

        public override void construct() {
            this.location = new Location(this);
            this.location.Position = new Vector3(0, -1500, 0);
            ModelEffect modelEffect = new ModelEffect(); // new Graphics3D.ModelEffect(UserInterface3D.graphicsDevice, null);
            modelEffect.AmbientLightColor = new Vector3(0.6f, 0.6f, 0.6f);
            modelEffect.setTextureName("Images\\cloudMap");
            modelEffect.TextureEnabled = true;
            //modelEffect.Texture = null;
            modelEffect.CommitProperties();
            this.graphics = new Graphics3DModel(this, modelEffect, "Models\\dome");
            this.graphics.SetScale(7000f);
        }
    }

}