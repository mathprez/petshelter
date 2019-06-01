using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace petshelterApi.Services
{
    public class ImageSaver
    {
        public async Task<Domain.Image> SaveImage(Stream input)
        {
            byte[] bytes = await GetBytes(input);
            Image image = GetImage(bytes);

            if (!ValidateImageFormat(image, out var format))
            {
                throw new ArgumentException();
            }

            return new Domain.Image()
            {
                Data = bytes,
                ContentType = format.ToString().ToLower(),
                Name = "maaktnuniuit"
            };
        }

        private static Image GetImage(byte[] bytes)
        {
            Image image;
            using (var imageStream = new MemoryStream(bytes))
            {
                image = Image.FromStream(imageStream);
            }

            return image;
        }

        private static async Task<byte[]> GetBytes(Stream input)
        {
            byte[] bytes;
            using (var stream = new MemoryStream())
            {
                await input.CopyToAsync(stream);
                bytes = stream.ToArray();
            }

            return bytes;
        }
        
        private static bool ValidateImageFormat(Image image, out ImageFormat format)
        {
            format = null;
            if (ImageFormat.Jpeg.Equals(image.RawFormat))
            {
                format = ImageFormat.Jpeg;
                return true;
            }
            if (ImageFormat.Png.Equals(image.RawFormat))
            {
                format = ImageFormat.Png;
                return true;
            }
            if (ImageFormat.Gif.Equals(image.RawFormat))
            {
                format = ImageFormat.Gif;
                return true;
            }
            if (ImageFormat.Bmp.Equals(image.RawFormat))
            {
                format = ImageFormat.Bmp;
                return true;
            }
            return false;
        }
    }
}
