/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
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
*/


/*
 * ObjectExtentsList
 * Achim Friedland, 2008 - 2010
 */

#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using sones.Lib;

#endregion

namespace sones.StorageEngines
{

    /// <summary>
    /// A ObjectExtentsList maps a logical extent onto a list
    /// of consecutive physical extents.
    /// </summary>

    public class ObjectExtentsList : IEnumerable<ObjectExtent>
    {

        #region Data

        protected UInt64                    _ObjectExtentCount;
        protected LinkedList<ObjectExtent>  _ObjectExtents;

        #endregion

        #region Properties

        #region StreamLength

        /// <summary>
        /// The total length of the stream
        /// </summary>
        public UInt64 StreamLength
        {
            get
            {
                return TotalLength - _ReservedLength;
            }
        }

        #endregion

        #region ReservedLength

        protected UInt64 _ReservedLength;

        /// <summary>
        /// The number of reserved bytes
        /// </summary>
        public UInt64 ReservedLength
        {

            get
            {
                return _ReservedLength;
            }

            set
            {
                _ReservedLength = value;
            }

        }

        #endregion

        #region TotalLength

        /// <summary>
        /// The number of reserved bytes
        /// </summary>
        public UInt64 TotalLength
        {
            get
            {
                return (from _ObjectExtent in _ObjectExtents select _ObjectExtent.Length).Sum();
            }
        }

        #endregion

        #endregion

        #region Constructors

        #region ExtentsList()

        /// <summary>
        /// Basic constructor
        /// </summary>
        public ObjectExtentsList()
        {
            _ReservedLength             = 0;
            _ObjectExtents              = new LinkedList<ObjectExtent>();
            _ObjectExtentCount          = 0;
        }

        #endregion

        #region ExtentsList(myObjectExtent)

        /// <summary>
        /// Additional constructor
        /// </summary>
        public ObjectExtentsList(ObjectExtent myObjectExtent)
            : this()
        {
            Add2(myObjectExtent);
        }

        #endregion

        #region ExtentsList(myExtendedPosition, myDataLength)

        /// <summary>
        /// Additional constructor
        /// </summary>
        public ObjectExtentsList(IEnumerable<ExtendedPosition> myExtendedPosition, UInt64 myDataLength)
        {

            _ObjectExtents = new LinkedList<ObjectExtent>();

            foreach (var _ExtendedPosition in myExtendedPosition)
                _ObjectExtents.AddLast(new ObjectExtent(0, myDataLength, _ExtendedPosition));

        }

        #endregion

        #region ExtentsList(myExtentsList)

        /// <summary>
        /// Creates a new ObjectDatastream based on the content of myObjectDatastream
        /// </summary>
        /// <param name="myObjectDatastream">The ObjectDatastream to be cloned</param>
        public ObjectExtentsList(ObjectExtentsList myExtentsList)
        {

            if (myExtentsList == null)
                throw new ArgumentNullException();

            _ReservedLength             = myExtentsList.ReservedLength;
            _ObjectExtents              = new LinkedList<ObjectExtent>();

            foreach (var _ObjectExtent in myExtentsList)
                Add2(new ObjectExtent(_ObjectExtent));

        }

        #endregion

        #endregion


        #region Object-specific methods

        #region Add(myObjectExtent)

        public Boolean Add(ObjectExtent myObjectExtent)
        {

            _ObjectExtents.AddLast(myObjectExtent);
            _ObjectExtentCount++;
            return true;

        }

