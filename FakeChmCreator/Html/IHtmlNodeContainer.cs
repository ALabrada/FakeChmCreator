using HtmlAgilityPack;

namespace FakeChmCreator.Html
{
    /// <summary>
    /// Contract of a type that represents a node in a HTML page tree.
    /// </summary>
    interface IHtmlNodeContainer
    {
        /// <summary>
        /// Gets the HTML node the instance represents.
        /// </summary>
        HtmlNode Node { get; } 
    }
}