using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.PropertyHyperGraph;
using System.IO;

namespace sones.GraphFS
{
    /// <summary>
    /// This class represents the filesystem definition for a vertex
    /// </summary>
    public sealed class VertexInsertDefinition
    {
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
        /// The edition of the vertex
        /// </summary>
        public readonly string Edition;

        /// <summary>
        /// The structured properties
        /// </summary>
        public readonly Dictionary<UInt64, Object> StructuredProperties;

        /// <summary>
        /// The unstructured properties
        /// </summary>
        public readonly Dictionary<String, Object> UnstructuredProperties;

        /// <summary>
        /// The definition of the outgoing edges
        /// </summary>
        public readonly Dictionary<UInt64, EdgeAddDefinition> OutgoingEdges;

        /// <summary>
        /// The binary properties
        /// </summary>
        public readonly Dictionary<UInt64, Stream> BinaryProperties;
    }
}
