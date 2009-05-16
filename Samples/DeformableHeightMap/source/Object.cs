using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Bullshoot.Code
{
    public class TickContext
    {
        public GameTime time;
        public float       secs;
        public GamePadState padState;
        public GamePadState prevPadState;

        public TickContext(GameTime time, GamePadState padState, GamePadState prevPadState)
        {
            this.time = time;
            secs = (float)time.ElapsedGameTime.Milliseconds / 1000.0f;
            this.padState = padState;
            this.prevPadState = prevPadState;
        }
    }

    public class Orientation
    {
        private Vector3 _position;
        private Quaternion _rotationQ;
        private Vector3 _scale = Vector3.One;
        
        public Vector3 position
        {
            set { _position = value; isDirty = true; }
            get { return _position; }
        }

        public Quaternion rotationQ
        {
            set { _rotationQ = value; isDirty = true; }
            get { return _rotationQ; }
        }

        public Vector3 scale
        {
            set { _scale = value; isDirty = true; }
            get { return _scale; }
        }

        public Orientation() : this(Vector3.Zero, Quaternion.Identity) { }
        public Orientation( Vector3 position, Quaternion rotationQ )
        {
            this.position = position;
            this.rotationQ = rotationQ;
            this.scale = Vector3.One;
            UpdateMtx();
        }

        Boolean isDirty;
        Matrix basisMtx;
        Matrix invBasisMtx;

        public Matrix BasisMtx
        {
            get { return basisMtx; }
        }

        public Matrix InvBasisMtx
        {
            get { return invBasisMtx; }
        }

        public void UpdateMtx()
        {
            if (isDirty)
            {
                basisMtx = Matrix.CreateScale(scale) * Matrix.CreateFromQuaternion(rotationQ) * Matrix.CreateTranslation(position);
                invBasisMtx = Matrix.Invert(basisMtx);
            }
            isDirty = false;
        }

    };

    abstract public class Object
    {
        internal Orientation  orientation;

        public Object() { }
        public Object(Orientation orientation)
        {
            this.orientation = orientation;
        }

        abstract public void Draw(DrawContext context);
        abstract public void Tick(TickContext context);
    }
}
