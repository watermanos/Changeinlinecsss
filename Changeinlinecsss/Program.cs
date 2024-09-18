using HtmlAgilityPack;
using System;
using System.Drawing;
using System.IO;
using static System.Net.Mime.MediaTypeNames;

class Program
{
    static void Main()
    {
        // Ζήτα από τον χρήστη να εισάγει το path του φακέλου
        Console.WriteLine("Please enter the path of the folder containing the HTML files:");
        string folderPath = Console.ReadLine();

        // Έλεγξε αν το path υπάρχει
        if (Directory.Exists(folderPath))
        {
            // Πάρε όλα τα αρχεία .html από τον φάκελο
            string[] htmlFiles = Directory.GetFiles(folderPath, "*.html");

            if (htmlFiles.Length > 0)
            {
                foreach (string htmlPath in htmlFiles)
                {
                    Console.WriteLine($"Edit file: {htmlPath}");

                    // Διαβάστε το περιεχόμενο του HTML αρχείου
                    string htmlContent = File.ReadAllText(htmlPath);

                    // Φορτώστε το HTML περιεχόμενο στο HtmlDocument
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(htmlContent);

                    // Κάντε τις τροποποιήσεις στο HTML
                    ModifyPageNumberElements(htmlDoc);
                    ModifySideNumberElements(htmlDoc);
                    ModifyTitleElements(htmlDoc);
                    Modifypraktika(htmlDoc);
                    Modifykt(htmlDoc);
                    ModifyKefalaio(htmlDoc);
                    RemoveAnchorTags(htmlDoc);
                    RemoveFootnotes(htmlDoc);
                    RemoveBreakTags(htmlDoc);

                    // Αποθηκεύστε το τροποποιημένο HTML πίσω στο αρχείο
                    File.WriteAllText(htmlPath, htmlDoc.DocumentNode.OuterHtml);

                    Console.WriteLine($"File Edit: File {htmlPath} updated.");
                }

                Console.WriteLine("Done.");
            }
            else
            {
                Console.WriteLine("The folder does not contain HTML files.");
            }
        }
        else
        {
            Console.WriteLine("The folder you entered does not exist.");
        }
    }

    // Μέθοδος που τροποποιεί τα στοιχεία με class="PAGENUMBER"
    private static void ModifyPageNumberElements(HtmlDocument htmlDoc)
    {
        var nodes = htmlDoc.DocumentNode.SelectNodes("//*[@class='PAGENUMBER']");

        if (nodes != null)
        {
            foreach (var node in nodes)
            {
                // Ορίστε το inline style
                node.SetAttributeValue("style", "text-align: center; font-size:12px;");

                // Προσθέστε το "Σελ. " πριν από το υπάρχον κείμενο (αν υπάρχει)
                string existingText = node.InnerText.Trim();
                node.InnerHtml = "Σελ. " + existingText;
            }
        }
    }

    // Μέθοδος που τροποποιεί τα στοιχεία με class="SIDENUMBER"
    private static void ModifySideNumberElements(HtmlDocument htmlDoc)
    {
        var nodes = htmlDoc.DocumentNode.SelectNodes("//*[@class='SIDENUMBER']");

        if (nodes != null)
        {
            foreach (var node in nodes)
            {
                // Ορίστε το inline style
                node.SetAttributeValue("style", "font-weight: bold;");


            }
        }
    }

    // Μέθοδος που τροποποιεί τα στοιχεία με class="praktika"
    private static void Modifypraktika(HtmlDocument htmlDoc)
    {
        var nodes = htmlDoc.DocumentNode.SelectNodes("//*[@class='PRAKTIKO' or @class='APANTHSH-TITLOS' or @class='x--------- x---------']");

        if (nodes != null)
        {
            foreach (var node in nodes)
            {
                // Ορίστε το inline style
                node.SetAttributeValue("style", "font - size:18px; font-weight: bold;");


            }
        }
    }

    // Μέθοδος που τροποποιεί τα στοιχεία με class="KEFALAIO-TITLO"
    private static void Modifykt(HtmlDocument htmlDoc)
    {
        var nodes = htmlDoc.DocumentNode.SelectNodes("//*[@class='KEFALAIO-TITLOS']");

        if (nodes != null)
        {
            foreach (var node in nodes)
            {
                // Ορίστε το inline style
                node.SetAttributeValue("style", "font - size:18px; text-align:center; font-weight: bold;");


            }
        }
    }

