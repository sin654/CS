using HW02;
using SixLabors.ImageSharp.PixelFormats;
using System.Text;
using SixLabors.ImageSharp;
using System.Diagnostics;

Console.OutputEncoding = Encoding.UTF8;

const string inputPath = "../../../../Data/";
const string outputPath = "../../../../Output/";

//var stegoObject = StegoObject.LoadObject(Samples.StringSample(), (s) => Encoding.Default.GetBytes(s)); // OLD
var stegoObject = StegoObject.LoadObject(Samples.StringSample(), (s) => Encoding.UTF8.GetBytes(s));


// Images to encode the stegoObject into
// Each image must be used to encode a part of the stegoObject
var imageNames = new[]
{
    "John_Martin_-_Belshazzar's_Feast.jpg",
    "John_Martin_-_Pandemonium.jpg",
    "John_Martin_-_Sodom_and_Gomorrah.jpg",
    "John_Martin_-_The_Great_Day_of_His_Wrath.jpg",
    "John_Martin_-_The_Last_Judgement.jpg",
    "John_Martin_-_The_Plains_of_Heaven.jpg"
};

// get the chunk of data for each image
List<byte[]> dataChunks = stegoObject.GetDataChunks(imageNames.Count()).ToList();


var stegoImageProcessor = new StegoImageProcessor();
// Do the magic...
// Load the images
Stopwatch stopwatch = new();
List<string> inputPaths = new();
foreach (string imageName in imageNames)
{
    inputPaths.Add(Path.Combine(inputPath, imageName));
}
stopwatch.Start();
Image<Rgba32>[] images = await stegoImageProcessor.LoadImagesAsync(inputPaths.ToArray());
stopwatch.Stop();
Console.WriteLine($"Initial loading of images took {stopwatch.ElapsedMilliseconds} [ms]");

// Encode it
stopwatch.Restart();
Image<Rgba32>[] encodedImages = await stegoImageProcessor.EncodeImagesAsync(images, dataChunks);
stopwatch.Stop();
Console.WriteLine($"Encoding took {stopwatch.ElapsedMilliseconds} [ms]");

// Save the images
stopwatch.Restart();
string[] savedImages = await stegoImageProcessor.SaveImagesAsync(encodedImages, outputPath, imageNames);
stopwatch.Stop();
Console.WriteLine($"Saving took {stopwatch.ElapsedMilliseconds} [ms]");

// Again load the images, but the encoded ones and decode the data
stopwatch.Restart();
Image<Rgba32>[] encodedImagesToDecode = await stegoImageProcessor.LoadImagesAsync(savedImages);
stopwatch.Stop();
Console.WriteLine($"Loading encoded images took {stopwatch.ElapsedMilliseconds} [ms]");

// Decode the data
stopwatch.Restart();
byte[][] decodedDataChunks = await stegoImageProcessor.DecodeImagesAsync(encodedImagesToDecode);
stopwatch.Stop();
Console.WriteLine($"Decoding took {stopwatch.ElapsedMilliseconds} [ms]");

List<byte> decodedBytes = new();
foreach (byte[] dataChunk in decodedDataChunks)
{
    decodedBytes.AddRange(dataChunk);
}

// Print the result
Console.WriteLine(Encoding.UTF8.GetString(decodedBytes.ToArray()));
