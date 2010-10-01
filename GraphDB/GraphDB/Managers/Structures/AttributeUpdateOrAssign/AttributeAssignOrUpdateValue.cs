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
 * AttributeAssignOrUpdateValue
 * (c) Stefan Licht, 2010
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using sones.GraphDB.Structures.Enums;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.TypeManagement;
using sones.Lib.ErrorHandling;

using sones.GraphDB.Errors;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.BasicTypes;


namespace sones.GraphDB.Managers.Structures
{

    #region AttributeAssignOrUpdateValue

    public class AttributeAssignOrUpdateValue : AAttributeAssignOrUpdate
    {

        #region Properties

        public Object Value { get; private set; }
        public BasicType AttributeAssignType { get; private set; }

        #endregion

        #region Ctor

        public AttributeAssignOrUpdateValue(IDChainDefinition myIDChainDefinition, BasicType myAttributeAssignType, Object myValue)
            : base(myIDChainDefinition)
        {
            this.Value = myValue;
            AttributeAssignType = myAttributeAssignType;
        }

        #endregion

        #region override AAttributeAssignOrUpdate.GetValueForAttribute

        public override Exceptional<IObject> GetValueForAttribute(DBObjectStream myDBObject, DBContext myDBContext, GraphDBType myGraphDBType)
        {

            if (AttributeIDChain.IsUndefinedAttribute)
            {
                return new Exceptional<IObject>(GraphDBTypeMapper.GetBaseObjectFromCSharpType(Value));
            }


            #region Simple value

            if (GraphDBTypeMapper.IsAValidAttributeType(AttributeIDChain.LastAttribute.GetDBType(myDBContext.DBTypeManager), AttributeAssignType, myDBContext, Value))
            {
                return new Exceptional<IObject>(GraphDBTypeMapper.GetGraphObjectFromType(AttributeAssignType, Value)); ;
            }
            else
            {
                return new Exceptional<IObject>(new Error_InvalidAttributeValue(AttributeIDChain.LastAttribute.Name, Value));
            }

            #endregion

        }

        #endregion

    }

    #endregion
}
