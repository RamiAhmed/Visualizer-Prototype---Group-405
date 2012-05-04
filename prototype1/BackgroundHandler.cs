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
    class BackgroundHandler 
    {
        // Class lists
        public List<Sprite> bgSprites = new List<Sprite>();
        public List<Texture2D> backgroundTextures = new List<Texture2D>();
        public List<Texture2D> fogTextures = new List<Texture2D>();

        public Texture2D maskTex;
        public List<Sprite> maskSprites = new List<Sprite>();

        // Creation of background elements
        private int xCreationPos = Controller.TOTAL_WIDTH;
        private int[] yCreationPos = { 390, 425, 460 };
        private float[] scaleValues = { 1f, 0.75f, 0.5f };
        private float globalScale = 0.9f; // all bg objects are scaled by this value        
        private long lastGeneration = 0;
        private int generationSpeed = 250; // every nth millisecond
        private bool fogCreated = false;
        private Sprite fog;

        public BackgroundHandler()
        {
        }

        public void updateBackground(GameTime gameTime)
        {
            if (!fogCreated)
            {
                createFog();
                fogCreated = true;
            }

            if (maskSprites.Count <= 0) 
            {
                createMask();
            }

            long currentMilliseconds = (long)gameTime.TotalGameTime.TotalMilliseconds;
            if (currentMilliseconds - lastGeneration > generationSpeed)
            {
                lastGeneration = currentMilliseconds;
                generateNewBackgroundObject();
            }

            int drawableSpritesCount = bgSprites.Count;
            if (drawableSpritesCount > 0)
            {
                for (int i = 0; i < drawableSpritesCount; i++)
                {
                    Sprite bgSprite = bgSprites.ElementAt(i);
                    if (!bgSprite.Active)
                    {
                        bgSprites.RemoveAt(i);

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

        public void drawBackground(SpriteBatch batch, GameTime gameTime)
        {
            
            if (bgSprites.Count > 0)
            {
                drawMask(batch, gameTime);

                foreach (Sprite bgSprite in bgSprites)
                {
                    if (bgSprite.Active)
                    {
                        Color spriteColor = bgSprite.Color;
                        if (bgSprite.Position.X > 550)
                        {
                            spriteColor = Color.Gray;
                        }

                        if (bgSprite.ScaleFactor == 1f && bgSprite.Speed == 0f)
                        {
                            Rectangle spriteRect = new Rectangle((int)bgSprite.Position.X, (int)bgSprite.Position.Y,
                                                                    bgSprite.Width, bgSprite.Height);
                            batch.Draw(bgSprite.Texture, spriteRect, null, spriteColor, 0f, 
                                new Vector2(0, 0), SpriteEffects.None, bgSprite.LayerDepth);
                        }
                        else
                        {
                            batch.Draw(bgSprite.Texture, bgSprite.Position, null, spriteColor, 0,
                                new Vector2(0, bgSprite.Height), bgSprite.ScaleFactor * globalScale, SpriteEffects.None, bgSprite.LayerDepth);
                        }
                    }
                }
            }
        }

        private void drawMask(SpriteBatch batch, GameTime time)
        {
            if (maskSprites.Count > 0)
            {
                foreach (Sprite maskSprite in maskSprites)
                {
                    int animationX = (int)(time.TotalGameTime.TotalSeconds * maskSprite.Speed) % 10;

                    Rectangle maskCycle = new Rectangle(animationX * maskSprite.Width, 0, maskSprite.Width, maskSprite.Height);
                    Rectangle maskRect = new Rectangle((int)maskSprite.Position.X, (int)maskSprite.Position.Y,
                                                            maskSprite.Width, maskSprite.Height);

                    batch.Draw(maskSprite.Texture, maskRect, maskCycle, maskSprite.Color, 0f, new Vector2(0, 0), SpriteEffects.None, maskSprite.LayerDepth);
                }
            }
        }

        private void createMask()
        {
            if (maskSprites.Count <= 0)
            {
                for (int i = 0; i < 2; i++)
                {
                    Sprite maskSprite = new Sprite();

                    maskSprite.Texture = maskTex;
                    maskSprite.Width = 351;
                    maskSprite.Height = Controller.TOTAL_HEIGHT;
                    maskSprite.Color = Color.White;

                    maskSprite.Active = true;
                    maskSprite.LayerDepth = 1f;
                    maskSprite.Speed = 12.5f/* + (i * 5f)*/;
                    maskSprite.ScaleFactor = 0.999f + RandomHandler.GetRandomFloat(0.001f);

                    maskSprite.Move(Controller.TOTAL_WIDTH - maskSprite.Width + 12, 0);

                    maskSprites.Add(maskSprite);
                }
            }
        }

        private void createFog()
        {
            for (int i = 0; i < 3; i++) {
                fog = new Sprite();

                fog.Texture = fogTextures[0];
                fog.Width = fog.Texture.Width;
                fog.Height = Controller.TOTAL_HEIGHT;
                fog.Color = Color.White;

                fog.Active = true;

                float layerDepth = 0f;
                switch (i)
                {
                    case 0: layerDepth = 0.059f; break;
                    case 1: layerDepth = 0.599f; break;
                    case 2: layerDepth = 0.999f; break;
                }

                fog.LayerDepth = layerDepth;
                fog.Speed = 0f;
                fog.ScaleFactor = 1f;

                fog.Move(0, 0);

                bgSprites.Add(fog);
            }
        }

        private Sprite placeOnRandomLayer(Sprite bgSprite)
        {
            // Layer 1 is the foremost, layer 3 is the furthest away
            int randomLayer = RandomHandler.GetRandomInt(1, 3);

            bgSprite.Speed = 4 - randomLayer;

            float yPos = yCreationPos[3 - randomLayer],
                  layerDepth = 1f;

            bgSprite.Move(xCreationPos, yPos);

            switch (randomLayer)
            {
                case 1: layerDepth = 0.9f; // layer 1 is foremost (front)
                        break;
                case 2: layerDepth = 0.5f;  // layer 2 is in the middle
                        break;
                case 3: layerDepth = 0.1f; // layer 3 is furthest away (back)
                        break;
            }
            bgSprite.LayerDepth = layerDepth + RandomHandler.GetRandomFloat(0.1f);
            bgSprite.ScaleFactor = scaleValues[randomLayer-1];

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

            bgSprites.Add(bgObject);
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
