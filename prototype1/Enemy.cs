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
    class Enemy : Sprite
    {
        public List<Enemy> enemySprites = new List<Enemy>();
        public List<Texture2D> enemyTextures = new List<Texture2D>();

        // After this X coordinate, all enemies will automatically die (fail-safe)
        public int deathPointX = 550;

        // The enemies' starting location
        private Vector2 enemyStartPosition = new Vector2(-5, 425);

        // Animation speed factor (speed of animation; the higher the quicker)
        private float enemyAnimationSpeedFactor = 15f;
        private long lastEnemyCreation = 0;
        // Every nth millisecond an enemy (might) be created
        private float enemyCreationSpeed = 1f;

        // Time this class waits before starting to make enemies
        private float enemyStartWait = 5f;

        public Enemy()
        {
        }

        public void updateEnemy(GameTime gameTime)
        {
            long currentMilliseconds = (long)gameTime.TotalGameTime.TotalMilliseconds;
            if (currentMilliseconds > enemyStartWait * 1000f)
            {
                if (currentMilliseconds - lastEnemyCreation > enemyCreationSpeed * 1000f)
                {
                    lastEnemyCreation = currentMilliseconds;

                    float noise = OSCHandler.inNoise,
                          amp = OSCHandler.inPeakAmplitude;
                    if (amp > 0.75f && noise > 0.75f)
                    {
                        createEnemy();
                    }
                }

                int enemySpritesCount = enemySprites.Count;
                if (enemySpritesCount > 0)
                {
                    for (int i = 0; i < enemySpritesCount; i++)
                    {
                        Enemy enemy = enemySprites.ElementAt(i);
                        if (enemy.Active)
                        {
                            enemy.Move(enemy.Position.X + enemy.Speed, enemy.Position.Y + RandomHandler.GetRandomFloat(-1, 1));
                        }
                        else
                        {
                            enemySprites.RemoveAt(i);

                            i--;
                            enemySpritesCount--;
                        }
                    }
                }
            }
        }

        public void drawEnemy(SpriteBatch batch, GameTime gameTime)
        {
            if (enemySprites.Count > 0)
            {
                foreach (Enemy enemy in enemySprites)
                {
                    int animationX = (int)(gameTime.TotalGameTime.TotalSeconds * enemyAnimationSpeedFactor) % 8;

                    Rectangle enemyCycle = new Rectangle(animationX * enemy.Width, 0,
                                                            enemy.Width, enemy.Height);

                    batch.Draw(enemy.Texture, enemy.Position, enemyCycle, enemy.Color);
                }
            }
        }

        public void removeEnemy(Enemy enemy)
        {
            if (enemy != null)
            {
                enemy.Active = false;
            }
        }

        private void createEnemy()
        {
            if (enemyTextures.Count > 0)
            {
                Enemy enemy = new Enemy();

                enemy.Texture = getRandomEnemyTexture();
                enemy.Width = 100;
                enemy.Height = 100;

                enemy.Speed = 0.5f;
                enemy.LayerDepth = 0f;
                enemy.ScaleFactor = 1f;
                enemy.Color = ColorHandler.getCurrentColor();

                enemy.Active = true;

                enemy.Move(enemyStartPosition.X + RandomHandler.GetRandomInt(-25, 25), enemyStartPosition.Y + RandomHandler.GetRandomInt(-10, 10));

                enemySprites.Add(enemy);
            }
        }        

        private Texture2D getRandomEnemyTexture()
        {
            Texture2D enemyTex = null;

            if (enemyTextures.Count > 0)
            {
                foreach (Texture2D tex in enemyTextures)
                {
                    if (RandomHandler.GetRandomFloat(1) < 0.01f)
                    {
                        enemyTex = tex;
                    }
                }

                if (enemyTex != null)
                {
                    return enemyTex;
                }
                else
                {
                    return getRandomEnemyTexture();
                }
            }
            else
            {
                return null;
            }
        }
    }
}
