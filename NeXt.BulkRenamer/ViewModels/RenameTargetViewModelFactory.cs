using System.IO;
using NeXt.BulkRenamer.Models;
using NeXt.BulkRenamer.Models.Background;

namespace NeXt.BulkRenamer.ViewModels
{
    internal class RenameTargetViewModelFactory : IRenameTargetViewModelFactory
    {
        public RenameTargetViewModel Create(FileInfo value)
        {
            return new RenameTargetViewModel(value);
        }
    }
}
