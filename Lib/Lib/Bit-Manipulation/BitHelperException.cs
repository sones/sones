using System;
using System.Collections.Generic;
using System.Text;

namespace sones.Libraries.Exceptions
{
    public class BitHelperException : ApplicationException
    {
        public BitHelperException(string message)
            : base(message) 
		{
			// do nothing extra
		}
    }
}
