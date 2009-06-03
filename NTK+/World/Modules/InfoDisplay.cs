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
| * InfoDisplay                 Class      |
| * InfoDisplayable             Interface  |
| * InfoTab                     Class      |
| * InfoButton                  GameObject |
| * InfoDisplayBox              GameObject |
\*••••••••••••••••••••••••••••••••••••••••*/

using InteractionEngine.Constructs.Datatypes;
using InteractionEngine.Constructs;
using InteractionEngine;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using InteractionEngine.UserInterface;
using InteractionEngine.UserInterface.ThreeDimensional;
using System;
using InteractionEngine.EventHandling;
using Microsoft.Xna.Framework.Graphics;
using InteractionEngine.Networking;

namespace NTKPlusGame.World.Modules {

    /**
     * Holds all information and methods regarding InfoDisplay.
     * That is, all information necessary for the GameObject to display its information in a handy little infobox.
     */

    public class InfoDisplay {

        // Contains a reference to the GameObject this InfoDisplay module is associated with.
        // Used for constructing Updatables.
        private readonly InfoDisplayable gameObject;
        // Contains the name of the image resource used to display the face icon.
        // We can dynamically change this?
        private readonly UpdatableString faceIcon;
        // Contains the in-game display name of the GameObject.
        private readonly UpdatableString displayName;
        // Contains the in-game description of the GameObject.
        private readonly UpdatableString description;

        private const int MAXIMUM_TABS = 5;
        private const int tabWidth = 100;
        // TODO: Updatable?
        private readonly UpdatableGameObject<InfoTab>[] tabs = new UpdatableGameObject<InfoTab>[MAXIMUM_TABS];
        private readonly UpdatableGameObject<InfoTab> activeTab;
        private int tabCount = 0;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="gameObject">The GameObject whose InfoDisplay this is.</param>
        public InfoDisplay(InfoDisplayable gameObject) {
            this.gameObject = gameObject;
            this.faceIcon = new UpdatableString(gameObject);
            this.displayName = new UpdatableString(gameObject);
            this.description = new UpdatableString(gameObject);
            this.displayName.value = "";
            this.description.value = "";
            this.activeTab = new UpdatableGameObject<InfoTab>(gameObject);
            for (int i = 0; i < MAXIMUM_TABS; i++) {
                tabs[i] = new UpdatableGameObject<InfoTab>(gameObject);
            }
        }

        public string FaceIcon {
            get { return this.faceIcon.value; }
            set { this.faceIcon.value = value; }
        }
        public string DisplayName {
            get { return this.displayName.value; }
            set { this.displayName.value = value; }
        }
        public string Description {
            get { return this.description.value; }
            set { this.description.value = value; }
        }

        public void addTab(InfoTab tab) {
            if (tabCount == MAXIMUM_TABS) throw new Exception("Too many InfoTabs on one GameObject!");
            tab.getLocation().Position = calculatePositionOfTab(tabCount);
            tabs[tabCount++].value = tab;
        }

        public void setActive(bool active) {
            foreach (UpdatableGameObject<InfoTab> tab in this.tabs) {
                if (tab.value != null) tab.value.getGraphics2D().Visible = active;
            }
        }

        public void setActiveTab(InfoTab tab) {
            if (this.activeTab.value != null) this.activeTab.value.setActive(false);
            tab.setActive(true);
            this.activeTab.value = tab;
        }

        private Vector3 calculatePositionOfTab(int tabCount) {
            return new Vector3(100 + tabWidth * tabCount, 400, 0);
        }

    }

    public class InfoTab : GameObject, Graphable2D, Interactable2D {
        
        #region FACTORY

        /// <summary>
        /// All GameObjects need a parameterless constructor for calling by GameObject.createGameObject() and GameObject.createFromUpdate().
        /// NEVER CALL THIS! This constructor is exclusively for use by the InteractionEngine. If anyone else calls it things will break.
        /// If you want to construct this object, use GameObject.createGameObject(LoadRegion).
        /// </summary>
        public InfoTab() {
        }

