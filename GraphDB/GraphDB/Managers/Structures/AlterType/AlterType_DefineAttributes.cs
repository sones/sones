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
 * AlterType_DefineAttributes
 * (c) Stefan Licht, 2010
 */

#region Usings

using System;
using System.Collections.Generic;

using sones.GraphDB.Errors;
using sones.GraphDB.NewAPI;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.Structures.Enums;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.Structures.ExpressionGraph;
using sones.GraphDB.Errors.AttributeAssignmentErrors;

using sones.Lib;
using sones.Lib.ErrorHandling;

#endregion

namespace sones.GraphDB.Managers.AlterType
{

    public class AlterType_DefineAttributes : AAlterTypeCommand
    {

        #region data

        private List<AttributeDefinition> _ListOfAttributes;

        #endregion

        public override TypesOfAlterCmd AlterType
        {
            get { throw new NotImplementedException(); }
        }

        public AlterType_DefineAttributes(List<AttributeDefinition> listOfAttributes)
        {
            _ListOfAttributes = listOfAttributes;
        }

        public override Exceptional Execute(DBContext dbContext, GraphDBType graphDBType)
        {
            var listOfTypeAttributes = new Dictionary<TypeAttribute, GraphDBType>();
            var retExcept = new Exceptional();
            var existingTypeAttributes = graphDBType.GetAllAttributes(dbContext);

            foreach (var attr in _ListOfAttributes)
            {
                var createExcept = attr.CreateTypeAttribute(dbContext);

                if (!createExcept.Success())
                {
                    retExcept.PushIExceptional(createExcept);
                }

                if (existingTypeAttributes.Exists(item => item.Name == createExcept.Value.Name))
                {
                    retExcept.PushIExceptional(new Exceptional(new Error_AttributeAlreadyExists(createExcept.Value.Name)));
                }

                var attrType = dbContext.DBTypeManager.GetTypeByName(attr.AttributeType.Name);

                if (attrType == null)
                {
                    retExcept.PushIExceptional(new Exceptional(new Error_TypeDoesNotExist(attr.AttributeType.Name)));
                    return retExcept;
                }

                if (attrType.IsUserDefined)
                {
                    retExcept.PushIExceptional(new Exceptional(new Error_InvalidReferenceAssignmentOfUndefAttr()));
                    return retExcept;
                }

                createExcept.Value.DBTypeUUID = attrType.UUID;
                createExcept.Value.RelatedGraphDBTypeUUID = graphDBType.UUID;

                graphDBType.AddAttribute(createExcept.Value, dbContext.DBTypeManager, true);

                var flushExcept = dbContext.DBTypeManager.FlushType(graphDBType);

                if (!flushExcept.Success())
                {
                    retExcept.PushIExceptional(flushExcept);
                }

                listOfTypeAttributes.Add(createExcept.Value, attrType);
            }

            var dbobjects = dbContext.DBObjectCache.SelectDBObjectsForLevelKey(new LevelKey(graphDBType, dbContext.DBTypeManager), dbContext);

            foreach (var item in dbobjects)
            {
                if (!item.Success())
                {
                    retExcept.PushIExceptional(item);
                }
                else
                {
                    var undefAttrExcept = item.Value.GetUndefinedAttributePayload(dbContext.DBObjectManager);

                    if (!undefAttrExcept.Success())
                    {
                        retExcept.PushIExceptional(undefAttrExcept);
                    }

                    foreach (var attr in listOfTypeAttributes)
                    {
                        IObject value;

                        if (undefAttrExcept.Value.TryGetValue(attr.Key.Name, out value))
                        {
                            var typeOfOperator = GraphDBTypeMapper.ConvertGraph2CSharp(attr.Value.Name);

                            if (GraphDBTypeMapper.IsAValidAttributeType(attr.Value, typeOfOperator, dbContext, value))
                            {
                                item.Value.AddAttribute(attr.Key.UUID, value);

                                var removeExcept = item.Value.RemoveUndefinedAttribute(attr.Key.Name, dbContext.DBObjectManager);

                                if (!removeExcept.Success())
                                {
                                    retExcept.PushIExceptional(removeExcept);
                                }

                                var flushExcept = dbContext.DBObjectManager.FlushDBObject(item.Value);

                                if (!flushExcept.Success())
                                {
                                    retExcept.PushIExceptional(flushExcept);
                                }
                            }
                            else
                            {
                                retExcept.PushIExceptional(new Exceptional(new Error_InvalidUndefAttrType(attr.Key.Name, attr.Value.Name)));
                            }
                        }
                    }
                }
            }

            return Exceptional.OK;
        }

        public override IEnumerable<Vertex> CreateVertex(DBContext dbContext, GraphDBType graphDBType)
        {
            return base.CreateVertex(dbContext, graphDBType);
        }

    }

}
