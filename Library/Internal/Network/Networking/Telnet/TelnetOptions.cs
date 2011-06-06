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
 * This class holds all TelnetOptions
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
    /// Found at http://www.iana.org/assignments/telnet-options
    /// </summary>
    public enum TelnetOptions : byte
    {

        /// <summary>
        ///     Binary Transmission (RFC 856)
        /// </summary>
        TransmitBinary = 0,

        /// <summary>
        ///     Echo (RFC 857)
        /// </summary>
        Echo = 1,

        /// <summary>
        ///     Reconnection (NIC 50005)
        /// </summary>
        Reconnection = 2,

        /// <summary>
        ///     Suppress Go Ahead (RFC 858)
        /// </summary>
        SuppressGoAhead = 3,

        /// <summary>
        ///     Approximate Message Size Negotiation (Ethernet)
        /// </summary>
        ApproximateMessageSizeNegotiation = 4,

        /// <summary>
        ///     Status (RFC 859)
        /// </summary>
        Status = 5,

        /// <summary>
        ///     Timing Mark (RFC 860)
        /// </summary>
        TimingMark = 6,

        /// <summary>
        ///     Remote Controlled Trans and Echo (RFC 726)
        /// </summary>
        RemoteControlledTransAndEcho = 7,

        /// <summary>
        ///     Output Line Width (NIC50005)
        /// </summary>
        OutputLineWidth = 8,

        /// <summary>
        ///     Output Page Size (NIC50005)
        /// </summary>
        OutputPageSize = 9,

        /// <summary>
        ///     Output Carriage-Return Disposition (RFC 652)
        /// </summary>
        OutputCarriageReturnDisposition = 10,

        /// <summary>
        ///     Output Horizontal Tab Stops (RFC 653)
        /// </summary>
        OutputHorizontalTabStops = 11,

        /// <summary>
        ///     Output Horizontal Tab Disposition (RFC 654)
        /// </summary>
        OutputHorizontalTabDisposition = 12,

        /// <summary>
        ///     Output formfeed disposition (RFC 655)
        /// </summary>
        OutputFormfeedDisposition = 13,

        /// <summary>
        ///     Output Vertical Tab Stops (RFC 656)
        /// </summary>
        OutputVerticalTabStops = 14,

        /// <summary>
        ///     Output Vertical Tab Disposition (RFC 657)
        /// </summary>
        OutputVerticalTabDisposition = 15,

        /// <summary>
        ///     Output Linefeed Disposition (RFC 658)
        /// </summary>
        OutputLinefeedDisposition = 16,

        /// <summary>
        ///     Extended ASCII (RFC 698)
        /// </summary>
        ExtendedAscii = 17,

        /// <summary>
        ///     Logout (RFC 727)
        /// </summary>
        Logout = 18,

        /// <summary>
        ///     Byte Macro (RFC 735)
        /// </summary>
        ByteMacro = 19,

        /// <summary>
        ///     Data Entry Terminal (RRC 1043)
        /// </summary>
        DataEntryTerminal = 20,

        /// <summary>
        ///     SUPDUP (RFC 736,RFC 734)
        /// </summary>
        SUPDUP = 21,

        /// <summary>
        ///     SUPDUP Output (RRC 749)
        /// </summary>
        SUPDUPOutput = 22,

        /// <summary>
        ///     Send Location (RFC 779)
        /// </summary>
        SendLocation = 23,

        /// <summary>
        ///     Terminal type (RFC 1091)
        /// </summary>
        TerminalType = 24,

        /// <summary>
        ///   End of Record (RFC 885)
        /// </summary>
        EndOfRecord = 25,

        /// <summary>
        ///     TACAS User Identification (RFC 927)
        /// </summary>
        TacacsUserId = 26,

        /// <summary>
        ///     Output Marking (RFC 933)
        /// </summary>
        OutputMarking = 27,

        /// <summary>
        ///     Terminal Location (RFC 946)
        /// </summary>
        TerminalLocation = 28,

        /// <summary>
        ///     IBM 3270 Regime (RFC 1041)
        /// </summary>
        Ibm3270Regime = 29,

        /// <summary>
        ///     X.3-PAD (RFC 1053)
        /// </summary>
        X3Pad = 30,

        /// <summary>
        ///     NAWS (Negotiate About Window Size), RFC 1073.
        /// </summary>
        WindowSize = 31,

        /// <summary>
        ///     Terminal Speed (RFC 1079)
        /// </summary>
        TerminalSpeed = 32,

        /// <summary>
        ///     Toggle Flow Control (RFC 1372)
        /// </summary>
        ToggleFlowControl = 33,

        /// <summary>
        ///     Line Mode (RFC 1184)
        /// </summary>
        LineMode = 34,

        /// <summary>
        ///     X Display Location (RFC 1096)
        /// </summary>
        XDisplayLocation = 35,

        /// <summary>
        ///     Environment (RFC 1408)
        /// </summary>
        Environment = 36,

        /// <summary>
        ///     Authentication (RFC 2941)
        /// </summary>
        Authentication = 37,

        /// <summary>
        ///     Encrypt (RFC 2946)
        /// </summary>
        Encrypt = 38,

        /// <summary>
        ///     New Environment (RFC 1572)
        /// </summary>
        NewEnvironment = 39,

        /// <summary>
        ///     TN3270 Enhancements (RFC 2355)
        /// </summary>
        TN3270E = 40,

        /// <summary>
        ///     XAUTH (Earhart)
        /// </summary>
        XAUTH = 41,

        /// <summary>
        ///     Character Set (RFC 2066)
        /// </summary>
        CharacterSet = 42,

        /// <summary>
        ///     Telnet Remote Serial Port (RSP) (Barnes)
        /// </summary>
        RemoteSerialPort = 43,

        /// <summary>
        ///     Communications Port (RFC 2217)
        /// </summary>
        ComPort = 44,

        /// <summary>
        ///     Telnet Suppress Local Echo (Atmar)
        /// </summary>
        SuppressLocalEcho = 45,

        /// <summary>
        ///     Telnet Start TLS (Boe)
        /// </summary>
        StartTLS = 46,

        /// <summary>
        ///     Kermit (RFC 2840)
        /// </summary>
        Kermit = 47,
    
    }
}
