/*••••••••••••••••••••••••••••••••••••••••*\
| Interaction Engine                       |
| (C) Copyright Bluestone Coding 2008-2009 |
|••••••••••••••••••••••••••••••••••••••••••|
|           __    ___ ___  ___             |
|          /++\  | _ ) __|/ __|            |
|          \++/  | _ \__ \ (__             |
|           \/   |___/___/\___|            |
|                                          |
|••••••••••••••••••••••••••••••••••••••••••|
| EXCEPTION HANDLING                       |
| * InteractionEngineException       Class |
| * GameWorldException               Class |
\*••••••••••••••••••••••••••••••••••••••••*/

namespace InteractionEngine {

    /**
     * This class represents an exception caused by an error within the InteractionEngine.
     */
    public class InteractionEngineException : System.Exception {

        /// <summary>
        /// Construct an InteractionEngineException.
        /// </summary>
        /// <param name="message">The error message.</param>
        public InteractionEngineException(string message)
            : base(message) {

        }

        /// <summary>
        /// Construct an InteractionEngineException.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The Exception that caused this InteractionEngineException to be thrown.</param>
        public InteractionEngineException(string message, System.Exception innerException)
            : base(message, innerException) {

        }

    }

    /**
     * This class represents an exception caused by an error within the InteractionEngine.
     */
    public class GameWorldException : System.Exception {

        /// <summary>
        /// Construct an InteractionEngineException.
        /// </summary>
        /// <param name="message">The error message.</param>
        public GameWorldException(string message)
            : base(message) {

        }

        /// <summary>
        /// Construct an InteractionEngineException.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The Exception that caused this GameWorldException to be thrown.</param>
        public GameWorldException(string message, System.Exception innerException)
            : base(message, innerException) {

        }

    }

}