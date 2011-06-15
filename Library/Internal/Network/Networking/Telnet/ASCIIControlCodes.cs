/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

/* GraphWebDAV
 * (c) Stefan Licht, 2009
 * 
 * This class holds all ASCII control codes
 * 
 * Lead programmer:
 *      Stefan Licht
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Networking.Telnet
{
    /// <summary>
    /// Found at http://www.faqs.org/rfcs/rfc854.html
    /// </summary>
    public enum ASCIIControlCodes : byte
    {

        /// <summary>
        ///     Null character - No Operation (ASCII 0)
        /// </summary>
        NUL = 0,

        /// <summary>
        ///     Bell (ASCII 7)
        /// </summary>
        BEL = 7,

        /// <summary>
        ///     Backspace (ASCII 8)
        /// </summary>
        BS = 8,

        /// <summary>
        ///     Horizontal tab (ASCII 9)
        /// </summary>
        HT = 9,

        /// <summary>
        ///     Line Feed (ASCII 10)
        /// </summary>
        LF = 10,

        /// <summary>
        ///     Vertical Tab (ASCII 11)
        /// </summary>
        VT = 11,

        /// <summary>
        ///     Form Feed (ASCII 12)
        /// </summary>
        FF = 12,

        /// <summary>
        ///     Carriage Return (ASCII 13)
        /// </summary>
        CR = 13,

    }
}
