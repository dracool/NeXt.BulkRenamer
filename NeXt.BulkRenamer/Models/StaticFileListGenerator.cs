using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NeXt.BulkRenamer.Models
{
    internal class StaticFileListGenerator : IFileListGenerator
    {
        public StaticFileListGenerator(IEnumerable<string> values)
        {
            this.values = values;
        }

        private readonly IEnumerable<string> values;
        public IEnumerable<string> Generate() => values;
    }
}
