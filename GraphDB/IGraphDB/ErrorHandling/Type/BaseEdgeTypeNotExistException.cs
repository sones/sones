using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.ErrorHandling
{
    public class BaseEdgeTypeNotExistException: AGraphDBTypeException
    {
        /// <summary>
        /// A specific base type does not exist.
        /// </summary>
        /// <param name="myType"></param>
        /// <param name="myInfo"></param>
        public BaseEdgeTypeNotExistException(String myType, String myInfo)
        {
            _msg = "The base edge type " + myType + " does not exist! " + myInfo;
        }

        /// <summary>
        /// All base types does not exist.
        /// </summary>
        /// <param name="myInfo"></param>
        public BaseEdgeTypeNotExistException(String myInfo)
        {
            _msg = "No base edge type found! Check base type management " + myInfo;
        }
    }
}
