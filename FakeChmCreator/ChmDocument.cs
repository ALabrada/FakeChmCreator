using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using FakeChmCreator.Html;

namespace FakeChmCreator
{
    public class ChmDocument
    {
        public ChmContent Content { get; private set; }

        public void Load(string filePath)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(filePath));
            Content = new ChmContent();
            var page = Page.LoadFromFile(filePath);
            var index = new Dictionary<string, Topic>();
            var chmNodes = new Stack<Tuple<int, Topic>>();
            var root = Content.Root;
            root.Name = page.Title;
            chmNodes.Push(new Tuple<int, Topic>(0, root));
            foreach (var section in page.Content.Sections)
            {
                // Creating section cover
                var sectionCover = new Topic {Name = section.Name ?? "Cover"};
                chmNodes.Push(new Tuple<int, Topic>(int.MaxValue, sectionCover));
                // Adding content.
                var topicItems = new List<SectionItem>(section.Items.Count);
                foreach (var item in section.Items)
                {
                    if (item.Heading != null)
                    {
                        AddLastTopic(section, topicItems, chmNodes, sectionCover, page, index);
                        // Updating the stack
                        while (chmNodes.Peek().Item1 >= item.Heading.Level)
                            chmNodes.Pop();
                        // Creating topic
                        var newTopic = new Topic { Name = item.Heading.Text };
                        chmNodes.Push(new Tuple<int, Topic>(item.Heading.Level, newTopic));

                    }
                    topicItems.Add(item);
                }
                AddLastTopic(section, topicItems, chmNodes, sectionCover, page, index);
                // Restoring node stack.
                while (chmNodes.Peek().Item1 > 0)
                    chmNodes.Pop();
            }
        }

        private static void AddLastTopic(ContentSection section, ICollection<SectionItem> topicItems, Stack<Tuple<int, Topic>> chmNodes, Topic sectionCover, Page page,
            IDictionary<string, Topic> index)
        {
            // Cloning section.
            var newSection = section.CloneSection();
            foreach (var tItem in topicItems)
                newSection.Items.Add(tItem.CloneItem());
            topicItems.Clear();
            // Skipping empty topics.
            var pair = chmNodes.Pop();
            var prevTopic = pair.Item2;
            if (prevTopic != sectionCover || !newSection.IsEmpty)
            {
                // Cloning page.
                var newPage = page.ClonePage();
                newPage.Content.Sections.Add(newSection);
                // Changing title
                newPage.Title = prevTopic.Name;
                // Adding the topic to the tree
                prevTopic.Content = newPage;
                chmNodes.Peek().Item2.SubTopics.Add(prevTopic);
                chmNodes.Push(pair);
                // Updating index
                foreach (var nodeId in newPage.Content.GetNodeIds())
                    index[nodeId] = prevTopic;
            }
        }
    }
}