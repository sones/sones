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
 * AttributeDefinition
 * (c) Stefan Licht, 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.TypeManagement;
using sones.GraphDBInterface.TypeManagement;
using sones.Lib.ErrorHandling;
using sones.GraphDB.TypeManagement.BasicTypes;

#endregion

namespace sones.GraphDB.Managers.Structures
{
    public class AttributeDefinition
    {

        #region Properties

        public String AttributeName { get; private set; }
        public DBTypeOfAttributeDefinition AttributeType { get; private set; }
        public IObject DefaultValue { get; private set; }

        #endregion

        #region Ctor

        public AttributeDefinition(DBTypeOfAttributeDefinition attributeType, String attributeName, IObject defaultValue = null)
        {

            AttributeType = attributeType;
            AttributeName = attributeName;
            DefaultValue = defaultValue;

        }

        #endregion

        #region CreateTypeAttribute(DBContext dbContext)

        internal Exceptional<TypeAttribute> CreateTypeAttribute(DBContext currentDBContext, List<GraphDBType> newTypes = null, UInt16 walkingNumber = 0)
        {

            //is this the right place?? Shouldn't this rather be checked in the usual attribute exist check?
            if (currentDBContext.DBSettingsManager.HasSetting(AttributeName))
            {
                throw new GraphDBException(new Error_AttributeAlreadyExists(AttributeName));
            }

            var typeManager = currentDBContext.DBTypeManager;
            var _TypeAttribute = new TypeAttribute(Convert.ToUInt16(walkingNumber + DBConstants.DefaultTypeAttributeIDStart));

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
                    if (_TypeAttribute.EdgeType is ASetOfReferencesEdgeType)
                    {
                        _TypeAttribute.KindOfType = KindsOfType.SetOfReferences;
                    }
                    else if (_TypeAttribute.EdgeType is ASingleReferenceEdgeType)
                    {
                        _TypeAttribute.KindOfType = KindsOfType.SingleReference;
                    }
                    else
                    {
                        return new Exceptional<TypeAttribute>(new Error_InvalidEdgeType(_TypeAttribute.EdgeType.GetType(), typeof(ASetOfReferencesEdgeType), typeof(ASingleReferenceEdgeType)));
                    }
                }
                else
                {
                    if (_TypeAttribute.EdgeType is ASetOfBaseEdgeType)
                    {
                        _TypeAttribute.KindOfType = KindsOfType.SetOfNoneReferences;
                    }
                    else if (_TypeAttribute.EdgeType is AListOfBaseEdgeType)
                    {
                        _TypeAttribute.KindOfType = KindsOfType.ListOfNoneReferences;
                    }
                    {
                        return new Exceptional<TypeAttribute>(new Error_InvalidEdgeType(_TypeAttribute.EdgeType.GetType(), typeof(ASetOfBaseEdgeType), typeof(AListOfBaseEdgeType)));
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
                if (DefaultValue is IEdgeType)
                {
                    if (_TypeAttribute.EdgeType.EdgeTypeUUID != (DefaultValue as IEdgeType).EdgeTypeUUID)
                    {
                        throw new GraphDBException(new Error_InvalidAttrDefaultValueAssignment(_TypeAttribute.Name, (DefaultValue as IEdgeType).EdgeTypeName, _TypeAttribute.EdgeType.EdgeTypeName));
                    }
                }
                else if (DefaultValue is ADBBaseObject)
                {
                    var attrVal = GraphDBTypeMapper.GetGraphObjectFromTypeName(AttributeType.Name);

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
