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
        GameModel selectedModel;
        BasicCamera camera;
        GraphicsDevice GraphicsDevice;

        public UserInterface(BasicCamera camera, GraphicsDevice GraphicsDevice)
        {
            selectedModel = null;
            this.camera = camera;
            models = new List<GameModel>();
            this.GraphicsDevice = GraphicsDevice;
        }
        public void Add(GameModel model)
        {
            models.Add(model);
        }
        public void Remove(GameModel model)
        {
            models.Remove(model);
        }

        public void Update()
        {
            /*if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                Ray mouseRay = getMouseRay();
                if (this.selectedModel != null)
                {
                    Vector3? intersect = this.selectedModel.Terrain.RayIntersects(mouseRay);
                    if (intersect.HasValue) // this will always be true, with the current method
                    {
                        if (!this.selectedModel.RayIntersects(mouseRay))
                        {
                            selectedModel.MoveTo(new Vector2(intersect.Value.X, intersect.Value.Z), 0.001f);
                            //selectedModel.SetTarget(new Vector2(intersect.Value.X, intersect.Value.Z));
                        }
                    }
                }
                this.selectedModel = null;
                foreach (GameModel model in this.models)
                {
                    if (model.RayIntersects(mouseRay))
                    {
                        this.selectedModel = model;
                        return;
                    }
                }
            }

            foreach (GameModel model in this.models)
            {
                model.UpdateMove();
            }*/

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
