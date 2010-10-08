/*
 * FastReflectionSerializerExceptions
 * (c) Daniel Kirstenpfad, 2009
 * 
 * Lead programmer:
 *      Daniel Kirstenpfad
 * 
 * */

#region Usings

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace sones.Lib.Serializer
{

    /// <summary>
    /// This is a class for all FastReflectionSerializerExceptions
    /// </summary>

    #region FastReflectionSerializerExceptions Superclass

    public class FastReflectionSerializerExceptions : ApplicationException
    {
        public FastReflectionSerializerExceptions(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    #endregion

    #region FastReflectionSerializerExceptions

    public class FastReflectionSerializerExceptions_DeSerializeHashNotValid : FastReflectionSerializerExceptions
    {
        public FastReflectionSerializerExceptions_DeSerializeHashNotValid(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    #endregion

}
