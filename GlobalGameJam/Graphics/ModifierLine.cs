using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GlobalGameJam.Graphics {
    class ModifierLine: Modifier {


        public Vector2 Direction {
            get { return direction; }
            set { direction = value; }
        }
        public Point StartPoint {
            get { return sp; }
            set { sp = value; }
        }
        private Vector2 direction;
        private Point sp;
        private float prevElapsed;
        private int tick;
        public ModifierLine(Vector2 direction, Point startPoint) {
            this.direction = direction;
            this.sp = startPoint;
        }

        public override void Update(Particle particle, float elapsed) {
            Update(particle, elapsed, 0);
        }
        public override void Update(Particle particle, float elapsed, int i) {
            
            particle.Velocity = 500*direction;
            particle.Position += direction * i;
        }
    }
}
