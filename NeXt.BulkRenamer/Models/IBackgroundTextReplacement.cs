using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NeXt.BulkRenamer.Models
{
    internal interface IBackgroundTextReplacement
    {
        event EventHandler<EventArgs> Invalidated;

        Task<string> RunAsync(string value);
        void SetRegex(Regex r);
        void SetReplacement(ITextReplacement r);
        void SetMatchExtension(bool matchExtension);
    }
}