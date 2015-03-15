using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FakeChmCreator
{
    public class ChmDocument
    {
        public ChmContent Content { get; private set; }

        public ChmDocument Load(string filePath)
        {
            Content = new ChmContent();
            var page = Page.LoadFromFile(filePath);
            var whiteSpace = new Regex(@"(\s|(&nbsp;))+", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
            var chmNodes = new Stack<Tuple<int, Topic>>();
            foreach (var section in page.Sections)
            {
                
            }
        }
    }
}