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
 * AttributeAssignOrUpdateSetRef
 * (c) Stefan Licht, 2010
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Structures.Enums;
using sones.Lib.ErrorHandling;

using sones.GraphDB.ObjectManagement;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.Structures.EdgeTypes;

using sones.GraphDB.Errors;
using sones.GraphDBInterface.TypeManagement;

namespace sones.GraphDB.Managers.Structures
{

    #region AttributeAssignOrUpdateSetRef

    public class AttributeAssignOrUpdateSetRef : AAttributeAssignOrUpdate
    {

        #region Properties

        public SetRefDefinition SetRefDefinition { get; private set; }

        #endregion

        #region Ctor

        public AttributeAssignOrUpdateSetRef(IDChainDefinition myIDChainDefinition, SetRefDefinition mySetRefDefinition)
            : base(myIDChainDefinition)
        {
            SetRefDefinition = mySetRefDefinition;
        }

        #endregion

        #region override ToString

        public override string ToString()
        {
            return "SetRefNode";
        }
        
        #endregion

        #region override AAttributeAssignOrUpdate.GetValueForAttribute

        public override Exceptional<IObject> GetValueForAttribute(DBObjectStream aDBObject, DBContext dbContext, GraphDBType _Type)
        {

            #region reference

            var validationResult = AttributeIDChain.Validate(dbContext, true, _Type);
            if (validationResult.Failed())
            {
                return new Exceptional<IObject>(validationResult);
            }

            if (AttributeIDChain.IsUndefinedAttribute)
            {
                return new Exceptional<IObject>(new Error_InvalidReferenceAssignmentOfUndefAttr());
            }

            // if we have a Userdefined Type, than all assignments will work on this type
            if (!AttributeIDChain.LastAttribute.GetDBType(dbContext.DBTypeManager).IsUserDefined)
            {
                //attributeType = _Type;
            }

            var value = AttributeIDChain.LastAttribute.EdgeType.GetNewInstance();

            var dbos = SetRefDefinition.GetCorrespondigDBObjects(AttributeIDChain.LastAttribute.GetDBType(dbContext.DBTypeManager), dbContext, AttributeIDChain.LastAttribute.GetRelatedType(dbContext.DBTypeManager));

            foreach (var dbo in dbos)
            {
                if (dbo.Failed())
                    return new Exceptional<IObject>(dbo);

                (value as ASingleReferenceEdgeType).Set(dbo.Value.ObjectUUID, AttributeIDChain.LastAttribute.DBTypeUUID, SetRefDefinition.Parameters);
            }

            #endregion

            return new Exceptional<IObject>(value);

        } 

        #endregion

    }

    #endregion

}
