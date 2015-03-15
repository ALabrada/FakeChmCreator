namespace FakeChmCreator
{
    /// <summary>
    /// Content of a CHM file.
    /// </summary>
    public class ChmContent
    {
        /// <summary>
        /// Creates an instance of <see cref="ChmContent"/>.
        /// </summary>
        public ChmContent()
        {
            Root = new Topic();
        }

        /// <summary>
        /// Gets the root topic node.
        /// </summary>
        public Topic Root { get; private set; }
    }
}