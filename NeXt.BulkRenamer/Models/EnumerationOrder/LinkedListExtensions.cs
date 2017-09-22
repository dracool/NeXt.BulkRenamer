using System;
using System.Collections.Generic;

namespace NeXt.BulkRenamer.Models.EnumerationOrder
{
    internal static class LinkedListExtensions
    {
        public static LinkedListNode<T> LastNode<T>(this LinkedList<T> @this, Func<T, bool> predicate)
        {
            var current = @this.Last;
            
            while (current != null)
            {
                if (predicate(current.Value)) return current;
                current = current.Previous;
            }

            return null;
        }

        public static LinkedListNode<T> FirstNode<T>(this LinkedList<T> @this, Func<T, bool> predicate)
        {
            var current = @this.First;

            while (current != null)
            {
                if (predicate(current.Value)) return current;
                current = current.Next;
            }

            return null;
        }
    }
}