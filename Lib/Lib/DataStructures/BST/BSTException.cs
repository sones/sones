using System;
using System.Collections.Generic;
using System.Text;

namespace sones.Lib.BExceptions
{
    public class BSTException : ApplicationException
    {
        public BSTException(string message) : base(message) 
		{
			// do nothing extra
		}
    }
}
