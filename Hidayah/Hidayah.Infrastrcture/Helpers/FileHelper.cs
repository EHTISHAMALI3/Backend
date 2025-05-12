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

            // Validate file type based on field
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

            var fileName = $"{sanitizedName}{extension}";
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", subFolder);
            Directory.CreateDirectory(folderPath);

            var fullPath = Path.Combine(folderPath, fileName);

            // Save the file to the folder
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return the relative path (used in the database)
            return Path.Combine(subFolder, fileName).Replace("\\", "/");
        }
    }
}
