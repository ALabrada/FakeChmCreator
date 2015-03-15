using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using HtmlAgilityPack;

namespace FakeChmCreator
{
    /// <summary>
    /// Represents the &lt;body/> section in a HTML  page.
    /// </summary>
    public class PageContent : IOwnedItem<Page>, IHtmlNodeContainer
    {
        private readonly PageSection.SectionCollection _sections;
        private readonly HtmlNode _node;

        internal PageContent(HtmlNode node, Page owner)
        {
            Contract.Requires<ArgumentNullException>(node != null, "node");
            _node = node;
            OwnerPage = owner;
            _sections = new PageSection.SectionCollection(this);
            foreach (var child in node.ChildNodes)
                _sections.Add(new PageSection(child));
        }

        /// <summary>
        /// Gets the HTML page this instance belongs to.
        /// </summary>
        public Page OwnerPage { get; private set; }

        internal HtmlNode Node
        {
            get { return _node; }
        }

        /// <summary>
        /// Gets the sections of the page content.
        /// </summary>
        public ICollection<PageSection> Sections
        {
            get { return _sections; }
        }

        /// <summary>
        /// Makes an exact copy of the page content.
        /// </summary>
        /// <param name="newPage"></param>
        /// <param name="copyContent">Whether to include a copy of the node's content in the created instance.</param>
        /// <returns>An exact copy of the instance.</returns>
        public PageContent CloneContent(Page newPage, bool copyContent = false)
        {
            return new PageContent(_node.CloneNode(copyContent), newPage);
        }

        /// <summary>
        /// Gets the owner of the item.
        /// </summary>
        Page IOwnedItem<Page>.Owner
        {
            get { return OwnerPage; }
        }

        /// <summary>
        /// Gets the HTML node the instance represents.
        /// </summary>
        HtmlNode IHtmlNodeContainer.Node
        {
            get { return _node; }
        }
    }
}