namespace NeXt.BulkRenamer.Models
{
    internal interface ITextReplacementFactory
    {
        ITextReplacement Create(string pattern);
    }
}