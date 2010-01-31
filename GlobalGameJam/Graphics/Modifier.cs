using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GlobalGameJam.Graphics {
    public abstract class Modifier {
        protected bool _enabled;
        public bool Enabled {
            get { return _enabled; }
            set { _enabled = value; }
        }

        public abstract void Update(Particle particle, float elapsed);

        public abstract void Update(Particle particle, float elapsed, int i);
    }
}
