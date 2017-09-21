using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeXt.BulkRenamer.Models.Background;

namespace NeXt.BulkRenamer.Models.Parsing
{
    internal class GrammarReplacementFactory : IReplacementFactory
    {
        public IReplacement Create(string pattern)
        {
            try
            {
                return new GrammarTextReplacement(pattern);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
