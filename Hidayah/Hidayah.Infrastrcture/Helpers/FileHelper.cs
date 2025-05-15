using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hidayah.Infrastrcture.Helpers
{
    public static class FileHelper
    {
        // Allowed file extensions
        private static readonly List<string> AllowedAudioExtensions = new List<string> { ".mp3", ".wav", ".ogg" };
        private static readonly List<string> AllowedSvgExtensions = new List<string> { ".svg" };

        public static async Task<string> SaveFileAsync(IFormFile file, string subFolder, string fileNameWithoutExt = null, string fileType = null)
        {
            if (file == null || file.Length == 0)
                return null;

            var extension = Path.GetExtension(file.FileName).ToLower();

            // Validate file type
            if (fileType == "audio" && !AllowedAudioExtensions.Contains(extension))
            {
                throw new InvalidOperationException("Only audio files (.mp3, .wav, .ogg) are allowed.");
            }

            if (fileType == "svg" && !AllowedSvgExtensions.Contains(extension))
            {
                throw new InvalidOperationException("Only SVG files (.svg) are allowed.");
            }

            var sanitizedName = fileNameWithoutExt ?? Guid.NewGuid().ToString("N");
            sanitizedName = Path.GetInvalidFileNameChars().Aggregate(sanitizedName, (current, c) => current.Replace(c, '_'));

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", subFolder);
            Directory.CreateDirectory(folderPath);

            string fileName = $"{sanitizedName}{extension}";
            string fullPath = Path.Combine(folderPath, fileName);

            // Check if the file exists, and append a timestamp if necessary
            if (System.IO.File.Exists(fullPath))
            {
                var uniqueSuffix = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff"); // or use Guid.NewGuid()
                fileName = $"{sanitizedName}_{uniqueSuffix}{extension}";
                fullPath = Path.Combine(folderPath, fileName);
            }

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Path.Combine(subFolder, fileName).Replace("\\", "/");
        }
    }
}
