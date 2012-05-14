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

        private static float smoothColor = 0.5f;

        private static bool debug = true;

        public static void loadColors()
        {
            // 12 colors in total
            /*
            acceptedColors.Add(new Color(189, 225, 0));     // Light yellow-green
            acceptedColors.Add(new Color(26, 144, 27));     // Darker green
            acceptedColors.Add(new Color(29, 144, 128));    // Dark cyan-blue
            acceptedColors.Add(new Color(25, 8, 136));      // Blue
            acceptedColors.Add(new Color(126, 6, 129));     // Purple
            acceptedColors.Add(new Color(214, 22, 138));    // Pink
            acceptedColors.Add(new Color(111, 14, 71));     // Dark purple-red
            acceptedColors.Add(new Color(160, 16, 0));      // Dark red
            acceptedColors.Add(new Color(250, 21, 0));      // Red
            acceptedColors.Add(new Color(248, 129, 0));     // Orange-salmon
            acceptedColors.Add(new Color(238, 241, 117));   // Sand
            acceptedColors.Add(new Color(246, 245, 0));     // Yellow
            */
            
            acceptedColors.Add(new Color(255, 11, 12));     // Red
            acceptedColors.Add(new Color(244, 71, 18));     // Orange-red
            acceptedColors.Add(new Color(248, 128, 16));    // Orange
            acceptedColors.Add(new Color(246, 209, 17));    // Orange-yellow
            acceptedColors.Add(new Color(245, 244, 60));    // Yellow
            acceptedColors.Add(new Color(188, 224, 57));    // Yellow-green
            acceptedColors.Add(new Color(20, 144, 51));     // Green
            acceptedColors.Add(new Color(27, 144, 129));    // Cyan
            acceptedColors.Add(new Color(28, 13, 130));     // Blue
            acceptedColors.Add(new Color(166, 21, 134));    // Purple
            acceptedColors.Add(new Color(215, 19, 134));    // Pink
            acceptedColors.Add(new Color(173, 14, 72));     // Pink-red             
        }

        public static Color smoothToGray(Color currentColor)
        {
            /*float incremental = 0.001f;
            float gray = 0.5f;

            float red = currentColor.R / 255,
                green = currentColor.G / 255,
                blue = currentColor.B / 255;

            if (debug)
            {
                Console.WriteLine("red: " + red.ToString() + ", green: " + green.ToString() + ", blue: " + blue.ToString());
            }

            if (red > gray)
                red -= incremental;
            else if (red < gray)
                red += incremental;

            if (green > gray)
                green -= incremental;
            else if (green < gray)
                green += incremental;

            if (blue > gray)
                blue -= incremental;
            else if (blue < gray)
                blue += incremental;

            return new Color(red, green, blue); // probably thinks it's ints*/
            return currentColor;
        }

        public static Color getSmoothFogColor()
        {
            float incremental = 0.001f;
            float amplitude = OSCHandler.inAmplitude;
            if (amplitude > 0)
            {
                smoothColor += incremental;
            }
            else
            {
                smoothColor -= (incremental * 0.5f);
            }

            float min = 0.2f, max = 0.9f;
            if (smoothColor < min)
            {
                smoothColor = min;
            }
            else if (smoothColor > max)
            {
                smoothColor = max;
            }

            return new Color(smoothColor, smoothColor, smoothColor);
        }

        public static Color getCurrentColor()
        {
            int currentColor = 0;
            float midiPitch = OSCHandler.inPitch;

            ////currentColor = (int)Math.Round((midiPitch - 50f) / 11);
            //Console.WriteLine("Current color: " + currentColor.ToString());
            
            if (midiPitch <= 48f)
            {
                currentColor = 0;
            }
            else if (midiPitch <= 50f)
            {
                currentColor = 1;
            }
            else if (midiPitch <= 52f)
            {
                currentColor = 2;
            }
            else if (midiPitch <= 54f)
            {
                currentColor = 3;
            }
            else if (midiPitch <= 56f)
            {
                currentColor = 4;
            }
            else if (midiPitch <= 58f)
            {
                currentColor = 5;
            }
            else if (midiPitch <= 60f)
            {
                currentColor = 6;
            }
            else if (midiPitch <= 62f)
            {
                currentColor = 7;
            }
            else if (midiPitch <= 64f)
            {
                currentColor = 8;
            }
            else if (midiPitch <= 66f)
            {
                currentColor = 9;
            }
            else if (midiPitch <= 68f)
            {
                currentColor = 10;
            }
            else
            {
                currentColor = 11;
            }

            return acceptedColors.ElementAt(currentColor);
        }

        public static Color getRandomColor()
        {
            return acceptedColors.ElementAt(RandomHandler.GetRandomInt(acceptedColors.Count-1));
        }
    }
}
