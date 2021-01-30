using System;
using System.Collections.Generic;
using System.Text;

namespace com.pmg.MapMaker
{
    public class BoundingBox
    {
        private double minX;
        private double minY;
        private double maxX;
        private double maxY;

        public BoundingBox() { }

        public BoundingBox(double minX, double minY, double maxX, double maxY)
        {
            this.minX = minX;
            this.minY = minY;
            this.maxX = maxX;
            this.maxY = maxY;
        }

        public double MinX
        {
            get { return this.minX; }
            set { this.minX = value; }
        }

        public double MinY
        {
            get { return this.minY; }
            set { this.minY = value; }
        }

        public double MaxX
        {
            get { return this.maxX; }
            set { this.maxX = value; }
        }

        public double MaxY
        {
            get { return this.maxY; }
            set { this.maxY = value; }
        }

        public override string ToString()
        {
            return "BBOX([" + minX + ", " + minY + "] => [" + maxX + ", " + maxY + "]";
        }
    }
}
