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
    class SkyHandler
    {
        public List<Sprite> skySprites = new List<Sprite>();
        public Texture2D skyTexture;

        private Vector2 skyStartPos = new Vector2(0, 75);
        private int numFrames = 4;
        private float skyDefaultMoveSpeed = 1f;
        private float skyDefaultAnimSpeed = 10f;

        private long lastCreation = 0;
        private float creationFrequency = 10f; // every nth second

        public SkyHandler()
        {
        }

        private void createSkyElement()
        {
            Sprite skyElement = new Sprite();

            skyElement.Texture = skyTexture;

            skyElement.Width = skyTexture.Width / numFrames;
            skyElement.Height = skyTexture.Height;

            skyElement.Speed = skyDefaultMoveSpeed + RandomHandler.GetRandomFloat(2f);
            skyElement.ScaleFactor = 0.75f + RandomHandler.GetRandomFloat(0.5f);
            skyElement.Rotation = RandomHandler.GetRandomFloat(360);
            skyElement.LayerDepth = 1f;
            skyElement.Color = ColorHandler.getCurrentColor();
            
            skyElement.Active = true;
            skyElement.Move(-skyElement.Width, skyStartPos.Y);

            skySprites.Add(skyElement);
        }

        public void updateSky(GameTime time)
        {
            long currentMilliseconds = (long)time.TotalGameTime.TotalMilliseconds;
            if (currentMilliseconds - lastCreation > creationFrequency * 1000f)
            {
                if (RandomHandler.GetRandomFloat(1f) < 0.25f)
                {
                    lastCreation = currentMilliseconds;
                    createSkyElement();
                }
            }

            int skyCount = skySprites.Count;
            if (skyCount > 0)
            {
                for (int i = 0; i < skyCount; i++)
                {
                    Sprite skyElement = skySprites.ElementAt(i);
                    if (skyElement.Active)
                    {
                        if (skyElement.Position.X > Controller.TOTAL_WIDTH || skyElement.Position.Y < 0)
                        {
                            skyElement.Active = false;
                        }
                        else
                        {
                            skyElement.Move(skyElement.Position.X + skyElement.Speed,
                                            skyElement.Position.Y + RandomHandler.GetRandomFloat(-2.5f, 2.5f));
                        }
                    }
                    else
                    {
                        skySprites.RemoveAt(i);

                        i--;
                        skyCount--;
                    }
                }
            }
        }

        public void drawSky(SpriteBatch batch, GameTime time)
        {
            if (skySprites.Count > 0)
            {
                foreach (Sprite skyElement in skySprites)
                {
                    if (skyElement.Active)
                    {
                        int animationX = (int)(time.TotalGameTime.TotalSeconds * skyDefaultAnimSpeed) % numFrames;
                        Rectangle animCycle = new Rectangle(animationX * skyElement.Width, 0, skyElement.Width, skyElement.Height);

                        batch.Draw(skyElement.Texture, skyElement.Position, animCycle, skyElement.Color, skyElement.Rotation,
                                new Vector2(skyElement.Width * 0.5f, skyElement.Height * 0.5f), skyElement.ScaleFactor, SpriteEffects.None, skyElement.LayerDepth);
                    }

                }
            }
        }

    }
}
