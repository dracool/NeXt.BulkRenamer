using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NeXt.BulkRenamer.Models.Background
{
    internal interface IBackgroundEngine
    {
        void UpdateMatchExtension(bool matchExtension);
        void UpdateRegex(Regex regex);
        void UpdateReplacement(IReplacement replacement);
        void UpdateTargets(IReadOnlyList<IReplacementTarget> targets);
    }
}