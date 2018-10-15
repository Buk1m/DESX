using System;
using System.Collections;
using System.Collections.Generic;
using DESX.ExtensionMethods;
using DESX.Utility;

namespace DESX.Model
{
    public static class DESXAlgorithm
    {
        public static string Encrypt(string message, List<string> keys)
        {
            List<BitArray> subKeys = GenerateSubKeys(Converters.StringToBitArrays(keys[1])[0]);
            return EncryptionFunction(message, keys, subKeys);
        }

        public static string Decrypt(string message, List<string> keys)
        {
            List<BitArray> subKeys = GenerateSubKeys(Converters.StringToBitArrays(keys[1])[0]);
            subKeys.Reverse();
            keys.Reverse();
            return EncryptionFunction(message, keys, subKeys);
        }

        #region Private

        private static string EncryptionFunction(string message, List<string> keys, List<BitArray> subKeys)
        {
            List<BitArray> cypherBlocks = Converters.StringToBitArrays(message);

            cypherBlocks.ForEach(
                block =>
                {
                    block = block.Xor(Converters.StringToBitArrays(keys[0])[0]);
                    Permutate(ref block, Constants.InitialPermutationMatrix);
                }
            );

            Operations.SplitBitArrayInHalf(cypherBlocks, out List<BitArray> leftBlocks, out List<BitArray> rightBlocks);
            for (int i = 0; i < Constants.CycleNumber; i++)
            {
                List<BitArray> prevRightBlocks = new List<BitArray>(rightBlocks);
                BitArray cycleKey = subKeys[i];
                for (int j = 0; j < rightBlocks.Count; j++)
                {
                    rightBlocks[j] = DESFunction(rightBlocks[j], leftBlocks[j], cycleKey);
                }

                leftBlocks = prevRightBlocks;
            }

            cypherBlocks = ConnectBlocks(leftBlocks, rightBlocks);
            cypherBlocks.ForEach(
                block =>
                {
                    Permutate(ref block, Constants.FinalPermutationMatrix);
                }
            );
            cypherBlocks.ForEach(block => block.Xor(Converters.StringToBitArrays(keys[2])[0]));

            return Converters.BitArraysToString(cypherBlocks);
        }

        private static BitArray DESFunction(BitArray rightBlock, BitArray leftBlock, BitArray key)
        {
            Permutate(ref rightBlock, Constants.ExpandedPermutationMatrix);
            rightBlock = rightBlock.Xor(key);
            rightBlock = SBoxesTransformation(rightBlock);
            Permutate(ref rightBlock, Constants.PBlockPermutationMatrix);
            return rightBlock.Xor(leftBlock);
        }

        private static void Permutate(ref BitArray bitArray, byte[,] permutationMatrix)
        {
            int xLength = permutationMatrix.GetLength(0);
            int yLength = permutationMatrix.GetLength(1);

            BitArray sourceBits = bitArray;

            bitArray = new BitArray(permutationMatrix.Length);
            for (int i = 0; i < xLength; i++)
            {
                for (int j = 0; j < yLength; j++)
                {
                    bitArray[i * yLength + j] = sourceBits[permutationMatrix[i, j] - 1];
                }
            }
        }

        private static List<BitArray> GenerateSubKeys(BitArray key)
        {
            Permutate(ref key, Constants.KeyPermutationMatrix);
            List<BitArray> keyHalves = Operations.SplitBitArrayInHalf(key);
            List<BitArray> subKeys = new List<BitArray>();

            for (int i = 0; i < Constants.CycleNumber; i++)
            {
                keyHalves.ForEach(half => half.ShiftLeft(Constants.KeyBitShiftValues[i]));
                BitArray subKey = keyHalves[0].Append(keyHalves[1]);
                Permutate(ref subKey, Constants.ReductionPermutationMatrix);
                subKeys.Add(subKey);
            }

            return subKeys;
        }

        private static BitArray SBoxesTransformation(BitArray array)
        {
            BitArray block = new BitArray(6);
            BitArray result = new BitArray(32);

            for (int i = 0; i < array.Length / 6; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    block[j] = array[i * 6 + j];
                }

                block.Reverse();

                byte sValue = FindSBlockValue(i, block);
                BitArray b = new BitArray(new byte[] {sValue});

                for (int j = 3; j >= 0; j--)
                {
                    result.Set(i * 4 + 3 - j, b[j]);
                }
            }

            return result;
        }

        private static List<BitArray> ConnectBlocks(List<BitArray> leftBlocks, List<BitArray> rightBlocks)
        {
            if (leftBlocks.Count != rightBlocks.Count || leftBlocks.Count == 0)
            {
                throw new ArgumentException(
                    "Number of blocks must be equal" +
                    $". leftBlocks.Count: {leftBlocks.Count} != rightBlocks.Count{rightBlocks.Count}");
            }

            List<BitArray> cypher = new List<BitArray>();
            for (int i = 0; i < leftBlocks.Count + 1 / 2; i++)
            {
                cypher.Add(rightBlocks[i].Append(leftBlocks[i]));
            }

            return cypher;
        }

        private static byte FindSBlockValue(int blockNumber, BitArray bitArray)
        {
            byte row = FindSBlockRow(bitArray);
            byte column = FindSBlockColumn(bitArray);
            return Constants.SBlocks[blockNumber, row, column];
        }

        private static byte FindSBlockRow(BitArray bitArray)
        {
            BitArray row = new BitArray(2);
            row.Set(0, bitArray[0]);
            row.Set(1, bitArray[5]);

            byte[] bytes = new byte[1];
            row.CopyTo(bytes, 0);
            return bytes[0];
        }

        private static byte FindSBlockColumn(BitArray bitArray)
        {
            BitArray column = new BitArray(4);
            for (int i = 1; i < 5; i++)
            {
                column.Set(i - 1, bitArray[i]);
            }

            byte[] bytes = new byte[1];
            column.CopyTo(bytes, 0);
            return bytes[0];
        }

        #endregion
    }
}