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
        private Texture2D heroTexture, surfaceTexture;

        private Hero hero;
        private BackgroundHandler bg;
        private ForegroundHandler fg;

        public Prototype(Game game) : base(game)
        {
        
        }

        private void lateInit()
        {
            hero = new Hero(heroTexture);
            bg = new BackgroundHandler();
            fg = new ForegroundHandler();
            osc = new OSCHandler();
            RandomHandler.init();
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
            heroTexture = loadTexture("Rami_Walk_GS");
            surfaceTexture = loadTexture("ground");

            lateInit();

            loadForegroundTextures();
            loadBackgroundTextures();

            base.LoadContent();
        }

        private void loadForegroundTextures()
        {
            fg.surfaceTex = surfaceTexture;

            fg.middleTextures.Add(loadTexture("stream_to_ground1"));
            fg.middleTextures.Add(loadTexture("stream_to_ground2"));
            fg.middleTextures.Add(loadTexture("stream_to_ground3"));

            fg.sinusoidTextures.Add(loadTexture("ground_stream1"));
            fg.sinusoidTextures.Add(loadTexture("ground_stream2"));
            fg.sinusoidTextures.Add(loadTexture("ground_stream3"));
        }

        private void loadBackgroundTextures()
        {
            bg.backgroundTextures.Add(loadTexture("Capacitor1_Scaled_GS"));
            bg.backgroundTextures.Add(loadTexture("Capacitor2_Scaled_GS"));
            bg.backgroundTextures.Add(loadTexture("Port-Audio1"));
            bg.backgroundTextures.Add(loadTexture("Port-Firewire1_Scaled_GS"));
            bg.backgroundTextures.Add(loadTexture("Port-InternalSpeaker1_Scaled_GS"));
            bg.backgroundTextures.Add(loadTexture("Port-PS1_Scaled_GS"));
            bg.backgroundTextures.Add(loadTexture("Port-Serial1_Scaled_GS"));
            bg.backgroundTextures.Add(loadTexture("RamBar1_Scaled_GS"));
            bg.backgroundTextures.Add(loadTexture("Resistor1_Scaled_GS"));
            bg.backgroundTextures.Add(loadTexture("LED1_Scaled_GS"));
        }

        public override void Update(GameTime gameTime)
        {
            bg.updateBackground(gameTime);
            fg.updateForeground(gameTime);
            hero.updateHero(gameTime);

            updateAllSprites();

            base.Update(gameTime);
        }

        private void updateAllSprites()
        {
            allSprites.Clear();

            allSprites.AddRange(fg.sinusoidSprites);
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
            bg.drawBackground(batch);
            batch.End();

            batch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            fg.drawForeground(batch);
            batch.End();

            batch.Begin();
            hero.drawHero(batch, gameTime);
            batch.End();

            base.Draw(gameTime);
        }

        protected override void UnloadContent()
        {
            heroTexture.Dispose();
            surfaceTexture.Dispose();

            foreach (Texture2D fgTexture in fg.sinusoidTextures)
            {
                fgTexture.Dispose();
            }
            fg.sinusoidTextures.Clear();
            fg.sinusoidSprites.Clear();

            foreach (Texture2D bgTexture in bg.backgroundTextures)
            {
                bgTexture.Dispose();
            }
            bg.backgroundTextures.Clear();
            bg.drawableBGSprites.Clear();

            osc.stopOSCServer();

            base.UnloadContent();
        }


    }
}