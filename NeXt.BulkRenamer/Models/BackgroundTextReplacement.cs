using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
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
        
        public void SetRegex(Regex r)
        {
            regex = r;
            foreach (var source in sources)
            {
                source.TrySetCanceled();
            }
            sources.Clear();
            invaliated.Raise(this, EventArgs.Empty);
        }

        public void SetReplacement(ITextReplacement r)
        {
            replacement = r;
            foreach (var source in sources)
            {
                source.TrySetCanceled();
            }
            sources.Clear();
            invaliated.Raise(this, EventArgs.Empty);
        }

        public void SetMatchExtension(bool m)
        {
            matchExtension = m;
            foreach (var source in sources)
            {
                source.TrySetCanceled();
            }
            sources.Clear();
            invaliated.Raise(this, EventArgs.Empty);
        }

        private bool matchExtension;

        public Task<string> RunAsync(string value)
        {
            var tcs = new TaskCompletionSource<string>();
            Task.Factory.StartNew(() => Execute(regex, replacement, value, tcs, matchExtension));
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
                    var ext = Path.GetExtension(value);
                    value = replacement.Apply(Path.ChangeExtension(value, null), regex);
                    value += ext;
                }

                tcs.TrySetResult(value);
            }
            catch (Exception)
            {
                tcs.TrySetResult(null);
            }
        }

        private readonly WeakEventSource<EventArgs> invaliated;
        public event EventHandler<EventArgs> Invalidated
        {
            add => invaliated.Subscribe(value);
            remove => invaliated.Unsubscribe(value);
        }
    }
}
