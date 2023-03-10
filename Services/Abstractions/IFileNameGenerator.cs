namespace Homework.Services.Abstractions
{
    public interface IFileNameGenerator
    {
        public string GenerateFileName(string? baseFileName = null, string? fileNameExtension = null);
    }
}