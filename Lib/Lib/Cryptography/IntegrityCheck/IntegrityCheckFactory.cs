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
 * IntegrityCheckFactory
 * Achim Friedland, 2008 - 2009
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
