/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
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
*/


/*
 * KeyValueSerializer
 * (c) Daniel Kirstenpfad 2009
 * 
 * Lead programmer:
 *      Daniel Kirstenpfad
 * 
 * */

using System;
using System.Reflection;
using sones.Lib.Cryptography.IntegrityCheck;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;

namespace sones.Lib.Serializer
{
    /// <summary>
    /// This class will serialize/deserialize any public field/member variable of an object that does not have the NonSerialized Attribute
    /// 
    /// It's version safe which means: You can DeSerialize an older version of the object into a newer version of the object.
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class KeyValuePairSerializer<T> where T : new()
    {
        #region Constructor
        public KeyValuePairSerializer()
        {
        }
        #endregion

        #region Serialize
        /// <summary>
        /// this method serializes the Input object into a KeyValuePair List e.g. Dictionary<String,Object>. Every public member variable that
        /// does not have the NonSerialized attribut will be serialized.
        /// </summary>
        /// <param name="Input">the Input object</param>
        /// <returns>a byte array containing the data stream</returns>
        public Dictionary<string,object> Serialize(T Input)
        {
            Dictionary<string, object> Result = new Dictionary<string, object>();
            Type reflectiontype = typeof(T);
            MD5 MD5CheckSum = new MD5();

            bool serialize = true;
            // reflect the given object...
            foreach (FieldInfo field in reflectiontype.GetFields())
            {
                if (field.IsPublic)
                {
                    serialize = true;
                    #region Check for NonSerialized Attribute
                    foreach (Attribute attr in field.GetCustomAttributes(true))
                    {
                        NonSerializedAttribute tattr = attr as NonSerializedAttribute;
                        if (null != tattr)
                        {
                            serialize = false;
                            break;
                        }
                    }
                    #endregion

                    if (serialize)
                    {

                        Result.Add(field.Name, field.GetValue(Input));
                    }
                }
            }
            // return the values...
            return Result;
        }
        #endregion

        #region DeSerialize
        /// <summary>
        /// this method deserializes a given dictionary into an object by matching the keys with the public member variable names
        /// of the object and adding their according values
        /// </summary>
        /// <param name="Input">an dictionary<string,object></param>
        /// <returns>an object</returns>
        public T DeSerialize(Dictionary<String, Object> Input)
        {
            Type reflectiontype = typeof(T);
            List<FieldInfo> Fields = new List<FieldInfo>();

            MD5 MD5CheckSum = new MD5();

            T Output = new T();

            bool serialize = true;
            // reflect the given object...
            foreach (FieldInfo field in reflectiontype.GetFields())
            {
                // check if this field is actually public
                if (field.IsPublic)
                {
                    serialize = true;
                    #region Check for NonSerialized Attribute
                    foreach (Attribute attr in field.GetCustomAttributes(true))
                    {
                        NonSerializedAttribute tattr = attr as NonSerializedAttribute;
                        if (null != tattr)
                        {
                            serialize = false;
                            break;
                        }
                    }
                    #endregion
                    if (serialize)
                    {
                        Fields.Add(field);
                    }
                }
            }
           
            // deserialize
            foreach (FieldInfo field in Fields)
            {
                if (Input.ContainsKey(field.Name))
                {
                    field.SetValue((object)Output, Input[field.Name]);
                    //Input.Remove(field.Name);
                }
                else
                {
                    Debug.WriteLine("[KeyValuePairSerializer] DeSerialize of "+field.Name+" unsuccessfull - does not exists in target object");
                }
            }

            //if (Input.Count > 0)
            //{
            //    foreach (KeyValuePair<string, object> kvpair in Input)
            //    {
            //        Debug.WriteLine("[KeyValuePairSerializer] Key " + kvpair.Key+ " with Value "+kvpair.Value+" was lost in DeSerialization process.");
            //    }
            //}


            return Output;
        }
        #endregion
    }
}
