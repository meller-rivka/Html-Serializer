using practice2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace practi2
{
    public class Selector
    {
        public string TagName { get; set; }
        public string Id { get; set; }
        public List<string> Classes { get; set; }
        public Selector Parent { get; set; }
        public Selector Child { get; set; }

        public Selector()
        {
            Classes = new List<string>();
        }

        public static Selector QuarySelector(string queryString)
        {
            Selector rootSelector = new Selector();
            Selector currentSelector = rootSelector;
            var parts = Regex.Split(queryString, @"(?=[# .])");
            foreach (var part in parts)
            {
                if (!string.IsNullOrWhiteSpace(part))
                {
                    if (part.StartsWith("#"))
                    {
                        currentSelector.Id = part.Substring(1);
                    }
                    else if (part.StartsWith("."))
                    {
                        currentSelector.Classes.Add(part.Substring(1));
                    }
                    else if(HtmlHelper.Instance.GetSelfClosingHtmlTags().Contains(part) ||
                        HtmlHelper.Instance.GetAllHtmlTags().Contains(part))
                    {
                        currentSelector.TagName = part;
                    }

                    Selector newSelector = new Selector();
                    currentSelector.Child = newSelector;
                    newSelector.Parent = currentSelector;
                    currentSelector = newSelector;
                }
            }

            return rootSelector;
        }


        public bool IsMatch(HtmlElement element)
        {
            if (element == null)
            {
                return false;
            }

            // Check if the element matches the selector criteria
            bool tagNameMatch = string.IsNullOrEmpty(TagName) || element.Name == TagName;
            bool idMatch = string.IsNullOrEmpty(Id) || element.Id?.ToString() == Id;
            bool classesMatch = Classes.All(className => element.Classes.Contains(className));

            return tagNameMatch && idMatch && classesMatch;
        }

        public static List<HtmlElement> SearchElementsBySelector(HtmlElement root, Selector selector)
        {
            List<HtmlElement> result = new List<HtmlElement>();
            HashSet<HtmlElement> visited = new HashSet<HtmlElement>();

            SearchElements(root, selector, result, visited);

            return result;
        }

        private static void SearchElements(HtmlElement element, Selector selector, List<HtmlElement> result, HashSet<HtmlElement> visited)
        {
            if (element == null || visited.Contains(element))
            {
                return;
            }

            visited.Add(element);

            // Check if the element matches the selector criteria
            if (selector.IsMatch(element))
            {
                result.Add(element);
            }

            // Recursively search in children
            foreach (HtmlElement child in element.Children)
            {
                SearchElements(child, selector, result, visited);
            }

        }
        
    }
}
