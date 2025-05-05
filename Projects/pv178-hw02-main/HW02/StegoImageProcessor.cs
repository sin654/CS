using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace HW02
{
    public class StegoImageProcessor
    {
        private struct PixelChannel
        {
            public const int Red = 1;
            public const int Green = 2;
            public const int Blue = 4;
            public const int Alpha = 8;
        }

        private struct ChunkSize
        {
            public const int OneBit = 1;
            public const int TwoBits = 2;
            public const int FourBits = 4;
            public const int EightBits = 8;
        }

        private const int HeaderChannel = PixelChannel.Blue;
        private const int DataChannel = PixelChannel.Red;
        private const int ChunkSizeInBits = ChunkSize.TwoBits;
        private const int ChunksInByte = 8 / ChunkSizeInBits;


        /*
        Image will hold the key for decoding of the payload in its pixels. This key will be located on Blue channel of pixels.
        First pixel will hold the information about the channel of payload (I will use Red) and chunk size, 4 LSB of the byte will be used for chunk size, 4 MSB for channel.
        Second and third pixel will hold the information about the size of the payload (in pixels) resulting in 32 bits of possible chunks of data encoded. 3rd pixel acts as LSB 2nd pixel and MSB
        After the header the payload will follow without any space between.
        */

        // Use constructor for additional configuration

        public async Task<Image<Rgba32>> LoadImageAsync(string path)
        {
            Image<Rgba32> image = await Image<Rgba32>.LoadAsync<Rgba32>(path);
            return image;
        }

        public async Task SaveImageAsync(Image<Rgba32> image, string path)
        {
            await image.SaveAsJpegAsync(path);
        }

        public Task<Image<Rgba32>> EncodePayload(Image<Rgba32> image, byte[] payload) => Task.Run(() => 
        {
            image.ProcessPixelRows(accessor =>
            {
                // 1. Split the payload to chunks and create some list from it or stack, which I can remove the elements from until empty
                List<byte> chunks = new List<byte>();
                foreach (byte b in payload)
                {
                    // Split the byte into chunks
                    byte[] chunksInByte = ByteSpliting.Split(b, ChunkSizeInBits).ToArray<byte>();
                    foreach (byte byteChunk in chunksInByte)
                    {
                        chunks.Add(byteChunk);
                    }
                }

                // 2. Create header and encode it to picture
                byte headerFirstByte = (byte)((DataChannel << 4) | (byte)ChunkSizeInBits);
                int pixelCount = chunks.Count(); // if bigger than 16 bits, then error?
                if (pixelCount > ushort.MaxValue)
                {
                    throw new ArgumentException($"The payload is too big! {pixelCount} pixels needed for encoding.");
                }

                byte headerSecondByte = (byte)((short)(pixelCount) >> 8); // taking 8 MSB bits of the short (16 bits)
                byte headerThirdByte = (byte)((short)(pixelCount)); // taking 8 LSB bits of the short (16 bits)

                // Encode header to picture
                EncodeHeaderPixel(ref accessor.GetRowSpan(0)[0], headerFirstByte);
                EncodeHeaderPixel(ref accessor.GetRowSpan(0)[1], headerSecondByte);
                EncodeHeaderPixel(ref accessor.GetRowSpan(0)[2], headerThirdByte);

                // 3. Encode chunks to pixels
                for (int rowIdx = 0; rowIdx < image.Height; rowIdx++)
                {
                    if (chunks.Count() == 0)
                    {
                        // No more data to encode
                        break;
                    }

                    Span<Rgba32> row = accessor.GetRowSpan(rowIdx);
                    for (int i = 0; i < row.Length; i++)
                    {
                        if (rowIdx == 0 && i < 3) // skip header
                        {
                            continue;
                        }

                        if (chunks.Count() == 0)
                        {
                            // No more data to encode
                            break;
                        }

                        byte chunk = chunks.First();
                        chunks.RemoveAt(0);
                        EncodeDataPixel(ref row[i], chunk);
                    }
                }
            });

            return image;
        });


        private void EncodeDataPixel(ref Rgba32 pixel, byte data)
        {
            byte EncodeByte(byte @byte, byte data)
            {
                byte mask = (byte)((1 << ChunkSizeInBits) - 1); // e.g., ChunkSizeInBits = 2 → mask = 0b00000011

                // clear N of LSB bits for data
                byte encodedByte = (byte)(@byte & ~mask);
                // encode the data
                encodedByte = (byte)(encodedByte | data);

                return encodedByte;
            }

            switch (DataChannel)
            {
                case PixelChannel.Red:
                    pixel.R = EncodeByte(pixel.R, data);
                    break;
                case PixelChannel.Green:
                    pixel.G = EncodeByte(pixel.R, data);
                    break;
                case PixelChannel.Blue:
                    pixel.B = EncodeByte(pixel.R, data);
                    break;
                case PixelChannel.Alpha:
                    pixel.A = EncodeByte(pixel.R, data);
                    break;
            }
        }

        private void EncodeHeaderPixel(ref Rgba32 pixel, byte headerData)
        {
            switch (HeaderChannel)
            {
                case PixelChannel.Red:
                    pixel.R = headerData;
                    break;
                case PixelChannel.Green:
                    pixel.G = headerData;
                    break;
                case PixelChannel.Blue:
                    pixel.B = headerData;
                    break;
                case PixelChannel.Alpha:
                    pixel.A = headerData;
                    break;
            }
        }

        public Task<byte[]> ExtractPayload(Image<Rgba32> image) => Task.Run(() =>
        {
            // Get header data
            int channel, chunkSize, pixelCount;
            (channel, chunkSize, pixelCount) = ExtractHeader(image);

            List<byte> payload = new List<byte>();
            image.ProcessPixelRows(accessor =>
            {
                // Decode data
                for (int rowIdx = 0; rowIdx < image.Height; rowIdx++)
                {
                    if (pixelCount == 0)
                    {
                        // No more data to decode
                        break;
                    }

                    List<byte> chunks = new List<byte>();
                    Span<Rgba32> row = accessor.GetRowSpan(rowIdx);
                    for (int i = 0; i < row.Length; i++)
                    {
                        if (rowIdx == 0 && i < 3) // skip header
                        {
                            continue;
                        }

                        if (pixelCount == 0)
                        {
                            // No more data to decode
                            break;
                        }

                        // unmask chunk from pixel
                        chunks.Add(ExtractChunk(row[i], channel, chunkSize));
                        pixelCount--;

                        if (chunks.Count() == ChunksInByte)
                        {
                            // reform byte from chunks
                            byte byteChunk = ByteSpliting.Reform(chunks, chunkSize);
                            payload.Add(byteChunk);
                            chunks.Clear();
                        }
                    }
                }
            });

            return payload.ToArray();
        });


        private byte ExtractChunk(Rgba32 pixel, int channel, int chunkSize)
        {
            byte mask = (byte)((1 << chunkSize) - 1);

            byte chunk = 0;
            switch (channel)
            {
                case PixelChannel.Red:
                    chunk = (byte)(pixel.R & mask);
                    break;
                case PixelChannel.Green:
                    chunk = (byte)(pixel.G & mask);
                    break;
                case PixelChannel.Blue:
                    chunk = (byte)(pixel.B & mask);
                    break;
                case PixelChannel.Alpha:
                    chunk = (byte)(pixel.A & mask);
                    break;
            }
            return chunk;
        }


        private (int channel, int chunkSize, int pixelCount) ExtractHeader(Image<Rgba32> image)
        {
            byte firstByte = 0;
            byte secondByte = 0;
            byte thirdByte = 0;

            image.ProcessPixelRows(accessor =>
            {
                // Decode header
                Span<Rgba32> row = accessor.GetRowSpan(0);
                switch (HeaderChannel)
                {
                    case PixelChannel.Red:
                        firstByte = row[0].R;
                        secondByte = row[1].R;
                        thirdByte = row[2].R;
                        break;
                    case PixelChannel.Green:
                        firstByte = row[0].G;
                        secondByte = row[1].G;
                        thirdByte = row[2].G;
                        break;
                    case PixelChannel.Blue:
                        firstByte = row[0].B;
                        secondByte = row[1].B;
                        thirdByte = row[2].B;
                        break;
                    case PixelChannel.Alpha:
                        firstByte = row[0].A;
                        secondByte = row[1].A;
                        thirdByte = row[2].A;
                        break;
                }
            });

            int channel = firstByte >> 4; // 4 MSB bits
            int chunkSize = firstByte & 0b0000_1111; // 4 LSB bits
            int pixelCount = (secondByte << 8) | thirdByte; // 8 MSB bits + 8 LSB bits

            return (channel, chunkSize, pixelCount);
        }
    }
}
