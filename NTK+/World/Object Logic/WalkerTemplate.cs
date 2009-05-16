/*••••••••••••••••••••••••••••••••••••••••*\
| NTK+                                     |
| (C) Copyright Bluestone Coding 2009      |
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
| * WalkerTemplate                   Class |
\*••••••••••••••••••••••••••••••••••••••••*/

using InteractionEngine.Constructs;
using InteractionEngine.GameWorld;
using InteractionEngine.Client;
using NTKPlusGame.World.Modules;
using InteractionEngine.Client.ThreeDimensional;
using Microsoft.Xna.Framework;

namespace NTKPlusGame.World {

    /// <summary>
    /// A template for 'em walkin' types.
    /// </summary>
    public abstract class WalkerTemplate : GameObject, TerrainMovable {

        private const string MOVE_EVENT_HASH = "move";

        /// <summary>
        /// Returns the Graphics module of this GameObject.
        /// </summary>
        /// <returns>The Graphics module associated with this GameObject.</returns>
        public abstract InteractionEngine.Client.Graphics getGraphics();

        /// <summary>
        /// Returns the Selection module of this GameObject.
        /// </summary>
        /// <returns>The Selection module associated with this GameObject.
        private readonly Selection selection;
        public Selection getSelection() {
            return selection;
        }

        /// <summary>
        /// Returns the Stats module of this GameObject.
        /// </summary>
        /// <returns>The Stats module associated with this GameObject.
        private readonly Stats stats;
        public Stats getStats() {
            return stats;
        }

        /// <summary>
        /// Returns the TerrainMovement module of this GameObject.
        /// </summary>
        /// <returns>The TerrainMovement module associated with this GameObject.
        private readonly TerrainMovement terrainMovement;
        public Location getLocation() {
            return terrainMovement;
        }
        public TerrainLocation getTerrainLocation() {
            return terrainMovement;
        }
        public TerrainMovement getTerrainMovement() {
            return terrainMovement;
        }
        private readonly TerrainedLoadRegion loadRegion;
        public TerrainedLoadRegion getTerrainedLoadRegion() {
            return loadRegion;
        }

        public abstract Graphics3D getGraphics3D();

        /// <summary>
        /// Constructs a new WalkerTemplate.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        public WalkerTemplate(TerrainedLoadRegion loadRegion)
            : base(loadRegion) {
            this.loadRegion = loadRegion;
            this.selection = new Selection(this);
            this.stats = new Stats(this);
            this.terrainMovement = new TerrainMovement(this);
            this.addEvent(MOVE_EVENT_HASH, new EventMethod(onSelected));
        }
        
        /// <summary>
        /// Constructs a GameObject and assigns it an ID.
        /// This is the constructor that should be used if and only if you are a MULTIPLAYER_CLIENT.
        /// Furthermore, it is only called by the GameObjectFactory method.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        /// <param name="id">This GameObject's ID.</param>
        protected WalkerTemplate(LoadRegion loadRegion, int id)
            : base(loadRegion, id) {
        }

        /// <summary>
        /// Gets an Event from this Interactable module.
        /// This specific implementation fetches the onSelected EventMethod for left-mouse-presses.
        /// </summary>
        /// <param name="invoker">The invoker of this Event. If you have multiple possible invokers (ie. mouse click and mouse over) then we recommend you define constants for them.</param>
        /// <param name="user">The User that invokes this Event. Needed often for associating User invokers with GameObject invokers.</param>
        /// <param name="position">The position where the interaction happened, if applicable.</param>
        /// <returns>An Event.</returns>
        public Event getEvent(int invoker, Vector3 coordinates) {
            if (invoker == UserInterface3D.MOUSEMASK_LEFT_PRESS)
                return new Event(this.getID(), MOVE_EVENT_HASH, null);
            else return null;
        }

        /// <summary>
        /// EventMethod method... handles click events by registering this GameObject with the SelectionFocus.
        /// </summary>
        /// <param name="selectedBy">Um... probably null I guess.</param>
        public virtual void onSelected(object selectedBy) {
            NTKPlusUser.localUser.selectionFocus.addSelection(this, null);
        }


        /// <summary>
        /// Handles the event where another GameObject is selected while this one possesses the SelectionFocus.
        /// </summary>
        /// <param name="second">The other GameObject that was selected.</param>
        /// <param name="param">Any additional information provided by the second selection.</param>
        /// <returns>True if the second selection indicates an action-triggering option,
        /// false if the SelectionFocus should be transferred to the new selection.</returns>
        public virtual bool acceptSecondSelection(GameObjectable second, object param) {
            if (second is Terrain && param is Vector3) {
                this.getTerrainMovement().startWalking((Vector3)param);
                return true;
            }
            return false;
        }

    }

}