using System;

namespace NeXt.BulkRenamer.Utility
{
    internal class VisibleRange
    {
        public static readonly VisibleRange None = new VisibleRange();

        private VisibleRange()
        {
            First = -1;
            Count = -1;
        }

        public VisibleRange(int first, int count)
        {
            if (first < 0) throw new ArgumentOutOfRangeException(nameof(first));
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));

            First = first;
            Count = count;
        }

        public int First { get; }
        public int Count { get; }

        public override string ToString()
        {
            return $"{{{First}, {Count}}}";
        }
    }
}