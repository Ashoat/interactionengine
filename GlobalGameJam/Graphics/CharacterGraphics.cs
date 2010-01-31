using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GlobalGameJam.GameObjects;
using InteractionEngine.UserInterface.TwoDimensional;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GlobalGameJam.Graphics
{
    public class CharacterGraphics : EntityGraphics
    {
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

        public CharacterGraphics(Character entity)
            : base(entity)
        {
            this.entity = entity;
            frameChangeTimePrev = DateTime.Now;
        }

        public override void loadContent()
        {
            pointSpritesEffect = UserInterface2D.content.Load<Effect>("Effects/pointsprites");
            pointSpritesEffect.Parameters["SpriteTexture"].SetValue(
                UserInterface2D.content.Load<Texture2D>("fire"));
            
            spriteArray = new VertexPositionColor[120];
            vertexPosColDecl = new VertexDeclaration(UserInterface2D.graphicsDevice,
                VertexPositionColor.VertexElements);
            rand = new Random();
            for (int i = 0; i < spriteArray.Length; i++)
            {
                spriteArray[i].Position = new Vector3(rand.Next(100) / 10f,
                   rand.Next(100) / 10f, rand.Next(100) / 10f);
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
            spriteArray[rand.Next(0, 120)].Position =
                new Vector3(rand.Next(100) / 10f,
                   rand.Next(100) / 10f, rand.Next(100) / 10f);
            spriteArray[rand.Next(0, 120)].Position =
                new Vector3(rand.Next(100) / 10f,
                  rand.Next(100) / 10f, rand.Next(100) / 10f);
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
            Matrix WVPMatrix = Matrix.Identity * viewMatrix * projectionMatrix;
            pointSpritesEffect.Parameters["WVPMatrix"].SetValue(WVPMatrix);

            pointSpritesEffect.Begin();
            foreach (EffectPass pass in
                pointSpritesEffect.CurrentTechnique.Passes)
            {
                pass.Begin();
                UserInterface2D.graphicsDevice.DrawUserPrimitives
                    <VertexPositionColor>(
                        PrimitiveType.PointList,
                        spriteArray,
                        0,
                        spriteArray.Length);
                pass.End();
            }
            pointSpritesEffect.End();

            renderState.PointSpriteEnable = false;
            renderState.DepthBufferWriteEnable = true;
            renderState.SourceBlend = Blend.SourceAlpha;
            renderState.DestinationBlend =
                Blend.InverseSourceAlpha;
            if (true) // change to character running property
                UpdateAnimation();
            base.onDraw();
        }

    }
}
