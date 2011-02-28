using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphFS
{
    public struct GraphElementInformation
    {
        #region data

        /// <summary>
        /// The id of the vertex type
        /// </summary>
        public readonly UInt64 TypeID;

        /// <summary>
        /// A comment for the vertex
        /// </summary>
        public readonly string Comment;

        /// <summary>
        /// The creation date of the vertex
        /// </summary>
        public readonly DateTime CreationDate;

        /// <summary>
        /// The modification date of the vertex
        /// </summary>
        public readonly DateTime ModificationDate;

        /// <summary>
        /// The structured properties
        /// </summary>
        public readonly Dictionary<UInt64, Object> StructuredProperties;

        /// <summary>
        /// The unstructured properties
        /// </summary>
        public readonly Dictionary<String, Object> UnstructuredProperties;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new graph element information
        /// </summary>
        /// <param name="myTypeID">The type id of this graph element</param>
        /// <param name="myComment">The comment on this graph element</param>
        /// <param name="myCreationDate">The creation date of this element</param>
        /// <param name="myModificationDate">The modification date of this element</param>
        /// <param name="myStructuredProperties">The structured properties of this element</param>
        /// <param name="myUnstructuredProperties">The unstructured properties of this element</param>
        public GraphElementInformation(
            UInt64 myTypeID,
            String myComment,
            DateTime myCreationDate,
            DateTime myModificationDate,
            Dictionary<UInt64, Object> myStructuredProperties,
            Dictionary<String, Object> myUnstructuredProperties)
        {
            TypeID = myTypeID;
            Comment = myComment;
            CreationDate = myCreationDate;
            ModificationDate = myModificationDate;
            StructuredProperties = myStructuredProperties;
            UnstructuredProperties = myUnstructuredProperties;
        }

        #endregion
    }
}
