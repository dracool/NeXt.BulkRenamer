using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Microsoft.Win32;
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
                Files = new StaticFileListGenerator(args.Skip(1).ToList());
            }
        }

        public IFileListGenerator Files { get; private set; }
        
        private bool isDirectoryRecursive;

        public bool IsDirectoryRecursive
        {
            get => isDirectoryRecursive;
            set
            {
                isDirectoryRecursive = value;
                UpdateToDirectory();
            }
        }

        public void SelectFiles()
        {
            var dlg = new VistaOpenFileDialog
            {
                Multiselect = true,
                Title = @"Select Files",
            };

            if (dlg.ShowDialog().GetValueOrDefault(false))
            {
                if (dlg.FileNames.Length > 0)
                {
                    Files = new StaticFileListGenerator(dlg.FileNames);
                }
                else if (!string.IsNullOrWhiteSpace(dlg.FileName))
                {
                    Files = new StaticFileListGenerator(new []{dlg.FileName});
                }
            }
        }

        private string directoryPath;
        
        private void UpdateToDirectory()
        {
            if (string.IsNullOrWhiteSpace(directoryPath)) return;
            Files = new DirectoryFileListGenerator(directoryPath, IsDirectoryRecursive);
        }

        public void SelectDirectory()
        {
            var dlg = new VistaFolderBrowserDialog()
            {
                Description = @"Select Directory",
                UseDescriptionForTitle = true,
                ShowNewFolderButton = false,
                SelectedPath = directoryPath ?? string.Empty
            };
            
            if (dlg.ShowDialog().GetValueOrDefault(false))
            {
                directoryPath = dlg.SelectedPath;
                UpdateToDirectory();
            }
        }
    }
}
