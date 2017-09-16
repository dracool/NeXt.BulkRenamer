using System.Text.RegularExpressions;

namespace NeXt.BulkRenamer.Models
{
    internal interface ITextReplacement
    {
        string Apply(string input, Regex regex);
    }
}