using System.Collections.Generic;
using sones.GraphDB.ErrorHandling.Expression;
using sones.GraphDB.Expression;
using sones.GraphDB.Manager;
using sones.Library.PropertyHyperGraph;
using sones.Library.Security;
using sones.Library.Transaction;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// This class is responsible for realizing a get vertices on the database
    /// </summary>
    public sealed class PipelineableGetVerticesRequest : APipelinableRequest
    {
        #region Data

        /// <summary>
        /// The request that contains the todo
        /// </summary>
        private readonly RequestGetVertices _request;

        /// <summary>
        /// The parentVertex that have been fetched by the Graphdb
        /// it is used for generating the output
        /// </summary>
        private IEnumerable<IVertex> _fetchedIVertices;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new pipelineable get vertices request
        /// </summary>
        /// <param name="myGetVerticesRequest">The get vertices type request</param>
        /// <param name="mySecurity">The security token of the request initiator</param>
        /// <param name="myTransactionToken">The myOutgoingEdgeVertex transaction token</param>
        public PipelineableGetVerticesRequest(
                                                RequestGetVertices myGetVerticesRequest, 
                                                SecurityToken mySecurity,
                                                TransactionToken myTransactionToken)
            : base(mySecurity, myTransactionToken)
        {
            _request = myGetVerticesRequest;
        }

        #endregion

        #region APipelinableRequest Members

        public override void Validate(MetaManager myMetaManager)
        {
            if (!IsValidExpression(_request.GetVerticesDefinition.Expression))
                throw new InvalidExpressionException(_request.GetVerticesDefinition.Expression);
        }

        public override void Execute(MetaManager myMetaManager)
        {
            _fetchedIVertices = myMetaManager.VertexManager.GetVertices(_request.GetVerticesDefinition.Expression, _request.GetVerticesDefinition.IsLongrunning, TransactionToken, SecurityToken, myMetaManager);
        }

        public override IRequest GetRequest()
        {
            return _request;
        }

        #endregion

        #region internal methods

        /// <summary>
        /// Creates the output for a get vertices request
        /// </summary>
        /// <typeparam name="TResult">The type of the myResult</typeparam>
        /// <param name="myOutputconverter">The delegate that is executed uppon output-generation</param>
        /// <returns>A TResult</returns>
        internal TResult GenerateRequestResult<TResult>(Converter.GetVerticesResultConverter<TResult> myOutputconverter)
        {
            return myOutputconverter(Statistics, _fetchedIVertices);
        }

        #endregion

        #region private helper

        #region IsValidExpression

        /// <summary>
        /// Is the expression valid
        /// </summary>
        /// <param name="myExpression">The to be validated expression</param>
        /// <returns>True or false</returns>
        private bool IsValidExpression(IExpression myExpression)
        {
            switch (myExpression.TypeOfExpression)
            {
                case TypeOfExpression.Binary:

                    return IsValidBinaryExpression((BinaryExpression)myExpression);

                case TypeOfExpression.Unary:

                    return IsValidUnaryExpression((UnaryExpression)myExpression);

                case TypeOfExpression.Constant:
                case TypeOfExpression.Property:
                default:
                    return false;
            }
        }

        #endregion

        #region IsValidUnaryExpression

        /// <summary>
        /// Is the unary expression valid
        /// </summary>
        /// <param name="unaryExpression">The to be validated expression</param>
        /// <returns>True or false</returns>
        private bool IsValidUnaryExpression(UnaryExpression unaryExpression)
        {
            return IsValidExpression(unaryExpression.Expression);
        }

        #endregion

        #region IsValidBinaryExpression

        /// <summary>
        /// Is this binary expression valid
        /// </summary>
        /// <param name="binaryExpression">The to be validated binary expression</param>
        /// <returns>True or false</returns>
        private bool IsValidBinaryExpression(BinaryExpression binaryExpression)
        {
            switch (binaryExpression.Operator)
            {
                #region comparative

                case BinaryOperator.Equals:
                case BinaryOperator.GreaterOrEqualsThan:
                case BinaryOperator.GreaterThan:
                case BinaryOperator.In:
                case BinaryOperator.InRange:
                case BinaryOperator.LessOrEqualsThan:
                case BinaryOperator.LessThan:
                case BinaryOperator.NotEquals:
                case BinaryOperator.NotIn:

                    if (binaryExpression.Left is PropertyExpression)
                    {
                        return (binaryExpression.Right is PropertyExpression) || (binaryExpression.Right is ConstantExpression);
                    }
                    else
                    {
                        return binaryExpression.Left is ConstantExpression && binaryExpression.Right is PropertyExpression;    
                    }

                #endregion

                #region logic

                case BinaryOperator.AND:
                case BinaryOperator.OR:

                    return IsValidExpression(binaryExpression.Left) && IsValidExpression(binaryExpression.Right);

                #endregion

                default:
                    break;
            }

            return false;
        }

        #endregion

        #endregion
    }
}