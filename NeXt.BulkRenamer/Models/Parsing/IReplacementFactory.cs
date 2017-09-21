using NeXt.BulkRenamer.Models.Background;

namespace NeXt.BulkRenamer.Models.Parsing
{
    internal interface IReplacementFactory
    {
        IReplacement Create(string pattern);
    }
}