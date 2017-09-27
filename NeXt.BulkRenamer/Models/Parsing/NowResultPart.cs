using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using NeXt.BulkRenamer.Models.Background;

namespace NeXt.BulkRenamer.Models.Parsing
{
    internal class NowResultPart : IResultPart
    {
        [DebuggerStepThrough]
        public NowResultPart(string format)
        {
            date = DateTime.Now;
            this.format = format ?? "yyyy-MM-dd";
        }

        [DebuggerStepThrough]
        public NowResultPart(DateTime date, string format)
        {
            this.date = date;
            this.format = format ?? "yyyy-MM-dd";
        }

        private readonly DateTime date;
        private readonly string format;

        public virtual string Process(GroupCollection matches, IReplacementTarget target)
        {
            return date.ToString(format);
        }

        public override string ToString()
        {
            return $"{format} [Now]";
        }
    }
}