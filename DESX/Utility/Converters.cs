using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DESX.Utility
{
    public class Converters
    {
        private static List<byte> StringToBytes(string text)
        {
            return Encoding.Default.GetBytes(text).ToList();
        }

        private static string BytesToString(byte[] bytes)
        {
            return Encoding.Default.GetString(bytes);
        }

        public static List<BitArray> BytesTo64BitArrays(List<byte> bytes)
        {
            while (bytes.Count % 8 != 0)
            {
                bytes.Add(new byte());
            }

            List<BitArray> blocks = new List<BitArray>();

            for (int i = 0; i < bytes.Count; i += 8)
            {
                byte[] temp = bytes.Skip(i).Take(8).ToArray();
                blocks.Add(new BitArray(temp));
            }

            return blocks;
        }

        public static byte[] BitArrayToBytes(BitArray bits)
        {
            byte[] ret = new byte[(bits.Length - 1) / 8 + 1];
            bits.CopyTo(ret, 0);
            return ret;
        }


        public static List<BitArray> StringToBitArrays(string message)
        {
            List<byte> bytes = StringToBytes(message);
            return Converters.BytesTo64BitArrays(bytes);
        }

        public static string BitArraysToString(List<BitArray> bitArrays)
        {
            string message = "";
            bitArrays.ForEach(array =>
            {
                var bytes = Converters.BitArrayToBytes(array);
                message += BytesToString(bytes);
            });
            return message;
        }
    }
}