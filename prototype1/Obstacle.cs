using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace prototype1
{
    public enum ObstacleType { HILL, HOLE, SLIDE, WALL, NULL };

    class Obstacle : Sprite
    {
        private ObstacleType currentType;        

        public Obstacle()
        {
        }

        public ObstacleType Type
        {
            get { return currentType; }
            set { currentType = value; }
        }
    }
}
