using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphDSRemoteClient.sonesGraphDSRemoteAPI;

namespace GraphDSRemoteClient.GraphElements
{
    public abstract class ARemoteGraphElement
    {
        #region data

        /// <summary>
        /// A comment for the vertex
        /// </summary>
        private string _comment;

        /// <summary>
        /// The creation date of the vertex
        /// </summary>
        private long _creationDate;

        /// <summary>
        /// The modification date of the vertex
        /// </summary>
        private long _modificationDate;

        /// <summary>
        /// The structured properties
        /// </summary>
        private IDictionary<Int64, IComparable> _structuredProperties;

        /// <summary>
        /// The unstructured properties
        /// </summary>
        private IDictionary<String, Object> _unstructuredProperties;

        #endregion


        #region Getter / Setter

        public String Comment { get; }

        public long CreationDate { get; }

        public long ModificationDate { get; }

        public IDictionary<Int64, IComparable> StructuredProperties { get; }

        public IDictionary<String, Object> UnstructuredProperties { get; }

        #endregion
    }
}
