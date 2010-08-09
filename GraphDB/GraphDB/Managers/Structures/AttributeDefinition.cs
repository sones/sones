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


using sones.GraphDB.Structures.Result;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.Structures.Enums;
using sones.GraphDB.TypeManagement.BasicTypes;
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

            var graphDBType = typeManager.GetTypeByName(AttributeType.Name);
            var isReferenceEdge = false;

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
                        isReferenceEdge = true;
                    }
                    else
                    {
                        return new Exceptional<TypeAttribute>(new Error_TypeDoesNotExist(AttributeType.Name));
                    }
                }
                else
                {
                    return new Exceptional<TypeAttribute>(new Error_TypeDoesNotExist(AttributeType.Name));
                }
            }
            else
            {
                if (graphDBType.IsUserDefined)
                {
                    isReferenceEdge = true;
                }
            }


            #endregion

            #region Edge validation

            if (!String.IsNullOrEmpty(AttributeType.EdgeType))
            {

                if (!currentDBContext.DBPluginManager.HasEdgeType(AttributeType.EdgeType))
                {
                    return new Exceptional<TypeAttribute>(new Error_EdgeTypeDoesNotExist(AttributeType.EdgeType));
                }

                _TypeAttribute.EdgeType = currentDBContext.DBPluginManager.GetEdgeType(AttributeType.EdgeType);
                _TypeAttribute.EdgeType.ApplyParams(AttributeType.Parameters);

                #region Validate edge type

                if (isReferenceEdge)
                {
                    if (_TypeAttribute.EdgeType is ASetReferenceEdgeType)
                    {
                        _TypeAttribute.KindOfType = KindsOfType.SetOfReferences;
                    }
                    else if (_TypeAttribute.EdgeType is ASingleReferenceEdgeType)
                    {
                        _TypeAttribute.KindOfType = KindsOfType.SingleReference;
                    }
                    else
                    {
                        return new Exceptional<TypeAttribute>(new Error_InvalidEdgeType(_TypeAttribute.EdgeType.GetType(), typeof(ASetReferenceEdgeType), typeof(ASingleReferenceEdgeType)));
                    }
                }
                else
                {
                    if (_TypeAttribute.EdgeType is ASetBaseEdgeType)
                    {
                        _TypeAttribute.KindOfType = KindsOfType.SetOfNoneReferences;
                    }
                    else if (_TypeAttribute.EdgeType is AListBaseEdgeType)
                    {
                        _TypeAttribute.KindOfType = KindsOfType.ListOfNoneReferences;
                    }
                    {
                        return new Exceptional<TypeAttribute>(new Error_InvalidEdgeType(_TypeAttribute.EdgeType.GetType(), typeof(ASetBaseEdgeType), typeof(AListBaseEdgeType)));
                    }
                }

                #endregion

            }
            else
            {

                if (isReferenceEdge)
                {

                    if (AttributeType.Type == KindsOfType.UnknownList)
                    {
                        return new Exceptional<TypeAttribute>(new Error_ListAttributeNotAllowed(AttributeType.Name));
                    }
                    else if (AttributeType.Type == KindsOfType.UnknownSet)
                    {
                        _TypeAttribute.KindOfType = KindsOfType.SetOfReferences;
                        _TypeAttribute.EdgeType = new EdgeTypeSetOfReferences(null, graphDBType.UUID);
                    }
                    else if (AttributeType.Type == KindsOfType.UnknownSingle)
                    {
                        _TypeAttribute.KindOfType = KindsOfType.SingleReference;
                        _TypeAttribute.EdgeType = new EdgeTypeSingleReference(null, graphDBType.UUID);
                    }
                    else
                    {
                        //return new Exceptional<TypeAttribute>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                    }

                }
                else
                {

                    #region Basic type

                    if (AttributeType.Type == KindsOfType.UnknownList)
                    {
                        _TypeAttribute.KindOfType = KindsOfType.ListOfNoneReferences;
                        _TypeAttribute.EdgeType = new EdgeTypeListOfBaseObjects();
                    }
                    else if (AttributeType.Type == KindsOfType.UnknownSet)
                    {
                        _TypeAttribute.KindOfType = KindsOfType.SetOfNoneReferences;
                        _TypeAttribute.EdgeType = new EdgeTypeSetOfBaseObjects();
                    }
                    else if (AttributeType.Type == KindsOfType.UnknownSingle)
                    {
                        _TypeAttribute.KindOfType = KindsOfType.SingleNoneReference;
                    }
                    else
                    {
                        //return new Exceptional<TypeAttribute>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
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
                    if (_TypeAttribute.EdgeType.EdgeTypeUUID != (DefaultValue as AEdgeType).EdgeTypeUUID)
                    {
                        throw new GraphDBException(new Error_InvalidAttrDefaultValueAssignment(_TypeAttribute.Name, (DefaultValue as AEdgeType).EdgeTypeName, _TypeAttribute.EdgeType.EdgeTypeName));
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

            if (AttributeType.TypeCharacteristics != null)
            {
                _TypeAttribute.TypeCharacteristics = AttributeType.TypeCharacteristics;
            }


            #endregion

            return new Exceptional<TypeAttribute>(_TypeAttribute);

        }

        #endregion

    }
}
