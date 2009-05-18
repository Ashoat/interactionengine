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
| SERVER                                   |
| * Server                           Class |
\*••••••••••••••••••••••••••••••••••••••••*/

namespace InteractionEngine.Networking {

    /**
     * This class represents a server in the InteractionEngine. It is the focal point for networking on the client-side.
     * This class is only ever touched on the client.
     */
    public class Server {

        // Contains a static reference to a BinaryFormatter.
        // Used for serialization/deserialization of objects.
        private static System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        // Contains a reference to the TcpClient connection that defines this class.
        // Used for communicating to the server this class represents.
        private readonly System.Net.Sockets.TcpClient tcpClient;
        // Contains a BinaryReader that wraps the above TcpClient.
        // Used so that we don't have to individually encode each byte into the stream.
        private readonly System.IO.BinaryWriter writer;

        /// <summary>
        /// Connect to a server at the specified IP.
        /// </summary>
        /// <param name="ipAddress">The IP address where we can find the server at.</param>
        public Server(int ipAddress) {
            tcpClient = new System.Net.Sockets.TcpClient(new System.Net.IPEndPoint((long)ipAddress, Client.listeningPort));
            writer = new System.IO.BinaryWriter(tcpClient.GetStream());
        }

        /// <summary>
        /// Send a packet containing an Event to the server.
        /// </summary>
        /// <param name="eventObject">The Event to send across.</param>
        public void sendEvent(EventHandling.Event eventObject) {
            writer.Write(eventObject.gameObjectID);
            writer.Write(eventObject.eventHash);
            formatter.Serialize(tcpClient.GetStream(), eventObject.parameter);
        }

    }

}