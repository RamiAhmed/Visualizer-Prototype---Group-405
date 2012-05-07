﻿using System;
using System.Net;
using System.IO;
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
    class Controller : DrawableGameComponent
    {
        /* Public variables */
        public static int TOTAL_WIDTH = 1024,
                          TOTAL_HEIGHT = 576;

        /* Private variables */
        private List<Sprite> allSprites = new List<Sprite>();
        private OSCHandler osc;

        private Hero hero;
        private BackgroundHandler bg;
        private ForegroundHandler fg;
        private ObstacleHandler obstacleHandler;
        private Enemy enemy;
        private CameraHandler cameraHandler;

        public Controller(Game game) : base(game)
        {
        
        }

        private Texture2D loadTexture(string assetName)
        {
            Texture2D texture = Game.Content.Load<Texture2D>(assetName);
            if (texture == null)
            {
                Console.WriteLine("Error loading texture by asset name: " + assetName);
            }

            return texture;
        }

        protected override void LoadContent()
        {
            hero = new Hero();
            hero.heroTex = loadTexture("Rami_Anim_GS");

            lateInit();

            loadObstacleTextures();
            loadForegroundTextures();
            loadBackgroundTextures();
            loadEnemyTextures();

            base.LoadContent();
        }

        private void lateInit()
        {
            hero.createHero();
            bg = new BackgroundHandler();
            fg = new ForegroundHandler();
            obstacleHandler = new ObstacleHandler();
            enemy = new Enemy();
            osc = new OSCHandler();

            RandomHandler.init();
        }

        private void loadEnemyTextures()
        {
            enemy.enemyTextures.Add(loadTexture("Sheep_Anim_GS"));
        }

        private void loadObstacleTextures()
        {
            // Holes
            obstacleHandler.holeTextures.Add(loadTexture("OBSTACLE_Pit1_Scaled_GS"));

            // Hills
            obstacleHandler.hillTextures.Add(loadTexture("OBSTACLE_Tesla1_Scaled_GS"));

            // Slides
            obstacleHandler.slideTextures.Add(loadTexture("OBSTACLE_Fan1_Scaled_GS"));

            // Walls
            obstacleHandler.wallTextures.Add(loadTexture("OBSTACLE_Wall1_Scaled_GS_Animation"));
        }

        private void loadForegroundTextures()
        {
            fg.maskTex = loadTexture("sin_to_stream_con1");
            fg.connectorTex = loadTexture("connection");
            fg.surfaceTex = loadTexture("ground");

            fg.middleTextures.Add(loadTexture("stream_to_ground1"));
            fg.middleTextures.Add(loadTexture("stream_to_ground2"));
            fg.middleTextures.Add(loadTexture("stream_to_ground3"));

            fg.sinusoidTextures.Add(loadTexture("ground_stream1"));
            fg.sinusoidTextures.Add(loadTexture("ground_stream2"));
            fg.sinusoidTextures.Add(loadTexture("ground_stream3"));
        }

        private void loadBackgroundTextures()
        {
            bg.fogTextures.Add(loadTexture("fog2"));

            bg.maskTex = loadTexture("BG_Mask1");

            bg.backgroundTextures.Add(loadTexture("Capacitor1_Scaled_GS"));
            bg.backgroundTextures.Add(loadTexture("Capacitor2_Scaled_GS"));
            bg.backgroundTextures.Add(loadTexture("Port-Audio1_Scaled_GS"));
            bg.backgroundTextures.Add(loadTexture("Port-Firewire1_Scaled_GS"));
            bg.backgroundTextures.Add(loadTexture("Port-InternalSpeaker1_Scaled_GS"));
            bg.backgroundTextures.Add(loadTexture("Port-PS1_Scaled_GS"));
            bg.backgroundTextures.Add(loadTexture("Port-Serial1_Scaled_GS"));
            bg.backgroundTextures.Add(loadTexture("RamBar1_Scaled_GS"));
            bg.backgroundTextures.Add(loadTexture("Resistor1_Scaled_GS"));
            bg.backgroundTextures.Add(loadTexture("LED1_Scaled_GS"));
            bg.backgroundTextures.Add(loadTexture("Port-Optical1_Scaled_GS"));
        }

        public override void Update(GameTime gameTime)
        {
            if (cameraHandler != null)
            {
                cameraHandler.updateCamera(gameTime, hero.Position);
            }

            obstacleHandler.updateObstacles(gameTime);
            bg.updateBackground(gameTime);
            fg.updateForeground(gameTime);
            hero.updateHero(gameTime);
            enemy.updateEnemy(gameTime);

            updateAllSprites();

            if (RandomHandler.GetRandomInt(100) < 25)
            {
                createObstacle(gameTime);
            }

            checkCollision();

            base.Update(gameTime);
        }

        private void createObstacle(GameTime gameTime)
        {
            Obstacle obs = obstacleHandler.generateObstacles(gameTime);
            if (obs != null)
            {
                Hero.HeroState state = Hero.HeroState.WALKING;

                switch (obs.Type)
                {
                    case ObstacleType.HILL:
                    case ObstacleType.HOLE: state = Hero.HeroState.JUMPING; break;
                    case ObstacleType.SLIDE: state = Hero.HeroState.SLIDING; break;
                    case ObstacleType.WALL: state = Hero.HeroState.KICKING; break;
                }

                hero.startAction(state);

                if (obs.AnimateOnDeath)
                {
                    obs.ReadyToAnimate = true;
                }
            }
        }

        private void updateAllSprites()
        {
            allSprites.Clear();

            allSprites.AddRange(obstacleHandler.obstacleSprites);
            allSprites.AddRange(fg.sinusoidSprites);
            allSprites.AddRange(bg.maskSprites);
            allSprites.AddRange(bg.bgSprites);
            //allSprites.Add(hero);

            foreach (Sprite sprite in allSprites)
            {
                sprite.CheckIsActive();
            }
        }

        private void checkCollision()
        {
            foreach (Enemy enemySprite in enemy.enemySprites) 
            {
                foreach (Obstacle obstacle in obstacleHandler.obstacleSprites)
                {
                    if (getIsWithinRange(obstacle.Position.X, enemySprite.Position.X, 50f) &&
                        obstacle.Type != ObstacleType.SLIDE)
                    {
                        enemy.removeEnemy(enemySprite);
                    }
                    else if (getIsWithinRange(hero.Position.X, enemySprite.Position.X, 50f) &&
                        enemySprite.Speed > 0f)
                    {
                        enemySprite.Speed -= 0.05f;
                    }
                }
            }
        }

        private bool getIsWithinRange(float a, float b, float range)
        {
            bool inRange = false;
            if (Math.Abs(a - b) < range)
            {
                inRange = true;
            }

            return inRange;
        }
        
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch batch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            GraphicsDeviceManager graphicsManager = (GraphicsDeviceManager)Game.Services.GetService(typeof(GraphicsDeviceManager));

            if (cameraHandler == null)
            {
                cameraHandler = new CameraHandler();
                
            }
            batch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, cameraHandler.getTransformation(graphicsManager.GraphicsDevice));

            //batch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
            bg.drawBackground(batch, gameTime);
            //batch.End();

           // batch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            fg.drawForeground(batch, gameTime);
           // batch.End();

           // batch.Begin();
            obstacleHandler.drawObstacles(batch, gameTime);
           // batch.End();

          //  batch.Begin();
            hero.drawHero(batch, gameTime);
          //  batch.End();

          //  batch.Begin();
            enemy.drawEnemy(batch, gameTime);
            batch.End();
 
            base.Draw(gameTime);
        }

        protected override void UnloadContent()
        {
            hero.heroTex.Dispose();

            foreach (Texture2D fgTexture in fg.sinusoidTextures)
            {
                fgTexture.Dispose();
            }
            fg.maskTex.Dispose();
            fg.connectorTex.Dispose();
            fg.surfaceTex.Dispose();

            fg.sinusoidTextures.Clear();
            fg.sinusoidSprites.Clear();

            foreach (Texture2D bgTexture in bg.backgroundTextures)
            {
                bgTexture.Dispose();
            }
            foreach (Texture2D fogTexture in bg.fogTextures)
            {
                fogTexture.Dispose();
            }
            bg.backgroundTextures.Clear();
            bg.bgSprites.Clear();

            osc.stopOSCServer();

            base.UnloadContent();
        }


    }
}