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
 * Henning Rauch, Stefan Licht, 2009 - 2010
 */

#region Usings

using System;
using System.Diagnostics;
using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.TypeManagement.BasicTypes;

using sones.Lib.ErrorHandling;

#endregion

namespace sones.GraphDB.TypeManagement
{

    /// <summary>
    /// Maps GraphDBTypes to C# types.
    /// </summary>

    public class GraphDBTypeMapper
    {

        public static BasicType GraphInteger          = BasicType.Int64;
        public static BasicType GraphInt32            = BasicType.Int32;
        public static BasicType GraphUnsignedInteger  = BasicType.UInt64;
        public static BasicType GraphString           = BasicType.String;
        public static BasicType GraphDouble           = BasicType.Double;
        public static BasicType GraphDateTime         = BasicType.DateTime;
        public static BasicType GraphBoolean          = BasicType.Boolean;

        public static Boolean IsBasicType(String typeName)
        {

            switch (typeName)
            {

                case DBConstants.DBInteger:
                    return true;

                case DBConstants.DBInt32:
                    return true;

                case DBConstants.DBUnsignedInteger:
                    return true;

                case DBConstants.DBString:
                    return true;

                case DBConstants.DBDouble:
                    return true;

                case DBConstants.DBDateTime:
                    return true;

                case DBConstants.DBBoolean:
                    return true;

                case "NumberLiteral":
                    return true;

                case "StringLiteral":
                    return true;

            }

            if (typeName.Contains(DBConstants.LIST_PREFIX))
                return true;

            return false;

        }

        /// <summary>
        /// Checks if the name of the attribute is valid for a certain GraphType.
        /// </summary>
        /// <param name="GraphTypeName">The GraphType.</param>
        /// <param name="myAttributeName">Name of the attribute.</param>
        /// <param name="typeOfAttributeValue">Kind of attribute value.</param>
        /// <param name="typeManager">The TypeManager of the GraphDB</param>
        /// <returns></returns>
        public static bool IsAValidAttributeType(GraphDBType aGraphType, BasicType typeOfAttributeValue, DBContext typeManager, Object myValue)
        {

            #region Data

            bool isValid = false;

            #endregion

            #region process types

            #region non-list type

            if (typeOfAttributeValue == BasicType.Unknown)
            {
                isValid = GraphDBTypeMapper.GetGraphObjectFromTypeName(aGraphType.Name).IsValidValue(myValue);
            }
            else
            {
                isValid = (GraphDBTypeMapper.ConvertGraph2CSharp(aGraphType.Name) == typeOfAttributeValue || aGraphType.IsUserDefined);
            }

            #endregion

            #endregion

            return isValid;

        }

        public static BasicType ConvertCSharp2Graph(Type myType)
        {

            if (myType == typeof(Int16))
                return GraphInteger;

            if (myType == typeof(Int32) || myType == typeof(Int64))
                return GraphInt32;

            if (myType == typeof(UInt16) || myType == typeof(UInt32) || myType == typeof(UInt64))
                return GraphUnsignedInteger;

            if (myType == typeof(Double))
                return GraphDouble;

            if (myType == typeof(String))
                return GraphString;

            if (myType == typeof(Boolean))
                return GraphBoolean;

            if (myType == typeof(DateTime))
                return GraphDateTime;

            return BasicType.Unknown;

        }

        public static BasicType ConvertGraph2CSharp(TypeAttribute attributeDefinition, GraphDBType typeOfAttribute)
        {

            if (typeOfAttribute.IsUserDefined)
            {
                if (attributeDefinition.KindOfType != KindsOfType.SetOfReferences && attributeDefinition.KindOfType != KindsOfType.SingleReference)
                    throw new GraphDBException(new Error_ListAttributeNotAllowed(typeOfAttribute.Name));

                return BasicType.Reference;
            }

            else
            {

                if (attributeDefinition.KindOfType == KindsOfType.ListOfNoneReferences || attributeDefinition.KindOfType == KindsOfType.SetOfNoneReferences)
                    return BasicType.SetOfDBObjects;

                switch (typeOfAttribute.Name)
                {

                    case DBConstants.DBInteger:
                        return GraphInteger;

                    case DBConstants.DBInt32:
                        return GraphInt32;

                    case DBConstants.DBUnsignedInteger:
                        return GraphUnsignedInteger;

                    case DBConstants.DBString:
                        return GraphString;

                    case DBConstants.DBDouble:
                        return GraphDouble;

                    case DBConstants.DBDateTime:
                        return GraphDateTime;

                    case DBConstants.DBBoolean:
                        return GraphBoolean;

                    case "NumberLiteral":
                        return BasicType.Unknown;

                    case "StringLiteral":
                        return BasicType.Unknown;

                    case DBConstants.DBObject:
                        return BasicType.Reference;

                    default:
                        throw new GraphDBException(new Error_TypeDoesNotExist(typeOfAttribute.Name));

                }

            }

        }

