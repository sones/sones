using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.ErrorHandling
{
    public class TypeRemoveException<T>: AGraphDBTypeException
        where T: IBaseType
    {
        public TypeRemoveException(String myType, String myInfo = "")
        {
            _msg = String.Format("The " +  typeof(T).Name + " Type {0} cannot be removed.\n\n{1}.", myType, myInfo);
        }
    }
}
