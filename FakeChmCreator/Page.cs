using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace FakeChmCreator
{
    public class Page
    {
        private readonly HtmlDocument _document;

        private Page(HtmlDocument document)
        {
            _document = document;
        }
    }
}
