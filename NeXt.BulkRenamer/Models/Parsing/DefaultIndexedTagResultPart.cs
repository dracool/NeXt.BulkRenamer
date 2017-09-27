using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NeXt.BulkRenamer.Models.Background;

namespace NeXt.BulkRenamer.Models.Parsing
{
    internal class DefaultIndexedTagResultPart : IResultPart
    {
        [DebuggerStepThrough]
        public DefaultIndexedTagResultPart(int index, IEnumerable<DefaultTagFormat> formats)
        {
            this.index = index;
            this.formats = formats.ToArray();
        }

        private readonly int index;
        private readonly IReadOnlyList<DefaultTagFormat> formats;

        public virtual string Process(GroupCollection matches, IReplacementTarget target)
        {
            var val = matches[index].Value;

            foreach (var fmt in formats)
            {
                switch (fmt)
                {
                    case DefaultTagFormat.UpperCase:
                        val = val.ToUpper();
                        break;
                    case DefaultTagFormat.LowerCase:
                        val = val.ToLower();
                        break;
                    case DefaultTagFormat.Capitalize:
                        if (val.Length == 1)
                        {
                            val = val.ToUpper();
                        }
                        else if (val.Length > 1)
                        {
                            val = val[0].ToString().ToUpper() + val.Substring(1);
                        }
                        break;
                    case DefaultTagFormat.Trim:
                        val = val.Trim();
                        break;
                    default: throw new ArgumentOutOfRangeException();
                }
            }

            return val;
        }

        public override string ToString()
        {
            return $"[{index}] = ({string.Join(", ", formats)})";
        }
    }
}