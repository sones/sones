using System;
using sones.Library.ErrorHandling;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// Undefined attributes can not inserted nor updated without setting SETUNDEFBEHAVE 
    /// </summary>
    public sealed class UndefinedAttributesException : AGraphDBVertexAttributeException
    {
        /// <summary>
        /// Creates a new UndefinedAttributesException exception
        /// </summary>
        public UndefinedAttributesException()
        {
        }

        public override string ToString()
        {
            return "Undefined attributes can not inserted nor updated. Use the setting SETUNDEFBEHAVE to change this behaviour.";
        }

    }
}
