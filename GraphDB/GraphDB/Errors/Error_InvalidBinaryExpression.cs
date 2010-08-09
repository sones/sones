/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
*
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using sones.GraphDB.Structures.Operators;
using sones.GraphDB.Structures.Enums;
using sones.GraphDB.Managers.Structures;

namespace sones.GraphDB.Errors
{
    public class Error_InvalidBinaryExpression : GraphDBError
    {
        public BinaryExpressionDefinition BinaryExpression { get; private set; }

        public ABinaryCompareOperator Operator { get; private set; }
        public Tuple<IDChainDefinition, IDChainDefinition> IDChainDefinitions { get; private set; }
        public Tuple<AExpressionDefinition, AExpressionDefinition> Operands { get; private set; }
        public TypesOfBinaryExpression TypeOfBinaryOperation { get; private set; }

        public Error_InvalidBinaryExpression(BinaryExpressionDefinition myBinaryExpression)
        {
            BinaryExpression = myBinaryExpression;
        }

        public Error_InvalidBinaryExpression(ABinaryCompareOperator myOperator, Tuple<IDChainDefinition, IDChainDefinition> myIDChainDefinitions, Tuple<AExpressionDefinition, AExpressionDefinition> myOperands, TypesOfBinaryExpression myTypeOfBinaryOperation)
        {
            Operator = myOperator;
            IDChainDefinitions = myIDChainDefinitions;
            Operands = myOperands;
            TypeOfBinaryOperation = myTypeOfBinaryOperation;

            BinaryExpression = null;
        }

        public override string ToString()
        {
            if (BinaryExpression != null)
            {
                return String.Format("The BinaryExpression is not valid: {0} {1} {2}", BinaryExpression.Left.ToString(), BinaryExpression.Operator.Symbol.First(), BinaryExpression.Right.ToString());
            }
            else
            {
                String binexpr;

                switch (TypeOfBinaryOperation)
                {
                    case TypesOfBinaryExpression.Atom:
                        binexpr = String.Format("Left: {0}, Operator: {1}, Right: {2}", Operands.Item1.ToString(), Operator.Symbol.ToString(), Operands.Item2.ToString());
                        break;

                    case TypesOfBinaryExpression.LeftComplex:
                    case TypesOfBinaryExpression.RightComplex:
                        binexpr = String.Format("Left: {0}, Operator: {1}, Right: {2}", IDChainDefinitions.Item1, Operator.Symbol, Operands.Item1);
                        break;

                    case TypesOfBinaryExpression.Complex:
                        binexpr = String.Format("Left: {0}, Operator: {1}, Right: {2}", IDChainDefinitions.Item1.ToString(), Operator.Symbol.ToString(), IDChainDefinitions.Item2.ToString());
                        break;

                    case TypesOfBinaryExpression.Unknown:
                    default:
                        binexpr = "Unknown";
                        break;
                }


                return String.Format("The following binary expression is not valid: {0}", binexpr);
            }
        }
    }
}
