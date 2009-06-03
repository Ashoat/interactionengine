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


    public class Human : WalkerTemplate, InfoDisplayable {


        #region FACTORY

        /// <summary>
        /// All GameObjects need a parameterless constructor for calling by GameObject.createGameObject() and GameObject.createFromUpdate().
        /// NEVER CALL THIS! This constructor is exclusively for use by the InteractionEngine. If anyone else calls it things will break.
        /// If you want to construct this object, use GameObject.createGameObject(LoadRegion).
        /// </summary>
        public Human() {
        }

        // The classHash, a unique identifying string for the class. Hmm, wow, that's kind of redundant, isn't that? C# already provides such a function through reflection. Oh well.
        // Used for the factory methods called when the client receives a CREATE_NEW_OBJECT update from the server computer.
        public const string realHash = "Human";
        public override string classHash {
            get { return realHash; }
        }

        /// <summary>
        /// The static constructor. Adds the class's factory method to the GameObject factoryList when the class is first loaded.
        /// </summary>
        static Human() {
            GameObject.factoryList.Add(realHash, new GameObjectFactory(GameObject.createFromUpdate<Human>));
        }

        #endregion

        private const string BUTTON_HASH = "kablammy!";

        //   private UpdatableGameObject<DebugSphere> debugSphere;

        /// <summary>
        /// Returns the Graphics module of this GameObject.
        /// </summary>
        /// <returns>The Selection module associated with this GameObject.
        private Graphics3DModel graphics;
        public override Graphics getGraphics() {
            return graphics;
        }
        public override Graphics3D getGraphics3D() {
            return graphics;
        }

        private InfoDisplay infoDisplay;
        public InfoDisplay getInfoDisplay() {
            return infoDisplay;
        }

        public override void construct() {
            // blah
            CreateObject.onCreateObject.Add(new Event(this.id, Human.INFO_HASH, null));
            base.construct();
            ModelEffect modelEffect = new ModelEffect(); // new Graphics3D.ModelEffect(UserInterface3D.graphicsDevice, null);
            modelEffect.SpecularColor = new Vector3(.1f, .3f, .6f);
            modelEffect.SpecularPower = 10f;
            modelEffect.setTextureName("Images\\human_texture");
            modelEffect.TextureEnabled = true;
            //modelEffect.Texture = null;
            modelEffect.SpecularPower = 1f;
            modelEffect.CommitProperties();
            this.graphics = new Graphics3DModel(this, modelEffect, "Models\\Borat");
            this.graphics.SetScale(3f);
            this.getLocation().yaw = 180;

            this.infoDisplay = new InfoDisplay(this);

            
            //button5.initialize(this, BUTTON_HASH, 4, "genericButton", "Do generic stuff 5!", "Attack: none! Defense: A button!");
            /*
            tab1.addInfoButton(button1);
            tab1.addInfoButton(button2);
            tab2.addInfoButton(button3);
            tab2.addInfoButton(button4);
            tab2.addInfoButton(button5);
            this.infoDisplay.addTab(tab1);
            this.infoDisplay.addTab(tab2);
            */

            InfoTab tab1 = GameObject.createGameObject<InfoTab>(this.getLoadRegion());
            if (tab1 != null) this.initializeInfoDisplay(null, tab1);
            //tab1.initialize("A tab!", this);

            InfoTab tab2 = GameObject.createGameObject<InfoTab>(this.getLoadRegion());
            if (tab2 != null) this.initializeInfoDisplay(null, tab2);
            //tab2.initialize("Another tab!", this);

            InfoButton button1 = GameObject.createGameObject<InfoButton>(this.getLoadRegion());
            if (button1 != null) this.initializeInfoDisplay(null, button1);
            //button1.initialize(this, BUTTON_HASH, 0, "genericButton", "Do generic stuff 1!", "Attack: none! Defense: A button!");

            InfoButton button2 = GameObject.createGameObject<InfoButton>(this.getLoadRegion());
            if (button2 != null) this.initializeInfoDisplay(null, button2);
            //button2.initialize(this, BUTTON_HASH, 1, "genericButton", "Do generic stuff 2!", "Attack: none! Defense: A button!");

            InfoButton button3 = GameObject.createGameObject<InfoButton>(this.getLoadRegion());
            if (button3 != null) this.initializeInfoDisplay(null, button3);
            //button3.initialize(this, BUTTON_HASH, 2, "genericButton", "Do generic stuff 3!", "Attack: none! Defense: A button!");

            InfoButton button4 = GameObject.createGameObject<InfoButton>(this.getLoadRegion());
            if (button4 != null) this.initializeInfoDisplay(null, button4);
            //button4.initialize(this, BUTTON_HASH, 3, "genericButton", "Do generic stuff 4!", "Attack: none! Defense: A button!");

            InfoButton button5 = GameObject.createGameObject<InfoButton>(this.getLoadRegion());
            if (button5 != null) this.initializeInfoDisplay(null, button5);

            this.infoDisplay.FaceIcon = "Images\\human_texture";
            this.infoDisplay.DisplayName = "Human";
            this.infoDisplay.Description = "Homo sapiens sapiens. With rectangular edges.";

            this.addEventMethod(BUTTON_HASH, new EventMethod(onButton));
            this.addEventMethod(SPHERE_HASH, new EventMethod(initializeSphere));
            this.addEventMethod(INFO_HASH, new EventMethod(initializeInfoDisplay));
        }

        private InfoTab[] tabs = new InfoTab[2];
        private int nextTabIndex = 0;
        private InfoButton[] buttons = new InfoButton[5];
        private int nextButtonIndex = 0;
        public const string INFO_HASH = "info21353egrfg";

        // This initializes the info display stuff from the constructor
        private void initializeInfoDisplay(Client client, object param) {
            if (param is InfoTab) {
                InfoTab tab = (InfoTab)param;
                tabs[nextTabIndex++] = tab;
                this.infoDisplay.addTab(tab);
                tab.initialize(nextTabIndex == 1 ? "A tab!" : "Another tab!", this);
            } else if (param is InfoButton) {
                InfoButton button = ((InfoButton)param); 
                button.initialize(this, BUTTON_HASH, nextButtonIndex, "genericButton", "Do generic stuff " + nextButtonIndex + "!", "Attack: none! Defense: A button!");
                buttons[nextButtonIndex++] = button;
                tabs[nextButtonIndex / 3].addInfoButton(button);
            }
        }

        private Vector3[] colors = { Color.Red.ToVector3(), Color.Yellow.ToVector3(), Color.Green.ToVector3(), Color.Blue.ToVector3(), Color.Purple.ToVector3() };
        private int nextColor;
        private const string SPHERE_HASH = "sphere324h4kj23gadsyt832";
        public void onButton(Client client, object param) {
            DebugSphere sphere = GameObject.createGameObject<DebugSphere>(this.getLoadRegion());
            nextColor = (int)param;
            if (sphere != null) {
                initializeSphere(client, sphere);
            } else CreateObject.onCreateObject.Add(new Event(this.id, SPHERE_HASH, null));
        }

        public void initializeSphere(Client client, object spherey) {
            DebugSphere sphere = (DebugSphere)spherey;
            sphere.setPosition(this.getLocation().Position + new Vector3(0, 25, 0), 2);
            sphere.setColor(colors[nextColor]);
           
        }

        public override Event getEvent(int invoker, Vector3 position) {
            return base.getEvent(invoker, position);
            //this.debugSphere.value.setPosition(this.graphics.BoundingSphere.Center, this.graphics.BoundingSphere.Radius);
        }

    }

}