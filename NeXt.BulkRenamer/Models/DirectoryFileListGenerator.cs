using System.Collections.Generic;
using System.IO;

namespace NeXt.BulkRenamer.Models
{
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