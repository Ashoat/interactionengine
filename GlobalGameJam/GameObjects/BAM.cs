using InteractionEngine.Constructs;
using InteractionEngine.UserInterface;
using Microsoft.Xna.Framework.Input;
using GlobalGameJam.Graphics;
using InteractionEngine.UserInterface.TwoDimensional;
using Microsoft.Xna.Framework;
using InteractionEngine.Constructs.Datatypes;
using InteractionEngine.Networking;
using InteractionEngine;
using InteractionEngine.EventHandling;
using System.Collections.Generic;

namespace GlobalGameJam.GameObjects {

    public class BAM : GameObject, Graphable2D {
        
        #region FACTORY

        /// <summary>
        /// All GameObjects need a parameterless constructor for calling by GameObject.createGameObject() and GameObject.createFromUpdate().
        /// NEVER CALL THIS! This constructor is exclusively for use by the InteractionEngine. If anyone else calls it things will break.
        /// If you want to construct this object, use GameObject.createGameObject(LoadRegion).
        /// </summary>
        public BAM() {
        }

        // The classHash, a unique identifying string for the class. Hmm, wow, that's kind of redundant, isn't that? C# already provides such a function through reflection. Oh well.
        // Used for the factory methods called when the client receives a CREATE_NEW_OBJECT update from the server computer.
        public const string realHash = "BAM";
        public override string classHash {
            get { return realHash; }
        }

        /// <summary>
        /// The static constructor. Adds the class's factory method to the GameObject factoryList when the class is first loaded.
        /// </summary>
        static BAM() {
            GameObject.factoryList.Add(realHash, new GameObjectFactory(GameObject.createFromUpdate<BAM>));
        }

        #endregion

        private UpdatableInteger lifeSpan;

        public bool isDisplaying() {
            return lifeSpan.value > 0;
        }

        Location location;
        public Location getLocation() {
            return location;
        }

        Graphics2DTexture graphics;
        public InteractionEngine.UserInterface.Graphics getGraphics() {
            return graphics;
        }
        public Graphics2D getGraphics2D() {
            return graphics;
        }

        public override void construct() {
            this.location = new Location(this);
            this.graphics = new Graphics2DTexture(this, "bam");
            this.lifeSpan = new UpdatableInteger(this);
            this.addEventMethod("tick", tick);
        }

        public void setLocationAndLifespan(Vector3 position, int lifeSpan) {
            this.location.Position = position;
            this.lifeSpan.value = lifeSpan;
            this.graphics.Visible = true;
            Engine.addEvent(new Event(this.id, "tick", null));
        }

        public void tick(Client client, object param) {
            this.lifeSpan.value -= Engine.gameTime.ElapsedGameTime.Milliseconds;
            if (this.lifeSpan.value < 0) {
                this.graphics.Visible = false;
                this.deconstruct();
            } else Engine.addEvent(new Event(this.id, "tick", null));
        }

    }

}