using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace FakeChmCreator.Html
{
    /// <summary>
    /// Represents a heading node in a HTML tree.
    /// </summary>
    /// <remarks>
    /// A heading node is an HTML node with the form &lt;h#>...&lt;/h#>, where '#' is a positive integer.
    /// </remarks>
    public class ItemHeading : IOwnedItem<SectionItem>
    {
        private readonly SectionItem _owner;
        private readonly string _text;

        private static readonly Regex WhiteSpace = new Regex(@"(\s|(&nbsp;))+",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Multiline);

        private ItemHeading(HtmlNode node, SectionItem owner)
        {
            _owner = owner;
            _text = WhiteSpace.Replace(node.InnerText, " ");
        }

        /// <summary>
        /// Gets the level of the heading in the topic tree.
        /// </summary>
        public int Level { get; private set; }

        /// <summary>
        /// Gets the text of the heading suitable for representation in a topic.
        /// </summary>
        public string Text
        {
            get { return _text; }
        }

        /// <summary>
        /// Checks if the given HTML node is a heading node.
        /// </summary>
        /// <param name="node">HTML node to check.</param>
        /// <param name="level">If the node is a heading, its level in the topic tree.</param>
        /// <returns>true if <paramref name="node"/> is a heading node; false otherwise.</returns>
        internal static bool IsHeading(HtmlNode node, out int level)
        {
            Contract.Requires<ArgumentNullException>(node != null, "node");
            level = 0;
            var nodeName = node.Name;
            return nodeName.Length > 0 && nodeName[0] == 'h' &&
                   int.TryParse(nodeName.Substring(1), NumberStyles.Integer, CultureInfo.InvariantCulture, out level) &&
                   level > 0;
        }

        /// <summary>
        /// Gets the owner of the item.
        /// </summary>
        SectionItem IOwnedItem<SectionItem>.Owner
        {
            get { return _owner; }
        }

        private static ItemHeading FindHeading(SectionItem item, HtmlNode node)
        {
            int level;
            if (IsHeading(node, out level))
                return new ItemHeading(node, item) {Level = level};
            var headings = from child in node.ChildNodes let h = FindHeading(item, child) where h != null select h;
            return headings.FirstOrDefault();
        }

        /// <summary>
        /// Finds a heading for the specified section item.
        /// </summary>
        /// <param name="item">Owner item.</param>
        /// <returns>A heading for <paramref name="item"/> or <see langword="null"/> if there is none.</returns>
        /// <remarks>
        /// <para>
        /// If <paramref name="item"/> is not a heading by itself, a heading will be searched
        /// recursivelly in the HTML tree rooted at <paramref name="item"/>. If the tree doesn't contain any heading
        /// nodes, the function will return <see langword="null"/>. 
        /// </para>
        /// </remarks>
        public static ItemHeading FindHeading(SectionItem item)
        {
            Contract.Requires<ArgumentNullException>(item != null, "item");
            return FindHeading(item, item.Node);
        }
    }
}