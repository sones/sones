/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
* 
*/

////////////////////////////////////////////////////////////////////////////////////
// RangeSet.cs
//
// By Scott McMaster (smcmaste@hotmail.com)
// 04/21/2005
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections;
using System.Diagnostics;

//adapted the namespace
namespace sones.Lib.DataStructures
{

    /// <summary>
    /// Integer set that stores ranges rather than necessarily every single integer member.
    /// </summary>
    
    public class RangeSet : IEnumerable
    {

        #region Ranges

        /// <summary>
        /// Interface for a range in the rangeset.
        /// </summary>
        private interface IRange
        {
            /// <summary>
            /// The low end of the range.
            /// </summary>
            int Low
            {
                get;
            }

            /// <summary>
            /// The high end of the range.
            /// </summary>
            int High
            {
                get;
            }

            /// <summary>
            /// Whether or not the range contains the given element.
            /// </summary>
            /// <param name="element"></param>
            /// <returns></returns>
            bool Contains(int element);
        }

        /// <summary>
        /// The range structure.
        /// </summary>
        
        private struct Range : IRange
        {
            /// <summary>
            /// The low end of the range, inclusive.
            /// </summary>
            private int low;

            /// <summary>
            /// The high end of the range, inclusive.
            /// </summary>
            private int high;

            /// <summary>
            /// Create the given range.
            /// </summary>
            /// <param name="low"></param>
            /// <param name="high"></param>
            public Range(int low, int high)
            {
                Debug.Assert(low <= high, "Can't create a range with low > high");
                this.low = low;
                this.high = high;
            }
            #region IRange Members

            public int Low
            {
                get
                {
                    return low;
                }
            }

            public int High
            {
                get
                {
                    return high;
                }
            }

            public bool Contains(int elem)
            {
                return (elem >= low && elem <= high);
            }

            #endregion
        }

        /// <summary>
        /// The range structure for a single-element "range" -- an optimization over the standard range.
        /// </summary>
        
        private struct SingleElementRange : IRange
        {
            private int element;

            #region IRange Members

            public int Low
            {
                get
                {
                    return element;
                }
            }

            public int High
            {
                get
                {
                    return element;
                }
            }

            public bool Contains(int elem)
            {
                return (elem == element);
            }

            #endregion

            /// <summary>
            /// Create a range consisting of a single element.
            /// </summary>
            /// <param name="element"></param>
            public SingleElementRange(int element)
            {
                this.element = element;
            }
        }

        #endregion

        #region Iterator

        /// <summary>
        /// Inner class for enumerating the rangeset.
        /// </summary>
        private class RangeSetIterator : IEnumerator
        {
            /// <summary>
            /// The set we're enumerating.
            /// </summary>
            private RangeSet theSet;

            /// <summary>
            /// The current range we're iterating in.
            /// </summary>
            private IRange currentRange = null;

            /// <summary>
            /// The index of the current range we're iterating in.
            /// </summary>
            private int currentRangeIdx;

            /// <summary>
            /// The current element to return.
            /// </summary>
            private int currentElement;

            #region IEnumerator Members

            public void Reset()
            {
                currentRange = null;
                currentRangeIdx = -1;
            }

            public object Current
            {
                get
                {
                    if (currentRange == null)
                    {
                        // Haven't started yet.
                        return null;
                    }

                    return currentElement;
                }
            }

            private bool NextRange()
            {
                ++currentRangeIdx;
                if (currentRangeIdx < theSet.rangeList.Count)
                {
                    currentRange = (IRange)theSet.rangeList[currentRangeIdx];
                    currentElement = currentRange.Low;
                    return true;
                }

                return false;
            }

            public bool MoveNext()
            {
                if (currentRange == null)
                {
                    // Start me up.
                    return NextRange();
                }
                else
                {
                    ++currentElement;
                    if (currentElement > currentRange.High)
                    {
                        // Move to the next range.
                        return NextRange();
                    }
                    else
                    {
                        // The current element is a part of the current range, so we'll return it.
                        return true;
                    }
                }
            }

            #endregion

            /// <summary>
            /// Create an enumerator for the given rangeset.
            /// </summary>
            /// <param name="theSet"></param>
            public RangeSetIterator(RangeSet theSet)
            {
                this.theSet = theSet;
                Reset();
            }
        }

