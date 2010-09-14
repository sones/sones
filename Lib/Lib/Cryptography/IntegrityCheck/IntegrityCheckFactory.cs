/*
 * IntegrityCheckFactory
 * (c) Achim Friedland, 2008 - 2009
 * 
 * A factory which generates a appropriate IIntegrityCheck for you!
 * 
 * Lead programmer:
 *      Achim Friedland
 * 
 * */

#region Usings

using System;
using System.Collections.Generic;

//using sones.Graph.Storage.StorageEngines;
using sones.Lib.Cryptography;

#endregion

namespace sones.Lib.Cryptography.IntegrityCheck
{

    /// <summary>
    /// A factory which generates a apropriate ICryptoHash for you!
    /// </summary>

    public class IntegrityCheckFactory
    {


        #region Generate(myIntegrityCheckType)

        public static IIntegrityCheck Generate(IntegrityCheckTypes myIntegrityCheckType)
        {

            if (myIntegrityCheckType == IntegrityCheckTypes.NULLAlgorithm)
                return new NULLAlgorithm();

            else if (myIntegrityCheckType == IntegrityCheckTypes.MD5)
                return new MD5();

            else if (myIntegrityCheckType == IntegrityCheckTypes.SHA1)
                return new SHA1();

            else
                throw new CryptographyExceptions_ProtocolNotSupported("The protocol id '" + myIntegrityCheckType + "' is not supported!");
            
        }

        #endregion


    }

}
