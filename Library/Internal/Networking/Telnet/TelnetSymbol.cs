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
