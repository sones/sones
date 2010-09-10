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

/* <id name="PandoraDB - atom value" />
 * <copyright file="AtomValue.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>This class implements an atom value.</summary>
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.QueryLanguage.Enums;
using sones.GraphDB.QueryLanguage.Operator;
using sones.GraphDB.TypeManagement.PandoraTypes;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.Exceptions;

#endregion

namespace sones.GraphDB.QueryLanguage.Operators
{
    /// <summary>
    /// This class implements an atom value.
    /// </summary>
    public class AtomValue : IOperationValue
    {
        #region Properties

        TypesOfOperatorResult _typeOfValue;
        ADBBaseObject _value = null;

        #endregion

        #region constructor

        public AtomValue(TypesOfOperatorResult TypeOfValue, Object Value)
        {

            _value = GraphDBTypeMapper.GetPandoraObjectFromType(TypeOfValue, Value);

            _typeOfValue = TypeOfValue;
        }

        public AtomValue(ADBBaseObject myValue)
        {
            _value = myValue;

            _typeOfValue = myValue.Type;
        }
        
        public AtomValue(AObject myValue)
        {

            if (myValue is ADBBaseObject)
            {
                _value = myValue as ADBBaseObject;
                _typeOfValue = _value.Type;
            }
            else if (myValue is ASingleReferenceEdgeType)
            {
                _value = new DBReference(myValue);
            }
            else
            {
                throw new GraphDBException(new Errors.Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
            }

        }
        
        #endregion

        #region Accessors

        public ADBBaseObject Value { get { return _value; } }

        #endregion

        #region IOperationValue Members

        public TypesOfOperatorResult TypeOfValue
        {
            get { return _typeOfValue; }
        }

        public IEnumerable<ADBBaseObject> GetAllValues()
        {
            yield return _value;
            yield break;
        }

        #endregion
    }
}
