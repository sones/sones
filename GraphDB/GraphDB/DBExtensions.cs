using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.GraphDB.Warnings;
using sones.GraphDB.Result;
using sones.GraphDB.TypeManagement;

using sones.GraphFS.DataStructures;
using sones.Lib.ErrorHandling;

namespace sones.GraphDB
{
    public static class DBExtensions
    {

        #region List<GraphDBError>

        public static String ToErrorString(this List<GraphDBError> myGraphErrors)
        {
            return myGraphErrors.Aggregate<GraphDBError, StringBuilder>(new StringBuilder(), (result, elem) => { result.AppendLine(elem.ToString()); return result; }).ToString();
        }

        #endregion

        #region ADBBaseObject GetValue

        public static Int32 GetValue(this DBInt32 value)
        {
            return (Int32)value.Value;
        }

        public static Int64 GetValue(this DBInt64 value)
        {
            return (Int64)value.Value;
        }

        public static UInt64 GetValue(this DBUInt64 value)
        {
            return (UInt64)value.Value;
        }

        public static Double GetValue(this DBDouble value)
        {
            return (Double)value.Value;
        }

        public static Boolean GetValue(this DBBoolean value)
        {
            return (Boolean)value.Value;
        }

        public static String GetValue(this DBString value)
        {
            return (String)value.Value;
        }

        public static DateTime GetValue(this DBDateTime value)
        {
            return (DateTime)value.Value;
        }

        public static TypeAttribute GetValue(this DBTypeAttribute value)
        {
            return value.Value as TypeAttribute;
        }

        public static GraphDBType GetValue(this DBType value)
        {
            return value.Value as GraphDBType;
        }

        #endregion

        #region DBTypeManager - some methods which should not be inside TM but needed from some tests or CLI_OM

        #region AddAttributeToType(targetClass, attributeName, attributeType)

        /// <summary>
        /// Adds an attribute with given name and type to the class with the given name
        /// </summary>
        /// <param name="targetClass">The class, we want to add the new attribute to.</param>
        /// <param name="myAttributeName">The name of the attribute.</param>
        /// <param name="attributeType">The type of the attribute.</param>
        /// <returns>Ture, if the attribute could be added to the target class. Else, false. (attribute contained in superclass)</returns>
        public static Exceptional<ResultType> AddAttributeToType(this DBTypeManager typeManager, String targetClass, String attributeName, String attributeType)
        {
            if (String.IsNullOrEmpty(attributeType))
            {
                return new Exceptional<ResultType>(new Error_ArgumentNullOrEmpty("attributeType"));
            }

            KindsOfType kindOfType = KindsOfType.SingleReference;

            if (attributeType.StartsWith(DBConstants.LIST_PREFIX) && attributeType.EndsWith(DBConstants.LIST_POSTFIX))
            {
                attributeType = attributeType.Substring(DBConstants.LIST_PREFIX.Length, attributeType.Length - DBConstants.LIST_PREFIX.Length - DBConstants.LIST_POSTFIX.Length);
                kindOfType = KindsOfType.SetOfReferences;
            }

            var attrType = typeManager.GetTypeByName(attributeType);

            if (attrType == null)
            {
                return new Exceptional<ResultType>(new Error_TypeDoesNotExist(attributeType));
            }

            //if we reach this code, no other superclass contains an attribute with this name, so add it!
            TypeAttribute ta = new TypeAttribute() { DBTypeUUID = attrType.UUID, Name = attributeName, KindOfType = kindOfType };

            var dbType = typeManager.GetTypeByName(targetClass);

            if(dbType == null)
            {
                return new Exceptional<ResultType>(new Error_TypeDoesNotExist(targetClass));
            }
            
            var addResult = typeManager.AddAttributeToType(dbType, ta);

            if(addResult.Failed())
            {
                return new Exceptional<ResultType>(addResult);
            }

            var flushResult = typeManager.FlushType(dbType);

            if (flushResult.Failed())
            {
                return new Exceptional<ResultType>(flushResult);
            }

            return new Exceptional<ResultType>(ResultType.Successful);
        }

        #endregion

        #endregion

        #region AObject

        public static Object GetReadoutValue(this IObject theObject)
        {

            if (theObject == null)
                return null;

            else if (theObject is ADBBaseObject)
            {

                return ((ADBBaseObject)theObject).Value;
                /*if (theObject is DBReference)
                {
                    return (theObject as DBReference).Value.ToString();
                }
                else
                {
                    return ((ADBBaseObject)theObject).Value;
                }*/
            }

            else if (theObject is ASetOfReferencesEdgeType)
                return ((ASetOfReferencesEdgeType)theObject).GetAllReferenceIDs();

            else if (theObject is ASingleReferenceEdgeType)
                return ((ASingleReferenceEdgeType)theObject).GetUUID();

            else if (theObject is IBaseEdge)
                return (theObject as IBaseEdge).GetReadoutValues();

            else
                throw new GraphDBException(new Errors.Error_NotImplemented(new System.Diagnostics.StackTrace(true), theObject.GetType().ToString()));

        }

        #endregion

        #region QueryResult

        #region GetFirstError<T>

        /// <summary>
        /// Returns the first error of the given type T
        /// </summary>
        /// <typeparam name="T">The type of the GraphDBError</typeparam>
        /// <returns>Any instance of error</returns>
        public static T GetFirstError<T>(this QueryResult myQueryResult)
            where T : IError
        {
            if (myQueryResult.Errors != null && myQueryResult.Errors.Count() > 0)
            {
                //return Errors.GetFirstError<T>();
                return (T)((from err in myQueryResult.Errors where err.GetType() == typeof(T) select err).FirstOrDefault());
            }
            return default(T);
        }

        #endregion

        #region GetFirstWarning<T>

        /// <summary>
        /// Returns the first error of the given type T
        /// </summary>
        /// <typeparam name="T">The type of the GraphDBError</typeparam>
        /// <returns>Any instance of error</returns>
        public static T GetFirstWarning<T>(this QueryResult myQueryResult)
            where T : IWarning
        {
            if (myQueryResult.Warnings != null && myQueryResult.Warnings.Count() > 0)
            {
                return (T)((from warning in myQueryResult.Warnings where warning.GetType() == typeof(T) select warning).FirstOrDefault());
            }
            return default(T);
        }

        #endregion


        #endregion

    }
}
