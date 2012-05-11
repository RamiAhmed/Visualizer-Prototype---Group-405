using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace prototype1
{
    class BackgroundObject : Sprite
    {
        private float _scaleToTheBeat = -1f;
        private bool _isFog = false;

        public BackgroundObject()
        {
        }

        public bool IsFog
        {
            get { return _isFog; }
            set { _isFog = value; }
        }

        public float ScaleToTheBeat
        {
            get { return _scaleToTheBeat; }
            set { _scaleToTheBeat = value; }
        }
    }
}
