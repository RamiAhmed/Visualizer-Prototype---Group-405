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
    public static class ColorHandler
    {
        private static List<Color> acceptedColors = new List<Color>();

        public static void loadColors()
        {
            acceptedColors.Add(new Color(189, 225, 0));
            acceptedColors.Add(new Color(26, 144, 27));
            acceptedColors.Add(new Color(29, 144, 128));
            acceptedColors.Add(new Color(25, 8, 136));
            acceptedColors.Add(new Color(126, 6, 129));
            acceptedColors.Add(new Color(214, 22, 138));
            acceptedColors.Add(new Color(111, 14, 71));
            acceptedColors.Add(new Color(160, 16, 0));
            acceptedColors.Add(new Color(250, 21, 0));
            acceptedColors.Add(new Color(248, 129, 0));
            acceptedColors.Add(new Color(238, 241, 117));
            acceptedColors.Add(new Color(246, 245, 0));
        }

        public static Color getCurrentColor()
        {
            return getRandomColor();
        }

        public static Color getRandomColor()
        {
            return acceptedColors.ElementAt(RandomHandler.GetRandomInt(acceptedColors.Count-1));
        }
    }
}
