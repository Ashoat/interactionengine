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

namespace swarmV0
{
    class Swarm
    {
        List<Unit> units;

        SpriteBatch sb;
        Texture2D dot;

        Random rand;

        public List<Unit> Units
        {
            get { return units; }
            set { units = value; }
        }
        public Swarm(SpriteBatch sb, Texture2D dotTex)
        {
            this.sb = sb;
            units = new List<Unit>();
            rand = new Random();
            this.dot = dotTex;
        }
        public void Update()
        {
            if (units == null) return;
            foreach (Unit unit in units)
            {
                unit.randomWalkStep(MathHelper.ToRadians(20));
            }
        }
        public void PopulateSwarm(int unitNum, float radius, float speed, int width, int height)
        {
            for (int i = 0; i < unitNum; i++)
            {
                Unit newUnit = new Unit(sb, dot);
                newUnit.Position = new Vector2( (float)rand.NextDouble() * width, (float)rand.NextDouble() * height);
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
