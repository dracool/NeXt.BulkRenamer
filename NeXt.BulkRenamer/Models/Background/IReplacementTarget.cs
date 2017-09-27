using System.IO;

namespace NeXt.BulkRenamer.Models.Background
{
    internal interface IReplacementTarget
    {
        int Index { get; }
        bool Success { set; }
        bool Enabled { get; }
        FileInfo Source { get; }

        string SourceName { get; }

        string ResultName { set; }
    }
}