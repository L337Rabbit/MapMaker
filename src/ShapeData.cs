using System;
using System.Collections.Generic;
using System.Text;

namespace com.pmg.MapMaker
{
    public class ShapeData
    {
        private ShapeType shapeType;
        private BoundingBox bbox;
        public List<Shape> shapes = new List<Shape>();

        public ShapeData() { }

        public ShapeData(ShapeType shapeType)
        {
            this.shapeType = shapeType;
        }

        public ShapeType ShapeType
        {
            get { return this.shapeType; }
            set { this.shapeType = value; }
        }

        public BoundingBox BoundingBox
        {
            get { return this.bbox; }
            set { this.bbox = value; }
        }

        public List<Shape> Shapes
        {
            get { return this.shapes; }
            set { this.shapes = value; }
        }

        public static bool IsPolygonType(ShapeType shapeType)
        {
            if(shapeType == ShapeType.POLYGON || shapeType == ShapeType.POLYGONM || shapeType == ShapeType.POLYGONZ)
            {
                return true;
            }

            return false;
        }

        public static bool IsPolylineType(ShapeType shapeType)
        {
            if (shapeType == ShapeType.POLYLINE || shapeType == ShapeType.POLYLINEM || shapeType == ShapeType.POLYLINEZ)
            {
                return true;
            }

            return false;
        }

        public static bool IsPointType(ShapeType shapeType)
        {
            if (shapeType == ShapeType.POINT || shapeType == ShapeType.POINTM || shapeType == ShapeType.POINTZ)
            {
                return true;
            }

            return false;
        }
    }
}
