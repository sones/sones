/*
 * AttributeDefinition
 * (c) Stefan Licht, 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Errors;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.QueryLanguage.Enums;
using sones.GraphDB.TypeManagement.PandoraTypes;
using sones.Lib.ErrorHandling;

#endregion

namespace sones.GraphDB.Managers.Structures
{
    public class AttributeDefinition
    {

        #region Properties

        public String AttributeName { get; private set; }
        public DBTypeOfAttributeDefinition AttributeType { get; private set; }
        public AObject DefaultValue { get; private set; }

        #endregion

        #region Ctor

        public AttributeDefinition(DBTypeOfAttributeDefinition attributeType, String attributeName, AObject defaultValue = null)
        {

            AttributeType = attributeType;
            AttributeName = attributeName;
            DefaultValue = defaultValue;

        }

        #endregion

        #region CreateTypeAttribute(DBContext dbContext)

        internal Exceptional<TypeAttribute> CreateTypeAttribute(DBContext currentDBContext, List<GraphDBType> newTypes = null)
        {

            //is this the right place?? Shouldn't this rather be checked in the usual attribute exist check?
            if (currentDBContext.DBSettingsManager.HasSetting(AttributeName))
            {
                throw new GraphDBException(new Error_AttributeAlreadyExists(AttributeName));
            }

            var typeManager = currentDBContext.DBTypeManager;
            var _TypeAttribute = new TypeAttribute();

            #region get Attribute Name

            _TypeAttribute.Name = AttributeName;

            #endregion

            #region Edge validation

            if (AttributeType.EdgeType == null && AttributeType.Type != KindsOfType.SingleNoneReference) // currently we don't have any edge for SingleNoneReference
            {

                #region No special edge - get the default edge

                var graphDBType = typeManager.GetTypeByName(AttributeType.Name);
                if (graphDBType == null || graphDBType.IsUserDefined) // This must be a user defined type - on bulk type creation the type does not exist currently
                {

                    #region Userdefined type

                    if (graphDBType == null)
                    {
                        //maybe there is an attribute with a new DBType
                        if (newTypes != null)
                        {
                            var newType = newTypes.Where(item => item.Name == AttributeType.Name).FirstOrDefault();
                            if (newType != null)
                            {
                                graphDBType = newType;
                            }
                            else
                            {
                                throw new GraphDBException(new Error_TypeDoesNotExist(AttributeType.Name));
                            }
                        }
                    }

                    if (AttributeType.Type == KindsOfType.UnknownList)
                    {
                        return new Exceptional<TypeAttribute>(new Error_ListAttributeNotAllowed(AttributeType.Name));
                    }
                    else if (AttributeType.Type == KindsOfType.UnknownSet)
                    {
                        AttributeType.Type = KindsOfType.SetOfReferences;
                        AttributeType.EdgeType = new EdgeTypeSetOfReferences(null, graphDBType.UUID);
                    }
                    else if (AttributeType.Type == KindsOfType.UnknownSingle)
                    {
                        AttributeType.Type = KindsOfType.SingleReference;
                        AttributeType.EdgeType = new EdgeTypeSingleReference(null, graphDBType.UUID);
                    }
                    else
                    {
                        //return new Exceptional<TypeAttribute>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                    }

                    #endregion

                }
                else
                {

                    #region Basic type

                    if (AttributeType.Type == KindsOfType.UnknownList)
                    {
                        AttributeType.Type = KindsOfType.ListOfNoneReferences;
                        AttributeType.EdgeType = new EdgeTypeListOfBaseObjects();
                    }
                    else if (AttributeType.Type == KindsOfType.UnknownSet)
                    {
                        AttributeType.Type = KindsOfType.SetOfNoneReferences;
                        AttributeType.EdgeType = new EdgeTypeSetOfBaseObjects();
                    }
                    else if (AttributeType.Type == KindsOfType.UnknownSingle)
                    {
                        AttributeType.Type = KindsOfType.SingleNoneReference;
                    }
                    else
                    {
                        //return new Exceptional<TypeAttribute>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                    }

                    #endregion

                }

                #endregion

            }
            else if (AttributeType.EdgeType != null)
            {

                #region Validate edge

                var graphDBType = typeManager.GetTypeByName(AttributeType.Name);
                if (graphDBType == null || graphDBType.IsUserDefined) // This must be a user defined type - on bulk type creation the type does not exist currently
                {
                    switch (AttributeType.Type)
                    {
                        case KindsOfType.SingleReference:
                            if (!(AttributeType.EdgeType is ASingleReferenceEdgeType))
                            {
                                return new Exceptional<TypeAttribute>(new Error_InvalidEdgeType(AttributeType.EdgeType.GetType(), typeof(ASingleReferenceEdgeType)));
                            }
                            break;
                        case KindsOfType.SetOfReferences:
                            if (!(AttributeType.EdgeType is ASetReferenceEdgeType))
                            {
                                return new Exceptional<TypeAttribute>(new Error_InvalidEdgeType(AttributeType.EdgeType.GetType(), typeof(ASetReferenceEdgeType)));
                            }
                            break;
                        case KindsOfType.SetOfNoneReferences:
                            if (!(AttributeType.EdgeType is ASetBaseEdgeType))
                            {
                                return new Exceptional<TypeAttribute>(new Error_InvalidEdgeType(AttributeType.EdgeType.GetType(), typeof(ASetBaseEdgeType)));
                            }
                            break;
                        case KindsOfType.ListOfNoneReferences:
                            if (!(AttributeType.EdgeType is AListBaseEdgeType))
                            {
                                return new Exceptional<TypeAttribute>(new Error_InvalidEdgeType(AttributeType.EdgeType.GetType(), typeof(AListBaseEdgeType)));
                            }
                            break;
                        case KindsOfType.SingleNoneReference:
                        default:
                            if (AttributeType.EdgeType != null)
                            {
                                return new Exceptional<TypeAttribute>(new Error_InvalidEdgeType(AttributeType.EdgeType.GetType()));
                            }
                            break;
                    }
                }

                #endregion

            }

            #endregion

            #region get Attribute type

            //we can not validate the type at this point, because in the bulk type creation we does not have the type
            var dbType = AttributeType.Name;

            //if we have an default value for this attribute, then check for correct value and attribute type
            if (DefaultValue != null)
            {
                if (DefaultValue is AEdgeType)
                {
                    if (AttributeType.EdgeType.EdgeTypeUUID != (DefaultValue as AEdgeType).EdgeTypeUUID)
                    {
                        throw new GraphDBException(new Error_InvalidAttrDefaultValueAssignment(_TypeAttribute.Name, (DefaultValue as AEdgeType).EdgeTypeName, AttributeType.EdgeType.EdgeTypeName));
                    }
                }
                else if (DefaultValue is ADBBaseObject)
                {
                    var attrVal = GraphDBTypeMapper.GetPandoraObjectFromTypeName(AttributeType.Name);

                    if (!attrVal.IsValidValue(DefaultValue))
                    {
                        throw new GraphDBException(new Error_InvalidAttrDefaultValueAssignment(_TypeAttribute.Name, dbType));
                    }
                }

                _TypeAttribute.DefaultValue = DefaultValue;
            }

            _TypeAttribute.KindOfType = AttributeType.Type;

            if (AttributeType.TypeCharacteristics != null)
            {
                _TypeAttribute.TypeCharacteristics = AttributeType.TypeCharacteristics;
            }

            _TypeAttribute.EdgeType = AttributeType.EdgeType;

            #endregion

            return new Exceptional<TypeAttribute>(_TypeAttribute);

        }

        #endregion

    }
}
