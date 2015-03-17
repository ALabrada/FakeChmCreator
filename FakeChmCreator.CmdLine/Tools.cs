using System;
using System.Collections.Generic;
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

        private static ChmDocument DocToChm(string docPath, string htmlPath, bool keepHtml = false, bool keepResources = false)
        {
            var isTempFile = false;
            if (htmlPath == null || !Directory.Exists(Path.GetDirectoryName(htmlPath) ?? string.Empty))
            {
                if (htmlPath != null)
                    ShowWarning("Could not find the directory of the target path. Using the default target path.");
                htmlPath = Path.GetTempFileName();
                isTempFile = true;
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
                ShowError(2, "An unexpected error was handled while converting the document to HTML:\n" + exception);
                return null;
            }
            var chm = new ChmDocument();
            chm.Load(htmlPath);
            var resDir = string.Format("{0}_files", Path.GetFileNameWithoutExtension(htmlPath));
            var parentDir = Path.GetDirectoryName(htmlPath);
            if (parentDir != null)
                resDir = Path.Combine(parentDir, resDir);
            if (isTempFile || !keepHtml)
                File.Delete(htmlPath);
            if ((isTempFile || !keepResources) && Directory.Exists(resDir))
                Directory.Delete(resDir, true);
            return chm;
        }

        [Verb(IsDefault = true, Description = "Prints the topic tree for the specified Microsoft Word document.", Aliases = "show")]
        public static int ShowTopics([Required] [Description("Path of the document.")] string docPath,
            [Description("Path of the target HTML file.")] string htmlPath)
        {
            if (string.IsNullOrWhiteSpace(docPath))
                return ShowError(1, "The path of the document cannot be empty.");
            if (!File.Exists(docPath))
                return ShowError(1, "The specified document does not exist.");
            var chm = DocToChm(docPath, htmlPath, true);
            if (chm == null)
                return 2;
            PrintTopics(chm.Content.Root);
            return 0;
        }

        private static void SaveTopics(Topic topic, string dirPath, Stack<int> levels)
        {
            var fileName = string.Format("H{0}.html", string.Join(".", from i in levels.Reverse() select i + 1),
                topic.Name);
            if (levels.Count > 0)
            {
                var filePath = Path.Combine(dirPath, fileName);
                topic.Content.Save(filePath);
                Console.WriteLine("Saved {0} ({1}).", fileName, topic.Name);
            }
            foreach (var item in topic.SubTopics.Select((t, i) => new {Topic = t, Index = i}))
            {
                levels.Push(item.Index);
                SaveTopics(item.Topic, dirPath, levels);
                levels.Pop();
            }
        }

        [Verb(Description = "Saves the topic tree in the specified directory.", Aliases = "save")]
        public static int SaveTopics([Required] [Description("Path of the document.")][Aliases("doc")] string docPath,
            [Required] [Description("Path of the destination directory")][Aliases("dir")] string dirPath,
            [Description("Delete the source HTML file when the operation completes?")][Aliases("del")][DefaultValue(false)] bool clean)
        {
            if (string.IsNullOrWhiteSpace(docPath))
                return ShowError(1, "The path of the document cannot be empty.");
            if (!File.Exists(docPath))
                return ShowError(1, "The specified document does not exist.");
            if (string.IsNullOrWhiteSpace(dirPath))
                return ShowError(1, "The path of the output directory cannot be empty.");
            if (!Directory.Exists(dirPath))
                try
                {
                    Directory.CreateDirectory(dirPath);
                }
                catch
                {
                    return ShowError(1, "Could not create the output directory. Check your write permissions.");
                }
                
            var chm = DocToChm(docPath, Path.Combine(dirPath, Path.GetFileNameWithoutExtension(docPath)) + ".html", !clean, true);
            if (chm == null)
                return 2;
            SaveTopics(chm.Content.Root, dirPath, new Stack<int>());
            return 0;
        }
    }
}