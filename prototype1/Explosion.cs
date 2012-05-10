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
    class Explosion : Sprite
    {
        public Texture2D explosionTexture;
        public List<Explosion> explosionSprites = new List<Explosion>();

        private int numFrames = 8;
        private long creationTime = 0;
        private float lifeTime = 0.4f; // how many seconds does the explosion live?
        private float animSpeed = 10f;

        private bool debug = false;

        public Explosion()
        {

        }

        public void createExplosion(Enemy enemy, GameTime time)
        {
            Explosion explosion = new Explosion();
            explosion.Texture = explosionTexture;

            explosion.Width = explosion.Texture.Width / numFrames;
            explosion.Height = explosion.Texture.Height;

            explosion.Move(enemy.Position.X + (enemy.Width * 0.5f), enemy.Position.Y + (enemy.Width * 0.5f));

            explosion.ScaleFactor = 4f + RandomHandler.GetRandomFloat(2);
            explosion.Rotation = RandomHandler.GetRandomFloat(360);
            explosion.Speed = animSpeed + RandomHandler.GetRandomFloat(2);
            explosion.LayerDepth = 0f;
            explosion.Color = Color.White;

            explosion.CreationTime = (long)time.TotalGameTime.TotalMilliseconds;
            explosion.Active = true;

            explosionSprites.Add(explosion);
            if (debug)
            {
                Console.WriteLine("Created new explosion at: " + enemy.Position.ToString());
            }
        }

        public void updateExplosions(GameTime time)
        {
            if (explosionSprites.Count > 0)
            {
                long currentMilliseconds = (long)time.TotalGameTime.TotalMilliseconds;
                int explosionsCount = explosionSprites.Count;
                for (int i = 0; i < explosionsCount; i++)
                {
                    Explosion explosion = explosionSprites.ElementAt(i);
                    if (explosion.Active)
                    {
                        if (currentMilliseconds - explosion.CreationTime > lifeTime * 1000f)
                        {
                            explosion.Active = false;
                        }
                    }
                    else
                    {
                        explosionSprites.RemoveAt(i);

                        i--;
                        explosionsCount--;
                    }
                }
            }
        }

        public void drawExplosions(SpriteBatch batch, GameTime time)
        {
            if (explosionSprites.Count > 0)
            {
                foreach (Explosion explosion in explosionSprites)
                {
                    if (explosion.Active)
                    {
                        int animationX = (int)(time.TotalGameTime.TotalSeconds * explosion.Speed) % numFrames;
                        Rectangle animCycle = new Rectangle(animationX * explosion.Width, 0, explosion.Width, explosion.Height);

                        batch.Draw(explosion.Texture, explosion.Position, animCycle, explosion.Color, explosion.Rotation,
                                    new Vector2(explosion.Width * 0.5f, explosion.Height * 0.5f), explosion.ScaleFactor, SpriteEffects.None, explosion.LayerDepth);
                    }
                }
            }
        }

        private long CreationTime
        {
            get { return creationTime; }
            set { creationTime = value; }
        }
    }
}
