using System.IO;

namespace NeXt.BulkRenamer.Models.Background
{
    internal interface IReplacementTarget
    {
        bool Success { set; }
        bool Enabled { get; }
        FileInfo Source { get; }
        string ResultName { set; }
    }
}