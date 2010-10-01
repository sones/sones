/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
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
* 
*/

/*
 * LinkNode
 * (c) Dirk Bludau, 2009-2010
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;

using sones.GraphDB.GraphQL.StructureNodes;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.Structures.Enums;
using sones.GraphDB.Structures.Operators;


using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.Result;

#endregion

namespace sones.GraphDB.GraphQL.StatementNodes.Link
{

    public class LinkNode : AStatement
    {

        #region Data

        private TypeReferenceDefinition                     _SourceType;        
        private TupleDefinition                             _Sources;
        private HashSet<AAttributeAssignOrUpdateOrRemove>   _Targets;
        private BinaryExpressionDefinition                  _Condition;

        #endregion

        #region Constructors

        public LinkNode()
        { }

        #endregion

        #region AStatement Members

        #region StatementName

        public override String StatementName
        {
            get
            {
                return "Link";
            }
        }

        #endregion

        #region TypeOfStatement

        public override TypesOfStatements TypeOfStatement
        {
            get
            {
                return TypesOfStatements.ReadWrite;
            }
        }

        #endregion

        #region GetContent(myCompilerContext, myParseTreeNode)

        public override void GetContent(CompilerContext myCompilerContext, ParseTreeNode myParseTreeNode)
        {

            var _GraphQLGrammar = GetGraphQLGrammar(myCompilerContext);

            _Targets = new HashSet<AAttributeAssignOrUpdateOrRemove>();
            

            #region FROM Sources

            var typeNode = (myParseTreeNode.ChildNodes[1].AstNode as ATypeNode);
            ParsingResult.PushIExceptional(typeNode.ParsingResult);
            _SourceType = typeNode.ReferenceAndType;

            _Sources = (myParseTreeNode.ChildNodes[2].AstNode as TupleNode).TupleDefinition;
            
            if(_Sources.Count() > 1 )
            {
                var firstElement = (BinaryExpressionDefinition)_Sources.First().Value;
                _Sources.Remove(_Sources.First());
                _Condition = GetConditionNode(new OrOperator().ContraryOperationSymbol, firstElement, _Sources);
            }
            
            else
            {
                _Condition = (BinaryExpressionDefinition) _Sources.First().Value;
            }

            #endregion

            #region Find statement "VIA Edge"

            IDNode _EdgeAttr = null;

            // Semantic Web Yoda-style...
            if (myParseTreeNode.ChildNodes[3].Token.AsSymbol == _GraphQLGrammar.S_VIA)
            {
                _EdgeAttr = (myParseTreeNode.ChildNodes[4].AstNode as IDNode);
            }

            else // Human language style...
            {
                if (myParseTreeNode.ChildNodes[5].Token.AsSymbol == _GraphQLGrammar.S_VIA)
                {
                    _EdgeAttr = (myParseTreeNode.ChildNodes[6].AstNode as IDNode);
                }
            }

            #endregion

            #region Find statement "TO Targets"

            TupleDefinition tupleDef = null;

            // Semantic Web Yoda-style...
            if (myParseTreeNode.ChildNodes[5].Token.AsSymbol == _GraphQLGrammar.S_TO)
            {
                typeNode = (myParseTreeNode.ChildNodes[6].ChildNodes[0].AstNode as ATypeNode);
                tupleDef = (myParseTreeNode.ChildNodes[6].ChildNodes[1].AstNode as TupleNode).TupleDefinition;
            }

            else // Human language style...
            {
                if (myParseTreeNode.ChildNodes[3].Token.AsSymbol == _GraphQLGrammar.S_TO)
                {
                    typeNode = (myParseTreeNode.ChildNodes[4].ChildNodes[0].AstNode as ATypeNode);
                    tupleDef = (myParseTreeNode.ChildNodes[4].ChildNodes[1].AstNode as TupleNode).TupleDefinition;
                }
            }

            #endregion

            #region Processing...

            ParsingResult.PushIExceptional(typeNode.ParsingResult);

            var _TargetType        = typeNode.ReferenceAndType;
            var tupleDefTargetType = new TupleDefinition(tupleDef.KindOfTuple);            

            foreach (var item in tupleDef.TupleElements)
            {

                var attrName = _TargetType.TypeName + DBConstants.EdgeTraversalDelimiterSymbol + ((IDChainDefinition)((BinaryExpressionDefinition)item.Value).Left).ContentString;
                var leftNode = new IDChainDefinition(attrName, new List<TypeReferenceDefinition>() { _TargetType });
                leftNode.AddPart(new ChainPartTypeOrAttributeDefinition(((IDChainDefinition)((BinaryExpressionDefinition)item.Value).Left).ContentString));
                var rightNode = ((BinaryExpressionDefinition)item.Value).Right;

                var binExpression = new BinaryExpressionDefinition(((BinaryExpressionDefinition)item.Value).OperatorSymbol, leftNode, rightNode);

                tupleDefTargetType.AddElement(new TupleElement(item.TypeOfValue, binExpression));

            }

            _Targets.Add(new AttributeAssignOrUpdateList(new CollectionDefinition(CollectionType.Set, tupleDefTargetType), _EdgeAttr.IDChainDefinition, false));

            #endregion
        
        }

        #endregion

        #region Execute(myIGraphDBSession)

        public override QueryResult Execute(IGraphDBSession myIGraphDBSession)
        {

            var result = myIGraphDBSession.Update(_SourceType.TypeName, _Targets, _Condition);
            result.PushIExceptional(ParsingResult);

            return result;

        }

        #endregion

        #endregion

        #region Private Helpers

        private BinaryExpressionDefinition GetConditionNode(String myOperator, BinaryExpressionDefinition myPrevNode, TupleDefinition myNodeList)
        {

            var binElem = myNodeList.FirstOrDefault();

            if (binElem == null)
            {
                return myPrevNode;
            }

            myNodeList.Remove(binElem);

            var binNode = new BinaryExpressionDefinition(myOperator, myPrevNode, binElem.Value);

            return GetConditionNode(myOperator, binNode, myNodeList);

        }

        #endregion
    
    }

}
