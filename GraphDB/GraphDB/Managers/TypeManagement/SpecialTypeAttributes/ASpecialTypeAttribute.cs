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


#region Usings

using System;
using sones.GraphDB.ObjectManagement;
using sones.Lib.ErrorHandling;

#endregion

namespace sones.GraphDB.TypeManagement.SpecialTypeAttributes
{

    public abstract class ASpecialTypeAttribute : TypeAttribute, IEquatable<ASpecialTypeAttribute>
    {

        /// <summary>
        /// The ShowSetting name
        /// </summary>
        public abstract String ShowSettingName { get; }

        /// <summary>
        /// This will change the <paramref name="myNewDBObject"/> dependend on the SpecialTypeAttribute implementation
        /// </summary>
        /// <param name="myNewDBObject">The DBObject</param>
        /// <param name="myValue">The values which should be assigned to the <paramref name="myNewDBObject"/></param>
        /// <param name="myOptionalParameters">Some optional parameters</param>
        public abstract Exceptional<Object> ApplyTo(DBObjectStream myNewDBObject, Object myValue, params object[] myOptionalParameters);

        /// <summary>
        /// Extracts the value dependend on the SpecialTypeAttribute from the <paramref name="dbObjectStream"/>
        /// </summary>
        /// <param name="dbObjectStream">The dbObjectStream</param>
        /// <param name="graphDBType"></param>
        /// <param name="sessionInfos"></param>
        /// <returns>The extracted value</returns>
        public abstract Exceptional<IObject> ExtractValue(DBObjectStream dbObjectStream, GraphDBType graphDBType, DBContext dbContext);


        #region Operator overloading

        #region Operator == (myASpecialTypeAttribute1, myASpecialTypeAttribute2)

        public static Boolean operator == (ASpecialTypeAttribute myASpecialTypeAttribute1, ASpecialTypeAttribute myASpecialTypeAttribute2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(myASpecialTypeAttribute1, myASpecialTypeAttribute2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) myASpecialTypeAttribute1 == null) || ((Object) myASpecialTypeAttribute2 == null))
                return false;

            return myASpecialTypeAttribute1.Equals(myASpecialTypeAttribute2);

        }

        #endregion

        #region Operator != (myASpecialTypeAttribute1, myASpecialTypeAttribute2)

        public static Boolean operator != (ASpecialTypeAttribute myASpecialTypeAttribute1, ASpecialTypeAttribute myASpecialTypeAttribute2)
        {
            return !(myASpecialTypeAttribute1 == myASpecialTypeAttribute2);
        }

        #endregion

        #endregion

        #region IEquatable<ASpecialTypeAttribute> Members

        #region Equals(myObject)

        public override Boolean Equals(Object myObject)
        {

            if (myObject == null)
                return false;

            var _Object = myObject as ASpecialTypeAttribute;
            if (_Object == null)
                return (Equals(_Object));

            return false;

        }

        #endregion

        #region Equals(myVertex)

        public Boolean Equals(ASpecialTypeAttribute myASpecialTypeAttribute)
        {

            if ((Object) myASpecialTypeAttribute == null)
            {
                return false;
            }

            return this.GetType().FullName == myASpecialTypeAttribute.GetType().FullName;

        }

        #endregion

        #endregion

        #region GetHashCode()

        public override Int32 GetHashCode()
        {
            return this.GetType().FullName.GetHashCode();
        }

        #endregion

    }

}
