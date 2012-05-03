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
    class ColorHandler
    {
        private List<Color> acceptedColors = new List<Color>();

        public ColorHandler()
        {
            init();
        }

        private void init()
        {
        }

        private void loadColors()
        {
            //Color color = w
        }

        public static Color getCurrentColor()
        {
            return getRandomColor();
        }

        public static Color getRandomColor()
        {
            return new Color(RandomHandler.GetRandomFloat(1),
                             RandomHandler.GetRandomFloat(1),
                             RandomHandler.GetRandomFloat(1));
        }
    }
}
