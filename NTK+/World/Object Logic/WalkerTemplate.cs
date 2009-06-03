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
using InteractionEngine;
using InteractionEngine.UserInterface;
using NTKPlusGame.World.Modules;
using InteractionEngine.UserInterface.ThreeDimensional;
using Microsoft.Xna.Framework;
using InteractionEngine.EventHandling;
using InteractionEngine.Networking;

namespace NTKPlusGame.World {

    /// <summary>
    /// A template for 'em walkin' types.
    /// </summary>
    public abstract class WalkerTemplate : GameObject, TerrainMovable, Graphable3D {

        private const string MOVE_EVENT_HASH = "move";
        private const string MULTI_MOVE_HASH = "multimove";

        /// <summary>
        /// Returns the Graphics module of this GameObject.
        /// </summary>
        /// <returns>The Graphics module associated with this GameObject.</returns>
        public abstract InteractionEngine.UserInterface.Graphics getGraphics();

        /// <summary>
        /// Returns the Stats module of this GameObject.
        /// </summary>
        /// <returns>The Stats module associated with this GameObject.
        private Stats stats;
        public Stats getStats() {
            return stats;
        }

        /// <summary>
        /// Returns the TerrainMovement module of this GameObject.
        /// </summary>
        /// <returns>The TerrainMovement module associated with this GameObject.
        private TerrainMovement terrainMovement;
        public Location getLocation() {
            return terrainMovement;
        }
        public TerrainLocation getTerrainLocation() {
            return terrainMovement;
        }
        public TerrainMovement getTerrainMovement() {
            return terrainMovement;
        }
        private Terrain terrain;
        public Terrain getTerrain() {
            return terrain;
        }

        public abstract Graphics3D getGraphics3D();

        public WalkerTemplate() {

        }

        public virtual void initialize(Terrain terrain) {
            this.terrain = terrain;
            this.terrainMovement.Position = Vector3.Zero;
        }

        /// <summary>
        /// Constructs a new WalkerTemplate.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        public override void construct() {
            this.stats = new Stats(this);
            this.terrainMovement = new TerrainMovement(this);
            this.addEventMethod(MOVE_EVENT_HASH, new EventMethod(onSelected));
            this.addEventMethod(MULTI_MOVE_HASH, new EventMethod(onDragSelected));
        }

        /// <summary>
        /// Gets an Event from this Interactable module.
        /// This specific implementation fetches the onSelected EventMethod for left-mouse-presses.
        /// </summary>
        /// <param name="invoker">The invoker of this Event. If you have multiple possible invokers (ie. mouse click and mouse over) then we recommend you define constants for them.</param>
        /// <param name="user">The User that invokes this Event. Needed often for associating User invokers with GameObject invokers.</param>
        /// <param name="position">The position where the interaction happened, if applicable.</param>
        /// <returns>An Event.</returns>
        public virtual Event getEvent(int invoker, Vector3 coordinates) {
            if (invoker == UserInterface3D.MOUSEMASK_LEFT_CLICK) {
                return new Event(this.id, MOVE_EVENT_HASH, null);
            } else if (invoker == UserInterface3D.MOUSEMASK_LEFT_DRAG) {
                return new Event(this.id, MULTI_MOVE_HASH, null);
            } else if (invoker == UserInterface3D.MOUSEMASK_OVER) {
                //this.getGraphics3D().Effect.AmbientLightColor = highlight(this.getGraphics3D().Effect.AmbientLightColor, 0.5f);
                //this.getGraphics3D().Effect.DiffuseColor = highlight(this.getGraphics3D().Effect.DiffuseColor, 0.5f);
                //this.getGraphics3D().Effect.SpecularColor = highlight(this.getGraphics3D().Effect.SpecularColor, 0.5f);
                this.getGraphics3D().Effect.SpecularPower *= 0.05f;
                this.getGraphics3D().Effect.CommitProperties();
                return null;
            } else if (invoker == UserInterface3D.MOUSEMASK_OUT) {
                //this.getGraphics3D().Effect.AmbientLightColor = highlight(this.getGraphics3D().Effect.AmbientLightColor, 2f);
                //this.getGraphics3D().Effect.DiffuseColor = highlight(this.getGraphics3D().Effect.DiffuseColor, 2f);
                //this.getGraphics3D().Effect.SpecularColor = highlight(this.getGraphics3D().Effect.SpecularColor, 2f);
                this.getGraphics3D().Effect.SpecularPower *= 20f;
                this.getGraphics3D().Effect.CommitProperties();
                return null;
            } else return null;
        }

        private Vector3 highlight(Vector3 input, float scale) {
            Vector3 inverse = Vector3.One - input;
            inverse = Vector3.Multiply(inverse, scale);
            return Vector3.One - inverse;
        }

        /// <summary>
        /// EventMethod method... handles click events by registering this GameObject with the SelectionFocus.
        /// </summary>
        /// <param name="selectedBy">Um... probably null I guess.</param>
        public virtual void onSelected(Client client, object selectedBy) {
            NTKPlusUser.localUser.selectionFocus.setSelection(this, client, null);
        }

        /// <summary>
        /// EventMethod method... handles click events by registering this GameObject with the SelectionFocus.
        /// </summary>
        /// <param name="selectedBy">Um... probably null I guess.</param>
        public virtual void onDragSelected(Client client, object selectedBy) {
            NTKPlusUser.localUser.selectionFocus.addAsMultipleSelection(this);
        }


        /// <summary>
        /// Handles the event where another GameObject is selected while this one possesses the SelectionFocus.
        /// </summary>
        /// <param name="second">The other GameObject that was selected.</param>
        /// <param name="param">Any additional information provided by the second selection.</param>
        /// <returns>True if the second selection indicates an action-triggering option,
        /// false if the SelectionFocus should be transferred to the new selection.</returns>
        public virtual bool acceptSecondSelection(GameObjectable second, Client client, object param) {
            if (second is Terrain && NTKPlusUser.isOnOurTeam(client) && param is Vector3) {
                this.getTerrainMovement().startWalking((Vector3)param);
                return true;
            }
            return false;
        }

    }

}