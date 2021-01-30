using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace com.pmg.MapMaker
{
    [XmlInclude(typeof(AzimuthalProjection))]
    [XmlInclude(typeof(EquirectangularProjection))]
    [XmlInclude(typeof(MercatorProjection))]
    [XmlInclude(typeof(MolleweideProjection))]
    public abstract class ProjectionSettings
    {
        protected ProjectionType projectionType;

        [XmlAttribute]
        public ProjectionType ProjectionType
        {
            get { return this.projectionType; }
            set { this.projectionType = value; }
        }
    }
}
