using System;
using sones.Library.ErrorHandling;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// The user defined type should not be used with LIST attributes
    /// </summary>
    public sealed class ListAttributeNotAllowedException : AGraphDBTypeException
    {
        public String TypeName { get; private set; }
        
        /// <summary>
        /// Creates a new ListAttributeNotAllowedException exception
        /// </summary>
        /// <param name="myTypeName">The name of the type</param>
        public ListAttributeNotAllowedException(String myTypeName)
        {
            TypeName = myTypeName;
        }

        public override string ToString()
        {
            return String.Format("The user defined type \\{0}\\ should not be used with LIST<> attributes, please use SET<> instead.", TypeName);
        }

        public override ushort ErrorCode
        {
            get { return ErrorCodes.ListAttributeNotAllowed; }
        }
    }
}
