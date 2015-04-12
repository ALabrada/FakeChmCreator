using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using FakeChmCreator.Interop;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;

namespace FakeChmCreator.Windows.ViewModel
{
    public class ChmDocumentViewModel : ViewModelBase
    {
        private readonly ChmDocument _document;
        private readonly ObservableCollection<Topic> _topics = new ObservableCollection<Topic>(); 

        public ChmDocumentViewModel()
        {
            _document = new ChmDocument();
            AddDocDocumentCommand = new RelayCommand(AddDocDocument);
        }

        public RelayCommand AddDocDocumentCommand { get; private set; }

        public ObservableCollection<Topic> Topics
        {
            get { return _topics; }
        }

        private async void AddDocDocument()
        {
            var dialog = new OpenFileDialog()
            {
                Filter = "Microsoft Word 2003 documents (*.doc)|*.doc|Microsoft Word 2007 documents (*.docx)|*.docx"
            };
            if (dialog.ShowDialog() != true)
                return;
            var htmlFile = null as string;
            await TaskEx.Run(() =>
            {
                using (var doc = new DocDocument(dialog.FileName))
                {
                    var dir = Path.GetDirectoryName(dialog.FileName);
                    var name = Path.GetFileNameWithoutExtension(dialog.FileName);
                    if (dir == null || name == null)
                        throw new ApplicationException("Invalid file.");
                    htmlFile = Path.Combine(dir, name) + ".html";
                    doc.SaveAsHtml(htmlFile);
                }
                _document.Load(htmlFile);
            });
            _topics.Clear();
            foreach (var topic in _document.Content.Root.SubTopics)
            {
                _topics.Add(topic);
            }
        }
    }
}