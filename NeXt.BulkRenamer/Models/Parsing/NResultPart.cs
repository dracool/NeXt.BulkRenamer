using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace NeXt.BulkRenamer.Models.Parsing
{
    internal class NResultPart : IResultPart
    {
        [DebuggerStepThrough]
        public NResultPart(string format, int startValue = 0)
        {
            this.format = format ?? "G";
            current = startValue - 1;
        }

        private int current;
        private readonly string format;

        public virtual string Process(GroupCollection matches, FileInfo file)
        {
            return Interlocked.Increment(ref current).ToString(format);
        }

        public override string ToString()
        {
            return $"{current.ToString(format)} [Consecutive]";
        }
    }
}