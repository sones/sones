using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphQL.GQL.Structure.Nodes.Expressions;
using sones.GraphQL.GQL.Structure.Nodes.Misc;
using sones.GraphQL.Structure.Helper.Enums;

namespace sones.GraphQL.GQL.Structure.Helper.Definition
{
    /// <summary>
    /// A definition for setReference
    /// </summary>
    public sealed class SetRefDefinition
    {

        #region Fields

        public Boolean IsREFUUID;

        #endregion

        #region Properties

        public Object[] Parameters { get; private set; }
        public TupleDefinition TupleDefinition { get; private set; }

        #endregion

        #region Ctor

        public SetRefDefinition(TupleDefinition tupleDefinition, Object[] myParameters)
        {

            System.Diagnostics.Debug.Assert(tupleDefinition != null);

            this.TupleDefinition = tupleDefinition;
            this.Parameters = myParameters;
        }

        public SetRefDefinition(TupleDefinition tupleDefinition, bool myIsREFUUID, Object[] parameters)
        {
            TupleDefinition = tupleDefinition;
            IsREFUUID = myIsREFUUID;
            Parameters = parameters;
        }

        #endregion

        #region private methods

        /// <summary>
        /// fast check if a given binary expression node is suitable for list integration
        /// </summary>
        /// <param name="aUniqueExpr">A BinaryExpressionNode.</param>
        /// <param name="myAttributes"></param>
        /// <returns></returns>
        private bool IsValidBinaryExpressionNode(BinaryExpressionDefinition aUniqueExpr, Dictionary<string, string> attributes)
        {
            switch (aUniqueExpr.TypeOfBinaryExpression)
            {
                case TypesOfBinaryExpression.LeftComplex:

                    #region left complex

                    return CheckIDNode(aUniqueExpr.Left, attributes);

                    #endregion

                case TypesOfBinaryExpression.RightComplex:

                    #region right complex

                    return CheckIDNode(aUniqueExpr.Right, attributes);

                    #endregion


                case TypesOfBinaryExpression.Complex:

                    #region complex

                    #region Data

                    BinaryExpressionDefinition leftNode = null;
                    BinaryExpressionDefinition rightNode = null;

                    #endregion

                    #region get expr

                    #region left

                    if (aUniqueExpr.Left is BinaryExpressionDefinition)
                    {
                        leftNode = (BinaryExpressionDefinition)aUniqueExpr.Left;
                    }
                    else
                    {
                        return false;
                    }

                    #endregion

                    #region right

                    if (aUniqueExpr.Right is BinaryExpressionDefinition)
                    {
                        rightNode = (BinaryExpressionDefinition)aUniqueExpr.Right;
                    }
                    else
                    {
                        return false;
                    }

                    #endregion

                    #endregion

                    #region check

                    if ((leftNode != null) && (rightNode != null))
                    {
                        if (IsValidBinaryExpressionNode(leftNode, attributes) && IsValidBinaryExpressionNode(rightNode, attributes))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }

                    #endregion

                    #endregion

                #region error cases

                case TypesOfBinaryExpression.Atom:

                default:

                    //in this kind of node it is not allowed to use Atom or complex expressions
                    return false;

                #endregion

            }

        }

        private bool CheckIDNode(AExpressionDefinition myPotentialIDNode, Dictionary<string, string> attributes)
        {
            if (myPotentialIDNode is IDChainDefinition)
            {
                var chainDefinition = (myPotentialIDNode as IDChainDefinition);
                if (chainDefinition.Level == 0)
                {
                    if (attributes.ContainsKey(chainDefinition.LastAttribute.Name))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            else
            {
                return false;
            }
        }

        #endregion

    }
}
