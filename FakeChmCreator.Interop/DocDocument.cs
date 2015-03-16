using System;
using System.Diagnostics.Contracts;
using System.IO;
using NetOffice.WordApi;
using NetOffice.WordApi.Enums;

namespace FakeChmCreator.Interop
{
    /// <summary>
    /// An abstraction of a DOC document from Microsoft Word.
    /// </summary>
    public class DocDocument : IDisposable
    {
        private readonly Application _application;
        private readonly Document _document;
        private bool _isDisposed;

        /// <summary>
        /// Creates an instance of <see cref="DocDocument"/> for the specified DOC.
        /// </summary>
        /// <param name="filePath">File path of the document.</param>
        public DocDocument(string filePath)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(filePath), "filePath");
            Contract.Requires<FileNotFoundException>(File.Exists(filePath), "filePath");
            _application = new Application {Visible = true};
            _document = _application.Documents.Open(filePath);
        }

        /// <summary>
        /// Saves the document in HTML format at the specified location.
        /// </summary>
        /// <param name="filePath">Destination file path.</param>
        public void SaveAsHtml(string filePath)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(filePath), "filePath");
            _document.SaveAs(filePath, WdSaveFormat.wdFormatFilteredHTML);
        }

        /// <summary>
        /// Frees the unmanaged resources used by the instance.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (_isDisposed) return;
            _document.Close(false);
            _document.Dispose();
            _application.Quit(false);
            _application.Dispose();
            _isDisposed = true;
        }
    }
}
