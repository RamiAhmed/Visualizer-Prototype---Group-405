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
    class SpaceshipHandler
    {
        public static bool gameOver = false;

        // Crashing space ship (START)
        public Texture2D shipCrashTex;
        public Sprite crashingShip;
        private float crashingShipAnimSpeed = 7.5f;

        // Taking off (ENDING)
        public Texture2D shipRepairTex,
                         shipTakeOffTex;
        private Vector2 crashingShipFoundSpot = new Vector2(Controller.TOTAL_WIDTH, 200);
        private bool shipRediscovered = false;
        private int shipReached = 0;
        private bool shipRepaired = false;
        private bool shipReadyForTakeOff = false;

        // Misc
        private long lastFrame = 0;
        private int walkcycleSpeed = 0;
        private long currentFrame = 0;
        private int numFrames = 0;

        // Hero variables
        private Hero heroRef;

        public SpaceshipHandler(Hero heroReference)
        {
            heroRef = heroReference;
        }

        public void drawSpaceship(SpriteBatch batch, GameTime time)
        {
            if (GameStateHandler.CurrentState == GameState.STARTING || GameStateHandler.CurrentState == GameState.RUNNING)
            {
                drawIntroSequence(batch, time);
            }
            else if (GameStateHandler.CurrentState == GameState.ENDING)
            {
                drawOutroSequence(batch, time);
            }
        }

        public void updateSpaceship(GameTime time)
        {
            if (crashingShip != null && crashingShip.Active)
            {
                updateFrame(time);
            }

            //if (GameStateHandler.CurrentState == GameState.STARTING)
            //{
                if (crashingShip == null)
                {
                    startIntroSequence();
                    return;
                }
            //}
            /*else */if (GameStateHandler.CurrentState == GameState.ENDING)
            {
                updateOutroSequence(time);
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
            crashingShip.LayerDepth = 0.75f;
            crashingShip.ScaleFactor = 0.5f;

            crashingShip.Move(-crashingShip.Width, 0);

            crashingShip.Active = true;            
        }

        private void drawIntroSequence(SpriteBatch batch, GameTime time)
        {
            if (crashingShip != null && crashingShip.Active)
            {
                walkcycleSpeed = 100;
                numFrames = 4;

                crashingShip.Move(crashingShip.Position.X + crashingShip.Speed, crashingShip.Position.Y + RandomHandler.GetRandomFloat(-2.5f, 4f));

                int animationX = (int)(currentFrame * crashingShipAnimSpeed) % numFrames;
                Rectangle animCycle = new Rectangle(animationX * crashingShip.Width, 0,
                                                crashingShip.Width, crashingShip.Height);

                batch.Draw(crashingShip.Texture, crashingShip.Position, animCycle, crashingShip.Color, 0f, new Vector2(0, 0), crashingShip.ScaleFactor, SpriteEffects.None, crashingShip.LayerDepth);

                if (crashingShip.Position.X > Controller.TOTAL_WIDTH)
                {
                    Hero.heroReady = true;
                    crashingShip.Active = false;
                    GameStateHandler.CurrentState = GameState.RUNNING;
                    return;
                }
            }
        }

        private void startOutroSequence()
        {
            heroRef.Color = Color.White;
            heroRef.Move(heroRef.heroStartPosition);

            crashingShip.ScaleFactor = 1.5f;
            crashingShip.LayerDepth = 0.00001f;

            crashingShip.Speed *= 0.5f;

            crashingShip.Move(crashingShipFoundSpot);

            crashingShip.Active = true;
        }

        private void drawOutroSequence(SpriteBatch batch, GameTime time)
        {
            if (crashingShip.Active)
            {
                numFrames = 4;
                int animationX = 0;

                if (shipReached != 0 && !shipRepaired)
                {
                    // Ship has reached hero, but is not repaired
                    walkcycleSpeed = 400; // REPAIR CYCLE SPEED
                }

                if (shipRepaired || shipReadyForTakeOff)
                {
                    // Hero has repaired the ship or is ready for take off
                    walkcycleSpeed = 350;
                    numFrames = 7;

                    //Console.WriteLine("currentFrame: " + currentFrame.ToString() + " >= " + numFrames.ToString());
                    if (currentFrame >= numFrames-1 && !shipReadyForTakeOff)
                    {                        
                        // TAKE OFF LOOPED THROUGH ALL FRAMES
                        currentFrame = numFrames;

                        shipReadyForTakeOff = true;
                        Console.WriteLine("Ship set ready for take off");
                    }
                }

                if (!shipReadyForTakeOff)
                {
                    animationX = (int)(currentFrame * crashingShipAnimSpeed) % numFrames;
                }
                else
                {
                    animationX = 7;
                }

                Rectangle animCycle = new Rectangle(animationX * crashingShip.Width, 0, crashingShip.Width, crashingShip.Height);

                //Console.WriteLine("draw ship");

                batch.Draw(crashingShip.Texture, crashingShip.Position, animCycle, crashingShip.Color, crashingShip.Rotation,
                    new Vector2(0, 0), crashingShip.ScaleFactor, SpriteEffects.None, crashingShip.LayerDepth);
            }
        }

        private void updateOutroSequence(GameTime time)
        {
            if (!shipRediscovered)
            {
                // Ship has not yet appeared 
                shipRediscovered = true;

                startOutroSequence();

                Console.WriteLine("Ship rediscovered, place and activate it");
            }
            else
            {
                // Ship has appeared
                if (shipReached == 0)
                {
                    // Ship has not reached hero yet; make it move left
                    crashingShip.Move(crashingShip.Position.X - crashingShip.Speed, crashingShip.Position.Y);

                    if (crashingShip.Position.X + (crashingShip.Width * 0.5f) <= heroRef.Position.X)
                    {
                        // If ship reaches hero
                        if (heroRef.Active)
                        {
                            // Inactivate hero and set ship to repair
                            Hero.heroReady = false;
                            shipReached = (int)time.TotalGameTime.TotalSeconds;
                            crashingShip.Texture = shipRepairTex;
                            currentFrame = 0;

                            heroRef.Active = false;

                            Console.WriteLine("Ship reached, change to repair");
                        }
                    }
                }
                else
                {
                    // Ship has reached hero
                    int currentSeconds = (int)time.TotalGameTime.TotalSeconds;
                    if (currentSeconds - shipReached > 4 && !shipRepaired) // 4 = how many seconds repairing takes
                    {
                        // hero has repaired long enough, ship is now repaired
                        shipRepaired = true;
                        currentFrame = 0;
                        crashingShip.Texture = shipTakeOffTex;

                        Console.WriteLine("Done repairing ship, ready to take off");
                    }
                    else if (shipRepaired)
                    {
                        // ship has been repaired
                        if (shipReadyForTakeOff)
                        {
                            // ship is ready for take off
                            crashingShip.Speed += 0.1f;
                            crashingShip.Move(crashingShip.Position.X + crashingShip.Speed + 1f, crashingShip.Position.Y - crashingShip.Speed);

                            if (crashingShip.Position.X + crashingShip.Width > Controller.TOTAL_WIDTH)
                            {
                                gameOver = true;
                            }
                        }
                    }
                }
            }
        }

        private void updateFrame(GameTime time)
        {
            if (getTimeSinceLastFrame(time) > walkcycleSpeed)
            {
                saveCurrentTime(time);

                if (currentFrame <= numFrames)
                {
                    currentFrame++;
                }
                else
                {
                    currentFrame = 0;
                }                
            }
        }

        private long getTimeSinceLastFrame(GameTime time)
        {
            return (long)time.TotalGameTime.TotalMilliseconds - lastFrame;
        }

        private void saveCurrentTime(GameTime time)
        {
            lastFrame = (long)time.TotalGameTime.TotalMilliseconds;
        }
    }
}
