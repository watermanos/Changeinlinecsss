using HtmlAgilityPack;
using System;
using System.IO;
using System.Linq;

class Program
{
    static void Main()
    {
        Console.WriteLine("Please enter the path of the folder containing the HTML files:");
        string folderPath = Console.ReadLine();

        // Έλεγχος φακέλου
        if (!Directory.Exists(folderPath))
        {
            Console.WriteLine("The folder does not exist.");
            return;
        }

        // Παίρνουμε όλα τα html αρχεία
        string[] htmlFiles = Directory.GetFiles(folderPath, "*.html");

        if (htmlFiles.Length == 0)
        {
            Console.WriteLine("No HTML files found.");
            return;
        }

        foreach (string htmlPath in htmlFiles)
        {
            Console.WriteLine($"\nEditing file: {htmlPath}");

            // Read HTML
            string htmlContent = File.ReadAllText(htmlPath);

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlContent);

            
            ModifyPageNumberElements(htmlDoc);

           
            ApplyCustomStyles(htmlDoc);

          
            ModifyTitleElements(htmlDoc);
            ModifyDir(htmlDoc);
            RemoveAnchorTags(htmlDoc);
            RemoveFootnotes(htmlDoc);
            RemoveBreakTags(htmlDoc);
            ReplaceListTags(htmlDoc);

            // Save
            File.WriteAllText(htmlPath, htmlDoc.DocumentNode.OuterHtml);

            Console.WriteLine($"File updated: {htmlPath}");
        }

