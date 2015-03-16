using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using HtmlAgilityPack;

namespace FakeChmCreator.Html
{
    /// <summary>
    /// Represents the &lt;body/> section in a HTML  page.
    /// </summary>
    public class PageContent : IOwnedItem<Page>, IHtmlNodeContainer
    {
        private readonly ContentSection.SectionCollection _sections;
        private readonly HtmlNode _node;

        internal PageContent(HtmlNode node, Page owner)
        {
            Contract.Requires<ArgumentNullException>(node != null, "node");
            _node = node;
            OwnerPage = owner;
            _sections = new ContentSection.SectionCollection(this, node.ChildNodes.Select(c => new ContentSection(c)));
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
        public ICollection<ContentSection> Sections
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

        private IEnumerable<string> GetNodeIds(HtmlNode node)
        {
            if (node.HasAttributes)
            {
                var nameAttr = node.GetAttributeValue("name", null);
                if (nameAttr != null)
                    yield return nameAttr;
            }
            foreach (var child in node.ChildNodes)
                foreach (var name in GetNodeIds(child))
                    yield return name;
        }

        /// <summary>
        /// Finds the IDs of all named nodes in the page's content.
        /// </summary>
        /// <returns>Sequence of node ID's.</returns>
        public IEnumerable<string> GetNodeIds()
        {
            return GetNodeIds(Node);
        }
    }
}