        #endregion

        /// <summary>
        /// The list we actually use to store the ranges.  It MUST be maintained sorted with disjoint ascending ranges.
        /// </summary>
        private ArrayList rangeList = new ArrayList();

        /// <summary>
        /// We track a cached count of all the elements in the set so we can have constant-time lookup.
        /// </summary>
        private int count = 0;

        /// <summary>
        /// Find the index of the range containing the given element currently in the list, if any.
        /// </summary>
        /// <param name="element">The element we're looking for.</param>
        /// <returns>The index of the containing range, or the appropriate insertion point (a negative number) if there isn't any.</returns>
        private int FindIndexOfRangeFor(int element)
        {
            if (rangeList.Count == 0)
            {
                return -1;
            }

            int low = 0;
            int high = rangeList.Count - 1;
            while (low <= high)
            {
                int middle = (low == high) ? low : ((low + high) / 2);
                IRange curRange = (IRange)rangeList[middle];

                if (curRange.Contains(element))
                {
                    return middle;
                }
                else if (curRange.Low > element)
                {
                    high = middle - 1;
                }
                else if (curRange.High < element)
                {
                    low = middle + 1;
                }
                else
                {
                    return -middle;
                }
            }

            // Standard data structures hack to indicate the appropriate insertion point as a part of this call.
            return -(low + 1);
        }

        /// <summary>
        /// Whether or not the set contains the given element.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public bool Contains(int element)
        {
            return (FindIndexOfRangeFor(element) >= 0);
        }

        /// <summary>
        /// Get the number of elements currently in the set.
        /// </summary>
        public int Count
        {
            get
            {
                return count;
            }
        }

        /// <summary>
        /// Add the given element to the set.
        /// </summary>
        /// <param name="element"></param>
        public void Add(int element)
        {
            int rangeIdx = FindIndexOfRangeFor(element);

            if (rangeIdx >= 0)
            {
                // Element is already there.
                return;
            }

            // We're going to add one, so up the count.
            ++count;

            int insertionPoint = -rangeIdx - 1;

            // Try to find a range we can expand to include this element.
            int rangeBelowIdx = FindIndexOfRangeFor(element - 1);
            int rangeAboveIdx = FindIndexOfRangeFor(element + 1);

            if (rangeBelowIdx >= 0)
            {
                IRange below = (IRange)rangeList[rangeBelowIdx];
                if (rangeAboveIdx >= 0)
                {
                    // We have ranges below and above, so we can merge two existing ranges.
                    rangeList[rangeBelowIdx] = new Range(below.Low, ((IRange)rangeList[rangeAboveIdx]).High);
                    rangeList.RemoveAt(rangeAboveIdx);
                }
                else
                {
                    // We don't have a range above, so just expand the existing range below.
                    rangeList[rangeBelowIdx] = new Range(below.Low, element);
                }
            }
            else if (rangeAboveIdx >= 0)
            {
                // We have an existing range above that we can expand downward.
                rangeList[rangeAboveIdx] = new Range(element, ((IRange)rangeList[rangeAboveIdx]).High);
            }
            else
            {
                // We need a new single-element range.
                IRange newRange = new SingleElementRange(element);
                rangeList.Insert(insertionPoint, newRange);
            }
        }

        /// <summary>
        /// Add a range of the appropriate type depending on whether the start and end of the range are equal.
        /// </summary>
        /// <param name="startElement"></param>
        /// <param name="endElement"></param>
        private void AddRange(int startElement, int endElement)
        {
            if (startElement == endElement)
            {
                rangeList.Add(new SingleElementRange(startElement));
            }
            else
            {
                rangeList.Add(new Range(startElement, endElement));
            }
        }

