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

namespace MoveRepulsV0
{
    class Swarm
    {
        List<Unit> units;

        SpriteBatch sb;
        Texture2D dot;

        Attractor attractor = null;

        private const float hookeConst = .000005f; //.000005f
        private const float gravConst = .00000001f;

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
        public Swarm(SpriteBatch sb, Texture2D dotTex)
        {
            this.sb = sb;
            units = new List<Unit>();
            this.dot = dotTex;
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

        public void Update()
        {
            if (units == null) return;
            foreach (Unit unit in units)
            {
                unit.Update();

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
                unit.Velocity *= .96f;
                unit.Position += unit.Velocity;
            }
        }
        public void PopulateSwarm(int unitNum, float radius, float speed, int width, int height)
        {
            Random rand = new Random();
            for (int i = 0; i < unitNum; i++)
            {
                Unit newUnit = new Unit(sb, dot);
                newUnit.Position = new Vector2((float)rand.NextDouble() * width, (float)rand.NextDouble() * height);
                newUnit.TrailOn();
                newUnit.Speed = speed;
                newUnit.Radius = radius;

                this.units.Add(newUnit);
            }
        }
        public void Draw()
        {
            if (units == null) return;
            sb.Begin(SpriteBlendMode.AlphaBlend);
            foreach (Unit unit in units)
            {
                unit.DrawTrailNoSb();
            }
            foreach (Unit unit in units)
            {
                unit.DrawNoSbNoTrail();
            }
            sb.End();
        }

    }
}
