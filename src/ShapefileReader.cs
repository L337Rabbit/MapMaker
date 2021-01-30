using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace com.pmg.MapMaker
{
    public class ShapefileReader
    {
        private int fileCursor;
        private FileStream fs;
        private BinaryReader br;
        private ShapeType shapeType;
        private int fileSize;
        private BoundingBox bbox;

        public ShapeType ShapeType
        {
            get { return this.shapeType; }
        }

        public BoundingBox BoundingBox
        {
            get { return this.bbox; }
            set { this.bbox = value; }
        }

        public static ShapeData ReadFile(string filePath)
        {
            ShapefileReader sfr = new ShapefileReader();
            sfr.Open(filePath);

            ShapeData data = new ShapeData(sfr.ShapeType);
            data.BoundingBox = sfr.BoundingBox;

            //Read Entries
            while (sfr.HasMore())
            {
                //Read shape.
                data.shapes.Add(sfr.ReadNext());
            }

            return data;            
        }

        public void Open(string filePath)
        {
            fs = new FileStream(filePath, FileMode.Open);
            br = new BinaryReader(fs);

            ReadHeader();

            fileCursor = 100;
        }

        public void Close()
        {
            br.Close();
            fs.Close();
            br = null;
            fs = null;
        }

        public bool HasMore()
        {
            return fileCursor < fileSize;
        }

        public Shape ReadNext()
        {
            Shape shape = null;

            if (fileCursor < fileSize)
            {
                //Read shape.
                shape = Shape.Read((ShapeType)shapeType, br);
                fileCursor += shape.SizeAsRead;
            }

            return shape;
        }

        private void ReadHeader()
        {
            //Bytes 0-3 = File Code (big endian)
            int fileCode = BitHelper.ReadIntBigEndian(br);

            //Bytes 4-23 = Unused (big endian)
            br.ReadBytes(20);

            //Bytes 24-27 = File Length (big endian)
            fileSize = BitHelper.ReadIntBigEndian(br) * 2;
            //Console.WriteLine("File Size: " + fileSize);

            //Bytes 28-31 = Version (big endian)
            int version = BitHelper.ReadIntBigEndian(br);
            //Console.WriteLine("Version: " + version);

            //Bytes 32-35 = Shape Type (big endian)
            shapeType = (ShapeType)br.ReadInt32();
            //Console.WriteLine("Shape Type: " + shapeType);

            //Bytes 36-67 = Minimum Bounding Rectangle - 4 doubles - minX, minY, maxX, maxY (little endian)
            double minX = br.ReadDouble();
            double minY = br.ReadDouble();
            double maxX = br.ReadDouble();
            double maxY = br.ReadDouble();
            bbox = new BoundingBox(minX, minY, maxX, maxY);
            //Console.WriteLine(bbox);

            //Bytes 68-83 = Z Range - minZ, maxZ (little endian)
            double minZ = br.ReadDouble();
            double maxZ = br.ReadDouble();
            //Console.WriteLine("Z-Range: " + minZ + " => " + maxZ);

            //Bytes 84-99 = M Range - minM, maxM (little endian)
            double minM = br.ReadDouble();
            double maxM = br.ReadDouble();
            //Console.WriteLine("M-Range: " + minM + " => " + maxM);
        }
    }
}
