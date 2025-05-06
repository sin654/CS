using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using static System.Net.Mime.MediaTypeNames;

namespace HW02
{
    public class StegoImageProcessor
    {

        SemaphoreSlim taskLimit = new SemaphoreSlim(3);

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

        private const int HeaderChannel = PixelChannel.Red;
        private const int DataChannel = PixelChannel.Green;
        private const int ChunkSizeInBits = ChunkSize.FourBits;
        private const int ChunksInByte = 8 / ChunkSizeInBits;

        /*
        Image will hold the key for decoding of the payload in its pixels. This key will be located on (Blue) channel of pixels.
        First pixel will hold the information about the channel of payload (I will use Red) and chunk size, 4 LSB of the byte will be used for chunk size, 4 MSB for channel.
        Second and third pixel will hold the information about the size of the payload (in pixels) resulting in 32 bits of possible chunks of data encoded. 3rd pixel acts as LSB 2nd pixel and MSB
        After the header the payload will follow without any space between.
        */

        /// <summary>
        /// Saves images to the given path.
        /// </summary>
        /// <param name="images">Array of images</param>
        /// <param name="basePath">Base path to save to</param>
        /// <param name="imagePaths">Image names</param>
        /// <returns>Task of string arrays containing full paths these images were saved to</returns>
        public async Task<string[]> SaveImagesAsync(Image<Rgba32>[] images, string basePath, string[] imagePaths)
        {
            List<string> savedPaths = new List<string>();

            if (Directory.Exists(basePath))
            {
                Directory.Delete(basePath, true);
            }
            Directory.CreateDirectory(basePath);

            List<Task<string>> tasks = new List<Task<string>>();
            for (int i = 0; i < images.Count(); i++)
            {
                string path = Path.Combine(basePath, Path.GetFileNameWithoutExtension(imagePaths[i]));
                int index = i; // quite important to keep the index in the loop, otherwise it will be overwritten by the time the task is executed

                Task<string> task = Task.Run(async () => 
                {
                    await taskLimit.WaitAsync();
                    try
                    {
                        return await SaveImageAsync(images[index], path);
                    }
                    finally
                    {
                        taskLimit.Release();
                    }
                });
                tasks.Add(task);
            }

            return await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Loads images from the given paths.
        /// </summary>
        /// <param name="imagePaths">Array of paths to load images from</param>
        /// <returns>Task with list of images</returns>
        public async Task<Image<Rgba32>[]> LoadImagesAsync(string[] imagePaths)
        {
            List<Task<Image<Rgba32>>> tasks = new List<Task<Image<Rgba32>>>();

            for (int i = 0; i < imagePaths.Count(); i++)
            {
                int index = i; // quite important to keep the index in the loop, otherwise it will be overwritten by the time the task is executed

                Task<Image<Rgba32>> task = Task.Run(async () =>
                {
                    await taskLimit.WaitAsync();
                    try
                    {
                        return await LoadImageAsync(imagePaths[index]);
                    }
                    finally
                    {
                        taskLimit.Release();
                    }
                });
                tasks.Add(task);
            }
            
            return await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Encodes the payloads to the images.
        /// </summary>
        /// <param name="images">Array of images to encode payloads to</param>
        /// <param name="payloads">List of payloads to encode to images</param>
        /// <returns>Task containing array of images with encoded payloads</returns>
        public async Task<Image<Rgba32>[]> EncodeImagesAsync(Image<Rgba32>[] images, List<byte[]> payloads)
        {
            List<Task<Image<Rgba32>>> tasks = new List<Task<Image<Rgba32>>>();

            for (int i = 0; i < images.Count(); i++)
            {
                int index = i; // quite important to keep the index in the loop, otherwise it will be overwritten by the time the task is executed

                Task<Image<Rgba32>> task = Task.Run(async () =>
                {
                    await taskLimit.WaitAsync();
                    try
                    {
                        return await EncodePayload(images[index], payloads[index]);
                    }
                    finally
                    {
                        taskLimit.Release();
                    }
                });
                tasks.Add(task);
            }

            return await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Decodes the payloads from the images.
        /// </summary>
        /// <param name="images">Array of images to decode from</param>
        /// <returns>Task containing array of byte arrays with decoded payloads from each image</returns>
        public async Task<byte[][]> DecodeImagesAsync(Image<Rgba32>[] images)
        {
            var tasks = new List<Task<byte[]>>();

            for (int i = 0; i < images.Count(); i++)
            {
                int index = i; // quite important to keep the index in the loop, otherwise it will be overwritten by the time the task is executed
                Task<byte[]> task = Task.Run(async () =>
                {
                    await taskLimit.WaitAsync();
                    try
                    {
                        return await ExtractPayload(images[index]);
                    }
                    finally
                    {
                        taskLimit.Release();
                    }
                });
                tasks.Add(task);
            }

            return await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Loads image from the given path.
        /// </summary>
        /// <param name="path">Path</param>
        /// <returns>Task containing image</returns>
        public async Task<Image<Rgba32>> LoadImageAsync(string path)
        {
            Image<Rgba32> image = await Image<Rgba32>.LoadAsync<Rgba32>(path);
            return image;
        }

        /// <summary>
        /// Saves image to the given path.
        /// </summary>
        /// <param name="image">Image</param>
        /// <param name="path">Path</param>
        /// <returns>Task containing path of the saved image</returns>
        public async Task<string> SaveImageAsync(Image<Rgba32> image, string path)
        {
            string fullPath = path + ".png";
            await image.SaveAsync(fullPath);

            return fullPath;
        }

        /// <summary>
        /// Encodes the payload to the image.
        /// </summary>
        /// <param name="image">Image</param>
        /// <param name="payload">Payload to encode to image</param>
        /// <returns>Task containing image</returns>
        /// <exception cref="ArgumentException">When payload would be larger than the image can include</exception>
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

        /// <summary>
        /// Encodes the data to the pixel.
        /// </summary>
        /// <param name="pixel">Pixel</param>
        /// <param name="data">Data</param>
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

        /// <summary>
        /// Encodes the header pixel.
        /// </summary>
        /// <param name="pixel">Pixel</param>
        /// <param name="headerData">Data</param>
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

        /// <summary>
        /// Extracts the payload from the image.
        /// </summary>
        /// <param name="image">Image</param>
        /// <returns>Task containig byte array with encoded payload</returns>
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


        /// <summary>
        /// Extracts the chunk from the pixel.
        /// </summary>
        /// <param name="pixel">Pixel</param>
        /// <param name="channel">Channel of the pixel (R/G/B/A)</param>
        /// <param name="chunkSize">Size of the chunk in bites</param>
        /// <returns></returns>
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


        /// <summary>
        /// Extracts the header from the image.
        /// </summary>
        /// <param name="image">Image</param>
        /// <returns>Touple containing header info to determine how the image should be decoded</returns>
        public (int channel, int chunkSize, int pixelCount) ExtractHeader(Image<Rgba32> image)
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
