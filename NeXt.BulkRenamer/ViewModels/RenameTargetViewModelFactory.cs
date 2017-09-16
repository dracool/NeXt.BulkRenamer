using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeXt.BulkRenamer.Models;

namespace NeXt.BulkRenamer.ViewModels
{
    internal class RenameTargetViewModelFactory : IRenameTargetViewModelFactory
    {
        private readonly IBackgroundTextReplacement replacer;

        public RenameTargetViewModelFactory(IBackgroundTextReplacement replacer)
        {
            this.replacer = replacer;
        }

        public RenameTargetViewModel Create(string value)
        {
            return new RenameTargetViewModel(value, replacer);
        }
    }
}
