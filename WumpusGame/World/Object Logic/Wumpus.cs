/*••••••••••••••••••••••••••••••••••••••••*\
| Wumpus Game                              |
| (C) Copyright Bluestone Coding 2008      |
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
| * Wumpus                           Class |
\*••••••••••••••••••••••••••••••••••••••••*/

using InteractionEngine.Constructs;
using InteractionEngine.GameWorld;
using WumpusGame.World.Modules;
using WumpusGame.Modules;
using InteractionEngine.Constructs.Datatypes;
using InteractionEngine.Client;
using WumpusGame.World.Graphics;

namespace WumpusGame.World {

    /**
     * The Wumpus. It's this little innocent creature that gets poached by cruel hunters, who are played by gamers.
     */

    public class Wumpus : GameObject, Conversable, Graphable, Locatable, Triviable {

#region FACTORY

		// The classHash, a unique identifying string for the class. Hmm, wow, that's kind of redundant, isn't that? C# already provides such a function through reflection. Oh well.
		// Used for the factory methods called when the client receives a CREATE_NEW_OBJECT update from the server computer.
		internal const string classHash = "Wumpus";

		/// <summary>
		/// A factory method that creates and returns a new instance of Wumpus. Used by the client when the server requests it to make a new GameObject.
		/// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        /// <param name="id">This GameObject's ID.</param>
        /// <param name="reader">The PacketReader from which we will read the fields of the newly constructed GameObject.</param>
        /// <returns>A new instance of Wumpus.</returns>
        static Wumpus makeWumpus(LoadRegion loadRegion, int id, Microsoft.Xna.Framework.Net.PacketReader reader) {
            if (GameWorld.status != GameWorld.Status.MULTIPLAYER_CLIENT)
                throw new System.Exception("You're not a client, so why are you calling the GameObject factory method?");
            Wumpus wumpus = new Wumpus(loadRegion, id);
            // ORDER OF STUFF (where you used the reader to construct datatypes, used factory methods exclusively. also, construct modules and their datatypes here too.)
            return wumpus;
        }

        /// <summary>
        /// Constructs a GameObject and assigns it an ID.
        /// This is the constructor that should be used if and only if you are a MULTIPLAYER_CLIENT.
        /// Furthermore, it is only called by the GameObjectFactory method.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        /// <param name="id">This GameObject's ID.</param>
        private Wumpus(LoadRegion loadRegion, int id) : base(loadRegion, id) {
        }

        /// <summary>
        /// The static constructor. Adds the class's factory method to the GameObject factoryList when the class is first loaded.
        /// </summary>
        static Wumpus() {
            GameObject.factoryList.Add(classHash, new GameObjectFactory(makeWumpus));
        }

        /// <summary>
        /// Returns the class hash. 
        /// </summary>
        /// <returns>The class hash. Do we really have to tell you everything twice?</returns>
        public override string getClassHash() {
            return classHash;
        }

#endregion

        static readonly System.Random randy = new System.Random();

        /*••••••••••••••••••••••••••••••••••••••••*\
          MODULES
        \*••••••••••••••••••••••••••••••••••••••••*/

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
        private Wumpus2DGraphics graphics;
        public InteractionEngine.Client.Graphics getGraphics() {
            return graphics;
        }

        /// <summary>
        /// Returns the Conversation module of this GameObject.
        /// </summary>
        /// <returns>The Conversation module associated with this GameObject.
        private Conversation conversation;
        public Conversation getConversation() {
            return conversation;
        }

        /// <summary>
        /// Constructs a new Wumpus object.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        public Wumpus(LoadRegion loadRegion) : base(loadRegion) {
            location = new Location(this);
            graphics = new Wumpus2DGraphics(this);
            conversation = new Conversation(this, "WumpusFace");
        }

        /*••••••••••••••••••••••••••••••••••••••••*\
          ACTIONS
        \*••••••••••••••••••••••••••••••••••••••••*/

