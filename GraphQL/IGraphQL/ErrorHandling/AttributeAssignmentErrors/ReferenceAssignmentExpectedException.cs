using System;

namespace sones.GraphQL.ErrorHandling
{
    /// <summary>
    /// A attribute expects a Reference assignment
    /// </summary>
    public sealed class ReferenceAssignmentExpectedException : AGraphQLAttributeAssignmentException
    {
        public String TypeAttribute { get; private set; }
        public String Info { get; private set; }
        
        /// <summary>
        /// Creates a new ReferenceAssignmentExpectedException exception
        /// </summary>
        /// <param name="myTypeAttribute">The type of the attribute</param>
        public ReferenceAssignmentExpectedException(String myTypeAttribute)
        {
            TypeAttribute = myTypeAttribute;
            _msg = String.Format("The attribute [{0}] expects a Reference assignment!", TypeAttribute);
        }
          
    }
}
