using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;
using System.Drawing;
using com.pmg.MapMaker.src;

namespace com.pmg.MapMaker
{
    public class MapCanvas
    {
        private RenderConfig renderConfig;
        private SKImageInfo imageInfo;
        private SKSurface surface;
        private SKCanvas canvas;
        private int zoom;
        private float xOffset;
        private float yOffset;

        public MapCanvas(int width, int height, RenderConfig renderConfig)
        {
            imageInfo = new SKImageInfo(width, height);
            surface = SKSurface.Create(imageInfo);
            canvas = surface.Canvas;
            this.renderConfig = renderConfig;
        }

        public MapCanvas(int width, int height, RenderConfig renderConfig, Color color) : this(width, height, renderConfig)
        {
            canvas.Clear(new SKColor(color.R, color.G, color.B, color.A));
        }

        public void FillCanvas(Color color)
        {
            canvas.Clear(new SKColor(color.R, color.G, color.B, color.A));
        }

        public void DrawPoint()
        {

        }

        public void FillPolygon(Polygon pgon, Color fillColor)
        {
            SKColor skFillColor = GetSKColor(fillColor);

            foreach (PointSet part in pgon.Parts)
            {
                FillPolygon(part, skFillColor);
            }
        }

        public void FillBorderedPolygon(Polygon pgon, Color fillColor, Color borderColor)
        {
            SKColor skFillColor = GetSKColor(fillColor);
            SKColor skBorderColor = GetSKColor(borderColor);

            foreach (PointSet part in pgon.Parts)
            {
                FillBorderedPolygon(part, skFillColor, skBorderColor);
            }
        }

        public void FillPolygon(PointSet pointSet, Color fillColor)
        {
            FillPolygon(pointSet, GetSKColor(fillColor));
        }

        public void FillBorderedPolygon(PointSet pointSet, Color fillColor, Color borderColor)
        {
            FillBorderedPolygon(pointSet, GetSKColor(fillColor), GetSKColor(borderColor));
        }

        public void FillPolygon(PointSet pointSet, SKColor skFillColor)
        {
            SKPoint[] points = CreatePointArrayForPart(pointSet);
            DrawPoly(points, skFillColor);
        }

        public void FillBorderedPolygon(PointSet pointSet, SKColor skFillColor, SKColor skBorderColor)
        {
            SKPoint[] points = CreatePointArrayForPart(pointSet);
            DrawPoly(points, skFillColor);
            DrawLines(points, skBorderColor);
        }

        public static SKColor GetSKColor(Color color)
        {
            return new SKColor(color.R, color.G, color.B, color.A);
        }

        private void DrawPoly(SKPoint[] points, SKColor color)
        {
            SKPaint paint = new SKPaint();

            SKPath polygonPath = CreatePolygonPath(points);
            paint.Color = color;
            canvas.DrawPath(polygonPath, paint);
        }

        private void DrawLines(SKPoint[] points, SKColor color)
        {
            SKPaint paint = new SKPaint();

            paint.Color = color;
            paint.StrokeWidth = 4.0f;
            paint.IsStroke = true;
            paint.IsDither = false;

            for(int i = 0; i < points.Length-1; i++)
            {
                canvas.DrawLine(points[i], points[i + 1], paint);
            }
            //canvas.DrawPoints(SKPointMode.Lines, points, paint);
        }

        public void DrawPolyLine(PolyLine pline, Color color)
        {
            SKColor skColor = GetSKColor(color);

            foreach (PointSet part in pline.Parts)
            {
                SKPoint[] points = CreatePointArrayForPart(part);
                DrawLines(points, skColor);
            }
        }

        public void DrawPolygon(Polygon pgon, Color color)
        {
            SKColor skColor = GetSKColor(color);

            foreach (PointSet part in pgon.Parts)
            {
                SKPoint[] points = CreatePointArrayForPart(part);
                DrawLines(points, skColor);
            }
        }

        private SKPath CreatePolygonPath(SKPoint[] points)
        {
            SKPath path = new SKPath();
            path.AddPoly(points);
            path.Close();
            return path;
        }

