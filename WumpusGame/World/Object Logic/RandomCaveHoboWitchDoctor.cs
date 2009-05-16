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
| * RandomCaveHoboWitchDoctor        Class |
\*••••••••••••••••••••••••••••••••••••••••*/

using InteractionEngine.Constructs;
using WumpusGame.World.Modules;
using InteractionEngine.Constructs.Datatypes;
using WumpusGame.Modules;
using InteractionEngine.GameWorld;
using WumpusGame.World.Graphics;
using InteractionEngine.Client;

namespace WumpusGame.World {

    public class RandomCaveHoboWitchDoctor : GameObject, Locatable, Graphable, Interactable, Triviable {

#region FACTORY

		// The classHash, a unique identifying string for the class. Hmm, wow, that's kind of redundant, isn't that? C# already provides such a function through reflection. Oh well.
		// Used for the factory methods called when the client receives a CREATE_NEW_OBJECT update from the server computer.
		internal const string classHash = "RandomCaveHoboWitchDoctor";

        /// <summary>
        /// The static constructor. Adds the class's factory method to the GameObject factoryList when the class is first loaded.
        /// </summary>
        static RandomCaveHoboWitchDoctor() {
            GameObject.factoryList.Add(classHash, new GameObjectFactory(makeRandomCaveHoboWitchDoctor));
        }
        
		/// <summary>
		/// A factory method that creates and returns a new instance of RandomCaveHoboWitchDoctor. Used by the client when the server requests it to make a new GameObject.
		/// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        /// <param name="id">This GameObject's ID.</param>
        /// <param name="reader">The PacketReader from which we will read the fields of the newly constructed GameObject.</param>
        /// <returns>A new instance of RandomCaveHoboWitchDoctor.</returns>
        static RandomCaveHoboWitchDoctor makeRandomCaveHoboWitchDoctor(LoadRegion loadRegion, int id, Microsoft.Xna.Framework.Net.PacketReader reader) {
            if (GameWorld.status != GameWorld.Status.MULTIPLAYER_CLIENT)
                throw new System.Exception("You're not a client, so why are you calling the GameObject factory method?");
            RandomCaveHoboWitchDoctor randomCaveHoboWitchDoctor = new RandomCaveHoboWitchDoctor(loadRegion, id);
            // ORDER OF STUFF (where you used the reader to construct datatypes, used factory methods exclusively. also, construct modules and their datatypes here too.)
            return randomCaveHoboWitchDoctor;
        }

        /// <summary>
        /// Constructs a GameObject and assigns it an ID.
        /// This is the constructor that should be used if and only if you are a MULTIPLAYER_CLIENT.
        /// Furthermore, it is only called by the GameObjectFactory method.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        /// <param name="id">This GameObject's ID.</param>
        private RandomCaveHoboWitchDoctor(LoadRegion loadRegion, int id) : base(loadRegion, id) {
        }

        /// <summary>
        /// Returns the class hash. 
        /// </summary>
        /// <returns>The class hash. Do we really have to tell you everything twice?</returns>
        public override string getClassHash() {
            return classHash;
        }

#endregion

        /*••••••••••••••••••••••••••••••••••••••••*\
          MODULES
        \*••••••••••••••••••••••••••••••••••••••••*/

        /**
         * Return the Location module.
         * @return Location The Location module.
         */
        private Location location;
        public Location getLocation() {
            return location;
        }

        /**
         * Return the Graphics module.
         * @return Graphics The Graphics module.
         */
        private RandomCaveHoboWitchDoctor2DGraphics graphics;
        public InteractionEngine.Client.Graphics getGraphics() {
            return graphics;
        }

        /**
         * Return the Conversation module.
         * @return Conversation The Conversation module.
         */
        private Conversation conversation;
        public Conversation getConversation() {
            return conversation;
        }

        /// <summary>
        /// Constructs the RandomCaveHoboWitchDoctor.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        public RandomCaveHoboWitchDoctor(Room room) : base(room.loadRegion) {
            location = new Location(this);
            graphics = new RandomCaveHoboWitchDoctor2DGraphics(this);
            conversation = new Conversation(this, "RCHWD");
            room.roomEntered += new RoomEnteredListener(objectEnteredRoom);
            this.addEvent("Play with a hobo!", this.interactedWith);
        }

        /// <summary>
        /// The discussion that should be displayed to the player when s/he enters the same room that the RCHWD is in.
        /// </summary>
        internal class RoomEnteredDiscussion : Discussion {

#region FACTORY

