using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace com.pmg.MapMaker
{
    [Serializable]
    public class MolleweideProjection : ProjectionSettings
    {
        private double centralMeridianLon;

        public MolleweideProjection()
        {
            this.projectionType = ProjectionType.MOLLWEIDE;
        }

        [XmlAttribute]
        public double CentralMeridianLongitude
        {
            get { return this.centralMeridianLon; }
            set { this.centralMeridianLon = value; }
        }
    }
}
