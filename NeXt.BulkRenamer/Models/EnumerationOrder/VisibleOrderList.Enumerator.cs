using System;
using System.Collections;
using System.Collections.Generic;
using NeXt.BulkRenamer.Utility;

namespace NeXt.BulkRenamer.Models.EnumerationOrder
{
    internal partial class VisibleOrderList<T>
    {
        internal class VisibleOrderEnumerator : IEnumerator<T>
        {
            private class Position
            {
                public int Forward;
                public int Backward;
            }

            private class Bounds
            {
                public Bounds(int lower, int upper)
                {
                    Lower = lower;
                    Upper = upper;
                }

                public readonly int Lower;
                public readonly int Upper;
            }

            public VisibleOrderEnumerator(VisibleOrderList<T> owner)
            {
                this.owner = owner;
                items = owner.source;
                Reset();
            }
            
            private readonly VisibleOrderList<T> owner;
            private readonly IReadOnlyList<T> items;

            private VisibleRange visible;
            private bool isForward = true;

            private LinkedList<Bounds> bounds;
            private LinkedListNode<Bounds> forwardBound;
            private LinkedListNode<Bounds> backwardBound;

            private Position position;

            public bool MoveNext()
            {
                var uvis = owner.VisibleRange;
                while (visible != uvis)
                {
                    UpdateBounds();
                    RetargetScans(uvis);
                    visible = uvis;
                    uvis = owner.VisibleRange;
                }

                return isForward
                    ? ScanForward() || ScanBackward()
                    : ScanBackward() || ScanForward();
            }

            private bool ScanForward()
            {
                isForward = false; //reverse direction after every operation

                // skip over bounds
                position.Forward++;
                while (position.Forward >= forwardBound.Value.Lower)
                {
                    position.Forward = forwardBound.Value.Upper + 1;
                    forwardBound = forwardBound.Next;
                    if (forwardBound == null) throw new InvalidOperationException("Missing artificial upper bound");
                }

                // forward scan is done
                if (position.Forward >= items.Count) return false;

                Current = items[position.Forward];
                return true;
            }

            private bool ScanBackward()
            {
                isForward = true; //reverse direction after every operation

                // skip over bounds
                position.Backward--;
                while (position.Backward <= backwardBound.Value.Upper)
                {
                    position.Backward = backwardBound.Value.Lower - 1;
                    backwardBound = backwardBound.Previous;
                    if (backwardBound == null) throw new InvalidOperationException("Missing artificial lower bound");
                }

                // forward scan is done
                if (position.Backward < 0) return false;

                Current = items[position.Backward];
                return true;
            }

            private void RetargetScans(VisibleRange range)
            {
                //create new starting positions
                var approxMiddle = range.First + (range.Last - range.First) / 2 + 1;

                var newpos = new Position {Backward = approxMiddle - 1, Forward = approxMiddle};
                if (newpos.Backward < 0) newpos.Backward = 0;
                if (newpos.Forward >= items.Count) newpos.Forward = items.Count - 1;

                //skip ahead until out of any previous bounds
                position = newpos;
                RetargetForwardPosition();
                RetargetBackwardPosition();
            }

            private void RetargetForwardPosition()
            {
                var index = position.Forward;

                // ReSharper disable once AccessToModifiedClosure
                // doesn't matter, executes immediately
                var bound = bounds.LastNode(b => b.Lower <= index);

                while (bound.Value.Lower <= index && bound.Value.Upper >= index)
                {
                    index = bound.Value.Upper + 1;
                    bound = bound.Next;
                    if (bound == null) throw new InvalidOperationException("Missing artificial upper bound");
                }

                forwardBound = bound;
                position.Forward = index;
            }

            private void RetargetBackwardPosition()
            {
                var index = position.Backward;

                // ReSharper disable once AccessToModifiedClosure
                // doesn't matter, executes immediately
                var bound = bounds.LastNode(b => b.Lower <= index);

                while (bound.Value.Lower <= index && bound.Value.Upper >= index)
                {
                    index = bound.Value.Lower - 1;
                    bound = bound.Previous;
                    if (bound == null) throw new InvalidOperationException("Missing artificial lower bound");
                }

                backwardBound = bound;
                position.Backward = index;
            }

            private void UpdateBounds()
            {
                var start = position.Backward + 1;
                var end = position.Forward - 1;

                // if the end shows up as before the start, no changes were made, no need to change bounds
                if (end < start) return;

                var newBound = new Bounds(start, end);

                // find the first node that could intersect the new bound
                var node = bounds.FirstNode(b => b.Upper >= newBound.Lower);

                // if new bound doesn't intersect the candidate, it is free-standing
                // and can be added in front of th candidate
                if (node.Value.Lower > newBound.Upper)
                {
                    bounds.AddBefore(node, newBound);
                    return;
                }

                var beforeCandiate = node.Previous;
                if (beforeCandiate == null) throw new InvalidOperationException("Missing artificial lower bound");
                // if the intersect we need to clean all intersecting nodes following the candiate
                do
                {
                    var next = node.Next;
                    bounds.Remove(node);
                    node = next;
                    if (node == null) throw new InvalidOperationException("Missing artificial upper bound");
                } while (node.Value.Lower <= newBound.Upper); // the first non-intersecting nodes lower bound
                // must be higher than the new bounds end
                bounds.AddAfter(beforeCandiate, newBound); // lastly insert the new bounds in place of the intersected ones
            }

            public void Reset()
            {
                //initialize bonds for enumeration
                bounds = new LinkedList<Bounds>();
                bounds.AddFirst(new LinkedListNode<Bounds>(new Bounds(-1, -1)));
                bounds.AddLast(new LinkedListNode<Bounds>(new Bounds(items.Count, items.Count)));
                visible = owner.VisibleRange;
                RetargetScans(visible);
            }

            public void Dispose() { }

            public T Current { get; private set; }
            object IEnumerator.Current => Current;
        }
    }
}