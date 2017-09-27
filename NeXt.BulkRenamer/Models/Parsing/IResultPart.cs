using System.IO;
using System.Text.RegularExpressions;
using NeXt.BulkRenamer.Models.Background;

namespace NeXt.BulkRenamer.Models.Parsing
{
    internal interface IResultPart
    {
        string Process(GroupCollection matches, IReplacementTarget target);
    }
}
