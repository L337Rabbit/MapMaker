using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace com.pmg.MapMaker
{
    public class BitHelper
    {
        public static int ReadIntBigEndian(BinaryReader br)
        {
            byte[] ba = br.ReadBytes(4);
            Array.Reverse(ba);
            return BitConverter.ToInt32(ba);
        }

        public static double ReadDoubleBigEndian(BinaryReader br)
        {
            byte[] ba = br.ReadBytes(8);
            Array.Reverse(ba);
            return BitConverter.ToDouble(ba);
        }
    }
}
