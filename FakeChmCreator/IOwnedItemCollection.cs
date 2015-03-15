using System.Collections.Generic;

namespace FakeChmCreator
{
    /// <summary>
    /// Abstraction of a list of <see cref="IOwnedItem{TOwner}"/> that share the same owner.
    /// </summary>
    /// <typeparam name="TOwner">Type of the owner.</typeparam>
    /// <typeparam name="TItem">Type of the items.</typeparam>
    interface IOwnedItemCollection<out TOwner, TItem> : ICollection<TItem> where TItem : IOwnedItem<TOwner>
    {
        /// <summary>
        /// Gets the common owner of the items in the list.
        /// </summary>
        TOwner Owner { get; }
    }
}