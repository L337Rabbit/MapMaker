using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Drawing;
using com.pmg.MapMaker.src;

namespace com.pmg.MapMaker
{
    [XmlInclude(typeof(LineLayer))]
    [XmlInclude(typeof(PolygonLayer))]
    [XmlInclude(typeof(MeridianLayer))]
    [XmlInclude(typeof(ParallelLayer))]
    [XmlType]
    public abstract class Layer
    {
        [XmlAttribute]
        public string FilePath { get; set; }

        [XmlAttribute]
        public bool IsEnabled { get; set; } = true;
    }
}
