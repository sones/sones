/*
 * IIntegrityCheck - SHA1
 * (c) Achim Friedland, 2008 - 2010
 */

#region Usings

using System;

#endregion

namespace sones.Lib.Cryptography.IntegrityCheck
{

    /// <summary>
    /// Generates cryptographical secure hashes based on the SHA1 algorithm
    /// </summary>

    public class SHA1 : AIntegrityCheck
    {

        #region Properties

        #region HashSize

        public override Int32 HashSize
        {
            get
            {
                return 20;
            }
        }

        #endregion

        #region HashStringLength

        public override Int32 HashStringLength
        {
            get
            {
                return 40;
            }
        }

        #endregion

        #endregion

        #region Constructor

        public SHA1()
        {
            _Hasher = System.Security.Cryptography.SHA1.Create();
        }

        #endregion

    }

}