        public static BasicType ConvertGraph2CSharp(String typeName)
        {
            if (!IsBasicType(typeName))
            {
                /*
                if (!IsValidType(typeName))
                {
                    throw new GraphDBException(new Error_TypeDoesNotExist(typeName));
                }
                */
                return BasicType.NotABasicType;
            }

            else
            {

                if (typeName.Contains(DBConstants.LIST_PREFIX))
                    return BasicType.SetOfDBObjects;

                switch (typeName)
                {

                    case DBConstants.DBInteger:
                        return GraphInteger;

                    case DBConstants.DBInt32:
                        return GraphInt32;

                    case DBConstants.DBUnsignedInteger:
                        return GraphUnsignedInteger;

                    case DBConstants.DBString:
                        return GraphString;

                    case DBConstants.DBDouble:
                        return GraphDouble;

                    case DBConstants.DBDateTime:
                        return GraphDateTime;

                    case DBConstants.DBBoolean:
                        return GraphBoolean;

                    case "NumberLiteral":
                    case "StringLiteral":
                        return BasicType.Unknown;
                    
                    default:
                        throw new GraphDBException(new Error_TypeDoesNotExist(typeName));

                }

            }

        }

        private static bool IsValidType(string typeName)
        {
            if (!IsBasicType(typeName))
            {
                switch (typeName)
                {

                    case DBConstants.DBObject:
                        return true;

                    default:
                        return false;

                }
            }
            else
            {
                return true;
            }
        }

        public static ADBBaseObject GetGraphObjectFromTypeName(String myTypeName)
        {
            return GetGraphObjectFromTypeName(myTypeName, null);
        }


        public static ADBBaseObject GetEmptyGraphObjectFromType(BasicType myTypeOfValue)
        {
            return GetGraphObjectFromType(myTypeOfValue, null);
        }

        public static ADBBaseObject GetADBBaseObjectFromUUID(TypeUUID myUUID, Object myValue)
        {

                if (myUUID == DBDouble.UUID)
                    return new DBDouble(myValue);

                if (myUUID == DBUInt64.UUID)
                    return new DBUInt64(myValue);

                if (myUUID == DBInt64.UUID)
                    return new DBInt64(myValue);

                if (myUUID == DBInt32.UUID)
                    return new DBInt32(myValue);

                if (myUUID == DBString.UUID)
                    return new DBString(myValue);

                if (myUUID == DBDateTime.UUID)
                    return new DBDateTime(myValue);

                if (myUUID == DBBoolean.UUID)
                    return new DBBoolean(myValue);

                if (myUUID == DBEdge.UUID)
                    return new DBEdge(myValue);

                if (myUUID == DBReference.UUID)   
                    return new DBReference(myValue);

                if (myUUID == DBBaseObject.UUID)
                    return new DBBaseObject(myValue);

                throw new GraphDBException(new Error_TypeDoesNotExist(myUUID.ToString()));

        }

