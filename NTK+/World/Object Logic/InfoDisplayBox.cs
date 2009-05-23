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
| * InfoDisplayBox                   Class |
\*••••••••••••••••••••••••••••••••••••••••*/

using InteractionEngine.Constructs;
using InteractionEngine;
using NTKPlusGame.World.Modules;
using InteractionEngine.UserInterface;
using InteractionEngine.EventHandling;
using InteractionEngine.Networking;

namespace NTKPlusGame.World {


    public class InfoDisplayBox : GameObject, Locatable { //, Graphable {

        #region FACTORY

        /// <summary>
        /// All GameObjects need a parameterless constructor for calling by GameObject.createGameObject() and GameObject.createFromUpdate().
        /// NEVER CALL THIS! This constructor is exclusively for use by the InteractionEngine. If anyone else calls it things will break.
        /// If you want to construct this object, use GameObject.createGameObject(LoadRegion).
        /// </summary>
        public InfoDisplayBox() {
        }

        // The classHash, a unique identifying string for the class. Hmm, wow, that's kind of redundant, isn't that? C# already provides such a function through reflection. Oh well.
        // Used for the factory methods called when the client receives a CREATE_NEW_OBJECT update from the server computer.
        public const string realHash = "InfoDisplayBox";
        public override string classHash {
            get { return realHash; }
        }

        /// <summary>
        /// The static constructor. Adds the class's factory method to the GameObject factoryList when the class is first loaded.
        /// </summary>
        static InfoDisplayBox() {
            GameObject.factoryList.Add(realHash, new GameObjectFactory(GameObject.createFromUpdate<InfoDisplayBox>));
        }

        #endregion

        public const string DESCRIPTION_CHANGE_EVENT_HASH = "description change";

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

        /// <summary>
        /// Constructs a new InfoDisplayBox.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        public override void construct() {
            this.location = new Location(this);
            this.graphics = null; // TODO
            this.addEventMethod(DESCRIPTION_CHANGE_EVENT_HASH, new EventMethod(changeActiveDescription));
        }

        public void changeActiveDescription(Client client, object param) {
            string description = (string)param;
            // TODO
        }

        // TODO

    }

}