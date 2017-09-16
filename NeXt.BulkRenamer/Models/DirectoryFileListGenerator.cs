using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace NeXt.BulkRenamer.Models
{
    internal class DirectoryFileListGenerator : IFileListGenerator
    {
        private readonly Lazy<IEnumerable<string>> result;

        public DirectoryFileListGenerator(string directoryPath, bool recursive)
        {
            result = new Lazy<IEnumerable<string>>(
                () => Directory.EnumerateFiles(directoryPath, "*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly),
                LazyThreadSafetyMode.ExecutionAndPublication
            );
        }

        public IEnumerable<string> Generate()
        {
            return result.Value;
        }
    }
}