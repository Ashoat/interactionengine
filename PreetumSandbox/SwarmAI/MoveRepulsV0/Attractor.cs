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
    class Attractor
    {
        public Vector2 Position = new Vector2(0, 0);
        public float Mass = 120;
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
}
