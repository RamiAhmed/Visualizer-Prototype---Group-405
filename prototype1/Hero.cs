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
    class Hero : Sprite
    {
        private Vector2 heroStartPosition = new Vector2(300, 400);
        private Texture2D heroTexture;
        private HeroState currentState;

        private long lastFrame = 0;
        private int currentFrame = 0;
        private int walkcycleSpeed = 75; // per nth millisecond
        private int amountOfFramesInWalkcycle = 8;

        public enum HeroState { WALKING, JUMPING, SLIDING };

        public Hero(Texture2D texture)
        {
            if (heroTexture == null)
            {
                heroTexture = texture;
            }

            init();
        }

        private void init()
        {
            this.Width = 100;
            this.Height = 100;
            this.Texture = heroTexture;
            this.Active = true;
            this.CurrentState = HeroState.WALKING;
        }

        public void updateHero(GameTime gameTime)
        {
            this.Color = new Color(RandomHandler.GetRandomFloat(1), RandomHandler.GetRandomFloat(1), RandomHandler.GetRandomFloat(1));
        }

        private int getCurrentFrame(GameTime gameTime)
        {
            long currentMilliseconds = (long)gameTime.TotalGameTime.TotalMilliseconds;
            if (currentMilliseconds - lastFrame > walkcycleSpeed)
            {
                lastFrame = currentMilliseconds;

                if (currentFrame < amountOfFramesInWalkcycle - 1)
                {
                    currentFrame++;
                }
                else
                {
                    currentFrame = 0;
                }
            }
            return currentFrame;
        }

        public void drawHero(SpriteBatch batch, GameTime gameTime)
        {
            if (this.CurrentState == HeroState.WALKING)
            {
                if (this.Active)
                {
                    Rectangle walkcycle = new Rectangle(getCurrentFrame(gameTime) * 100, 0, this.Width, this.Height);

                    Rectangle heroRect = new Rectangle((int)heroStartPosition.X, (int)heroStartPosition.Y,
                                                        this.Width, this.Height);
                    batch.Draw(this.Texture, heroRect, walkcycle, this.Color);
                }
            }
        }

        public HeroState CurrentState
        {
            get { return currentState; }
            set { currentState = value; }
        }
    }
}
