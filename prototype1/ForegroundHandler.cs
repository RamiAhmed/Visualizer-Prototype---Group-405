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
    class ForegroundHandler
    {
        public List<Texture2D> sinusoidTextures = new List<Texture2D>();
        public List<Sprite> sinusoidSprites = new List<Sprite>();

        // Middle variables
        public Texture2D middleTexture;
        private Sprite middleStream;
        private int middleWidth = 325;
        private int middleX = 350,
                    middleY = 475;

        /* Sinusoid variables */        
        private int sinusoidWidth = 420;
        private int sinusoidX = 600,
                    sinusoidY = 475;
        private float[] currentSinusoidFrame = {0f, 0f, 0f};
        private long[] lastSinusoidUpdate = {0, 0, 0};
        private int sinusoidUpdateSpeed = 50;

        public ForegroundHandler()
        {
        }

        public void updateForeground(GameTime gameTime)
        {
            if (sinusoidSprites.Count > 0)
            {
                updateSinusoids(gameTime);
            }
            else
            {
                createSinusoids();
            }
        }

        public void drawForeground(SpriteBatch batch)
        {
            drawSinusoids(batch);
        }

        private void createMiddleArea()
        {
            if (middleTexture != null)
            {
                middleStream = new Sprite();

                middleStream.Color = Color.White;
                middleStream.Width = 
            }
        }

        private void createSinusoids()
        {
            if (sinusoidTextures.Count > 0)
            {
                for (int i = 0; i < 2; i++)
                {
                    Sprite sinusoid = new Sprite();

                    sinusoid.Texture = sinusoidTextures[i];

                    sinusoid.Color = Color.White;
                    sinusoid.Width = sinusoidWidth;
                    sinusoid.Height = sinusoid.Texture.Height;
                    sinusoid.Move(sinusoidX, sinusoidY);

                    sinusoid.Active = true;

                    sinusoidSprites.Add(sinusoid);
                }
            }
        }

        private void drawSinusoids(SpriteBatch batch)
        {
            if (sinusoidSprites.Count > 0)
            {
                foreach (Sprite sinusoid in sinusoidSprites)
                {
                    if (sinusoid.Active)
                    {
                        int index = sinusoidSprites.IndexOf(sinusoid);
                        Rectangle sinusoidAnimation = new Rectangle((int)(currentSinusoidFrame[index] * sinusoidWidth), 0, 
                                                                     sinusoidWidth, sinusoid.Height);
                        batch.Draw(sinusoid.Texture, sinusoid.Position, sinusoidAnimation, sinusoid.Color);
                    }
                }
            }
        }

        private void updateSinusoids(GameTime gameTime)
        {
            long currentMilliseconds = (long)gameTime.TotalGameTime.TotalMilliseconds;
            for (int i = 0; i < 2; i++)
            {
                if ((currentMilliseconds - lastSinusoidUpdate[i]) > (sinusoidUpdateSpeed * (i+1)))
                {
                    lastSinusoidUpdate[i] = currentMilliseconds;
                    if (currentSinusoidFrame[i] > 3f)
                    {
                        currentSinusoidFrame[i] = 0f;
                    }
                    else
                    {
                        currentSinusoidFrame[i] += 0.05f;
                    }
                }
            }
        }
    }
}
