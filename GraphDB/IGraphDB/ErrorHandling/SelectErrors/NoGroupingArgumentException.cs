using System;
using sones.Library.ErrorHandling;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// Missing grouped argument in a selection with aggregates 
    /// </summary>
    public sealed class NoGroupingArgumentException : AGraphDBSelectException
    {

        public String Selection { get; private set; }

        /// <summary>
        /// Creates a new NoGroupingArgumentException exception
        /// </summary>
        /// <param name="mySelection"></param>
        public NoGroupingArgumentException(String mySelection)
        {
            Selection = mySelection;
        }

        public override string ToString()
        {
            return "A selection with aggregates must be grouped. Missing for selection " + Selection;
        }

        public override ushort ErrorCode
        {
            get { return ErrorCodes.NoGroupingArgument; }
        }
    }
}
