﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace FakeChmCreator.Html
{
    /// <summary>
    /// Representation of a section in a DOC document.
    /// </summary>
    public class ContentSection : IOwnedItem<PageContent>, ICloneable, IHtmlNodeContainer
    {
        internal class SectionCollection : HtmlTreeLevel<PageContent, ContentSection>
        {
            public SectionCollection(PageContent owner) : base(owner)
            {
            }

            public SectionCollection(PageContent owner, IEnumerable<ContentSection> items) : base(owner, items)
            {
            }

            protected override void SetCommonOwner(ContentSection item)
            {
                item.OwnerPage = Owner;
            }

            protected override void ClearOwner(ContentSection item)
            {
                item.OwnerPage = null;
            }
        }

        private readonly HtmlNode _node;
        private readonly SectionItem.ItemCollection _items;
        private static readonly Regex EmptyTextRegex = new Regex(@"^(\s|(&nbsp;))+$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Multiline);

        private readonly Lazy<string> _name; 

        internal ContentSection(HtmlNode node)
        {
            Contract.Requires<ArgumentNullException>(node != null, "node");
            _node = node;
            _name = new Lazy<string>(() => _node.GetAttributeValue("class", null));
            _items = new SectionItem.ItemCollection(this, node.ChildNodes.Select(c => new SectionItem(c)));
        }

        /// <summary>
        /// Gets whether the section contains any text.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return EmptyTextRegex.IsMatch(_node.InnerText);
            }
        }

        /// <summary>
        /// Gets the name of the section.
        /// </summary>
        public string Name
        {
            get { return _name.Value; }
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
        public ContentSection CloneSection(bool copyContent = false)
        {
            return new ContentSection(_node.CloneNode(copyContent));
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

        /// <summary>
        /// Gets the list of items in the section.
        /// </summary>
        public ICollection<SectionItem> Items
        {
            get { return _items; }
        }
    }
}