/*
 * TupleDefinition
 * (c) Stefan Licht, 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Structures.Operators;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.Structures.Enums;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.Structures.Result;
using sones.Lib.ErrorHandling;
using sones.GraphDB.Errors;
using sones.GraphFS.DataStructures;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Structures.ExpressionGraph;
using System.Diagnostics;
using sones.GraphDB.Managers.Select;

#endregion

namespace sones.GraphDB.Managers.Structures
{
    /// <summary>
    /// Refactor!!!! Remove TupleValue!!!!
    /// </summary>
    public class TupleDefinition : AOperationDefinition, IEnumerable<TupleElement>
    {

        #region Properties

        public KindOfTuple KindOfTuple { get; private set; }
        public List<TupleElement> TupleElements { get; private set; }
        public TypesOfOperatorResult TypeOfOperatorResult { get; private set; }

        #endregion

        #region Ctors

        public TupleDefinition(KindOfTuple kindOfTuple = KindOfTuple.Inclusive)
        {
            TupleElements = new List<TupleElement>();
            TypeOfOperatorResult = TypesOfOperatorResult.Unknown;
            KindOfTuple = kindOfTuple;
        }

        public TupleDefinition(TypesOfOperatorResult myTypesOfOperatorResult, AObject myObject, GraphDBType myPandoraType, KindOfTuple kindOfTuple = KindOfTuple.Inclusive)
            : this()
        {
            TypeOfOperatorResult = myTypesOfOperatorResult;
            KindOfTuple = kindOfTuple;

            if (myObject is AListBaseEdgeType)
            {
                foreach (var obj in (AListBaseEdgeType)myObject)
                {
                    TupleElements.Add(new TupleElement(new ValueDefinition(obj as sones.GraphDB.TypeManagement.BasicTypes.ADBBaseObject)));
                }
            }
            else
            {
                throw new NotImplementedException(myObject.GetType().ToString());
            }
        }

        #endregion

        public void AddElement(TupleElement myTupleElement)
        {
            TupleElements.Add(myTupleElement);
        }

        #region IEnumerable<TupleElement> Members

        public IEnumerator<TupleElement> GetEnumerator()
        {
            return TupleElements.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Remove

        public void Remove(TupleElement left)
        {
            TupleElements.Remove(left);
        }

        public void Remove(ValueDefinition myValueDefinition)
        {
            TupleElements.Remove(new TupleElement(myValueDefinition));
        }

        public void Remove(TupleDefinition myTupleDefinition)
        {
            if (myTupleDefinition == this)
            {
                TupleElements = new List<TupleElement>();
            }
            else
            {
                foreach (var td in myTupleDefinition)
                {
                    Remove(td);
                }
            }
        }

        #endregion

        internal Exceptional ConvertToAttributeType(TypeAttribute typeAttribute, DBContext myDBContext)
        {

            var newTuple = new List<TupleElement>();

            for (int i = 0; i < TupleElements.Count; i++)
            {
                var tupleEl = TupleElements[i].Value;

                if (tupleEl is SelectDefinition)
                {

                    #region partial select

                    var selectManager = new SelectManager();
                    var qresult = selectManager.ExecuteSelect(myDBContext, (tupleEl as SelectDefinition));
                    if (qresult.Failed)
                    {
                        return new Exceptional(qresult.Errors);
                    }

                    foreach (SelectionResultSet aSelResult in qresult.Results)
                    {
                        //Hack:
                        int lowestSelectedLevel = (from aSelectionForReference in aSelResult.SelectedAttributes select aSelectionForReference.Key).Min();

                        String attrName = aSelResult.SelectedAttributes[lowestSelectedLevel].First().Value;
                        TypeAttribute curAttr = aSelResult.Type.GetTypeAttributeByName(attrName);

                        var dbTypeOfAttribute = curAttr.GetDBType(myDBContext.DBTypeManager);

                        var aTypeOfOperatorResult = GraphDBTypeMapper.ConvertPandora2CSharp(dbTypeOfAttribute.Name);

                        foreach (DBObjectReadout dbo in aSelResult.Objects)
                        {
                            if (!(dbo.Attributes.ContainsKey(attrName)))
                                continue;

                            if (curAttr != null)
                            {
                                var val = new ValueDefinition(aTypeOfOperatorResult, dbo.Attributes[attrName]);
                                newTuple.Add(new TupleElement(aTypeOfOperatorResult, val));
                            }
                            else
                            {
                                throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                            }
                        }
                    }

                    #endregion

                }
                else if (TupleElements[i].Value is ValueDefinition)
                {
                    newTuple.Add(new TupleElement(new ValueDefinition(GraphDBTypeMapper.GetADBBaseObjectFromUUID(typeAttribute.DBTypeUUID, (TupleElements[i].Value as ValueDefinition).Value.Value))));
                }
                else
                {
                    return new Exceptional(new Error_InvalidTuple(TupleElements[i].Value.GetType().Name));
                }

            }

            TupleElements = newTuple;

            return Exceptional.OK;
        }

        internal Exceptional<ASingleReferenceEdgeType> GetAsUUIDSingleEdge(DBContext dbContext, TypeAttribute attr)
        {
            var edge = attr.EdgeType.GetNewInstance() as ASingleReferenceEdgeType;
            if (TupleElements.Count > 1)
            {
                return new Exceptional<ASingleReferenceEdgeType>(new Error_TooManyElementsForEdge(edge, (UInt64)TupleElements.Count));
            }

            var aTupleElement = TupleElements.First();

            if (aTupleElement.Value is BinaryExpressionDefinition)
            {
                return new Exceptional<ASingleReferenceEdgeType>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
            }
            else
            {
                edge.Set(ObjectUUID.FromString(aTupleElement.Value.ToString()), attr.DBTypeUUID);
            }

            return new Exceptional<ASingleReferenceEdgeType>(edge);
        }

        internal Exceptional<ASetReferenceEdgeType> GetAsUUIDEdge(DBContext dbContext, TypeAttribute attr)
        {
            var edge = attr.EdgeType.GetNewInstance() as ASetReferenceEdgeType;

            foreach (TupleElement aTupleElement in TupleElements)
            {

                if (!(aTupleElement.Value is ValueDefinition))
                {
                    return new Exceptional<ASetReferenceEdgeType>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                }
                else
                {

                    edge.Add(ObjectUUID.FromString((aTupleElement.Value as ValueDefinition).Value.ToString()), attr.DBTypeUUID);

                }

            }

            return new Exceptional<ASetReferenceEdgeType>(edge);
        }

        /// <summary>
        /// Checks if there is a valid tuple node. Valid tuple nodes in this case look like : (Name = 'Henning', Age = 10)
        /// </summary>
        /// <param name="tupleElementList">List of tuple elements</param>
        /// <param name="myAttributes">myAttributes of the type</param>
        /// <returns>True if valid or otherwise false</returns>
        internal bool IsValidTupleNode(List<TupleElement> tupleElementList, GraphDBType myPandoraType, DBContext dbContext)
        {
            foreach (TupleElement aTupleElement in tupleElementList)
            {
                if (aTupleElement.Value is BinaryExpressionDefinition)
                {
                    var validateResult = ((BinaryExpressionDefinition)aTupleElement.Value).Validate(dbContext, myPandoraType);
                    if (validateResult.Failed)
                    {
                        throw new GraphDBException(validateResult.Errors);
                    }
                    //if (!IsValidBinaryExpressionNode((BinaryExpressionDefinition)aTupleElement.Value, myPandoraType))
                    //{
                    //    return false;
                    //}
                }
                else
                {
                    return false;
                }
            }

            return true;

        }

        /// <summary>
        /// returns a list of guids which match the tupleNode of the ListOfDBObjects object.
        /// </summary>
        /// <param name="TypeOfAttribute">PandoraType of the attribute.</param>
        /// <param name="dbContext">The TypeManager of the PandoraDatabase</param>
        /// <returns>A List of Guids.</returns>
        public Exceptional<ASetReferenceEdgeType> GetCorrespondigDBObjectUUIDAsList(GraphDBType myType, DBContext dbContext, AEdgeType mySourceEdge, GraphDBType validationType)
        {
            #region data

            ASetReferenceEdgeType _referenceEdge = (ASetReferenceEdgeType)mySourceEdge.GetNewInstance();

            #endregion

            #region Evaluate tuple

            //ask guid-index of type
            foreach (TupleElement aTupleElement in TupleElements)
            {
                switch (aTupleElement.TypeOfValue)
                {
                       
                    case TypesOfOperatorResult.NotABasicType:

                        if (aTupleElement.Value is BinaryExpressionDefinition)
                        {

                            #region Binary Expression

                            var aUniqueExpr = (BinaryExpressionDefinition)aTupleElement.Value;

                            var validationResult = aUniqueExpr.Validate(dbContext, validationType);
                            if (validationResult.Failed)
                            {
                                return new Exceptional<ASetReferenceEdgeType>(validationResult);
                            }

                            var _graphResult = aUniqueExpr.Calculon(dbContext, new CommonUsageGraph(dbContext));

                            if (_graphResult.Success)
                            {
                                foreach (var aDBO in _graphResult.Value.Select(new LevelKey(validationType, dbContext.DBTypeManager), null, true))
                                {
                                    if (aDBO.Failed)
                                    {
                                        return new Exceptional<ASetReferenceEdgeType>(aDBO);
                                    }
                                    _referenceEdge.Add(aDBO.Value.ObjectUUID, aDBO.Value.TypeUUID, aTupleElement.Parameters.ToArray());
                                }
                            }
                            else
                            {
                                return new Exceptional<ASetReferenceEdgeType>(_graphResult);
                            }

                            #endregion

                        }
                        else
                        {

                            #region tuple node

                            if (aTupleElement.Value is TupleDefinition)
                            {
                                var aTupleNode = (TupleDefinition)aTupleElement.Value;

                                if (IsValidTupleNode(aTupleNode.TupleElements, myType, dbContext))
                                {
                                    #region get partial results

                                    BinaryExpressionDefinition tempNode = null;

                                    foreach (TupleElement aElement in aTupleNode)
                                    {
                                        tempNode = (BinaryExpressionDefinition)aElement.Value;

                                        if (ValidateBinaryExpression(tempNode, validationType, dbContext).Failed)
                                        {
                                            return new Exceptional<ASetReferenceEdgeType>(new Error_InvalidBinaryExpression(tempNode));
                                        }

                                        var tempGraphResult = tempNode.Calculon(dbContext, new CommonUsageGraph(dbContext));

                                        if (tempGraphResult.Success)
                                        {
                                            foreach (var aDBO in tempGraphResult.Value.Select(new LevelKey(validationType, dbContext.DBTypeManager), null, true))
                                            {
                                                if (aDBO.Failed)
                                                {
                                                    return new Exceptional<ASetReferenceEdgeType>(aDBO);
                                                }
                                                _referenceEdge.Add(aDBO.Value.ObjectUUID, aDBO.Value.TypeUUID, aTupleElement.Parameters.ToArray());
                                            }
                                        }
                                        else
                                        {
                                            return new Exceptional<ASetReferenceEdgeType>(tempGraphResult);
                                        }
                                    }

                                    #endregion
                                }
                                else
                                {
                                    throw new GraphDBException(new Error_UnknownDBError("Found an invalid TupleNode while analyzing ListOfDBObjects"));
                                }
                            }
                            else
                            {
                                return new Exceptional<ASetReferenceEdgeType>(new Error_NotImplemented(new StackTrace(true), "Error while checking the elements of ListOfDBObjects. A tupleElement is not a BinaryExpression or a Tuple."));
                                //throw new GraphDBException(new Error_SetOfAssignment("Error while checking the elements of ListOfDBObjects. A tupleElement is not a BinaryExpression or a Tuple."));
                            }

                            #endregion

                        }

                        break;

                    default:

                        return new Exceptional<ASetReferenceEdgeType>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                }
            }

            #endregion

            return new Exceptional<ASetReferenceEdgeType>(_referenceEdge);
        }


        #region ValidateBinaryExpression

        protected Exceptional ValidateBinaryExpression(BinaryExpressionDefinition aUniqueExpr, GraphDBType validationType, DBContext typeManager)
        {



            switch (aUniqueExpr.TypeOfBinaryExpression)
            {

                case TypesOfBinaryExpression.LeftComplex:
                    return ValidateBinaryExpressionInternal(aUniqueExpr.Left, validationType, typeManager);

                case TypesOfBinaryExpression.RightComplex:
                    return ValidateBinaryExpressionInternal(aUniqueExpr.Right, validationType, typeManager);

                case TypesOfBinaryExpression.Complex:
                    return new Exceptional()
                        .Push(ValidateBinaryExpressionInternal(aUniqueExpr.Left, validationType, typeManager))
                        .Push(ValidateBinaryExpressionInternal(aUniqueExpr.Right, validationType, typeManager));

                case TypesOfBinaryExpression.Atom:

                default:
                    return Exceptional.OK;

            }


        }

        private Exceptional ValidateBinaryExpressionInternal(AExpressionDefinition aUniqueExpr, GraphDBType validationType, DBContext dbContext)
        {
            if (aUniqueExpr is BinaryExpressionDefinition)
            {
                return (aUniqueExpr as BinaryExpressionDefinition).Validate(dbContext, validationType);
                //return ValidateBinaryExpression((BinaryExpressionDefinition)aUniqueExpr, validationType, dbContext);
            }
            else
            {
                var _potIdNode = aUniqueExpr as IDChainDefinition;

                if (_potIdNode != null)
                {
                    //var validationResult = _potIdNode.ValidateMe(validationType, typeManager);
                    var validationResult = _potIdNode.Validate(dbContext, false, validationType);
                    return validationResult;
                }
                else
                {
                    return Exceptional.OK;
                }
            }
        }

        #endregion

    }
}
