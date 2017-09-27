using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using NeXt.BulkRenamer.Utility;

namespace NeXt.BulkRenamer.Models.Background
{
    internal class BackgroundEngine : IBackgroundEngine
    {
        private void Execute(CancellationToken cancel)
        {
            if (targets == null) return;
            foreach (var target in targets)
            {
                if (cancel.IsCancellationRequested) return;
                ExecuteSingle(target, cancel);
            }
        }

        private void ExecuteSingle(IReplacementTarget target, CancellationToken cancel)
        {
            try
            {
                if (replacement == null || regex == null)
                {
                    target.Success = false;
                    return;
                }

                var text = replacement.Apply(
                    regex,
                    matchExtension ? target.SourceName : Path.GetFileNameWithoutExtension(target.SourceName),
                    target
                );
                if (!matchExtension) text += Path.GetExtension(target.SourceName);

                if (cancel.IsCancellationRequested) return;

                target.ResultName = text;
                target.Success = true;
            }
            catch (Exception)
            {
                target.Success = false;
            }
        }
        
        private readonly object runLock = new object();

        private IReadOnlyCollection<IReplacementTarget> targets;
        private IReplacement replacement;
        private bool matchExtension;
        private Regex regex;

        public void ExecuteFor(IReplacementTarget target)
        {
            ExecuteSingle(target, CancellationToken.None);
        }
       
        public void UpdateTargets(IReadOnlyCollection<IReplacementTarget> value)
        {
            targets = value;
            Invalidate();
        }
        public void UpdateReplacement(IReplacement value)
        {
            replacement = value;
            Invalidate();
        }
        public void UpdateRegex(Regex value)
        {
            regex = value;
            Invalidate();
        }
        public void UpdateMatchExtension(bool value)
        {
            matchExtension = value;
            Invalidate();
        }
        
        private CancellationTokenSource cts;
        private Task running;
        private void Invalidate()
        {
            lock (runLock)
            {
                cts?.Cancel();
                cts?.Dispose();
                cts = new CancellationTokenSource();
                running = Task.Run(() => Execute(cts.Token));
            }
        }
    }
}