        public static ADBBaseObject GetADBBaseObjectFromUUID(TypeUUID myUUID)
        {

            if (myUUID == DBDouble.UUID)
                return new DBDouble();

            if (myUUID == DBUInt64.UUID)
                return new DBUInt64();

            if (myUUID == DBInt64.UUID)
                return new DBInt64();

            if (myUUID == DBInt32.UUID)
                return new DBInt32();

            if (myUUID == DBString.UUID)
                return new DBString();

            if (myUUID == DBDateTime.UUID)
                return new DBDateTime();

            if (myUUID == DBBoolean.UUID)
                return new DBBoolean();

            if (myUUID == DBEdge.UUID)
                return new DBEdge();

            if (myUUID == DBReference.UUID)
                return new DBReference();

            if (myUUID == DBBaseObject.UUID)
                return new DBBaseObject();

            throw new GraphDBException(new Error_TypeDoesNotExist(myUUID.ToString()));

        }
        
        public static ADBBaseObject GetBaseObjectFromCSharpType(Object myValue)
        {
            return GetGraphObjectFromType(ConvertCSharp2Graph(myValue.GetType()), myValue);
        }

        public static ADBBaseObject GetGraphObjectFromType(BasicType myTypeOfValue, Object myValue)
        {

            switch (myTypeOfValue)
            {

                case BasicType.Double:
                    if (myValue == null)
                        return new DBDouble();
                    else
                        return new DBDouble(myValue);

                case BasicType.UInt64:
                    if (myValue == null)
                        return new DBUInt64();
                    else
                        return new DBUInt64(myValue);

                case BasicType.Int64:
                    if (myValue == null)
                        return new DBInt64();
                    else
                        return new DBInt64(myValue);

                case BasicType.Int32:
                    if (myValue == null)
                        return new DBInt32();
                    else
                        return new DBInt32(myValue);

                case BasicType.String:
                    if (myValue == null)
                        return new DBString();
                    else
                        return new DBString(myValue);

                case BasicType.DateTime:
                    if (myValue == null)
                        return new DBDateTime();
                    else
                        return new DBDateTime(myValue);

                case BasicType.Boolean:
                    if (myValue == null)
                        return new DBBoolean();
                    else
                        return new DBBoolean(myValue);

                case BasicType.SetOfDBObjects:
                    if (myValue != null)
                        return new DBEdge(myValue);
                    return new DBEdge();

                case BasicType.Unknown:
                    if (myValue is String)
                        return new DBString(myValue);
                    return new DBNumber(myValue);

                case BasicType.NotABasicType:
                case BasicType.Reference:

                    if (myValue != null)
                    {

                        var _ADBBaseObject = myValue as ADBBaseObject;

                        if (_ADBBaseObject != null)
                            return _ADBBaseObject;
                        
                        return new DBReference(myValue);

                    }

                    else
                    {

                        if (myTypeOfValue == BasicType.Reference)
                            return new DBReference();

                        return null;

                    }

                default:
                    throw new GraphDBException(new Error_TypeDoesNotExist(myTypeOfValue.ToString()));

            }
        }

        public static ADBBaseObject GetGraphObjectFromTypeName(String myTypeName, Object myValue)
        {
            return GetGraphObjectFromType(ConvertGraph2CSharp(myTypeName), myValue);
        }

