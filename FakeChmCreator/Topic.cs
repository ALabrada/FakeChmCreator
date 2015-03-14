using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FakeChmCreator
{
    /// <summary>
    /// Represents a node in the topic tree of a CHM document.
    /// </summary>
    public class Topic
    {
        class TopicList : IList<Topic>
        {
            private readonly IList<Topic> _items = new List<Topic>();
            private readonly Topic _parent;

            public TopicList(Topic parent)
            {
                _parent = parent;
            }

            public IEnumerator<Topic> GetEnumerator()
            {
                return _items.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public void Add(Topic item)
            {
                if (item == null) throw new ArgumentNullException("item");
                if (item.Parent != null) throw new ArgumentException("Cannot add the topic because it is already a subtopic of this topic.", "item");
                if (item.Parent != null) throw new ArgumentException("Cannot add the topic because it is the sub-topic of another topic.", "item");
                _items.Add(item);
                item.Parent = null;
            }

            public void Clear()
            {
                foreach (var topic in _items)
                    topic.Parent = null;
                _items.Clear();
            }

            public bool Contains(Topic item)
            {
                if (item == null) throw new ArgumentNullException("item");
                return _items.Contains(item);
            }

            public void CopyTo(Topic[] array, int arrayIndex)
            {
                _items.CopyTo(array, arrayIndex);
            }

            public bool Remove(Topic item)
            {
                if (item == null) throw new ArgumentNullException("item");
                if (_items.Remove(item))
                {
                    item.Parent = null;
                    return true;
                }
                return false;
            }

            public int Count { get { return _items.Count; } }
            public bool IsReadOnly { get { return false; } }
            public int IndexOf(Topic item)
            {
                if (item == null) throw new ArgumentNullException("item");
                return _items.IndexOf(item);
            }

            public void Insert(int index, Topic item)
            {
                if (item == null) throw new ArgumentNullException("item");
                if (item.Parent == _parent) throw new ArgumentException("Cannot insert the topic because it is already a subtopic of this topic.", "item");
                if (item.Parent != null) throw new ArgumentException("Cannot insert the topic because it is the sub-topic of another topic.", "item");
                _items.Insert(index, item);
                item.Parent = _parent;
            }

            public void RemoveAt(int index)
            {
                var item = _items[index];
                _items.RemoveAt(index);
                item.Parent = null;
            }

            public Topic this[int index]
            {
                get { return _items[index]; }
                set
                {
                    if (value == null) throw new ArgumentNullException("value");
                    if (value.Parent == _parent) throw new ArgumentException("Cannot insert the topic because it is already a subtopic of this topic.", "value");
                    if (value.Parent != null) throw new ArgumentException("Cannot insert the topic because it is the sub-topic of another topic.", "value");
                    var item = _items[index];
                    _items[index] = value;
                    value.Parent = _parent;
                    item.Parent = null;
                }
            }
        }

        private readonly TopicList _subTopics;

        /// <summary>
        /// Creates an instance of <see cref="Topic"/>.
        /// </summary>
        public Topic()
        {
            _subTopics = new TopicList(this);
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
        public Page Content { get; set; }

        /// <summary>
        /// Gets the sub-topics of this topic.
        /// </summary>
        public IList<Topic> SubTopics { get { return _subTopics; } }        
    }
}
