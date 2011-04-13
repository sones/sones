using System;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// The name of the attribute is not valid
    /// </summary>
    public sealed class InvalidVertexAttributeNameException : AGraphDBVertexAttributeException
    {
        public String Info { get; private set; }

        /// <summary>
        /// Creates a new InvalidVertexAttributeNameException exception
        /// </summary>
        /// <param name="myInfo"></param>
        public InvalidVertexAttributeNameException(String myInfo)
        {
            Info = myInfo;
            _msg = String.Format("The vertex attribute name is not valid: {0}", Info);

        }

    }
}
