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
using System.ComponentModel;
using System.IO;

namespace Wumpus3Drev0
{
    class Animation
    {

        List<Model> frames = new List<Model>();
        int index = 0;
        string id;


        public List<Model> Frames
        {
            get { return frames; }
            set { frames = value; }
        }
        public int Index
        {
            get { return index; }
            set { index = value; }
        }
        public Model CurrentFrame
        {
            get { return frames[index]; }
        }
        public string Name
        {
            get { return id; }
            set { id = value; }
        }
        public Animation(string name)
        {
            this.id = name;
        }
        public Animation(Model[] frames, string name)
        {
            this.frames = frames.ToList<Model>();
            this.id = name;
        }
        public Animation(List<Model> frames, string name)
        {
            this.frames = frames.ToList<Model>();
            this.id = name;
        }
        /// <summary>
        /// Incriments the frame index
        /// </summary>
        /// <returns>Returns false if index is looped to 0 after incriment</returns>
        public bool NextFrame()
        {
            index++;
            if (index >= frames.Count)
            {
                index = 0;
                return false;
            }
            return true;
        }

    }
}
