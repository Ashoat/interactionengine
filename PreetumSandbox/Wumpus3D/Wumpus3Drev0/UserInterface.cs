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
//using Microsoft.Xna.Framework.Content.Pipeline;
//using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
//using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using System.IO;

namespace Wumpus3Drev0
{
    class UserInterface
    {
        List<GameModel> models;
        BasicCamera camera;
        GraphicsDevice GraphicsDevice;

        Terrain terrain;

        Swarm swarm;
        Attractor attr = null;

        public UserInterface(BasicCamera camera, Terrain terrain, GraphicsDevice GraphicsDevice)
        {
            this.camera = camera;
            models = new List<GameModel>();
            this.GraphicsDevice = GraphicsDevice;
            this.terrain = terrain;
            swarm = new Swarm();
        }
        public void Add(GameModel model)
        {
            models.Add(model);
            Unit unit = new Unit();
            unit.Position = model.Position2;
            swarm.Units.Add(unit);
        }
        public void Remove(GameModel model)
        {
            models.Remove(model);
        }

        public void Update()
        {
            Ray mouseRay = getMouseRay();
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                swarm.Attractor = new Attractor();
                Vector3? tmp = this.terrain.RayIntersects(mouseRay);
                swarm.Attractor.Position = new Vector2(tmp.Value.X, tmp.Value.Z);
            }
            else
            {
                //swarm.Attractor = null;
            }

            swarm.Update();
            
        }
        public void Draw()
        {
            foreach (GameModel m in models)
            {
                m.Draw();
            }
        }
        public Ray getMouseRay()
        {
            Vector2 mouse = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);

            Vector3 near = new Vector3(mouse.X, mouse.Y, 0f);
            Vector3 far = new Vector3(mouse.X, mouse.Y, 1f);
            Matrix world = Matrix.Identity;

            Vector3 nearPt = GraphicsDevice.Viewport.Unproject(near, camera.Projection, camera.View, world);
            Vector3 farPt = GraphicsDevice.Viewport.Unproject(far, camera.Projection, camera.View, world);

            Vector3 dir = farPt - nearPt;
            dir.Normalize();

            return new Ray(camera.Position, dir);
        }

    }
}
