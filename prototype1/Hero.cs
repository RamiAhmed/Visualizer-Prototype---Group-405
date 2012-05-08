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

        // Crashing space ship (START)
        public Texture2D shipCrashTex;
        private Sprite crashingShip;
        private float crashingShipAnimSpeed = 7.5f;
        private bool landHero = false;
        private bool heroStartPositionSet = false;
        private Vector2 heroStartIntroPosition = new Vector2(0, 0);

        // Position & scale
        private Vector2 heroStartPosition = new Vector2(350, 385);
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
        private float superJumpChance = 0.10f;
        private float failureChance = 0.05f;
        private float gravity = 0.35f;

        public enum HeroState { WALKING, SUPERJUMPING, JUMPING, SLIDING, KICKING };

        public Hero()
        {

        }

        public void createHero()
        {
            this.Move(heroStartIntroPosition);
            this.Width = 100;
            this.Height = 100;
            this.Texture = heroTex;
            this.Active = true;
            this.LayerDepth = 1f;
            this.CurrentState = HeroState.WALKING;
        }

        public void updateHero(GameTime gameTime)
        {
            updateJump();
            this.Color = ColorHandler.getCurrentColor();

            if (GameStateHandler.CurrentState == GameState.RUNNING)
            {
                if (this.CurrentState == HeroState.WALKING) 
                {
                    float tolerance = 50f;
                    if (Math.Abs(this.Position.X - heroStartPosition.X) < tolerance &&
                        Math.Abs(this.Position.Y - heroStartPosition.Y) < tolerance)
                    {

                    this.Move(this.Position.X + RandomHandler.GetRandomFloat(-1f, 1f), 
                              this.Position.Y + RandomHandler.GetRandomFloat(-1f, 1f));
                    }
                    else 
                    {
                        this.Move(heroStartPosition);
                    }
                }
            }
        }

        private void startIntroSequence()
        {
            crashingShip = new Sprite();

            crashingShip.Texture = shipCrashTex;

            crashingShip.Width = 500;
            crashingShip.Height = 250;

            crashingShip.Color = Color.White;
            crashingShip.Speed = 4f;
            crashingShip.LayerDepth = 0.25f;
            crashingShip.ScaleFactor = 0.5f;

            crashingShip.Move(-crashingShip.Width, 0);

            crashingShip.Active = true;
        }

        private void drawIntroSequence(SpriteBatch batch, GameTime time)
        {
            if (crashingShip == null)
            {
                startIntroSequence();
                return;
            }

            if (crashingShip.Active)
            {          
                crashingShip.Move(crashingShip.Position.X + crashingShip.Speed, crashingShip.Position.Y + RandomHandler.GetRandomFloat(-2.5f, 4f));

                int animationX = (int)(time.TotalGameTime.TotalSeconds * crashingShipAnimSpeed) % 4;
                Rectangle animCycle = new Rectangle(animationX * crashingShip.Width, 0,
                                                crashingShip.Width, crashingShip.Height);

                batch.Draw(crashingShip.Texture, crashingShip.Position, animCycle, crashingShip.Color, 0f, new Vector2(0, 0), crashingShip.ScaleFactor, SpriteEffects.None, crashingShip.LayerDepth);

                if (crashingShip.Position.X > Controller.TOTAL_WIDTH)
                {
                    crashingShip.Active = false;
                    GameStateHandler.CurrentState = GameState.RUNNING;
                    return;
                }
            }
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
                if (GameStateHandler.CurrentState == GameState.STARTING)
                {
                    drawIntroSequence(batch, gameTime);

                    if (crashingShip.Position.X > 0)
                    {
                        int yPos = this.Height * 7;
                        walkcycleSpeed = 400;

                        if (this.Position.Y > heroStartPosition.Y)
                        {
                            if (!heroStartPositionSet) 
                            {
                                heroStartPositionSet = true;
                                this.Move(heroStartPosition);
                                Console.WriteLine("Moving hero to (run) " + heroStartPosition.ToString());

                                currentFrame = 0;
                            }

                            walkcycleSpeed = 375;
                            yPos = this.Height * 8;

                            if (currentFrame > 8)
                            {
                                GameStateHandler.CurrentState = GameState.RUNNING;
                            }
                        }
                        else
                        {
                            this.Move(this.Position.X + 3f, this.Position.Y + 4f);
                        }

                        Rectangle animcycle = new Rectangle(getCurrentFrame(gameTime) * this.Width, yPos, this.Width, this.Height);

                        batch.Draw(this.Texture, this.Position, animcycle, this.Color, 0f, 
                                new Vector2(0, 0), heroScale, SpriteEffects.None, 0f);
                    }
                                       
                }
                else if (GameStateHandler.CurrentState == GameState.RUNNING)
                {
                    int yPos = 0;
                    switch (this.CurrentState)
                    {
                        case HeroState.WALKING:
                            yPos = 0;
                            walkcycleSpeed = 75;
                            break;

                        case HeroState.JUMPING:
                            yPos = this.Height + 1;
                            walkcycleSpeed = 100;
                            break;

                        case HeroState.SLIDING:
                            yPos = (this.Height + 1) * 2;
                            walkcycleSpeed = 125;
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

                    batch.Draw(this.Texture, this.Position, animcycle, this.Color, 0f, new Vector2(0, 0), heroScale, SpriteEffects.None, 0f);
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

        private void startKick()
        {
            if (this.Active)
            {
                if (this.CurrentState == HeroState.WALKING)
                {
                    this.CurrentState = HeroState.KICKING;
                    currentFrame = 0;
                }
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
                    this.JumpHeight = defaultJumpHeight + RandomHandler.GetRandomFloat(1f);
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
