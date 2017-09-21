using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using NeXt.BulkRenamer.Models.Background;

namespace NeXt.BulkRenamer.ViewModels
{
    internal sealed class MainViewModel : Screen
    {
        private readonly IRenameTargetViewModelFactory renameFactory;
        private readonly IBackgroundEngine backgroundEngine;

        public MainViewModel(
            FileSelectionViewModel fileSelection,
            PatternSelectionViewModel patternSelection,
            IRenameTargetViewModelFactory renameFactory,
            IBackgroundEngine backgroundEngine
        )
        {
            DisplayName = "Bulk Regex Renamer";

            this.renameFactory = renameFactory;
            this.backgroundEngine = backgroundEngine;

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
            backgroundEngine.UpdateTargets(Targets);
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
                if (!target.Enabled 
                    || !target.Success
                    || string.IsNullOrWhiteSpace(target.ResultName) 
                    || target.ResultName.StartsWith("<"))
                continue;

                try
                {
                    target.Source.Rename(target.ResultName);
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
