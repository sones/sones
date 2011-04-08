using System;
using sones.Library.ErrorHandling;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// The selected vertex attribute is not valid
    /// </summary>
    public sealed class InvalidVertexAttributeSelectionException : AGraphDBVertexAttributeException
    {
        public String SelectedVertexAttribute { get; private set; }

        /// <summary>
        /// Creates a new InvalidVertexAttributeSelectionException exception
        /// </summary>
        /// <param name="mySelectedVertexAttribute">The current selected vertex attribute</param>
        public InvalidVertexAttributeSelectionException(String mySelectedVertexAttribute)
        {
            SelectedVertexAttribute = mySelectedVertexAttribute;
        }

        public override string ToString()
        {
            return String.Format("The selected vertex attribute \"{0}\" is not valid!", SelectedVertexAttribute);
        }
    }
}
