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
        private int obstacleCreationFrequency = 5000; // every nth millisecond
        private long lastObstacleCreation = 0;

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
                    obstacle.Move(obstacle.Position.X - obstacle.Speed, obstacle.Position.Y);

                    int yOffset = getObstacleYOffset(obstacle);
                    obstacle.BoundingBox = new Rectangle((int)obstacle.Position.X, (int)obstacle.Position.Y,
                                                    obstacle.Width, obstacle.Height);
                }
                else
                {
                    obstacleSprites.RemoveAt(i);

                    i--;
                    obstacleSpritesCount--;
                }

            }
        }

        public void drawObstacles(SpriteBatch batch)
        {
            foreach (Obstacle obstacle in obstacleSprites)
            {
                int yOffset = getObstacleYOffset(obstacle);
                //Rectangle obsRect = new Rectangle((int)obstacle.Position.X, (int)obstacle.Position.Y, obstacle.Width, obstacle.Height);
                batch.Draw(obstacle.Texture, obstacle.BoundingBox, null, obstacle.Color, 0f, new Vector2(0, yOffset), SpriteEffects.None, obstacle.LayerDepth);
            }
        }


        public ObstacleType generateObstacles(GameTime gameTime)
        {
            long currentMilliseconds = (long)gameTime.TotalGameTime.TotalMilliseconds;
            if (currentMilliseconds - lastObstacleCreation > obstacleCreationFrequency)
            {
                lastObstacleCreation = currentMilliseconds;

                Obstacle newObs = createObstacle();
                if (newObs != null)
                {
                    return newObs.Type;
                }
            }

            return ObstacleType.NULL;
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
            Obstacle newObs = new Obstacle();

            ObstacleType obsType = ObstacleType.NULL;
            int typeOfObstacle = RandomHandler.GetRandomInt(0, 4);
            Texture2D obstacleTexture = null;
            switch (typeOfObstacle)
            {
                case 0: obstacleTexture = getRandomHoleTexture();
                        obsType = ObstacleType.HOLE; 
                        break;
                case 1: obstacleTexture = getRandomHillTexture();
                        obsType = ObstacleType.HILL;
                        break;
                case 2: obstacleTexture = getRandomSlideTexture();
                        obsType = ObstacleType.SLIDE;
                        break;
                case 3: obstacleTexture = getRandomWallTexture();
                        obsType = ObstacleType.WALL;
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
                newObs.Width = newObs.Texture.Width;
                newObs.Height = newObs.Texture.Height;
                newObs.Move(obstacleCreationPointX, obstacleCreationPointY);
                newObs.Speed = 2.5f;
                newObs.Active = true;

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
            Texture2D wallTex = null;

            if (wallTextures.Count > 0)
            {
                foreach (Texture2D tex in wallTextures)
                {
                    if (RandomHandler.GetRandomFloat(1) < 0.01f)
                    {
                        wallTex = tex;
                        break;
                    }
                }

                if (wallTex != null)
                {
                    return wallTex;
                }
                else
                {
                    return getRandomWallTexture();
                }
            }
            else
            {
                return null;
            }
            }

        private Texture2D getRandomSlideTexture()
        {
            Texture2D slideTex = null;

            if (slideTextures.Count > 0)
            {
                foreach (Texture2D tex in slideTextures)
                {
                    if (RandomHandler.GetRandomFloat(1) < 0.01f)
                    {
                        slideTex = tex;
                        break;
                    }
                }

                if (slideTex != null)
                {
                    return slideTex;
                }
                else
                {
                    return getRandomSlideTexture();
                }
            }
            else
            {
                return null;
            }
        }

        private Texture2D getRandomHillTexture()
        {
            Texture2D hillTex = null;

            if (hillTextures.Count > 0)
            {
                foreach (Texture2D tex in hillTextures)
                {
                    if (RandomHandler.GetRandomFloat(1) < 0.01f)
                    {
                        hillTex = tex;
                        break;
                    }
                }


                if (hillTex != null)
                {
                    return hillTex;
                }
                else
                {
                    return getRandomHillTexture();
                }
            }
            else
            {
                return null;
            }
        }

        private Texture2D getRandomHoleTexture()
        {
            Texture2D holeTex = null;

            if (holeTextures.Count > 0)
            {
                foreach (Texture2D tex in holeTextures)
                {
                    if (RandomHandler.GetRandomFloat(1) < 0.01f)
                    {
                        holeTex = tex;
                        break;
                    }
                }

                if (holeTex != null)
                {
                    return holeTex;
                }
                else
                {
                    return getRandomHoleTexture();
                }
            }
            else
            {
                return null;
            }
        }
    }
}
