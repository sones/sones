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
