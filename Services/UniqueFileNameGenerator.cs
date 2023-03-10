using Homework.Services.Abstractions;
using System.Text;

namespace Homework.Services
{
    public class UniqueFileNameGenerator : IFileNameGenerator
    {
        public string GenerateFileName(string? baseFileName = null, string? fileNameExtension = null)
        {
            string fileName;
            StringBuilder fileNameBuilder = new();
            if (!string.IsNullOrWhiteSpace(baseFileName))
            {
                fileNameBuilder.Append(baseFileName)
                               .Append('_');
            }
            fileNameBuilder.Append(Guid.NewGuid().ToString("N"));
            
            fileName = fileNameBuilder.ToString();
            if (!string.IsNullOrWhiteSpace(fileNameExtension))
            {
                fileName = Path.ChangeExtension(fileName, fileNameExtension);
            }

            return fileName;
        }
    }
}