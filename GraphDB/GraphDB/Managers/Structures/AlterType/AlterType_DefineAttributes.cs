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

    /// <summary>
    /// Create defined attributes for undefined attributes
    /// </summary>
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

        /// <summary>
        /// Execute the definition of undefined attributes
        /// <seealso cref=" AAlterTypeCommand"/>
        /// </summary>        
        public override Exceptional Execute(DBContext myDBContext, GraphDBType myGraphDBType)
        {
            var listOfTypeAttributes = new Dictionary<TypeAttribute, GraphDBType>();
            var retExcept = new Exceptional();
            var existingTypeAttributes = myGraphDBType.GetAllAttributes(myDBContext);

            foreach (var attr in _ListOfAttributes)
            {
                var createExcept = attr.CreateTypeAttribute(myDBContext);

                if (!createExcept.Success())
                {
                    retExcept.PushIExceptional(createExcept);
                }

                if (existingTypeAttributes.Exists(item => item.Name == createExcept.Value.Name))
                {
                    retExcept.PushIExceptional(new Exceptional(new Error_AttributeAlreadyExists(createExcept.Value.Name)));
                }

                var attrType = myDBContext.DBTypeManager.GetTypeByName(attr.AttributeType.Name);

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
                createExcept.Value.RelatedGraphDBTypeUUID = myGraphDBType.UUID;

                myGraphDBType.AddAttribute(createExcept.Value, myDBContext.DBTypeManager, true);

                listOfTypeAttributes.Add(createExcept.Value, attrType);
            }

            var dbobjects = myDBContext.DBObjectCache.SelectDBObjectsForLevelKey(new LevelKey(myGraphDBType, myDBContext.DBTypeManager), myDBContext);

            foreach (var item in dbobjects)
            {
                if (!item.Success())
                {
                    retExcept.PushIExceptional(item);
                }
                else
                {
                    var undefAttrExcept = item.Value.GetUndefinedAttributePayload(myDBContext.DBObjectManager);

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

                            if (GraphDBTypeMapper.IsAValidAttributeType(attr.Value, typeOfOperator, myDBContext, value))
                            {
                                item.Value.AddAttribute(attr.Key.UUID, value);

                                var removeExcept = item.Value.RemoveUndefinedAttribute(attr.Key.Name, myDBContext.DBObjectManager);

                                if (!removeExcept.Success())
                                {
                                    retExcept.PushIExceptional(removeExcept);
                                }

                                var flushExcept = myDBContext.DBObjectManager.FlushDBObject(item.Value);

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

        /// <summary>
        /// <seealso cref=" AAlterTypeCommand"/>
        /// </summary>
        public override IEnumerable<Vertex> CreateVertex(DBContext dbContext, GraphDBType graphDBType)
        {
            return base.CreateVertex(dbContext, graphDBType);
        }
    }

}