		    // The classHash, a unique identifying string for the class. Hmm, wow, that's kind of redundant, isn't that? C# already provides such a function through reflection. Oh well.
		    // Used for the factory methods called when the client receives a CREATE_NEW_OBJECT update from the server computer.
		    internal const string classHash = "RCHWD.RoomEnteredDiscussion";

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
            private RoomEnteredDiscussion(LoadRegion loadRegion, int id) : base(loadRegion, id) {
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
            /// Constructs a new RoomEnteredDiscussion object, and sets its dialogue.
            /// </summary>
            /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
            public RoomEnteredDiscussion(LoadRegion loadRegion) : base(loadRegion) {
                messages = new string[2];
                usingFaces = new bool[] { true, false };
                options = new string[2][];

                messages[0] = "Yrrr, what's this little light, playing 'pon my eyes?\n" +
                "Gasp! It's an ALIEN, and it's come to take my life!\n" +
                "Go, git, and run away! You'll never take me! You never will!";
                messages[1] = "The old ragged guy chucks something large and shiny at you. It briefly flashes over your sight. ... *KLUNK*\nOW! That hurt!\n... You pick it up. You've acquired a Gold Piece(1)! Congrats!!!!!! You've earned it!";
                // Todo: random trivia answer goes here
            }

            /// <summary>
            /// Sets this Discussion to start specifying the next message in the Conversation.
            /// Or to close if no more messages remain.
            /// </summary>
            /// <param name="option">The option chosen by the user.</param>
            public override void advanceDiscussion(string option) {
                if (curMessage.value == 0) curMessage.value++;
                else curMessage.value = -1;
            }

        }

        /// <summary>
        /// The discussion that should be displayed to the player when s/he clicks on the RCHWD.
        /// </summary>
        internal class ArrowBuyingDiscussion : Discussion {

#region FACTORY

		    // The classHash, a unique identifying string for the class. Hmm, wow, that's kind of redundant, isn't that? C# already provides such a function through reflection. Oh well.
		    // Used for the factory methods called when the client receives a CREATE_NEW_OBJECT update from the server computer.
		    internal const string classHash = "ArrowBuyingDiscussion";

            /// <summary>
            /// The static constructor. Adds the class's factory method to the GameObject factoryList when the class is first loaded.
            /// </summary>
            static ArrowBuyingDiscussion() {
                GameObject.factoryList.Add(classHash, new GameObjectFactory(makeArrowBuyingDiscussion));
            }
        
			/// <summary>
			/// A factory method that creates and returns a new instance of ArrowBuyingDiscussion. Used by the client when the server requests it to make a new GameObject.
			/// </summary>
            /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
            /// <param name="id">This GameObject's ID.</param>
            /// <param name="reader">The PacketReader from which we will read the fields of the newly constructed GameObject.</param>
            /// <returns>A new instance of ArrowBuyingDiscussion.</returns>
            static ArrowBuyingDiscussion makeArrowBuyingDiscussion(LoadRegion loadRegion, int id, Microsoft.Xna.Framework.Net.PacketReader reader) {
                if (GameWorld.status != GameWorld.Status.MULTIPLAYER_CLIENT)
                    throw new System.Exception("You're not a client, so why are you calling the GameObject factory method?");
                ArrowBuyingDiscussion arrowBuyingDiscussion = new ArrowBuyingDiscussion(loadRegion, id);
                // ORDER OF STUFF (where you used the reader to construct datatypes, used factory methods exclusively. also, construct modules and their datatypes here too.)
                return arrowBuyingDiscussion;
            }

            /// <summary>
            /// Constructs a GameObject and assigns it an ID.
            /// This is the constructor that should be used if and only if you are a MULTIPLAYER_CLIENT.
            /// Furthermore, it is only called by the GameObjectFactory method.
            /// </summary>
            /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
            /// <param name="id">This GameObject's ID.</param>
            private ArrowBuyingDiscussion(LoadRegion loadRegion, int id) : base(loadRegion, id) {
            }

            /// <summary>
            /// Returns the class hash. 
            /// </summary>
            /// <returns>The class hash. Do we really have to tell you everything twice?</returns>
            public override string getClassHash() {
                return classHash;
            }

#endregion

            private readonly UpdatableGameObject<RandomCaveHoboWitchDoctor> witchy;
            private readonly UpdatableGameObject<Player> playie;

            private const string affirmative = "Yeah, whatever.";

