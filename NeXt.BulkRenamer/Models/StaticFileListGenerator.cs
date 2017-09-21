using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NeXt.BulkRenamer.Models
{
    internal class StaticFileListGenerator : IFileListGenerator
    {
        public StaticFileListGenerator(IEnumerable<string> values)
        {
            this.values = values.Select(fn => new FileInfo(fn));
        }

        private readonly IEnumerable<FileInfo> values;
        public IEnumerable<FileInfo> Generate() => values;
    }
}
