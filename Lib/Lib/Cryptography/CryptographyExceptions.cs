/*
 * CryptographyExceptions
 * (c) Achim Friedland, 2008 - 2009
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace sones.Lib.Cryptography
{

    /// <summary>
    /// This is a class for all CryptographyExceptions
    /// </summary>

    #region CryptographyExceptions Superclass

    public class CryptographyExceptions : ApplicationException
    {
        public CryptographyExceptions(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    #endregion

    #region CryptographyExceptions

    public class CryptographyExceptions_ProtocolNotSupported : CryptographyExceptions
    {
        public CryptographyExceptions_ProtocolNotSupported(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    #endregion

}