        // The classHash, a unique identifying string for the class. Hmm, wow, that's kind of redundant, isn't that? C# already provides such a function through reflection. Oh well.
        // Used for the factory methods called when the client receives a CREATE_NEW_OBJECT update from the server computer.
        public const string realHash = "InfoTab";
        public override string classHash {
            get { return realHash; }
        }

        /// <summary>
        /// The static constructor. Adds the class's factory method to the GameObject factoryList when the class is first loaded.
        /// </summary>
        static InfoTab() {
            GameObject.factoryList.Add(realHash, new GameObjectFactory(GameObject.createFromUpdate<InfoTab>));
        }

        #endregion

        private const int MAXIMUM_BUTTONS = 10;
        private const int BUTTON_LENGTH = 50;
        private const int BUTTON_PADDING = 10;
        private static readonly Vector2 tabPaneOrigin = new Vector2(120, 440);

        private const string TAB_CHANGE_HASH = "tab change";

        public string name;
        // TODO: Updatable?
        private readonly UpdatableGameObject<InfoButton>[] buttons = new UpdatableGameObject<InfoButton>[MAXIMUM_BUTTONS];
        private int buttonCount = 0;
        private InfoDisplayable gameObject;

        private Location location;
        public Location getLocation() {
            return location;
        }

        private InfoTabGraphics graphics;
        public Graphics getGraphics() {
            return graphics;
        }
        public Graphics2D getGraphics2D() {
            return graphics;
        }

        public override void construct() {
            this.location = new Location(this);
            this.graphics = new InfoTabGraphics(this);
            this.graphics.Visible = false;
            for (int i = 0; i < MAXIMUM_BUTTONS; i++) {
                this.buttons[i] = new UpdatableGameObject<InfoButton>(this);
            }
            this.addEventMethod(TAB_CHANGE_HASH, new EventMethod(onClicked));
        }

        public void initialize(string name, InfoDisplayable gameObject) {
            this.name = name;
            this.gameObject = gameObject;
        }

        public void addInfoButton(InfoButton button) {
            if (buttonCount == MAXIMUM_BUTTONS) throw new Exception("Too many buttons in a tab!");
            button.getLocation().Position = (calculatePositionOfButton(buttonCount));
            buttons[buttonCount++].value = button;
        }

        private Vector3 calculatePositionOfButton(int index) {
            float x = tabPaneOrigin.X + (BUTTON_LENGTH+BUTTON_PADDING)*(index%5);
            float y = tabPaneOrigin.Y + (BUTTON_LENGTH+BUTTON_PADDING)*(index/5);
            return new Vector3(x, y, 0);
        }

        public virtual void setActive(bool active) {
            foreach (UpdatableGameObject<InfoButton> button in buttons) {
                if (button.value != null) button.value.getGraphics2D().Visible = active;
            }
        }

        public void onClicked(Client client, object param) {
            gameObject.getInfoDisplay().setActiveTab(this);
        }

        public Event getEvent(int invoker, Vector3 coordinates) {
            if (invoker == UserInterface3D.MOUSEMASK_LEFT_CLICK) {
                return new Event(this.id, TAB_CHANGE_HASH, null);
            }
            return null;
        }

        class InfoTabGraphics : Graphics2DTexture {

            private readonly InfoTab infoTab;
            private SpriteFont spriteFont;

            public InfoTabGraphics(InfoTab gameObject)
                : base(gameObject, "InfoBox\\InfoTab") {
                this.infoTab = gameObject;
                this.LayerDepth = 0.25f;
            }

