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

using System;
using System.Collections.Generic;
using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.QueryLanguage.Enums;
using sones.GraphDB.QueryLanguage.Operator;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.PandoraTypes;

namespace sones.GraphDB.QueryLanguage.Operators
{

    public enum KindOfTuple
    {
        Inclusive,
        LeftExclusive,
        RightExclusive,
        Exclusive
    }

    public class TupleValue : IOperationValue
    {
        #region Properties

        TypesOfOperatorResult _typeOfValue;
        List<ADBBaseObject> _values;
        KindOfTuple _kindOfTuple;

        #endregion

        #region constructor

        public TupleValue(KindOfTuple kindOfTuple = KindOfTuple.Inclusive)
        {
            _typeOfValue = TypesOfOperatorResult.Unknown;
            _values = new List<ADBBaseObject>();
            _kindOfTuple = kindOfTuple;
        }

        public TupleValue(TypesOfOperatorResult myTypesOfOperatorResult, Object myObject, GraphDBType myPandoraType, KindOfTuple kindOfTuple = KindOfTuple.Inclusive)
        {
            _typeOfValue = myTypesOfOperatorResult;
            _values = new List<ADBBaseObject>();
            _kindOfTuple = kindOfTuple;

            if (myObject is AListBaseEdgeType)
            {
                foreach (ADBBaseObject obj in (AListBaseEdgeType)myObject)
                {
                    _values.Add(obj);
                }
            }
            else throw new NotImplementedException(myObject.GetType().ToString());
        }

        #endregion

        #region Accessors

        public List<ADBBaseObject> Values { get { return _values; } }
        public KindOfTuple KindOfTuple { get { return _kindOfTuple; } }

        #endregion

        #region IOperationValue Members

        public TypesOfOperatorResult TypeOfValue
        {
            get { return _typeOfValue; }
        }

        public IEnumerable<ADBBaseObject> GetAllValues()
        {
            foreach (var aValue in _values)
            {
                yield return aValue;
            }

            yield break;
        }

        #endregion

        public void Add(ADBBaseObject aElement)
        {
            if (_typeOfValue == TypesOfOperatorResult.Unknown)
                _typeOfValue = aElement.Type;
            else
                if (_typeOfValue != aElement.Type)
                {
                    if (aElement.Type != TypesOfOperatorResult.Unknown)
                        throw new GraphDBException(new Error_DataTypeDoesNotMatch(_typeOfValue.ToString(), _typeOfValue.ToString()));

                    ADBBaseObject newElement = GraphDBTypeMapper.GetPandoraObjectFromType(_typeOfValue, aElement.Value);
                    if (newElement.CompareTo(aElement) != 0) // the value changed after type convertion
                        throw new GraphDBException(new Error_DataTypeDoesNotMatch(_typeOfValue.ToString(), _typeOfValue.ToString()));
                    aElement = newElement;
                }

            //_values.Add(PandoraTypeMapper.GetPandoraObjectFromType(_typeOfValue, aElement));
            _values.Add(aElement);
        }

        public void Union(TupleValue myTupleValue)
        {
            if (_typeOfValue == TypesOfOperatorResult.Unknown)
                _typeOfValue = myTupleValue.TypeOfValue;
            else
                if (_typeOfValue != myTupleValue.TypeOfValue)
                    throw new GraphDBException(new Error_DataTypeDoesNotMatch(myTupleValue.TypeOfValue.ToString(), _typeOfValue.ToString()));

            foreach (ADBBaseObject val in myTupleValue.Values)
            {
                if (!_values.Contains(val))
                    _values.Add(val);
            }
        }

        public void Remove(ADBBaseObject myElement)
        {
            if (_values.Contains(myElement))
                _values.Remove(myElement);
        }

        public void Remove(TupleValue myTupleValue)
        {
            foreach (ADBBaseObject val in myTupleValue.Values)
            {
                if (_values.Contains(val))
                    _values.Remove(val);
            }
        }

        public bool Contains(ADBBaseObject myElement)
        {
            return _values.Contains(myElement);
        }

        public void ConvertToAttributeType(TypeAttribute typeAttribute, DBContext myTypeManager)
        {
            for (int i = 0; i < _values.Count; i++)
            {
                _values[i] = GraphDBTypeMapper.GetADBBaseObjectFromUUID(typeAttribute.DBTypeUUID, _values[i]);
            }
        }
    }
}
