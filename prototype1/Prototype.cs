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
    class Prototype : DrawableGameComponent
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

        public Prototype(Game game) : base(game)
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
            obstacleHandler.wallTextures.Add(loadTexture("OBSTACLE_Wall1_Scaled_GS"));
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


            base.Update(gameTime);
        }

        private void createObstacle(GameTime gameTime)
        {
            ObstacleType type = obstacleHandler.generateObstacles(gameTime);
            Hero.HeroState state = Hero.HeroState.WALKING;

            switch (type) {
                case ObstacleType.HILL: 
                case ObstacleType.HOLE: state = Hero.HeroState.JUMPING; break;
                case ObstacleType.SLIDE: state = Hero.HeroState.SLIDING; break;
                case ObstacleType.WALL: state = Hero.HeroState.KICKING; break;
            }

            hero.startAction(state);
        }

        private void updateAllSprites()
        {
            allSprites.Clear();

            allSprites.AddRange(obstacleHandler.obstacleSprites);
            allSprites.AddRange(fg.sinusoidSprites);
            allSprites.AddRange(bg.maskSprites);
            allSprites.AddRange(bg.drawableBGSprites);
            allSprites.Add(hero);

            foreach (Sprite sprite in allSprites)
            {
                sprite.CheckIsActive();
            }
        }
        

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch batch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));

            batch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
            bg.drawBackground(batch, gameTime);
            batch.End();

            batch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            fg.drawForeground(batch, gameTime);
            batch.End();

            batch.Begin();
            obstacleHandler.drawObstacles(batch);
            batch.End();

            batch.Begin();
            hero.drawHero(batch, gameTime);
            batch.End();

            batch.Begin();
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
            bg.drawableBGSprites.Clear();

            osc.stopOSCServer();

            base.UnloadContent();
        }


    }
}