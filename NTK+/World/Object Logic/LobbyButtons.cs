/*••••••••••••••••••••••••••••••••••••••••*\
| NTK+                                     |
| (C) Copyright Bluestone Coding 2009      |
|••••••••••••••••••••••••••••••••••••••••••|
| Built using the Interaction Engine       |
| (C) Copyright Bluestone Coding 2008-2009 |
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
using InteractionEngine.UserInterface.ThreeDimensional;
using InteractionEngine.Constructs.Datatypes;
using InteractionEngine.UserInterface.Audio;

namespace NTKPlusGame.World {


    public class LobbyButtons : GameObject, Interactable2D, Audible {

        #region FACTORY

        /// <summary>
        /// All GameObjects need a parameterless constructor for calling by GameObject.createGameObject() and GameObject.createFromUpdate().
        /// NEVER CALL THIS! This constructor is exclusively for use by the InteractionEngine. If anyone else calls it things will break.
        /// If you want to construct this object, use GameObject.createGameObject(LoadRegion).
        /// </summary>
        public LobbyButtons() {
        }

        // The classHash, a unique identifying string for the class. Hmm, wow, that's kind of redundant, isn't that? C# already provides such a function through reflection. Oh well.
        // Used for the factory methods called when the client receives a CREATE_NEW_OBJECT update from the server computer.
        public const string realHash = "LobbyButtons";
        public override string classHash {
            get { return realHash; }
        }

        /// <summary>
        /// The static constructor. Adds the class's factory method to the GameObject factoryList when the class is first loaded.
        /// </summary>
        static LobbyButtons() {
            GameObject.factoryList.Add(realHash, new GameObjectFactory(GameObject.createFromUpdate<LobbyButtons>));
        }

        #endregion

        /// <summary>
        /// Constructs a new LobbyButtons.
        /// </summary>
        public override void construct() {
            this.location = new Location(this);
            this.graphics = new Graphics2DTexture(this);
            //Audio
            this.audio = new Audio(this, "Content\\StartTheme", "Content\\StartTheme");
            audio.playSound("02 Chaje Shukarije - Esma Redzepova");
            this.graphics.TextureName = "host+join";
            this.location.Position = new Microsoft.Xna.Framework.Vector3(15f, 565f, 0f);
            // EventMethods
            this.addEventMethod("host", host);
            this.addEventMethod("join", join);
            this.addEventMethod("handleNewPlayer", handleNewPlayer);
            this.addEventMethod("catchHostedLoadRegion", catchHostedLoadRegion);
            this.addEventMethod("handleDroppedConnection", handleDroppedConnection);
        }


        /// <summary>
        /// Returns the Location module of this GameObject.
        /// </summary>
        /// <returns>The Location module associated with this GameObject.
        private Location location;
        public Location getLocation() {
            return location;
        }
        private Audio audio;
        public Audio getAudio() {
            return audio;
        }


        /// <summary>
        /// Returns the Graphics module of this GameObject.
        /// </summary>
        /// <returns>The Graphics module associated with this GameObject.
        private Graphics2DTexture graphics;
        public InteractionEngine.UserInterface.Graphics getGraphics() {
            return graphics;
        }
        public Graphics2D getGraphics2D() {
            return graphics;
        }

        /// <summary>
        /// Tell the Engine what to do when this button is interacted with.
        /// </summary>
        /// <param name="invoker">The invoker constant.</param>
        /// <param name="point">The coordinates of the click.</param>
        /// <returns></returns>
        public Event getEvent(int invoker, Microsoft.Xna.Framework.Vector3 point) {
            if (invoker == UserInterface3D.MOUSEMASK_LEFT_CLICK) {
                if (point.X < 100) return new Event(this.id, "host", null);
                else return new Event(this.id, "join", null);
            } else return null;
        }

        // Contains the LoadRegion within which this game is actually occuring.
        // Used to keep a reference to the 3D GameWorld.
        private LoadRegion hostedRegion;

        #region EventMethods

        /// <summary>
        /// The EventMethod that causes this instance to become a host.
        /// </summary>
        /// <param name="client">The Client that
        /// 
        /// raised this Event.</param>
        /// <param name="parameter">Optional extra information regarding this Event.</param>
        public void host(Client client, object parameter) {
            // Kill any current sessions.
            disconnect();
            // Do the real stuff.
            Engine.status = Engine.Status.MULTIPLAYER_SERVERCLIENT;
            Client.startListening();
            // Set up the LoadRegion.
            hostedRegion = LoadRegion.createLoadRegion();
            // Make sure that we're graphing it locally.
            Client.addPrivateLoadRegion(hostedRegion);
            // Make the GameWorld!
            Terrain terrain = GameObject.createGameObject<Terrain>(hostedRegion);
            Human human = GameObject.createGameObject<Human>(hostedRegion);
            human.initialize(terrain);
            human.getLocation().move(Microsoft.Xna.Framework.Vector3.One*10);
            Human human1 = GameObject.createGameObject<Human>(hostedRegion);
            human1.initialize(terrain);
            Human human2 = GameObject.createGameObject<Human>(hostedRegion);
            human2.initialize(terrain);
            Human human3 = GameObject.createGameObject<Human>(hostedRegion);
            human3.initialize(terrain);
            Human human4 = GameObject.createGameObject<Human>(hostedRegion);
            human4.initialize(terrain);
            FrameRateCounter frameRateCounter = GameObject.createGameObject<FrameRateCounter>(hostedRegion);
            SkyDome skydome = GameObject.createGameObject<SkyDome>(hostedRegion);
            // Send the hosted region to anybody who joins the game
            Client.onJoin.Add(new Event(this.id, "handleNewPlayer", null));
        }
        
        /// <summary>
        /// The EventMethod that causes this instance to join a hosted game.
        /// </summary>
        /// <param name="client">The Client that raised this Event.</param>
        /// <param name="parameter">Optional extra information regarding this Event.</param>
        public void join(Client client, object parameter) {
            // Kill any current sessions.
            disconnect();
            // Do the real stuff.
            Engine.status = Engine.Status.MULTIPLAYER_CLIENT;
            try {
                Engine.server = new Server("127.0.0.1");
                // Make sure we know what region is the hosted region so we can kill it later
                CreateRegion.onCreateRegion.Add(new Event(this.id, "catchHostedLoadRegion", null));
                Server.onDisconnect.Add(new Event(this.id, "handleDroppedConnection", null));
            } catch (GameWorldException) {
                Engine.status = Engine.Status.SINGLE_PLAYER;
            }
        }

        /// <summary>
        /// The EventMethod that causes this host instance to let a new player join the game.
        /// </summary>
        /// <param name="client">The Client that raised this Event (null).</param>
        /// <param name="parameter">Optional extra information regarding this Event (the actual new Client).</param>
        public void handleNewPlayer(Client client, object parameter) {
            if (parameter is Client) {
                Client realClient = (Client)parameter;
                hostedRegion.sentToClient(realClient);
                foreach (GameObjectable gameObject in hostedRegion.getGameObjectArray())
                    realClient.addPermission(gameObject.id);
            }
        }

        /// <summary>
        /// The EventMethod that causes this client instance to catch a new LoadRegion so we know which are hosted (as opposed to local).
        /// </summary>
        /// <param name="client">The Client that raised this Event (null).</param>
        /// <param name="parameter">Optional extra information regarding this Event (the new LoadRegion).</param>
        public void catchHostedLoadRegion(Client client, object parameter) {
            if (parameter is LoadRegion) hostedRegion = (LoadRegion)parameter;
        }

        /// <summary>
        /// The EventMethod that causes this client instance to handle a dropped connection.
        /// </summary>
        /// <param name="client">The Client that raised this Event (null).</param>
        /// <param name="parameter">Optional extra information regarding this Event (null).</param>
        public void handleDroppedConnection(Client client, object parameter) {
            disconnect();
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Whether you are a client or server, this method will terminate networking.
        /// Note that you (or the Engine itself) will still need to reset the Engine.status.
        /// </summary>
        public void disconnect() {
            Client.stopListening();
            if (hostedRegion == null) return;
            hostedRegion.deconstruct();
            hostedRegion = null;
            Client.onJoin.Clear();
            CreateRegion.onCreateRegion.Clear();
        }

        #endregion

    }

}