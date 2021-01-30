using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Drawing;

namespace com.pmg.MapMaker
{
    [XmlRoot]
    public class RenderConfig
    {
        private List<Layer> layers = new List<Layer>();

        [XmlAttribute]
        public int ImageWidth { get; set; }

        [XmlAttribute]
        public int ImageHeight { get; set; }

        [XmlElement]
        public ProjectionSettings ProjectionSettings { get; set; }

        [XmlIgnore]
        public Color ClearColor { get; set; }

        [XmlAttribute]
        public string ClearColorValue
        {
            get { return ColorUtils.ToHex(ClearColor); }
            set { this.ClearColor = ColorUtils.ToColor(value); }
        }

        [XmlArray]
        public List<Layer> Layers
        {
            get { return this.layers; }
            set { this.layers = value; }
        }
    }
}