        public Boolean Add2(ObjectExtent myObjectExtent)
        {

            // Endposition = Startposition + Length

            #region Initial checks

            if (myObjectExtent == null)
                throw new ArgumentNullException();

            if (myObjectExtent.Length == 0)
                throw new ArgumentException("The lenght of the given ObjectExtent is invalid!");

            #endregion


            lock (this)
            {

                #region Adding the first extent

                if (_ObjectExtents.Count == 0)
                {

                    if (myObjectExtent.LogicalPosition != 0)
                        throw new ArgumentException("The first extent must start at LogicalPosition 0!");

                    _ObjectExtents.AddFirst(myObjectExtent);
                    _ObjectExtentCount++;

                    
                    
                        

                    return true;

                }

                #endregion

                #region Appending the given ObjectExtent

                else if (myObjectExtent.LogicalPosition == _ObjectExtents.Last.Value.LogicalPosition + _ObjectExtents.Last.Value.Length)
                {
                    _ObjectExtents.AddLast(myObjectExtent);
                    _ObjectExtentCount++;
                    
                    
                        
                    return true;
                }

                #endregion

                else
                {

                    var _ActualNode = _ObjectExtents.First;

                    do
                    {

                        #region Equals

                        // Equals:    |----------------ActualExtent----------------|
                        //            |---------------myObjectExtent---------------|
                        // Result:    |---------------myObjectExtent---------------|
                        if (myObjectExtent.LogicalPosition == _ActualNode.Value.LogicalPosition &&
                            myObjectExtent.Length          == _ActualNode.Value.Length)
                        {

                            // Previous Extent
                            if (_ActualNode.Previous != null)
                                _ActualNode.Previous.Value.NextExtent = new ExtendedPosition(myObjectExtent.StorageUUID, myObjectExtent.PhysicalPosition);

                            _ObjectExtents.AddAfter(_ActualNode, myObjectExtent);
                            _ObjectExtents.Remove(_ActualNode);
                            _ObjectExtentCount++;
                            
                            
                                
                            return true;

                        }

                        #endregion


                        #region Inner

                        // Inner:     |----------------ActualExtent----------------|
                        //            :            |--myObjectExtent--|            :
                        // Result:    |-OldExtent-||--myObjectExtent--||-NewExtent-|
                        // Tested by: OneBigExtent_AddingASmallInTheMiddle()
                        if (myObjectExtent.LogicalPosition > _ActualNode.Value.LogicalPosition &&
                            myObjectExtent.LogicalPosition + myObjectExtent.Length < _ActualNode.Value.LogicalPosition + _ActualNode.Value.Length)
                        {

                            // OldExtent
                            var OldExtent = new ObjectExtent();
                            OldExtent.LogicalPosition               = _ActualNode.Value.LogicalPosition;
                            OldExtent.Length                        = myObjectExtent.LogicalPosition - _ActualNode.Value.LogicalPosition;
                            OldExtent.PhysicalPosition              = _ActualNode.Value.PhysicalPosition;
                            OldExtent.StorageUUID                     = _ActualNode.Value.StorageUUID;
                            OldExtent.NextExtent                    = new ExtendedPosition(myObjectExtent.StorageUUID, myObjectExtent.PhysicalPosition);

                            // NewExtent
                            var NewExtent = new ObjectExtent();
                            NewExtent.LogicalPosition               = myObjectExtent.LogicalPosition + myObjectExtent.Length;
                            NewExtent.Length                        = _ActualNode.Value.LogicalPosition + _ActualNode.Value.Length - myObjectExtent.Length - OldExtent.Length;
                            NewExtent.PhysicalPosition              = _ActualNode.Value.PhysicalPosition + myObjectExtent.Length + OldExtent.Length;
                            NewExtent.StorageUUID                     = _ActualNode.Value.StorageUUID;
                            NewExtent.NextExtent                    = _ActualNode.Value.NextExtent;

                            // myObjectExtent
                            myObjectExtent.NextExtent               = new ExtendedPosition(NewExtent.StorageUUID, NewExtent.PhysicalPosition);

                            // Add all extents after the _ActualNode and remove it afterwards
                            _ObjectExtents.AddAfter(_ActualNode, NewExtent);
                            _ObjectExtents.AddAfter(_ActualNode, myObjectExtent);
                            _ObjectExtents.AddAfter(_ActualNode, OldExtent);
                            _ObjectExtents.Remove(_ActualNode);
                            _ObjectExtentCount++;
                            _ObjectExtentCount++;
                            
                            
                                
                            return true;

                        }

                        #endregion

                        #region InnerLeft

                        // InnerLeft: |----------------ActualExtent----------------|
                        //            |------myObjectExtent------|                 :
                        // Result:    |------myObjectExtent------||---NewExtent----|
                        // Tested by: OneBigExtent_AddingASmallOneIntheBeginning()
                        if (myObjectExtent.LogicalPosition == _ActualNode.Value.LogicalPosition &&
                            myObjectExtent.LogicalPosition + myObjectExtent.Length < _ActualNode.Value.LogicalPosition + _ActualNode.Value.Length)
                        {

                            // NewExtent
                            var NewExtent = new ObjectExtent();
                            NewExtent.LogicalPosition               = myObjectExtent.LogicalPosition + myObjectExtent.Length;
                            NewExtent.Length                        = _ActualNode.Value.LogicalPosition + _ActualNode.Value.Length - myObjectExtent.Length;
                            NewExtent.PhysicalPosition              = _ActualNode.Value.PhysicalPosition + myObjectExtent.Length;
                            NewExtent.StorageUUID                     = _ActualNode.Value.StorageUUID;
                            NewExtent.NextExtent                    = _ActualNode.Value.NextExtent;

                            // Previous Extent
                            if (_ActualNode.Previous != null)
                                _ActualNode.Previous.Value.NextExtent = new ExtendedPosition(myObjectExtent.StorageUUID, myObjectExtent.PhysicalPosition);

                            // myObjectExtent
                            myObjectExtent.NextExtent               = new ExtendedPosition(NewExtent.StorageUUID, NewExtent.PhysicalPosition);

                            // Add all extents after the _ActualNode and remove it afterwards
                            _ObjectExtents.AddAfter(_ActualNode, NewExtent);
                            _ObjectExtents.AddAfter(_ActualNode, myObjectExtent);
                            _ObjectExtents.Remove(_ActualNode);
                            _ObjectExtentCount++;
                            
                            
                                
                            return true;

                        }

                        #endregion

                        #region InnerRight

                        // InnerRight: |----------------ActualExtent----------------|
                        //             :                 |------myObjectExtent------|
                        // Result:     |---OldExtent----||------myObjectExtent------|
                        // Tested by: OneBigExtent_AddingASmallOneAtTheEnd()
                        if (myObjectExtent.LogicalPosition + myObjectExtent.Length == _ActualNode.Value.LogicalPosition + _ActualNode.Value.Length &&
                            myObjectExtent.LogicalPosition                          > _ActualNode.Value.LogicalPosition)
                        {

                            // OldExtent
                            var OldExtent = new ObjectExtent();
                            OldExtent.LogicalPosition               = _ActualNode.Value.LogicalPosition;
                            OldExtent.Length                        = myObjectExtent.LogicalPosition - _ActualNode.Value.LogicalPosition;
                            OldExtent.PhysicalPosition              = _ActualNode.Value.PhysicalPosition;
                            OldExtent.StorageUUID                     = _ActualNode.Value.StorageUUID;
                            OldExtent.NextExtent                    = new ExtendedPosition(myObjectExtent.StorageUUID, myObjectExtent.PhysicalPosition);

                            // myObjectExtent
                            myObjectExtent.NextExtent               = _ActualNode.Value.NextExtent;

                            // Add all extents after the _ActualNode and remove it afterwards
                            _ObjectExtents.AddAfter(_ActualNode, myObjectExtent);
                            _ObjectExtents.AddAfter(_ActualNode, OldExtent);
                            _ObjectExtents.Remove(_ActualNode);
                            _ObjectExtentCount++;
                                
                            return true;

                        }

                        #endregion


                        #region Bigger

                        // Bigger:   |------ActualExtent------|         :
                        //           |----------myObjectExtent----------|
                        // Result:   |XXXXXXX REMOVED XXXXXXXX|         :
                        //
                        // Bigger:   :     |------ActualExtent------|   :
                        //           |----------myObjectExtent----------|
                        // Result:         |XXXXXXX REMOVED XXXXXXXX|   :
                        //
                        // Bigger:   :         |------ActualExtent------|
                        //           |----------myObjectExtent----------|
                        // Result:   :         |XXXXXXX REMOVED XXXXXXXX|
                        if (_ActualNode.Value.LogicalPosition >= myObjectExtent.LogicalPosition &&
                            _ActualNode.Value.LogicalPosition + _ActualNode.Value.Length <= myObjectExtent.Length)
                        {


                            // Previous Extent
                            if (_ActualNode.Previous != null)
                                _ActualNode.Previous.Value.NextExtent = new ExtendedPosition(myObjectExtent.StorageUUID, myObjectExtent.PhysicalPosition);

                        }

                        #endregion


                        // Greater: |---ActualExtent---|               :
                        //          :               |--myObjectExtent--|
                        // Result:  |-ActualExtent-||--myObjectExtent--|


                        // Smaller: :               |---ActualExtent---|
                        //          |--myObjectExtent--|               :
                        // Result:  |--myObjectExtent--||-ActualExtent-|


                        _ActualNode = _ActualNode.Next;

                    } while (_ActualNode != null);

                }

                return true;
            
            }

        }

