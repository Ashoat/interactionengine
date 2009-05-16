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
| * Pit                              Class |
\*••••••••••••••••••••••••••••••••••••••••*/

using WumpusGame.World.Modules;
using InteractionEngine.Constructs;
using InteractionEngine.Constructs.Datatypes;
using System.Collections.Generic;
using InteractionEngine.Client;
using WumpusGame.World.Graphics;
using WumpusGame.Modules;
using InteractionEngine.GameWorld;

namespace WumpusGame.World {

    /**
     * The PIT is an environmentalist organization striving to preserve endangered species such as the gentle Wumpus.
     * It does this by quizzing potential poachers with a series of tough environmental questions.
     * If the poacher fails the quiz, they are brutally murdered (using the latest in green weapons technology, of course!).
     */
    
    public class Pit : GameObject, Locatable, Graphable, Triviable {

#region FACTORY

		// Contains a unique identifying string for the class. 
		// Used for the factory methods called when the client receives a CREATE_OBJECT update from the server computer.
		internal const string classHash = "Pit";

        /// <summary>GameObject factoryList when the class is first loaded.
        /// </summary>
        /// The static constructor. Adds the class's factory method to the 
        static Pit() {
            GameObject.factoryList.Add(classHash, new GameObjectFactory(makePit));
        }

        /// <summary>
        /// A factory method that creates and returns a new instance of Pit. Used by the client when the server requests it to make a new GameObject.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        /// <param name="id">This GameObject's ID.</param>
        /// <param name="reader">The PacketReader from which we will read the fields of the newly constructed GameObject.</param>
        /// <returns>A new instance of Pit.</returns>
        static Pit makePit(LoadRegion loadRegion, int id, Microsoft.Xna.Framework.Net.PacketReader reader) {
            if (GameWorld.status != GameWorld.Status.MULTIPLAYER_CLIENT)
                throw new System.Exception("You're not a client, so why are you calling the GameObject factory method?");
            Pit pit = new Pit(loadRegion, id);
            // ORDER OF STUFF (where you used the reader to construct datatypes, used factory methods exclusively. also, construct modules and their datatypes here too.)
            return pit;
        }
        
        /// <summary>
        /// Constructs a GameObject and assigns it an ID.
        /// This is the constructor that should be used if and only if you are a MULTIPLAYER_CLIENT.
        /// Furthermore, it is only called by the GameObjectFactory method.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        /// <param name="id">This GameObject's ID.</param>
        private Pit(LoadRegion loadRegion, int id) : base(loadRegion, id){
        }

        /// <summary>
        /// Contains a string hash of this type.
        /// Used when passing a CREATE_FIELD to specify which type to use.
        /// </summary>
        /// <returns>The string hash.</returns>
        public override string getClassHash() {
            return classHash;
        }

#endregion

        /*••••••••••••••••••••••••••••••••••••••••*\
          MODULES
        \*••••••••••••••••••••••••••••••••••••••••*/

        /// <summary>
        /// Returns the Location module.
        /// </summary>
        private Location location;
        public Location getLocation() {
            return location;
        }

        /// <summary>
        /// Returns the Graphics module.
        /// </summary>
        private Pit2DGraphics graphics;
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

        /*••••••••••••••••••••••••••••••••••••••••*\
          MEMBERS
        \*••••••••••••••••••••••••••••••••••••••••*/
        
        /// <summary>
        /// Constructs a new Pit. Can only be called by the factory; hence, it is only accessible by clients in a multiplayer game.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        private Pit(LoadRegion loadRegion, int id, Microsoft.Xna.Framework.Net.PacketReader reader)
            : base(loadRegion, id) {
            location = new Location(this, reader);
            graphics = new Pit2DGraphics(this);
        }

        /// <summary>
        /// Constructs a new Pit. Cannot be used by clients in a multiplayer game.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        public Pit(Room room)
            : base(room.loadRegion) {
             room.roomEntered += this.objectEntersRoom;
             location = new Location(this);
            graphics = new Pit2DGraphics(this);
             conversation = new Conversation(this, "Pit");
        }

