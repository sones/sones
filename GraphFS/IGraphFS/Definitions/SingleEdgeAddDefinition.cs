using System;
namespace sones.GraphFS.Definitions
{
    /// <summary>
    /// This struct represents the filesystem definition for an edge
    /// </summary>
    public sealed class SingleEdgeAddDefinition
    {
        #region data

        /// <summary>
        /// The property id of the edge
        /// </summary>
        public readonly Int64 PropertyID;

        /// <summary>
        /// The graph element properties
        /// </summary>
        public readonly GraphElementInformation GraphElementInformation;

        /// <summary>
        /// The source vertex information
        /// </summary>
        public readonly VertexInformation SourceVertexInformation;

        /// <summary>
        /// The target vertex informantion
        /// </summary>
        public readonly VertexInformation TargetVertexInformation;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new single edge definition
        /// </summary>
        /// <param name="myPropertyID">The id of the edge property</param>
        /// <param name="myGraphElementInformation">The source vertex information</param>
        /// <param name="mySourceVertexInformation">The target vertex informantion</param>
        /// <param name="myTargetVertexInformation">The graph element properties</param>
        public SingleEdgeAddDefinition(
            Int64 myPropertyID,
            GraphElementInformation myGraphElementInformation,
            VertexInformation mySourceVertexInformation,
            VertexInformation myTargetVertexInformation)
        {
            PropertyID = myPropertyID;
            SourceVertexInformation = mySourceVertexInformation;
            TargetVertexInformation = myTargetVertexInformation;
            GraphElementInformation = myGraphElementInformation;
        }

        #endregion
    }
}