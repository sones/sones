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

/* <id name="PandoraDB – DBObject Readout" />
 * <copyright file="DBObjectReadout.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>Carries information of DBObjects but without their whole functionality.</summary>
 */

#region Usings

using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.PandoraTypes;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.Exceptions;

#endregion

namespace sones.GraphDB.QueryLanguage.Result
{
    /// <summary>
    /// Carries information of DBObjects but without their whole functionality.
    /// </summary>
    public class DBObjectReadout    
    {

        #region Properties

        public IDictionary<String, Object> Attributes { get; private set; }
 
        #endregion

        #region Constructor

        public DBObjectReadout()
        {
            Attributes = new Dictionary<String, Object>();
        }

        public DBObjectReadout(IDictionary<String, Object> myAttributes)
        {
            Attributes = myAttributes;
        }

        public DBObjectReadout(IDictionary<AttributeUUID, AObject> myAttributes, GraphDBType myTypeOfDBObject)
        {
            Attributes = myAttributes.ToDictionary(
                            key   => myTypeOfDBObject.GetTypeAttributeByUUID(key.Key).Name,
                            value => value.Value.GetReadoutValue());
        }

        #endregion


        #region this[myAttribute]

        public Object this[String myAttribute]
        {

            get
            {

                Object _Object = null;

                Attributes.TryGetValue(myAttribute, out _Object);
                
                return _Object;

            }

        }

        #endregion


        #region ToString()

        public override String ToString()
        {

            var _ReturnValue = new StringBuilder(Attributes.Count + " Attributes: ");
            
            foreach (var _KeyValuePair in Attributes)
                _ReturnValue.Append(_KeyValuePair.Key + " = '" + _KeyValuePair.Value + "', ");
            
            _ReturnValue.Length = _ReturnValue.Length - 2;

            return _ReturnValue.ToString();

        }

        #endregion

    }

}
