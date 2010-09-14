using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Networking.HTTP
{

    public interface ICustomErrorPageHandler
    {

        Byte[] GetCustomErrorPage(HTTPStatusCodes myHTTPStatusCodes, HTTPHeader myRequestHeader, Byte[] myRequestBody, Exception myLastException);

    }

}
