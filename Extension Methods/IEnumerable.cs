using System;
using System.Collections.Generic;
using System.Linq;

namespace MichaelAllenMiller.ExtensionMethods {
    public static class IEnumberableExtensions {
        /// <summary>
        /// Represents an Enumerated Element with additional properties about its context in the sequence
        /// </summary>
        public class ElementWithContext<T> {
            internal ElementWithContext(T previous, T current, T next, int index, IEnumerable<T> all, bool isLast) {
                Current = current;
                Previous = previous;
                Next = next;
                Index = index;
                All = all;
                IsLast = isLast;
            }

            /// <summary>All elements in the original order</summary>
            [Newtonsoft.Json.JsonIgnore]
            public IEnumerable<T> All { get; private set; }

            /// <summary>The element before the current element</summary>
            [Newtonsoft.Json.JsonIgnore]
            public T Previous { get; private set; }

            /// <summary>The current element</summary>
            public T Current { get; private set; }

            /// <summary>The element after the current element</summary>
            [Newtonsoft.Json.JsonIgnore]
            public T Next { get; private set; }

            /// <summary>The index of the current element</summary>
            public int Index { get; private set; }

            /// <summary>True if this is the first element</summary>
            public bool IsFirst { get { return Index == 0; } }

            /// <summary>True if this is the last element</summary>
            public bool IsLast { get; private set; }

            /// <summary>The elements before the current element</summary>
            [Newtonsoft.Json.JsonIgnore]
            public IEnumerable<T> Preceding { get { return All.Take(Index); } }

            /// <summary>The elements before the current element</summary>
            [Newtonsoft.Json.JsonIgnore]
            public IEnumerable<T> CurrentAndPreceding { get { return All.Take(Index + 1); } }

            /// <summary>The elements after the current element</summary>
            [Newtonsoft.Json.JsonIgnore]
            public IEnumerable<T> Following { get { return All.Skip(Index + 1); } }

            /// <summary>All elements except the current element</summary>
            [Newtonsoft.Json.JsonIgnore]
            public IEnumerable<T> Other { get { return Preceding.Concat(Following); } }

            public override string ToString() {
                if (Current == null) {
                    return "Current is null.";
                }
                return Current.ToString();
            }
        }


        /// <summary>
        /// Creates a sequence providing the sequence context with each element
        /// </summary>
        /// <param name="source">The input sequence</param>
        /// <returns>An output sequence with information about the context of each element</returns>
        static public IEnumerable<ElementWithContext<T>> WithContext<T>(this IEnumerable<T> source) {
            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }
            using var enumerator = source.GetEnumerator();
            // move to the first element
            if (!enumerator.MoveNext()) { yield break; }

            T previous = default;
            T current = enumerator.Current;
            var index = 0;

            // Continue from the seceond element 
            while (enumerator.MoveNext()) {
                T next = enumerator.Current;
                yield return new ElementWithContext<T>(previous, current, next, index, source, false);

                previous = current;
                current = next;
                index++;
            }

            // Return the last element
            yield return new ElementWithContext<T>(previous, current, default, index, source, true);
        }


        public static IEnumerable<T> CurrentValues<T>(this IEnumerable<ElementWithContext<T>> elementsWithContext) {
            if (elementsWithContext == null) {
                throw new ArgumentNullException(nameof(elementsWithContext));
            }
            return elementsWithContext.Select(e => e.Current);
        }
    }
}