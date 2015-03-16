using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using HtmlAgilityPack;

namespace FakeChmCreator.Html
{
    /// <summary>
    /// Abstraction of an HTML page.
    /// </summary>
    public class Page : ICloneable
    {
        private readonly HtmlDocument _document;
        private readonly HtmlNode _head;
        private PageContent _content;
        private HtmlNode _titleNode;

        private Page(HtmlDocument document)
        {
            _document = document;
            var html = _document.DocumentNode.ChildNodes["html"];
            _head = html.ChildNodes["head"];
            _titleNode = _head.ChildNodes["title"];
            var body = html.ChildNodes["body"];
            Content = body == null ? null : new PageContent(body, this);
        }

        /// <summary>
        /// Gets or sets the title of the HTML page.
        /// </summary>
        public string Title
        {
            get { return _titleNode == null ? null : _titleNode.InnerText; }
            set
            {
                if (Title == value) return;
                if (string.IsNullOrWhiteSpace(value))
                {
                    _head.ChildNodes.Remove(_titleNode);
                    _titleNode = null;
                }
                else
                {
                    var newNode = HtmlNode.CreateNode(string.Format("<title>{0}</title>", value));
                    if (_titleNode == null)
                        _head.ChildNodes.Add(_titleNode = newNode);
                    else
                    {
                        var ti = _head.ChildNodes.IndexOf(_titleNode);
                        _head.ChildNodes.RemoveAt(ti);
                        _head.ChildNodes.Insert(ti, _titleNode = newNode);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the content of the HTML page.
        /// </summary>
        public PageContent Content
        {
            get { return _content; }
            private set
            {
                var html = _document.DocumentNode.ChildNodes["html"];
                if (_content != null)
                    html.RemoveChild(_content.Node);
                if (value != null)
                    html.AppendChild(value.Node);
                _content = value;
            }
        }

        /// <summary>
        /// Loads an HTML page from a file.
        /// </summary>
        /// <param name="filePath">Path of the file.</param>
        /// <returns>Loaded page instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="filePath"/> is empty.</exception>
        public static Page LoadFromFile(string filePath)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(filePath), "filePath");
            var doc = new HtmlDocument();
            doc.Load(filePath);
            return new Page(doc);
        }

        /// <summary>
        /// Makes an exact copy of the page.
        /// </summary>
        /// <param name="copyContent">Whether to include a copy of the page's content in the new instance.</param>
        /// <returns>An exact copy of the instance.</returns>
        public Page ClonePage(bool copyContent = false)
        {
            var newDoc = new HtmlDocument();
            var newHead = _head.CloneNode(true);
            var newHtml = _document.DocumentNode.ChildNodes["html"].CloneNode(false);
            newHtml.AppendChild(newHead);
            newDoc.DocumentNode.AppendChild(newHtml);
            var newPage = new Page(newDoc);
            var newBody = Content.CloneContent(newPage, copyContent);
            newPage.Content = newBody;
            return newPage;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public object Clone()
        {
            return ClonePage(true);
        }
    }
}
