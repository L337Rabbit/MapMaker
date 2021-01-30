using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace com.pmg.MapMaker
{
    public class Shape
    {
        protected int sizeAsRead = 0;
        protected BoundingBox bbox = new BoundingBox();

        protected static void ReadHeader(BinaryReader br)
        {
            //Read header -----------------------------
            //Read the number of the shape entry.
            int recordNum = br.ReadInt32();

            //Read the length of the shape entry.
            int recordLength = br.ReadInt32();
            //-----------------------------------------
        }

        public static Shape Read(ShapeType shapeType, BinaryReader br)
        {
            ReadHeader(br);

            if (shapeType == ShapeType.POINT || shapeType == ShapeType.POINTZ || shapeType == ShapeType.POINTM)
            {
                return Point.Read(br);
            }
            else if (shapeType == ShapeType.POLYLINE)
            {
                return PolyLine.Read(br);
            }
            else if (shapeType == ShapeType.POLYGON)
            {
                return Polygon.Read(br);
            }
            else
            {
                Console.WriteLine("Shape type \"" + shapeType + "\" is not currently supported.");
            }

            return null;
        }

        public int SizeAsRead
        {
            get { return this.sizeAsRead; }
            protected set { this.sizeAsRead = value; }
        }

        public BoundingBox BoundingBox
        {
            get { return this.bbox; }
            set { this.bbox = value; }
        }
    }
}