            /// <summary>
            /// Constructs a new ArrowBuyingDiscussion object, and sets its dialogue.
            /// </summary>
            /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
            /// <param name="parent">The RandomCaveHoboWitchDoctor who is speaking in this discussion.</param>
            /// <param name="yuppie">The Player to whom the RandomCaveHoboWitchDoctor is talking.</param>
            public ArrowBuyingDiscussion(LoadRegion loadRegion, RandomCaveHoboWitchDoctor parent, Playable yuppie) : this(loadRegion) {
                witchy = new UpdatableGameObject<RandomCaveHoboWitchDoctor>(this);
                playie = new UpdatableGameObject<Player>(this);
                this.witchy.value = parent;
                this.playie.value = (Player)yuppie;
            }

            /// <summary>
            /// Constructs a new ArrowBuyingDiscussion object, and sets its dialogue.
            /// This bare constructor is intended for use in the client-side factory methods.
            /// </summary>
            /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
            ArrowBuyingDiscussion(LoadRegion loadRegion) : base(loadRegion) {

                messages = new string[9];
                usingFaces = new bool[9];
                options = new string[9][];

                messages[0] = "Hrrr, come 'ere, boy. I'm runnin' out of cash, you see, and I notice you've got quite a few o' them coins in yer purse there.\nSo how's 'bout gettin' me maybeh three o' them, huh? Here, I'll give you this nice little piece of candy in return. What d'ya say?";
                usingFaces[0] = true;
                options[0] = null;
              
                messages[1] = "Upon closer inspection, you deduce that the \"candy\" that he's offering you is actually a barbed arrow.";
                usingFaces[1] = false;
                options[1] = new string[] {affirmative, "Umm... no thanks."};

                messages[2] = "RAWR you stinkin' little rowdy ruff boy! What, don't you have any money?! You'll be gettin' yer commance! Yu'll see...";   // lol! Powerpuff girls! lol!
                usingFaces[2] = true;
                options[2] = null;

                messages[3] = "The old bearded man jumps up on one leg with his eyes spread wide and one of his arms stretched upwards while the other one curved in front of his abdomen area. As if that weren't weird enough already, his hands are positioned in a way that they seem to be holding little dainty pieces of paper, or maybe making dog-shaped shadow puppets or something.";
                usingFaces[3] = false;
                options[3] = null;
                
                messages[4] = "Nitsha-Hammee-Superbowl-X, X, I, I, I, I, I! Yarr, I be puttin' a curse on yer thigh!";
                usingFaces[4] = true;
                options[4] = null;

                messages[4] = "After this, the old guy just dances around with his eyes still spread as if he were in a trance or something. You decide that this is none of your concern and that you're just going to walk away. Besides, how many dialog boxes have you wasted already on this guy? It's just not worth it.";
                usingFaces[4] = false;
                options[4] = null;

                messages[5] = "The old man squeals in delight. ... Okay, whatever.";
                usingFaces[5] = false;
                options[5] = null;

                messages[6] = "Ho-ho! Yes, give me the coins! And I'll give you the candeh! ... But first, we must perform... *shifty eyes* the Ritual!";
                usingFaces[6] = true;
                options[6] = null;

                messages[7] = "The greasily bearded guy jumps up, waves his arms around, and starts chanting. Disturbingly, his eyes roll up, showing you only the white of his eyes, abundantly streaked by fleshy red blood vessels. Then he suddenly freezes, yells \"HAAA!,\" and starts posing you with questions.";
                usingFaces[7] = false;
                options[7] = null;

                messages[8] = "Upon closer inspection, you deduce that the \"candy\" that he's offering you is actually a barbed arrow.";
                usingFaces[8] = false;
                options[8] = new string[] { "Uhh... sorry, I don't have that many coins." };
            }

            /// <summary>
            /// Sets this Discussion to start specifying the next message in the Conversation.
            /// Or to close if no more messages remain.
            /// </summary>
            /// <param name="option">The option chosen by the user.</param>
            public override void advanceDiscussion(string option) {
                if (curMessage.value == 0) {
                    Gold gold = playie.value.getInventory().getItem<Gold>();
                    if (gold == null || gold.getItem().getAmount() < 3) {
                        curMessage.value = 8;
                    } else curMessage.value = 1;
                } else if (curMessage.value == 1 || curMessage.value == 8) {
                    if (option == affirmative) { // Yes
                        playie.value.getInventory().removeItem<Gold>(3);
                        curMessage.value = 5;
                    } else { // No
                        curMessage.value = 2;
                    }
                }
                else if (curMessage.value == 7) {
                    curMessage.value = -1;
                    WumpusGame.World.Pit.PitTrivia trivia = new WumpusGame.World.Pit.PitTrivia(this.loadRegion);
                    trivia.startTrivia(witchy.value, playie.value, 3, 2);
                    GameObject gameObject = playie.value;
                    if(gameObject is Playable) witchy.value.getConversation().startConversation((Playable) gameObject, trivia);
                    // Todo: I wonder what happens when a new conversation is started over an old one.
                } else if (curMessage.value == 5) {
                    killDiscussion();
                    return;
                } else {
                    curMessage.value++;
                }
            }
        }

