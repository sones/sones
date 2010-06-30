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
 * IIntegrityCheck - The Interface for all cryptographical secure hashes
 * Achim Friedland, 2008 - 2009
 * 
 * Lead programmer:
 *      Achim Friedland
 *      Daniel Kirstenpfad
 * 
 * 
 * ToDo:
 *  - Check http://www.bouncycastle.org/csharp/index.html
 * 
 * */

#region Usings

using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Threading;

#endregion

namespace sones.Lib.Cryptography.IntegrityCheck
{

    /// <summary>
    /// The Interface for all cryptographical secure hashes
    /// </summary>

    public interface IIntegrityCheck
    {

        int     HashSize { get; }
        int     HashStringLength { get; }

        String  GetHashValue(String ClearText);
        String  GetHashValue(byte[] ClearText);

        Byte[]  GetHashValueAsByteArray(String ClearText);
        Byte[]  GetHashValueAsByteArray(byte[] ClearText);

        Boolean CheckHashValue(String ClearText, String HashValue);
        Boolean CheckHashValue(byte[] ClearText, byte[] HashValue);
        Boolean CheckHashValue(byte[] ClearText, String HashValue);
        Boolean CheckHashValue(String ClearText, byte[] HashValue);

    }

}