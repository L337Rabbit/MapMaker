using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace com.pmg.MapMaker
{
    [Serializable]
    public class AzimuthalProjection : ProjectionSettings
    {
        private double outerEdgeLatitude = 0.0;

        public AzimuthalProjection()
        {
            this.projectionType = ProjectionType.AZIMUTHAL;
        }

        [XmlAttribute]
        public double OuterEdgeLatitude
        {
            get { return this.outerEdgeLatitude; }
            set { this.outerEdgeLatitude = value; }
        }
    }
}
