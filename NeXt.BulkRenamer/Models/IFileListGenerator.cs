using System.Collections.Generic;

namespace NeXt.BulkRenamer.Models
{
    internal interface IFileListGenerator
    {
        IEnumerable<string> Generate();
    }
}