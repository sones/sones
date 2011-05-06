using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.ErrorHandling
{
    public sealed class VertexTypeRemoveException : AGraphDBException
    {
        public String VertexType { get; private set; }
        public String Info { get; private set; }

        /// <summary>
        /// Creates a new TypeDoesNotMatchException exception
        /// </summary>
        /// <param name="myExpectedVertexType">The expected type</param>
        /// <param name="myCurrentVertexType">The current type</param>
        public VertexTypeRemoveException(String myVertexType, String myInfo)
        {
            VertexType = myVertexType;
            Info = myInfo;

            _msg = String.Format("The Vertex Type {0} cannot be removed.\n\n{1}.", VertexType, Info);
        }
    }
}
