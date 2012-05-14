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
    class BossHandler
    {
        public Texture2D bossTexture;

        private float bossAnimSpeed = 5f;
        private int numFrames = 4;
        private Sprite boss;

        private float assembleAtX = 750;

        private Hero heroRef;

        private BossState _currentState;
        private enum BossState { IDLE, SWARM, ASSEMBLING, ASSEMBLED, DEAD };

        private bool debug = true;

        public BossHandler(Hero heroReference)
        {
            heroRef = heroReference;
            this.CurrentState = BossState.IDLE;

            if (debug)
            {
                Console.WriteLine("BossHandler instantiated");
            }
        }

        public void updateBoss(GameTime time)
        {
            if (boss != null && boss.Active)
            {
                if (debug)
                {
                    Console.WriteLine("Update Boss");
                }
                switch (this.CurrentState)
                {
                    case BossState.ASSEMBLED:
                        if (boss.Position.Y < heroRef.heroStartPosition.Y)
                        {
                            boss.Move(boss.Position.X, boss.Position.Y + boss.Speed);
                        }
                        else
                        {
                            boss.Move(boss.Position.X + RandomHandler.GetRandomFloat(-1f, 1f), boss.Position.Y + RandomHandler.GetRandomFloat(-1f, 1f));

                            heroRef.CurrentState = Hero.HeroState.FIGHTING;

                            Console.WriteLine("Boss starts the hero fighting");
                        }
                        break;
                    case BossState.ASSEMBLING:
                        boss.Move(boss.Position.X + RandomHandler.GetRandomFloat(-1f, 1f), boss.Position.Y + RandomHandler.GetRandomFloat(-1f, 1f));
                        break;
                    case BossState.SWARM:
                        if (boss.Position.X >= assembleAtX)
                        {
                            this.CurrentState = BossState.ASSEMBLING;
                            if (debug)
                            {
                                Console.WriteLine("Boss is now assembling");
                            }
                        }
                        else
                        {
                            if (debug)
                            {
                                Console.WriteLine("Move boss to the right");
                            }
                            boss.Move(boss.Position.X + boss.Speed, boss.Position.Y);
                        }
                        break;
                }
            }
        }

        public void drawBoss(SpriteBatch batch, GameTime time)
        {
            if (boss != null && boss.Active)
            {
                int yPos = 0,
                    xPos = -1;
                switch (this.CurrentState)
                {
                    case BossState.ASSEMBLED: yPos = boss.Height; break;
                    case BossState.ASSEMBLING: yPos = 0; break;
                    case BossState.SWARM: yPos = 0; 
                                          xPos = 0; 
                                          break;                     
                }

                int animationX = 0;
                if (xPos == -1)
                {
                    animationX = (int)(time.TotalGameTime.TotalSeconds * bossAnimSpeed) % numFrames;

                    if (this.CurrentState == BossState.ASSEMBLING)
                    {
                        if (debug)
                        {
                            Console.WriteLine("X: " + animationX.ToString());
                        }
                        if (animationX > numFrames-1)
                        {
                            this.CurrentState = BossState.ASSEMBLED;

                            if (debug)
                            {
                                Console.WriteLine("Boss is now assembled");
                            }
                        }
                    }
                }
                else
                {
                    animationX = xPos;
                }

                Rectangle animCycle = new Rectangle(animationX * boss.Width, yPos, boss.Width, boss.Height);

                batch.Draw(boss.Texture, boss.Position, animCycle, boss.Color, boss.Rotation, 
                        new Vector2(0, (boss.Height * 0.5f)), boss.ScaleFactor, SpriteEffects.None, boss.LayerDepth);
            }
        }

        public void createBoss()
        {
            if (boss == null)
            {
                boss = new Sprite();

                boss.Texture = bossTexture;
                boss.Width = boss.Texture.Width / numFrames;
                boss.Height = boss.Texture.Height / 2;

                boss.Speed = 2.5f;
                boss.ScaleFactor = 1f;
                boss.LayerDepth = 0f;
                boss.Rotation = 0f;
                boss.Color = ColorHandler.getCurrentColor();

                boss.Active = true;

                this.CurrentState = BossState.IDLE;//BossState.SWARM;

                boss.Move(-boss.Width * 0.5f, (boss.Height * 0.5f));
            }
        }

        private BossState CurrentState
        {
            get { return _currentState; }
            set { _currentState = value; }
        }
    }
}
