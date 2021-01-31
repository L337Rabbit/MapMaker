using System;
using System.IO;
using System.Drawing;
using SkiaSharp;
using System.Xml.Serialization;
using com.pmg.MapMaker.src;
using System.Threading.Tasks;

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

            foreach (Layer layer in renderConfig.Layers)
            {
                if (layer is ParallelLayer)
                {
                    DrawParallels(canvas, (ParallelLayer)layer);
                }
                else if (layer is MeridianLayer)
                {
                    DrawMeridians(canvas, (MeridianLayer)layer);
                }
                else if (layer is PolygonLayer)
                {
                    string filepath = layer.FilePath;
                    ShapeData shapeData = ShapefileReader.ReadFile(filepath);
                    DrawPolygons(shapeData, canvas, (PolygonLayer)layer);

                }
                else if (layer is LineLayer)
                {
                    string filepath = layer.FilePath;
                    ShapeData shapeData = ShapefileReader.ReadFile(filepath);
                    DrawPolylines(shapeData, canvas, (LineLayer)layer);
                }
            }

            canvas.SaveImage("map.png", ImageFormat.PNG);

            //154 = USA
            //10 = CHINA
            //49 = RUSSIA
        }

        private static void DrawPolylines(ShapeData shapeData, MapCanvas canvas, LineLayer layer)
        {
            foreach (Shape shape in shapeData.shapes)
            {
                DrawPolyline(shape, canvas, layer);
            }
        }

        private static void DrawPolyline(Shape shape, MapCanvas canvas, LineLayer layer)
        {
            PolyLine line = null;

            if(shape is Polygon)
            {
                line = new PolyLine();
                line.Parts = ((Polygon)shape).Parts;
            }
            else { line = (PolyLine)shape; }

            canvas.DrawPolyLine(line, layer.EdgeColor);
        }

        private static void DrawPolygons(ShapeData shapeData, MapCanvas canvas, PolygonLayer layer)
        {
            foreach (Shape shape in shapeData.shapes)
            {
                DrawPolygon(shape, canvas, layer);
            }
        }

        private static void DrawPolygon(Shape shape, MapCanvas canvas, PolygonLayer layer)
        {
            Polygon polygon = null;

            if (shape is PolyLine)
            {
                polygon = new Polygon();
                polygon.Parts = ((PolyLine)shape).Parts;
            }
            else { polygon = (Polygon)shape; }

            if(layer.FillPolygons)
            {
                if(layer.DrawEdges)
                {
                    canvas.FillBorderedPolygon(polygon, layer.FillColor, layer.EdgeColor);
                }
                else
                {
                    canvas.FillPolygon(polygon, layer.FillColor);
                }
            }
            else if(layer.DrawEdges)
            {
                canvas.DrawPolygon(polygon, layer.EdgeColor);

            }
        }

        private static void DrawMeridians(MapCanvas canvas, MeridianLayer layer)
        {
            for (double lon = layer.MinimumLongitude; lon <= layer.MaximumLongitude; lon += layer.LongitudeInterval)
            {
                PolyLine meridianLine = CreateMeridanLine(lon, layer);

                canvas.DrawPolyLine(meridianLine, layer.Color);
            }
        }

        private static PolyLine CreateMeridanLine(double longitude, MeridianLayer layer)
        {
            PointSet pointset = new PointSet();

            for (double lat = layer.MinimumLatitude; lat <= layer.MaximumLatitude; lat += layer.LatitudeInterval)
            {
                Point b = new Point(longitude, lat);
                pointset.Points.Add(b);
            }

            PolyLine line = new PolyLine();
            line.Parts.Add(pointset);
            return line;
        }

        private static void DrawParallels(MapCanvas canvas, ParallelLayer layer)
        {
            for (double lat = layer.MinimumLatitude + layer.LatitudeOffset; lat <= layer.MaximumLatitude; lat += layer.LatitudeInterval)
            {
                PolyLine circleLine = CreateParallel(lat, layer);

                canvas.DrawPolyLine(circleLine, layer.Color);
            }
        }

        private static PolyLine CreateParallel(double latitude, ParallelLayer layer)
        {
            PointSet pointset = new PointSet();

            for (double lon = layer.MinimumLongitude; lon <= layer.MaximumLongitude; lon += layer.LongitudeInterval)
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
