/*
 * UnlinkNode
 * (c) Dirk Bludau, 2009-2010
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Structures.Enums;
using sones.Lib.Frameworks.Irony.Parsing;

using sones.GraphDB.Managers.Structures;
using sones.GraphDB.GraphQL.StructureNodes;
using sones.GraphDB.Structures.Operators;
using sones.GraphDB.Result;

#endregion

namespace sones.GraphDB.GraphQL.StatementNodes.Unlink
{
    public class UnlinkNode : AStatement
    {

        #region data

        private TypeReferenceDefinition                     _SourceType;
        private TupleDefinition                             _Targets;
        private HashSet<AAttributeAssignOrUpdateOrRemove>   _Sources;
        private BinaryExpressionDefinition                  _Condition;

        #endregion

        #region constructors

        public UnlinkNode()
        { }
        
        #endregion

        #region AStatement        

        public override string StatementName
        {
            get { return "Unlink"; }
        }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.ReadWrite; }
        }

        public override void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            _Sources = new HashSet<AAttributeAssignOrUpdateOrRemove>();

            #region VIA edge

            IDNode _EdgeAttr = null;

            if (parseNode.ChildNodes[4].AstNode is IDNode) //Semantic Web Yoda style
            {
                _EdgeAttr = (parseNode.ChildNodes[4].AstNode as IDNode);
            }
            else //Human language style
            { 
                _EdgeAttr = (parseNode.ChildNodes[6].AstNode as IDNode);
            }

            #endregion

            #region sources
            
            var typeNode = (parseNode.ChildNodes[1].AstNode as ATypeNode);
            ParsingResult.PushIExceptional(typeNode.ParsingResult);

            var tupleDef = (parseNode.ChildNodes[2].AstNode as TupleNode).TupleDefinition;

            var tupleDefSourceType = new TupleDefinition(tupleDef.KindOfTuple);

            foreach (var item in tupleDef.TupleElements)
            {
                var attrName = typeNode.ReferenceAndType.TypeName + DBConstants.EdgeTraversalDelimiterSymbol + ((IDChainDefinition)((BinaryExpressionDefinition)item.Value).Left).ContentString;
                var leftNode = new IDChainDefinition(attrName, new List<TypeReferenceDefinition>() { typeNode.ReferenceAndType });
                leftNode.AddPart(new ChainPartTypeOrAttributeDefinition(((IDChainDefinition)((BinaryExpressionDefinition)item.Value).Left).ContentString));
                var rightNode = ((BinaryExpressionDefinition)item.Value).Right;

                var binExpression = new BinaryExpressionDefinition(((BinaryExpressionDefinition)item.Value).OperatorSymbol, leftNode, rightNode);

                tupleDefSourceType.AddElement(new TupleElement(item.TypeOfValue, binExpression));
            }

            _Sources.Add(new AttributeRemoveList(_EdgeAttr.IDChainDefinition, _EdgeAttr.IDChainDefinition.TypeName, tupleDefSourceType));

            #endregion

            #region sources FROM

            if (parseNode.ChildNodes[6].ChildNodes[0].AstNode is ATypeNode)  //Semantic Web Yoda style
            {
                typeNode = (parseNode.ChildNodes[6].ChildNodes[0].AstNode as ATypeNode);
                _Targets = (parseNode.ChildNodes[6].ChildNodes[1].AstNode as TupleNode).TupleDefinition;
            }
            else  //Human language style
            {
                typeNode = (parseNode.ChildNodes[4].ChildNodes[0].AstNode as ATypeNode);
                _Targets = (parseNode.ChildNodes[4].ChildNodes[1].AstNode as TupleNode).TupleDefinition;
            }

            ParsingResult.PushIExceptional(typeNode.ParsingResult);
            _SourceType = typeNode.ReferenceAndType;
            
            if (_Targets.Count() > 1)
            {
                var firstElement = (BinaryExpressionDefinition)_Targets.First().Value;
                _Targets.Remove(_Targets.First());

                _Condition = GetConditionNode(new OrOperator().ContraryOperationSymbol, firstElement, _Targets);
            }
            else
            {
                _Condition = (BinaryExpressionDefinition)_Targets.First().Value;
            }

            #endregion
        }       

        public override QueryResult Execute(IGraphDBSession graphDBSession)
        {
            var result = graphDBSession.Update(_SourceType.TypeName, _Sources, _Condition);
            result.PushIExceptional(ParsingResult);

            return result;
        }

        #endregion

        #region private helpers

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
