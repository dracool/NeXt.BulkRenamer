using System;
using System.Collections.Generic;
using System.Diagnostics;

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
                case "nroman": return new NRomanResultPart(format);
                case "nroman0": return new NRomanResultPart(format, 0);
                case "now": return new NowResultPart(format);
                case "nowutc": return new NowResultPart(DateTime.UtcNow, format);
                case "creation": return FileInfoResultPart.CreationTime(format);
                case "creationutc": return FileInfoResultPart.CreationTimeUtc(format);
                case "lastwrite": return FileInfoResultPart.LastWriteTime(format);
                case "lastwriteutc": return FileInfoResultPart.LastWriteTimeUtc(format); 
                case "lastaccess": return FileInfoResultPart.LastAccessTime(format);
                case "lastaccessutc": return FileInfoResultPart.LastAccessTimeUtc(format);
                case "ext":
                case "extension":
                    return FileInfoResultPart.Extension;
                case "dir":
                case "directory":
                case "dirname":
                    return FileInfoResultPart.DirectoryName;
                
                default: throw new InvalidOperationException($"The special tag does not exist: {name}");
            }
        }

    }
}