        /// <summary>
        /// The Trivia that the Player has to answer in order to buy an Arrow.
        /// </summary>
        internal class ArrowBuyingTrivia : Trivia
        {

#region FACTORY

            // The classHash, a unique identifying string for the class. Hmm, wow, that's kind of redundant, isn't that? C# already provides such a function through reflection. Oh well.
            // Used for the factory methods called when the client receives a CREATE_NEW_OBJECT update from the server computer.
            internal const string classHash = "ArrowBuyingTrivia";

            /// <summary>
            /// The static constructor. Adds the class's factory method to the GameObject factoryList when the class is first loaded.
            /// </summary>
            static ArrowBuyingTrivia() {
                GameObject.factoryList.Add(classHash, new GameObjectFactory(makeArrowBuyingTrivia));
            }

            /// <summary>
            /// A factory method that creates and returns a new instance of ArrowBuyingTrivia. Used by the client when the server requests it to make a new GameObject.
            /// </summary>
            /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
            /// <param name="id">This GameObject's ID.</param>
            /// <param name="reader">The PacketReader from which we will read the fields of the newly constructed GameObject.</param>
            /// <returns>A new instance of ArrowBuyingTrivia.</returns>
            static ArrowBuyingTrivia makeArrowBuyingTrivia(LoadRegion loadRegion, int id, Microsoft.Xna.Framework.Net.PacketReader reader) {
                if (GameWorld.status != GameWorld.Status.MULTIPLAYER_CLIENT)
                    throw new System.Exception("You're not a client, so why are you calling the GameObject factory method?");
                ArrowBuyingTrivia arrowBuyingTrivia = new ArrowBuyingTrivia(loadRegion, id);
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
            private ArrowBuyingTrivia(LoadRegion loadRegion, int id) : base(loadRegion, id) {
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
            /// Constructs a new ArrowBuyingTrivia object, and sets its dialogue.
            /// </summary>
            /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
            public ArrowBuyingTrivia(LoadRegion loadRegion) : base(loadRegion) {
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
        /// This method handles the objectEnteredRoom event.
        /// If it's a player, then it proceeds to start a conversation and give the player a gold and give the player a random trivia tidbit.
        /// </summary>
        /// <param name="user">The object that entered the room.</param>
        public void objectEnteredRoom(Locatable gameObject) {
            if (gameObject is Playable) {
                conversation.startConversation((Playable)gameObject, new RoomEnteredDiscussion(this.loadRegion));
                ((Playable)gameObject).getInventory().addItem(new Gold( ((Playable) gameObject).getPlayer().getLocalLoadRegion() ) );
            }
        }


        /// <summary>
        /// Gets an event from the eventHashlist.
        /// </summary>
        /// <param name="invoker">The type of event (ie. left-click, right-click, etc).</param>
        /// <returns>The method that is called by the event.</returns>
        public Event getEvent(int invoker) {
            if (invoker == InteractionEngine.Client.TwoDimensional.UserInterface2D.MOUSE_LEFT_CLICK)
                return new Event(this.id, "Play with a hobo!", (object)GameWorld.user.getPermission(0));
            return null;
        }

        /// <summary>
        /// When the player interacts with the Wumpus, ie, the user clicks on the Wumpus graphic,
        /// this method gets called. It starts the discussion that might ultimately result in the
        /// player getting an arrow.
        /// </summary>
        /// <param name="player">The Player who's talking to the RCHWD.</param>
        public void interactedWith(object Object) {
            Player player;
            if (Object is Player) {
                player = (Player)Object;
                conversation.startConversation(player, new ArrowBuyingDiscussion(player.getPlayer().getLocalLoadRegion(), this, player));
            }
        }

        /// <summary>
        /// Alerts this GameObject that a trivia session was completed, and reports
        /// whether or not the player succeeded in answering enough questions correctly.
        /// </summary>
        /// <param name="player">The Player that the conversation was with.</param>
        /// <param name="success">Whether or not the player succeeded.</param>
        public void triviaCompleted(Playable player, bool success) {
            if (success) player.getInventory().addItem(new Arrow(player.getPlayer().getLocalLoadRegion()));
        }

    }

} 