    // Μέθοδος που τροποποιεί τα στοιχεία με class="Kefalaio"
    private static void ModifyKefalaio(HtmlDocument htmlDoc)
    {
        var nodes = htmlDoc.DocumentNode.SelectNodes("//*[@class='KEFALAIO']");

        if (nodes != null)
        {
            foreach (var node in nodes)
            {
                // Ορίστε το inline style
                node.SetAttributeValue("style", "font - size:18px; text-align:center;");


            }
        }
    }

    // Μέθοδος που τροποποιεί τα στοιχεία με class="H01-Titlos"
    private static void ModifyTitleElements(HtmlDocument htmlDoc)
    {
        var allNodes = htmlDoc.DocumentNode.Descendants();

        foreach (var node in allNodes)
        {
            bool containsTitlosOrTitle = false;

            // Έλεγχος στο InnerText (κείμενο του κόμβου)
            if (!string.IsNullOrEmpty(node.InnerText) &&
                (node.InnerText.Contains("Titlos") || node.InnerText.Contains("Title")))
            {
                containsTitlosOrTitle = true;
            }

            // Έλεγχος στα attributes (π.χ. class, id, name, κ.λπ.)
            foreach (var attribute in node.Attributes)
            {
                if (attribute.Value.Contains("Titlos") || attribute.Value.Contains("Title"))
                {
                    containsTitlosOrTitle = true;
                    break;
                }
            }

            // Αν βρέθηκε το "Titlos" ή "Title", τροποποίησε το style
            if (containsTitlosOrTitle)
            {
                // Έλεγξε αν υπάρχει ήδη το style attribute, αν ναι, πρόσθεσε το νέο στυλ
                if (node.Attributes["style"] != null)
                {
                    node.Attributes["style"].Value += " font-weight: bold; font-size:18px;";
                }
                else
                {
                    // Αν δεν υπάρχει το style, πρόσθεσέ το
                    
                    node.SetAttributeValue("style", "font-weight: bold; font-size:18px;");
                }
            }
        }
    }



    // Μέθοδος που αφαιρεί τα <a> tags
    private static void RemoveAnchorTags(HtmlDocument htmlDoc)
    {
        // Βρείτε όλα τα <a> tags
        var anchorNodes = htmlDoc.DocumentNode.SelectNodes("//a");

        if (anchorNodes != null)
        {
            foreach (var node in anchorNodes)
            {
                // Αντικαταστήστε το <a> με το κείμενο που περιέχει
                var parentNode = node.ParentNode;
                if (parentNode != null)
                {
                    parentNode.ReplaceChild(HtmlNode.CreateNode(node.InnerHtml), node);
                }
            }
        }
    }

    // Μέθοδος που αφαιρεί τα footnotes
    private static void RemoveFootnotes(HtmlDocument htmlDoc)
    {
        // Βρείτε όλα τα στοιχεία με οποιοδήποτε attribute που περιέχει τη λέξη "ekthetis"
        var nodesWithEkthetis = htmlDoc.DocumentNode.SelectNodes("//*");

        if (nodesWithEkthetis != null)
        {
            foreach (var node in nodesWithEkthetis)
            {
                // Ελέγχουμε όλα τα attributes του κόμβου
                foreach (var attribute in node.Attributes)
                {
                    // Εάν το attribute περιέχει τη λέξη "ekthetis", αφαιρούμε τον κόμβο
                    if (attribute.Value.Contains("ekthetis") || attribute.Value.Contains("ektheths") || attribute.Value.Contains("Ekthetis") || attribute.Value.Contains("Ektheths"))
                    {
                        var parentNode = node.ParentNode;
                        if (parentNode != null)
                        {
                            parentNode.RemoveChild(node);
                            break; // Προχωράμε στον επόμενο κόμβο, αφού βρήκαμε το attribute που θέλουμε
                        }
                    }
                }
            }
        }
    }
    // Μέθοδος που αφαιρεί τα <br> tags
    private static void RemoveBreakTags(HtmlDocument htmlDoc)
    {
        // Βρείτε όλα τα <br> tags (με διάφορες μορφές)
        var brNodes = htmlDoc.DocumentNode.SelectNodes("//br");

        if (brNodes != null)
        {
            foreach (var node in brNodes)
            {
                // Αφαιρέστε το <br> tag
                var parentNode = node.ParentNode;
                if (parentNode != null)
                {
                    parentNode.RemoveChild(node);
                }
            }
        }
    }
}