        public static Exceptional<Boolean> ConvertToBestMatchingType(ref ADBBaseObject myDBBaseObjectA, ref ADBBaseObject myDBBaseObjectB)
        {

            #region Both are not unknown

            if ((myDBBaseObjectA.Type != BasicType.Unknown) && (myDBBaseObjectB.Type != BasicType.Unknown))
            {

                #region Types matching, we can leave

                if (myDBBaseObjectA.Type == myDBBaseObjectB.Type)
                {
                    return new Exceptional<Boolean>(true);
                }

                #endregion

                #region DBList and DBReference are not matchable with other types

                else if (myDBBaseObjectA is DBEdge || myDBBaseObjectB is DBEdge)
                {
                    return new Exceptional<Boolean>(false);
                }

                #endregion

                #region The types does not match - so try to convert the right-handed to the left handed type

                else if (myDBBaseObjectA is DBUInt64)
                {
                    //
                    if (DBUInt64.IsValid(myDBBaseObjectB.Value))
                    {
                        myDBBaseObjectB = myDBBaseObjectA.Clone(myDBBaseObjectB.Value);
                        return new Exceptional<Boolean>(true);
                    }
                    return new Exceptional<Boolean>(ConvertToBestMatchingTypeReverse(ref myDBBaseObjectA, ref myDBBaseObjectB));
                }

                else if (myDBBaseObjectA is DBInt64)
                {
                    //
                    if (DBInt64.IsValid(myDBBaseObjectB.Value))
                    {
                        myDBBaseObjectB = myDBBaseObjectA.Clone(myDBBaseObjectB.Value);
                        return new Exceptional<Boolean>(true);
                    }
                    return new Exceptional<Boolean>(ConvertToBestMatchingTypeReverse(ref myDBBaseObjectA, ref myDBBaseObjectB));
                }

                else if (myDBBaseObjectA is DBInt32)
                {
                    //
                    if (DBInt32.IsValid(myDBBaseObjectB.Value))
                    {
                        myDBBaseObjectB = myDBBaseObjectA.Clone(myDBBaseObjectB.Value);
                        return new Exceptional<Boolean>(true);
                    }
                    return new Exceptional<Boolean>(ConvertToBestMatchingTypeReverse(ref myDBBaseObjectA, ref myDBBaseObjectB));
                }

                else if (myDBBaseObjectA is DBDouble)
                {
                    //
                    if (DBDouble.IsValid(myDBBaseObjectB.Value))
                    {
                        myDBBaseObjectB = myDBBaseObjectA.Clone(myDBBaseObjectB.Value);
                        return new Exceptional<Boolean>(true);
                    }
                    return new Exceptional<Boolean>(ConvertToBestMatchingTypeReverse(ref myDBBaseObjectA, ref myDBBaseObjectB));
                }

                else if (myDBBaseObjectA is DBDateTime)
                {
                    // 
                    if (DBDateTime.IsValid(myDBBaseObjectB.Value))
                    {
                        myDBBaseObjectB = myDBBaseObjectA.Clone(myDBBaseObjectB.Value);
                        return new Exceptional<Boolean>(true);
                    }
                    return new Exceptional<Boolean>(ConvertToBestMatchingTypeReverse(ref myDBBaseObjectA, ref myDBBaseObjectB));
                }

                else if (myDBBaseObjectA is DBBoolean)
                {
                    //
                    if (DBBoolean.IsValid(myDBBaseObjectB.Value))
                    {
                        myDBBaseObjectB = myDBBaseObjectA.Clone(myDBBaseObjectB.Value);
                        return new Exceptional<Boolean>(true);
                    }
                    return new Exceptional<Boolean>(ConvertToBestMatchingTypeReverse(ref myDBBaseObjectA, ref myDBBaseObjectB));
                }
                
                #endregion

                // we couldnt find a matching type for both are not unknown
                return new Exceptional<bool>(new Error_NotImplemented(new StackTrace(true), String.Format("no type conversion implemented for {0} and {1}", myDBBaseObjectA.GetType(), myDBBaseObjectB.GetType())));

            }

            #endregion

            #region Both are strings - thats fine

            else if (myDBBaseObjectA is DBString && myDBBaseObjectB is DBString)
            {
                return new Exceptional<Boolean>(true);
            }

            #endregion

            #region only one is unknown 

            else if (!(myDBBaseObjectA.Type == BasicType.Unknown && myDBBaseObjectB.Type == BasicType.Unknown))
            {

                #region myDBBaseObjectA is unknown - try to use the type of myDBBaseObjectB

                if (myDBBaseObjectA.Type == BasicType.Unknown)
                {

                    // avoid changing a Double to Int64
                    if (myDBBaseObjectA is DBNumber
                        && (myDBBaseObjectB.Type == BasicType.Int64 || myDBBaseObjectB.Type == BasicType.UInt64))
                    {
                        if (myDBBaseObjectA.Value is Double && DBDouble.IsValid(myDBBaseObjectB.Value))
                        {
                            myDBBaseObjectA = new DBDouble(myDBBaseObjectA.Value);
                            myDBBaseObjectB = new DBDouble(myDBBaseObjectB.Value);
                            return new Exceptional<bool>(true);
                        }
                    }

                    try
                    {
                        myDBBaseObjectA = myDBBaseObjectB.Clone(myDBBaseObjectA.Value);
                    }
                    catch
                    {
                        return new Exceptional<bool>(new Error_DataTypeDoesNotMatch(myDBBaseObjectB.GetType().Name, myDBBaseObjectA.Value.GetType().Name));
                    }

                }

                #endregion

                #region myDBBaseObjectB is unknown - try to use the type of myDBBaseObjectA

                else if (myDBBaseObjectB.Type == BasicType.Unknown)
                {

                    // avoid changing a Double to Int64
                    if (myDBBaseObjectB is DBNumber
                        && (myDBBaseObjectA.Type == BasicType.Int64 || myDBBaseObjectA.Type == BasicType.UInt64))
                    {
                        if (myDBBaseObjectB.Value is Double && DBDouble.IsValid(myDBBaseObjectA.Value))
                        {
                            myDBBaseObjectA = new DBDouble(myDBBaseObjectA.Value);
                            myDBBaseObjectB = new DBDouble(myDBBaseObjectB.Value);
                            return new Exceptional<bool>(true);
                        }
                    }
                    
                    try
                    {
                        myDBBaseObjectB = myDBBaseObjectA.Clone(myDBBaseObjectB.Value);
                    }
                    catch
                    {
                        return new Exceptional<bool>(new Error_DataTypeDoesNotMatch(myDBBaseObjectA.Value.GetType().ToString(), myDBBaseObjectB.GetType().ToString()));
                    }

                }

                else
                {
                    return new Exceptional<bool>(false);
                }

                #endregion
            
            }

            #endregion

            #region both are unknown

            #region One of them is expected as a Number - so try to parse both as a number

            else if (myDBBaseObjectA is DBNumber || myDBBaseObjectB is DBNumber)
            {

                if (DBInt64.IsValid(myDBBaseObjectA.Value) && DBInt64.IsValid(myDBBaseObjectB.Value))
                {
                    myDBBaseObjectA = new DBInt64(myDBBaseObjectA.Value);
                    myDBBaseObjectB = new DBInt64(myDBBaseObjectB.Value);
                }

                else if (DBUInt64.IsValid(myDBBaseObjectA.Value) && DBUInt64.IsValid(myDBBaseObjectB.Value))
                {
                    myDBBaseObjectA = new DBUInt64(myDBBaseObjectA.Value);
                    myDBBaseObjectB = new DBUInt64(myDBBaseObjectB.Value);
                }

                else if (DBDouble.IsValid(myDBBaseObjectA.Value) && DBDouble.IsValid(myDBBaseObjectB.Value))
                {
                    myDBBaseObjectA = new DBDouble(myDBBaseObjectA.Value);
                    myDBBaseObjectB = new DBDouble(myDBBaseObjectB.Value);
                }

                else
                {
                    return new Exceptional<Boolean>(false);
                }

            }

            #endregion

            #region Try all other dataTypes

            else
            {

                // check all types beginning with the hardest and ending with string (matches all)
                if (DBDateTime.IsValid(myDBBaseObjectA.Value) && DBDateTime.IsValid(myDBBaseObjectB.Value))
                {
                    myDBBaseObjectA = new DBDateTime(myDBBaseObjectA.Value);
                    myDBBaseObjectB = new DBDateTime(myDBBaseObjectB.Value);
                }

                else if (DBBoolean.IsValid(myDBBaseObjectA.Value) && DBBoolean.IsValid(myDBBaseObjectB.Value))
                {
                    myDBBaseObjectA = new DBBoolean(myDBBaseObjectA.Value);
                    myDBBaseObjectB = new DBBoolean(myDBBaseObjectB.Value);
                }

                else if (DBEdge.IsValid(myDBBaseObjectA.Value) && DBEdge.IsValid(myDBBaseObjectB.Value))
                {
                    myDBBaseObjectA = new DBEdge(myDBBaseObjectA.Value);
                    myDBBaseObjectB = new DBEdge(myDBBaseObjectB.Value);
                }

                else if (DBReference.IsValid(myDBBaseObjectA.Value) && DBReference.IsValid(myDBBaseObjectB.Value))
                {
                    myDBBaseObjectA = new DBReference(myDBBaseObjectA.Value);
                    myDBBaseObjectB = new DBReference(myDBBaseObjectB.Value);
                }

                else if (DBString.IsValid(myDBBaseObjectA.Value) && DBString.IsValid(myDBBaseObjectB.Value))
                {
                    myDBBaseObjectA = new DBString(myDBBaseObjectA.Value);
                    myDBBaseObjectB = new DBString(myDBBaseObjectB.Value);
                }

                else
                {
                    return new Exceptional<Boolean>(false);
                }

            }

            #endregion

            #endregion

            return new Exceptional<bool>(true);

        }

