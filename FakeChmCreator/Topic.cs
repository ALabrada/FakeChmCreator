﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FakeChmCreator.Html;
using HtmlAgilityPack;

namespace FakeChmCreator
{
    /// <summary>
    /// Represents a node in the topic tree of a CHM document.
    /// </summary>
    public class Topic : IOwnedItem<Topic>
    {
        class TopicCollection : OwnedItemCollectionBase<Topic, Topic>
        {
            public TopicCollection(Topic owner) : base(owner)
            {
            }

            protected override void SetCommonOwner(Topic item)
            {
                item.Parent = Owner;
            }

            protected override void ClearOwner(Topic item)
            {
                item.Parent = null;
            }
        }

        private readonly TopicCollection _subTopics;

        /// <summary>
        /// Creates an instance of <see cref="Topic"/>.
        /// </summary>
        public Topic()
        {
            _subTopics = new TopicCollection(this);
        }

        /// <summary>
        /// Gets the parent of the topic.
        /// </summary>
        public Topic Parent { get; private set; }

        /// <summary>
        /// Gets or sets the name of the topic.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the index of the image that represents the topic from the image list.
        /// </summary>
        public string ImageNumber { get; set; }

        /// <summary>
        /// Gets or sets the content of the topic.
        /// </summary>
        public Page Content { get; internal set; }

        /// <summary>
        /// Gets the sub-topics of this topic.
        /// </summary>
        public ICollection<Topic> SubTopics { get { return _subTopics; } }

        /// <summary>
        /// Gets the owner of the topic.
        /// </summary>
        Topic IOwnedItem<Topic>.Owner
        {
            get { return Parent; }
        }
    }
}
