using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media;
using Caliburn.Micro;
using MahApps.Metro;
using NeXt.BulkRenamer.Models;
using NeXt.BulkRenamer.Models.Background;
using PropertyChanged;

namespace NeXt.BulkRenamer.ViewModels
{
    internal class RenameTargetViewModel : PropertyChangedBase, IReplacementTarget
    {
        public RenameTargetViewModel(FileInfo value)
        {
            Source = value;
            OriginalName = Source.Name;
        }

        public FileInfo Source { get; }
        public string OriginalName { get; }

        [AlsoNotifyFor(nameof(DisplayResultName))]
        public bool Enabled { get; set; } = true;

        [AlsoNotifyFor(nameof(DisplayResultName))]
        public bool Success { get; set; } = true;

        [AlsoNotifyFor(nameof(DisplayResultName))]
        public string ResultName { get; set; }
        
        public string DisplayResultName
        {
            get
            {
                if (!Enabled)
                {
                    return string.Empty;
                }

                return Success ? ResultName : "<invalid>";
            }
        }
    }
}
