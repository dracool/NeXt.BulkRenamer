using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using NeXt.BulkRenamer.Models;
using PropertyChanged;

namespace NeXt.BulkRenamer.ViewModels
{
    internal class RenameTargetViewModel : PropertyChangedBase
    {
        public RenameTargetViewModel(string value, IBackgroundTextReplacement backgroundReplacer)
        {
            replacement = backgroundReplacer;
            replacement.Invalidated += UpdateName;
            FilePath = value;
            OriginalName = Path.GetFileName(FilePath);
        }

        public string FilePath { get; }

        private readonly IBackgroundTextReplacement replacement;

        private bool enabled = true;

        public bool Enabled
        {
            get => enabled;
            set
            {
                enabled = value;
                UpdateName(this, EventArgs.Empty);
            }
        }

        private string originalName;
        public string OriginalName
        {
            get => originalName;
            set
            {
                originalName = value;
                UpdateName(this, EventArgs.Empty);
            }
        }
        
        public string ResultName { get; private set; }

        private async void UpdateName(object sender, EventArgs e)
        {
            if (!Enabled)
            {
                ResultName = string.Empty;
                return;
            }
            
            try
            {
                var value = await replacement.RunAsync(OriginalName);
                ResultName = value ?? "<invalid>";
            }
            catch(TaskCanceledException) { }
        }
    }
}
