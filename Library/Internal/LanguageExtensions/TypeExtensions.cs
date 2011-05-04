using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.LanguageExtensions
{
    public static class TypeExtensions
    {

        /// <summary>
        /// Determines if the type implements a specific interface type
        /// </summary>
        /// <param name="myType"></param>
        /// <param name="myInterfaceType"></param>
        /// <returns></returns>
        public static bool HasInterface(this Type myType, Type myInterfaceType)
        {

            Type[] array = myType.FindInterfaces(
                delegate(Type typeObj, Object criteriaObj)
                {

                    return typeObj.Equals((Type)criteriaObj);

                },

                myInterfaceType

            );

            return array.Length > 0;

        }
    }
}
