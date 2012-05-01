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
    class BackgroundHandler : Sprite
    {
        // Class lists
        public List<Sprite> drawableBGSprites = new List<Sprite>();
        public List<Texture2D> backgroundTextures = new List<Texture2D>();

        // Creation of background elements
        private int xCreationPos = Prototype.TOTAL_WIDTH;
        private int[] yCreationPos = { 250, 350, 450 };
        private float globalScale = 0.75f; // all bg objects are scaled by this value        
        private long lastGeneration = 0;
        private int generationSpeed = 250; // every nth millisecond



        public BackgroundHandler()
        {
        }

        public void updateBackground(GameTime gameTime)
        {
            long currentMilliseconds = (long)gameTime.TotalGameTime.TotalMilliseconds;
            if (currentMilliseconds - lastGeneration > generationSpeed)
            {
                lastGeneration = currentMilliseconds;
                generateNewBackgroundObject();
            }

            int drawableSpritesCount = drawableBGSprites.Count;
            if (drawableSpritesCount > 0)
            {
                for (int i = 0; i < drawableSpritesCount; i++)
                {
                    Sprite bgSprite = drawableBGSprites.ElementAt(i);
                    if (!bgSprite.Active)
                    {
                        drawableBGSprites.RemoveAt(drawableBGSprites.IndexOf(bgSprite));

                        i--;
                        drawableSpritesCount--;
                    }
                    else
                    {
                        bgSprite.Move(bgSprite.Position.X - bgSprite.Speed, bgSprite.Position.Y);
                    }
                }
            }
        }

        public void drawBackground(SpriteBatch batch)
        {
            if (drawableBGSprites.Count > 0)
            {
                foreach (Sprite bgSprite in drawableBGSprites)
                {
                    if (bgSprite.Active)
                    {
                        batch.Draw(bgSprite.Texture, bgSprite.Position, null, bgSprite.Color, 0, 
                            new Vector2(0, bgSprite.Height), bgSprite.ScaleFactor * globalScale, SpriteEffects.None, bgSprite.LayerDepth);
                    }
                }
            }
        }

        private Sprite placeOnRandomLayer(Sprite bgSprite)
        {
            // Layer 1 is the foremost, layer 3 is the furthest away
            int randomLayer = RandomHandler.GetRandomInt(1, 3);

            bgSprite.Speed = 4 - randomLayer;

            float yPos = yCreationPos[3 - randomLayer],
                  scaleFactor = 1f,
                  layerDepth = 1f;

            bgSprite.Move(xCreationPos, yPos);

            switch (randomLayer)
            {
                case 1: layerDepth = 0.9f; // layer 1 is foremost (front)
                        scaleFactor = 1.25f; 
                        break;
                case 2: layerDepth = 0.5f;  // layer 2 is in the middle
                        scaleFactor = 1f; 
                        break;
                case 3: layerDepth = 0.1f; // layer 3 is furthest away (back)
                        scaleFactor = 0.75f;
                        break;
                default: layerDepth = 0f;
                         scaleFactor = 1f;
                         break;
            }
            bgSprite.LayerDepth = layerDepth + RandomHandler.GetRandomFloat(0.1f);
            bgSprite.ScaleFactor = scaleFactor;

            return bgSprite;
        }

        private void generateNewBackgroundObject()
        {
            Sprite bgObject = new Sprite();
            bgObject.Texture = getRandomBGTexture();

            bgObject.Active = true;
            bgObject.Width = bgObject.Texture.Width;
            bgObject.Height = bgObject.Texture.Height;
            bgObject.Color = new Color(RandomHandler.GetRandomFloat(1), RandomHandler.GetRandomFloat(1), RandomHandler.GetRandomFloat(1));

            bgObject = placeOnRandomLayer(bgObject);

            drawableBGSprites.Add(bgObject);
        }

        private Texture2D getRandomBGTexture()
        {
            Texture2D returnTexture = null;
            foreach (Texture2D texture in backgroundTextures)
            {
                if (RandomHandler.GetRandomFloat(1) < 0.01f)
                {
                    returnTexture = texture;
                    break;
                }
            }

            if (returnTexture == null)
            {
                return getRandomBGTexture();
            }
            else
            {
                return returnTexture;
            }
        }
    }
}
