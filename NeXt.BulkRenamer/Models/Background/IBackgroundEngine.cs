using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NeXt.BulkRenamer.Utility;

namespace NeXt.BulkRenamer.Models.Background
{
    internal interface IBackgroundEngine
    {
        void UpdateMatchExtension(bool matchExtension);
        void UpdateRegex(Regex regex);
        void UpdateReplacement(IReplacement replacement);
        void UpdateTargets(IReadOnlyCollection<IReplacementTarget> targets);
        void ExecuteFor(IReplacementTarget target);
    }
}