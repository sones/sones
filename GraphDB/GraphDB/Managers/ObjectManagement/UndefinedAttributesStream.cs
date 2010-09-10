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
 * UndefinedAttributesStream
 * (c) Dirk Bludau, 2010
 *     Achim Friedland, 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using sones.GraphDBInterface.TypeManagement;
using sones.GraphFS.DataStructures;
using sones.GraphFS.Objects;
using sones.Lib.DataStructures;
using sones.Lib.DataStructures.Indices;
using sones.Lib.NewFastSerializer;

#endregion

namespace sones.GraphDB.ObjectManagement
{

    /// <summary>
    /// Contains undefined attributes of an particular DBObject.
    /// </summary>
    public class UndefinedAttributesStream : AIndexObject<String, IObject>// ADictionaryObject<String, IObject>
    {


        #region Constructor(s)

        public UndefinedAttributesStream()
        {
            // Members of AGraphStructure
            _StructureVersion = 1;

            // Members of AGraphObject
            _ObjectStream = DBConstants.UNDEFATTRIBUTESSTREAM;

            // Object specific data...

            // Set ObjectUUID
            if (ObjectUUID.Length == 0)
                ObjectUUID = base.ObjectUUID;
        }

        public UndefinedAttributesStream(Dictionary<String, IObject> myUndefAttributes, ObjectLocation myObjectLocation)
            : this()
        {
            if (myObjectLocation == null || myObjectLocation.Length < FSPathConstants.PathDelimiter.Length)
                throw new ArgumentNullException("Invalid ObjectLocation!");
            
            ObjectLocation = myObjectLocation;

            if(myUndefAttributes != null)
                base.Add(myUndefAttributes);
        }

        public UndefinedAttributesStream(ObjectLocation myObjectLocation)
            : this(null, myObjectLocation)
        { 
        
        }
        
        #endregion


        /// <summary>
        /// true if the attribute is in the stream
        /// </summary>
        /// <param name="myName">the name of the undefined attribute</param>
        /// <returns>an Boolean</returns>
        public Boolean ContainsAttribute(String myName)
        {
            return base.ContainsKey(myName);
        }


        /// <summary>
        /// add an undefined attribute to the stream without flush
        /// </summary>
        /// <param name="myName">the name of the attribute</param>
        /// <param name="myValue">the value for the attribute</param>
        public void AddAttribute(String myName, IObject myValue)
        {
            Set(myName, myValue, IndexSetStrategy.REPLACE);
            isDirty = true;
        }
        

        /// <summary>
        /// remove an existing attribute from stream
        /// </summary>
        /// <param name="myName">attribute name</param>
        /// <returns>an Boolean</returns>
        public Boolean RemoveAttribute(String myName)
        {

            if (!ContainsAttribute(myName) || !base.Remove(myName))
                return false;

            isDirty = true;

            return true;

        }


        /// <summary>
        /// return a dictionary of all undefined attributes
        /// </summary>
        /// <returns></returns>
        public IDictionary<String, IObject> GetAllAttributes()
        {
            return base.GetIDictionary().ToDictionary(k => k.Key, v => v.Value.First());
        }


        /// <summary>
        /// return the value for an undefined attribute
        /// </summary>
        /// <param name="myName">attribute name</param>
        /// <returns></returns>
        public new IObject this[String myName]
        {

            get
            {
                
                if (ContainsAttribute(myName))
                    return base[myName].First();
                
                return null;

            }

        }


        #region Clone

        public override AFSObject Clone()
        {
            var NewAttributes = new UndefinedAttributesStream();
            NewAttributes.Deserialize(Serialize(null, null, false), null, null, this);

            return NewAttributes;
        }

        #endregion

        #region (de)serialize

        public override void Serialize(ref SerializationWriter mySerializationWriter)
        {
            base.Serialize(ref mySerializationWriter);
        }

        public override void Deserialize(ref SerializationReader mySerializationReader)
        {
            base.Deserialize(ref mySerializationReader);
        }

        #endregion


    }

}
