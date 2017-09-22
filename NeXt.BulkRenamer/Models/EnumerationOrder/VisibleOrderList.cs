using System;
using System.Collections;
using System.Collections.Generic;
using NeXt.BulkRenamer.Utility;

namespace NeXt.BulkRenamer.Models.EnumerationOrder
{
    internal partial class VisibleOrderList<T> : IReadOnlyList<T>
    {
        public VisibleOrderList(IReadOnlyList<T> source)
        {
            this.source = source;
            VisibleRange = new VisibleRange(0, source.Count - 1);
        }

        private readonly IReadOnlyList<T> source;

        public IEnumerator<T> GetEnumerator()
        {
            return new VisibleOrderEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => source.Count;
        public T this[int index] => source[index];

        private VisibleRange visibleRange;

        public VisibleRange VisibleRange
        {
            get => visibleRange;
            set => visibleRange = value ?? throw new ArgumentNullException(nameof(value));
        }
    }

}
