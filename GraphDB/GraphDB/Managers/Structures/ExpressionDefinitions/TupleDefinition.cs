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

using sones.Lib.ErrorHandling;
using sones.GraphDB.Errors;
using sones.GraphFS.DataStructures;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Structures.ExpressionGraph;
using System.Diagnostics;
using sones.GraphDB.Managers.Select;
using sones.Lib.ErrorHandling;
using sones.GraphDB.Result;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.BasicTypes;

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
        public BasicType TypeOfOperatorResult { get; private set; }

        #endregion

        #region Ctors

        public TupleDefinition(KindOfTuple kindOfTuple = KindOfTuple.Inclusive)
        {
            TupleElements = new List<TupleElement>();
            TypeOfOperatorResult = BasicType.Unknown;
            KindOfTuple = kindOfTuple;
        }

        public TupleDefinition(BasicType myBasicType, IObject myObject, GraphDBType myGraphType, KindOfTuple kindOfTuple = KindOfTuple.Inclusive)
            : this()
        {
            TypeOfOperatorResult = myBasicType;
            KindOfTuple = kindOfTuple;

            if (myObject is IBaseEdge)
            {
                foreach (var obj in (IBaseEdge)myObject)
                {
                    TupleElements.Add(new TupleElement(new ValueDefinition(obj as ADBBaseObject)));
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
                var tupleElement = TupleElements[i].Value;

                if (tupleElement is SelectDefinition)
                {

                    #region partial select

                    var selectManager = new SelectManager();
                    var qresult = selectManager.ExecuteSelect(myDBContext, (tupleElement as SelectDefinition));
                    if (qresult.Failed)
                    {
                        return new Exceptional(qresult.Errors);
                    }

                    TypeAttribute curAttr = ((tupleElement as SelectDefinition).SelectedElements.First().Item1 as IDChainDefinition).LastAttribute;

                    var dbTypeOfAttribute = curAttr.GetDBType(myDBContext.DBTypeManager);

                    var aTypeOfOperatorResult = GraphDBTypeMapper.ConvertGraph2CSharp(dbTypeOfAttribute.Name);

                    foreach (var _Vertex in qresult.Vertices)
                    {
                        if (!(_Vertex.IsAttribute(curAttr.Name)))
                            continue;

                        if (curAttr != null)
                        {
                            var val = new ValueDefinition(aTypeOfOperatorResult, _Vertex.ObsoleteAttributes[curAttr.Name]);
                            newTuple.Add(new TupleElement(aTypeOfOperatorResult, val));
                        }
                        else
                        {
                            throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
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

        internal Exceptional<ASetOfReferencesEdgeType> GetAsUUIDEdge(DBContext dbContext, TypeAttribute attr)
        {
            var edge = attr.EdgeType.GetNewInstance() as ASetOfReferencesEdgeType;

            foreach (TupleElement aTupleElement in TupleElements)
            {

                if (!(aTupleElement.Value is ValueDefinition))
                {
                    return new Exceptional<ASetOfReferencesEdgeType>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                }
                else
                {

                    edge.Add(ObjectUUID.FromString((aTupleElement.Value as ValueDefinition).Value.ToString()), attr.DBTypeUUID);

                }

            }

            return new Exceptional<ASetOfReferencesEdgeType>(edge);
        }

        public TupleDefinition Simplyfy()
        {
            if (TupleElements.Count == 1 && TupleElements[0].Value is TupleDefinition)
            {
                return (TupleElements[0].Value as TupleDefinition).Simplyfy();
            }
            return this;
        }

        /// <summary>
        /// Checks if there is a valid tuple node. Valid tuple nodes in this case look like : (Name = 'Henning', Age = 10)
        /// </summary>
        /// <param name="tupleElementList">List of tuple elements</param>
        /// <param name="myAttributes">myAttributes of the type</param>
        /// <returns>True if valid or otherwise false</returns>
        internal bool IsValidTupleNode(List<TupleElement> tupleElementList, GraphDBType myGraphType, DBContext dbContext)
        {
            foreach (TupleElement aTupleElement in tupleElementList)
            {
                if (aTupleElement.Value is BinaryExpressionDefinition)
                {
                    var validateResult = ((BinaryExpressionDefinition)aTupleElement.Value).Validate(dbContext, myGraphType);
                    if (validateResult.Failed())
                    {
                        throw new GraphDBException(validateResult.IErrors);
                    }
                    //if (!IsValidBinaryExpressionNode((BinaryExpressionDefinition)aTupleElement.Value, myGraphType))
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
        /// <param name="TypeOfAttribute">GraphType of the attribute.</param>
        /// <param name="dbContext">The TypeManager of the GraphDatabase</param>
        /// <returns>A List of Guids.</returns>
        public Exceptional<ASetOfReferencesEdgeType> GetCorrespondigDBObjectUUIDAsList(GraphDBType myType, DBContext dbContext, IEdgeType mySourceEdge, GraphDBType validationType)
        {
            #region data

            ASetOfReferencesEdgeType _referenceEdge = (ASetOfReferencesEdgeType)mySourceEdge.GetNewInstance();

            #endregion

            #region Evaluate tuple

            //ask guid-index of type
            foreach (TupleElement aTupleElement in TupleElements)
            {
                switch (aTupleElement.TypeOfValue)
                {

                    case BasicType.NotABasicType:

                        if (aTupleElement.Value is BinaryExpressionDefinition)
                        {

                            #region Binary Expression

                            var aUniqueExpr = (BinaryExpressionDefinition)aTupleElement.Value;

                            var validationResult = aUniqueExpr.Validate(dbContext, validationType);
                            if (validationResult.Failed())
                            {
                                return new Exceptional<ASetOfReferencesEdgeType>(validationResult);
                            }

                            var _graphResult = aUniqueExpr.Calculon(dbContext, new CommonUsageGraph(dbContext));

                            if (_graphResult.Success())
                            {
                                foreach (var aDBO in _graphResult.Value.Select(new LevelKey(validationType, dbContext.DBTypeManager), null, true))
                                {
                                    if (aDBO.Failed())
                                    {
                                        return new Exceptional<ASetOfReferencesEdgeType>(aDBO);
                                    }
                                    _referenceEdge.Add(aDBO.Value.ObjectUUID, aDBO.Value.TypeUUID, aTupleElement.Parameters.ToArray());
                                }
                            }
                            else
                            {
                                return new Exceptional<ASetOfReferencesEdgeType>(_graphResult);
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

                                        if (ValidateBinaryExpression(tempNode, validationType, dbContext).Failed())
                                        {
                                            return new Exceptional<ASetOfReferencesEdgeType>(new Error_InvalidBinaryExpression(tempNode));
                                        }

                                        var tempGraphResult = tempNode.Calculon(dbContext, new CommonUsageGraph(dbContext));

                                        if (tempGraphResult.Success())
                                        {
                                            foreach (var aDBO in tempGraphResult.Value.Select(new LevelKey(validationType, dbContext.DBTypeManager), null, true))
                                            {
                                                if (aDBO.Failed())
                                                {
                                                    return new Exceptional<ASetOfReferencesEdgeType>(aDBO);
                                                }
                                                _referenceEdge.Add(aDBO.Value.ObjectUUID, aDBO.Value.TypeUUID, aTupleElement.Parameters.ToArray());
                                            }
                                        }
                                        else
                                        {
                                            return new Exceptional<ASetOfReferencesEdgeType>(tempGraphResult);
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
                                return new Exceptional<ASetOfReferencesEdgeType>(new Error_NotImplemented(new StackTrace(true), "Error while checking the elements of ListOfDBObjects. A tupleElement is not a BinaryExpression or a Tuple."));
                                //throw new GraphDBException(new Error_SetOfAssignment("Error while checking the elements of ListOfDBObjects. A tupleElement is not a BinaryExpression or a Tuple."));
                            }

                            #endregion

                        }

                        break;

                    default:

                        return new Exceptional<ASetOfReferencesEdgeType>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                }
            }

            #endregion

            return new Exceptional<ASetOfReferencesEdgeType>(_referenceEdge);
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
                        .PushIExceptional(ValidateBinaryExpressionInternal(aUniqueExpr.Left, validationType, typeManager))
                        .PushIExceptional(ValidateBinaryExpressionInternal(aUniqueExpr.Right, validationType, typeManager));

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
