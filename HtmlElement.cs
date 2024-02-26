
using Microsoft.VisualBasic;
using Newtonsoft.Json.Linq;
using practice2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace practi2
{
    public class HtmlElement
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> Attributes { get; set; } 
        public List<string> Classes { get; set; }
        public string InnerHtml { get; set; }
        public HtmlElement Parent { get; set; }
        public List<HtmlElement> Children { get; set; }
        public HtmlElement()
        {
            Attributes = new Dictionary<string, string>();
            Classes = new List<string>();
            Children = new List<HtmlElement>();
        }

        public HtmlElement BuildElementTree(List<string> tags)
        {
            HtmlElement root = new HtmlElement();
            HtmlElement currentElement = root;
            Stack<HtmlElement> stack = new Stack<HtmlElement>();

            foreach (string tag in tags)
            {
                string[] includeTag = tag.Split(' ');

                if (string.IsNullOrEmpty(tag))
                {
                    continue;
                }
                if (includeTag[0].Equals("html"))
                {
                    root.Name = "html";
                    string[] parts = tag.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
                    string attributesString = parts.Length > 1 ? parts[1].Trim() : "";
                    Dictionary<string, string> attributes = ParseAttributes(attributesString);
                    UpdateClasses(root, attributes);
                    root.Attributes = attributes;
                    currentElement = root;
                    continue;
                }
                if (includeTag[0].StartsWith("!DOCTYPE") || includeTag[0].Equals("/html") )
                {
                    continue;
                }
                if (tag.StartsWith("/") && HtmlHelper.Instance.GetSelfClosingHtmlTags().Contains(includeTag[0]))
                {
                    // Closing tag for self-closing element
                    currentElement = stack.Pop();
                }
                else if (tag.StartsWith("/"))
                {
                    // Closing tag
                    currentElement = currentElement.Parent;
                }
                else if (HtmlHelper.Instance.GetAllHtmlTags().Contains(includeTag[0]))
                {
                    // Opening tag
                    HtmlElement newElement = new HtmlElement();
                    string[] parts = tag.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
                    newElement.Name = parts[0];
                    string attributesString = parts.Length > 1 ? parts[1].Trim() : "";
                    Dictionary<string, string> attributes = ParseAttributes(attributesString);

                    // Update classes
                    UpdateClasses(newElement, attributes);
                    newElement.Attributes = attributes;
                    newElement.Parent = currentElement;
                    currentElement.Children.Add(newElement);
                    currentElement = newElement;
                   
                }
                else if(HtmlHelper.Instance.GetSelfClosingHtmlTags().Contains(includeTag[0]))
                {
                    HtmlElement newElement = new HtmlElement();
                    string[] parts = tag.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
                    newElement.Name = parts[0];
                    string attributesString = parts.Length > 1 ? parts[1].Trim() : "";
                    Dictionary<string, string> attributes = ParseAttributes(attributesString);

                    UpdateClasses(newElement, attributes);
                    newElement.Attributes = attributes;
                    newElement.Parent = currentElement;
                    currentElement.Children.Add(newElement);
                }
                else
                {
                    currentElement.InnerHtml += tag;
                }
            }

            return root;
        }


        private static void UpdateClasses(HtmlElement element, Dictionary<string,string> attributes)
        {
            foreach (var (key,value) in attributes)
            {
                
                if (key.ToLower() == "class")
                {
                    attributes.Remove(key);
                    string[] classes = value.Split(' ');
                    foreach (string className in classes)
                    {
                        if (!string.IsNullOrEmpty(className))
                        {
                            element.Classes.Add(className);
                        }
                    }
                }
                else if (key.ToLower() == "id")
                {
                    attributes.Remove(key);
                    element.Id = value;
                }
            }
        }

        private static Dictionary<string,string> ParseAttributes(string attributes)
        {
            Dictionary<string,string> result = new Dictionary<string,string>();
            string pattern = @"(\w+)\s*=\s*""([^""]*)""";

            foreach (Match match in Regex.Matches(attributes, pattern))
            {
                string attributeName = match.Groups[1].Value.ToLower();
                string attributeValue = match.Groups[2].Value;
                result.Add(attributeName,attributeValue);
            }

            return result;
        }

        public IEnumerable<HtmlElement> Descendants()
        {
            Queue<HtmlElement> queue = new Queue<HtmlElement>();
            queue.Enqueue(this);

            while (queue.Count > 0)
            {
                HtmlElement element = queue.Dequeue();
                yield return element;

                foreach (HtmlElement child in element.Children)
                {
                    queue.Enqueue(child);
                }
            }
        }

        public IEnumerable<HtmlElement> Ancestors()
        {
            HtmlElement current = this.Parent;

            while (current != null)
            {
                yield return current;
                current = current.Parent;
            }
        }
    }
}

