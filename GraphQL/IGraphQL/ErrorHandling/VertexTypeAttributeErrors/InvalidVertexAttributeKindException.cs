using System;
using System.Linq;
using System.Text;
using sones.Library.ErrorHandling;

namespace sones.GraphQL.ErrorHandling
{
    /// <summary>
    /// The given kind of attribute does not match
    /// </summary>
    public sealed class InvalidVertexAttributeKindException : AGraphQLVertexAttributeException
    {
        #region data

        public String[] ExpectedKindsOfType { get; private set; }
        public String CurrentKindsOfType { get; private set; }

        #endregion

        #region constructor        

        /// <summary>
        /// Creates a new InvalidVertexAttributeKindException exception
        /// </summary>
        public InvalidVertexAttributeKindException()
        {
            ExpectedKindsOfType = new String[0];
        }

        /// <summary>
        /// Creates a new InvalidVertexAttributeKindException exception
        /// </summary>
        /// <param name="myCurrentKindsOfType">The current kind of type</param>
        /// <param name="myExpectedKindsOfType">A List of expected kind of types</param>
        public InvalidVertexAttributeKindException(String myCurrentKindsOfType, params String[] myExpectedKindsOfType)
            : this()
        {
            ExpectedKindsOfType = myExpectedKindsOfType;
            CurrentKindsOfType = myCurrentKindsOfType;
        }

        #endregion

        public override string ToString()
        {
            return String.Format("The given kind \"{0}\" does not match the expected \"{0}\"", CurrentKindsOfType,
                ExpectedKindsOfType.Aggregate<String, StringBuilder>(new StringBuilder(), (result, elem) => { result.AppendFormat("{0},", elem); return result; }));
        }

    }
}
