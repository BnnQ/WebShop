using Homework.Utils;

namespace Homework
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.ConfigureServices();
            var app = builder.Build();

            app.Configure();
            app.Run();
        }
    }
}