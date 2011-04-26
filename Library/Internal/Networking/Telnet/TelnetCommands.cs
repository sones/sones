/* GraphWebDAV
 * (c) Stefan Licht, 2009
 * 
 * This class holds all TelnetCommands
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
    public enum TelnetCommands : byte
    {
        /// <summary>
        ///     End of Subnegotiation (SE)
        ///     http://www.tcpipguide.com/free/t_TelnetOptionsandOptionNegotiation-4.htm
        /// </summary>
        /// <remarks>
        ///     End of subnegotiation parameters.
        /// </remarks>
        /// <seealso cref="Subnegotiation"/>
        SubnegotiationEnd = 240,

        /// <summary>
        ///     No operation (NOP)
        /// </summary>
        NoOperation = 241,

        /// <summary>
        ///     Data Mark - The data stream portion of a Synch.
        ///     This should always be accompanied by a TCP Urgent notification.
        /// </summary>
        DataMark = 242,

        /// <summary>
        ///     Break (BRK)
        ///     NVT character BRK.
        /// </summary>
        Break = 243,

        /// <summary>
        ///     Interrupt Process (IP)
        /// </summary>
        InterruptProcess = 244,

        /// <summary>
        ///     Abort output (AO)
        /// </summary>
        AbortOutput = 245,

        /// <summary>
        ///     Are You There (AYT)
        /// </summary>
        AreYouThere = 246,

        /// <summary>
        ///     Erase Character (EC)
        /// </summary>
        /// <seealso cref="EraseLine"/>
        EraseCharacter = 247,

        /// <summary>
        ///     Erase Line (EL)
        /// </summary>
        /// <seealso cref="EraseCharacter"/>
        EraseLine = 248,

        /// <summary>
        ///     Go Ahead (GA)
        /// </summary>
        GoAhead = 249,

        /// <summary>
        ///     Subnegotiation (SB)
        ///     Indicates that what follows is subnegotiation of the indicated option.
        ///     http://www.tcpipguide.com/free/t_TelnetOptionsandOptionNegotiation-4.htm
        /// </summary>
        /// <seealso cref="EndSubnegotiation"/>
        Subnegotiation = 250,

        /// <summary>
        ///     Will do option (WILL)
        ///     Indicates the desire to begin performing, or confirmation that you are now performing, the indicated option.
        /// </summary>
        /// <seealso cref="Do"/>
        /// <seealso cref="Dont"/>
        /// <seealso cref="Wont"/>
        Will = 251,

        /// <summary>
        ///     Will not do option (WONT)
        ///     Indicates the refusal to perform, or continue performing, the indicated option.
        /// </summary>
        /// <seealso cref="Do"/>
        /// <seealso cref="Dont"/>
        /// <seealso cref="Will"/>
        Wont = 252,

        /// <summary>
        ///     Do option (DO)
        ///     Indicates the request that the other party perform, or confirmation that you are expecting
        ///     the other party to perform, the indicated option.
        /// </summary>
        /// <seealso cref="Dont"/>
        /// <seealso cref="Will"/>
        /// <seealso cref="Wont"/>
        Do = 253,

        /// <summary>
        ///     Don't do option (DONT)
        ///     Indicates the demand that the other party stop performing, or confirmation that you are no
        ///     longer expecting the other party to perform, the indicated option.
        /// </summary>
        /// <seealso cref="Do"/>
        /// <seealso cref="Will"/>
        /// <seealso cref="Wont"/>
        Dont = 254,

        /// <summary>
        ///     Data Byte 255 (IAC)
        /// </summary>
        Iac = 255
    }
}