        /// <summary>
        /// The discussion that should be displayed to the player when s/he enters the same room that the Wumpus is in.
        /// </summary>
        private class RoomEnteredDiscussion : Discussion {

#region FACTORY

            // The classHash, a unique identifying string for the class. Hmm, wow, that's kind of redundant, isn't that? C# already provides such a function through reflection. Oh well.
            // Used for the factory methods called when the client receives a CREATE_NEW_OBJECT update from the server computer.
            internal const string classHash = "Wumpus.RoomEnteredDiscussion";

            /// <summary>
            /// The static constructor. Adds the class's factory method to the GameObject factoryList when the class is first loaded.
            /// </summary>
            static RoomEnteredDiscussion() {
                GameObject.factoryList.Add(classHash, new GameObjectFactory(makeRoomEnteredDiscussion));
            }

            /// <summary>
            /// A factory method that creates and returns a new instance of RoomEnteredDiscussion. Used by the client when the server requests it to make a new GameObject.
            /// </summary>
            /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
            /// <param name="id">This GameObject's ID.</param>
            /// <param name="reader">The PacketReader from which we will read the fields of the newly constructed GameObject.</param>
            /// <returns>A new instance of RoomEnteredDiscussion.</returns>
            static RoomEnteredDiscussion makeRoomEnteredDiscussion(LoadRegion loadRegion, int id, Microsoft.Xna.Framework.Net.PacketReader reader) {
                if (GameWorld.status != GameWorld.Status.MULTIPLAYER_CLIENT)
                    throw new System.Exception("You're not a client, so why are you calling the GameObject factory method?");
                RoomEnteredDiscussion roomEnteredDiscussion = new RoomEnteredDiscussion(loadRegion, id);
                // ORDER OF STUFF (where you used the reader to construct datatypes, used factory methods exclusively. also, construct modules and their datatypes here too.)
                return roomEnteredDiscussion;
            }

            /// <summary>
            /// Constructs a GameObject and assigns it an ID.
            /// This is the constructor that should be used if and only if you are a MULTIPLAYER_CLIENT.
            /// Furthermore, it is only called by the GameObjectFactory method.
            /// </summary>
            /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
            /// <param name="id">This GameObject's ID.</param>
            private RoomEnteredDiscussion(LoadRegion loadRegion, int id)
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

            private readonly UpdatableGameObject<Wumpus> wumpy;
            private readonly UpdatableGameObject<Player> playie;

            /// <summary>
            /// Constructs a new ArrowBuyingDiscussion object, and sets its dialogue.
            /// </summary>
            /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
            /// <param name="yuppie">The Player who just wandered into the wrong room.</param>
            /// <param name="parent">The Wumpus who is threatening the Player with death.</param>
            public RoomEnteredDiscussion(LoadRegion loadRegion, Player yuppie, Wumpus parent) : this(loadRegion) {
                wumpy  = new UpdatableGameObject<Wumpus>(this, parent);
                playie = new UpdatableGameObject<Player>(this, yuppie);
            }

            /// <summary>
            /// Constructs a new RoomEnteredDiscussion object, and sets its dialogue.
            /// </summary>
            /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
            public RoomEnteredDiscussion(LoadRegion loadRegion)
                : base(loadRegion) {
                messages = new string[1];
                usingFaces = new bool[] { false };
                options = new string[1][];

                messages[0] = "You see a huddled furry mass... OMG ITSA WUMPUS! The Wumpus twitches its ears up and spins dramatically around to face you. It stomps over to you and looms menacingly over you, pulling you into its shadow. Then it yanks out at your throat and puts a suffocating strangleholds you... Then--mysteriously enough--it starts asking you trivia questions.";
            }

            /// <summary>
            /// Sets this Discussion to start specifying the next message in the Conversation.
            /// Or to close if no more messages remain.
            /// </summary>
            /// <param name="option">The option chosen by the user.</param>
            public override void advanceDiscussion(string option) {
                curMessage.value = -1;
                WumpusGame.World.Pit.PitTrivia trivia = new WumpusGame.World.Pit.PitTrivia(wumpy.value.loadRegion);
                trivia.startTrivia((Triviable)wumpy.value, playie.value, 5, 3);
                wumpy.value.getConversation().startConversation(playie.value, trivia);
            }

        }
        
