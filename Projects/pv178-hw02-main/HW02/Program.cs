using HW02;
using SixLabors.ImageSharp.PixelFormats;
using System.Text;
using SixLabors.ImageSharp;

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
List<string> inputPaths = new();
foreach (string imageName in imageNames)
{
    inputPaths.Add(Path.Combine(inputPath, imageName));
}
Image<Rgba32>[] images = await stegoImageProcessor.LoadImagesAsync(inputPaths.ToArray());

// Encode it
Image<Rgba32>[] encodedImages = await stegoImageProcessor.EncodeImagesAsync(images, dataChunks);

// Save the images
string[] savedImages = await stegoImageProcessor.SaveImagesAsync(encodedImages, outputPath, imageNames);

// Again load the images, but the encoded ones and decode the data
Image<Rgba32>[] encodedImagesToDecode = await stegoImageProcessor.LoadImagesAsync(savedImages);

// Decode the data
byte[][] decodedDataChunks = await stegoImageProcessor.DecodeImagesAsync(encodedImagesToDecode);

List<byte> decodedBytes = new();
foreach (byte[] dataChunk in decodedDataChunks)
{
    decodedBytes.AddRange(dataChunk);
}

// Print the result
Console.WriteLine(Encoding.UTF8.GetString(decodedBytes.ToArray()));
