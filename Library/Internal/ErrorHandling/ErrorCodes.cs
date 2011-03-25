using System;

namespace sones.Library.ErrorHandling
{
    /// <summary>
    /// Contains static ErrorCodes
    /// </summary>
    public static class ErrorCodes
    {
        #region General (prefix: 1)

        /// <summary>
        /// A totally unknown error
        /// </summary>
        public static UInt16 UnknownError = 100;

        #endregion

        #region IGraphFS (prefix: 2)

        /// <summary>
        /// An unknown GraphFS error
        /// </summary>
        public static UInt16 UnknownFSError = 200;

        /// <summary>
        /// A certain vertex does not exist
        /// </summary>
        public static UInt16 VertexDoesNotExist = 201;

        /// <summary>
        /// The desired vertex already exists
        /// </summary>
        public static UInt16 VertexAlreadyExist = 202;

        /// <summary>
        /// A certain structured property could not be found (on a vertex)
        /// </summary>
        public static UInt16 CouldNotFindStructuredVertexProperty = 210;

        /// <summary>
        /// A certain unstructured property could not be found (on a vertex)
        /// </summary>
        public static UInt16 CouldNotFindUnStructuredVertexProperty = 211;

        /// <summary>
        /// A certain structured property could not be found (on an edge)
        /// </summary>
        public static UInt16 CouldNotFindStructuredEdgeProperty = 212;

        /// <summary>
        /// A certain unstructured property could not be found (on an edge)
        /// </summary>
        public static UInt16 CouldNotFindUnStructuredEdgeProperty = 213;
        
        #endregion

        #region IGraphDB (prefix: 3)

        /// <summary>
        /// An unknown GraphDB error
        /// </summary>
        public static UInt16 UnknownDBError = 300;

        #endregion

        #region IGraphDS (prefix: 4)

        

        #endregion

        #region IGraphQL (prefix: 5)
        

        #region AttributeAssignmentErrors

        /// <summary>
        /// An unknown GraphQL error
        /// </summary>
        public static UInt16 UnknownQLError = 500;

        /// <summary>
        /// An assignment of a certain reference type with a list is not allowed
        /// </summary>
        public static UInt16 InvalidAssignOfSet = 501;

        /// <summary>
        /// An assignment for an attribute from a certain type with a value of a second type is not valid
        /// </summary>
        public static UInt16 InvalidAttrDefaultValueAssignment = 502;

        /// <summary>
        /// An reference assignment for undefined attributes is not allowed
        /// </summary>
        public static UInt16 InvalidReferenceAssignmentOfUndefAttr = 503;

        /// <summary>
        /// Could not assign the value of the undefined attribute to an defined attribute of a certain type 
        /// </summary>
        public static UInt16 InvalidUndefAttrType = 504;

        /// <summary>
        /// A single reference attribute does not contain any value
        /// </summary>
        public static UInt16 ReferenceAssignmentEmptyValue = 505;

        /// <summary>
        /// 
        /// </summary>
        public static UInt16 ReferenceAssignment = 506;

        /// <summary>
        /// A attribute expects a Reference assignment
        /// </summary>
        public static UInt16 ReferenceAssignmentExpected = 507;

        #endregion

        #region DumpErrors

        /// <summary>
        /// The dump format is invalid
        /// </summary>
        public static UInt16 InvalidDumpFormat = 508;        

        /// <summary>
        /// The desire dump type is not supported
        /// </summary>
        public static UInt16 InvalidDumpType = 509;

        /// <summary>
        /// The grammar is not dumpable
        /// </summary>
        public static UInt16 NotADumpableGrammar = 510;


        #endregion



        #endregion

        #region Library (prefix: 6)

        #region PluginManager

        /// <summary>
        /// An assembly file could not loaded
        /// </summary>
        public static UInt16 CouldNotLoadAssembly = 601;

        /// <summary>
        /// A plugin version is incompatible
        /// </summary>
        public static UInt16 IncompatiblePluginVersion = 602;

        #endregion


        #region PropertyHyperGraph 

        #endregion


        #region Security 

        #endregion


        #region Transaction 

        #endregion


        #endregion


        #region Plugins (prefix: 7)

        #region QGLAggregates 

        /// <summary>
        /// An unknown QGLAggregates error
        /// </summary>
        public static UInt16 UnknownAggregateError = 700;
        
        /// <summary>
        /// The aggregate does not match the group level
        /// </summary>
        public static UInt16 AggregateDoesNotMatchGroupLevel = 701;

        /// <summary>
        /// An aggregate is not valid on an attribute
        /// </summary>
        public static UInt16 AggregateIsNotValidOnThisAttribute = 702;

        /// <summary>
        /// An aggregate is not allowed in a context
        /// </summary>
        public static UInt16 AggregateNotAllowed = 703;

        /// <summary>
        /// An aggregate is on multi attributes not allowed
        /// </summary>
        public static UInt16 AggregateOnMultiAttributesNotAllowed = 704;

        /// <summary>
        /// An aggregate or function does not exist
        /// </summary>
        public static UInt16 AggregateOrFunctionDoesNotExist = 705;

        /// <summary>
        /// The number of parameters of the function does not match the definition
        /// </summary>
        public static UInt16 AggregateParameterCountMismatch = 706;

        /// <summary>
        /// A type is not implemented for aggregates
        /// </summary>
        public static UInt16 NotImplementedAggregateTarget = 707;                      

        #endregion

        

        #endregion

    }
}