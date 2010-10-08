/*
 * WebDAVExceptions
 * (c) Achim Friedland, 2009
 * 
 * This is a class for all Graph WebDAVException
 * 
 * Lead programmer:
 *      Achim Friedland
 * 
 * */

#region Usings

using System;
using System.Text;

#endregion

namespace sones.GraphDS.Connectors.WebDAV
{

    /// <summary>
    /// This is a class for all WebDAV exceptions!
    /// </summary>

    #region WebDAVException Superclass

    public class WebDAVException : ApplicationException
    {
        public WebDAVException(String message)
            : base(message) 
		{
			// do nothing extra
		}
    }

    #endregion

    #region WebDAVException

    public class WebDAVException_ProtocolNotSupported : WebDAVException
    {
        public WebDAVException_ProtocolNotSupported(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    #endregion



}
