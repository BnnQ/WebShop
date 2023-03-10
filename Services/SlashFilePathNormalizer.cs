using System.Text.RegularExpressions;

namespace Homework.Services
{
    public class SlashFilePathNormalizer
    {
        public string Normalize(string path)
        {
            string pattern1 = @"^\\+";
            string pattern2 = @"\\";
            string replacement1 = string.Empty;
            string replacement2 = "/";
            string result = Regex.Replace(path, pattern1, replacement1);
            result = Regex.Replace(result, pattern2, replacement2);

            return result;
        }
    }
}