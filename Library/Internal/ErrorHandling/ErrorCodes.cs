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

        #region GraphFS (prefix: 2)

        /// <summary>
        /// An unknown GraphFS error
        /// </summary>
        public static UInt16 UnknownFSError = 2000;

        /// <summary>
        /// A certain vertex does not exist
        /// </summary>
        public static UInt16 VertexDoesNotExist = 2001;

        /// <summary>
        /// The desired vertex already exists
        /// </summary>
        public static UInt16 VertexAlreadyExist = 2002;

        /// <summary>
        /// A certain structured property could not be found (on a vertex)
        /// </summary>
        public static UInt16 CouldNotFindStructuredVertexProperty = 2003;

        /// <summary>
        /// A certain unstructured property could not be found (on a vertex)
        /// </summary>
        public static UInt16 CouldNotFindUnStructuredVertexProperty = 2004;

        /// <summary>
        /// A certain structured property could not be found (on an edge)
        /// </summary>
        public static UInt16 CouldNotFindStructuredEdgeProperty = 2005;

        /// <summary>
        /// A certain unstructured property could not be found (on an edge)
        /// </summary>
        public static UInt16 CouldNotFindUnStructuredEdgeProperty = 2006;
        
        #endregion

        #region GraphDB (prefix: 3)

        /// <summary>
        /// An unknown GraphDB error
        /// </summary>
        public static UInt16 UnknownDBError = 3000;

        /// <summary>
        /// An aggregate or function does not exist
        /// </summary>
        public static UInt16 AggregateOrFunctionDoesNotExist = 3001;

        /// <summary>
        /// An aggregate or function is duplicate
        /// </summary>
        public static UInt16 DuplicateAggregateOrFunction = 3002;

        /// <summary>
        /// An edge type does not exist
        /// </summary>
        public static UInt16 EdgeTypeDoesNotExist = 3003;

        /// <summary>
        /// 
        /// </summary>
        public static UInt16 IndexDoesNotExist = 3004;

        #endregion

        #region GraphDS (prefix: 4)

        

        #endregion

        #region GraphQL (prefix: 5)

        /// <summary>
        /// An unknown GraphQL error
        /// </summary>
        public static UInt16 UnknownQLError = 5000;


        #region AttributeAssignmentErrors (prefix: 51)
                
        /// <summary>
        /// An assignment of a certain reference type with a list is not allowed
        /// </summary>
        public static UInt16 InvalidAssignOfSet = 5101;

        /// <summary>
        /// An assignment for an attribute from a certain type with a value of a second type is not valid
        /// </summary>
        public static UInt16 InvalidAttrDefaultValueAssignment = 5102;

        /// <summary>
        /// An reference assignment for undefined attributes is not allowed
        /// </summary>
        public static UInt16 InvalidReferenceAssignmentOfUndefAttr = 5103;

        /// <summary>
        /// Could not assign the value of the undefined attribute to an defined attribute of a certain type 
        /// </summary>
        public static UInt16 InvalidUndefAttrType = 5104;

        /// <summary>
        /// A single reference attribute does not contain any value
        /// </summary>
        public static UInt16 ReferenceAssignmentEmptyValue = 5105;

        /// <summary>
        /// 
        /// </summary>
        public static UInt16 ReferenceAssignment = 5106;

        /// <summary>
        /// A attribute expects a Reference assignment
        /// </summary>
        public static UInt16 ReferenceAssignmentExpected = 5007;

        #endregion        
        
        #region DumpErrors (prefix: 52)

        /// <summary>
        /// The dump format is invalid
        /// </summary>
        public static UInt16 InvalidDumpFormat = 5201;        

        /// <summary>
        /// The desire dump type is not supported
        /// </summary>
        public static UInt16 InvalidDumpType = 5202;

        /// <summary>
        /// The grammar is not dumpable
        /// </summary>
        public static UInt16 NotADumpableGrammar = 5203;

        #endregion


        #region EdgeErrors (prefix: 53)

        /// <summary>
        /// The number of parameters of an edge does not match
        /// </summary>
        public static UInt16 EdgeParameterCountMismatch = 5301;

        /// <summary>
        /// The type of the edge parameter does not match
        /// </summary>
        public static UInt16 EdgeParameterTypeMismatch = 5302;

        /// <summary>
        /// Too many elements for a type of an edge 
        /// </summary>
        public static UInt16 TooManyElementsForEdge = 5303;

        #endregion

        #region FunctionErrors (prefix: 54)

        /// <summary>
        /// A function does not exists
        /// </summary>
        public static UInt16 FunctionDoesNotExist = 5401;

        /// <summary>
        /// An invalid reference for a function parameter
        /// </summary>
        public static UInt16 FunctionParameterInvalidReference = 5402;

        /// <summary>
        /// The function has a invalid working base
        /// </summary>
        public static UInt16 InvalidFunctionBase = 5403;

        /// <summary>
        /// The return type of the function is invalid
        /// </summary>
        public static UInt16 InvalidFunctionReturnType = 5404;

        #endregion


        #endregion

        #region Library (prefix: 6)

        #region PluginManager (prefix: 61)

        /// <summary>
        /// An assembly file could not loaded
        /// </summary>
        public static UInt16 CouldNotLoadAssembly = 6101;

        /// <summary>
        /// A plugin version is incompatible
        /// </summary>
        public static UInt16 IncompatiblePluginVersion = 6102;

        #endregion


        #region PropertyHyperGraph (prefix: 62)

        #endregion


        #region Security (prefix: 63)

        #endregion


        #region Transaction (prefix: 64)

        #endregion


        #endregion


        #region Plugins (prefix: 7)

        #region SonesQGLAggregate (prefix: 71)

        /// <summary>
        /// An unknown QGLAggregate error
        /// </summary>
        public static UInt16 UnknownAggregateError = 7100;
        
        /// <summary>
        /// The aggregate does not match the group level
        /// </summary>
        public static UInt16 AggregateDoesNotMatchGroupLevel = 7101;
                
        /// <summary>
        /// An aggregate is not valid on an attribute
        /// </summary>
        public static UInt16 AggregateIsNotValidOnThisAttribute = 7102;

        /// <summary>
        /// An aggregate is not allowed in a context
        /// </summary>
        public static UInt16 AggregateNotAllowed = 7103;

        /// <summary>
        /// An aggregate is on multi attributes not allowed
        /// </summary>
        public static UInt16 AggregateOnMultiAttributesNotAllowed = 7104;            

        /// <summary>
        /// The number of parameters of the function does not match the definition
        /// </summary>
        public static UInt16 AggregateParameterCountMismatch = 7105;

        /// <summary>
        /// A type is not implemented for aggregates
        /// </summary>
        public static UInt16 NotImplementedAggregateTarget = 7106;                      

        #endregion


        #region SonesQGLFunction (prefix: 72)
                
        /// <summary>
        /// An unknown QGLFunction error
        /// </summary>
        public static UInt16 UnknownFunctionError = 7100;
        
        /// <summary>
        /// The number of parameters of the function does not match the definition
        /// </summary>
        public static UInt16 FunctionParameterCountMismatch = 7201;

        /// <summary>
        /// The parameter value for this function is invalid
        /// </summary>
        public static UInt16 InvalidFunctionParameter = 7202;
        
        #endregion

        #region SonesIndex (prefix: 73)

        /// <summary>
        /// An unknown Index error
        /// </summary>
        public static UInt16 UnknownIndexError = 7300;
        
        /// <summary>
        /// 
        /// </summary>
        public static UInt16 UniqueIndexConstraintException = 7301;

        #endregion

        #endregion

    }
}