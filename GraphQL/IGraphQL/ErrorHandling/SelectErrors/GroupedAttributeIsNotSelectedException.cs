using System;

namespace sones.GraphQL.ErrorHandling
{
    /// <summary>
    /// A to group attribute is not selected
    /// </summary>
    public sealed class GroupedAttributeIsNotSelectedException : AGraphQLSelectException
    {
        public String TypeAttribute { get; private set; }

        /// <summary>
        /// Creates a new GroupedAttributeIsNotSelectedException exception
        /// </summary>
        /// <param name="myTypeAttribute">The name of the type attribute</param>
        public GroupedAttributeIsNotSelectedException(String myTypeAttribute)
        {
            TypeAttribute = myTypeAttribute;
            _msg = String.Format("The attribute '{0}' is not selected and can not be grouped.", TypeAttribute);
        }
        
    }
}
