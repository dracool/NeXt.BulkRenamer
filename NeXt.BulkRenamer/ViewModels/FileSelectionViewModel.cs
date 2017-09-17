using System;
using System.Linq;
using Caliburn.Micro;
using NeXt.BulkRenamer.Models;
using Ookii.Dialogs.Wpf;

namespace NeXt.BulkRenamer.ViewModels
{
    internal class FileSelectionViewModel : PropertyChangedBase
    {
        public FileSelectionViewModel()
        {
            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                // skip the argument containing the executables path
                Files = new StaticFileListGenerator(args.Skip(1).ToList());
            }
        }

        private string directoryPath;
        private bool isDirectoryRecursive;

        /// <summary>
        /// generator for the list of files to rename
        /// </summary>
        public IFileListGenerator Files { get; private set; }
        
        /// <summary>
        /// sets whether a directory search is recursive or not
        /// </summary>
        public bool IsDirectoryRecursive
        {
            get => isDirectoryRecursive;
            set
            {
                isDirectoryRecursive = value;
                UpdateToDirectory();
            }
        }

        /// <summary>
        /// Called when the select files button is clicked in the view
        /// </summary>
        public void SelectFiles()
        {
            var dlg = new VistaOpenFileDialog
            {
                Multiselect = true,
                Title = @"Select Files",
            };
            if (!dlg.ShowDialog().GetValueOrDefault(false)) return;

            if (dlg.FileNames.Length > 0)
            {
                Files = new StaticFileListGenerator(dlg.FileNames);
            }
            else if (!string.IsNullOrWhiteSpace(dlg.FileName))
            {
                Files = new StaticFileListGenerator(new []{dlg.FileName});
            }
        }
        
        /// <summary>
        /// Called when the select directory button is clicked in the view
        /// </summary>
        public void SelectDirectory()
        {
            var dlg = new VistaFolderBrowserDialog
            {
                Description = @"Select Directory",
                UseDescriptionForTitle = true,
                ShowNewFolderButton = false,
                SelectedPath = directoryPath ?? string.Empty
            };
            if (!dlg.ShowDialog().GetValueOrDefault(false)) return;

            directoryPath = dlg.SelectedPath;
            UpdateToDirectory();
        }

        /// <summary>
        /// sets the file listing to contain all files in <see cref="directoryPath"/>
        /// </summary>
        private void UpdateToDirectory()
        {
            if (string.IsNullOrWhiteSpace(directoryPath)) return;
            Files = new DirectoryFileListGenerator(directoryPath, IsDirectoryRecursive);
        }
    }
}
