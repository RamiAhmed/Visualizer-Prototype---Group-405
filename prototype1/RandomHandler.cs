using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace prototype1
{
    public static class RandomHandler
    {
        private static Random random;

        public static void init()
        {
            random = new Random();
        }

        public static int GetRandomInt(int min, int max)
        {
            if (max+1 > min)
            {
                return random.Next(min, max+1);
            }

            return 0;
        }

        public static int GetRandomInt(int max)
        {
            return GetRandomInt(0, max);
        }

        public static float GetRandomFloat(float min, float max) {
            if (max > min) {
                return min + (float)random.NextDouble() * (max - min);
            }

            return 0;
        }

        public static float GetRandomFloat(float max)
        {
            return GetRandomFloat(0, max);
        }
    }
}
