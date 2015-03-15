namespace FakeChmCreator
{
    /// <summary>
    /// Abstraction of an item of some type with an associated owner.
    /// </summary>
    /// <typeparam name="TOwner">Type of the owner.</typeparam>
    public interface IOwnedItem<out TOwner>
    {
        /// <summary>
        /// Gets the owner of the item.
        /// </summary>
        TOwner Owner { get; }
    }
}