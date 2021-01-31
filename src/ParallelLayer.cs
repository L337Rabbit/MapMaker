using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace com.pmg.MapMaker.src
{
    [Serializable]
    public class ParallelLayer : Layer
    {
        [XmlIgnore]
        public Color Color { get; set; }

        [XmlAttribute]
        public string ColorValue
        {
            get { return ColorUtils.ToHex(Color); }
            set { this.Color = ColorUtils.ToColor(value); }
        }

        [XmlAttribute]
        public double MinimumLatitude { get; set; } = -90.0;

        [XmlAttribute]
        public double MaximumLatitude { get; set; } = 90.0;

        [XmlAttribute]
        public double MinimumLongitude { get; set; } = -180.0;

        [XmlAttribute]
        public double MaximumLongitude { get; set; } = 180.0;

        [XmlAttribute]
        public double LatitudeOffset { get; set; } = 0.0;

        [XmlAttribute]
        public double LatitudeInterval { get; set; } = 10.0;

        [XmlAttribute]
        public double LongitudeInterval { get; set; } = 5.0;
    }
}
