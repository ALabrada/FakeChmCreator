using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using HtmlAgilityPack;

namespace FakeChmCreator
{
    /// <summary>
    /// Abstraction of an HTML page.
    /// </summary>
    public class Page : ICloneable
    {
        private readonly HtmlDocument _document;
        private readonly HtmlNode _body, _head;
        private readonly PageSection.SectionList _sections;
        private HtmlNode _titleNode;

        private Page(HtmlDocument document)
        {
            _document = document;
            var html = _document.DocumentNode.ChildNodes["html"];
            _head = html.ChildNodes["head"];
            _body = html.ChildNodes["body"];
            _titleNode = html.ChildNodes["title"];
            _sections = new PageSection.SectionList(this);
            foreach (var child in _body.ChildNodes)
                _sections.Add(new PageSection(child));
        }

        /// <summary>
        /// Gets or sets the title of the HTML page.
        /// </summary>
        public string Title
        {
            get { return _titleNode == null ? null : _titleNode.InnerText; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    _head.ChildNodes.Remove(_titleNode);
                    _titleNode = null;
                }
                else
                {
                    var ti = _head.ChildNodes.IndexOf(_titleNode);
                    _head.ChildNodes.RemoveAt(ti);
                    _titleNode = HtmlNode.CreateNode(string.Format("<title>{0}</title>", value));
                    _head.ChildNodes.Insert(ti, _titleNode);
                }
            }
        }

        /// <summary>
        /// Gets the sections of the page content.
        /// </summary>
        public IList<PageSection> Sections
        {
            get { return _sections; }
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
            var newBody = _body.CloneNode(copyContent);
            var newHtml = _document.DocumentNode.ChildNodes["html"].CloneNode(false);
            newHtml.AppendChild(newHead);
            newHtml.AppendChild(newBody);
            newDoc.DocumentNode.AppendChild(newHtml);
            return new Page(newDoc);
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
