
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using practi2;
using System.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using practice2;

var html = await Load("https://learn.malkabruk.co.il/");
var cleanHtml = new Regex("\\s\\s").Replace(html, " ");
string pattern_remove_tags = "[<>]";
List<string> listTags = new List<string>();
IEnumerable<string> tags = GetHtmlTags(cleanHtml);
foreach (string tag in tags)
{
    // Replace < and > characters in the current string
    string modifiedTag = Regex.Replace(tag, pattern_remove_tags, "");
    listTags.Add(modifiedTag);
}

HtmlElement root = new HtmlElement().BuildElementTree(listTags);

// Define a query string
string queryString = "input";

// Convert the query string to a Selector object
Selector selector = Selector.QuarySelector(queryString);

// Search for HTML elements based on the selector
List<HtmlElement> result = Selector.SearchElementsBySelector(root, selector);

// Print the results
Console.WriteLine("Matching HTML elements:");
foreach (var element in result)
{
    Console.WriteLine($"Element Name: {element.Name}, Id: {element.Id}, Classes: [{string.Join(", ", element.Classes)}]");
}
var tree=root.Descendants();
foreach (var item in tree)
{
    Console.WriteLine(item.Name);
}

static IEnumerable<string> GetHtmlTags(string html)
{
    List<string> tags = new List<string>();

    // Define the regular expression pattern to match HTML tags
    string pattern = @"<[^<>]+>|[^<]+";

    // Match the pattern in the HTML string
    MatchCollection matches = Regex.Matches(html, pattern);

    // Iterate through the matches and add them to the list
    foreach (Match match in matches)
    {
        string tag = match.Value.Trim();
        if (!string.IsNullOrEmpty(tag))
        {
            tags.Add(tag);
        }
    }

    return tags;
}
async Task<string> Load(string url)
{
    HttpClient client = new HttpClient();
    var response = await client.GetAsync(url);
    var html = await response.Content.ReadAsStringAsync();
    return html;
}

