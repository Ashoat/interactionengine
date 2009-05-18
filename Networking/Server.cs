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
            // The simplest way would, of course, be to mark Event with [Serializable], but hey,
            // at least this takes a minimal amount of manual work.
            writer.Write(eventObject.gameObjectID);
            writer.Write(eventObject.eventHash);
            formatter.Serialize(tcpClient.GetStream(), eventObject.parameter);
            /* OLD CODE THAT ASHOAT SHOULD REMOVE AS SOON AS HE VERIFIES MINE
            // The first four bytes are the GameObjectID.
            byte[] gameObjectIdBytes = new byte[4];
            int id = eventObject.gameObjectID;
            for (int i = 3; i >= 0; i++)
                gameObjectIdBytes[i] = System.Convert.ToByte(id << ((3 - i) * 8));
            // I have absolutely no idea how to write a string in bytes. This might be painful.
            byte[] stringBytes = new byte[0];
            // Write the gameObjectID and the eventHash to the steam.
            byte[] bytes = new byte[gameObjectIdBytes.Length + stringBytes.Length];
            gameObjectIdBytes.CopyTo(bytes, 0);
            stringBytes.CopyTo(bytes, gameObjectIdBytes.Length);
            tcpClient.GetStream().Write(bytes, 0, bytes.Length);
            // Write the parameter.
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            formatter.Serialize(tcpClient.GetStream(), eventObject.parameter);
             */
        }

    }

}