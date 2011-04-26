using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Networking.UDPSocket
{
    public class UDPSocketBufferOverflowException : Exception
    {
        public UDPSocketBufferOverflowException(String message)
            : base(message)
        { }
    }
}
