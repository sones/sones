/*
 * IIntegrityCheck - MD5
 * (c) Achim Friedland, 2008 - 2010
 */

#region Usings

using System;

#endregion

namespace sones.Lib.Cryptography.IntegrityCheck
{

    /// <summary>
    /// Generates cryptographical secure hashes based on the MD5 algorithm
    /// </summary>
    
    public class MD5 : AIntegrityCheck
    {

        #region Properties

        #region HashSize

        public override Int32 HashSize
        {
            get
            {
                return 16;
            }
        }

        #endregion

        #region HashStringLength

        public override Int32 HashStringLength
        {
            get
            {
                return 32;
            }
        }

        #endregion

        #endregion

        #region Constructor

        public MD5()
        {
            _Hasher = System.Security.Cryptography.MD5.Create();
        }

        #endregion

    }

}
