using System;
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
        /* Public static variables */
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
        private ItemsHandler itemsHandler;
        private Explosion explosion;
        private SkyHandler skyHandler;

        private float obstacleCreationChance = 0.2f; // 20 %

        /* SONG HARDCODED PROPERTIES */
        private float songDuration = 300;
        private float beatsPerMinute = 100;

        public Controller(Game game) : base(game)
        {
            Console.WriteLine("Controller instantiated");
        }

        /*
         * Loading texture and instantiating classes
         */
        protected override void LoadContent()
        {
            hero = new Hero();
            loadHeroTextures();

            lateInit();

            loadObstacleTextures();
            loadForegroundTextures();
            loadBackgroundTextures();
            loadEnemyTextures();
            loadItemsTextures();
            loadExplosionTexure();
            loadSkyTexture();

            itemsHandler.createItem();

            base.LoadContent();
        }

        private void lateInit()
        {
            hero.createHero();
            bg = new BackgroundHandler();
            fg = new ForegroundHandler();
            skyHandler = new SkyHandler();
            obstacleHandler = new ObstacleHandler();
            itemsHandler = new ItemsHandler(hero);
            enemy = new Enemy();            
            explosion = new Explosion();
            osc = new OSCHandler();

            RandomHandler.init();
            ColorHandler.loadColors();

            GameStateHandler.CurrentState = GameState.STARTING; 
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

        private void loadSkyTexture()
        {
            skyHandler.skyTexture = loadTexture("alianBugSwam");
        }

        private void loadExplosionTexure()
        {
            explosion.explosionTexture = loadTexture("Explosion");
        }

        private void loadHeroTextures()
        {
            hero.heroTex = loadTexture("Rami_Anim_GS");

            // Spaceship textures
            hero.shipHandler.shipCrashTex = loadTexture("ramiBotShipCrash");
            hero.shipHandler.shipRepairTex = loadTexture("ramiBotShipFix");
            hero.shipHandler.shipTakeOffTex = loadTexture("ramiBotShipTakeoff");
        }

        private void loadEnemyTextures()
        {
            enemy.enemyTextures.Add(loadTexture("Sheep_Anim_GS"));

            for (int i = 1; i <= 3; i++) 
            {
                enemy.enemyTextures.Add(loadTexture("alianBugType" + i.ToString()));
            }
        }

        private void loadItemsTextures()
        {
            int i = 0, j = 1, loops = 4;
            string itemAssetName = "ITEM_", 
                   itemType = "Bolt";

            for (i = 0; i < 5; i++)
            {
                switch (i)
                {
                    case 1: itemType = "Cog"; break;
                    case 2: itemType = "Wrench"; break;
                    case 3: itemType = "Hammer"; 
                            loops = 1; 
                            break;
                    case 4: itemType = "Screwdriver"; 
                            loops = 1; 
                            break;
                }

                        
                for (j = 1; j <= loops; j++)
                {
                    itemAssetName += itemType;
                    itemAssetName += j.ToString();
                    itemAssetName += "_Scaled_Animation";
                    itemsHandler.itemTextures.Add(loadTexture(itemAssetName));

                    itemAssetName = "ITEM_";
                }
            }
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

        /*
         * Updating
         */
        public override void Update(GameTime gameTime)
        {
            if (cameraHandler != null)
            {
                cameraHandler.updateCamera(gameTime, hero.Position);
            }

            bg.updateBackground(gameTime);
            fg.updateForeground(gameTime);
            hero.updateHero(gameTime);

            if (GameStateHandler.CurrentState == GameState.RUNNING)
            {
                obstacleHandler.updateObstacles(gameTime);
                explosion.updateExplosions(gameTime);
                enemy.updateEnemy(gameTime);
                itemsHandler.updateItems(gameTime);
                skyHandler.updateSky(gameTime);

                updateAllSprites();

                if (RandomHandler.GetRandomFloat(100f) < obstacleCreationChance * 100f)
                {
                    createObstacle(gameTime);
                }

                checkCollision(gameTime);

                if ((int)gameTime.TotalGameTime.TotalSeconds > songDuration-10)
                {
                    GameStateHandler.CurrentState = GameState.ENDING;
                    Console.WriteLine("Ending visualization");
                }
            }
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

                hero.startAction(state, gameTime);

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
            allSprites.AddRange(bg.bgMaskSprites);
            allSprites.AddRange(bg.bgSprites);
            allSprites.Add(hero);

            foreach (Sprite sprite in allSprites)
            {
                sprite.CheckIsActive();
            }
        }

        private void checkCollision(GameTime time)
        {
            foreach (Enemy enemySprite in enemy.enemySprites) 
            {
                foreach (Obstacle obstacle in obstacleHandler.obstacleSprites)
                {
                    if (getIsWithinRange(obstacle.Position.X, enemySprite.Position.X, 50f) &&
                        obstacle.Type != ObstacleType.SLIDE)
                    {
                        explosion.createExplosion(enemySprite, time);
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
        
        /*
         * Drawing
         */
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch batch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            GraphicsDeviceManager graphicsManager = (GraphicsDeviceManager)Game.Services.GetService(typeof(GraphicsDeviceManager));

            if (cameraHandler == null)
            {
                cameraHandler = new CameraHandler();                
            }

            batch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, cameraHandler.getTransformation(graphicsManager.GraphicsDevice));

            bg.drawBackground(batch, gameTime);
            fg.drawForeground(batch, gameTime);
            hero.drawHero(batch, gameTime);
            itemsHandler.drawItems(batch, gameTime);            

            if (GameStateHandler.CurrentState == GameState.RUNNING)
            {
                explosion.drawExplosions(batch, gameTime);
                obstacleHandler.drawObstacles(batch, gameTime);
                enemy.drawEnemy(batch, gameTime);
                skyHandler.drawSky(batch, gameTime);
            }
            batch.End();
 
            base.Draw(gameTime);
        }

        /*
         * Unloading textures
         */
        protected override void UnloadContent()
        {
            hero.heroTex.Dispose();

            hero.shipHandler.shipCrashTex.Dispose();
            hero.shipHandler.shipRepairTex.Dispose();
            hero.shipHandler.shipTakeOffTex.Dispose();

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

            foreach (Texture2D itemTexture in itemsHandler.itemTextures)
            {
                itemTexture.Dispose();
            }
            itemsHandler.itemSprites.Clear();

            osc.stopOSCServer();

            base.UnloadContent();
        }


    }
}