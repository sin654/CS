using System.ComponentModel.Design;

namespace HW02
{
    public class ByteSpliting
    {
        public const int BitsInByte = 8;

        /// <summary>
        /// Splits the byte to chunks of given size.
        /// Mind the endianness! The least significant chunks are on lower index.
        /// </summary>
        /// <param name="byte">byte to split</param>
        /// <param name="size">bits in each chunk</param>
        /// <example>Split(207,2) => [3,3,0,3]</example>
        /// <returns>chunks</returns>
        public static IEnumerable<byte> Split(byte @byte, int size)
        {
            byte mask = size switch
            {
                1 => 0b0000_0001,
                2 => 0b0000_0011,
                4 => 0b0000_1111,
                8 => 0b1111_1111,
                _ => throw new ArgumentException($"Size {size} is not supported!"),
            };

            int numberOfChunks = BitsInByte / size;
            byte processedByte = @byte;
            for (int i = 0; i < numberOfChunks; i++)
            {
                // 1 - unmask
                byte chunk = (byte)(processedByte & mask);
                // 2 - bit move by size
                processedByte = (byte)(processedByte >> size);

                yield return chunk;
            }

            while (processedByte != 0);
        }

        /// <summary>
        /// Reforms chunks to a byte.
        /// Mind the endianness! The least significant chunks are on lower index.
        /// </summary>
        /// <param name="parts">chunks to reform</param>
        /// <param name="size">bits in each chunk</param>
        /// <example>Split([3,3,0,3],2) => 207</example>
        /// <returns>byte</returns>
        public static byte Reform(IEnumerable<byte> parts, int size)
        {
            if(parts.Count() * size != BitsInByte)
                throw new ArgumentException($"The number of parts and size do not match the byte size {BitsInByte}!");

            byte mask = size switch
            {
                1 => 0b0000_0001,
                2 => 0b0000_0011,
                4 => 0b0000_1111,
                8 => 0b1111_1111,
                _ => throw new ArgumentException($"Size {size} is not supported!"),
            };

            byte reformedByte = 0;
            int offset = 0;
            foreach (byte chunk in parts)
            {
                // unmask and offset
                reformedByte = (byte)(reformedByte | ((chunk & mask) << offset));
                offset += size;
            }

            return reformedByte;
        }
    }
}