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
| MODULE                                   |
| * Trigger                      Class     |
| * Triggerable                  Interface |
\*••••••••••••••••••••••••••••••••••••••••*/

using InteractionEngine.Constructs.Datatypes;

namespace Wumpus.World.Modules {

    /**
     * Holds all information and methods regarding Triggers, the mechanism for messaging within the GameWorld.
     */
    [System.Serializable]
    public class Trigger {
        public void eventTriggered(TriggerEvent trig) {
            
        }
    }

    /**
     * Implemented by GameObjects that have the Trigger module.
     */
    public interface Triggerable {

        /// <summary>
        /// Returns the Trigger module of this GameObject.
        /// </summary>
        /// <returns>The Trigger module associated with this GameObject.
        Trigger getTrigger();

    }

    /*[System.Serializable]
    public class TriggerEvent {
    }

    public class Trigger*/



}