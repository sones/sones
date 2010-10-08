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
