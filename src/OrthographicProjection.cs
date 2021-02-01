using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace com.pmg.MapMaker.src
{
    [Serializable]
    public class OrthographicProjection : ProjectionSettings
    {
        [XmlAttribute]
        public double CenterLatitude { get; set; } = 0.0;

        [XmlAttribute]
        public double CenterLongitude { get; set; } = 0.0;
    }
}
