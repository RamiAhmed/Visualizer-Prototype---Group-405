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
        // Settings
        public static bool IS_FULL_SCREEN = false;
        public static string PARTICIPANT_ID = "";
        public static int SCENARIO_NUM = -1;

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
        private SpriteFont mainFont;

        /* SONG HARDCODED PROPERTIES */
        private float songDuration = 343;
        private float beatsPerMinute = 100;

        /* TIME LOGGING */
        private TimeSpan timeOfStart;

        public Controller(Game game) : base(game)
        {
            Console.WriteLine("Controller instantiated");
        }

        private void logTime(String message)
        {
            // Write the string to a file.
            System.IO.StreamWriter file = new System.IO.StreamWriter("c:\\test.log", true); // true == append to file
            
            file.WriteLine(message); 

            file.Close();
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

            mainFont = Game.Content.Load<SpriteFont>("MainFont");

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

            GameStateHandler.CurrentState = GameState.IDLE;

            handleSettingsInput();
        }

        private void handleSettingsInput()
        {
            int scenario = -1;
            while (scenario == -1)
            {
                Console.WriteLine("Please input scenario number: \n(0 = complete, 1 = no intro/outro, 2 = no enemies/obstacles)");
                scenario = Convert.ToInt32(Console.ReadLine());
            }
            SCENARIO_NUM = scenario;

            string id = "";
            while (id == "")
            {
                Console.WriteLine("Please input participant ID");
                id = Console.ReadLine();
            }
            PARTICIPANT_ID = id;

            string fullscreen = "";
            while (fullscreen == "")
            {
                Console.WriteLine("Choose FULL SCREEN (y or yes) or WINDOWED (n or no)");
                fullscreen = Console.ReadLine().ToLower();
            }

            if (fullscreen == "y" || fullscreen == "yes")
            {
                IS_FULL_SCREEN = true;
            }
            else if (fullscreen == "n" || fullscreen == "no")
            {
                IS_FULL_SCREEN = false;
            }
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

            if (GameStateHandler.CurrentState == GameState.IDLE)
            {
                if (OSCHandler.inAmplitude != 0f && OSCHandler.inBrightness != 0 && OSCHandler.inFundamentalFrequency != 0f &&
                    OSCHandler.inLoudness != 0f && OSCHandler.inNoise != 0f && OSCHandler.inPeakAmplitude != 0f && OSCHandler.inPitch != 0f &&
                    PARTICIPANT_ID != "" && SCENARIO_NUM != -1) // Only start when participant ID has been detected and values from Max start coming in
                {
                    if (SCENARIO_NUM == 1)
                    {
                        GameStateHandler.CurrentState = GameState.RUNNING;
                    }
                    else
                    {
                        GameStateHandler.CurrentState = GameState.STARTING;
                    }
                    timeOfStart = DateTime.Now.TimeOfDay;
                }

            }
            else
            {
                bg.updateBackground(gameTime);
                fg.updateForeground(gameTime);
                hero.updateHero(gameTime);
            }

            if (GameStateHandler.CurrentState == GameState.RUNNING)
            {
                if (SCENARIO_NUM != 1)
                {
                    itemsHandler.updateItems(gameTime);
                }
                else if (SCENARIO_NUM != 2)
                {
                    createObstacle(gameTime);
                    checkCollision(gameTime);
                    obstacleHandler.updateObstacles(gameTime);
                    explosion.updateExplosions(gameTime);
                    enemy.updateEnemy(gameTime);
                }
                
                skyHandler.updateSky(gameTime);

                updateAllSprites();
                

                if ((int)gameTime.TotalGameTime.TotalSeconds > songDuration-10)
                {
                    if (SCENARIO_NUM != 1)
                    {
                        GameStateHandler.CurrentState = GameState.ENDING;
                    }
                    Console.WriteLine("Ending visualization");
                }
            }
            base.Update(gameTime);
        }

        private void createObstacle(GameTime gameTime)
        {
            if (!getIsItemNear(hero))
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
        }

        private bool getIsItemNear(Hero heroRef)
        {
            bool near = false;
            float range = 100f;

            foreach (Sprite item in itemsHandler.itemSprites)
            {
                if (getIsWithinRange(heroRef.Position.X, item.Position.X, range))
                {
                    near = true;
                }
            }

            return near;
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
            if (enemy.enemySprites.Count > 0) 
            {
                foreach (Enemy enemySprite in enemy.enemySprites)
                {
                    if (getIsWithinRange(hero.Position.X, enemySprite.Position.X, 50f) && enemySprite.Speed > 0f)
                    {
                        enemySprite.Speed -= 0.05f;
                    }
                    else if (enemySprite.Position.X > enemy.deathPointX)
                    {
                        explosion.createExplosion(enemySprite, time);
                        enemy.removeEnemy(enemySprite);
                    }

                    if (obstacleHandler.obstacleSprites.Count > 0)
                    {
                        foreach (Obstacle obstacle in obstacleHandler.obstacleSprites)
                        {
                            if (getIsWithinRange(obstacle.Position.X, enemySprite.Position.X, 50f) && obstacle.Type != ObstacleType.SLIDE)
                            {
                                explosion.createExplosion(enemySprite, time);
                                enemy.removeEnemy(enemySprite);
                            }
                        }
                    }
                }
            }
        }

        private bool getIsWithinRange(float a, float b, float range)
        {
            bool inRange = false;
            if (Math.Abs(a - b) <= range)
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

            if (GameStateHandler.CurrentState == GameState.IDLE)
            {
                string text = "In IDLE mode. \nWaiting for input from Max.";
                Vector2 textPos = new Vector2(Controller.TOTAL_WIDTH * 0.5f - (mainFont.MeasureString(text).X * 0.5f), 
                                              Controller.TOTAL_HEIGHT * 0.5f - (mainFont.MeasureString(text).Y * 0.5f));
                batch.DrawString(mainFont, text, textPos, Color.WhiteSmoke); 
            }
            else 
            {
                bg.drawBackground(batch, gameTime);
                fg.drawForeground(batch, gameTime);
                hero.drawHero(batch, gameTime);
                if (SCENARIO_NUM != 1)
                {
                    itemsHandler.drawItems(batch, gameTime);
                }
            }
            if (GameStateHandler.CurrentState == GameState.RUNNING)
            {                
                if (SCENARIO_NUM != 2)
                {
                    explosion.drawExplosions(batch, gameTime);
                    obstacleHandler.drawObstacles(batch, gameTime);
                    enemy.drawEnemy(batch, gameTime);
                }
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

            timeOfStart = DateTime.Now.TimeOfDay.Subtract(timeOfStart);
            logTime("ID: " + PARTICIPANT_ID + ", Scenario: " + SCENARIO_NUM.ToString() + " - " + timeOfStart.ToString());

            base.UnloadContent();
        }


    }
}