using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DESX.Utility
{
    public static class Operations
    {
        public static void SplitBitArrayInHalf(List<BitArray> bitArrayList, out List<BitArray> leftBlocks,
            out List<BitArray> rightBlocks)
        {
            leftBlocks = new List<BitArray>();
            rightBlocks = new List<BitArray>();

            foreach (var bitArray in bitArrayList)
            {
                List<BitArray> halves = SplitBitArrayInHalf(bitArray);
                leftBlocks.Add(halves[0]);
                rightBlocks.Add(halves[1]);
            }
        }

        public static List<BitArray> SplitBitArrayInHalf(BitArray bitArray)
        {
            List<BitArray> bits = new List<BitArray>();
            int blockSize = (bitArray.Count / 2);
            BitArray leftBitsArray = new BitArray(blockSize);
            BitArray rightBitsArray = new BitArray(blockSize);
            for (int i = 0; i < blockSize; i++)
            {
                leftBitsArray.Set(i, bitArray[i]);
                rightBitsArray.Set(i, bitArray[blockSize + i]);
            }

            bits.Add(leftBitsArray);
            bits.Add(rightBitsArray);

            return bits;
        }

        public static BitArray GenerateRandomKey()
        {
            Random random = new Random();

            byte[] leftBytes = BitConverter.GetBytes(random.Next());
            byte[] rightBytes = BitConverter.GetBytes(random.Next());

            byte[] keyBytes = leftBytes.Concat(rightBytes).ToArray();
            return new BitArray(keyBytes);
        }

        public static BitArray GenerateRandomKeyWithParityBits()
        {
            const int keyLengthInBytes = 8;
            Random random = new Random();
            byte[] key = new byte[keyLengthInBytes];
            int left = random.Next();
            int right = random.Next();
            byte[] leftBytes = BitConverter.GetBytes(left);
            byte[] rightBytes = BitConverter.GetBytes(right);
            key = leftBytes.Concat(rightBytes).ToArray();
            BitArray keyBits = new BitArray(key);
            int bitSum = 0;
            for (int i = 0; i < keyBits.Count; i++)
            {
                if (i % 8 == 7)
                {
                    if (bitSum % 2 == 0)
                    {
                        keyBits[i] = true;
                    }
                    else
                    {
                        keyBits[i] = false;
                    }

                    bitSum = 0;
                }
                else if (keyBits[i])
                {
                    bitSum++;
                }
            }

            return keyBits;
        }
    }
}