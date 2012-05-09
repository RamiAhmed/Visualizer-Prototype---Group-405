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
        private int foregroundY = 475;
        private int overlapArea = 5;

        // Surface variables (PAST PART)
        public Texture2D surfaceTex;
        private Sprite surface;
        private int surfaceWidth = 500;
        private int surfaceMovementSpeed = 150;

        // Connector
        public Texture2D connectorTex;
        private Sprite connector;
        private int connectorWidth = 40;
        private int connectorSpeed = 150;

        // Middle variables (PRESENT PART)
        public List<Texture2D> middleTextures = new List<Texture2D>();
        private List<Sprite> middleSprites = new List<Sprite>();
        private int middleWidth = 224;
        private int middleX = -1;
        private int middleUpdateSpeed = 150;

        // The Mask
        public Texture2D maskTex;
        private Sprite connectorMask;
        private int maskWidth = 40;
        private int maskSpeed = 150;

        // Sinusoid variables (UNKNOWN PART) 
        public List<Texture2D> sinusoidTextures = new List<Texture2D>();
        public List<Sprite> sinusoidSprites = new List<Sprite>();
        private int sinusoidWidth = 300;
        private int sinusoidX = -1;
        private int sinusoidUpdateSpeed = 150;

        public ForegroundHandler()
        {
            middleWidth += overlapArea;
            sinusoidWidth += overlapArea;

            middleX = surfaceWidth;
            sinusoidX = middleX + middleWidth - overlapArea+5;
        }

        public void updateForeground(GameTime gameTime)
        {
            if (sinusoidSprites.Count <= 0)
            {
                createSinusoids();
            }


            if (middleSprites.Count <= 0)
            {
                createMiddleArea();
            }

            if (surface == null)
            {
                createSurface();
            }

            if (connector == null)
            {
                createConnector();
            }

            if (connectorMask == null)
            {
                createMask();
            }
        }

        public void drawForeground(SpriteBatch batch, GameTime gameTime)
        {
            drawSurface(batch, gameTime);
            drawSinusoids(batch, gameTime);
            drawMiddleArea(batch, gameTime);
            drawConnector(batch, gameTime);
            drawMask(batch, gameTime);
        }

        private void createMask()
        {
            if (connectorMask == null)
            {
                connectorMask = new Sprite();

                connectorMask.Texture = maskTex;
                connectorMask.Width = maskWidth;
                connectorMask.Height = connectorMask.Texture.Height;

                connectorMask.Speed = 0.5f;
                connectorMask.Color = Color.White;
                connectorMask.LayerDepth = 0.2f + RandomHandler.GetRandomFloat(0.001f);

                connectorMask.Active = true;

                connectorMask.Move(sinusoidX, foregroundY);
            }
        }

        private void drawMask(SpriteBatch batch, GameTime gameTime)
        {
            if (connectorMask != null && connectorMask.Texture != null && connectorMask.Active)
            {
                int animationX = (int)(gameTime.TotalGameTime.TotalSeconds * maskSpeed * connectorMask.Speed) % 1800;
                if (!Hero.heroReady)
                {
                    animationX = 0;
                }

                Rectangle maskCycle = new Rectangle(animationX, 0, connectorMask.Width, connectorMask.Height);
                Rectangle maskRect = new Rectangle((int)connectorMask.Position.X, (int)connectorMask.Position.Y,
                                                    connectorMask.Width, connectorMask.Height);

                batch.Draw(connectorMask.Texture, maskRect, maskCycle, connectorMask.Color, 0f, 
                        new Vector2(0, 0), SpriteEffects.None, connectorMask.LayerDepth);
            }
        }

        private void createConnector()
        {
            if (connector == null)
            {
                connector = new Sprite();

                connector.Texture = connectorTex;
                connector.Width = connectorWidth;
                connector.Height = connector.Texture.Height;

                connector.Speed = 1f;
                connector.Color = Color.White;
                connector.LayerDepth = 0.2f;

                connector.Active = true;

                connector.Move(middleX - (connector.Width / 4), foregroundY);
            }
        }

        private void drawConnector(SpriteBatch batch, GameTime gameTime)
        {
            if (connector != null && connector.Texture != null && connector.Active)
            {
                int animationX = (int)(gameTime.TotalGameTime.TotalSeconds * connectorSpeed * connector.Speed) % 1800;
                if (!Hero.heroReady)
                {
                    animationX = 0;
                }

                Rectangle connectorCycle = new Rectangle(animationX, 0, connector.Width, connector.Height);
                Rectangle connectRect = new Rectangle((int)connector.Position.X, (int)connector.Position.Y,
                                                            connector.Width, connector.Height);

                batch.Draw(connector.Texture, connectRect, connectorCycle, connector.Color, 0f, new Vector2(0, 0), SpriteEffects.None, connector.LayerDepth);
            }
        }

        private void createSurface()
        {
            if (surface == null)
            {
                surface = new Sprite();

                surface.Move(0, foregroundY);
                surface.Speed = 1f;
                surface.Texture = surfaceTex;
                surface.Width = surfaceWidth;
                surface.Height = surface.Texture.Height;
                surface.Color = Color.White;
                surface.LayerDepth = 0.6f;

                surface.Active = true;
            }
        }

        private void drawSurface(SpriteBatch batch, GameTime gameTime)
        {
            if (surface != null && surface.Texture != null && surface.Active)
            {
                int animationX = (int)(gameTime.TotalGameTime.TotalSeconds * surfaceMovementSpeed * surface.Speed) % 1800;
                if (!Hero.heroReady)
                {
                    animationX = 0;
                }

                Rectangle animationCycle = new Rectangle(animationX, 0, surface.Width, surface.Height);
                Rectangle surfaceRect = new Rectangle((int)surface.Position.X, (int)surface.Position.Y,
                                                            surface.Width, surface.Height);
                batch.Draw(surface.Texture, surfaceRect, animationCycle, surface.Color, 0f, new Vector2(0,0), SpriteEffects.None, surface.LayerDepth);
            }
        }

        private void createMiddleArea()
        {
            int middleTexturesCount = middleTextures.Count;
            if (middleTexturesCount > 0)
            {
                for (int i = 0; i < middleTexturesCount; i++)
                {
                    Sprite middleStream = new Sprite();

                    middleStream.Texture = middleTextures[i];
                    middleStream.Speed = (i + 1f) * 0.5f;
                    middleStream.Color = Color.White;
                    middleStream.Width = middleWidth;
                    middleStream.Height = middleStream.Texture.Height;

                    float layerDepth = 0f;
                    switch (i)
                    {
                        case 1: layerDepth = 0.0095f; break;
                        case 2: layerDepth = 0.0094f; break;
                        case 3: layerDepth = 0.0093f; break;
                    }
                    middleStream.LayerDepth = layerDepth + RandomHandler.GetRandomFloat(0.0001f);

                    middleStream.Move(middleX, foregroundY);
                    middleStream.Active = true;

                    middleSprites.Add(middleStream);
                }
            }
        }

        private void drawMiddleArea(SpriteBatch batch, GameTime gameTime)
        {
            foreach (Sprite middleSprite in middleSprites)
            {
                if (middleSprite.Active)
                {
                    int animationX = (int)(gameTime.TotalGameTime.TotalSeconds * middleUpdateSpeed * middleSprite.Speed) % 1800;
                    if (!Hero.heroReady)
                    {
                        animationX = 0;
                    }

                    Rectangle middleAnimation = new Rectangle(animationX, 0, middleSprite.Width, middleSprite.Height);

                    batch.Draw(middleSprite.Texture, middleSprite.Position, middleAnimation, middleSprite.Color);
                }
            }
        }

        private void createSinusoids()
        {
            if (sinusoidTextures.Count > 0)
            {
                for (int i = 0; i < sinusoidTextures.Count; i++)
                {
                    Sprite sinusoid = new Sprite();

                    sinusoid.Texture = sinusoidTextures[i];

                    sinusoid.Color = Color.White;
                    sinusoid.Width = sinusoidWidth;
                    sinusoid.Height = sinusoid.Texture.Height;
                    sinusoid.Move(sinusoidX, foregroundY);
                    sinusoid.Speed = (i + 1f) * 0.5f;
                    sinusoid.Active = true;

                    float layerDepth = 0f;
                    switch (i)
                    {
                        case 1: layerDepth = 0.0095f; break;
                        case 2: layerDepth = 0.0094f; break;
                        case 3: layerDepth = 0.0093f; break;
                    }
                    sinusoid.LayerDepth = layerDepth + RandomHandler.GetRandomFloat(0.0001f);

                    sinusoidSprites.Add(sinusoid);
                }
            }
        }

        private void drawSinusoids(SpriteBatch batch, GameTime gameTime)
        {
            if (sinusoidSprites.Count > 0)
            {
                foreach (Sprite sinusoid in sinusoidSprites)
                {
                    if (sinusoid.Active)
                    {
                        int animationX = (int)(gameTime.TotalGameTime.TotalSeconds * sinusoidUpdateSpeed * sinusoid.Speed) % 1800;
                        if (!Hero.heroReady)
                        {
                            animationX = 0;
                        }

                        Rectangle sinusoidAnimation = new Rectangle(animationX, 0, sinusoid.Width, sinusoid.Height);
                        Rectangle sinusoidRect = new Rectangle((int)sinusoid.Position.X, (int)sinusoid.Position.Y,
                                                                sinusoid.Width, sinusoid.Height);

                        batch.Draw(sinusoid.Texture, sinusoidRect, sinusoidAnimation, sinusoid.Color, 0f,
                                new Vector2(0, 0), SpriteEffects.None, sinusoid.LayerDepth);
                    }
                }
            }
        }

    }
}
