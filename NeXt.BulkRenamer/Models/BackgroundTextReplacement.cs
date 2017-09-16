using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using WeakEvent;

namespace NeXt.BulkRenamer.Models
{
    internal class BackgroundTextReplacement : IBackgroundTextReplacement
    {
        private Regex regex;
        private ITextReplacement replacement;
        private readonly List<TaskCompletionSource<string>> sources;

        public BackgroundTextReplacement()
        {
            invaliated = new WeakEventSource<EventArgs>();
            sources = new List<TaskCompletionSource<string>>();
        }
        
        /// <summary>
        /// Sets the new regex and invalidates results
        /// </summary>
        public void SetRegex(Regex r)
        {
            regex = r;
            Invalidate();
        }

        /// <summary>
        /// Sets the new replacement and invalidates results
        /// </summary>
        public void SetReplacement(ITextReplacement r)
        {
            replacement = r;
            Invalidate();
        }
        
        /// <summary>
        /// Sets whether extensions are included in the regex and invalidates results
        /// </summary>
        public void SetMatchExtension(bool m)
        {
            matchExtension = m;
            Invalidate();
        }

        private bool matchExtension;

        /// <summary>
        /// Gets a new result value from the original value given
        /// </summary>
        public Task<string> RunAsync(string value)
        {
            var tcs = new TaskCompletionSource<string>();
            Task.Factory.StartNew(
                () => Execute(regex, replacement, value, tcs, matchExtension),
                CancellationToken.None,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default
            );
            sources.Add(tcs);
            return tcs.Task;
        }

        private static void Execute(Regex regex, ITextReplacement replacement, string value, TaskCompletionSource<string> tcs, bool matchExtension)
        {
            try
            {
                if (matchExtension)
                {
                    value = replacement.Apply(value, regex);
                }
                else
                {
                    //remove extension, run, re-add extension
                    var ext = Path.GetExtension(value);
                    value = replacement.Apply(Path.ChangeExtension(value, null), regex);
                    value += ext; //manually append to not mess up file names container dots
                }

                tcs.TrySetResult(value);
            }
            catch (Exception)
            {
                tcs.TrySetResult(null);
            }
        }

        /// <summary>
        /// invalidate current and in-progress results
        /// </summary>
        private void Invalidate()
        {
            foreach (var source in sources)
            {
                source.TrySetCanceled();
            }
            sources.Clear();
            invaliated.Raise(this, EventArgs.Empty);
        }

        private readonly WeakEventSource<EventArgs> invaliated;
        /// <summary>
        /// Invoked when the current or in progress result values are no longer valid due to property changes
        /// </summary>
        public event EventHandler<EventArgs> Invalidated
        {
            add => invaliated.Subscribe(value);
            remove => invaliated.Unsubscribe(value);
        }
    }
}
