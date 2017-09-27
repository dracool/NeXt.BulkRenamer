using System;
using System.IO;

namespace NeXt.BulkRenamer.Models.Background
{
    internal static class FileInfoExtensions
    {
        public static string NameWithoutExtension(this FileInfo @this)
        {
            if (@this == null) throw new ArgumentNullException(nameof(@this));
            return Path.GetFileNameWithoutExtension(@this.Name);
        }
        
        public static void Rename(this FileInfo @this, string name)
        {
            if (@this == null) throw new ArgumentNullException(nameof(@this));
            if (name == null) throw new ArgumentNullException(nameof(name));
            @this.MoveTo(Path.Combine(@this.DirectoryName, name));
        }
    }
}
