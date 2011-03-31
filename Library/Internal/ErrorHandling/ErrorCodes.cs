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

        #region IncomingEdge (prefix: 31)

        /// <summary>
        /// The incoming edge destination is invalid
        /// </summary>
        public static UInt16 IncomingEdgeDestinationIsInvalid = 3101;

        /// <summary>
        /// Defining of incoming edge for not referenced attribute is not allowed
        /// </summary>
        public static UInt16 IncomingEdgeForNotReferenceAttributeTypes = 3102;

        #endregion

        #region IndexErrors (prefix: 32)

        /// <summary>
        /// The index already exists
        /// </summary>
        public static UInt16 IndexAlreadyExist = 3201;        

        /// <summary>
        /// The index does not exists
        /// </summary>
        public static UInt16 IndexDoesNotExist = 3202;

        /// <summary>
        /// The index type does not exists
        /// </summary>
        public static UInt16 IndexTypeDoesNotExist = 3203;

        #endregion
        
        #region VertexTypeErrors (prefix: 33)

        /// <summary>
        /// A base vertex type is not a user defined type 
        /// </summary>
        public static UInt16 InvalidBaseVertexType = 3301;

        /// <summary>
        /// The vertex type is invalid
        /// </summary>
        public static UInt16 InvalidVertexType = 3302;

        /// <summary>
        /// The user defined vertex type should not be used with LIST attributes
        /// </summary>
        public static UInt16 ListAttributeNotAllowed = 3303;

        /// <summary>
        /// The parent vertex type of a vertex type does not exist
        /// </summary>
        public static UInt16 ParentVertexTypeDoesNotExist = 3304;

        /// <summary>
        /// Truncate on an inherited vertex type is not allowed
        /// </summary>
        public static UInt16 TruncateNotAllowedOnInheritedType = 3305;
                
        /// <summary>
        /// The vertex type already exists
        /// </summary>
        public static UInt16 VertexTypeAlreadyExist = 3306;

        /// <summary>
        /// The vertex type does not exists
        /// </summary>
        public static UInt16 VertexTypeDoesNotExist = 3307;

        /// <summary>
        /// A vertex type does not match the expected type
        /// </summary>
        public static UInt16 VertexTypeDoesNotMatch = 3308;

        #endregion

        #region VertexTypeAttributeErrors (prefix: 34)

        /// <summary>
        /// Droping of derived vertex attribute on the child vertex type is not allowed
        /// </summary>
        public static UInt16 DropOfDerivedVertexAttributeIsNotAllowed = 3401;

        /// <summary>
        /// Droping a vertex attribute is not allowed, because of remaining references from other attributes
        /// </summary>
        public static UInt16 DropOfVertexAttributeNotAllowed = 3402;        

        /// <summary>
        /// The name of the attribute is not valid
        /// </summary>
        public static UInt16 InvalidVertexAttributeName = 3403;

        /// <summary>
        /// The selected vertex attribute is not valid
        /// </summary>
        public static UInt16 InvalidVertexAttributeSelection = 3404;

        /// <summary>
        /// Undefined attributes can not inserted nor updated without setting SETUNDEFBEHAVE 
        /// </summary>
        public static UInt16 UndefinedAttributes = 3405;

        /// <summary>
        /// The vertex attribute already exists in supertype
        /// </summary>
        public static UInt16 VertexAttributeExistsInSuperVertexType = 3406;

        #endregion

        #endregion

        #region GraphDS (prefix: 4)



        #endregion

        #region GraphQL (prefix: 5)

        /// <summary>
        /// An unknown GraphQL error
        /// </summary>
        public static UInt16 UnknownQLError = 5000;

        /// <summary>
        /// The datatype does not match the type
        /// </summary>
        public static UInt16 DataTypeDoesNotMatch = 5001;

        /// <summary>
        /// The type is already referenced
        /// </summary>
        public static UInt16 DuplicateReferenceOccurrence = 5002;

        /// <summary>
        /// The IDNode is not valid
        /// </summary>
        public static UInt16 InvalidIDNode = 5003;

        /// <summary>
        /// The tuple is not valid
        /// </summary>
        public static UInt16 InvalidTuple = 5004;

        /// <summary>
        /// Currently the type has not been implemented for expressions
        /// </summary>
        public static UInt16 NotImpementedExpressionNode = 5005;

        #region Aggregate (prefix: 51)

        /// <summary>
        /// An unknown QGLAggregate error
        /// </summary>
        public static UInt16 UnknownAggregateError = 5100;

        /// <summary>
        /// The aggregate does not match the group level
        /// </summary>
        public static UInt16 AggregateDoesNotMatchGroupLevel = 5101;

        /// <summary>
        /// An aggregate is not valid on an attribute
        /// </summary>
        public static UInt16 AggregateIsNotValidOnThisAttribute = 5102;

        /// <summary>
        /// An aggregate is not allowed in a context
        /// </summary>
        public static UInt16 AggregateNotAllowed = 5103;

        /// <summary>
        /// An aggregate is on multi attributes not allowed
        /// </summary>
        public static UInt16 AggregateOnMultiAttributesNotAllowed = 5104;

        /// <summary>
        /// The number of parameters of the function does not match the definition
        /// </summary>
        public static UInt16 AggregateParameterCountMismatch = 5105;

        /// <summary>
        /// A type is not implemented for aggregates
        /// </summary>
        public static UInt16 NotImplementedAggregateTarget = 5106;

        #endregion

        #region AttributeAssignmentErrors (prefix: 52)
                
        /// <summary>
        /// An assignment of a certain reference type with a list is not allowed
        /// </summary>
        public static UInt16 InvalidAssignOfSet = 5201;

        /// <summary>
        /// An assignment for an attribute from a certain type with a value of a second type is not valid
        /// </summary>
        public static UInt16 InvalidAttrDefaultValueAssignment = 5202;

        /// <summary>
        /// An reference assignment for undefined attributes is not allowed
        /// </summary>
        public static UInt16 InvalidReferenceAssignmentOfUndefAttr = 5203;

        /// <summary>
        /// Could not assign the value of the undefined attribute to an defined attribute of a certain type 
        /// </summary>
        public static UInt16 InvalidUndefAttrType = 5204;

        /// <summary>
        /// A single reference attribute does not contain any value
        /// </summary>
        public static UInt16 ReferenceAssignmentEmptyValue = 5205;

        /// <summary>
        /// 
        /// </summary>
        public static UInt16 ReferenceAssignment = 5206;

        /// <summary>
        /// A attribute expects a Reference assignment
        /// </summary>
        public static UInt16 ReferenceAssignmentExpected = 5207;

        #endregion        
        
        #region DumpErrors (prefix: 53)

        /// <summary>
        /// The dump format is invalid
        /// </summary>
        public static UInt16 InvalidDumpFormat = 5301;        

        /// <summary>
        /// The desire dump type is not supported
        /// </summary>
        public static UInt16 InvalidDumpType = 5302;

        /// <summary>
        /// The grammar is not dumpable
        /// </summary>
        public static UInt16 NotADumpableGrammar = 5303;

        #endregion

        #region EdgeErrors (prefix: 54)

        /// <summary>
        /// The number of parameters of an edge does not match
        /// </summary>
        public static UInt16 EdgeParameterCountMismatch = 5401;

        /// <summary>
        /// The type of the edge parameter does not match
        /// </summary>
        public static UInt16 EdgeParameterTypeMismatch = 5402;

        /// <summary>
        /// Too many elements for a type of an edge 
        /// </summary>
        public static UInt16 TooManyElementsForEdge = 5403;

        #endregion

        #region FunctionErrors (prefix: 55)

        /// <summary>
        /// A function does not exists
        /// </summary>
        public static UInt16 FunctionDoesNotExist = 5501;

        /// <summary>
        /// An invalid reference for a function parameter
        /// </summary>
        public static UInt16 FunctionParameterInvalidReference = 5502;

        /// <summary>
        /// The function has a invalid working base
        /// </summary>
        public static UInt16 InvalidFunctionBase = 5503;

        /// <summary>
        /// The return type of the function is invalid
        /// </summary>
        public static UInt16 InvalidFunctionReturnType = 5504;

        #endregion

        #region IndexErrors (prefix: 56)

        /// <summary>
        /// Could not alter the index on a type
        /// </summary>
        public static UInt16 CouldNotAlterIndexOnType = 5601;

        /// <summary>
        /// The index operation is invalid
        /// </summary>
        public static UInt16 InvalidIndexOperation = 5602;

        #endregion

        #region SelectErrors (prefix: 57)

        /// <summary>
        /// A duplicate attribute selection is not allowed
        /// </summary>
        public static UInt16 DuplicateAttributeSelection = 5701;

        /// <summary>
        /// A to group attribute is not selected
        /// </summary>
        public static UInt16 GroupedAttributeIsNotSelected = 5702;

        /// <summary>
        /// An invalid group level when adding a group element to a selection
        /// </summary>
        public static UInt16 InvalidGroupByLevel = 5703;

        /// <summary>
        /// The assignment of the select value is invalid
        /// </summary>
        public static UInt16 InvalidSelectValueAssignment = 5704;

        /// <summary>
        /// Missing grouped argument in a selection with aggregates 
        /// </summary>
        public static UInt16 NoGroupingArgument = 5705;

        /// <summary>
        /// The data type of the SelectValueAssignment does not match the type
        /// </summary>
        public static UInt16 SelectValueAssignmentDataTypeDoesNotMatch = 5706;

        /// <summary>
        /// The timeout of a query has been reached
        /// </summary>
        public static UInt16 SelectTimeOut = 5707;

        #endregion

        #region VertexTypeAttributeErrors (prefix: 58)

        /// <summary>
        /// The vertex attribute is ambiguous  
        /// </summary>
        public static UInt16 AmbiguousVertexAttribute = 5801;

        /// <summary>
        /// The undefined vertex attribute has a invalid name
        /// </summary>
        public static UInt16 InvalidUndefinedVertexAttributeName = 5802;

        /// <summary>
        /// The vertex does not contain an undefined attribute with this name
        /// </summary>
        public static UInt16 InvalidUndefinedVertexAttributes = 5803;

        /// <summary>
        /// The vertex attribute is not valid 
        /// </summary>
        public static UInt16 InvalidVertexAttribute = 5804;

        /// <summary>
        /// The given kind of attribute does not match
        /// </summary>
        public static UInt16 InvalidVertexAttributeKind = 5805;

        /// <summary>
        /// The attribute has an invalid value
        /// </summary>
        public static UInt16 InvalidVertexAttributeValue = 5806;

        /// <summary>
        /// The attribute from a type could not be removed
        /// </summary>
        public static UInt16 RemoveVertexTypeAttribute = 5807;

        /// <summary>
        /// Could not find any objects while updating elements to the list attribute
        /// </summary>
        public static UInt16 UpdateListVertexAttributeNoElements = 5808;

        /// <summary>
        /// Could not update a value for a vertex attribute
        /// </summary>
        public static UInt16 UpdateVertexAttributeValue = 5809;

        /// <summary>
        /// The vertex attribute already exists in the type
        /// </summary>
        public static UInt16 VertexAttributeAlreadyExists = 5810;

        /// <summary>
        /// The vertex attribute already exists in subtype
        /// </summary>
        public static UInt16 VertexAttributeExistsInSubtype = 5811;

        /// <summary>
        /// The attribute is not defined on this type
        /// </summary>
        public static UInt16 VertexAttributeIsNotDefined = 5812;

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