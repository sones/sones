using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Expression;
using sones.GraphQL.Structure.Helper.Enums;
using sones.GraphQL.GQL.Structure.Helper.ExpressionGraph;
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphQL.GQL.Manager.Plugin;

namespace sones.GraphQL.GQL.Structure.Nodes.Expressions
{
    /// <summary>
    /// This abstract class is responsible for operators
    /// which are triggered by binary expression nodes.
    /// </summary>
    public abstract class ABinaryOperator
    {

        #region General Operator infos

        public abstract String[] Symbol { get; }
        public abstract String ContraryOperationSymbol { get; }
        public abstract BinaryOperator Label { get; }
        public abstract TypesOfOperators Type { get; }

        #endregion

        //#region (public) Methods

        //public abstract IExpressionGraph TypeOperation(
        //    AExpressionDefinition myLeftValueObject, 
        //    AExpressionDefinition myRightValueObject, 
        //    GQLPluginManager myPluginManager,
        //    IGraphDB myIGraphDB,
        //    SecurityToken mySecurityToken,
        //    TransactionToken myTransactionToken,
        //    TypesOfBinaryExpression typeOfBinExpr, 
        //    TypesOfAssociativity associativity, 
        //    IExpressionGraph result, 
        //    Boolean aggregateAllowed = true);

        //#endregion

        public List<String> GetAttributeList(List<String> aList)
        {

            if (aList.Count == 1)
            {
                return aList;
            }
            else
            {
                return aList.GetRange(1, aList.Count - 1);
            }

        }

        #region Get tuple based on the operator (InRange allows other tuples than + or = ...)

        public virtual AOperationDefinition GetValidTupleReloaded(TupleDefinition myTupleDefinition,
            IGraphDB myIGraphDB,
            SecurityToken mySecurityToken,
            TransactionToken myTransactionToken)
        {
            switch (myTupleDefinition.KindOfTuple)
            {
                case KindOfTuple.Exclusive:

                    if (myTupleDefinition.TupleElements.Count == 1)
                    {
                        return myTupleDefinition.TupleElements[0].Value as ValueDefinition;
                    }
                    else
                    {
                        return myTupleDefinition;
                    }

                default:

                    return myTupleDefinition;

            }
        }

        protected AOperationDefinition CreateTupleValue(TupleDefinition myTupleDefinition)
        {
            return myTupleDefinition;
        }

        #endregion

    }
}
