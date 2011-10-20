using System;
using System.Collections.ObjectModel;
//+
namespace Jampad.Dojo.Rpc
{
    [Serializable]
    public class DojoRpcException : Exception
    {
        //- @Ctor -//
        public DojoRpcException() { }

        //- @Ctor -//
        public DojoRpcException(string message) : base(message) { }

        //- @Ctor -//
        public DojoRpcException(string message, Exception inner) : base(message, inner) { }

        //- #Ctor -//
        protected DojoRpcException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}