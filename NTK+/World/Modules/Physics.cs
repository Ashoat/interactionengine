/*••••••••••••••••••••••••••••••••••••••••*\
| NTK+ Game                                |
| (C) Copyright Bluestone Coding 2009      |
|••••••••••••••••••••••••••••••••••••••••••|
|           __    ___ ___  ___             |
|          /++\  | _ ) __|/ __|            |
|          \++/  | _ \__ \ (__             |
|           \/   |___/___/\___|            |
|                                          |
|••••••••••••••••••••••••••••••••••••••••••|
| MODULE                                   |
| * Physics                       Class     |
| * Physable                   Interface |
\*••••••••••••••••••••••••••••••••••••••••*/

using InteractionEngine.Constructs.Datatypes;
using InteractionEngine.Constructs;
using InteractionEngine.GameWorld;
using Microsoft.Xna.Framework;

namespace NTKPlusGame.World.Modules {

    /**
     * Holds all information and methods regarding Physics.
     * That is, all information necessary for the GameObject to interact with other GameObjects in collisions.
     * This includes:
     * -- Bounding box/sphere
     */

    public class Physics {

        // Contains a reference to the GameObject this Physics module is associated with.
        // Used for constructing Updatables.
        private readonly Physable gameObject;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="gameObject">The GameObject whose Physics this is.</param>
        public Physics(Physable gameObject) {
            this.gameObject = gameObject;
        }

        /// <summary>
        /// Constructor for the client-side.
        /// </summary>
        /// <param name="gameObject">The GameObject of whose Physics this is.</param>
        /// <param name="reader">The reader from which to read teh field datas.</param>
        internal Physics(GameObject gameObject, Microsoft.Xna.Framework.Net.PacketReader reader) {
            /*byte transferCode = reader.ReadByte();
            UpdatableInteger intty = (UpdatableInteger)GameWorld.createField(reader);
            roomPhysics = new UpdatableGameObject<Room>(intty);
            if (reader.ReadByte() == GameWorld.UPDATE_FIELD) GameWorld.updateField(reader);
            else reader.Position--;
            this.gameObject = gameObject;*/
        }

        // TODO

    }

    /**
     * Implemented by GameObjects that have the Physics module.
     */
    public interface Physable : Locatable {

        /// <summary>
        /// Returns the Physics module of this GameObject.
        /// </summary>
        /// <returns>The Physics module associated with this GameObject.
        Physics getPhysics();

    }

}