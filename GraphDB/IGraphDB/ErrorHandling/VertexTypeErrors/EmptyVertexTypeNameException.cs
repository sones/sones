using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Request;

namespace sones.GraphDB.ErrorHandling.VertexTypeErrors
{
    /// <summary>
    /// The exception that is thrown if a vertex type with an empty name should be added.
    /// </summary>
    public class EmptyVertexTypeNameException: AGraphDBException
    {
        /// <summary>
        /// Creates an instance of EmptyVertexTypeNameException.
        /// </summary>
        /// <param name="myPredefinition">The predefinition that causes the exception.</param>
        public EmptyVertexTypeNameException(VertexTypePredefinition myPredefinition)
        {
            Predefinition = myPredefinition;
            _msg = "A vertex type with no name can not be added.";
        }

        /// <summary>
        /// The predefinition that causes the exception.
        /// </summary>
        public VertexTypePredefinition Predefinition { get; set; }
    }
}
