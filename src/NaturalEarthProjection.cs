using System;
using System.Collections.Generic;
using System.Text;

namespace com.pmg.MapMaker.src
{
    [Serializable]
    public class NaturalEarthProjection : ProjectionSettings
    {
        /// <summary>
        /// Lookup table for parallel lengths and distance from parallel to equator for given latitude (in 5 degree intervals).
        /// </summary>
        public static Dictionary<int, (double, double)> Lookup = new Dictionary<int, (double, double)>()
        {
            { 0, (1.0, 0.0) },
            { 5, (0.9988, 0.0620) },
            { 10, (0.9953, 0.1240) },
            { 15, (0.9894, 0.1860) },
            { 20, (0.9811, 0.2480) },
            { 25, (0.9703, 0.3100) },
            { 30, (0.9570, 0.3720) },
            { 35, (0.9409, 0.4340) },
            { 40, (0.9222, 0.4958) },
            { 45, (0.9006, 0.5571) },
            { 50, (0.8763, 0.6176) },
            { 55, (0.8492, 0.6769) },
            { 60, (0.8196, 0.7346) },
            { 65, (0.7874, 0.7903) },
            { 70, (0.7525, 0.8435) },
            { 75, (0.7160, 0.8936) },
            { 80, (0.6754, 0.9394) },
            { 85, (0.6270, 0.9761) },
            { 90, (0.5630, 1.0) },
        };


    }
}
