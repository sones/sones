/*
 * StorageEngineExceptions
 * (c) Achim Friedland, 2008 - 2009
 * 
 * This is a class for all GraphFSExceptions
 * 
 * Lead programmer:
 *      Achim Friedland
 * 
 * */

#region Usings

using System;
using System.Text;

#endregion

namespace sones.StorageEngines
{

    /// <summary>
    /// This is a class for all storage engine exceptions!
    /// </summary>

    #region StorageEngineException Superclass

    public class StorageEngineException : Exception
    {
        public StorageEngineException(String message)
            : base(message) 
		{
			// do nothing extra
		}
    }

    #endregion

    #region StorageEngineException

    public class StorageEngineException_ProtocolNotSupported : StorageEngineException
    {
        public StorageEngineException_ProtocolNotSupported(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class StorageEngineException_AlreadyUsed : StorageEngineException
    {
        public StorageEngineException_AlreadyUsed(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    #endregion

}
