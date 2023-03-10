namespace Homework.Services.Abstractions
{
    public interface IFormImageProcessor
    {
        public string Process(IFormFile formImage);
    }
}