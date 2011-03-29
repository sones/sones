using System;
using sones.Library.ErrorHandling;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// A to group attribute is not selected
    /// </summary>
    public sealed class GroupedAttributeIsNotSelectedException : AGraphDBSelectException
    {
        public String TypeAttribute { get; private set; }

        /// <summary>
        /// Creates a new GroupedAttributeIsNotSelectedException exception
        /// </summary>
        /// <param name="myTypeAttribute">The name of the type attribute</param>
        public GroupedAttributeIsNotSelectedException(String myTypeAttribute)
        {
            TypeAttribute = myTypeAttribute;
        }

        public override string ToString()
        {
            return String.Format("The attribute '{0}' is not selected and can not be grouped.", TypeAttribute);
        }

        public override ushort ErrorCode
        {
            get { return ErrorCodes.GroupedAttributeIsNotSelected; }
        }
    }
}
