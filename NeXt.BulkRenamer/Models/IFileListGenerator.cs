using System.Collections.Generic;
using System.IO;

namespace NeXt.BulkRenamer.Models
{
    internal interface IFileListGenerator
    {
        IEnumerable<FileInfo> Generate();
    }
}