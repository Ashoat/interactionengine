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
| * Bat                              Class |
\*••••••••••••••••••••••••••••••••••••••••*/

using WumpusGame.World.Modules;
using InteractionEngine.Constructs;
using System.Collections.Generic;
using InteractionEngine.Constructs.Datatypes;
using WumpusGame.Modules;
using WumpusGame.World.Graphics;
using InteractionEngine.GameWorld;
using InteractionEngine.Client;

namespace WumpusGame.World {

    /**
     * The Bat. It's this big kind-hearted creature that tries to peacefully remove cruel hunters from the cave in order to save the poor Wumpus.
     */
    
    public class Bat : GameObject, Conversable, Graphable, Locatable {

#region FACTORY

        // The classHash, a unique identifying string for the class. Hmm, wow, that's kind of redundant, isn't that? C# already provides such a function through reflection. Oh well.
        // Used for the factory methods called when the client receives a CREATE_NEW_OBJECT update from the server computer.
        internal const string classHash = "bat";

        /// <summary>
        /// The static constructor. Adds the class's factory method to the GameObject factoryList when the class is first loaded.
        /// </summary>
        static Bat() {
            GameObject.factoryList.Add(classHash, new GameObjectFactory(makeBat));
        }

        /// <summary>
        /// A factory method that creates and returns a new instance of Player. Used by the client when the server requests it to make a new GameObject.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        /// <param name="id">This GameObject's ID.</param>
        /// <param name="reader">The PacketReader from which we will read the fields of the newly constructed GameObject.</param>
        /// <returns>A new instance of Bat.</returns>
        static Bat makeBat(LoadRegion loadRegion, int id, Microsoft.Xna.Framework.Net.PacketReader reader) {
            if (GameWorld.status != GameWorld.Status.MULTIPLAYER_CLIENT)
                throw new System.Exception("You're not a client, so why are you calling the GameObject factory method?");
            Bat bat = new Bat(loadRegion, id);
            // ORDER OF STUFF (where you used the reader to construct datatypes, used factory methods exclusively. also, construct modules and their datatypes here too.)
            return bat;
        }

        /// <summary>
        /// Constructs a GameObject and assigns it an ID.
        /// This is the constructor that should be used if and only if you are a MULTIPLAYER_CLIENT.
        /// Furthermore, it is only called by the GameObjectFactory method.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        /// <param name="id">This GameObject's ID.</param>
        private Bat(LoadRegion loadRegion, int id) : base(loadRegion, id) {
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

        /// <summary>
        /// Returns the Location module of this GameObject.
        /// </summary>
        /// <returns>The Location module associated with this GameObject.
        private Location location;
        public Location getLocation() {
            return location;
        }
            
        /// <summary>
        /// Returns the Graphics module of this GameObject.
        /// </summary>
        /// <returns>The Graphics module associated with this GameObject.
        private Bat2DGraphics graphics;
        public InteractionEngine.Client.Graphics getGraphics() {
            return graphics;
        }

        /// <summary>
        /// Returns the Conversation module of this GameObject.
        /// </summary>
        /// <returns>The Conversation module associated with this GameObject.
        private Conversation conversation ;
        public Conversation getConversation() {
            return conversation;
        }

        /// <summary>
        /// Constructs a new Bat GameObject.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        public Bat(Room room) : base(room.loadRegion) {
            location = new Location(this);
            graphics = new Bat2DGraphics(this);
            conversation = new Conversation(this, "Bat");
            room.roomEntered += new RoomEnteredListener(objectEnteredRoom);
        }

        /// <summary>
        /// The discussion that should be displayed to the player when s/he enters the same room that the bat is in.
        /// </summary>
        private class RoomEnteredDiscussion : Discussion {

#region FACTORY

            // The classHash, a unique identifying string for the class. Hmm, wow, that's kind of redundant, isn't that? C# already provides such a function through reflection. Oh well.
            // Used for the factory methods called when the client receives a CREATE_NEW_OBJECT update from the server computer.
            internal const string classHash = "Bat.RoomEnteredDiscussion";

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
            private readonly UpdatableGameObject<Bat> bat;

            /// <summary>
            /// Constructs a new RoomEnteredDiscussion object, and sets its dialogue.
            /// </summary>
            /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
            /// <param name="player">The player who entered the room that the bat is in.</param>
            /// <param name="bat">The bat whose conversation this is.</param>
            public RoomEnteredDiscussion(LoadRegion loadRegion, Player player, Bat bat) : this(loadRegion) {
                this.player = new UpdatableGameObject<Player>(this);
                this.bat = new UpdatableGameObject<Bat>(this);
                this.player.value = player;
                this.bat.value = bat;
            }

            /// <summary>
            /// Constructs a new RoomEnteredDiscussion object, and sets its dialogue.
            /// </summary>
            /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
            public RoomEnteredDiscussion(LoadRegion loadRegion) : base(loadRegion) {

                messages = new string[2];
                usingFaces = new bool[] { true, false };
                options = new string[2][];

                messages[0] = "*squeek!* Gasp! Another poacher come to kill our big fuzzy Wumpus friend?! Not on my watch! Come, comrades, let us gloriously unite and stand our ground against this selfish capitalist swine!";
                options[0] = null;

                messages[1] = "You turn around and quickly run away... but did you really think you could outrun bats? They swoop down, lift you up, and start carrying you away as you struggle helplessly. After a while though, the bats start squabbling over weight-lifting quotas, and eventually they drop you into darkness. You hit your head on the hard rock floor.";
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
            /// Whisk the player away to a different room now that the conversation has ended, and move the bats somewhere else too.
            /// </summary>
            private void conversationEndAction() {
                player.value.getLocation().move(player.value.getLocation().getRoomLocation().getRandomRoom());
                Room randomRoom;
                do {
                    randomRoom = bat.value.getLocation().getRoomLocation().getRandomRoom();
                } while (!randomRoom.contains<Bat>() && !randomRoom.contains<Pit>() && !randomRoom.contains<World.Player>());
                bat.value.location.getRoomLocation().roomEntered -= bat.value.objectEnteredRoom;
                bat.value.location.move(randomRoom);
                randomRoom.roomEntered += new RoomEnteredListener(bat.value.objectEnteredRoom);
            }

        }

        /*••••••••••••••••••••••••••••••••••••••••*\
          ACTIONS
        \*••••••••••••••••••••••••••••••••••••••••*/

        /// <summary>
        /// This method handles the objectEnteredRoom event. If it's a player, then it proceeds to go do other stuff.
        /// </summary>
        /// <param name="user">The object that entered the room.</param>
        public void objectEnteredRoom(Locatable enterer) {
            if (enterer is Player) playerEntersRoom((Player)enterer);
        }

        /// <summary>
        /// This method is triggered when a player enters the Room.
        /// It displays the bat graphics, then whisks the player and the bat into other rooms.
        /// </summary>
        /// <param name="player">The player who entered the room.</param>
        public void playerEntersRoom(Playable player) {
            conversation.startConversation(player, new RoomEnteredDiscussion(loadRegion, (Player)player, this));
        }

    }

}