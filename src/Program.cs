using System;
using System.IO;
using System.Drawing;
using SkiaSharp;
using System.Xml.Serialization;

namespace com.pmg.MapMaker
{
    public class MapMaker
    {
        private static string inputFilePath = "render_settings.xml";
        private static int imageWidth = 1800;
        private static int imageHeight = 900;

        private static RenderConfig renderConfig;
        private static ProjectionSettings projectionSettings = new MercatorProjection();

        static void Main(string[] args)
        {
            ProcessArgs(args);

            XmlSerializer serializer = new XmlSerializer(typeof(RenderConfig));
            FileStream fs = new FileStream(inputFilePath, FileMode.Open);
            renderConfig = (RenderConfig)serializer.Deserialize(fs);
            fs.Close();

            Console.WriteLine("Render projection: " + renderConfig.ProjectionSettings);
            MapCanvas canvas = new MapCanvas(renderConfig.ImageWidth, renderConfig.ImageHeight, renderConfig.ProjectionSettings);

            if(renderConfig.ClearColor != null)
            {
                canvas.FillCanvas(renderConfig.ClearColor);
            }

            Color lineColor = Color.FromArgb(126, 149, 164);
            DrawMeridians(canvas, lineColor);
            DrawGreatCircles(canvas, lineColor);

            foreach (Layer layer in renderConfig.Layers)
            {
                string filepath = layer.FilePath;
                ShapeData shapeData = ShapefileReader.ReadFile(filepath);

                foreach(Shape shape in shapeData.Shapes) 
                {
                    if (layer.FillPolygons)
                    {
                        Polygon polygon = null;

                        if (shape is PolyLine)
                        {
                            polygon = new Polygon();
                            polygon.Parts = ((PolyLine)shape).Parts;
                        }
                        else
                        {
                            polygon = (Polygon)shape;
                        }

                        if (layer.DrawEdges)
                        {
                            canvas.FillBorderedPolygon(polygon, layer.PolygonFillColor, layer.EdgeColor);
                        }
                        else
                        {
                            canvas.FillPolygon(polygon, layer.PolygonFillColor);
                        }
                    }
                    else if (layer.DrawEdges)
                    {
                        PolyLine line = null;

                        if (shape is Polygon) 
                        {
                            line = new PolyLine();
                            line.Parts = ((Polygon)shape).Parts;
                        }
                        else
                        {
                            line = (PolyLine)shape;
                        }

                        canvas.DrawPolyLine(line, layer.EdgeColor);
                    }
                }
            }

            canvas.SaveImage("map.png", ImageFormat.PNG);

            //154 = USA
            //10 = CHINA
            //49 = RUSSIA
        }

        private static void DrawMeridians(MapCanvas canvas, Color color)
        {
            for (float lon = -180f; lon <= 180; lon += 10.0f)
            {
                PolyLine meridianLine = CreateMeridanLine(lon);

                canvas.DrawPolyLine(meridianLine, color);
            }
        }

        private static PolyLine CreateMeridanLine(float longitude)
        {
            PointSet pointset = new PointSet();

            for (float lat = -90.0f; lat <= 90.0f; lat += 5.0f)
            {
                Point b = new Point(longitude, lat);
                pointset.Points.Add(b);
            }

            PolyLine line = new PolyLine();
            line.Parts.Add(pointset);
            return line;
        }

        private static void DrawGreatCircles(MapCanvas canvas, Color color)
        {
            for (float lat = -90.0f; lat <= 90.0f; lat += 10.0f)
            {
                PolyLine circleLine = CreateGreatCircle(lat);

                canvas.DrawPolyLine(circleLine, color);
            }
        }

        private static PolyLine CreateGreatCircle(float latitude)
        {
            PointSet pointset = new PointSet();

            for (float lon = -180.0f; lon <= 180.0f; lon += 5.0f)
            {
                Point b = new Point(lon, latitude);
                pointset.Points.Add(b);
            }

            PolyLine line = new PolyLine();
            line.Parts.Add(pointset);
            return line;
        }