        /// <summary>
        /// The Trivia that the Player has to answer in order to escape the Wumpus.
        /// </summary>
        private class WumpusTrivia : Trivia {

#region FACTORY

            // The classHash, a unique identifying string for the class. Hmm, wow, that's kind of redundant, isn't that? C# already provides such a function through reflection. Oh well.
            // Used for the factory methods called when the client receives a CREATE_NEW_OBJECT update from the server computer.
            internal const string classHash = "WumpusTrivia";

            /// <summary>
            /// The static constructor. Adds the class's factory method to the GameObject factoryList when the class is first loaded.
            /// </summary>
            static WumpusTrivia() {
                GameObject.factoryList.Add(classHash, new GameObjectFactory(makeWumpusTrivia));
            }

            /// <summary>
            /// A factory method that creates and returns a new instance of WumpusTrivia. Used by the client when the server requests it to make a new GameObject.
            /// </summary>
            /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
            /// <param name="id">This GameObject's ID.</param>
            /// <param name="reader">The PacketReader from which we will read the fields of the newly constructed GameObject.</param>
            /// <returns>A new instance of WumpusTrivia.</returns>
            static WumpusTrivia makeWumpusTrivia(LoadRegion loadRegion, int id, Microsoft.Xna.Framework.Net.PacketReader reader) {
                if (GameWorld.status != GameWorld.Status.MULTIPLAYER_CLIENT)
                    throw new System.Exception("You're not a client, so why are you calling the GameObject factory method?");
                WumpusTrivia arrowBuyingTrivia = new WumpusTrivia(loadRegion, id);
                // ORDER OF STUFF (where you used the reader to construct datatypes, used factory methods exclusively. also, construct modules and their datatypes here too.)
                return arrowBuyingTrivia;
            }

            /// <summary>
            /// Constructs a GameObject and assigns it an ID.
            /// This is the constructor that should be used if and only if you are a MULTIPLAYER_CLIENT.
            /// Furthermore, it is only called by the GameObjectFactory method.
            /// </summary>
            /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
            /// <param name="id">This GameObject's ID.</param>
            private WumpusTrivia(LoadRegion loadRegion, int id)
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

            /// <summary>
            /// Constructs a new WumpusTrivia object, and sets its dialogue.
            /// </summary>
            /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
            public WumpusTrivia(LoadRegion loadRegion) : base(loadRegion) {
                messages = new string[1];
                usingFaces = new bool[] { false };
                options = new string[1][];
                correctAnswers = new string[1];

                messages[0] = "What is the answer to this question?";
                options[0] = new string[] { "The correct answer", "A wrong answer", "A pretty stupid answer", "Whatever" };
                correctAnswers[0] = options[0][0];

            }

        }

        /// <summary>
        /// This method handles the objectEnteredRoom event. If it's a player, then it proceeds to go into the trivia process.
        /// </summary>
        /// <param name="user">The object that entered the room.</param>
        public void playerEntersRoom(Locatable gameObject) {
            if (gameObject is Playable) {
                this.conversation.startConversation((Playable)gameObject, new RoomEnteredDiscussion(loadRegion, (Player)gameObject, this));
            }
        }

        /**
         * This method is called after the trivia session is over.
         * @param user The user the trivia session happened to.
         * @param result True is the player succeeds; false if they failed.
         */
        public void triviaCompleted(Playable user, bool result) {
            if (result) {
                int moves = randy.Next(2,5);
                Room newRoom;
                do { newRoom = this.getLocation().getRoomLocation().getRandomRoomByDistance(moves); }
                while (newRoom.contains<Player>());
                this.location.move(newRoom);
            } else {
                // TODO: user.die();
            }
        }

    }

}