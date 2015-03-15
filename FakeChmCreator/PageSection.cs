using System;
using System.Diagnostics.Contracts;
using HtmlAgilityPack;

namespace FakeChmCreator
{
    /// <summary>
    /// Representation of a section in a DOC document.
    /// </summary>
    public class PageSection : IOwnedItem<PageContent>, ICloneable, IHtmlNodeContainer
    {
        internal class SectionCollection : HtmlTreeLevel<PageContent, PageSection>
        {
            public SectionCollection(PageContent owner) : base(owner)
            {
            }

            protected override void SetCommonOwner(PageSection item)
            {
                item.OwnerPage = Owner;
            }

            protected override void ClearOwner(PageSection item)
            {
                item.OwnerPage = null;
            }
        }

        private readonly HtmlNode _node;

        internal PageSection(HtmlNode node)
        {
            Contract.Requires<ArgumentNullException>(node != null, "node");
            _node = node;
        }

        /// <summary>
        /// Gets the page that owns the section.
        /// </summary>
        public PageContent OwnerPage { get; private set; }

        /// <summary>
        /// Gets the page that owns the section.
        /// </summary>
        PageContent IOwnedItem<PageContent>.Owner
        {
            get { return OwnerPage; }
        }

        /// <summary>
        /// Makes an exact copy of the section.
        /// </summary>
        /// <param name="copyContent">Whether to include a copy of the section's content in the new section instance.</param>
        /// <returns>An exact copy of the instance.</returns>
        public PageSection CloneSection(bool copyContent = false)
        {
            return new PageSection(_node.CloneNode(copyContent));
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
            return CloneSection(true);
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