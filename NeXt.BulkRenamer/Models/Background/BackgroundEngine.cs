using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace NeXt.BulkRenamer.Models.Background
{
    internal class BackgroundEngine : IBackgroundEngine
    {
        private class Job
        {
            public static Job Start(IReadOnlyList<IReplacementTarget> targets, IReplacement replacement, Regex regex, bool matchExtension)
            {
                void Execute(CancellationToken cancel)
                {
                    if (targets == null) return;

                    foreach (var target in targets)
                    {
                        try
                        {
                            if (cancel.IsCancellationRequested) return;
                            
                            if (replacement == null || regex == null)
                            {
                                target.Success = false;
                                continue;
                            }

                            var text = replacement.Apply(
                                regex,
                                matchExtension ? target.Source.Name : target.Source.NameWithoutExtension(),
                                target.Source
                            );
                            if (!matchExtension) text += target.Source.Extension;

                            if (cancel.IsCancellationRequested) return;

                            target.ResultName = text;
                            target.Success = true;
                        }
                        catch (Exception)
                        {
                            target.Success = false;
                        }
                    }
                }

                return new Job(Execute);
            }

            private Job(Action<CancellationToken> action)
            {
                cts = new CancellationTokenSource();
                task = Task.Run(() => action(cts.Token));
            }

            private readonly Task task;
            private readonly CancellationTokenSource cts;

            public void Cancel()
            {
                cts.Cancel();
            }
        }

        private IReadOnlyList<IReplacementTarget> targets;
        private IReplacement replacement;
        private Regex regex;
        private bool matchExtension;
        
        private readonly object runLock = new object();
        private Job running;
        
        public void UpdateTargets(IReadOnlyList<IReplacementTarget> value)
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

        private void Invalidate()
        {
            lock (runLock)
            {
                running?.Cancel();
                running = Job.Start(targets, replacement, regex, matchExtension);
            }
        }

    }
}