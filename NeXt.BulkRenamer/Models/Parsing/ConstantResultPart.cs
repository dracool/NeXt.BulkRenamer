using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using NeXt.BulkRenamer.Models.Background;

namespace NeXt.BulkRenamer.Models.Parsing
{
    internal class ConstantResultPart : IResultPart
    {
        [DebuggerStepThrough]
        public ConstantResultPart(string text)
        {
            this.text = text;
        }

        private readonly string text;

        public virtual string Process(GroupCollection matches, IReplacementTarget target) => text;

        public override string ToString()
        {
            return $"\"{text}\" [Constant]";
        }
    }
}