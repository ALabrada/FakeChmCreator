using System;
using System.IO;
using System.Linq;
using CLAP;
using FakeChmCreator.Interop;

namespace FakeChmCreator.CmdLine
{
    public class Tools
    {
        private static int ShowError(int code, string message)
        {
            Console.Error.WriteLine("Error: {0}", message);
            return code;
        }

        private static void ShowWarning(string message)
        {
            Console.WriteLine("Warning: {0}", message);
        }

        private static void PrintTopics(Topic topic, int level = 0)
        {
            Console.WriteLine("{0}-{1}", string.Join("", Enumerable.Range(0, level).Select(i => "  ")), topic.Name);
            foreach (var subTopic in topic.SubTopics)
                PrintTopics(subTopic, level + 1);
        }

        [Verb(IsDefault = true, Description = "Prints the topic tree for the specified Microsoft Word document.")]
        public static int ShowTopics([Required] [Description("Path of the document.")] string docPath,
            [Description("Path of the target HTML file.")] string htmlPath)
        {
            if (string.IsNullOrWhiteSpace(docPath))
                return ShowError(1, "The path of the document cannot be empty.");
            if (!File.Exists(docPath))
                return ShowError(1, "The specified document does not exist.");
            if (htmlPath == null || !Directory.Exists(Path.GetDirectoryName(htmlPath) ?? string.Empty))
            {
                if (htmlPath != null)
                    ShowWarning("Could not find the directory of the target path. Using the default target path.");
                htmlPath = Path.GetTempFileName();
            }
            try
            {
                using (var doc = new DocDocument(docPath))
                {
                    doc.SaveAsHtml(htmlPath);
                }
            }
            catch (Exception exception)
            {
                return ShowError(2, "An unexpected error was handled while converting the document to HTML:\n" + exception);
            }
            var chm = new ChmDocument();
            chm.Load(htmlPath);
            PrintTopics(chm.Content.Root);
            return 0;
        }
    }
}