        Console.WriteLine("\nDone.");
    }

  
    // PAGE NUMBER
   
    private static void ModifyPageNumberElements(HtmlDocument htmlDoc)
    {
        var nodes = htmlDoc.DocumentNode
            .SelectNodes("//*[@class='PAGENUMBER']");

        if (nodes != null)
        {
            foreach (var node in nodes)
            {
                node.SetAttributeValue(
                    "style",
                    "text-align:center; font-size:12px;"
                );

                string existingText = node.InnerText.Trim();

                node.InnerHtml = "Σελ. " + existingText;
            }

            Console.WriteLine($"PAGENUMBER updated ({nodes.Count} nodes).");
        }
    }


    // DYNAMIC CSS ENGINE
  
    private static void ApplyCustomStyles(HtmlDocument htmlDoc)
    {
        Console.WriteLine("\n=== CUSTOM STYLE MODE ===");
        Console.WriteLine("Type EXIT to stop adding styles.\n");

        while (true)
        {
            Console.WriteLine("Enter class name:");
            string className = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(className))
            {
                Console.WriteLine("Class name cannot be empty.");
                continue;
            }

            if (className.Trim().ToUpper() == "EXIT")
                break;

            Console.WriteLine($"Enter CSS style for '{className}':");

            string style = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(style))
            {
                Console.WriteLine("Style cannot be empty.");
                continue;
            }

            var nodes = htmlDoc.DocumentNode
                .SelectNodes($"//*[@class='{className}']");

            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    node.SetAttributeValue("style", style);
                }

                Console.WriteLine(
                    $"Applied style to {nodes.Count} node(s).");
            }
            else
            {
                Console.WriteLine(
                    $"No nodes found for class '{className}'.");
            }
        }
    }

    // TITLE / TITLOS
    
    private static void ModifyTitleElements(HtmlDocument htmlDoc)
    {
        var allNodes = htmlDoc.DocumentNode.Descendants();

        foreach (var node in allNodes)
        {
            bool containsTitle = false;

            // InnerText check
            if (!string.IsNullOrEmpty(node.InnerText) &&
                (node.InnerText.Contains("Titlos") ||
                 node.InnerText.Contains("Title")))
            {
                containsTitle = true;
            }

            // Attribute check
            foreach (var attribute in node.Attributes)
            {
                if (attribute.Value.Contains("Titlos") ||
                    attribute.Value.Contains("Title"))
                {
                    containsTitle = true;
                    break;
                }
            }

            // Apply style
            if (containsTitle &&
                node.NodeType == HtmlNodeType.Element)
            {
                if (node.Attributes["style"] != null)
                {
                    node.Attributes["style"].Value +=
                        " font-weight:bold; font-size:18px;";
                }
                else
                {
                    node.SetAttributeValue(
                        "style",
                        "font-weight:bold; font-size:18px;"
                    );
                }
            }
        }
    }

    // RTL -> LTR
  
    private static void ModifyDir(HtmlDocument htmlDoc)
    {
        var nodes = htmlDoc.DocumentNode
            .SelectNodes("//*[@dir='rtl']");

        if (nodes != null)
        {
            foreach (var node in nodes)
            {
                node.SetAttributeValue("dir", "ltr");
            }

            Console.WriteLine($"RTL changed to LTR ({nodes.Count} nodes).");
        }
    }

    
    // REMOVE <a>
  
    private static void RemoveAnchorTags(HtmlDocument htmlDoc)
    {
        var anchorNodes = htmlDoc.DocumentNode
            .SelectNodes("//a");

        if (anchorNodes != null)
        {
            foreach (var node in anchorNodes.ToList())
            {
                var parentNode = node.ParentNode;

                if (parentNode != null)
                {
                    parentNode.ReplaceChild(
                        HtmlNode.CreateNode(node.InnerHtml),
                        node
                    );
                }
            }

            Console.WriteLine($"Removed {anchorNodes.Count} anchor tags.");
        }
    }

    
    // REMOVE FOOTNOTES
   
    private static void RemoveFootnotes(HtmlDocument htmlDoc)
    {
        var nodes = htmlDoc.DocumentNode.SelectNodes("//*");

        int removedCount = 0;

        if (nodes != null)
        {
            foreach (var node in nodes.ToList())
            {
                foreach (var attribute in node.Attributes)
                {
                    string value = attribute.Value.ToLower();

                    if (value.Contains("ekthetis") ||
                        value.Contains("ektheths"))
                    {
                        node.ParentNode?.RemoveChild(node);

                        removedCount++;
                        break;
                    }
                }
            }
        }

        Console.WriteLine($"Removed {removedCount} footnote nodes.");
    }

    
    // REMOVE <br>
    
    private static void RemoveBreakTags(HtmlDocument htmlDoc)
    {
        var brNodes = htmlDoc.DocumentNode
            .SelectNodes("//br");

        if (brNodes != null)
        {
            foreach (var node in brNodes.ToList())
            {
                node.ParentNode?.RemoveChild(node);
            }

            Console.WriteLine($"Removed {brNodes.Count} <br> tags.");
        }
    }

    
    // REPLACE LIST TAGS
  
    private static void ReplaceListTags(HtmlDocument htmlDoc)
    {
        // LI -> P
        var liNodes = htmlDoc.DocumentNode
            .SelectNodes("//li");

        if (liNodes != null)
        {
            foreach (var node in liNodes.ToList())
            {
                var newNode = htmlDoc.CreateElement("p");

                newNode.InnerHtml = node.InnerHtml;

                if (node.Attributes["class"] != null)
                {
                    newNode.SetAttributeValue(
                        "class",
                        node.Attributes["class"].Value
                    );
                }

                node.ParentNode.ReplaceChild(newNode, node);
            }

            Console.WriteLine($"Converted {liNodes.Count} li tags to p.");
        }

        // REMOVE OL
        var olNodes = htmlDoc.DocumentNode
            .SelectNodes("//ol");

        if (olNodes != null)
        {
            foreach (var node in olNodes.ToList())
            {
                var parent = node.ParentNode;

                if (parent == null)
                    continue;

                foreach (var child in node.ChildNodes.ToList())
                {
                    parent.InsertBefore(child, node);
                }

                parent.RemoveChild(node);
            }

            Console.WriteLine($"Removed {olNodes.Count} ol tags.");
        }
    }
}