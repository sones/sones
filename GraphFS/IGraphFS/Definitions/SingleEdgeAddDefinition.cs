namespace sones.GraphFS.Definitions
{
    /// <summary>
    /// This struct represents the filesystem definition for an edge
    /// </summary>
    public struct SingleEdgeAddDefinition
    {
        #region data

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
        /// <param name="myGraphElementInformation">The source vertex information</param>
        /// <param name="mySourceVertexInformation">The target vertex informantion</param>
        /// <param name="myTargetVertexInformation">The graph element properties</param>
        public SingleEdgeAddDefinition(
            GraphElementInformation myGraphElementInformation,
            VertexInformation mySourceVertexInformation,
            VertexInformation myTargetVertexInformation)
        {
            SourceVertexInformation = mySourceVertexInformation;
            TargetVertexInformation = myTargetVertexInformation;
            GraphElementInformation = myGraphElementInformation;
        }

        #endregion
    }
}