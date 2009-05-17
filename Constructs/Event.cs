/*••••••••••••••••••••••••••••••••••••••••*\
| Interaction Engine                       |
| (C) Copyright Bluestone Coding 2008      |
|••••••••••••••••••••••••••••••••••••••••••|
|           __    ___ ___  ___             |
|          /++\  | _ ) __|/ __|            |
|          \++/  | _ \__ \ (__             |
|           \/   |___/___/\___|            |
|                                          |
|••••••••••••••••••••••••••••••••••••••••••|
| CLIENT                                   |
| * Event                       Class      |
| * Interactable                Interface  |
\*••••••••••••••••••••••••••••••••••••••••*/

namespace InteractionEngine.EventHandling {

    /// <summary>
    /// This delegate holds references to methods that can be executed by Events.
    /// </summary>
    /// <param name="client">The Client that called this Event. If called locally, give as null.</param>
    /// <param name="parameter">The parameter that this EventMethod needs for input.</param>
    public delegate void EventMethod(Networking.Client client, System.Object parameter);

    /**
     * This class is a wrapper around a call to a method in the GameWorld.
     * It holds a GameObject ID and an "event hash", which is basically a pointer to the method on the GameObject.
     * It also hols a parameter to be given the Event.
     */
    public class Event {

        // Contains a reference to a GameObject.
        // Used for knowing where to find the EventHashlist for this Event.
        public int gameObjectID;
        // Contains a string referencing this particular event.
        // Used for figuring out which method to use on the GameObject's EventHashlist.
        public string eventHash;
        // Contains extra information for the event.
        // Used when extra information needs to be passed with the event.
        public object parameter;

        /// <summary>
        /// Constructs the event.
        /// </summary>
        /// <param name="id">The ID of the GameObject this Event is associated with.</param>
        /// <param name="hash">The eventHash of this Event.</param>
        /// <param name="parameter">Any extra information that needs to be passed with this Event.</param>
        public Event(int gameObjectID, string hash, object parameter) {
            this.gameObjectID = gameObjectID;
            this.eventHash = hash;
            this.parameter = parameter;
        }

    }

    /**
     * Implemented by GameObjects that can be interacted with.
     */
    public interface Interactable : Graphable {

        /// <summary>
        /// Gets an Event from this Interactable module.
        /// </summary>
        /// <param name="invoker">The invoker of this Event. If you have multiple possible invokers (ie. mouse click and mouse over) then we recommend you define constants for them.</param>
        /// <param name="user">The User that invokes this Event. Needed often for associating User invokers with GameObject invokers.</param>
        /// <returns>An Event.</returns>
        Event getEvent(int invoker);

    }

}
