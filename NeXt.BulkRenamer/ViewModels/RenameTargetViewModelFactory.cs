using System.IO;
using NeXt.BulkRenamer.Models;
using NeXt.BulkRenamer.Models.Background;

namespace NeXt.BulkRenamer.ViewModels
{
    internal class RenameTargetViewModelFactory : IRenameTargetViewModelFactory
    {
        public RenameTargetViewModel Create(string fullPath, int index)
        {
            return new RenameTargetViewModel(fullPath, index);
        }
    }
}
