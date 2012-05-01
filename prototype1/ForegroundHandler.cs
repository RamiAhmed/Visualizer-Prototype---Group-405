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
        // Surface variables (PAST PART)
        private long lastSurfaceMove = 0;
        private int surfaceMovementSpeed = 15;
        private float surfaceCurrentFrame = 0f;
        private int surfaceWidth = 350;
        public Texture2D surfaceTex;
        private Sprite surface;

        // Middle variables (PRESENT PART)
        //public Texture2D middleTexture;
        public List<Texture2D> middleTextures = new List<Texture2D>();
        private List<Sprite> middleSprites = new List<Sprite>();
        private int middleWidth = 250;
        private int middleX = 350,
                    middleY = 475;
        private float[] currentMiddleFrame = {0f, 0f, 0f};
        private long[] lastMiddleUpdate = {0, 0, 0};
        private int middleUpdateSpeed = 5;

        /* Sinusoid variables (UNKNOWN PART) */
        public List<Texture2D> sinusoidTextures = new List<Texture2D>();
        public List<Sprite> sinusoidSprites = new List<Sprite>();
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
            updateSurface(gameTime);

            if (sinusoidSprites.Count > 0)
            {
                updateSinusoids(gameTime);
            }
            else
            {
                createSinusoids();
            }

            if (middleSprites.Count > 0)
            {
                updateMiddleArea(gameTime);
            }
            else
            {
                createMiddleArea();
            }
        }

        public void drawForeground(SpriteBatch batch)
        {
            drawSurface(batch);
            drawSinusoids(batch);
            drawMiddleArea(batch);
        }

        private void createSurface()
        {
            if (surface == null)
            {
                surface = new Sprite();

                surface.Move(0, 470);

                surface.Texture = surfaceTex;
                surface.Width = surfaceWidth;
                surface.Height = surface.Texture.Height;
                surface.Color = Color.White;

                surface.Active = true;
            }
        }

        private void drawSurface(SpriteBatch batch)
        {
            if (surface != null && surface.Texture != null && surface.Active)
            {
                int animationX = (int)(surfaceCurrentFrame * surfaceWidth);
                Rectangle animationCycle = new Rectangle(animationX, 0, surface.Width, surface.Height);
                Rectangle surfaceRect = new Rectangle((int)surface.Position.X, (int)surface.Position.Y,
                                                            surface.Width, surface.Height);
                batch.Draw(surface.Texture, surfaceRect, animationCycle, Color.White);
            }
            else
            {
                createSurface();
            }
        }

        private void updateSurface(GameTime gameTime)
        {
            long currentMilliseconds = (long)gameTime.TotalGameTime.TotalMilliseconds;
            if (currentMilliseconds - lastSurfaceMove > surfaceMovementSpeed)
            {
                lastSurfaceMove = currentMilliseconds;

                if (surfaceCurrentFrame > (float)(surface.Texture.Width / surfaceWidth) - 1f)
                {
                    surfaceCurrentFrame = 0f;
                }
                else
                {
                    surfaceCurrentFrame += 0.01f;
                }
            }
        }

        private void createMiddleArea()
        {
            if (middleTextures.Count > 0)
            {
                for (int i = 0; i < 2; i++)
                {
                    Sprite middleStream = new Sprite();

                    middleStream.Texture = middleTextures[i];

                    middleStream.Color = Color.White;
                    middleStream.Width = middleWidth;
                    middleStream.Height = middleStream.Texture.Height;

                    middleStream.Move(middleX, middleY);
                    middleStream.Active = true;

                    middleSprites.Add(middleStream);
                }
            }
        }

        private void updateMiddleArea(GameTime gameTime)
        {
            if (middleTextures.Count > 0)
            {
                for (int i = 0; i < 2; i++)
                {
                    long currentMilliseconds = (long)gameTime.TotalGameTime.TotalMilliseconds;
                    if (currentMilliseconds - lastMiddleUpdate[i] > middleUpdateSpeed * (i + 1))
                    {
                        lastMiddleUpdate[i] = currentMilliseconds;
                        if (currentMiddleFrame[i] > (float)(middleTextures[i].Width / middleWidth) - 1f)
                        {
                            currentMiddleFrame[i] = 0f;
                        }
                        else
                        {
                            currentMiddleFrame[i] += 0.01f;
                        }
                    }
                }
            }
        }

        private void drawMiddleArea(SpriteBatch batch)
        {
            foreach (Sprite middleSprite in middleSprites)
            {
                if (middleSprite.Active)
                {
                    int index = middleSprites.IndexOf(middleSprite);
                    Rectangle middleAnimation = new Rectangle((int)(currentMiddleFrame[index] * middleWidth), 0,
                                                                    middleWidth, middleSprites[index].Height);
                    batch.Draw(middleSprite.Texture, middleSprite.Position, middleAnimation, middleSprite.Color);
                }
            }
        }

        private void createSinusoids()
        {
            if (sinusoidTextures.Count > 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    Sprite sinusoid = new Sprite();

                    sinusoid.Texture = sinusoidTextures[i];

                    sinusoid.Color = Color.White;
                    sinusoid.Width = sinusoidWidth;
                    sinusoid.Height = sinusoid.Texture.Height;
                    sinusoid.Move(sinusoidX, sinusoidY);

                    sinusoid.Active = true;

                    float layerDepth = 0f;
                    switch (i)
                    {
                        case 1: layerDepth = 0.9f; break;
                        case 2: layerDepth = 0.5f; break;
                        case 3: layerDepth = 0.1f; break;
                    }
                    sinusoid.LayerDepth = layerDepth;

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
                        Rectangle sinusoidRect = new Rectangle((int)sinusoid.Position.X, (int)sinusoid.Position.Y,
                                                                sinusoid.Width, sinusoid.Height);
                        batch.Draw(sinusoid.Texture, sinusoidRect, sinusoidAnimation, sinusoid.Color, 0f,
                                new Vector2(0, 0), SpriteEffects.None, sinusoid.LayerDepth);
                    }
                }
            }
        }

        private void updateSinusoids(GameTime gameTime)
        {
            long currentMilliseconds = (long)gameTime.TotalGameTime.TotalMilliseconds;
            for (int i = 0; i < 3; i++)
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