        public static void ProcessArgs(string[] args)
        {
            for(int i = 0; i < args.Length; i++)
            {
                if(args[i].ToLower().EndsWith("-h") || args[i].ToLower().EndsWith("-help"))
                {
                    Console.WriteLine("  -input                         |   The path of the input render settings file to process (Default render_settings.xml).");
                    Console.WriteLine("");
                    Console.WriteLine("  -projection                    |   The projeciton type to use. Default is MERCATOR. Options:");
                    Console.WriteLine("                                       AZIMUTHAL, EQUIRECTANGULAR, MERCATOR, MOLLWEIDE");
                    Console.WriteLine("");
                    Console.WriteLine("  -img-width                     |   The desired width of the output image in pixels (Default 1800).");
                    Console.WriteLine("");
                    Console.WriteLine("  -img-height                    |   The desired height of the output image in pixels (Default 900).");
                    Console.WriteLine("");
                    Console.WriteLine("  -central-meridian-longitude    |   The longitude (in degrees) of the central meridian for the projection.");
                    Console.WriteLine("                                       Applies to EQUIRECTANGULAR, MERCATOR, and MOLLWEIDE projections.");
                    Console.WriteLine("");
                    Console.WriteLine("  -central-parallel-latitude     |   The latitude (in degrees) of the central parallel for the projection.");
                    Console.WriteLine("                                       Applies to EQUIRECTANGULAR projections.");
                    Console.WriteLine("");
                    Console.WriteLine("  -standard-parallel-latitude    |   The latitude (in degrees) of the standard parallels of the projection.");
                    Console.WriteLine("                                       Applies to EQUIRECTANGULAR projections.");
                    Console.WriteLine("");
                    Console.WriteLine("  -latitude-clip                 |   The latitudes beyond which the render will be cut off. Used in web mercator.");
                    Console.WriteLine("                                       Applies to MERCATOR projections.");
                    Console.WriteLine("");
                    Console.WriteLine("  -outer-edge-latitude           |   The lower bound (in degrees) of the latitude for the outer-most edge of the projection.");
                    Console.WriteLine("                                       Applies to AZIMUTHAL projections.");
                    Console.WriteLine("");
                    Console.WriteLine("  -h                             |   Show this help.");
                    Console.WriteLine("");
                    System.Environment.Exit(0);
                }
                else if(args[i].ToLower().Equals("-input"))
                {
                    inputFilePath = args[i + 1];
                }
                else if (args[i].ToLower().Equals("-img-width"))
                {
                    imageWidth = Int32.Parse(args[i + 1]);
                }
                else if (args[i].ToLower().Equals("-img-height"))
                {
                    imageHeight = Int32.Parse(args[i + 1]);
                }
                else if (args[i].ToLower().Equals("-projection"))
                {
                    try
                    {
                        ProjectionType projType = (ProjectionType)Enum.Parse(typeof(ProjectionType), args[i + 1]);
                        projectionSettings = CreateProjectionSettingsFromProjectionType(projType);
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine("The projection specified is not valid. Valid options are: AZIMUTHAL, EQUIRECTANGULAR, MERCATOR, AND MOLLWEIDE.");
                        System.Environment.Exit(0);
                    }
                }
                else if (args[i].ToLower().Equals("-central-meridian-longitude"))
                {
                    if (projectionSettings is EquirectangularProjection)
                    {
                        ((EquirectangularProjection)projectionSettings).CentralMeridianLongitude = double.Parse(args[i + 1]);
                    }
                    else if(projectionSettings is MercatorProjection)
                    {
                        ((MercatorProjection)projectionSettings).CentralMeridianLongitude = double.Parse(args[i + 1]);
                    }
                    else if (projectionSettings is MolleweideProjection)
                    {
                        ((MolleweideProjection)projectionSettings).CentralMeridianLongitude = double.Parse(args[i + 1]);
                    }
                }
                else if (args[i].ToLower().Equals("-central-parallel-latitude"))
                {
                    ((EquirectangularProjection)projectionSettings).CentralParallelLatitude = double.Parse(args[i + 1]);
                }
                else if (args[i].ToLower().Equals("-standard-parallel-latitude"))
                {
                    ((EquirectangularProjection)projectionSettings).StandardParallelLatitude = double.Parse(args[i + 1]);
                }
                else if (args[i].ToLower().Equals("-latitude-clip"))
                {
                    ((MercatorProjection)projectionSettings).LatitudeClip = double.Parse(args[i + 1]);
                }
                else if (args[i].ToLower().Equals("-outer-edge-latitude"))
                {
                    ((AzimuthalProjection)projectionSettings).OuterEdgeLatitude = double.Parse(args[i + 1]);
                }
            }
        }

        private static ProjectionSettings CreateProjectionSettingsFromProjectionType(ProjectionType projType)
        {
            switch(projType)
            {
                case ProjectionType.AZIMUTHAL: return new AzimuthalProjection();
                case ProjectionType.EQUIRECTANGULAR: return new EquirectangularProjection();
                case ProjectionType.MERCATOR: return new MercatorProjection();
                case ProjectionType.MOLLWEIDE: return new MolleweideProjection();
                default: return null;
            }
        }
    }
}
