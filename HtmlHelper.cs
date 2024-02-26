using System.IO;
using System.Text.Json;

namespace practice2
{
    public class HtmlHelper
    {
        private readonly static HtmlHelper _instance = new HtmlHelper();

        public static HtmlHelper Instance => _instance;

        private string[] allHtmlTags;
        private string[] selfClosingHtmlTags;

        private HtmlHelper()
        {
            // Load data from JSON files
            string htmlTagsFilePath = "HtmlTags.json";
            string selfClosingHtmlTagsFilePath = "HtmlVoidTags.json";
            LoadHtmlTags(htmlTagsFilePath);
            LoadSelfClosingHtmlTags(selfClosingHtmlTagsFilePath);
        }

        private void LoadHtmlTags(string filePath)
        {
            // Read JSON data from the file
            string json = File.ReadAllText(filePath);

            // Deserialize JSON array to string array using JsonSerializer
            allHtmlTags = JsonSerializer.Deserialize<string[]>(json);
        }

        private void LoadSelfClosingHtmlTags(string filePath)
        {
            // Read JSON data from the file
            string json = File.ReadAllText(filePath);

            // Deserialize JSON array to string array using JsonSerializer
            selfClosingHtmlTags = JsonSerializer.Deserialize<string[]>(json);
        }

        public string[] GetAllHtmlTags()
        {
            return allHtmlTags;
        }

        public string[] GetSelfClosingHtmlTags()
        {
            return selfClosingHtmlTags;
        }
    }
}
