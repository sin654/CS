using HW02;
using SixLabors.ImageSharp.PixelFormats;
using System.Text;
using SixLabors.ImageSharp;

const string inputPath = "../../../../Data/";
const string outputPath = "../../../../Output/";

var stegoObject = StegoObject.LoadObject(Samples.StringSample(), (s) => Encoding.Default.GetBytes(s));

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

// Do the magic...
// load the images

// testing
var input = "The quick brown fox jumps over the lazy dog";
var inputBytes = Encoding.Default.GetBytes(input);

var inputImage = await Image.LoadAsync<Rgba32>(Path.Combine(inputPath, "Coat_of_arms_of_Bruntal.jpg"));

var stegoImageProcessor = new StegoImageProcessor();

var stegoImage = await stegoImageProcessor.EncodePayload(inputImage, inputBytes);
var outputBytes = await stegoImageProcessor.ExtractPayload(stegoImage);

var output = Encoding.Default.GetString(outputBytes);
Console.WriteLine(output);

//Assert.Equal(input, output);

// Encode it
// Decode it
byte[] decodedData = Array.Empty<byte>();

Console.WriteLine(Encoding.Default.GetString(decodedData));
