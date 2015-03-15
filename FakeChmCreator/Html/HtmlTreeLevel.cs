using System.Collections.Generic;

namespace FakeChmCreator.Html
{
    abstract class HtmlTreeLevel<TOwner, TItem> : OwnedItemCollectionBase<TOwner, TItem>
        where TItem : class, IOwnedItem<TOwner>, IHtmlNodeContainer
        where TOwner : class, IHtmlNodeContainer
    {
        protected HtmlTreeLevel(TOwner owner) : base(owner)
        {
        }

        protected HtmlTreeLevel(TOwner owner, IList<TItem> items) : base(owner, items)
        {
        }

        public override void Add(TItem item)
        {
            base.Add(item);
            Owner.Node.ChildNodes.Append(item.Node);
        }

        public override void Clear()
        {
            base.Clear();
            Owner.Node.ChildNodes.Clear();
        }

        public override bool Remove(TItem item)
        {
            if (base.Remove(item))
            {
                Owner.Node.ChildNodes.Remove(item.Node);
                return true;
            }
            return false;
        }

        public override void Insert(int index, TItem item)
        {
            base.Insert(index, item);
            Owner.Node.ChildNodes.Insert(index, item.Node);
        }

        public override void RemoveAt(int index)
        {
            base.RemoveAt(index);
            Owner.Node.ChildNodes.RemoveAt(index);
        }
    }
}