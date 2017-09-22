using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace NeXt.BulkRenamer.Models.Parsing
{
    internal class FileInfoResultPart : IResultPart
    {
        [DebuggerStepThrough]
        public static IResultPart CreationTime(string format)
        {
            return new FileInfoResultPart(format, (fi, fmt) => fi.CreationTime.ToString(fmt));
        }
        
        [DebuggerStepThrough]
        public static IResultPart CreationTimeUtc(string format)
        {
            return new FileInfoResultPart(format, (fi, fmt) => fi.CreationTimeUtc.ToString(fmt));
        }


        [DebuggerStepThrough]
        public static IResultPart LastWriteTime(string format)
        {
            return new FileInfoResultPart(format, (fi, fmt) => fi.LastWriteTime.ToString(fmt));
        }

        [DebuggerStepThrough]
        public static IResultPart LastWriteTimeUtc(string format)
        {
            return new FileInfoResultPart(format, (fi, fmt) => fi.LastWriteTimeUtc.ToString(fmt));
        }


        [DebuggerStepThrough]
        public static IResultPart LastAccessTime(string format)
        {
            return new FileInfoResultPart(format, (fi, fmt) => fi.LastAccessTime.ToString(fmt));
        }

        [DebuggerStepThrough]
        public static IResultPart LastAccessTimeUtc(string format)
        {
            return new FileInfoResultPart(format, (fi, fmt) => fi.LastAccessTimeUtc.ToString(fmt));
        }
        

        public static IResultPart Extension { get; } = new FileInfoResultPart(string.Empty, (fi, fmt) => fi.Extension);
        public static IResultPart DirectoryName { get; } = new FileInfoResultPart(string.Empty, (fi, fmt) => Path.GetDirectoryName(fi.DirectoryName));


        [DebuggerStepThrough]
        public static IResultPart Custom(string format, Func<FileInfo, string, string> result)
        {
            return new FileInfoResultPart(format, result);
        }

        [DebuggerStepThrough]
        private FileInfoResultPart(string format, Func<FileInfo, string, string> result)
        {
            this.format = format;
            resultFunction = result;
        }

        private readonly Func<FileInfo, string, string> resultFunction;
        private readonly string format;

        public string Process(GroupCollection matches, FileInfo file)
        {
            return resultFunction(file, format);
        }
    }
}