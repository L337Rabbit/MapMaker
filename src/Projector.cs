using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace com.pmg.MapMaker
{
    public class Projector
    {
        public const double EARTH_RADIUS = 6371000; //In meters
        public const double TROPIC_OF_CANCER_LATITUDE = 23.439166;
        public const double TROPIC_OF_CAPRICORN_LATITUDE = -23.439166;

        public static Point ToEquirectangular(Point point, double centralMeridianLon = 0.0, double centralParallelLat = 0.0, double standardParallelLat = TROPIC_OF_CANCER_LATITUDE)
        {
            double x = EARTH_RADIUS * ToRadians(point.x - centralMeridianLon) * Math.Cos(ToRadians(standardParallelLat));
            double y = EARTH_RADIUS * ToRadians(point.y - centralParallelLat);
            return new Point(x, y);
        }

        public static Point NormalizeEquirectangular(Point equiPoint)
        {
            double x = equiPoint.X;
            x += (EARTH_RADIUS * ToRadians(180));
            x /= (EARTH_RADIUS * ToRadians(360));

            double y = equiPoint.Y;
            y += (EARTH_RADIUS * ToRadians(90));
            y /= (EARTH_RADIUS * ToRadians(180));

            return new Point(x, y);
        }

        public static Point ToPolarAzimuth(Point point, double edgeLatitude = -30.0)
        {
            double lonRadians = ToRadians(point.x);
            double edgeRadians = ToRadians(edgeLatitude);

            double distance = 0.0;

            if (point.y < edgeLatitude)
            {
                distance = 1.0;
            }
            else
            {
                double latRange = 90.0 - edgeLatitude;

                double transformedLat = ((point.y - edgeLatitude) / latRange) * 90.0;

                double latRadians = ToRadians(transformedLat);
                distance = Math.Cos(latRadians);
            }

            //if (point.y < 0.0f) { distance = 1.0f - distance; }
            return new Point(Math.Cos(lonRadians) * distance, Math.Sin(lonRadians) * distance);
        }

        public static Point NormalizeAzimuth(Point point)
        {
            return new Point((point.X + 1.0) / 2.0, (point.Y + 1.0) / 2.0);
        }

        public static Point ToMollweide(Point point, double centralMeridian = 0.0)
        {
            //Console.WriteLine("Converting: " + point.X + " , " + point.Y);

            double lonRadians = ToRadians(point.x);
            double latRadians = ToRadians(point.y);
            double centralMeridianRadians = ToRadians(centralMeridian);

            double theta = latRadians;
            double epsilon = .000001;

            if (Math.PI / 2.0 - Math.Abs(theta) <= .0000000000001)
            {
                if (theta < 0) { theta = -Math.PI / 2.0; }
                else { theta = Math.PI / 2.0; }
            }
            else
            {
                while (true)
                {
                    double nextTheta = theta - ((2.0 * theta + Math.Sin(2.0 * theta) - Math.PI * Math.Sin(latRadians)) / (2.0 + 2.0 * Math.Cos(2.0 * theta)));

                    if (Math.Abs(theta - nextTheta) < epsilon)
                    {
                        break;
                    }
                    theta = nextTheta;
                }
            }

            //Subtract central meridian from each value. If the final x is less than -180, subtract the absolute value of the difference from positive 180.
            double lonVal = ToRadians(point.x - centralMeridian);
            if (lonVal < -180.0)
            {
                lonVal = 180.0 - Math.Abs(lonVal + 180.0);
                lonVal = ToRadians(lonVal);
            }

            double x = EARTH_RADIUS * ((2.0 * Math.Sqrt(2.0)) / Math.PI) * (lonVal) * Math.Cos(theta);
            double y = EARTH_RADIUS * Math.Sqrt(2.0) * Math.Sin(theta);

            return new Point(x, y);
        }

        public static Point NormalizeMollweide(Point mollWeidePoint)
        {
            //Range is X:[|+-|2 * EARTH_RADIUS * SQRT(2)]   Y:[|+-|EARTH_RADIUS * SQRT(2)]

            //Normalize values to be between 0 and 1.
            double x = mollWeidePoint.X;
            x += (2.0 * EARTH_RADIUS * Math.Sqrt(2));
            x /= (4.0 * EARTH_RADIUS * Math.Sqrt(2));
            double y = mollWeidePoint.Y;
            y += (EARTH_RADIUS * Math.Sqrt(2));
            y /= (2.0 * EARTH_RADIUS * Math.Sqrt(2));

            return new Point(x, y);
        }

        public static Point ToMercator(Point point, double latClip = 85.051129, double centralMeridian = 0.0)
        {
            double x = EARTH_RADIUS * ( ToRadians(point.X) - ToRadians(centralMeridian));

            double yVal = point.y;
            if(yVal > latClip) { yVal = latClip; }
            if(yVal < -latClip) { yVal = -latClip; }
            double y = EARTH_RADIUS * Math.Log(Math.Tan(Math.PI / 4.0 + ToRadians(yVal) / 2.0));

            return new Point(x, y);
        }

        public static Point NormalizeMercator(Point mercPoint)
        {
            double x = ((mercPoint.X / (EARTH_RADIUS * Math.PI)) + 1.0) / 2;
            double y = ((mercPoint.Y / (EARTH_RADIUS * Math.PI)) + 1.0) / 2;
            return new Point(x, y);
        }

        public static double ToDegrees(double radians)
        {
            return (radians / (2.0 * Math.PI)) * 360.0;
        }

        public static double ToRadians(double degrees)
        {
            return (degrees * Math.PI) / 180.0;
        }
    }
}
