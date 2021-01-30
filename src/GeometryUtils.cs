using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace com.pmg.MapMaker
{
    public class GeometryUtils
    {
        public static Polygon SplitAtLongitude(Polygon polygon, double longitude)
        {
            Polygon newPoly = new Polygon();

            foreach(PointSet part in polygon.Parts)
            {
                //Start building a new pointset.

                //Go through each pair of vertices and see if the line formed between them crosses the longitudinal line specified.
                //If a crossing is found, determine the intersection point with the meridian.
                //Stop adding points to the current set (left) and begin adding points to a new set (right).
                //When another intersection is found, stop adding points to the current set (right).
                //Continue this process, swawpping between sides and creating new sets until all points are processed. 

                //Any time an intersection occurs which lies between the other intersection points of a segment latitudinally, 
                //pickup at the end of the segment and start adding points to it again.

                Point cp;
                Point lp = part.Points[0];

                List<PointSet> leftQueue = new List<PointSet>();
                List<PointSet> rightQueue = new List<PointSet>();
                bool usingLeftSet = true;
                PointSet currentSet = new PointSet();

                for(int i = 1; i < part.Points.Count; i++)
                {
                    cp = part.Points[i];
                    currentSet.Points.Add(cp);

                    //Check for intersection with meridian
                    if((cp.x < longitude && lp.x > longitude) || cp.x > longitude && lp.x < longitude)
                    {
                        //Find intersection point.
                        double xDiff = cp.x - lp.x;
                        double yDiff = cp.y - lp.y;
                        double meridianY = cp.y + ((Math.Abs(longitude - cp.x) / xDiff) * yDiff);

                        //Add the intersection point to the current section
                        Point iPoint = new Point(longitude, meridianY);
                        currentSet.Points.Add(iPoint);

                        //Check to see if the intersection point lies between the start and end of any set in either queue.
                        //If the point is between the start and end, we have to determine whether the defined segment will be a new part or appended
                        //to the end of the existing part.

                        if (usingLeftSet) 
                        { 
                            //Check to the start and end of the set. If the last point in the current set lies between its first point
                            //and the last point of the last set in the queue, make it a separate part; otherwise, append the points to the
                            //last item in the queue.

                            leftQueue.Add(currentSet); 
                        }
                        else 
                        { 
                            rightQueue.Add(currentSet); 
                        }

                        usingLeftSet = !usingLeftSet;

                        //Start on a new section on the other side of the meridian.
                        currentSet = new PointSet();
                        currentSet.Points.Add(iPoint);
                    }

                    lp = cp;
                }
            }

            return polygon;
        }
    }
}
