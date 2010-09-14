using System;
using System.Collections.Generic;
using System.Text;

namespace sones.GraphDS.API.CSharp
{

    public class GraphDSSharpException : Exception
    {
        public GraphDSSharpException(String myMessage)
            : base(myMessage)
        {
            // do nothing extra
        }
    }

}
