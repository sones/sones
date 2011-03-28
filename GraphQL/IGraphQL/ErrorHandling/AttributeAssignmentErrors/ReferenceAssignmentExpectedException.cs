using System;
using sones.Library.ErrorHandling;

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
        }

        public override string ToString()
        {
            return String.Format("The attribute [{0}] expects a Reference assignment!", TypeAttribute);
        }

        public override ushort ErrorCode
        {
            get { return ErrorCodes.ReferenceAssignmentExpected; }
        }  
    }
}
