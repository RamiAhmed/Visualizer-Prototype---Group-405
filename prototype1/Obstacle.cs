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
    public enum ObstacleType { HILL, HOLE, SLIDE, WALL, NULL };

    class Obstacle : Sprite
    {
        private ObstacleType currentType;
        private Rectangle boundingBox;
        private bool animateOnDeath, readyToAnimate, stayAtFrame;
        private float animationSpeed;

        public Obstacle()
        {
        }

        public ObstacleType Type
        {
            get { return currentType; }
            set { currentType = value; }
        }

        public Rectangle BoundingBox
        {
            get { return boundingBox; }
            set { boundingBox = value; }
        }

        public bool AnimateOnDeath
        {
            get { return animateOnDeath; }
            set { animateOnDeath = value; }
        }

        public bool ReadyToAnimate
        {
            get { return readyToAnimate; }
            set { readyToAnimate = value; }
        }

        public bool StayAtFrame
        {
            get { return stayAtFrame; }
            set { stayAtFrame = value; }
        }

        public float AnimationSpeed
        {
            get { return animationSpeed; }
            set { animationSpeed = value; }
        }
    }
}
