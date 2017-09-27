using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using NeXt.BulkRenamer.Models.Background;
using NeXt.BulkRenamer.Utility;

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
            Targets = null;
            IsLoading = true;
            await Task.Run(() =>
            {
                var i = 0;
                var targets = FileSelection.Files?.Generate()
                                           .Select(f => renameFactory.Create(f, i++));
                Targets = new BindableCollection<RenameTargetViewModel>(targets ?? new List<RenameTargetViewModel>());
            });
            IsLoading = false;
        }

        public bool IsLoading { get; set; }
        public FileSelectionViewModel FileSelection { get; }
        public PatternSelectionViewModel PatternSelection { get; }
        public BindableCollection<RenameTargetViewModel> Targets { get; private set; }

        private VisibleRange visibleRange;
        public VisibleRange VisibleRange
        {
            get => visibleRange;
            set
            {
                visibleRange = value;
                backgroundEngine.UpdateTargets(VisibleTargets);
            }
        }

        private IReadOnlyCollection<IReplacementTarget> VisibleTargets
        {
            get
            {

                if (VisibleRange == null) throw new InvalidOperationException();

                return Targets
                    .Skip(VisibleRange.First)
                    .Take(VisibleRange.Count)
                    .ToArray();
            }
        }
        
        public int MaximumProgressValue { get;  set; }
        public int ProgressValue { get;  set; }
        public Visibility ProgressVisiblility { get;  set; }

        public async Task RenameSelected()
        {
            ProgressVisiblility = Visibility.Visible;
            MaximumProgressValue = Targets.Count * 2;
            ProgressValue = 0;
            await Task.Run(() =>
            {
                foreach (var target in Targets.ToList())
                {
                    backgroundEngine.ExecuteFor(target);
                    ProgressValue++;

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
                        ProgressValue++;
                        continue;
                    }
                    Targets.Remove(target);
                    ProgressValue++;
                }
            });

            ProgressVisiblility = Visibility.Collapsed;
        }

        /// <inheritdoc />
        protected override void OnInitialize()
        {
            base.OnInitialize();
            ProgressVisiblility = Visibility.Collapsed;
        }
    }
}
