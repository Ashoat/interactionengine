using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GlobalGameJam.GameObjects;
using InteractionEngine.UserInterface.TwoDimensional;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using InteractionEngine;

namespace GlobalGameJam.Graphics {
    public class CharacterGraphics : EntityGraphics {
        private const int fireCount = 6;

        private DateTime frameChangeTimePrev;
        private TimeSpan frameChangeTimeDelay = new TimeSpan(5000000);// 0.5ms
        private int frameIndex = 0;
        private string[] textures;
        private Emitter attack_emitter;
        private Emitter shift_emitter;
        private bool attacking;
        private bool shifting;
        private List<Modifier> modifiers;
        ModifierLine lmod;
        private Character entity;

        private float attackAnimationCountdown;
        private float shiftAnimationCountdown;
        DateTime frameTimePrev = DateTime.Now;

        private Effect pointSpritesEffect;
        VertexPositionColor[] spriteArray;
        VertexDeclaration vertexPosColDecl;
        Random rand;

        Matrix projectionMatrix;
        Matrix viewMatrix;
        float tick;
        public CharacterGraphics(Character entity)
            : base(entity) {
            this.entity = entity;
            frameChangeTimePrev = DateTime.Now;
            tick = 0;
        }

        public override void loadContent()
        {
                pointSpritesEffect = UserInterface2D.content.Load<Effect>("Effects/pointsprites");
                Texture2D fireTexture = UserInterface2D.content.Load<Texture2D>("fire");
                Texture2D smokeTexture = UserInterface2D.content.Load<Texture2D>("smoke");
                pointSpritesEffect.Parameters["SpriteTexture"].SetValue(fireTexture);

            Vector2 pos = new Vector2(entity.getLocation().Position.X,entity.getLocation().Position.Y);
            Vector2 delta = new Vector2(fireTexture.Height / 2, fireTexture.Width / 2);
            //pos = pos - delta;
            if (entity is Player) {
                if (attack_emitter == null) {
                    attack_emitter = new Emitter(fireTexture, pos, 1.0f, 100);
                    lmod = new ModifierLine(Vector2.One, new Point(100,100));
                    //lmod.Enabled = true;
                }
                if (shift_emitter == null) {
                    shift_emitter = new Emitter(smokeTexture, pos, 1.0f, 100);
                }
            }
            base.loadContent();
        }

        public void setTextures(string[] textures) {
            this.textures = textures;
            frameIndex = 0;
            setTexture(textures[0]);
        }

        private void UpdateAnimation() {
            if (textures != null &&
                DateTime.Now - frameChangeTimePrev > frameChangeTimeDelay) {
                if (++frameIndex >= textures.Length)
                    frameIndex = 0;
                setTexture(textures[frameIndex]);
                frameChangeTimePrev = DateTime.Now;
            }
        }

        private void UpdateEffect() {
            // TODO: Add your update logic here
            spriteArray[rand.Next(0, fireCount)].Position =
                new Vector3(rand.Next(100) / 10f,
                   rand.Next(100) / 10f, 0);
            spriteArray[rand.Next(0, fireCount)].Position =
                new Vector3(rand.Next(100) / 10f,
                  rand.Next(100) / 10f, 0);
        }
        public override void onDraw() {
            //UpdateEffect();
            if (gameObject is Player) {
                if (attacking && Engine.gameTime.TotalGameTime.TotalMilliseconds - attackAnimationCountdown < 100) {
                    attack_emitter.Update((float)Engine.gameTime.ElapsedGameTime.TotalSeconds);
                    attack_emitter.Draw(((UserInterface2D)Engine.userInterface).spriteBatch);
                } else {
                    attacking = false;
                }
                if (shifting && Engine.gameTime.TotalGameTime.TotalMilliseconds - shiftAnimationCountdown < 300) {
                    shift_emitter.Update((float)Engine.gameTime.ElapsedGameTime.TotalSeconds);
                    shift_emitter.Draw(((UserInterface2D)Engine.userInterface).spriteBatch);
                } else {
                    shifting = false;
                }
            }
            /*
            RenderState renderState = UserInterface2D.graphicsDevice.RenderState;
            renderState.PointSpriteEnable = true;
            renderState.PointSize = 64.0f;
            renderState.AlphaBlendEnable = true;

            //renderState.SourceBlend = Blend.Zero;
            //renderState.BlendFunction = BlendFunction.Subtract;
            //renderState.DestinationBlend = Blend.Zero;
            renderState.DepthBufferWriteEnable = false;

            UserInterface2D.graphicsDevice.VertexDeclaration
                = vertexPosColDecl;
            Matrix WVPMatrix = Matrix.Identity;// *viewMatrix * projectionMatrix;
            pointSpritesEffect.Parameters["WVPMatrix"].SetValue(WVPMatrix);
            Texture2D fire = UserInterface2D.content.Load<Texture2D>("fire");
            pointSpritesEffect.Begin();
            if (gameObject is Player)
                foreach (EffectPass pass in
                    pointSpritesEffect.CurrentTechnique.Passes) {
                    pass.Begin();
                    foreach (VertexPositionColor sprite in spriteArray) {
                        SpriteBatch sb = ((UserInterface2D)Engine.userInterface).spriteBatch;
                        Vector3 possy = gameObject.getLocation().Position + sprite.Position;
                        sb.Draw(fire, new Vector2(possy.X, possy.Y - 10), null, Color.WhiteSmoke, 0, Vector2.Zero, 0.12f, SpriteEffects.None, 0.15f);
                    }
                    pass.End();
                }
            pointSpritesEffect.End();

            renderState.PointSpriteEnable = false;
            renderState.DepthBufferWriteEnable = true;
            //renderState.SourceBlend = Blend.SourceAlpha;
            //renderState.DestinationBlend = Blend.InverseSourceAlpha;
            */

            if (true) // change to character running property
                UpdateAnimation();
            base.onDraw();
        }

        internal void startShiftAnimation() {
            shiftAnimationCountdown = (float)Engine.gameTime.TotalGameTime.TotalMilliseconds;
            shift_emitter.Position = new Vector2(entity.getLocation().Position.X, entity.getLocation().Position.Y);
            shift_emitter.Reset();
            shifting = true;
        }


        internal void startAttackAnimation(Direction Direction) {
            Point p = Map.getPointInDirection(new Point(), Direction);
            Vector2 dir = new Vector2(p.X, p.Y);
            lmod.Direction = dir;
            attack_emitter.Position = new Vector2(entity.getLocation().Position.X,entity.getLocation().Position.Y);
            attack_emitter.Reset();
            attackAnimationCountdown = (float)Engine.gameTime.TotalGameTime.TotalMilliseconds;
            attacking = true;
            if (entity is Player) {
                if (!attack_emitter.Modifiers.Contains(lmod)) {
                    attack_emitter.Modifiers.Add(lmod);
                }
            }
        }
    }
}
