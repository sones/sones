#region Usings

using System;
using sones.Library.ErrorHandling;

#endregion

namespace sones.Plugins.Index.ErrorHandling
{
    public class ASonesIndexException : ASonesException
    {
        public override ushort ErrorCode
        {
            get { throw new NotImplementedException(); }
        }
    }
}