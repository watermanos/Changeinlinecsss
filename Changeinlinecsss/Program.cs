using HtmlAgilityPack;
using System;
using System.IO;

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
        // Βρείτε όλα τα span στοιχεία με κλάσεις Ekthetis ή ekthetis
        var footnoteNodes = htmlDoc.DocumentNode.SelectNodes("//span[@class='Ekthetis' or @class='ekthetis']");

        if (footnoteNodes != null)
        {
            foreach (var node in footnoteNodes)
            {
                // Αφαιρέστε ολόκληρο το span element που περιέχει το footnote
                var parentNode = node.ParentNode;
                if (parentNode != null)
                {
                    parentNode.RemoveChild(node);
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
