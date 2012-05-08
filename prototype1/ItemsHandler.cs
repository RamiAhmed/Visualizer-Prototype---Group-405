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
    class ItemsHandler
    {
        public List<Texture2D> itemTextures = new List<Texture2D>();
        public List<Sprite> itemSprites = new List<Sprite>();

        private int deathPointX = 350;
        private int lastCreation = 0;
        private int creationFrequency = 30; // every nth second
        private int numFrames = 4;
        private float itemAnimSpeed = 5f;

        public ItemsHandler()
        {
        }

        public void updateItems(GameTime time)
        {
            int currentSeconds = (int)time.TotalGameTime.TotalSeconds;
            if (currentSeconds - lastCreation > creationFrequency)
            {
                lastCreation = currentSeconds;

                createItem();
            }

            int itemsCount = itemSprites.Count;
            for (int i = 0; i < itemsCount; i++)
            {
                Sprite item = itemSprites.ElementAt(i);
                if (item.Active)
                {
                    item.Move(item.Position.X - item.Speed, item.Position.Y + RandomHandler.GetRandomFloat(-1.5f, 1.5f));
                    if (item.Position.X - (item.Width * 0.75f) < deathPointX)
                    {
                        item.Active = false;
                    }
                }
                else
                {
                    itemSprites.RemoveAt(i);

                    i--;
                    itemsCount--;
                }
            }
        }

        public void drawItems(SpriteBatch batch, GameTime time)
        {
            foreach (Sprite item in itemSprites)
            {
                if (item.Active)
                {
                    int animationX = (int)(time.TotalGameTime.TotalSeconds * itemAnimSpeed) % numFrames;
                    Rectangle animCycle = new Rectangle(animationX * item.Width, 0, item.Width, item.Height);

                    batch.Draw(item.Texture, item.Position, animCycle, item.Color, MathHelper.ToRadians(item.Rotation),
                            new Vector2(0, 0), item.ScaleFactor, SpriteEffects.None, item.LayerDepth);
                }

            }
        }

        private void createItem()
        {
            Sprite item = new Sprite();

            item.Texture = getRandomItemTexture();

            item.Width = item.Texture.Width / numFrames;
            item.Height = item.Texture.Height;
            item.Color = Color.White;

            item.Speed = 1.5f;
            item.LayerDepth = 0.01f;
            item.ScaleFactor = 1f + RandomHandler.GetRandomFloat(-0.25f, 0.25f);
            item.Rotation = RandomHandler.GetRandomFloat(-90, 90);

            item.Active = true;

            item.Move(Controller.TOTAL_WIDTH + item.Width, 400 + RandomHandler.GetRandomFloat(-25, 25));

            itemSprites.Add(item);
        }

        private Texture2D getRandomItemTexture()
        {
            return itemTextures.ElementAt(RandomHandler.GetRandomInt(itemTextures.Count - 1));
        }


    }
}