        /// <summary>
        /// The discussion that should be displayed to the player when s/he enters the same room that the Pit is in.
        /// </summary>
        private class RoomEnteredDiscussion : Discussion {

#region FACTORY

            // The classHash, a unique identifying string for the class. Hmm, wow, that's kind of redundant, isn't that? C# already provides such a function through reflection. Oh well.
            // Used for the factory methods called when the client receives a CREATE_NEW_OBJECT update from the server computer.
            internal const string classHash = "Pit.RoomEnteredDiscussion";

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

            private readonly UpdatableGameObject<Player> player;
            private readonly UpdatableGameObject<Pit> pit;

            /// <summary>
            /// Constructs a new RoomEnteredDiscussion object, and sets its dialogue.
            /// </summary>
            /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
            /// <param name="player">The player to whom the PIT agent is talking.</param>
            /// <param name="bat">The PIT whose conversation this is.</param>
            public RoomEnteredDiscussion(LoadRegion loadRegion, Player player, Pit pit) : this(loadRegion) {
                this.player = new UpdatableGameObject<Player>(this);
                this.pit = new UpdatableGameObject<Pit>(this);
                this.player.value = player;
                this.pit.value = pit;
            }

            /// <summary>
            /// Constructs a new RoomEnteredDiscussion object, and sets its dialogue.
            /// </summary>
            /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
            public RoomEnteredDiscussion(LoadRegion loadRegion) : base(loadRegion) {

                messages = new string[2];
                usingFaces = new bool[] { false, true };
                options = new string[2][];

                messages[0] = "You notice something scuttling on the floor... WHUSH! Suddenly you find yourself hanging upside-down in a net. Some mysterious people in black coats and shades walk up to you, carrying rigid square briefcases.";
                options[0] = null;

                messages[1] = "*Ahem* This area is under the jurisdiction of the PIT environmentalist organization. Under Presidential decree number 54327-A, section 4, all poachers apprehended in this area must be killed, though only with the latest environmentally friendly procedures. Are you a poacher? No? Well, you'll have to prove it.";
                options[1] = null;
            }

            /// <summary>
            /// Sets this Discussion to start specifying the next message in the Conversation.
            /// Or to close if no more messages remain.
            /// </summary>
            /// <param name="option">The option chosen by the user.</param>
            public override void advanceDiscussion(string option) {
                if (curMessage.value == 0) curMessage.value++;
                else {
                    curMessage.value = -1;
                    conversationEndAction();
                }
            }

            /// <summary>
            /// Start the trivia!
            /// </summary>
            private void conversationEndAction() {
                PitTrivia trivia = new PitTrivia(pit.value.loadRegion);
                trivia.startTrivia(pit.value, player.value, 3, 2);
                pit.value.getConversation().startConversation(player.value, trivia);
            }

        }
        
        /// <summary>
        /// The trivia that the player must answer in order to escape the Pit.
        /// </summary>
        public class PitTrivia : Trivia {

#region FACTORY

            // The classHash, a unique identifying string for the class. Hmm, wow, that's kind of redundant, isn't that? C# already provides such a function through reflection. Oh well.
            // Used for the factory methods called when the client receives a CREATE_NEW_OBJECT update from the server computer.
            internal const string classHash = "PitTrivia";

            /// <summary>
            /// The static constructor. Adds the class's factory method to the GameObject factoryList when the class is first loaded.
            /// </summary>
            static PitTrivia() {
                GameObject.factoryList.Add(classHash, new GameObjectFactory(makePitTrivia));
            }

