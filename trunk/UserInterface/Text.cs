

namespace InteractionEngine.UserInterface.Text {

    /**
     * Holds all information and methods regarding inventory.
     */
    public class UserInterfaceText : UserInterface {

        // Contains a dictionary pointing an ID to an Event.
        // Used for allowing users to enter an integer to indicate an Event. 
        System.Collections.Generic.List<EventHandling.Event> options = new System.Collections.Generic.List<EventHandling.Event>();
        // Contains all the text that is going to be output this next loop.
        // Used for saving that text until it needs to be output.
        private string outputBuffer = "";
        // Contains a boolean specifying whether or not output needs to be made.
        // Used for preventing output to be made every loop... it only needs to be made after input is retrieved.
        private bool readyToOutput = true;
        // Contains a regex statement to search for numbers.
        // Used for figuring out if retrieved input is in the correct format.
        private System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("^[0-9]+$");
        // Contains an event invoker constant.
        // Used for figuring out what EventHandling.Event an Interactable should return.
        public const int TEXT_EVENT_CHOSEN = 245340;

        /// <summary>
        /// Checks state of user input devices to see if an input event should be triggered.
        /// If so, collects the EventHandling.Event objects and inserts them into the given list.
        /// Make it quick!
        /// </summary>
        /// <param name="newEventList">The list into which newly detected events are to be inserted.</param>
        protected override void retrieveInput(System.Collections.Generic.List<EventHandling.Event> newEvents) {
            string nextLine = System.Console.ReadLine();
            while (nextLine != null) {
                readyToOutput = true;
                int parse;
                if (regex.Match(nextLine).Success && options.Count >= (parse = System.Int32.Parse(nextLine) - 1)) {
                    newEvents.Add(options[parse]);
                } else {
                    println("That wasn't a valid option. The world just died. Good job.");
                }
            }
        }

        /// <summary>
        /// Output text to the user. Outputs nothing unless readyToOutput is true.
        /// </summary>
        public override void output() {
            if (!readyToOutput) return;
            options.Clear();
            foreach (Constructs.LoadRegion loadRegion in InteractionEngine.Engine.getLoadRegionList()) {
                for (int i = 0; i < loadRegion.getObjectCount(); i++) {
                    Constructs.GameObjectable gameObject = InteractionEngine.Engine.getGameObject(i);
                    // Go through every GameObject and see if they have something to output
                    if (gameObject is Graphable)
                        ((Graphable)gameObject).getGraphics().onDraw();
                    // Go through every GameObject and see if they can be interacted with
                    if (gameObject is InteractionEngine.EventHandling.Interactable)
                        options.Add(((InteractionEngine.EventHandling.Interactable)gameObject).getEvent(TEXT_EVENT_CHOSEN, Microsoft.Xna.Framework.Vector3.Zero));
                }
            }
            for (int i = 0; i < options.Count; i++)
                this.println((i + 1) + ": " + options[i].eventHash);
            System.Console.Out.Write(outputBuffer);
            outputBuffer = "";
            readyToOutput = false;
        }

        /// <summary>
        /// Print text to the user.
        /// </summary>
        /// <param name="output">The text to print.</param>
        public void println(string output) {
            outputBuffer += output + "\n";
        }

        /// <summary>
        /// Initialize stuff.
        /// </summary>
        public override void initialize() {

        }

    }

}