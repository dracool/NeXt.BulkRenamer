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
        public RenameTargetViewModel(string fullPath, int index)
        {
            Index = index;
            source = new Lazy<FileInfo>(() => new FileInfo(fullPath));
            OriginalName = Path.GetFileName(fullPath);
        }

        private readonly Lazy<FileInfo> source;
        public FileInfo Source => source.Value;
        public string OriginalName { get; }

        string IReplacementTarget.SourceName => OriginalName;

        [AlsoNotifyFor(nameof(DisplayResultName))]
        public bool Enabled { get; set; } = true;

        public int Index { get; }

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
