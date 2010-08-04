/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
*
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
*/


/*
 * SHA1 - Generates cryptographical secure hashes based on the SHA1 algorithm
 *
 * Achim Friedland, 2008 - 2009
 * (c) Daniel Kirstenpfad 2009
 * 
 * Lead programmer:
 *      Achim Friedland
 *      Daniel Kirstenpfad
 * 
 * */

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
