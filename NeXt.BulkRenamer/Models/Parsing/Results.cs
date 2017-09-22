using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeXt.BulkRenamer.Models.Parsing
{
    internal static class Results
    {
        [DebuggerStepThrough]
        public static IResultPart CreateDefaultTag(string identifier, IEnumerable<DefaultTagFormat> formats)
        {
            if (int.TryParse(identifier, out var index)) return new DefaultIndexedTagResultPart(index, formats);
            return new DefaultNamedTagResultPart(identifier, formats);
        }
        
        public static IResultPart CreateSpecialTag(string name, string format)
        {
            switch (name.ToLowerInvariant())
            {
                case "n": return new NResultPart(format);
                case "n1": return new NResultPart(format, 1);
                case "now": return new DateResultPart(format, fi => DateTime.Now);
                case "creation": return new DateResultPart(format, fi => fi.CreationTime);
                case "creationutc": return new DateResultPart(format, fi => fi.CreationTimeUtc);
                case "lastwrite": return new DateResultPart(format, fi => fi.LastWriteTime);
                case "lastwriteutc": return new DateResultPart(format, fi => fi.LastWriteTimeUtc);
                case "lastaccess": return new DateResultPart(format, fi => fi.LastAccessTime);
                case "lastaccessutc": return new DateResultPart(format, fi => fi.LastAccessTimeUtc);
                default: throw new InvalidOperationException($"The special tag does not exist: {name}");
            }
        }

    }
}
