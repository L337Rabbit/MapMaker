using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Drawing;

namespace com.pmg.MapMaker.src
{
    [Serializable]
    public class PolygonLayer : Layer
    {
        [XmlAttribute]
        public bool DrawEdges { get; set; } = false;

        [XmlIgnore]
        public Color EdgeColor { get; set; }

        [XmlAttribute]
        public bool FillPolygons { get; set; } = false;

        [XmlIgnore]
        public Color FillColor { get; set; }

        [XmlAttribute]
        public string EdgeColorValue
        {
            get { return ColorUtils.ToHex(EdgeColor); }
            set 
            { 
                this.EdgeColor = ColorUtils.ToColor(value);
                DrawEdges = true;
            }
        }

        [XmlAttribute]
        public string FillColorValue
        {
            get { return ColorUtils.ToHex(FillColor); }
            set 
            { 
                this.FillColor = ColorUtils.ToColor(value);
                FillPolygons = true;
            }
        }
    }
}