        private SKPoint[] CreatePointArrayForPart(PointSet part)
        {
            SKPoint[] points = new SKPoint[part.Points.Count];
            for (int i = 0; i < part.Points.Count; i++)
            {
                //Point currentPoint = part.Points[i];
                Point currentPoint = ReprojectPoint(part.Points[i]);

                //Take zoom and offset into consideration
                SKPoint pf = new SKPoint((float)((currentPoint.X +renderConfig.XOffset) * imageInfo.Width * (renderConfig.ZoomLevel + 1)), 
                    imageInfo.Height - (float)((currentPoint.Y + renderConfig.YOffset) * imageInfo.Height * (renderConfig.ZoomLevel + 1)));

                //SKPoint pf = new SKPoint((float)(currentPoint.X  * imageInfo.Width), imageInfo.Height - (float)(currentPoint.Y * imageInfo.Height));
                points[i] = pf;
            }

            return points;
        }

        public Point ReprojectPoint(Point point)
        {
            switch(renderConfig.ProjectionSettings.ProjectionType) 
            {
                case ProjectionType.AZIMUTHAL: 
                    return Projector.NormalizeAzimuth(Projector.ToPolarAzimuth(point, 
                        ((AzimuthalProjection)renderConfig.ProjectionSettings).OuterEdgeLatitude));
                case ProjectionType.EQUIRECTANGULAR:
                    return Projector.NormalizeEquirectangular(Projector.ToEquirectangular(point,
                        ((EquirectangularProjection)renderConfig.ProjectionSettings).CentralMeridianLongitude,
                        ((EquirectangularProjection)renderConfig.ProjectionSettings).CentralParallelLatitude,
                        ((EquirectangularProjection)renderConfig.ProjectionSettings).StandardParallelLatitude));
                case ProjectionType.MERCATOR:
                    return Projector.NormalizeMercator(Projector.ToMercator(point, 
                        ((MercatorProjection)renderConfig.ProjectionSettings).LatitudeClip,
                        ((MercatorProjection)renderConfig.ProjectionSettings).CentralMeridianLongitude));
                case ProjectionType.MOLLWEIDE:
                    return Projector.NormalizeMollweide(Projector.ToMollweide(point, 
                        ((MolleweideProjection)renderConfig.ProjectionSettings).CentralMeridianLongitude));
                case ProjectionType.WAGNERVI:
                    return Projector.NormalizeWagnerVI(Projector.ToWagnerVI(point));
                case ProjectionType.NATURAL_EARTH:
                    return Projector.NormalizeNaturalEarth(Projector.ToNaturalEarth(point));
                case ProjectionType.ECKERTIV:
                    return Projector.NormalizeEckertIV(Projector.ToEckertIV(point));
                case ProjectionType.ORTHOGRAPHIC:
                    return Projector.NormalizeOrthographic(Projector.ToOrthographic(point, 
                        ((OrthographicProjection)renderConfig.ProjectionSettings).CenterLatitude,
                        ((OrthographicProjection)renderConfig.ProjectionSettings).CenterLongitude));
                default:
                    return null;
            }
        }

        public void SaveImage(string filePath, ImageFormat format = ImageFormat.PNG, int quality = 100)
        {
            SKImage img = surface.Snapshot();
            SKData imgData = img.Encode(GetSKEncodingFormat(format), quality);
            System.IO.FileStream fs = System.IO.File.OpenWrite(filePath);
            System.IO.BinaryWriter bw = new System.IO.BinaryWriter(fs);

            System.IO.Stream imgStream = imgData.AsStream();

            byte[] buffer = new byte[1024];
            int length = 0;
            while ((length = imgStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                bw.Write(buffer, 0, length);
            }

            bw.Flush();
            imgStream.Close();
            bw.Close();
            fs.Close();
        }

        private SKEncodedImageFormat GetSKEncodingFormat(ImageFormat format)
        {
            switch(format)
            {
                case ImageFormat.BMP: return SKEncodedImageFormat.Bmp;
                case ImageFormat.GIF: return SKEncodedImageFormat.Gif;
                case ImageFormat.JPEG: return SKEncodedImageFormat.Jpeg;
                case ImageFormat.PNG: return SKEncodedImageFormat.Png;
            }

            return SKEncodedImageFormat.Png;
        }
    }
}
