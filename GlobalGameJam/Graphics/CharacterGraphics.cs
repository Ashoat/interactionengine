using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GlobalGameJam.GameObjects;
using InteractionEngine.UserInterface.TwoDimensional;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using InteractionEngine;

namespace GlobalGameJam.Graphics
{
    public class CharacterGraphics : EntityGraphics
    {
        private const int fireCount = 6;

        private DateTime frameChangeTimePrev;
        private TimeSpan frameChangeTimeDelay = new TimeSpan(5000000);// 0.5ms
        private int frameIndex = 0;
        private string[] textures;
        private Character entity;
        DateTime frameTimePrev = DateTime.Now;

        private Effect pointSpritesEffect;
        VertexPositionColor[] spriteArray;
        VertexDeclaration vertexPosColDecl;
        Random rand;
        
        Matrix projectionMatrix;
        Matrix viewMatrix;
        float tick;
        public CharacterGraphics(Character entity)
            : base(entity)
        {
            this.entity = entity;
            frameChangeTimePrev = DateTime.Now;
            tick = 0;
        }

        public override void loadContent()
        {
            pointSpritesEffect = UserInterface2D.content.Load<Effect>("Effects/pointsprites");
            pointSpritesEffect.Parameters["SpriteTexture"].SetValue(
                UserInterface2D.content.Load<Texture2D>("fire"));

            spriteArray = new VertexPositionColor[fireCount];
            vertexPosColDecl = new VertexDeclaration(UserInterface2D.graphicsDevice,
                VertexPositionColor.VertexElements);
            rand = new Random();
            for (int i = 0; i < spriteArray.Length; i++)
            {
                spriteArray[i].Position = new Vector3(rand.Next(100) / 10f,
                   rand.Next(100) / 10f, 0);
                spriteArray[i].Color = Color.WhiteSmoke;
            }
            viewMatrix =
                Matrix.CreateLookAt(Vector3.One * 25, Vector3.Zero, Vector3.Up);
            projectionMatrix =
                Matrix.CreatePerspectiveFieldOfView(Microsoft.Xna.Framework.MathHelper.PiOver4,
                4.0f / 3.0f, 1.0f, 10000f);
            base.loadContent();
        }

        public void setTextures(string[] textures)
        {
            this.textures = textures;
            frameIndex = 0;
            setTexture(textures[0]);
        }

        private void UpdateAnimation()
        {
            if (textures != null &&
                DateTime.Now - frameChangeTimePrev > frameChangeTimeDelay)
            {
                if (++frameIndex >= textures.Length)
                    frameIndex = 0;
                setTexture(textures[frameIndex]);
                frameChangeTimePrev = DateTime.Now;
            }
        }

        private void UpdateEffect()
        {
            // TODO: Add your update logic here
            spriteArray[rand.Next(0, fireCount)].Position =
                new Vector3(rand.Next(100) / 10f,
                   rand.Next(100) / 10f, 0);
            spriteArray[rand.Next(0, fireCount)].Position =
                new Vector3(rand.Next(100) / 10f,
                  rand.Next(100) / 10f, 0);
        }
        public override void onDraw()
        {
            UpdateEffect();
            
            RenderState renderState = UserInterface2D.graphicsDevice.RenderState;
            
            renderState.PointSpriteEnable = true;
            renderState.PointSize = 64.0f;
            renderState.AlphaBlendEnable = true;

            renderState.SourceBlend = Blend.SourceAlpha;
            //renderState.BlendFunction = BlendFunction.Subtract;
            renderState.DestinationBlend = Blend.One;
            renderState.DepthBufferWriteEnable = false;

            UserInterface2D.graphicsDevice.VertexDeclaration
                = vertexPosColDecl;
            Matrix WVPMatrix = Matrix.Identity; // *viewMatrix * projectionMatrix;
            pointSpritesEffect.Parameters["WVPMatrix"].SetValue(WVPMatrix);
            Texture2D fire = UserInterface2D.content.Load<Texture2D>("fire");
            pointSpritesEffect.Begin();
            if (gameObject is Player)
            foreach (EffectPass pass in
                pointSpritesEffect.CurrentTechnique.Passes)
            {
                pass.Begin();
                foreach (VertexPositionColor sprite in spriteArray) {
                    SpriteBatch sb = ((UserInterface2D)Engine.userInterface).spriteBatch;
                    Vector3 possy =  gameObject.getLocation().Position + sprite.Position;
                    sb.Draw(fire, new Vector2(possy.X, possy.Y - 10), null, Color.WhiteSmoke, 0, Vector2.Zero, 0.12f, SpriteEffects.None, 0.15f);
                }
                pass.End();
            }
            pointSpritesEffect.End();

            renderState.PointSpriteEnable = false;
            renderState.DepthBufferWriteEnable = true;
            renderState.SourceBlend = Blend.SourceAlpha;
            renderState.DestinationBlend =
                Blend.InverseSourceAlpha;
            
            tick+=32/3;
                
                int xSign = Math.Sign(entity.Position.X*32 - ((int)entity.getLocation().Position.X));
                int ySign = Math.Sign(entity.Position.Y*32+88 - ((int)entity.getLocation().Position.Y));
                if (((Character)entity).running) {
                    setPosition((int)(((Character)entity).oldPosition.X * 32 + tick*xSign), (int)(88+((Character)entity).oldPosition.Y * 32 + tick*ySign));
                    if (tick >= 32 && entity is Player) {
                        entity.running = false;
                        tick = 0;
                    }
                        
                } else {
                    tick = 0;
                }
            if (true) // change to character running property
                UpdateAnimation();
            base.onDraw();
        }

    }
}
