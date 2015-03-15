using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace FakeChmCreator
{
    /// <summary>
    /// Base implementation of <see cref="IOwnedItemList{TOwner,TItem}"/>.
    /// </summary>
    /// <typeparam name="TOwner">Type of the owner.</typeparam>
    /// <typeparam name="TItem">Type of the items.</typeparam>
    public abstract class OwnedItemListBase<TOwner, TItem> : IOwnedItemList<TOwner, TItem> where TItem : class, IOwnedItem<TOwner> where TOwner : class
    {
        protected IList<TItem> Items;

        /// <summary>
        /// Creates an instance of <see cref="OwnedItemListBase{TOwner,TItem}"/>.
        /// </summary>
        /// <param name="owner">Common owner.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="owner"/> is <see langword="null"/>.</exception>
        protected OwnedItemListBase(TOwner owner) : this(owner, new List<TItem>())
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="OwnedItemListBase{TOwner,TItem}"/>.
        /// </summary>
        /// <param name="owner">Common owner.</param>
        /// <param name="items">Items collection.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="owner"/> or <paramref name="items"/> are <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="items"/> is not empty.</exception>
        protected OwnedItemListBase(TOwner owner, IList<TItem> items)
        {
            Contract.Requires<ArgumentNullException>(items != null);
            Contract.Requires<ArgumentNullException>(owner != null);
            Contract.Requires<ArgumentException>(items.Count == 0);
            Items = items;
            Owner = owner;
        }

        /// <summary>
        /// Gets the common owner of the items in the list.
        /// </summary>
        public TOwner Owner { get; private set; }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<TItem> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="item"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Thrown when the <see cref="IOwnedItem{TOwner}.Owner"/> property of <paramref name="item"/> is not <see langword="null"/>.</exception>
        /// <remarks>
        /// <para>
        /// The item should not have an owner when it is included in the collection. When included, its owner will be set to <see cref="Owner"/>.
        /// Do not add <see langword="null"/> values to the collection.
        /// </para>
        /// </remarks> 
        public void Add(TItem item)
        {
            Contract.Requires<ArgumentNullException>(item != null, "item");
            Contract.Requires<ArgumentException>(item.Owner == null, "Cannot add the item because it already has an owner.");
            Items.Add(item);
            SetCommonOwner(item);
            Contract.Assert(item.Owner == Owner);
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only. </exception>
        /// <remarks>
        /// <para>
        /// The owners of the items that are currently in the collection will be set to <see langword="null"/>.
        /// </para>
        /// </remarks>
        public void Clear()
        {
            if (IsReadOnly)
                throw new NotSupportedException("Cannot modify a read-only collection.");
            foreach (var item in Items)
            {
                ClearOwner(item);
                Contract.Assert(item.Owner == null);
            }
            Items.Clear();
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
        /// </summary>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
        /// </returns>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        public bool Contains(TItem item)
        {
            return item != null && Items.Contains(item);
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param><param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param><exception cref="T:System.ArgumentNullException"><paramref name="array"/> is null.</exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception><exception cref="T:System.ArgumentException">The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.</exception>
        public void CopyTo(TItem[] array, int arrayIndex)
        {
            Items.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
        public bool Remove(TItem item)
        {
            if (item == null || !Items.Remove(item))
                return false;
            ClearOwner(item);
            Contract.Assert(item.Owner == null);
            return true;
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        public int Count
        {
            get { return Items.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.
        /// </returns>
        public bool IsReadOnly
        {
            get { return Items.IsReadOnly; }
        }

        /// <summary>
        /// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1"/>.
        /// </summary>
        /// <returns>
        /// The index of <paramref name="item"/> if found in the list; otherwise, -1.
        /// </returns>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
        public int IndexOf(TItem item)
        {
            if (item == null) return -1;
            return Items.IndexOf(item);
        }

        /// <summary>
        /// Inserts an item to the <see cref="T:System.Collections.Generic.IList`1"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.</exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IList`1"/> is read-only.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="item"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Thrown when the <see cref="IOwnedItem{TOwner}.Owner"/> property of <paramref name="item"/> is not <see langword="null"/>.</exception>
        public void Insert(int index, TItem item)
        {
            Contract.Requires<ArgumentNullException>(item != null, "item");
            Contract.Requires<ArgumentException>(item.Owner == null, "Cannot insert the item because it already has an owner.");
            Items.Insert(index, item);
            SetCommonOwner(item);
            Contract.Assert(item.Owner == Owner);
        }

        /// <summary>
        /// Removes the <see cref="T:System.Collections.Generic.IList`1"/> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.</exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IList`1"/> is read-only.</exception>
        public void RemoveAt(int index)
        {
            var item = Items[index];
            Items.RemoveAt(index);
            ClearOwner(item);
            Contract.Assert(item.Owner == null);
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <returns>
        /// The element at the specified index.
        /// </returns>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.</exception>
        /// <exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.Generic.IList`1"/> is read-only.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Thrown when the <see cref="IOwnedItem{TOwner}.Owner"/> property of <paramref name="value"/> is not <see langword="null"/>.</exception>
        public TItem this[int index]
        {
            get { return Items[index]; }
            set
            {
                Contract.Requires<ArgumentNullException>(value != null, "value");
                Contract.Requires<ArgumentException>(value.Owner == null, "Cannot insert the item because it already has an owner.");
                var prevItem = Items[index];
                ClearOwner(prevItem);
                Contract.Assert(prevItem.Owner == null);
                Items[index] = value;
                SetCommonOwner(value);
                Contract.Assert(value.Owner == Owner);
            }
        }

        /// <summary>
        /// When implemented in a derived class sets the common owner as the owner of the specified item.
        /// </summary>
        /// <param name="item">Item which ownership will be changed.</param>
        protected abstract void SetCommonOwner(TItem item);

        /// <summary>
        /// When implemented in a derived class leaves the specified item without an owner.
        /// </summary>
        /// <param name="item">Item which ownership will be changed.</param>
        protected abstract void ClearOwner(TItem item);
    }
}