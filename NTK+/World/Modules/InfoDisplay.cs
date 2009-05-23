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

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="gameObject">The GameObject whose InfoDisplay this is.</param>
        public InfoDisplay(InfoDisplayable gameObject) {
            this.gameObject = gameObject;
            this.faceIcon = new UpdatableString(gameObject);
            this.displayName = new UpdatableString(gameObject);
            this.description = new UpdatableString(gameObject);
        }

        /// <summary>
        /// Constructor for the client-side.
        /// </summary>
        /// <param name="gameObject">The GameObject of whose InfoDisplay this is.</param>
        /// <param name="reader">The reader from which to read teh field datas.</param>
        internal InfoDisplay(GameObject gameObject, Microsoft.Xna.Framework.Net.PacketReader reader) {
            /*byte transferCode = reader.ReadByte();
            UpdatableInteger intty = (UpdatableInteger)Engine.createField(reader);
            roomInfoDisplay = new UpdatableGameObject<Room>(intty);
            if (reader.ReadByte() == Engine.UPDATE_FIELD) Engine.updateField(reader);
            else reader.Position--;
            this.gameObject = gameObject;*/
        }

        // TODO

    }

    public class InfoTab {
        private readonly string name;
        private const int MAXIMUM_BUTTONS = 8;
        // TODO: Updatable?
        private readonly UpdatableGameObject<InfoButton>[] buttons = new UpdatableGameObject<InfoButton>[MAXIMUM_BUTTONS];
        private int buttonCount = 0;

        protected InfoTab(string name, InfoDisplayable gameObject) {
            this.name = name;
            for (int i = 0; i < MAXIMUM_BUTTONS; i++ ) {
                this.buttons[i] = new UpdatableGameObject<InfoButton>(gameObject);
            }
        }

        public void addInfoButton(InfoButton button) {
            if (buttonCount == MAXIMUM_BUTTONS) throw new Exception("Too many buttons in a tab!");
            buttons[buttonCount++].value = button;
        }

        // TODO

    }

    public class InfoButton : GameObject, Interactable {

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
        private string imageResource;
        private string name;
        private string description;

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
        private InteractionEngine.UserInterface.Graphics graphics;
        public InteractionEngine.UserInterface.Graphics getGraphics() {
            return graphics;
        }

        public override void construct() {
            this.location = new Location(this);
            this.graphics = null; // TODO
        }
            
        protected void initialize(GameObject gameObject, string onClickHash, string imageResource, string name, string description) {
            this.gameObject = gameObject;
            this.onClickHash = onClickHash;
            this.imageResource = imageResource;
            this.name = name;
            this.description = description;
        }

        public Event getEvent(int invoker) {
            if (invoker == UserInterface3D.setMask(UserInterface3D.MOUSEMASK_LEFT_PRESS, UserInterface3D.MOUSEMASK_OVER)) {
                return new Event(NTKPlusUser.localUser.infoDisplayBox.id, InfoDisplayBox.DESCRIPTION_CHANGE_EVENT_HASH, this.name + "\n" + this.description);
            }
            if (invoker == UserInterface3D.MOUSEMASK_LEFT_PRESS) {
                return new Event(gameObject.id, onClickHash, null);
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