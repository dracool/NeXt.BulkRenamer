using System.IO;
using System.Text.RegularExpressions;

namespace NeXt.BulkRenamer.Models.Parsing
{
    internal interface IResultPart
    {
        string Process(GroupCollection matches, FileInfo file);
    }
}
