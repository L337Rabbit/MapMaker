using System;
using System.Collections.Generic;
using System.Text;

namespace com.pmg.MapMaker
{
    public class PointSet
    {
        private List<Point> points = new List<Point>();

        public List<Point> Points
        {
            get { return this.points; }
            set { this.points = value; }
        }
    }
}
