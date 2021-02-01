using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace com.pmg.MapMaker
{
    [XmlType]
    public enum ProjectionType
    {
        AZIMUTHAL, EQUIRECTANGULAR, MERCATOR, MOLLWEIDE, WAGNERVI, NATURAL_EARTH, ECKERTIV, ORTHOGRAPHIC
    }
}