        #endregion

        #region Add(myObjectExtents)

        public Boolean Add(IEnumerable<ObjectExtent> myObjectExtents)
        {
            lock (this)
            {

                foreach (var _ObjectExtent in myObjectExtents)
                    Add(_ObjectExtent);                    

                return true;
            
            }
        }

        #endregion

        #region Replace(myObjectExtent, OldLength)

        public Boolean Replace(ObjectExtent myObjectExtent, UInt64 OldLength)
        {
            return false;
        }

        #endregion

        #region Remove(LogicalPosition, Length)

        public Boolean Remove(UInt64 LogicalPosition, UInt64 Length)
        {
            return false;
        }

        #endregion


        #region this[myNumber]

        public ObjectExtent this[UInt64 myNumber]
        {
            get
            {

                if (myNumber < _ObjectExtentCount)//_ObjectExtents.ULongCount())
                {

                    try
                    {

                        var _Node = _ObjectExtents.First;

                        for (var i = myNumber; i != 0; i--)
                            _Node = _Node.Next;

                        return _Node.Value;

                    }

                    catch (Exception e)
                    {
                        throw new ArgumentException("Something is wrong!" + e);
                    }

                }

                throw new ArgumentException("Parameter myNumber is invalid!");

            }

        }

        #endregion

        #region Count

