/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
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
* 
*/

/* GraphWebDAV
 * (c) Stefan Licht, 2009
 * 
 * This class acts like a Packet for telnet commands.
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

    public enum TelnetSymbolTypes
    {
        Unknown,
        Data,
        Command,
        Subnegotiation,
    }

    /// <summary>
    /// This interpretes a bunch of bytes into a meaningfull telnet symbol.
    /// This could be an option command or the data itself.
    /// </summary>
    public class TelnetSymbol
    {

        private TelnetSymbolTypes _TelnetSymbolType;
        public TelnetSymbolTypes TelnetSymbolType
        {
            get { return _TelnetSymbolType; }
        }

        private Byte[] _Data;
        public Byte[] Data
        {
            get { return _Data; }
        }

        private TelnetCommands _Command;
        public TelnetCommands Command
        {
            get { return _Command; }
        }

        private TelnetOptions _Option;
        public TelnetOptions Option
        {
            get { return _Option; }
        }

        public TelnetSymbol(TelnetCommands myCommand, TelnetOptions myOption)
        {
            _Command = myCommand;
            _Option = myOption;

            _TelnetSymbolType = TelnetSymbolTypes.Command;
        }

        public TelnetSymbol(TelnetCommands myCommand)
        {
            _Command = myCommand;
            _TelnetSymbolType = TelnetSymbolTypes.Command;
        }

        public TelnetSymbol(Byte[] myDataBytes)
        {
            _Data = myDataBytes;
            _TelnetSymbolType = TelnetSymbolTypes.Data;
        }
    }
}
