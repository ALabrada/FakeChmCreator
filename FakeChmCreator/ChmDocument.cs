﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FakeChmCreator.Html;

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
            foreach (var section in page.Content.Sections)
            {
                foreach (var item in section.Items)
                {
                    if (item.Heading != null)
                    {
                        
                    }
                }
            }
            throw new NotImplementedException();
        }
    }
}