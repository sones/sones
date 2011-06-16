/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

#endregion


namespace sones.Library.LanguageExtensions
{
    public static class AnyClassExtensions
    {
        public static IEnumerable<T> SingleEnumerable<T>(this T mySingleInstance)
        {
            return Enumerable.Repeat(mySingleInstance, 1);
        }

        public static void CheckNull(this Object myObject, String myArgumentName = "")
        {
            if (null == myObject)
                throw new ArgumentNullException(myArgumentName);
        }        

        public static IComparable ConvertToIComparable(this Object myObject, Type myConvertType)
        {
            if (myObject == null)
                throw new NullReferenceException();

            #region some special magic

            if (typeof(DateTime).Equals(myConvertType) && typeof(long).Equals(myObject.GetType()))
                return DateTime.FromBinary((long) myObject);

            #endregion

            return (IComparable) Convert.ChangeType(myObject, myConvertType, CultureInfo.GetCultureInfo("en-us"));
        }

    }
}