        public UInt64 Count
        {
            get
            {
                //return _ObjectExtents.ULongCount();
                return _ObjectExtentCount;
            }
        }

        #endregion


        #region Duplicate()

        public void Duplicate()
        {

            var _NewObjectDatastream = new ObjectExtentsList();
            _NewObjectDatastream.ReservedLength             = _ReservedLength;

            foreach (var _ObjectExtent in _ObjectExtents)
                _NewObjectDatastream.Add2(new ObjectExtent(_ObjectExtent));

        }

        #endregion

        #endregion

        #region IEnumerable<ObjectExtent> Members

        public IEnumerator<ObjectExtent> GetEnumerator()
        {
            return _ObjectExtents.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _ObjectExtents.GetEnumerator();
        }

        #endregion


        #region ToString()

        public override String ToString()
        {

            var _StringBuilder = new StringBuilder();

            #region Write Length information

            _StringBuilder.Append("Length (T=S+R) => (").Append(TotalLength).Append("=").Append(StreamLength).Append("+").Append(ReservedLength).Append("), ");

            #endregion

            #region Write Extents information

            _StringBuilder.Append("Extents => (");

            foreach (var _ObjectExtent in _ObjectExtents)
                _StringBuilder.Append(_ObjectExtent.ToString()).Append(", ");

            _StringBuilder.Length = _StringBuilder.Length - 2;

            _StringBuilder.Append(")");

            #endregion

            return _StringBuilder.ToString();

        }

        #endregion

    }

}