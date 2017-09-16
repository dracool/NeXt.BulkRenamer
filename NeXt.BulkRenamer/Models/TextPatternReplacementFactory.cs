using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeXt.BulkRenamer.Models
{
    internal class TextPatternReplacementFactory : ITextReplacementFactory
    {
        public ITextReplacement Create(string pattern)
        {
            return new TextPatternReplacement(pattern);
        }
    }
}