            /// <summary>
            /// A factory method that creates and returns a new instance of PitTrivia. Used by the client when the server requests it to make a new GameObject.
            /// </summary>
            /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
            /// <param name="id">This GameObject's ID.</param>
            /// <param name="reader">The PacketReader from which we will read the fields of the newly constructed GameObject.</param>
            /// <returns>A new instance of PitTrivia.</returns>
            static PitTrivia makePitTrivia(LoadRegion loadRegion, int id, Microsoft.Xna.Framework.Net.PacketReader reader) {
                if (GameWorld.status != GameWorld.Status.MULTIPLAYER_CLIENT)
                    throw new System.Exception("You're not a client, so why are you calling the GameObject factory method?");
                PitTrivia arrowBuyingTrivia = new PitTrivia(loadRegion, id);
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
            private PitTrivia(LoadRegion loadRegion, int id) : base(loadRegion, id) {
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
            /// Constructs a new PitTrivia object, and sets its dialogue.
            /// </summary>
            /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
            public PitTrivia(LoadRegion loadRegion)
                : base(loadRegion) {
                messages = new string[20];
                usingFaces = new bool[20];
                options = new string[20][];
                correctAnswers = new string[20];

                messages[0] = "What is the term for a place that was once a toxic waste site but is now safe to residential use?";
                options[0] = new string[] {
                    "A brownfield",
                    "A leachfield",
                    "A superfund site",
                    "restored",
                    "Trick question! Once toxic, places can never become safe!"};
                correctAnswers[0] = options[0][0];

                messages[1] = "Which of these choices is not an indoor air pollutant?";
                options[1] = new string[] {
                    "Dry-cleaning substances",
                    "Aerosols",
                    "Radon",
                    "Carbon Monoxide",
                    "Ozone"};
                correctAnswers[1] = options[1][4];

                messages[2] = "Which of these can have deleterious effects on human DNA?";
                options[2] = new string[] {
                    "Carcinogens",
                    "Mutagens",
                    "Teratogens",
                    "A and B",
                    "A, B, C"};
                correctAnswers[2] = options[2][3];

                messages[3] = "Which of these provides the most freshwater to humans?";
                options[3] = new string[] {
                    "Ice/Snow",
                    "Estuaries",
                    "Groundwater",
                    "Lakes/Reservoirs",
                    "Rivers/Streams"};
                correctAnswers[3] = options[3][2];

                messages[4] = "_____ exists in the stratosphere and protects organisms from UV radiation";
                options[4] = new string[] {
                    "Carbon Dioxide",
                    "Nitrogen",
                    "Atmospheric Oxygen",
                    "Ozone",
                    "CFCs"};
                correctAnswers[4] = options[4][3];

                messages[5] = "Snowy Owls in the Pacific Northwest are examples of a:";
                options[5] = new string[] {
                    "marine species",
                    "indicator species",
                    "keystone species",
                    "primary consumer",
                    "extinct species"};
                correctAnswers[5] = options[5][2];

                messages[6] = "Which of these traits do K-strategists not have?";
                options[6] = new string[] {
                    "Overshooting the carrying capacity of a population",
                    "Tending to be long-lived",
                    "Tending to mature gradually",
                    "Investing considerable energy in offspring",
                    "Being higher on the food chain"};
                correctAnswers[6] = options[6][0];

                messages[7] = "Denitrifying bacteria:";
                options[7] = new string[] {
                    "Convert nitrites into nitrate",
                    "Convert nitrate into nitrites",
                    "Convert ammonia into nitrates",
                    "Convert nitrates into atmospheric nitrogen",
                    "Convert atmospheric nitrogen into nitrites"};
                correctAnswers[7] = options[7][2];

                messages[8] = "Which of these is not a good way to disinfect pathogens when treating water?";
                options[8] = new string[] {
                    "Chlorine",
                    "Soap",
                    "Ozone",
                    "Ultraviolet light",
                    "More than one of the above"};
                correctAnswers[8] = options[8][4];

                messages[9] = "Which is a renewable source of energy?";
                options[9] = new string[] {
                    "Oil",
                    "Coal",
                    "Geothermal",
                    "Natural gas",
                    "A and B"};
                correctAnswers[9] = options[9][2];

                messages[10] = "The farming of shrimp and fish is:";
                options[10] = new string[] {
                    "an example of a GMO",
                    "a method that produces healthier seafood",
                    "part of the Green Revolution",
                    "an environmentally responsible method",
                    "part of the Blue Revolution"};
                correctAnswers[10] = options[10][3];

                messages[11] = "If a stream has a pH of 6.1, the body of water is:";
                options[11] = new string[] {
                    "infected with animal waste",
                    "acidic",
                    "basic",
                    "very unsafe for human consumption",
                    "probably at a high altitude"};
                correctAnswers[11] = options[11][1];

                messages[12] = "The environmental crisis that Rachel Carson exposed is:";
                options[12] = new string[] {
                    "Bhopal",
                    "Chernobyl",
                    "Exxon Valdez",
                    "Three Mile Island",
                    "DDT"};
                correctAnswers[12] = options[12][4];

                messages[13] = "Which of these does not allow any mechanical device, tool, or vehicle?";
                options[13] = new string[] {
                    "Wilderness area",
                    "National park",
                    "BLM land",
                    "Green belt",
                    "National forest"};
                correctAnswers[13] = options[13][0];

                messages[14] = "A Secchi disc:";
                options[14] = new string[] {
                    "measures acidity",
                    "measures salinity",
                    "measures pH",
                    "measures hardness",
                    "measures turbidity"};
                correctAnswers[14] = options[14][4];

                messages[15] = "Which of these is found in much of Canada and Russia?";
                options[15] = new string[] {
                    "Tropical rainforest",
                    "Boreal forest",
                    "Deciduous forest",
                    "Kelp forest",
                    "Temperate rainforest"};
                correctAnswers[15] = options[15][1];

                messages[16] = "The movement of a fat-soluble pollutant to organisms at higher trophic levels is called:";
                options[16] = new string[] {
                    "bioremediation",
                    "biomagnification",
                    "bioaccumulation",
                    "bioaggravation",
                    "bioexoneration"};
                correctAnswers[16] = options[16][1];

                messages[17] = "Which describes the process in which one community gradually replaces another in an area?";
                options[17] = new string[] {
                    "Adaptive radiation",
                    "Convergent evolution",
                    "Natural Selection",
                    "Succession",
                    "Mutualism"};
                correctAnswers[17] = options[11][3];

                messages[18] = "This type of rock is created by the cooling of lava that originates in the lithosphere:";
                options[18] = new string[] {
                    "Sedimentary rock",
                    "Igneous rock",
                    "Metamorphic rock",
                    "Oil",
                    "Natural gas"};
                correctAnswers[18] = options[18][1];

                messages[19] = "Which of these states that chemical reactions have the same amount of reactants as products?";
                options[19] = new string[] {
                    "Nutrient cycle",
                    "“Spaceship Earth”",
                    "First Law of Thermodynamics",
                    "Second Law of Thermodynamics",
                    "Conservation of mass"};
                correctAnswers[19] = options[19][4];

            }

        }

        /// <summary>
        /// The discussion where the Pit decides whether or not to kill the player.
        /// </summary>
        private class TriviaSucceededDiscussion : Discussion {

#region FACTORY

            // The classHash, a unique identifying string for the class. Hmm, wow, that's kind of redundant, isn't that? C# already provides such a function through reflection. Oh well.
            // Used for the factory methods called when the client receives a CREATE_NEW_OBJECT update from the server computer.
            internal const string classHash = "Pit.TriviaSucceededDiscussion";

            /// <summary>
            /// The static constructor. Adds the class's factory method to the GameObject factoryList when the class is first loaded.
            /// </summary>
            static TriviaSucceededDiscussion() {
                GameObject.factoryList.Add(classHash, new GameObjectFactory(makeTriviaSucceededDiscussion));
            }

            /// <summary>
            /// A factory method that creates and returns a new instance of TriviaSucceededDiscussion. Used by the client when the server requests it to make a new GameObject.
            /// </summary>
            /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
            /// <param name="id">This GameObject's ID.</param>
            /// <param name="reader">The PacketReader from which we will read the fields of the newly constructed GameObject.</param>
            /// <returns>A new instance of TriviaSucceededDiscussion.</returns>
            static TriviaSucceededDiscussion makeTriviaSucceededDiscussion(LoadRegion loadRegion, int id, Microsoft.Xna.Framework.Net.PacketReader reader) {
                if (GameWorld.status != GameWorld.Status.MULTIPLAYER_CLIENT)
                    throw new System.Exception("You're not a client, so why are you calling the GameObject factory method?");
                TriviaSucceededDiscussion roomEnteredDiscussion = new TriviaSucceededDiscussion(loadRegion, id);
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
            private TriviaSucceededDiscussion(LoadRegion loadRegion, int id) : base(loadRegion, id) {
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
            /// Constructs a new TriviaSucceededDiscussion object, and sets its dialogue.
            /// </summary>
            /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
            public TriviaSucceededDiscussion(LoadRegion loadRegion) : base(loadRegion) {
                messages = new string[1];
                usingFaces = new bool[] { true };
                options = new string[1][];

                messages[0] = "Well! Someone possessing that much knowledge of the environmental sciences couldn't possibly be a poacher. Sorry for the trouble, sir. You're free to go..";
                options[0] = null;
            }

            /// <summary>
            /// Sets this Discussion to start specifying the next message in the Conversation.
            /// Or to close if no more messages remain.
            /// </summary>
            /// <param name="option">The option chosen by the user.</param>
            public override void advanceDiscussion(string option) {
                curMessage.value = -1;
            }

        }


        /// <summary>
        /// The discussion where the Pit decides whether or not to kill the player.
        /// </summary>
        private class TriviaFailedDiscussion : Discussion {

#region FACTORY

            // The classHash, a unique identifying string for the class. Hmm, wow, that's kind of redundant, isn't that? C# already provides such a function through reflection. Oh well.
            // Used for the factory methods called when the client receives a CREATE_NEW_OBJECT update from the server computer.
            internal const string classHash = "Pit.TriviaFailedDiscussion";

            /// <summary>
            /// The static constructor. Adds the class's factory method to the GameObject factoryList when the class is first loaded.
            /// </summary>
            static TriviaFailedDiscussion() {
                GameObject.factoryList.Add(classHash, new GameObjectFactory(makeTriviaFailedDiscussion));
            }

            /// <summary>
            /// A factory method that creates and returns a new instance of TriviaFailedDiscussion. Used by the client when the server requests it to make a new GameObject.
            /// </summary>
            /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
            /// <param name="id">This GameObject's ID.</param>
            /// <param name="reader">The PacketReader from which we will read the fields of the newly constructed GameObject.</param>
            /// <returns>A new instance of TriviaFailedDiscussion.</returns>
            static TriviaFailedDiscussion makeTriviaFailedDiscussion(LoadRegion loadRegion, int id, Microsoft.Xna.Framework.Net.PacketReader reader) {
                if (GameWorld.status != GameWorld.Status.MULTIPLAYER_CLIENT)
                    throw new System.Exception("You're not a client, so why are you calling the GameObject factory method?");
                TriviaFailedDiscussion roomEnteredDiscussion = new TriviaFailedDiscussion(loadRegion, id);
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
            private TriviaFailedDiscussion(LoadRegion loadRegion, int id)
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
            /// Constructs a new TriviaFailedDiscussion object, and sets its dialogue.
            /// </summary>
            /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
            public TriviaFailedDiscussion(LoadRegion loadRegion) : base(loadRegion) {
                messages = new string[2];
                usingFaces = new bool[] { true, false };
                options = new string[2][];

                messages[0] = "Hmm. Looks like we've got a poacher on our hands....";
                options[0] = null;

                messages[1] = "By the way, this means you're dead.";
                options[1] = null;
            }

            /// <summary>
            /// Sets this Discussion to start specifying the next message in the Conversation.
            /// Or to close if no more messages remain.
            /// </summary>
            /// <param name="option">The option chosen by the user.</param>
            public override void advanceDiscussion(string option) {
                if (curMessage.value == 0) curMessage.value++;
                else {
                    curMessage.value = -1;
                }
            }

        }

        /// <summary>
        /// This method handles the objectEnteredRoom event. If it's a player, then it proceeds to go into the trivia process.
        /// </summary>
        /// <param name="user">The object that entered the room.</param>
        public void objectEntersRoom(Locatable gameObject) {
            if (gameObject is Player) {
                conversation.startConversation((Playable)gameObject, new RoomEnteredDiscussion(loadRegion, (Player)gameObject, this));
            }
        }

        public void triviaCompleted(Playable player, bool success) {
            if (success) conversation.startConversation(player, new TriviaSucceededDiscussion(loadRegion));
            else conversation.startConversation(player, new TriviaFailedDiscussion(loadRegion));
            // Todo: kill the player
        }

    }

}