using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace com.pmg.MapMaker
{
    public class PolyLine : Shape
    {
        private List<PointSet> parts = new List<PointSet>();

        public static PolyLine Read(BinaryReader br)
        {
            int shapeType = br.ReadInt32();

            return ReadPolyline(br);
        }

        private static PolyLine ReadPolyline(BinaryReader br)
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

            //Setup variables for processing the polyline's parts.
            int nextPartStartPointIdx = 0;
            if (numParts > 1) { nextPartStartPointIdx = partPointIndices[1]; }
            int partIdx = 1;
            int pointIdx = 0;            

            //Create a new PolyLine.
            PolyLine polyLine = new PolyLine();

            //Create a variable to store the current part being processed.
            PointSet currentPart = new PointSet();

            //Read in and process each part in the polyline.
            while (pointIdx < numPoints)
            {
                //Read the values of the next point in the current polyline and add the point to the current part.
                Point p = Point.ReadPointValues(br);
                currentPart.Points.Add(p);
                pointIdx++;

                //If we are at the point index for the next part's starting point, or we have processed all points, add the part
                //which is currently being processed to the PolyLine.
                if (pointIdx == nextPartStartPointIdx || pointIdx == numPoints)
                {
                    polyLine.Parts.Add(currentPart);

                    //If we are not at the last point yet, start processing a new part.
                    if (pointIdx < numPoints) 
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
            polyLine.SizeAsRead = (52 + (numParts * 4) + (numPoints * 16));

            //Set the bounding box on the PolyLine.
            polyLine.bbox = new BoundingBox(minX, minY, maxX, maxY);

            return polyLine;
        }

        public List<PointSet> Parts
        {
            get { return this.parts; }
            set { this.parts = value; }
        }
    }
}
