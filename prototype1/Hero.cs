using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace prototype1
{
    class Hero : Sprite
    {
        private Vector2 heroStartPosition = new Vector2(300, 385);
        private Texture2D heroTexture;
        private HeroState currentState;
        private float jumpHeight;
        private float heroScale = 1.5f;
        private long lastFrame = 0;
        private int currentFrame = 0;
        private int walkcycleSpeed = 75; // per nth millisecond
        private int amountOfFramesInWalkcycle = 8;
        private float defaultJumpHeight = 15f;

        private float gravity = 0.5f;

        public enum HeroState { WALKING, JUMPING, SLIDING };

        public Hero(Texture2D texture)
        {
            if (heroTexture == null)
            {
                heroTexture = texture;
            }

            init();
        }

        private void init()
        {
            this.Move(heroStartPosition);
            this.Width = 100;
            this.Height = 100;
            this.Texture = heroTexture;
            this.Active = true;
            this.CurrentState = HeroState.WALKING;
        }

        public void updateHero(GameTime gameTime)
        {
            updateJump();
            this.Color = new Color(RandomHandler.GetRandomFloat(1), RandomHandler.GetRandomFloat(1), RandomHandler.GetRandomFloat(1));
        }

        private int getCurrentFrame(GameTime gameTime)
        {
            long currentMilliseconds = (long)gameTime.TotalGameTime.TotalMilliseconds;
            if (currentMilliseconds - lastFrame > walkcycleSpeed)
            {
                lastFrame = currentMilliseconds;

                if (currentFrame < amountOfFramesInWalkcycle - 1)
                {
                    currentFrame++;
                }
                else
                {
                    currentFrame = 0;
                }
            }
            return currentFrame;
        }

        public void drawHero(SpriteBatch batch, GameTime gameTime)
        {
           // if (this.CurrentState == HeroState.WALKING)
           // {
                if (this.Active)
                {
                    Rectangle walkcycle = new Rectangle(getCurrentFrame(gameTime) * 100, 0, this.Width, this.Height);

                    Rectangle heroRect = new Rectangle((int)heroStartPosition.X, (int)heroStartPosition.Y,
                                                        this.Width, this.Height);
                    //batch.Draw(this.Texture, heroRect, walkcycle, this.Color);
                    batch.Draw(this.Texture, this.Position, walkcycle, this.Color, 0f, new Vector2(0, 0), heroScale, SpriteEffects.None, 0f);
                }
           // }
        }

        private void startJump()
        {
            if (this.Active)
            {
                if (this.CurrentState == HeroState.WALKING)
                {
                    this.CurrentState = HeroState.JUMPING;
                    this.JumpHeight = defaultJumpHeight;           
                }
            }
        }

        private void updateJump()
        {
            if (this.CurrentState == HeroState.JUMPING)
            {
                this.Move(this.Position.X, this.Position.Y - this.JumpHeight);
                this.JumpHeight -= gravity;

                if (this.Position.Y >= heroStartPosition.Y)
                {
                    this.CurrentState = HeroState.WALKING;
                    this.JumpHeight = 0;
                    this.Move(heroStartPosition);
                }
            }
            else
            {
                if (RandomHandler.GetRandomFloat(1) < 0.0025f)
                {
                   startJump();
                }
            }
        }

        public HeroState CurrentState
        {
            get { return currentState; }
            set { currentState = value; }
        }

        private float JumpHeight
        {
            get { return jumpHeight; }
            set { jumpHeight = value; }
        }
    }
}
