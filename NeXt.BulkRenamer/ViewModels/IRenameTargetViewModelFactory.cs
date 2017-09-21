using System.IO;

namespace NeXt.BulkRenamer.ViewModels
{
    internal interface IRenameTargetViewModelFactory
    {
        RenameTargetViewModel Create(FileInfo value);
    }
}