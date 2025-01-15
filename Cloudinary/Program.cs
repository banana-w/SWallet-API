// Import the required packages
//==============================

using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using dotenv.net;

// Set your Cloudinary credentials
//=================================


DotEnv.Load(options: new DotEnvOptions(probeForEnv: true));
Console.WriteLine("CLOUDINARY_URL: " + Environment.GetEnvironmentVariable("CLOUDINARY_URL"));
Cloudinary cloudinary = new Cloudinary(Environment.GetEnvironmentVariable("CLOUDINARY_URL"));
cloudinary.Api.Secure = true;



// Upload an image and log the response to the console
//=================

// Upload an image and log the response to the console
//=================

var uploadParams = new ImageUploadParams()
{
    File = new FileDescription(@"https://cloudinary-devs.github.io/cld-docs-assets/assets/images/cld-sample.jpg"),
    UseFilename = true,
    UniqueFilename = false,
    Overwrite = true
};
var uploadResult = cloudinary.Upload(uploadParams);
Console.WriteLine(uploadResult.JsonObj);


