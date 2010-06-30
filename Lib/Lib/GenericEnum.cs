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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Lib
{

    public static class Enum<T>
    {

        public static T Parse(String myValue)
        {
            return Enum<T>.Parse(myValue, false);
        }

        public static T Parse(String myValue, Boolean myIgnoreCase)
        {
            return (T)Enum.Parse(typeof(T), myValue, myIgnoreCase);
        }

        public static T ParseWithDefault(String myValue, T myDefault)
        {
            return Enum<T>.ParseWithDefault(myValue, false, myDefault);
        }

        public static T ParseWithDefault(String myValue, Boolean myIgnoreCase, T myDefault)
        {
            T retval;
            Enum<T>.TryParse(myValue, myIgnoreCase, myDefault, out retval);
            return retval;
        }

        public static Boolean TryParse(String myValue, out T rc)
        {
            return Enum<T>.TryParse(myValue, false, out rc);
        }

        public static Boolean TryParse(String myValue, Boolean myIgnoreCase, out T rc)
        {
            return Enum<T>.TryParse(myValue, myIgnoreCase, default(T), out rc);
        }

        public static Boolean TryParse(String myValue, Boolean myIgnoreCase, T myDefault, out T rc)
        {

            try
            {
                rc = Enum<T>.Parse(myValue, myIgnoreCase);
                return true;
            }

            catch
            {                
                rc = myDefault;
                return false;
            }

        }

        public static T[] Values
        {
            get { return Enum.GetValues(typeof(T)) as T[]; }
        }

    }

}
