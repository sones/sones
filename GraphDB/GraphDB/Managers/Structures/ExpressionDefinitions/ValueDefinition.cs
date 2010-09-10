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

/*
 * ValueDefinition
 * (c) Stefan Licht, 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Structures.Operators;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.GraphDB.Structures.Enums;

using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Errors;
using sones.GraphDBInterface.TypeManagement;

#endregion

namespace sones.GraphDB.Managers.Structures
{
    public class ValueDefinition : AOperationDefinition
    {

        #region Properties

        public ADBBaseObject Value { get; private set; }
        public Boolean IsDefined { get; private set; }

        #endregion

        #region Ctors

        public ValueDefinition(BasicType TypeOfValue, Object myValue)
        {
            Value = GraphDBTypeMapper.GetGraphObjectFromType(TypeOfValue, myValue);
            IsDefined = true;
        }

        public ValueDefinition(BasicType TypeOfValue, ADBBaseObject myValue)
        {
            Value = GraphDBTypeMapper.GetGraphObjectFromType(TypeOfValue, myValue.Value);
            IsDefined = true;
        }

        public ValueDefinition(ADBBaseObject myValue)
        {
            Value = myValue;
            IsDefined = true;
        }

        public ValueDefinition(ValueDefinition myValue)
        {
            Value = myValue.Value;
            IsDefined = myValue.IsDefined;
        }

        public ValueDefinition(Object myUndefinedValue)
        {
            Value = GraphDBTypeMapper.GetBaseObjectFromCSharpType(myUndefinedValue);
            IsDefined = false;
        }

        public ValueDefinition(IObject myValue)
        {

            if (myValue is ADBBaseObject)
            {
                Value = myValue as ADBBaseObject;
            }
            else if (myValue is ASingleReferenceEdgeType)
            {
                Value = new DBReference(myValue);
            }
            else
            {
                throw new GraphDBException(new Errors.Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
            }
            IsDefined = false;

        }

        #endregion

        #region ToString

        public override string ToString()
        {
            return Value.ToString();
        }
        
        #endregion

        #region ChangeType

        internal void ChangeType(AExpressionDefinition aExpressionDefinition)
        {
            if (aExpressionDefinition is IDChainDefinition)
            {
                ChangeType((aExpressionDefinition as IDChainDefinition).LastAttribute.DBTypeUUID);
            }
        }

        internal void ChangeType(TypeUUID typeUUID)
        {
            var val = GraphDBTypeMapper.GetADBBaseObjectFromUUID(typeUUID);
            if (!val.IsValidValue(Value.Value))
            {
                throw new GraphDBException(new Error_DataTypeDoesNotMatch(val.ObjectName, Value.ObjectName));
            }
            val.SetValue(Value.Value);
            Value = val;
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (obj is ValueDefinition)
            {
                return (obj as ValueDefinition).Value.Equals(Value);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

    }
}
