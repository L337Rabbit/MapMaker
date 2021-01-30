using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace com.pmg.MapMaker
{
    public class Point : Shape
    {
        public double x, y, z, m;
        private bool hasZ = false;
        private bool hasM = false;

        public Point(double x, double y, double m = 0.0, double z = 0.0)
        {
            this.x = x;
            this.y = y;
            this.m = m;
            this.z = z;
        }

        public static Point Read(BinaryReader br)
        {
            //Read values -----------------------------
            //Read shape type value
            ShapeType shapeType = (ShapeType)br.ReadInt32();

            switch(shapeType)
            {
                case ShapeType.POINT: return ReadPointValues(br);
                case ShapeType.POINTM: return ReadPointMValues(br);
                case ShapeType.POINTZ: return ReadPointZValues(br);
            }
            //-----------------------------------------

            return null;
        }

        public static Point ReadPointValues(BinaryReader br)
        {
            double x = br.ReadDouble();
            double y = br.ReadDouble();

            Point p = new Point(x, y);
            p.sizeAsRead = 28;

            return p;
        }

        private static Point ReadPointMValues(BinaryReader br)
        {
            double x = br.ReadDouble();
            double y = br.ReadDouble();
            double m = br.ReadDouble();
            Point p = new Point(x, y, m);
            p.sizeAsRead = 36;
            p.hasM = true;

            return p;
        }

        private static Point ReadPointZValues(BinaryReader br)
        {
            double x = br.ReadDouble();
            double y = br.ReadDouble();
            double z = br.ReadDouble();
            double m = br.ReadDouble();
            Point p = new Point(x, y, m, z);
            p.sizeAsRead = 44;
            p.hasZ = true;
            p.hasM = true;

            return p;
        }

        public override string ToString()
        {
            return "(" + x + ", " + y + ")";
        }

        public double X
        {
            get { return this.x; }
            set { this.x = value; }
        }

        public double Y
        {
            get { return this.y; }
            set { this.y = value; }
        }

        public double Z
        {
            get { return this.z; }
            set { this.z = value; }
        }

        public double M
        {
            get { return this.M; }
            set { this.M = value; }
        }

        public bool HasZ
        {
            get { return this.hasZ; }
            protected set { this.hasZ = value; }
        }

        public bool HasM
        {
            get { return this.hasM; }
            protected set { this.hasM = value; }
        }
    }
}
