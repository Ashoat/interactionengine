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
| NETWORKING                               |
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
        private readonly System.IO.BinaryReader reader;
        // Contains a BinaryReader that wraps the above TcpClient.
        // Used so that we don't have to individually encode each byte into the stream.
        private readonly System.IO.BinaryWriter writer;
        // Contains the thread that handles reading of updates from the network stream.
        // Used for concurrently reading updates so that we don't block the main game loop waiting for one to completely arrive.
        private System.Threading.Thread updateReaderThread;
        // Contains the list of Updates that have been read by the updateReaderThread.
        // Used for passing events from the reader thread to the main game thread.
        private System.Collections.Generic.List<Update> updateBuffer = new System.Collections.Generic.List<Update>();
        // Contains a list of Events that need to be triggered when the connection ends.
        // Used so that the GameWorld can deal with the connection however it wants.
        public static System.Collections.Generic.List<EventHandling.Event> onDisconnect = new System.Collections.Generic.List<InteractionEngine.EventHandling.Event>();

        /// <summary>i
        /// Connect to a server at the specified IP.
        /// </summary>
        /// <param name="ipAddress">The IP address where we can find the server at.</param>
        public Server(string ipAddress) {
            if (InteractionEngine.Engine.status != Engine.Status.MULTIPLAYER_CLIENT)
                throw new GameWorldException("You can only construct a Server object on a client. This object is intended to manage interfacing with the server for a client.");
            this.tcpClient = new System.Net.Sockets.TcpClient();
            try {
                this.tcpClient.Connect(new System.Net.IPEndPoint(System.Net.IPAddress.Parse(ipAddress), Client.listeningPort));
            } catch (System.Net.Sockets.SocketException e) {
                throw new GameWorldException("The specified client could not be connected to.", e);
            }
            this.reader = new System.IO.BinaryReader(tcpClient.GetStream());
            this.writer = new System.IO.BinaryWriter(tcpClient.GetStream());
            this.updateReaderThread = new System.Threading.Thread(new System.Threading.ThreadStart(readUpdates));
            this.updateReaderThread.Start();
        }

        /// <summary>
        /// This method closes the current Server object.
        /// </summary>
        public void disconnect() {
            this.tcpClient.Close();
            Engine.server = null;
            Engine.status = Engine.Status.SINGLE_PLAYER;
            // Trigger the onQuit Events.
            foreach (EventHandling.Event eventObject in onDisconnect) {
                eventObject.parameter = (object)this;
                InteractionEngine.Engine.addEvent(eventObject);
            }
        }

        /// <summary>
        /// Send a packet containing an EventHandling.Event to the server.
        /// </summary>
        /// <param name="eventObject">The EventHandling.Event to send across.</param>
        public void sendEvent(EventHandling.Event eventObject) {
            try {
                writer.Write(eventObject.gameObjectID);
                writer.Write(eventObject.eventHash);
                if (eventObject.parameter == null) eventObject.parameter = new object();
                formatter.Serialize(tcpClient.GetStream(), eventObject.parameter);
            // The server dropped the connection.
            } catch (System.IO.IOException) {
                updateReaderThread.Abort();
                disconnect();
            } catch (System.Runtime.Serialization.SerializationException) {
                updateReaderThread.Abort();
                disconnect();
            }
        }

        /// <summary>
        /// Receive and process updates from the server.
        /// </summary>
        private void readUpdates() {
            // TODO: handle IO exceptions
            while (true) {
                try {
                    byte transferCode = reader.ReadByte();
                    switch (transferCode) {
                        case Update.CREATE_REGION:
                            Update createRegion = new CreateRegion(reader);
                            lock (updateBuffer) updateBuffer.Add(createRegion);
                            break;
                        case Update.DELETE_REGION:
                            Update deleteRegion = new DeleteRegion(reader);
                            lock (updateBuffer) updateBuffer.Add(deleteRegion);
                            break;
                        case Update.CREATE_OBJECT:
                            Update createObject = new CreateObject(reader, tcpClient.GetStream(), formatter);
                            lock (updateBuffer) updateBuffer.Add(createObject);
                            break;
                        case Update.DELETE_OBJECT:
                            Update deleteObject = new DeleteObject(reader);
                            lock (updateBuffer) updateBuffer.Add(deleteObject);
                            break;
                        case Update.MOVE_OBJECT:
                            Update moveObject = new MoveObject(reader);
                            lock (updateBuffer) updateBuffer.Add(moveObject);
                            break;
                        case Update.UPDATE_FIELD:
                            Update updateField = new UpdateField(reader, tcpClient.GetStream(), formatter);
                            lock (updateBuffer) updateBuffer.Add(updateField);
                            break;
                    }
                // The server dropped the connection.
                } catch (System.IO.IOException) {
                    disconnect();
                    break;
                } catch (System.Runtime.Serialization.SerializationException) {
                    disconnect();
                    break;
                }
            }
        }

        /// <summary>
        /// Read a list of Updates from the server.
        /// </summary>
        /// <returns>A list of Updates that we got from the server.</returns>
        internal System.Collections.Generic.List<Update> getUpdates() {
            System.Collections.Generic.List<Update> updates = new System.Collections.Generic.List<Update>();
            lock (updateBuffer) {
                updates.AddRange(updateBuffer);
                updateBuffer.Clear();
            }
            return updates;
        }

    }

}