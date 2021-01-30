using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace com.pmg.MapMaker
{
    [Serializable]
    public class EquirectangularProjection : ProjectionSettings
    {
        private double centralMeridianLon = 0.0;
        private double centralParallelLat = 0.0;
        private double standardParallelLat = Projector.TROPIC_OF_CANCER_LATITUDE;

        public EquirectangularProjection()
        {
            this.projectionType = ProjectionType.EQUIRECTANGULAR;
        }

        [XmlAttribute]
        public double CentralMeridianLongitude
        {
            get { return this.centralMeridianLon; }
            set { this.centralMeridianLon = value; }
        }

        [XmlAttribute]
        public double CentralParallelLatitude
        {
            get { return this.centralParallelLat; }
            set { this.centralParallelLat = value; }
        }

        [XmlAttribute]
        public double StandardParallelLatitude
        {
            get { return this.standardParallelLat; }
            set { this.standardParallelLat = value; }
        }
    }
}
