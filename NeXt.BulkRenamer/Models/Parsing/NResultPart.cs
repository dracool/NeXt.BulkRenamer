using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using NeXt.BulkRenamer.Models.Background;

namespace NeXt.BulkRenamer.Models.Parsing
{
    internal class NResultPart : IResultPart
    {
        [DebuggerStepThrough]
        public NResultPart(string format, int startValue = 0)
        {
            this.format = format ?? "G";
            this.startValue = startValue;
        }

        private readonly int startValue;
        private readonly string format;

        public virtual string Process(GroupCollection matches, IReplacementTarget target)
        {
            return (startValue + target.Index).ToString(format);
        }

        public override string ToString()
        {
            return $"{startValue.ToString(format)} [Consecutive]";
        }
    }
}