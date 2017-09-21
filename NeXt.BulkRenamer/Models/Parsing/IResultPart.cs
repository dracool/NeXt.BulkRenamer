using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NeXt.BulkRenamer.Models.Parsing
{
    internal interface IResultPart
    {
        string Process(GroupCollection matches, FileInfo file);
    }
}
