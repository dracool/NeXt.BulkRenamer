using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace NeXt.BulkRenamer.Models.Parsing
{
    internal class DefaultNamedTagResultPart : IResultPart
    {
        [DebuggerStepThrough]
        public DefaultNamedTagResultPart(string identifier, IEnumerable<DefaultTagFormat> formats)
        {
            this.identifier = identifier;
            this.formats = formats.ToArray();
        }

        private readonly string identifier;
        private readonly IReadOnlyList<DefaultTagFormat> formats;

        public virtual string Process(GroupCollection matches, FileInfo file)
        {
            var val = matches[identifier].Value;

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
            return $"[\"{identifier}\"] = ({string.Join(", ", formats)})";
        }
    }
}