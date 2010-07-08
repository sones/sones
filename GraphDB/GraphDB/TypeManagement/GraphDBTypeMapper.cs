/* 
 * Henning Rauch, Stefan Licht, 2009 - 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.QueryLanguage.Enums;
using sones.GraphDB.Exceptions;
using sones.Lib;
using sones.Lib.Frameworks.CLIrony.Compiler;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.PandoraTypes;
using sones.GraphDB.QueryLanguage;
using sones.Lib.ErrorHandling;
using sones.GraphDB.Errors;
using System.Diagnostics;

#endregion

namespace sones.GraphDB.TypeManagement
{

    /// <summary>
    /// Maps GraphDBTypes to C# types.
    /// </summary>

    public class GraphDBTypeMapper
    {

        public static TypesOfOperatorResult PandoraInteger          = TypesOfOperatorResult.Int64;
        public static TypesOfOperatorResult PandoraInt32            = TypesOfOperatorResult.Int32;
        public static TypesOfOperatorResult PandoraUnsignedInteger  = TypesOfOperatorResult.UInt64;
        public static TypesOfOperatorResult PandoraString           = TypesOfOperatorResult.String;
        public static TypesOfOperatorResult PandoraDouble           = TypesOfOperatorResult.Double;
        public static TypesOfOperatorResult PandoraDateTime         = TypesOfOperatorResult.DateTime;
        public static TypesOfOperatorResult PandoraBoolean          = TypesOfOperatorResult.Boolean;

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
        /// Checks if the name of the attribute is valid for a certain PandoraType.
        /// </summary>
        /// <param name="PandoraTypeName">The PandoraType.</param>
        /// <param name="myAttributeName">Name of the attribute.</param>
        /// <param name="typeOfAttributeValue">Kind of attribute value.</param>
        /// <param name="typeManager">The TypeManager of the PandoraDB</param>
        /// <returns></returns>
        public static bool IsAValidAttributeType(GraphDBType aPandoraType, TypesOfOperatorResult typeOfAttributeValue, DBContext typeManager, Object myValue)
        {

            #region Data

            bool isValid = false;

            #endregion

            #region process types

            #region non-list type

            if (typeOfAttributeValue == TypesOfOperatorResult.Unknown)
            {
                isValid = GraphDBTypeMapper.GetPandoraObjectFromTypeName(aPandoraType.Name).IsValidValue(myValue);
            }
            else
            {
                isValid = (GraphDBTypeMapper.ConvertPandora2CSharp(aPandoraType.Name) == typeOfAttributeValue || aPandoraType.IsUserDefined);
            }

            #endregion

            #endregion

            return isValid;

        }

        public static TypesOfOperatorResult ConvertCSharp2Pandora(Type myType)
        {

            if (myType == typeof(Int16))
                return PandoraInteger;

            if (myType == typeof(Int32) || myType == typeof(Int64))
                return PandoraInt32;

            if (myType == typeof(UInt16) || myType == typeof(UInt32) || myType == typeof(UInt64))
                return PandoraUnsignedInteger;

            if (myType == typeof(Double))
                return PandoraDouble;

            if (myType == typeof(String))
                return PandoraString;

            if (myType == typeof(Boolean))
                return PandoraBoolean;

            if (myType == typeof(DateTime))
                return PandoraDateTime;

            return TypesOfOperatorResult.Unknown;

        }
        
        public static TypesOfOperatorResult ConvertPandora2CSharp(TypeAttribute attributeDefinition, GraphDBType typeOfAttribute)
        {

            if (typeOfAttribute.IsUserDefined)
            {
                if (attributeDefinition.KindOfType != KindsOfType.SetOfReferences && attributeDefinition.KindOfType != KindsOfType.SingleReference)
                    throw new GraphDBException(new Error_ListAttributeNotAllowed(typeOfAttribute.Name));
                
                return TypesOfOperatorResult.Reference;
            }

            else
            {

                if (attributeDefinition.KindOfType == KindsOfType.ListOfNoneReferences || attributeDefinition.KindOfType == KindsOfType.SetOfNoneReferences)
                    return TypesOfOperatorResult.SetOfDBObjects;

                switch (typeOfAttribute.Name)
                {

                    case DBConstants.DBInteger:
                        return PandoraInteger;

                    case DBConstants.DBInt32:
                        return PandoraInt32;

                    case DBConstants.DBUnsignedInteger:
                        return PandoraUnsignedInteger;

                    case DBConstants.DBString:
                        return PandoraString;

                    case DBConstants.DBDouble:
                        return PandoraDouble;

                    case DBConstants.DBDateTime:
                        return PandoraDateTime;

                    case DBConstants.DBBoolean:
                        return PandoraBoolean;

                    case "NumberLiteral":
                        return TypesOfOperatorResult.Unknown;

                    case "StringLiteral":
                        return TypesOfOperatorResult.Unknown;

                    case DBConstants.DBObject:
                        return TypesOfOperatorResult.Reference;

                    default:
                        throw new GraphDBException(new Error_TypeDoesNotExist(typeOfAttribute.Name));

                }

            }

        }

        public static TypesOfOperatorResult ConvertPandora2CSharp(String typeName)
        {
            if (!IsBasicType(typeName))
            {
                /*
                if (!IsValidType(typeName))
                {
                    throw new GraphDBException(new Error_TypeDoesNotExist(typeName));
                }
                */
                return TypesOfOperatorResult.NotABasicType;
            }

            else
            {

                if (typeName.Contains(DBConstants.LIST_PREFIX))
                    return TypesOfOperatorResult.SetOfDBObjects;

                switch (typeName)
                {

                    case DBConstants.DBInteger:
                        return PandoraInteger;

                    case DBConstants.DBInt32:
                        return PandoraInt32;

                    case DBConstants.DBUnsignedInteger:
                        return PandoraUnsignedInteger;

                    case DBConstants.DBString:
                        return PandoraString;

                    case DBConstants.DBDouble:
                        return PandoraDouble;

                    case DBConstants.DBDateTime:
                        return PandoraDateTime;

                    case DBConstants.DBBoolean:
                        return PandoraBoolean;

                    case "NumberLiteral":
                    case "StringLiteral":
                        return TypesOfOperatorResult.Unknown;
                    
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

        public static ADBBaseObject GetPandoraObjectFromTypeName(String myTypeName)
        {
            return GetPandoraObjectFromTypeName(myTypeName, null);
        }


        public static ADBBaseObject GetEmptyPandoraObjectFromType(TypesOfOperatorResult myTypeOfValue)
        {
            return GetPandoraObjectFromType(myTypeOfValue, null);
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

                throw new GraphDBException(new Error_TypeDoesNotExist(myUUID.ToHexString()));

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
            return GetPandoraObjectFromType(ConvertCSharp2Pandora(myValue.GetType()), myValue);
        }

        public static ADBBaseObject GetPandoraObjectFromType(TypesOfOperatorResult myTypeOfValue, Object myValue)
        {

            switch (myTypeOfValue)
            {

                case TypesOfOperatorResult.Double:
                    return new DBDouble(myValue);

                case TypesOfOperatorResult.UInt64:
                    return new DBUInt64(myValue);

                case TypesOfOperatorResult.Int64:
                    return new DBInt64(myValue);

                case TypesOfOperatorResult.Int32:
                    return new DBInt32(myValue);

                case TypesOfOperatorResult.String:
                    return new DBString(myValue);

                case TypesOfOperatorResult.DateTime:
                    return new DBDateTime(myValue);

                case TypesOfOperatorResult.Boolean:
                    return new DBBoolean(myValue);

                case TypesOfOperatorResult.SetOfDBObjects:
                    if (myValue != null)
                        return new DBEdge(myValue);
                    return new DBEdge();

                case TypesOfOperatorResult.Unknown:
                    if (myValue is String)
                        return new DBString(myValue);
                    return new DBNumber(myValue);

                case TypesOfOperatorResult.NotABasicType:
                case TypesOfOperatorResult.Reference:

                    if (myValue != null)
                    {

                        var _ADBBaseObject = myValue as ADBBaseObject;

                        if (_ADBBaseObject != null)
                            return _ADBBaseObject;
                        
                        return new DBReference(myValue);

                    }

                    else
                    {

                        if (myTypeOfValue == TypesOfOperatorResult.Reference)
                            return new DBReference();

                        return null;

                    }

                default:
                    throw new GraphDBException(new Error_TypeDoesNotExist(myTypeOfValue.ToString()));

            }
        }

        public static ADBBaseObject GetPandoraObjectFromTypeName(String myTypeName, Object myValue)
        {
            return GetPandoraObjectFromType(ConvertPandora2CSharp(myTypeName), myValue);
        }

        public static Exceptional<Boolean> ConvertToBestMatchingType(ref ADBBaseObject myDBBaseObjectA, ref ADBBaseObject myDBBaseObjectB)
        {

            #region Both are not unknown

            if ((myDBBaseObjectA.Type != TypesOfOperatorResult.Unknown) && (myDBBaseObjectB.Type != TypesOfOperatorResult.Unknown))
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

            else if (!(myDBBaseObjectA.Type == TypesOfOperatorResult.Unknown && myDBBaseObjectB.Type == TypesOfOperatorResult.Unknown))
            {

                #region myDBBaseObjectA is unknown - try to use the type of myDBBaseObjectB

                if (myDBBaseObjectA.Type == TypesOfOperatorResult.Unknown)
                {

                    // avoid changing a Double to Int64
                    if (myDBBaseObjectA is DBNumber
                        && (myDBBaseObjectB.Type == TypesOfOperatorResult.Int64 || myDBBaseObjectB.Type == TypesOfOperatorResult.UInt64))
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

                else if (myDBBaseObjectB.Type == TypesOfOperatorResult.Unknown)
                {

                    // avoid changing a Double to Int64
                    if (myDBBaseObjectB is DBNumber
                        && (myDBBaseObjectA.Type == TypesOfOperatorResult.Int64 || myDBBaseObjectA.Type == TypesOfOperatorResult.UInt64))
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

            if ((myDBBaseObjectA.Type != TypesOfOperatorResult.Unknown) && (myDBBaseObjectB.Type != TypesOfOperatorResult.Unknown))
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

            else if (!(myDBBaseObjectA.Type == TypesOfOperatorResult.Unknown && myDBBaseObjectB.Type == TypesOfOperatorResult.Unknown))
            {

                #region myDBBaseObjectA is unknown - try to use the type of myDBBaseObjectB

                if (myDBBaseObjectA.Type == TypesOfOperatorResult.Unknown)
                {

                    try
                    {
                        // avaoid changing a Double to Int64
                        if (myDBBaseObjectA is DBNumber
                            && (myDBBaseObjectB.Type == TypesOfOperatorResult.Int64 || myDBBaseObjectB.Type == TypesOfOperatorResult.UInt64))
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

                else if (myDBBaseObjectB.Type == TypesOfOperatorResult.Unknown)
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
