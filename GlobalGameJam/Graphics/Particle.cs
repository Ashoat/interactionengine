using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GlobalGameJam.Graphics {
    public class Particle {
        private float opacity = 1.0f;
        private Color color;

        private float _age = 0.0f;
        public float Age {
            get { return _age; }
            set { _age = value; }
        }

        private float _lifeTime = 0.0f;
        public float LifeTime {
            get { return _lifeTime; }
            set { _lifeTime = value; }
        }

        private Vector2 _position = Vector2.Zero;
        public Vector2 Position {
            get { return _position; }
            set { _position = value; }
        }

        private Vector2 _velocity = Vector2.Zero;
        public Vector2 Velocity {
            get { return _velocity; }
            set { _velocity = value; }
        }

        public bool started;
        public Particle(float lifeTime, Vector2 position, Vector2 velocity) {
            _lifeTime = lifeTime;
            _position = position;
            _velocity = velocity;
        }

        public void Update(float elapsed) {
            _position += _velocity * elapsed;
            _age += elapsed;

            //calc fade effect
            opacity = (float)Math.Sin(_age);
            opacity = (opacity + 1.0f) / 2.0f;
            color = new Color(new Vector4(1.0f, 1.0f, 1.0f, opacity));
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D texture) {
            if (_age <= _lifeTime) {
                spriteBatch.Draw(
                    texture,
                    _position,
                    null,
                    color,
                    0.0f,
                    Vector2.Zero,
                    0.1f,
                    SpriteEffects.None,
                    0.0f);
            }
        }
    }
}
