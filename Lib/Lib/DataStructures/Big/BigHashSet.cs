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

/* GraphLib - BigHashSet
 * (c) Daniel Kirstenpfad, 2009
 * 
 * This class implements a generic HashSet datastructure which
 * can go beyond the 2 GB per object limit for objects in .NET.
 * It can do this by partitioning different smaller HashSets
 * into one management datastructure.
 * 
 * Lead programmer:
 *      Daniel Kirstenpfad
 * 
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Lib.DataStructures.Big
{
    /// <summary>
    /// This is a BigHashSet class for the .NET generic HashSet data structure.
    /// </summary>
    /// <typeparam name="T">the type of this generic HashSet</typeparam>
    public class BigHashSet<T>
    {
        private ulong _Length;
        private List<HashSet<T>> _Data;
        private HashSet<T> _DataElement;
        private bool previouslyAdded = false;
        private int _MaximumElementsPerPartition = 1000000; // 1 Mio elements by default

        #region Constructors
        /// <summary>
        /// the simple constructor, initalizes with the default value for MaximumElementsPerPartition
        /// </summary>
        public BigHashSet()
        {
            _Data = new List<HashSet<T>>();
            _DataElement = new HashSet<T>();
            _Length = 0;
        }

        /// <summary>
        /// initializes the data structure with a user defined MaximumNumberOfElementsPerPartition.
        /// Important: Make sure that you're Data is going to be smaller than 2 Gbyte of memory per
        /// Partition.
        /// </summary>
        /// <param name="MaximumNumberOfElementsPerPartition"></param>
        public BigHashSet(int MaximumNumberOfElementsPerPartition)
        {
            _Data = new List<HashSet<T>>();
            _DataElement = new HashSet<T>();
            _MaximumElementsPerPartition = MaximumNumberOfElementsPerPartition;
            _Length = 0;
        }
        #endregion

        public int MaximumElementsPerPartition { get { return _MaximumElementsPerPartition; } }

        public ulong Length { get { return _Length; } }

        public ulong Count { get { return _Length; } }

        public int PartitionCount
        {
            get
            {
                if (!previouslyAdded)
                    return _Data.Count + 1;
                else
                    return _Data.Count;
            }
        }

        private HashSet<T> CurrentDataElementHashSet
        {
            get { return _DataElement; }
        }


        public bool Add(T element)
        {
            CurrentDataElementHashSet.Add(element);
            _Length++;

            #region MaximumElementsPerPartition reached...
            if (CurrentDataElementHashSet.Count == _MaximumElementsPerPartition)
            {
                if (!previouslyAdded)
                {
                    _Data.Add(CurrentDataElementHashSet);
                }

                // switch to the next DataElementHashSet which is not filled up or create a new one
                foreach (HashSet<T> _HashSet in _Data)
                {
                    if (_HashSet.Count < _MaximumElementsPerPartition)
                    {
                        // found a HashSet which is not filled up
                        previouslyAdded = true;
                        _DataElement = _HashSet;
                        return true;
                    }
                }

                // we only get here if we filled up a HashSet and haven't found any HashSet that can
                // take more elements elsewhere

                previouslyAdded = false;

                _DataElement = new HashSet<T>();
            }
            #endregion

            return true;
        }

        public bool Contains(T element)
        {
            if (CurrentDataElementHashSet.Contains(element))
                return true;
            else
            {
                foreach (HashSet<T> _tDataElement in _Data)
                {
                    if (_tDataElement.Contains(element))
                        return true;
                }
            }
            return false;
        }

        public bool Remove(T element)
        {
            if (CurrentDataElementHashSet.Remove(element))
            {
                _Length--;
                return true;
            }
            else
                foreach (HashSet<T> _tDataElement in _Data)
                {
                    if (_tDataElement.Remove(element))
                    {
                        _Length--;
                        return true;
                    }
                }

            return false;
        }
    }
}
