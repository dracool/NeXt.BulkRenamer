using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace NeXt.BulkRenamer.ViewModels
{
    internal sealed class MainViewModel : Screen
    {
        private readonly IRenameTargetViewModelFactory renameFactory;

        public MainViewModel(FileSelectionViewModel fileSelection, PatternSelectionViewModel patternSelection, IRenameTargetViewModelFactory renameFactory)
        {
            DisplayName = "Bulk Regex Renamer";
            this.renameFactory = renameFactory;
            FileSelection = fileSelection;
            PatternSelection = patternSelection;
            FileSelection.PropertyChanged += FileSelectionOnPropertyChanged;
            UpdateFiles();
        }


        private void FileSelectionOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(FileSelectionViewModel.Files))
            {
                UpdateFiles();
            }
        }

        private async void UpdateFiles()
        {
            Targets?.Clear();
            IsLoading = true;
            await Task.Factory.StartNew(() =>
            {
                var targets = FileSelection.Files?.Generate()
                                           .Select(f => renameFactory.Create(f));
                Targets = new BindableCollection<RenameTargetViewModel>(targets ?? new List<RenameTargetViewModel>());
            }, CancellationToken.None,TaskCreationOptions.LongRunning, TaskScheduler.Default);
            
            IsLoading = false;
        }

        public bool IsLoading { get; set; }
        public FileSelectionViewModel FileSelection { get; }
        public PatternSelectionViewModel PatternSelection { get; }
        public BindableCollection<RenameTargetViewModel> Targets { get; private set; }

        public void RenameSelected()
        {
            foreach (var target in Targets.ToList())
            {
                if (!target.Enabled || string.IsNullOrWhiteSpace(target.ResultName) || target.ResultName.StartsWith("<")) continue;

                try
                {
                    File.Move(target.FilePath, Path.Combine(Path.GetDirectoryName(target.FilePath), target.ResultName));
                }
                catch (Exception)
                {
                    continue;
                }
                Targets.Remove(target);
            }
        }
    }
}
