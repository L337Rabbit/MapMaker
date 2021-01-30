using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace com.pmg.MapMaker
{
    public class ColorUtils
    {
        public static string ToHex(Color color)
        {
            return color.ToArgb().ToString("X");
        }

        public static Color ToColor(string hex)
        {
            return Color.FromArgb(Convert.ToInt32(hex, 16));
        }
    }
}