        /// <summary>
        /// Add the given elements under the assumption that the set is currently empty.
        /// If it's not, we throw.
        /// </summary>
        /// <param name="elements">The elements to add.</param>
        private void AddFromEmpty(int[] elements)
        {
            if (count > 0)
            {
                throw new InvalidOperationException("The set must be initially empty to use this Add() technique.");
            }

            if (elements.Length == 0)
            {
                // Nothing to do.
                return;
            }

            Array.Sort(elements);

            int startElement = elements[0];
            int lastElement = elements[0];
            count = 1;

            for (int currentElementIdx = 1; currentElementIdx < elements.Length; ++currentElementIdx)
            {
                if (elements[currentElementIdx] == lastElement)
                {
                    // Duplicate -- skip it.
                    continue;
                }

                // Else we need to bump the count.
                ++count;

                // Now see if we need to add a new range.
                if (elements[currentElementIdx] == lastElement + 1)
                {
                    // We're in the middle of a run -- continue.
                    ++lastElement;
                }
                else
                {
                    // We need to add a new range.
                    AddRange(startElement, lastElement);

                    // Move up.
                    startElement = lastElement = elements[currentElementIdx];
                }
            }

            // Have to add the last range.
            AddRange(startElement, lastElement);
        }

        /// <summary>
        /// Convenience method to add multiple items to the set in one call.
        /// </summary>
        /// <param name="elements"></param>
        public void Add(params int[] elements)
        {
            if (count == 0)
            {
                // If we don't have any elements yet, we can use the faster method that
                // builds the ranges in sorted order.
                AddFromEmpty(elements);
            }
            else
            {
                foreach (int element in elements)
                {
                    Add(element);
                }
            }
        }

        /// <summary>
        /// Compress to the minimum size.
        /// </summary>
        public void TrimToSize()
        {
            rangeList.TrimToSize();
        }

        /// <summary>
        /// Remove all elements from the set.
        /// </summary>
        public void Clear()
        {
            rangeList = new ArrayList();
            count = 0;
        }

        /// <summary>
        /// Remove the given element from the rangeset.
        /// </summary>
        /// <param name="element"></param>
        public void Remove(int element)
        {
            int rangeIdx = FindIndexOfRangeFor(element);
            if (rangeIdx < 0)
            {
                // It's not here, nothing to do.
                return;
            }

            // OK, we have it.  There are four cases.
            IRange range = (IRange)rangeList[rangeIdx];
            if (range.Low == element && range.High == element)
            {
                // 1.  It's part of a single-element range.  Remove the whole thing.
                rangeList.RemoveAt(rangeIdx);
            }
            else if (range.Low == element)
            {
                // 2.  We have a range that we need to shrink from the bottom.
                if (range.High == element + 1)
                {
                    rangeList[rangeIdx] = new SingleElementRange(range.High);
                }
                else
                {
                    rangeList[rangeIdx] = new Range(range.Low + 1, range.High);
                }
            }
            else if (range.High == element)
            {
                // 3.  We have a range that we need to shrink from the top.
                if (range.Low == element - 1)
                {
                    rangeList[rangeIdx] = new SingleElementRange(range.Low);
                }
                else
                {
                    rangeList[rangeIdx] = new Range(range.Low, range.High - 1);
                }
            }
            else
            {
                // 4.  Darn, we have to split a range.
                if (range.Low == element - 1)
                {
                    rangeList[rangeIdx] = new SingleElementRange(range.Low);
                }
                else
                {
                    rangeList[rangeIdx] = new Range(range.Low, element - 1);
                }

                if (range.High == element + 1)
                {
                    rangeList.Insert(rangeIdx + 1, new SingleElementRange(range.High));
                }
                else
                {
                    rangeList.Insert(rangeIdx + 1, new Range(element + 1, range.High));
                }
            }

            // We also have to make sure to decrement the cached count.
            --count;
        }

#if DEBUG
        public void DumpRanges()
        {
            foreach (IRange r in rangeList)
            {
                Debug.WriteLine(r.Low.ToString() + " - " + r.High.ToString());
            }
        }
#endif

        /// <summary>
        /// Create a new RangeSet initialized with the given elements.  Note that this is somewhat
        /// faster than successive Add() calls.
        /// </summary>
        /// <param name="elements"></param>
        public RangeSet(params int[] elements)
        {
            AddFromEmpty(elements);
        }

        /// <summary>
        /// Create a default empty rangeset.
        /// </summary>
        public RangeSet()
        {
        }

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return new RangeSetIterator(this);
        }

        #endregion
    }
}
