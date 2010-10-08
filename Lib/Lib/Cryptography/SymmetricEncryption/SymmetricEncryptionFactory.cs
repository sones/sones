/*
 * SymmetricEncryptionFactory
 * (c) Achim Friedland, 2008 - 2009
 * 
 * A factory which generates a appropriate ISymmetricEncryption for you!
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

namespace sones.Lib.Cryptography.SymmetricEncryption
{

    /// <summary>
    /// A factory which generates a apropriate ISymmetricEncryption for you!
    /// </summary>

    public class SymmetricEncryptionFactory
    {


        #region Generate(myIntegrityCheckType)

        public static ISymmetricEncryption Generate(SymmetricEncryptionTypes mySymmetricEncryptionType)
        {

            if (mySymmetricEncryptionType == SymmetricEncryptionTypes.NULLAlgorithm)
                return new NULLAlgorithm();

            else if (mySymmetricEncryptionType == SymmetricEncryptionTypes.AES)
                return new AES();

            else
                throw new CryptographyExceptions_ProtocolNotSupported("The protocol id '" + mySymmetricEncryptionType + "' is not supported!");

        }

        #endregion

    }

}
