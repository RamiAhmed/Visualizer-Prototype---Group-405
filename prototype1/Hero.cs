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
        public Texture2D heroTex;

        // Position & scale
        private Vector2 heroStartPosition = new Vector2(300, 385);
        private float heroScale = 1.5f;      
        
        // Getters & Setters
        private HeroState currentState;
        private float jumpHeight;

        // Animation
        private long lastFrame = 0;
        private int currentFrame = 0;
        private int walkcycleSpeed = 75; // per nth millisecond
        private int amountOfFramesInWalkcycle = 8;

        // Jumping
        private float defaultJumpHeight = 10f;
        private float superJumpChance = 0.50f;
        private float failureChance = 0.05f;
        private float gravity = 0.35f;

        public enum HeroState { WALKING, SUPERJUMPING, JUMPING, SLIDING, KICKING };

        public Hero()
        {
            
        }

        public void createHero()
        {
            this.Move(heroStartPosition);
            this.Width = 100;
            this.Height = 100;
            this.Texture = heroTex;
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
                    if (this.CurrentState == HeroState.SLIDING || this.CurrentState == HeroState.KICKING)
                    {
                        this.CurrentState = HeroState.WALKING;
                    }
                }
            }
            return currentFrame;
        }

        public void drawHero(SpriteBatch batch, GameTime gameTime)
        {
            if (this.Active)
            {
                int yPos = 0;
                switch (this.CurrentState)
                {
                    case HeroState.JUMPING:
                        yPos = this.Height + 1;
                        walkcycleSpeed = 100;
                        break;

                    case HeroState.SLIDING:
                        yPos = (this.Height + 1) * 2;
                        walkcycleSpeed = 125;
                        break;

                    case HeroState.WALKING:
                        yPos = 0;
                        walkcycleSpeed = 75;
                        break;

                    case HeroState.KICKING: 
                        yPos = (this.Height + 1) * 4;
                        walkcycleSpeed = 85;
                        break;

                    case HeroState.SUPERJUMPING:
                        yPos = (this.Height + 1) * 6;
                        walkcycleSpeed = 175;
                        break;
                }
              

                Rectangle animcycle = new Rectangle(getCurrentFrame(gameTime) * this.Width, yPos, this.Width, this.Height);

                Rectangle heroRect = new Rectangle((int)heroStartPosition.X, (int)heroStartPosition.Y,
                                                    this.Width, this.Height);

                batch.Draw(this.Texture, this.Position, animcycle, this.Color, 0f, new Vector2(0, 0), heroScale, SpriteEffects.None, 0f);
            }
        }

        private void startKick()
        {
            if (this.Active) 
            {
                if (this.CurrentState == HeroState.WALKING) {
                    this.CurrentState = HeroState.KICKING;
                    currentFrame = 0;
                }
            }
        }

        public void startAction(HeroState action)
        {
            switch (action)
            {
                case HeroState.SLIDING: startSlide(); break;
                case HeroState.KICKING: startKick(); break;
                case HeroState.JUMPING: startJump(); break;
            }
        }

        private void startSlide()
        {
            if (this.Active)
            {
                if (this.CurrentState == HeroState.WALKING)
                {
                    this.CurrentState = HeroState.SLIDING;
                    currentFrame = 0;
                }
            }
        }

        private void startJump()
        {
            if (this.Active)
            {
                if (this.CurrentState == HeroState.WALKING)
                {
                    this.CurrentState = HeroState.JUMPING;
                    this.JumpHeight = defaultJumpHeight + RandomHandler.GetRandomFloat(2.5f);
                    if (RandomHandler.GetRandomFloat(1) < superJumpChance)
                    {
                        this.JumpHeight *= RandomHandler.GetRandomFloat(1.25f, 1.5f);
                        Console.WriteLine("SUPER JUMP!");
                        this.CurrentState = HeroState.SUPERJUMPING;
                    }
                }
            }
        }

        private void updateJump()
        {
            if (this.CurrentState == HeroState.JUMPING || this.CurrentState == HeroState.SUPERJUMPING)
            {
                if (this.JumpHeight == defaultJumpHeight)
                {
                    this.Move(this.Position.X, this.Position.Y + 15);
                    this.JumpHeight -= gravity;
                }
                else
                {
                    this.Move(this.Position.X, this.Position.Y - this.JumpHeight);
                    this.JumpHeight -= gravity;

                    if (this.Position.Y >= heroStartPosition.Y && this.JumpHeight < 0)
                    {
                        this.CurrentState = HeroState.WALKING;
                        this.JumpHeight = 0;
                        this.Move(heroStartPosition);
                    }
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
