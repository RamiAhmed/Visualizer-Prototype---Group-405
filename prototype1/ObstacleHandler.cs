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
        public List<Obstacle> obstacleSprites = new List<Obstacle>();

        private int obstacleCreationPointX = 450,
                    obstacleCreationPointY = 540;
        private int obstacleCreationFrequency = 5000; // every nth millisecond
        private long lastObstacleCreation = 0;

        public ObstacleHandler()
        {
        }

        public void updateObstacles(GameTime gameTime)
        {
            generateObstacles(gameTime);
            foreach (Obstacle obstacle in obstacleSprites)
            {
                obstacle.Move(obstacle.Position.X - obstacle.Speed, obstacle.Position.Y);
            }
        }

        public void drawObstacles(SpriteBatch batch)
        {
            foreach (Obstacle obstacle in obstacleSprites)
            {
                int yOffset = 0;
                switch (obstacle.Type)
                {
                    case ObstacleType.HILL: yOffset = obstacle.Height - 2;  break;
                    case ObstacleType.HOLE: yOffset = obstacle.Height - 25;  break;
                    case ObstacleType.SLIDE: yOffset = (int)(obstacle.Height * 1.1f);  break;
                }
                Rectangle obsRect = new Rectangle((int)obstacle.Position.X, (int)obstacle.Position.Y,
                                                        obstacle.Width, obstacle.Height);
                batch.Draw(obstacle.Texture, obsRect, null, obstacle.Color, 0f, new Vector2(0, yOffset), SpriteEffects.None, obstacle.LayerDepth);
            }
        }

        private void generateObstacles(GameTime gameTime)
        {
            long currentMilliseconds = (long)gameTime.TotalGameTime.TotalMilliseconds;
            if (currentMilliseconds - lastObstacleCreation > obstacleCreationFrequency)
            {
                lastObstacleCreation = currentMilliseconds;

                Obstacle newObs = createObstacle();
                if (newObs != null)
                {
                    obstacleSprites.Add(newObs);
                }
            }
        }

        private Obstacle createObstacle()
        {
            Obstacle newObs = new Obstacle();

            ObstacleType obsType = ObstacleType.NULL;
            int typeOfObstacle = RandomHandler.GetRandomInt(0, 3);
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
            }
            if (obstacleTexture != null)
            {
                if (obsType != ObstacleType.NULL)
                {
                    newObs.Type = obsType;
                }

                newObs.Texture = obstacleTexture;

                // newObs.Color = new Color(RandomHandler.GetRandomFloat(1), RandomHandler.GetRandomFloat(1), RandomHandler.GetRandomFloat(1));
                newObs.Color = Color.White;
                newObs.Width = newObs.Texture.Width;
                newObs.Height = newObs.Texture.Height;
                newObs.Move(obstacleCreationPointX, obstacleCreationPointY);
                newObs.Speed = 2.5f;
                newObs.Active = true;
            }
            else
            {
                newObs = null;
            }

            return newObs;
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
