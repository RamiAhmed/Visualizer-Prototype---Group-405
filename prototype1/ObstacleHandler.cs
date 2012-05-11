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
    class ObstacleHandler
    {
        public List<Texture2D> holeTextures = new List<Texture2D>();
        public List<Texture2D> hillTextures = new List<Texture2D>();
        public List<Texture2D> slideTextures = new List<Texture2D>();
        public List<Texture2D> wallTextures = new List<Texture2D>();
        public List<Obstacle> obstacleSprites = new List<Obstacle>();

        private int obstacleCreationPointX = 475,
                    obstacleCreationPointY = 540;
        private int obstacleCreationFrequency = 1500; // every nth millisecond
        private long lastObstacleCreation = 0;
        private float obstacleAnimSpeed = 5f;
        private float obstacleStartWait = 10f;

        public ObstacleHandler()
        {
        }

        public void updateObstacles(GameTime gameTime)
        {
            int obstacleSpritesCount = obstacleSprites.Count;
            for (int i = 0; i < obstacleSpritesCount; i++) 
            {
                Obstacle obstacle = obstacleSprites.ElementAt(i);
                if (obstacle.Active)
                {
                    if (Hero.heroReady)
                    {
                        obstacle.Move(obstacle.Position.X - obstacle.Speed, obstacle.Position.Y);

                        obstacle.BoundingBox = new Rectangle((int)obstacle.Position.X, (int)obstacle.Position.Y,
                                                        obstacle.Width, obstacle.Height);
                    }
                }
                else
                {
                    obstacleSprites.RemoveAt(i);

                    i--;
                    obstacleSpritesCount--;
                }

            }
        }

        public void drawObstacles(SpriteBatch batch, GameTime gameTime)
        {
            foreach (Obstacle obstacle in obstacleSprites)
            {
                if (obstacle.Active)
                {
                    Rectangle animCycle;
                    if (obstacle.AnimateOnDeath && obstacle.ReadyToAnimate)
                    {
                        int animationX = (int)(gameTime.TotalGameTime.TotalSeconds * obstacleAnimSpeed) % 5;
                        if (animationX >= 4 && !obstacle.StayAtFrame)
                        {
                            obstacle.StayAtFrame = true;
                        }

                        if (obstacle.StayAtFrame)
                        {
                            animCycle = new Rectangle(4 * obstacle.Width, 0, obstacle.Width, obstacle.Height);
                        }
                        else
                        {
                            animCycle = new Rectangle(animationX * obstacle.Width, 0, obstacle.Width, obstacle.Height);
                        }
                    }
                    else
                    {
                        animCycle = new Rectangle(0, 0, obstacle.Width, obstacle.Height);
                    }

                    int yOffset = getObstacleYOffset(obstacle);

                    batch.Draw(obstacle.Texture, obstacle.BoundingBox, animCycle, obstacle.Color, 0f, 
                            new Vector2(0, yOffset), SpriteEffects.None, obstacle.LayerDepth);
                }
            }
        }


        public Obstacle generateObstacles(GameTime gameTime)
        {
            Obstacle newObs = null;
            long currentMilliseconds = (long)gameTime.TotalGameTime.TotalMilliseconds;
            if (currentMilliseconds > obstacleStartWait * 1000f)
            {
                if (currentMilliseconds - lastObstacleCreation > obstacleCreationFrequency)
                {
                    lastObstacleCreation = currentMilliseconds;

                    float freq = OSCHandler.inFundamentalFrequency;
                    int bright = OSCHandler.inBrightness;
                    if (freq > 100f && freq < 180f && bright < 400)
                    {
                        newObs = createObstacle(ObstacleType.HOLE);
                    }
                    else if (freq > 100f && freq < 180f && bright > 400)
                    {
                        newObs = createObstacle(ObstacleType.HILL);
                    }
                    else if (freq < 100 && bright < 100)
                    {
                        newObs = createObstacle(ObstacleType.WALL);
                    }
                    else if (freq > 180f && freq < 500f && bright > 500)
                    {
                        newObs = createObstacle(ObstacleType.SLIDE);
                    }
                }
            }
            return newObs;
        }

        private int getObstacleYOffset(Obstacle obstacle)
        {
            int yOffset = obstacle.Height;
            switch (obstacle.Type)
            {
                case ObstacleType.HILL: yOffset -= 2; break;
                case ObstacleType.HOLE: yOffset -= 34; break;
                case ObstacleType.SLIDE: yOffset += 50; break;
                case ObstacleType.WALL: yOffset += 0; break;
            }
            return yOffset;
        }

        private Obstacle createObstacle()
        {
            ObstacleType obsType = ObstacleType.NULL;
            int typeOfObstacle = RandomHandler.GetRandomInt(0, 4);
            switch (typeOfObstacle)
            {
                case 0: obsType = ObstacleType.HOLE; break;
                case 1: obsType = ObstacleType.HILL; break;
                case 2: obsType = ObstacleType.SLIDE; break;
                case 3: obsType = ObstacleType.WALL; break;
            }
            return createObstacle(obsType);
        }

        private Obstacle createObstacle(ObstacleType obsType)
        {
            Obstacle newObs = new Obstacle();

            int newWidth = 0;
            Texture2D obstacleTexture = null;
            switch (obsType)
            {
                case ObstacleType.HOLE: obstacleTexture = getRandomHoleTexture(); break;
                case ObstacleType.HILL: obstacleTexture = getRandomHillTexture(); break;
                case ObstacleType.SLIDE: obstacleTexture = getRandomSlideTexture(); break;
                case ObstacleType.WALL: obstacleTexture = getRandomWallTexture();
                        newObs.AnimateOnDeath = true;
                        newWidth = 100;
                        break;
            }

            if (obstacleTexture != null)
            {
                if (obsType != ObstacleType.NULL)
                {
                    newObs.Type = obsType;
                }

                newObs.Texture = obstacleTexture;

                newObs.Color = ColorHandler.getCurrentColor();

                if (newWidth == 0)
                {
                    newWidth = newObs.Texture.Width;
                }

                newObs.Width = newWidth;
                newObs.Height = newObs.Texture.Height;
                newObs.Move(obstacleCreationPointX, obstacleCreationPointY);
                newObs.AnimationSpeed = obstacleAnimSpeed;
                newObs.Speed = 2.5f;
                newObs.LayerDepth = 0.1f;
                newObs.Active = true;

                newObs.ReadyToAnimate = false;
                newObs.StayAtFrame = false;
                if (newObs.AnimateOnDeath != true)
                {
                    newObs.AnimateOnDeath = false;
                }

                obstacleSprites.Add(newObs);
            }
            else
            {
                newObs = null;
            }

            return newObs;
        }

        private Texture2D getRandomWallTexture()
        {
            if (wallTextures.Count > 0)
            {
                return getRandomTexture(wallTextures);
            }
            else
            {
                return null;
            }
            }

        private Texture2D getRandomSlideTexture()
        {
            if (slideTextures.Count > 0)
            {
                return getRandomTexture(slideTextures);
            }
            else
            {
                return null;
            }
        }

        private Texture2D getRandomHillTexture()
        {
            if (hillTextures.Count > 0)
            {
                return getRandomTexture(hillTextures);
            }
            else
            {
                return null;
            }
        }

        private Texture2D getRandomHoleTexture()
        {
            if (holeTextures.Count > 0)
            {
                return getRandomTexture(holeTextures);
            }
            else
            {
                return null;
            }
        }

        private Texture2D getRandomTexture(List<Texture2D> anyList)
        {
            return anyList.ElementAt(RandomHandler.GetRandomInt(anyList.Count - 1));
        }
    }
}
