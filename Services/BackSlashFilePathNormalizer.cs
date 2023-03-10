using System.Text.RegularExpressions;

namespace Homework.Services
{
    public class BackSlashFilePathNormalizer
    {
        public string Normalize(string path)
        {
            string pattern1 = @"(^|[^a-zA-Z/])/";
            string pattern2 = "/";
            string replacement1 = "$1";
            string replacement2 = "\\";
            string result = Regex.Replace(path, pattern1, replacement1);
            result = Regex.Replace(result, pattern2, replacement2);
            
            return result;
        }
    }
}