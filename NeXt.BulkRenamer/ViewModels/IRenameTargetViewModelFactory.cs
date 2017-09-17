namespace NeXt.BulkRenamer.ViewModels
{
    internal interface IRenameTargetViewModelFactory
    {
        RenameTargetViewModel Create(string value);
    }
}