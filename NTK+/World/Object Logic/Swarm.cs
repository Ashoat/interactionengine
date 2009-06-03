/*••••••••••••••••••••••••••••••••••••••••*\
| NTK+   Game                              |
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
| WORLD                                    |
| * Swarm                           Class |
\*••••••••••••••••••••••••••••••••••••••••*/

using InteractionEngine.Constructs;
using InteractionEngine;
using InteractionEngine.Networking;
using NTKPlusGame.World.Modules;
using InteractionEngine.EventHandling;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace WumpusGame.World
{

    public class Unit
    {
        float rotation;
        Vector2 pos;

        public Vector2 Velocity = Vector2.Zero;

        float radius = 20;

        public float Mass = 20; //20

        bool trailOn;

        SpriteBatch sb;
        Texture2D dot;
        Color color = Color.Red;

        float repul = .5f; //1.3

        List<Vector2> trail;
        int trailLen = 100;
        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }
        public float Repulsion
        {
            get { return repul; }
            set { repul = value; }
        }
        public bool HasTrail
        {
            get { return trailOn; }
            set { trailOn = value; }
        }
        public float Radius
        {
            get { return radius; }
            set { radius = value; }
        }
        public Vector2 Position
        {
            get { return pos; }
            set
            {
                this.Heading = Vector2.Normalize(value - pos);
                pos = value;
            }
        }

        public Vector2 Heading
        {
            get
            {
                return Vector2.Transform(-Vector2.UnitY, Matrix.CreateRotationZ(rotation));
            }
            set
            {
                //speed = value.Length();
                rotation = (float)Math.Atan2(value.X, -value.Y);
            }
        }

        public Unit(SpriteBatch sb, Texture2D dot)
        {
            this.sb = sb;
            this.dot = dot;
            trail = new List<Vector2>();
        }
        public Unit()
        {
            trail = new List<Vector2>();
        }

    }
    public class Attractor
    {
        public Vector2 Position = new Vector2(0, 0);
        public float Mass = 120; //120
        public float Repulsion = .5f;

        public Attractor()
        {
        }
        public Attractor(Vector2 pos, float mass)
        {
            this.Mass = mass;
            this.Position = pos;
        }
    }

    public class Swarm : GameObject
    {

        #region FACTORY

        /// <summary>
        /// All GameObjects need a parameterless constructor for calling by GameObject.createGameObject() and GameObject.createFromUpdate().
        /// NEVER CALL THIS! This constructor is exclusively for use by the InteractionEngine. If anyone else calls it things will break.
        /// If you want to construct this object, use GameObject.createGameObject(LoadRegion).
        /// </summary>
        public Swarm()
        {
        }

        // The classHash, a unique identifying string for the class. Hmm, wow, that's kind of redundant, isn't that? C# already provides such a function through reflection. Oh well.
        // Used for the factory methods called when the client receives a CREATE_NEW_OBJECT update from the server computer.
        public const string realHash = "Swarm";
        public override string classHash
        {
            get { return realHash; }
        }

        /// <summary>
        /// The static constructor. Adds the class's factory method to the GameObject factoryList when the class is first loaded.
        /// </summary>
        static Swarm()
        {
            GameObject.factoryList.Add(realHash, new GameObjectFactory(GameObject.createFromUpdate<Swarm>));
        }

        #endregion

        /*••••••••••••••••••••••••••••••••••••••••*\
          MODULES
        \*••••••••••••••••••••••••••••••••••••••••*/

        List<Unit> units = new List<Unit>();
        public Dictionary<Unit, TerrainMovable> UnitToModel;

        SpriteBatch sb;
        Texture2D dot;

        Attractor attractor = null;

        private const float hookeConst = .000005f; //.000005f
        private const float gravConst = .00000001f;

        public float Dampening = .96f; //IMPORTANT!!!!

        public Attractor Attractor
        {
            get { return attractor; }
            set { attractor = value; }
        }
        public List<Unit> Units
        {
            get { return units; }
            set { units = value; }
        }

        public Vector2 CoulombRepulsion(Unit unit, Attractor attr)  //ON node1 BY node2
        {
            Vector2 direction = Vector2.Subtract(unit.Position, attr.Position);
            direction.Normalize();

            if (Vector2.DistanceSquared(unit.Position, attr.Position) != 0)
            {
                return attr.Mass * attr.Repulsion / Vector2.DistanceSquared(unit.Position, attr.Position) * direction;
            }
            else
            {
                return Vector2.Zero;
            }
        }

        public Vector2 CoulombRepulsion(Unit unit, Unit attr)  //ON node1 BY node2
        {
            Vector2 direction = Vector2.Subtract(unit.Position, attr.Position);
            direction.Normalize();

            if (Vector2.DistanceSquared(unit.Position, attr.Position) != 0)
            {
                return attr.Mass * attr.Repulsion / Vector2.DistanceSquared(unit.Position, attr.Position) * direction;
            }
            else
            {
                return Vector2.Zero;
            }
        }

        public Vector2 HookeAttraction(Unit unit, Attractor attr) //ON node1 BY node2
        {
            Vector2 direction = attr.Position - unit.Position;
            direction.Normalize();

            return hookeConst * attr.Mass * Vector2.Distance(unit.Position, attr.Position) * direction;
        }

        public float CalcTotalEnergy()
        {
            float totEng = 0;
            foreach (Unit unit in units)
            {
                totEng += unit.Velocity.Length() * unit.Velocity.Length() * unit.Mass;
            }
            return totEng;
        }

        public void Update()
        {
            if (units == null) return;
            if (attractor == null) return;
            foreach (Unit unit in units)
            {

                Vector2 netForce = Vector2.Zero;
                foreach (Unit otherUnit in units)
                {
                    netForce += CoulombRepulsion(unit, otherUnit);
                }
                if (this.Attractor != null)
                {
                    netForce += CoulombRepulsion(unit, this.Attractor);
                    netForce += HookeAttraction(unit, this.Attractor);
                }

                unit.Velocity += netForce;
                unit.Velocity *= this.Dampening;
                unit.Position += unit.Velocity;
            }
        }
        public void PopulateSwarm(List<TerrainMovable> models)
        {
            foreach (TerrainMovable m in models)
            {
                Unit unit = new Unit();
                unit.Position = new Vector2(m.getLocation().Position.X, m.getLocation().Position.Z);
                this.units.Add(unit);
            }
        }


        /*••••••••••••••••••••••••••••••••••••••••*\
          MEMBERS
        \*••••••••••••••••••••••••••••••••••••••••*/

        private const string SWARM_HASH = "swarmy423423knrbfous4t";

        /// <summary>
        /// Constructs the Swarm.
        /// </summary>
        public override void construct() {
            this.addEventMethod(SWARM_HASH, new EventMethod(updateSwarm));
            Engine.addEvent(new Event(this.id, SWARM_HASH, null));
        }
        
        public void updateSwarm(Client client, object param)
        {
            this.Update();
            foreach(Unit u in UnitToModel.Keys)
            {
                TerrainMovable model;
                UnitToModel.TryGetValue(u, out model);
                model.getTerrainMovement().Position = new Vector3(u.Position.X, 0, u.Position.Y);
                model.getTerrainMovement().yaw = MathHelper.ToDegrees(u.Rotation);
            }
            Engine.addEvent(new Event(this.id, SWARM_HASH, null));
        }

    }

}