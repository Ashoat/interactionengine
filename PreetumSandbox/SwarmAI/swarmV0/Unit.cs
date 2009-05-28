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
    class Unit
    {
        float rotation = 0;
        Vector2 position = new Vector2(-1337,-1337);
        float radius = 20;
        float speed = 0;
        bool trailOn;

        SpriteBatch sb;
        Texture2D dot;
        Color color = Color.Red;

        Random rand;

        List<Vector2> trail;
        int trailLen = 100;

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
            get { return position; }
            set 
            {
                if (position.X == -1337 && position.Y == -1337) //if position has not yet been initialized
                {
                    rand = new Random((int)(value.X * value.Y));
                }
                position = value;
            }
        }
        public float Speed
        {
            get { return speed; }
            set
            {
                speed = value;
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
                speed = value.Length();
                rotation = (float)Math.Atan2(value.X, value.Y);
            }
        }
        public float Rotation
        {
            get { return rotation; }
            set
            {
                rotation = value;
            }
        }
        
        public Unit(SpriteBatch sb, Texture2D dot)
        {
            this.sb = sb;
            this.dot = dot;
            rand = new Random((int)(this.position.X * this.position.Y));
            trail = new List<Vector2>();
        }
        public void GetNewRandom()
        {
            rand = new Random((int)(this.position.X * this.position.Y));
        }
        public void randomWalkStep(float maxAngle)
        {
            this.Rotation += (float)(rand.NextDouble() * 2 - 1) * maxAngle;
            this.Update();
        }
        public Vector2 randomWalkHeading(float maxAngle) //returns randomWalkVector
        {
            float rot = this.Rotation + (float)(rand.NextDouble() * 2 - 1) * maxAngle;
            return Vector2.Transform(-Vector2.UnitY, Matrix.CreateRotationZ(rot)) * speed; //heading calculation
        }
        public void Displace(Vector2 disp)
        {
            this.Position += disp;
        }
        public void Update()
        {
            trail.Add(this.Position);
            if (trail.Count > trailLen)
                trail.RemoveAt(0);
            this.Position += this.Heading;
        }
        public void TrailOn()
        {
            trailOn = true;
        }
        public void Draw()
        {
            sb.Begin(SpriteBlendMode.AlphaBlend);
            if (trailOn)
                this.DrawTrailNoSb();
            this.drawDot(this.position, radius, color);
            sb.End();
        }
        public void DrawNoSbNoTrail() //no sb.Begin/End
        {
            this.drawDot(this.position, radius, color);
        }
        private void drawDot(Vector2 pos, float rad, Color c, byte alpha)
        {
            c.A = alpha;
            sb.Draw(dot, pos, null, c, rotation, new Vector2(dot.Width / 2, dot.Height / 2), 2 * rad / dot.Width, SpriteEffects.None, 0f);
        }
        private void drawDot(Vector2 pos, float rad, Color c)
        {
            sb.Draw(dot, pos, null, c, rotation, new Vector2(dot.Width / 2, dot.Height / 2), 2 * rad / dot.Width, SpriteEffects.None, 0f);
        }
        public void DrawTrailNoSb()
        {
            float rad = radius;
            float alpha = 255f;

            for(int i=trail.Count-1; i>=0;i--)
            {
                this.drawDot(trail[i], rad, Color.LightBlue, (byte)alpha);
                rad *= .98f;
                alpha *= .98f;
            }
        }
        public void DrawTrail()
        {
            sb.Begin(SpriteBlendMode.AlphaBlend);
            DrawTrailNoSb();
            sb.End();
        }
    }
}
