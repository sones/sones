using System;

namespace sones.GraphQL.ErrorHandling
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class ReferenceAssignmentException : AGraphQLAttributeAssignmentException
    {
        public String Info { get; private set; }

        /// <summary>
        /// Creates a new ReferenceAssignmentException exception
        /// </summary>
        /// <param name="myInfo">Variable for additional infos</param>
        public ReferenceAssignmentException(String myInfo)
        {
            Info = myInfo;
            _msg = Info;
        }
                        
    }
}
