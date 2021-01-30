using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace com.pmg.MapMaker
{
    [Serializable]
    public class MercatorProjection : ProjectionSettings
    {
        private double latitudeClip = 85.051129;
        private double centralMeridianLon = 0.0;

        public MercatorProjection()
        {
            this.projectionType = ProjectionType.MERCATOR;
        }

        [XmlAttribute]
        public double LatitudeClip
        {
            get { return this.latitudeClip; }
            set { this.latitudeClip = value; }
        }

        [XmlAttribute]
        public double CentralMeridianLongitude
        {
            get { return this.centralMeridianLon; }
            set { this.centralMeridianLon = value; }
        }
    }
}