            public override void onDraw() {
                base.onDraw();
                if (this.Visible) {
                    SpriteBatch spriteBatch = ((UserInterface3D)(Engine.userInterface)).spriteBatch;
                    Vector3 position3 = infoTab.getLocation().Position;
                    Vector2 position = new Vector2(position3.X + 5, position3.Y + 3);
                    spriteBatch.DrawString(spriteFont, infoTab.name, position, Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                }
            }

            public override void loadContent() {
                base.loadContent();
                this.spriteFont = UserInterface3D.content.Load<SpriteFont>("SpriteFont1");
            }

        }

    }

    public class InfoButton : GameObject, Interactable2D {

        #region FACTORY

        /// <summary>
        /// All GameObjects need a parameterless constructor for calling by GameObject.createGameObject() and GameObject.createFromUpdate().
        /// NEVER CALL THIS! This constructor is exclusively for use by the InteractionEngine. If anyone else calls it things will break.
        /// If you want to construct this object, use GameObject.createGameObject(LoadRegion).
        /// </summary>
        public InfoButton() {
        }

        // The classHash, a unique identifying string for the class. Hmm, wow, that's kind of redundant, isn't that? C# already provides such a function through reflection. Oh well.
        // Used for the factory methods called when the client receives a CREATE_NEW_OBJECT update from the server computer.
        public const string realHash = "InfoButton";
        public override string classHash {
            get { return realHash; }
        }

        /// <summary>
        /// The static constructor. Adds the class's factory method to the GameObject factoryList when the class is first loaded.
        /// </summary>
        static InfoButton() {
            GameObject.factoryList.Add(realHash, new GameObjectFactory(GameObject.createFromUpdate<InfoButton>));
        }

        #endregion

        private GameObject gameObject;
        private string onClickHash;
        private string name;
        private string description;
        private object param;

        /// <summary>
        /// Returns the Location module of this GameObject.
        /// </summary>
        /// <returns>The Location module associated with this GameObject.
        private Location location;
        public Location getLocation() {
            return location;
        }

        /// <summary>
        /// Returns the Graphics module of this GameObject.
        /// </summary>
        /// <returns>The Graphics module associated with this GameObject.
        private InteractionEngine.UserInterface.ThreeDimensional.Graphics2DTexture graphics2D;
        public InteractionEngine.UserInterface.Graphics getGraphics() {
            return graphics2D;
        }
        public InteractionEngine.UserInterface.ThreeDimensional.Graphics2D getGraphics2D() {
            return graphics2D;
        }

        public override void construct() {
            this.location = new Location(this);
            this.graphics2D = new Graphics2DTexture(this);
            this.graphics2D.Visible = false;
        }
            
        public void initialize(GameObject gameObject, string onClickHash, object param, string imageResource, string name, string description) {
            this.gameObject = gameObject;
            this.onClickHash = onClickHash;
            this.param = param;
            this.name = name;
            this.description = description;
            this.graphics2D.TextureName = imageResource;
        }

        public Event getEvent(int invoker, Vector3 coordinates) {
            if (invoker == UserInterface3D.MOUSEMASK_OVER) {
                return new Event(NTKPlusUser.localUser.infoDisplayBox.id, InfoDisplayBox.DESCRIPTION_CHANGE_EVENT_HASH, this.name + "\n" + this.description);
            }
            else if (invoker == UserInterface3D.MOUSEMASK_OUT) {
                return new Event(NTKPlusUser.localUser.infoDisplayBox.id, InfoDisplayBox.DESCRIPTION_CHANGE_EVENT_HASH, null);
            }
            else if (invoker == UserInterface3D.MOUSEMASK_LEFT_CLICK) {
                return new Event(gameObject.id, onClickHash, param);
            }
            return null;
        }

    }

    /**
     * Implemented by GameObjects that have the InfoDisplay module.
     */
    public interface InfoDisplayable : Selectable {

        /// <summary>
        /// Returns the InfoDisplay module of this GameObject.
        /// </summary>
        /// <returns>The InfoDisplay module associated with this GameObject.
        InfoDisplay getInfoDisplay();

    }

}