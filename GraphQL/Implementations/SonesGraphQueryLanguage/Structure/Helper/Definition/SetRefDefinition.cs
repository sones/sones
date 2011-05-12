/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

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
        public String ReferencedVertexType;

        #endregion

        #region Properties

        public Dictionary<string, object> Parameters { get; private set; }
        public TupleDefinition TupleDefinition { get; private set; }

        #endregion

        #region Ctor

        public SetRefDefinition(TupleDefinition tupleDefinition, Dictionary<String, object> myParameters)
        {

            System.Diagnostics.Debug.Assert(tupleDefinition != null);

            this.TupleDefinition = tupleDefinition;
            this.Parameters = myParameters;
            ReferencedVertexType = null;
        }

        public SetRefDefinition(TupleDefinition tupleDefinition, bool myIsREFUUID, String myReferencedVertexType, Dictionary<String, object> parameters)
        {
            TupleDefinition = tupleDefinition;
            IsREFUUID = myIsREFUUID;
            Parameters = parameters;
            ReferencedVertexType = myReferencedVertexType;
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
