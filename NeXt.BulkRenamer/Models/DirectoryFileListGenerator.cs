using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace NeXt.BulkRenamer.Models
{
    internal class DirectoryFileListGenerator : IFileListGenerator
    {
        private readonly Lazy<IEnumerable<FileInfo>> result;

        public DirectoryFileListGenerator(string directoryPath, bool recursive)
        {
            result = new Lazy<IEnumerable<FileInfo>>(
                () => Directory.EnumerateFiles(directoryPath, "*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).Select(fn => new FileInfo(fn)),
                LazyThreadSafetyMode.ExecutionAndPublication
            );
        }

        public IEnumerable<FileInfo> Generate()
        {
            return result.Value;
        }
    }
}