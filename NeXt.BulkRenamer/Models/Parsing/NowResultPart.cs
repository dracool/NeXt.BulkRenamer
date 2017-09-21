using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace NeXt.BulkRenamer.Models.Parsing
{
    internal class DateResultPart : IResultPart
    {
        [DebuggerStepThrough]
        public DateResultPart(string format, Func<FileInfo, DateTime> source)
        {
            this.source = source;
            this.format = format ?? "yyyy-MM-dd";
        }

        private readonly Func<FileInfo, DateTime> source;
        private readonly string format;

        public virtual string Process(GroupCollection matches, FileInfo file)
        {
            return source(file).ToString(format);
        }

        public override string ToString()
        {
            return $"{format} [Now]";
        }
    }
}