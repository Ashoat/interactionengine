using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GlobalGameJam.Graphics {
    public class Emitter {
        private List<Particle> _particles = new List<Particle>();

        private List<Modifier> _modifiers = new List<Modifier>();
        public List<Modifier> Modifiers {
            get { return _modifiers; }
            set { _modifiers = value; }
        }

        private Texture2D _texture = null;
        public Texture2D Texture {
            get { return _texture; }
            set { _texture = value; }
        }

        private Vector2 _position = Vector2.Zero;
        public Vector2 Position {
            get { return _position; }
            set { _position = value; }
        }

        private float _lifeTime = 5.0f;
        public float LifeTime {
            get { return _lifeTime; }
            set { _lifeTime = value; }
        }

        private long _particleCount = 100;
        public long ParticleCount {
            get { return _particleCount; }
            set { _particleCount = value; }
        }

        private Random _random = new Random();
        private Vector2 RandomVector(int minValue, int maxValue) {
            return new Vector2(
                _random.Next(minValue, maxValue),
                _random.Next(minValue, maxValue));
        }

        public Emitter(Texture2D texture, Vector2 position, float lifeTime, long particleCount) {
            _texture = texture;
            _position = position;
            _lifeTime = lifeTime;
            _particleCount = particleCount;

            for (int i = 0; i < _particleCount; i++) {
                _particles.Add(new Particle(
                    _lifeTime,
                    _position,
                    RandomVector(-150, 150)));
            }
        }

        public void Reset() {
            for (int i = 0; i < _particleCount; i++) {
                _particles[i].Age = 0.0f;
                _particles[i].Position = _position;
                _particles[i].Velocity = RandomVector(-150, 150);
            }
        }

        public void Update(float elapsed) {
            for (int i = 0; i < _particles.Count; i++) {
                for (int j = 0; j < _modifiers.Count; j++) {
                    _modifiers[j].Update(_particles[i], elapsed, i);
                }

                _particles[i].Update(elapsed);
            }
        }

        public void Draw(SpriteBatch spriteBatch) {
            for (int i = 0; i < _particles.Count; i++) {
                _particles[i].Draw(spriteBatch, _texture);
            }
        }
    }
}
