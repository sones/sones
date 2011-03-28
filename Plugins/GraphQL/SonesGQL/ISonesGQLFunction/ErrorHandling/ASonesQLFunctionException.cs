using System;
using sones.Library.ErrorHandling;

namespace sones.Plugins.SonesGQL.Function.ErrorHandling
{
    public class ASonesQLFunctionException : ASonesException
    {
        public override ushort ErrorCode
        {
            get { throw new NotImplementedException(); }
        }
    }
}
