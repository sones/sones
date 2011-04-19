using System;

namespace sones.GraphQL.ErrorHandling
{
    /// <summary>
    /// The mandatory constraint of a vertex attribute was violated
    /// </summary>
    public sealed class MandatoryConstraintViolationException : AGraphQLVertexAttributeException
    {
        public String MandatoryConstraint { get; private set; }

        /// <summary>
        /// Creates a new MandatoryConstraintViolationException exception
        /// </summary>
        /// <param name="myMandatoryConstraint">The mandatory constraint info message</param>
        public MandatoryConstraintViolationException(String myMandatoryConstraint)
        {
            MandatoryConstraint = myMandatoryConstraint;
            _msg = String.Format("The mandatory constraint \"{0}\" was violated!", MandatoryConstraint);
        }
        
    }
}

