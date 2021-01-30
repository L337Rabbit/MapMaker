using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace com.pmg.MapMaker
{
    public class Polygon : Shape
    {
        private List<PointSet> parts = new List<PointSet>();

        public static Polygon Read(BinaryReader br)
        {
            int shapeType = br.ReadInt32();

            return ReadPolygon(br);
        }

        private static Polygon ReadPolygon(BinaryReader br)
        {
            //Read bounding box values.
            double minX = br.ReadDouble();
            double minY = br.ReadDouble();
            double maxX = br.ReadDouble();
            double maxY = br.ReadDouble();

            //Read number of parts and number of points values.
            int numParts = br.ReadInt32();
            int numPoints = br.ReadInt32();

            //Create an array to hold the starting point indices for each part.
            int[] partPointIndices = new int[numParts];

            //Read part starting point indices in.
            for(int i = 0; i < numParts; i++)
            {
                partPointIndices[i] = br.ReadInt32();
            }

            //Setup variables for processing the polygon's parts.
            int nextPartStartPointIdx = 0;
            if(numParts > 1) { nextPartStartPointIdx = partPointIndices[1]; }
            int partIdx = 1;
            int pointIdx = 0;

            //Create a new Polygon.
            Polygon polygon = new Polygon();

            //Create a variable to store the current part being processed.
            PointSet currentPart = new PointSet();

            int pointCount = 0;

            //Read in and process each part in the polygon.
            while(pointIdx < numPoints)
            {
                //Read the values of the next point in the current polygon and add the point to the current part.
                Point p = Point.ReadPointValues(br);
                currentPart.Points.Add(p);
                pointIdx++;
                pointCount++;

                //If we are at the point index for the next part's starting point, or we have processed all points, add the part
                //which is currently being processed to the Polygon.
                if(pointIdx == nextPartStartPointIdx || pointIdx == numPoints)
                {
                    polygon.Parts.Add(currentPart);

                    //If we are not at the last point yet, start processing a new part.
                    if(pointIdx < numPoints)
                    {
                        currentPart = new PointSet();

                        if (partIdx < numParts - 1)
                        {
                            partIdx++;
                            nextPartStartPointIdx = partPointIndices[partIdx];
                        }
                    }
                }
            }

            //Set the size in bytes value (as read from the shapefile).
            //Total size = 44 bytes + (numParts * 4) + (numPoints * 16)
            polygon.SizeAsRead = (52 + (numParts * 4) + (numPoints * 16));

            //Set the bounding box on the Polygon.
            polygon.bbox = new BoundingBox(minX, minY, maxX, maxY);

            return polygon;
        }

        public override string ToString()
        {
            string str = "";

            foreach (PointSet part in parts)
            {
                str += "[";

                foreach (Point p in part.Points)
                {
                    str += p.ToString() + " ";
                }

                str += "]";
            }

            return str;
        }

        private BoundingBox FindBBox()
        {
            BoundingBox bbox = new BoundingBox();
            bbox.MinX = Double.MaxValue;
            bbox.MaxX = Double.MinValue;
            bbox.MinY = Double.MaxValue;
            bbox.MaxY = Double.MinValue;

            foreach(PointSet part in parts)
            {
                foreach(Point p in part.Points)
                {
                    if(p.x < bbox.MinX)
                    {
                        bbox.MinX = p.x;
                    }

                    if(p.x > bbox.MaxX)
                    {
                        bbox.MaxX = p.x;
                    }

                    if(p.y < bbox.MinY)
                    {
                        bbox.MinY = p.y;
                    }

                    if(p.y > bbox.MaxY)
                    {
                        bbox.MaxY = p.y;
                    }
                }
            }

            return bbox;
        }

        public List<PointSet> Parts
        {
            get { return this.parts; }
            set { this.parts = value; }
        }

        public void ReversePointOrder()
        {
            for(int i = 0; i < parts.Count; i++)
            {
                PointSet part = parts[i];
                part.Points.Reverse();
            }
        }
    }
}
