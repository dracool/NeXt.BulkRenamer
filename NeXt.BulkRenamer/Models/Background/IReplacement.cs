using System.IO;
using System.Text.RegularExpressions;

namespace NeXt.BulkRenamer.Models.Background
{
    internal interface IReplacement
    {
        string Apply(Regex regex, string name, FileInfo file);
    }
}