        private static Boolean ConvertToBestMatchingTypeReverse(ref ADBBaseObject myDBBaseObjectB, ref ADBBaseObject myDBBaseObjectA)
        {

            #region Both are not unknown

            if ((myDBBaseObjectA.Type != BasicType.Unknown) && (myDBBaseObjectB.Type != BasicType.Unknown))
            {

                #region Types matching, we can leave

                if (myDBBaseObjectA.Type == myDBBaseObjectB.Type)
                {
                    return true;
                }

                #endregion

                #region DBList and DBReference are not matchable with other types

                else if (myDBBaseObjectA is DBEdge || myDBBaseObjectA is DBReference || myDBBaseObjectB is DBEdge || myDBBaseObjectB is DBReference)
                {
                    return false;
                }

                #endregion

                #region The types does not match - so try to convert the right-handed to the left handed type

                else if (myDBBaseObjectA is DBUInt64)
                {
                    //
                    if (DBUInt64.IsValid(myDBBaseObjectB.Value))
                    {
                        myDBBaseObjectB = myDBBaseObjectA.Clone(myDBBaseObjectB.Value);
                        return true;
                    }
                    return false;
                }

                else if (myDBBaseObjectA is DBInt64)
                {
                    //
                    if (DBInt64.IsValid(myDBBaseObjectB.Value))
                    {
                        myDBBaseObjectB = myDBBaseObjectA.Clone(myDBBaseObjectB.Value);
                        return true;
                    }
                    // do not change to ConvertToBestMatchingType(ref myDBBaseObjectB, ref myDBBaseObjectA)
                    return false;
                }

                else if (myDBBaseObjectA is DBDouble)
                {
                    //
                    if (DBDouble.IsValid(myDBBaseObjectB.Value))
                    {
                        myDBBaseObjectB = myDBBaseObjectA.Clone(myDBBaseObjectB.Value);
                        return true;
                    }
                    return false;
                }

                else if (myDBBaseObjectA is DBDateTime)
                {
                    //
                    if (DBDateTime.IsValid(myDBBaseObjectB.Value))
                    {
                        myDBBaseObjectB = myDBBaseObjectA.Clone(myDBBaseObjectB.Value);
                        return true;
                    }
                    return false;
                }

                else if (myDBBaseObjectA is DBBoolean)
                {
                    //
                    if (DBDouble.IsValid(myDBBaseObjectB.Value))
                    {
                        myDBBaseObjectB = myDBBaseObjectA.Clone(myDBBaseObjectB.Value);
                        return true;
                    }
                    return false;
                }

                else if (myDBBaseObjectA is DBString)
                {
                    //
                    if (DBString.IsValid(myDBBaseObjectB.Value))
                    {
                        myDBBaseObjectB = myDBBaseObjectA.Clone(myDBBaseObjectB.Value);
                        return true;
                    }
                    return false;
                }

                #endregion

                // we couldnt find a matching type for both are not unknown
                return false;

            }

            #endregion

            #region Both are strings - thats fine

            else if (myDBBaseObjectA is DBString && myDBBaseObjectB is DBString)
            {
                return true;
            }

            #endregion

            #region only one is unknown

            else if (!(myDBBaseObjectA.Type == BasicType.Unknown && myDBBaseObjectB.Type == BasicType.Unknown))
            {

                #region myDBBaseObjectA is unknown - try to use the type of myDBBaseObjectB

                if (myDBBaseObjectA.Type == BasicType.Unknown)
                {

                    try
                    {
                        // avaoid changing a Double to Int64
                        if (myDBBaseObjectA is DBNumber
                            && (myDBBaseObjectB.Type == BasicType.Int64 || myDBBaseObjectB.Type == BasicType.UInt64))
                        {
                            if (myDBBaseObjectA.Value is Double && DBDouble.IsValid(myDBBaseObjectB.Value))
                            {
                                myDBBaseObjectA = new DBDouble(myDBBaseObjectA.Value);
                                myDBBaseObjectB = new DBDouble(myDBBaseObjectB.Value);
                                return true;
                            }
                        }

                        myDBBaseObjectA = myDBBaseObjectB.Clone(myDBBaseObjectA.Value);

                    }

                    catch
                    {
                        return false;
                    }

                }

                #endregion

                #region myDBBaseObjectB is unknown - try to use the type of myDBBaseObjectA

                else if (myDBBaseObjectB.Type == BasicType.Unknown)
                {
                    try
                    {
                        myDBBaseObjectB = myDBBaseObjectA.Clone(myDBBaseObjectB.Value);
                    }
                    catch
                    {
                        return false;
                    }
                }

                else
                {
                    return false;
                }

                #endregion
            
            }

            #endregion

            #region both are unknown

            #region One of them is expected as a Number - so try to parse both as a number

            else if (myDBBaseObjectA is DBNumber || myDBBaseObjectB is DBNumber)
            {

                if (DBInt64.IsValid(myDBBaseObjectA.Value) && DBInt64.IsValid(myDBBaseObjectB.Value))
                {
                    myDBBaseObjectA = new DBInt64(myDBBaseObjectA.Value);
                    myDBBaseObjectB = new DBInt64(myDBBaseObjectB.Value);
                }

                else if (DBUInt64.IsValid(myDBBaseObjectA.Value) && DBUInt64.IsValid(myDBBaseObjectB.Value))
                {
                    myDBBaseObjectA = new DBUInt64(myDBBaseObjectA.Value);
                    myDBBaseObjectB = new DBUInt64(myDBBaseObjectB.Value);
                }

                else if (DBDouble.IsValid(myDBBaseObjectA.Value) && DBDouble.IsValid(myDBBaseObjectB.Value))
                {
                    myDBBaseObjectA = new DBDouble(myDBBaseObjectA.Value);
                    myDBBaseObjectB = new DBDouble(myDBBaseObjectB.Value);
                }

                else
                {
                    return false;
                }

            }

            #endregion

            #region Try all other dataTypes

            else
            {

                // check all types beginning with the hardest and ending with string (matches all)
                if (DBDateTime.IsValid(myDBBaseObjectA.Value) && DBDateTime.IsValid(myDBBaseObjectB.Value))
                {
                    myDBBaseObjectA = new DBDateTime(myDBBaseObjectA.Value);
                    myDBBaseObjectB = new DBDateTime(myDBBaseObjectB.Value);
                }

                else if (DBBoolean.IsValid(myDBBaseObjectA.Value) && DBBoolean.IsValid(myDBBaseObjectB.Value))
                {
                    myDBBaseObjectA = new DBBoolean(myDBBaseObjectA.Value);
                    myDBBaseObjectB = new DBBoolean(myDBBaseObjectB.Value);
                }

                else if (DBEdge.IsValid(myDBBaseObjectA.Value) && DBEdge.IsValid(myDBBaseObjectB.Value))
                {
                    myDBBaseObjectA = new DBEdge(myDBBaseObjectA.Value);
                    myDBBaseObjectB = new DBEdge(myDBBaseObjectB.Value);
                }

                else if (DBReference.IsValid(myDBBaseObjectA.Value) && DBReference.IsValid(myDBBaseObjectB.Value))
                {
                    myDBBaseObjectA = new DBReference(myDBBaseObjectA.Value);
                    myDBBaseObjectB = new DBReference(myDBBaseObjectB.Value);
                }

                else if (DBString.IsValid(myDBBaseObjectA.Value) && DBString.IsValid(myDBBaseObjectB.Value))
                {
                    myDBBaseObjectA = new DBString(myDBBaseObjectA.Value);
                    myDBBaseObjectB = new DBString(myDBBaseObjectB.Value);
                }

                else
                {
                    return false;
                }

            }

            #endregion

            #endregion

            return true;

        }

    }
}
