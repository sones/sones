using System.Collections.Generic;
using System;
namespace sones.Library.Commons.VertexStore.Definitions
{
    /// <summary>
    /// This class represents the filesystem update definition for a vertex
    /// </summary>
    public sealed class VertexUpdateDefinition
    {
    
        #region data

        /// <summary>
        /// The vertex type id
        /// </summary>
        public readonly Int64 VertexTypeId;

        /// <summary>
        /// The vertex id
        /// </summary>
        public readonly Int64 VertexID;

         /// <summary>
        /// A comment for the vertex
        /// </summary>
        public struct CommentType
        {
            public bool IsUpdated;
            public string Changed;
        }

        public CommentType Comment;

        /// <summary>
        /// The modification date of the vertex
        /// </summary>
        public struct  ModificationDateType
        {
            public bool IsUpdated;
            public long Changed;
        }

        public ModificationDateType ModificationDate;


        /// <summary>
        /// The structured properties
        /// </summary>
        public struct StructuredPropertiesType
        {
            public bool IsUpdated;
            public Dictionary<Int64, IComparable> Added;
            public Dictionary<Int64, IComparable> Changed;
            public IEnumerable<Int64> Deleted;
        }

        public StructuredPropertiesType StructuredProperties;


        /// <summary>
        /// The unstructured properties
        /// </summary>
        public struct UnstructuredPropertiesType
        {
            public bool IsUpdated;
            public Dictionary<String, Object> Added;
            public Dictionary<String, Object> Changed;
            public IEnumerable<String> Deleted;
        }

        public UnstructuredPropertiesType UnstructuredProperties;
        

        /// <summary>
        /// The binary properties
        /// </summary>
        public struct BinaryPropertiesType
        {
            public bool IsUpdated;
            public IEnumerable<StreamAddDefinition> Added;
            public IEnumerable<StreamAddDefinition> Changed;
            public IEnumerable<Int64> Deleted;
        }

        public BinaryPropertiesType BinaryProperties;


        /// <summary>
        /// The edition of the vertex
        /// </summary>
        public struct EditionType
        {
            public bool IsUpdated;
            public string Changed;
        }

        public EditionType Edition;

        /// <summary>
        /// The definition of the outgoing hyper edges
        /// </summary>
        public struct OutgoingHyperEdgesType
        {
            public bool IsUpdated;
            public IEnumerable<HyperEdgeAddDefinition> Added;
            public IEnumerable<HyperEdgeAddDefinition> Changed;
            public IEnumerable<Int64> Deleted;
        }

        public OutgoingHyperEdgesType OutgoingHyperEdges;

        /// <summary>
        /// The definition of the outgoing single edges
        /// </summary>
        public struct OutgoingSingleEdgesType
        {
            public bool IsUpdated;
            public IEnumerable<SingleEdgeAddDefinition> Added;
            public IEnumerable<SingleEdgeAddDefinition> Changed;
            public IEnumerable<Int64> Deleted;
        }

        public readonly OutgoingSingleEdgesType OutgoingSingleEdges;

        #endregion

        #region constructor
        
        /// <summary>
        /// The default constructor which takes the vertex id (type and id) of the vertex to update as argument.
        /// Use it by setting the items to update after the construction!
        /// </summary>
        private VertexUpdateDefinition(Int64 myVertexTypeId, Int64 myVertexId)
        {
            this.VertexTypeId = myVertexTypeId;
            this.VertexID = myVertexId;
        }

        /// <summary>
        /// The constructor which takes the vertex id (type and id) as argument by declaring which items will become updated after the
        /// construction.
        /// </summary>
        /// <param name="myVertexTypeId"></param>
        /// <param name="myVertexId"></param>
        /// <param name="myIsCommentUpdated"></param>
        /// <param name="myIsModificationDateUpdated"></param>
        /// <param name="myIsStructuredPropertiesUpdated"></param>
        /// <param name="myIsUnstructuredPropertiesUpdated"></param>
        /// <param name="myIsBinaryPropertiesUpdated"></param>
        /// <param name="myIsEditionUpdated"></param>
        /// <param name="myIsOutgoingSingleEdgesUpdated"></param>
        /// <param name="myIsOutgoingHyperEdgesUpdated"></param>
        public VertexUpdateDefinition(Int64 myVertexTypeId, Int64 myVertexId,
                                      bool myIsCommentUpdated = false,
                                      bool myIsModificationDateUpdated = false,
                                      bool myIsStructuredPropertiesUpdated = false,
                                      bool myIsUnstructuredPropertiesUpdated = false,
                                      bool myIsBinaryPropertiesUpdated = false,
                                      bool myIsEditionUpdated = false,
                                      bool myIsOutgoingSingleEdgesUpdated = false,
                                      bool myIsOutgoingHyperEdgesUpdated = false) : this(myVertexTypeId, myVertexId)
        {
            Comment.IsUpdated = myIsCommentUpdated;
            ModificationDate.IsUpdated = myIsModificationDateUpdated;
            StructuredProperties.IsUpdated = myIsStructuredPropertiesUpdated;
            UnstructuredProperties.IsUpdated = myIsUnstructuredPropertiesUpdated;
            BinaryProperties.IsUpdated = myIsBinaryPropertiesUpdated;
            Edition.IsUpdated = myIsEditionUpdated;
            OutgoingSingleEdges.IsUpdated = myIsOutgoingSingleEdgesUpdated;
            OutgoingHyperEdges.IsUpdated = myIsOutgoingHyperEdgesUpdated;
        } 
                                       

        
        #endregion
    }
}