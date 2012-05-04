﻿using System;
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
    }
}
