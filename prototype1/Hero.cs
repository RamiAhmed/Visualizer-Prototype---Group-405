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
        // General stuff
        public Texture2D heroTex;
        static public bool heroReady = false;
        private float failureChance = 0.05f;

        // Intro & outro stuff
        public Vector2 heroStartIntroPosition = new Vector2(0, 0);
        private float heroLandedTime = 0f;
        public SpaceshipHandler shipHandler;

        // Position & scale
        public Vector2 heroStartPosition = new Vector2(350, 385);
        private float heroScale = 1.5f;      

        // Animation
        private long lastFrame = 0;
        private int currentFrame = 0;
        private int walkcycleSpeed = 75; // per nth millisecond (adjusted locally)
        private int amountOfFramesInWalkcycle = 8;

        private long lastItemPickUp = 0;
        private float pickUpPoseTime = 0.75f;

        private long lastFail = 0;
        private float failTime = 1f;

        // Jumping
        private float defaultJumpHeight = 10f;
        private float superJumpChance = 0.10f;
        private float gravity = 0.35f;
        private float introGravity = 0.0225f;
        private float introDefaultJumpHeight = 9f;

        // Getters & Setters
        private HeroState currentState;
        private float jumpHeight;

        public enum HeroState { WALKING, SUPERJUMPING, JUMPING, SLIDING, KICKING, PICKING_UP, FAILING };

        public Hero()
        {
            shipHandler = new SpaceshipHandler(this);
        }

        public void createHero()
        {            
            this.Width = 100;
            this.Height = 100;
            this.Texture = heroTex;
           
            this.LayerDepth = 0f;
            this.ScaleFactor = heroScale;
            this.Rotation = 0f;
            this.Color = Color.Gray;

            this.Move(heroStartIntroPosition);
            this.Active = true;
        }

        public void updateHero(GameTime gameTime)
        {
            if (shipHandler != null)
            {
                shipHandler.updateSpaceship(gameTime);
            }

            if (GameStateHandler.CurrentState == GameState.RUNNING)
            {
                updateJump();
                this.Color = ColorHandler.getCurrentColor();

                if (this.CurrentState == HeroState.WALKING) 
                {
                    float tolerance = 50f;
                    if (Math.Abs(this.Position.X - heroStartPosition.X) < tolerance &&
                        Math.Abs(this.Position.Y - heroStartPosition.Y) < (tolerance * 0.5f))
                    {

                    this.Move(this.Position.X + RandomHandler.GetRandomFloat(-1f, 1f), 
                              this.Position.Y + RandomHandler.GetRandomFloat(-1f, 1f));
                    }
                    else 
                    {
                        this.Move(heroStartPosition);
                    }
                }
                else if (this.CurrentState == HeroState.PICKING_UP)
                {
                    long currentMilliseconds = (long)gameTime.TotalGameTime.TotalMilliseconds;
                    if (currentMilliseconds - lastFrame <= pickUpPoseTime * 1000f)
                    {
                        this.Move(this.Position.X, this.Position.Y - 1f);
                    }
                    else 
                    {
                        this.Move(this.Position.X, this.Position.Y + 0.75f);
                        if (this.Position.Y >= heroStartPosition.Y)
                        {
                            this.Move(heroStartPosition);
                            this.CurrentState = HeroState.WALKING;
                        }                        
                    }
                }
                else if (this.CurrentState == HeroState.FAILING)
                {
                    long currentMilliseconds = (long)gameTime.TotalGameTime.TotalMilliseconds;
                    if (currentMilliseconds - lastFail <= failTime * 1000f)
                    {
                       // heroReady = false;
                    }
                    else 
                    {
                       // heroReady = true;
                        //this.CurrentState = HeroState.WALKING;                        
                    }
                }
            }
        }

        private void drawHeroIntro(SpriteBatch batch, GameTime time)
        {
            if (shipHandler.crashingShip.Position.X > 0)
            {
                int yPos = this.Height * 7,
                    numFrames = 4;
                walkcycleSpeed = 100;

                if (this.Position.Y >= heroStartPosition.Y)
                {
                    // Hero has landed on ground
                    if (heroLandedTime == 0f)
                    {
                        heroLandedTime = (float)time.TotalGameTime.TotalSeconds;
                        this.Move(this.Position.X, heroStartPosition.Y);
                        //Console.WriteLine("Moving hero to (run) " + heroStartPosition.ToString());

                        //currentFrame = 0;
                    }
                    else if ((float)time.TotalGameTime.TotalSeconds - heroLandedTime < 2f && heroLandedTime != -1f) // 2 = how long in seconds will the hero lie down
                    {
                        // Hero is lying on surface
                        //yPos = this.Height * 7;
                        currentFrame = 7;
                        //Console.WriteLine("Hero stays on frame " + currentFrame.ToString() + " and yPos: " + yPos.ToString());

                        this.JumpHeight = introDefaultJumpHeight;
                    }
                    else
                    {
                        // Hero is getting back up and jumping
                        if (heroLandedTime != -1)
                        {
                            heroLandedTime = -1;            
                        }
                    }

                    if (this.Position.X >= heroStartPosition.X)
                    {
                        // Hero has reached start position
                        heroReady = true;
                        GameStateHandler.CurrentState = GameState.RUNNING;
                        return;
                    }
                }
                else
                {
                    // Hero is falling
                    walkcycleSpeed = 300;
                    this.Move(this.Position.X, this.Position.Y + 8f);
                }

                Rectangle animcycle;
                if (heroLandedTime != 0f && heroLandedTime != -1f)
                {
                    //Console.WriteLine("Hero fixed frame (lying down)");
                    animcycle = new Rectangle(currentFrame * this.Width, this.Height * 7, this.Width, this.Height);
                }
                else
                {
                    if (heroLandedTime == -1)
                    {
                        numFrames = 8;
                        walkcycleSpeed = 175;
                        yPos = this.Height * 8;
                        this.Move(this.Position.X + 4.25f, this.Position.Y - this.JumpHeight);
                        this.JumpHeight -= introGravity;
                    }

                    //Console.WriteLine("Hero moving frame");
                    animcycle = new Rectangle((int)(getCurrentFrame(time, numFrames) * this.Width), yPos, this.Width, this.Height);
                }

                batch.Draw(this.Texture, this.Position, animcycle, this.Color, 0f,
                        new Vector2(0, 0), this.ScaleFactor, SpriteEffects.None, this.LayerDepth);
            }
        }

        public void drawHero(SpriteBatch batch, GameTime gameTime)
        {
            if (shipHandler != null)
            {
                shipHandler.drawSpaceship(batch, gameTime);
            }

            if (GameStateHandler.CurrentState == GameState.STARTING)
            {
                if (this.Active)
                {
                    //shipHandler.drawIntroSequence(batch, gameTime);
                    drawHeroIntro(batch, gameTime);
                }
            }
            else if (GameStateHandler.CurrentState == GameState.RUNNING)
            {
                if (this.Active)
                {
                    int yPos = 0,
                        xPos = 0;
                    switch (this.CurrentState)
                    {
                        case HeroState.WALKING:
                            yPos = 0;
                            walkcycleSpeed = 75;
                            break;

                        case HeroState.JUMPING:
                            yPos = this.Height;
                            walkcycleSpeed = 100;
                            break;

                        case HeroState.SLIDING:
                            yPos = this.Height * 2;
                            walkcycleSpeed = 125;
                            break;

                        case HeroState.KICKING:
                            yPos = this.Height  * 4;
                            walkcycleSpeed = 85;
                            break;

                        case HeroState.PICKING_UP:
                            yPos = this.Height * 5;
                            xPos = 1;
                            break;

                        case HeroState.FAILING:
                            yPos = this.Height * 5;
                            xPos = this.Width;
                            break;

                        case HeroState.SUPERJUMPING:
                            yPos = this.Height * 6;
                            walkcycleSpeed = 175;
                            break;                        
                    }

                    Rectangle animcycle;
                    if (xPos == 0)
                    {
                        animcycle = new Rectangle(getCurrentFrame(gameTime) * this.Width, yPos, this.Width, this.Height);
                    }
                    else
                    {
                        animcycle = new Rectangle(xPos, yPos, this.Width, this.Height);
                    }


                    batch.Draw(this.Texture, this.Position, animcycle, this.Color, this.Rotation, 
                        new Vector2(0, 0), this.ScaleFactor, SpriteEffects.None, this.LayerDepth);
                }
            }
            else if (GameStateHandler.CurrentState == GameState.ENDING)
            {
                //shipHandler.drawOutroSequence(batch, gameTime);

                if (this.Active)
                {
                    walkcycleSpeed = 75;

                    Rectangle animCycle = new Rectangle(getCurrentFrame(gameTime) * this.Width, 0, this.Width, this.Height);

                    batch.Draw(this.Texture, this.Position, animCycle, this.Color, 0f, new Vector2(0, 0), this.ScaleFactor, SpriteEffects.None, this.LayerDepth);
                }
            }
        }

        public void startPickUpPose(GameTime time)
        {
            this.CurrentState = HeroState.PICKING_UP;
            lastItemPickUp = (long)time.TotalGameTime.TotalMilliseconds;
        }

        public void startAction(HeroState action, GameTime time)
        {
           /* if (RandomHandler.GetRandomFloat(1f) < failureChance)
            {
                if (this.CurrentState == HeroState.WALKING)
                {
                    this.CurrentState = HeroState.FAILING;
                    lastFail = (long)time.TotalGameTime.TotalMilliseconds;
                }
            }
            else
            {*/
                switch (action)
                {
                    case HeroState.SLIDING: startSlide(); break;
                    case HeroState.KICKING: startKick(); break;
                    case HeroState.JUMPING: startJump(); break;
                }
           // }
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

        private int getCurrentFrame(GameTime gameTime)
        {
            return getCurrentFrame(gameTime, amountOfFramesInWalkcycle);
        }

        private int getCurrentFrame(GameTime gameTime, int numFrames)
        {
            long currentMilliseconds = (long)gameTime.TotalGameTime.TotalMilliseconds;
            if (currentMilliseconds - lastFrame > walkcycleSpeed)
            {
                lastFrame = currentMilliseconds;

                if (currentFrame < numFrames - 1)
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
