using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Drawing;

namespace com.pmg.MapMaker
{
    [XmlType]
    public class Layer
    {
        [XmlAttribute]
        public string FilePath { get; set; }

        [XmlAttribute]
        public bool DrawEdges { get; set; }

        [XmlIgnore]
        public Color EdgeColor { get; set; }

        [XmlAttribute]
        public bool FillPolygons { get; set; }

        [XmlIgnore]
        public Color PolygonFillColor { get; set; }

        [XmlAttribute]
        public string EdgeColorValue
        {
            get { return ColorUtils.ToHex(EdgeColor); }
            set { this.EdgeColor = ColorUtils.ToColor(value); }
        }

        [XmlAttribute]
        public string PolygonFillColorValue
        {
            get { return ColorUtils.ToHex(PolygonFillColor); }
            set { this.PolygonFillColor = ColorUtils.ToColor(value); }
        }
    }
}
