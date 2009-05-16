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
| * Triviable                    Interface |
| * Trivia                       Class     |
\*••••••••••••••••••••••••••••••••••••••••*/

using WumpusGame.Modules;
using WumpusGame.World.Modules;
using InteractionEngine.Constructs;
using InteractionEngine.Constructs.Datatypes;
using InteractionEngine.GameWorld;

namespace WumpusGame.World {

    /**
     * Any GameObject that needs to be notified of the results of a series of trivia questions should implement this interface.
     * It gets notified through the triviaCompleted event
     * Perhaps we should also pass a sort of "trivia ID" along too. *shrug*
     */
    public interface Triviable : Conversable {

        /// <summary>
        /// Alerts this GameObject that a trivia session was completed, and reports
        /// whether or not the player succeeded in answering enough questions correctly.
        /// </summary>
        /// <param name="player">The Player that the conversation was with.</param>
        /// <param name="success">Whether or not the player succeeded.</param>
        void triviaCompleted(Playable player, bool success);

    }

    /**
     * It's a special case of Discussion. Yep. To use it, all you need is to set the non-Updatable array fields.
     * Make sure that the reference to the string in correctAnswers is the same as the corresponding string reference in options.
     */
    public abstract class Trivia : Discussion {

        // A very spiffy-looking random number generator.
        // Used for generating random spaghetti.
        static System.Random randy = new System.Random();
        // The array of all correct answers to all the trivia questions specified by Discussion.messages.
        // The index of the answer to the current question is specified by Discussion.curMessage.
        protected string[] correctAnswers;
        // The number of questions remaining to ask the user.
        // Used when this Trivia needs to know whether or not to ask another question!
        protected readonly UpdatableInteger questionsLeft;
        // The number of correct responses remaining before the user passes the trivia test.
        // Used when this Trivia needs to know whether or not the user passed.
        protected readonly UpdatableInteger correctsLeft;
        // The GameObject to alert when the user has completed the trivia quiz.
        // Used when the user has completed the trivia quiz.
        protected readonly UpdatableGameObject<GameObject/*Triviable*/> trivier;
        // The Player who's being quizzed.
        // Given to the trivier as event information when this Trivia needs to report back when the user completes the quiz.
        protected readonly UpdatableGameObject<Player> player;

        /// <summary>
        /// Constructs a new Trivia object.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        public Trivia(LoadRegion loadRegion) : base(loadRegion) {
            questionsLeft = new UpdatableInteger(this);
            correctsLeft = new UpdatableInteger(this);
            trivier = new UpdatableGameObject<GameObject/*Triviable*/>(this);
            player = new UpdatableGameObject<Player>(this);
        }
        
        /// <summary>
        /// Constructs a new Trivia object. The client-side constructor.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        /// <param name="id">The id of this new GameObject</param>
        public Trivia(LoadRegion loadRegion, int id) : base(loadRegion, id) {
        }

        /// <summary>
        /// Starts quizzing the player on a series of trivia questions.
        /// When enough questions have been answered, the status of the trivia quiz
        /// will be reported back through Triviable.triviaCompleted().
        /// </summary>
        /// <param name="requester">The GameObject to alert when the trivia quiz has been completed.</param>
        /// <param name="player">The Player to whom to present the trivia.</param>
        /// <param name="numQuestions">Maximum number of questions to display.</param>
        /// <param name="numRequired">Number of questions the player must answer correctly to succeed.</param>
        public void startTrivia(Triviable requester, Player player, int numQuestions, int numRequired) {
            trivier.value = (GameObject)requester;
            this.player.value = player;
            questionsLeft.value = numQuestions;
            correctsLeft.value = numRequired;
        }

        public override void startDiscussion() {
            curMessage.value = randy.Next(messages.Length);
        }

        /// <summary>
        /// A trivia question has been answered. Either presents the next trivia question
        /// or alerts the trivia-requesting GameObject of the status of the completed trivia quiz.
        /// </summary>
        /// <param name="option">The answer chosen by the user.</param>
        public override void advanceDiscussion(string option) {
            questionsLeft.value = questionsLeft.value - 1;
            // warning: Make sure that the references are the same.... if inlined in the program code this is automatic via string interning
            if (option == correctAnswers[curMessage.value]) correctsLeft.value = correctsLeft.value - 1;
            if (questionsLeft.value <= 0 || correctsLeft.value <= 0) {
                this.killDiscussion();
                ((Triviable)trivier.value).triviaCompleted(player.value, correctsLeft.value <= 0);
            } else if (GameWorld.status != GameWorld.Status.MULTIPLAYER_CLIENT) {
                curMessage.value = randy.Next(messages.Length);
            } else { // client
                this.killDiscussion(); // temporary, until the server comes and updates it
            }
        }

    }

}