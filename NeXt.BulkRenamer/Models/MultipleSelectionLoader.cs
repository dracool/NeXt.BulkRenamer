using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeXt.BulkRenamer.Models
{
    internal interface IFileListGenerator
    {
        IEnumerable<string> Generate();
    }

    internal class StaticFileListGenerator : IFileListGenerator
    {
        public StaticFileListGenerator(IEnumerable<string> values)
        {
            this.values = values;
        }

        private readonly IEnumerable<string> values;

        public IEnumerable<string> Generate() => values;
    }

    internal class DirectoryFileListGenerator : IFileListGenerator
    {
        private readonly string directoryPath;
        private readonly bool recursive;

        public DirectoryFileListGenerator(string directoryPath, bool recursive)
        {
            this.directoryPath = directoryPath;
            this.recursive = recursive;
        }

        public IEnumerable<string> Generate()
        {
            return Directory.EnumerateFiles(directoryPath, "*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
        }
    }
}
