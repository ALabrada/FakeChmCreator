﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace FakeChmCreator.Html
{
    /// <summary>
    /// Represents a child item in a section in the HTML tree.
    /// The item can be a text paragraph, a heading, a table, an image, among many other things that can be present in a DOC document.
    /// </summary>
    public class SectionItem : IOwnedItem<ContentSection>, ICloneable, IHtmlNodeContainer
    {
        internal class ItemCollection : HtmlTreeLevel<ContentSection, SectionItem>
        {
            public ItemCollection(ContentSection owner) : base(owner)
            {
            }

            public ItemCollection(ContentSection owner, IEnumerable<SectionItem> items) : base(owner, items)
            {
            }

            protected override void SetCommonOwner(SectionItem item)
            {
                item.OwnerSection = Owner;
            }

            protected override void ClearOwner(SectionItem item)
            {
                item.OwnerSection = Owner;
            }
        }

        private readonly Lazy<ItemHeading> _heading;

        internal SectionItem(HtmlNode node)
        {
            Contract.Requires<ArgumentNullException>(node != null, "node");
            Node = node;
            _heading = new Lazy<ItemHeading>(() => ItemHeading.FindHeading(this));
        }

        /// <summary>
        /// Gets the section this item belongs to.
        /// </summary>
        public ContentSection OwnerSection { get; private set; }

        /// <summary>
        /// Gets the item's heading.
        /// </summary>
        public ItemHeading Heading
        {
            get { return _heading.Value; }
        }

        internal HtmlNode Node { get; private set; }

        /// <summary>
        /// Gets the HTML node the instance represents.
        /// </summary>
        HtmlNode IHtmlNodeContainer.Node
        {
            get { return Node; }
        }

        /// <summary>
        /// Makes an exact copy of the instance.
        /// </summary>
        /// <returns>An exact copy of the instance.</returns>
        public SectionItem CloneItem()
        {
            return new SectionItem(Node.CloneNode(true));
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
            return CloneItem();
        }

        /// <summary>
        /// Gets the owner of the item.
        /// </summary>
        ContentSection IOwnedItem<ContentSection>.Owner
        {
            get { return OwnerSection; }
        }
    }
}