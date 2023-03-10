using Homework.Services.Abstractions;

namespace Homework.Services
{
    public class ProductImageSaver : IFormImageProcessor
    {
        private readonly IWebHostEnvironment environment;
        private readonly IFileNameGenerator fileNameGenerator;
        private readonly SlashFilePathNormalizer pathNormalizer;

        public ProductImageSaver(IWebHostEnvironment environment, IFileNameGenerator fileNameGenerator, SlashFilePathNormalizer pathNormalizer)
        {
            this.environment = environment;
            this.fileNameGenerator = fileNameGenerator;
            this.pathNormalizer = pathNormalizer;
        }

        public string Process(IFormFile formImage)
        {
            string fileName = fileNameGenerator.GenerateFileName(baseFileName: Path.GetFileNameWithoutExtension(formImage.FileName),
                                                                 fileNameExtension: Path.GetExtension(formImage.FileName));

            const string rootDirectory = "media";
            const string imagesDirectory = "Images";
            const string controllerDirectory = "Product";
            string photoWebRelativePath = Path.Combine(rootDirectory, imagesDirectory, controllerDirectory, fileName);

            string photoFullPath = Path.Combine(environment.WebRootPath, photoWebRelativePath);
            using (FileStream fileStream = new(photoFullPath, FileMode.Create, FileAccess.Write))
            {
                using (Stream imageStream = formImage.OpenReadStream())
                    imageStream.CopyTo(fileStream);
            }

            return $"/{pathNormalizer.Normalize(photoWebRelativePath)}";
        }

    }
}