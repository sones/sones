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
using ISonesGQLFunction.Structure;
using sones.GraphDB;
using sones.GraphDB.ErrorHandling;
using sones.GraphDB.Extensions;
using sones.GraphDB.TypeSystem;
using sones.GraphQL.ErrorHandling;
using sones.GraphQL.GQL.Manager.Plugin;
using sones.GraphQL.GQL.Structure.Helper.ExpressionGraph;
using sones.GraphQL.GQL.Structure.Helper.ExpressionGraph.Helper;
using sones.GraphQL.GQL.Structure.Nodes.Expressions;
using sones.GraphQL.Structure.Helper.Enums;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Library.PropertyHyperGraph;
using sones.Plugins.SonesGQL.Aggregates;
using sones.Plugins.SonesGQL.Function.ErrorHandling;
using sones.Plugins.SonesGQL.Functions;

namespace sones.GraphQL.GQL.Structure.Nodes.Misc
{

    #region ChainPartAggregateDefinition

    public class ChainPartAggregateDefinition : ChainPartFuncDefinition
    {

        public ChainPartAggregateDefinition(ChainPartFuncDefinition chainPartFuncDefinition)
        {
            this.FuncName = chainPartFuncDefinition.FuncName;
            this.Parameters = chainPartFuncDefinition.Parameters;
            this.SourceParsedString = chainPartFuncDefinition.SourceParsedString;
        }

        private IDChainDefinition _Parameter = null;
        public IDChainDefinition Parameter
        {
            get
            {
                System.Diagnostics.Debug.Assert(_Parameter != null);
                return _Parameter;
            }
        }

        public IGQLAggregate Aggregate { get; private set; }

        public override void Validate(GQLPluginManager myPluginManager, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            if (!myPluginManager.HasPlugin<IGQLAggregate>(FuncName))
            {
                throw new AggregateOrFunctionDoesNotExistException(FuncName);
            }

            Aggregate = myPluginManager.GetAndInitializePlugin<IGQLAggregate>(FuncName);

            if (Parameters.Count != 1)
            {
                throw new AggregateParameterCountMismatchException(FuncName, 1, Parameters.Count);
            }

            _Parameter = Parameters.FirstOrDefault() as IDChainDefinition;
            if (_Parameter == null)
            {
                throw new AggregateNotAllowedException(this.FuncName);
            }

            _Parameter.Validate(myPluginManager, myGraphDB, mySecurityToken, myTransactionToken, false);
        }

    }

    #endregion

    #region ChainPartFuncDefinition

    public class ChainPartFuncDefinition : AIDChainPart
    {

        /// <summary>
        /// Replace me
        /// </summary>
        //public FuncCallNode FuncCallNode;

        public String SourceParsedString { get; set; }
        public String FuncName { get; set; }
        //public String Alias { get; set; }
        public IGQLFunction Function { get; private set; }

        /// <summary>
        /// Refactor me. This could be SimpleValue or BinExpr as well
        /// </summary>
        public List<AExpressionDefinition> Parameters { get; set; }

        public ChainPartFuncDefinition()
        {
            Parameters = new List<AExpressionDefinition>();
        }

        public virtual void Validate(GQLPluginManager myPluginManager, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            if (myPluginManager.HasPlugin<IGQLFunction>(FuncName))
            {
                Function = myPluginManager.GetAndInitializePlugin<IGQLFunction>(FuncName);
            }
            else
            {
                throw new AggregateOrFunctionDoesNotExistException(FuncName);
            }

            #region parameter exceptions

            #region check number of parameters

            Boolean containsVariableNumOfParams = this.Function.GetParameters().Exists(p => p.VariableNumOfParams);

            if (this.Function.GetParameters().Count != Parameters.Count && (!containsVariableNumOfParams))
            {
                throw new FunctionParameterCountMismatchException(this.Function.FunctionName, this.Function.GetParameters().Count, Parameters.Count);
            }
            else if (containsVariableNumOfParams && Parameters.Count == 0)
            {
                throw new FunctionParameterCountMismatchException(this.Function.FunctionName, 1, Parameters.Count);
            }

            #endregion

            #endregion

        }


        public List<FuncParameter> Execute(IVertexType myTypeOfDBObject, IVertex myDBObject, String myReference, GQLPluginManager myPluginManager, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {

            List<FuncParameter> evaluatedParams = new List<FuncParameter>();
            int paramCounter = 0;
            FuncParameter validationOutput;
            //ParameterValue currentParameter;

            for (int i = 0; i < Parameters.Count; i++)
            {
                ParameterValue currentParameter = Function.GetParameter(paramCounter);

                if (Parameters[i] is BinaryExpressionDefinition)
                {

                    ((BinaryExpressionDefinition)Parameters[i]).Validate(myPluginManager, myGraphDB, mySecurityToken, myTransactionToken);
                    
                    #region handle BinExp

                    var calculatedGraphResult = ((BinaryExpressionDefinition)Parameters[i]).Calculon(myPluginManager, myGraphDB, mySecurityToken, myTransactionToken, new CommonUsageGraph(myGraphDB, mySecurityToken, myTransactionToken));

                    var extractedDBOs = calculatedGraphResult.Select(new LevelKey(myTypeOfDBObject.ID, myGraphDB, mySecurityToken, myTransactionToken), null, true);

                    #region validation
                    
                    validationOutput = ValidateAndAddParameter(currentParameter, extractedDBOs, null);

                    evaluatedParams.Add(validationOutput);

                    #endregion

                    #endregion
                }
                else
                {
                    if (Parameters[i] is IDChainDefinition)
                    {
                        #region handle IDNode

                        var tempIDChain = (IDChainDefinition)Parameters[i];

                        tempIDChain.Validate(myPluginManager, myGraphDB, mySecurityToken, myTransactionToken, false);

                        if (currentParameter.Value is IAttributeDefinition)
                        {
                            #region validation

                            validationOutput = ValidateAndAddParameter(currentParameter, tempIDChain.LastAttribute, null);

                            evaluatedParams.Add(validationOutput);

                            #endregion
                        }
                        else
                        {
                            if ((tempIDChain.LastAttribute == null) && (tempIDChain.SelectType != TypesOfSelect.None))
                            {
                                #region IDNode with asterisk

                                #region validation

                                validationOutput = ValidateAndAddParameter(currentParameter, tempIDChain, null);

                                evaluatedParams.Add(validationOutput);

                                #endregion

                                #endregion
                            }
                            else
                            {
                                switch (tempIDChain.LastAttribute.Kind)
                                {
                                    case AttributeType.Property:

                                        #region Property

                                        IPropertyDefinition propertyDefinition = (IPropertyDefinition)tempIDChain.LastAttribute;

                                        IComparable value = null;

                                        if (myDBObject.HasProperty(propertyDefinition.ID))
	                                    {
                                            value = propertyDefinition.GetValue(myDBObject);
	                                    }

                                        #region validation

                                        validationOutput = ValidateAndAddParameter(currentParameter, value, tempIDChain.LastAttribute);

                                        evaluatedParams.Add(validationOutput);

                                        #endregion

                                        #endregion
                                        break;
                                    
                                    case AttributeType.IncomingEdge:

                                        #region IncomingEdge

                                        IIncomingEdgeDefinition incomingEdgeAttribute = (IIncomingEdgeDefinition)tempIDChain.LastAttribute;

                                        IEnumerable<IVertex> dbos = new List<IVertex>();

                                        if (myDBObject.HasIncomingVertices(incomingEdgeAttribute.RelatedEdgeDefinition.SourceVertexType.ID, incomingEdgeAttribute.RelatedEdgeDefinition.ID))
	                                    {
                                            dbos = myDBObject.GetIncomingVertices(incomingEdgeAttribute.RelatedEdgeDefinition.SourceVertexType.ID, incomingEdgeAttribute.RelatedEdgeDefinition.ID);
	                                    }

                                        #region validation

                                        validationOutput = ValidateAndAddParameter(currentParameter, dbos, tempIDChain.LastAttribute);

                                        evaluatedParams.Add(validationOutput);

                                        #endregion

                                        #endregion
                                        break;

                                    case AttributeType.OutgoingEdge:
                                        
                                        #region outgoing Edge

                                        IOutgoingEdgeDefinition outgoingEdgeAttribute = (IOutgoingEdgeDefinition)tempIDChain.LastAttribute;

                                        IEnumerable<IVertex> outgoingDBOs = new List<IVertex>();

                                        if (myDBObject.HasOutgoingEdge(outgoingEdgeAttribute.ID))
	                                    {
                                            outgoingDBOs = myDBObject.GetOutgoingEdge(outgoingEdgeAttribute.ID).GetTargetVertices();
	                                    }

                                        #region validation

                                        validationOutput = ValidateAndAddParameter(currentParameter, outgoingDBOs, tempIDChain.LastAttribute);

                                        evaluatedParams.Add(validationOutput);

                                        #endregion

                                        #endregion
                                        break;

                                    default:
                                        break;
                                }   
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        #region any other

                        #region validation

                        validationOutput = ValidateAndAddParameter(currentParameter, Parameters[i], null);
                        
                        evaluatedParams.Add(validationOutput);

                        #endregion

                        #endregion

                    }
                }

                #region increase parameter counter

                if (!currentParameter.VariableNumOfParams)
                {
                    paramCounter++;
                }

                #endregion

            }


            return evaluatedParams;

        }
      
        private FuncParameter ValidateAndAddParameter(ParameterValue myParameter, Object myValue, IAttributeDefinition myTypeAttribute)
        {
            if (myParameter.Value != null)
            {
                if (!(myParameter.Value as Type).IsAssignableFrom(myValue.GetType()))
                {
                    throw new FunctionParameterTypeMismatchException(myParameter.Value as Type, myValue.GetType());
                }
            }
            else
                throw new InvalidFunctionParameterException(myParameter.Name, myValue.GetType(), null);
                                    
            return new FuncParameter(myValue, myTypeAttribute);
        }

        private FuncParameter ValidateAndAddParameter(ParameterValue myParameter, AExpressionDefinition myValue, IAttributeDefinition myTypeAttribute)
        {

            if (!(myValue is ValueDefinition))
            {
                throw new InvalidVertexAttributeValueException(myParameter.Name, myValue);
            }

            var val = (myValue as ValueDefinition).Value;

            try
            {
                Convert.ChangeType(val, myParameter.Value as Type);
            }
            catch (Exception)
            {
                throw new FunctionParameterTypeMismatchException(myParameter.Value as Type, val.GetType());
            }

            return new FuncParameter(val, myTypeAttribute);
        }


    }

    #endregion

    #region ChainPartTypeOrAttributeDefinition

    /// <summary>
    /// Type or Attribute or Reference
    /// </summary>
    public class ChainPartTypeOrAttributeDefinition : AIDChainPart
    {

        /// <summary>
        /// Type or Attribute or Reference
        /// </summary>
        public String TypeOrAttributeName { get; private set; }

        public IAttributeDefinition TypeAttribute { get; set; }
        public IVertexType DBType { get; set; }

        public EdgeKey EdgeKey
        {
            get
            {
                System.Diagnostics.Debug.Assert(TypeAttribute != null);
                return new EdgeKey(DBType.ID, TypeAttribute.ID);
            }
        }

        public ChainPartTypeOrAttributeDefinition(String myTypeOrAttributeName)
        {
            TypeOrAttributeName = myTypeOrAttributeName;
        }

        public override string ToString()
        {
            return TypeOrAttributeName;
        }

    }

    #endregion

    #region IDChainDelemiter

    public class IDChainDelemiter
    {
        public KindOfDelimiter KindOfDelimiter { get; private set; }

        public IDChainDelemiter(KindOfDelimiter myKindOfDelimiter)
        {
            KindOfDelimiter = myKindOfDelimiter;
        }
    }

    #endregion

    #region AIDChainPart

    public class AIDChainPart
    {
        public AIDChainPart Next;
        public IDChainDelemiter NextDelemiter;
        public IDChainDelemiter PrevDelemiter;
    }

    #endregion

    public class IDChainDefinition : ATermDefinition, IEnumerable<AIDChainPart>
    {

        #region statics

        /// <summary>
        /// Create a new IDCHainDefinition of the <paramref name="myType"/> and the chain of attributes.
        /// </summary>
        /// <param name="myType"></param>
        /// <param name="myAttribute"></param>
        /// <returns></returns>
        public static IDChainDefinition Create(String myType, params String[] myAttribute)
        {
            var chain = new IDChainDefinition(myType + "." + myAttribute, new List<TypeReferenceDefinition>() { new TypeReferenceDefinition(myType, myType) });
            foreach (var attr in myAttribute)
            {
                chain.AddPart(new ChainPartTypeOrAttributeDefinition(attr));
            }

            return chain;
        }

        #endregion

        #region Properties

        public Boolean IsValidated { get; private set; }
        public AIDChainPart RootPart { get; private set; }
        public TypesOfSelect SelectType { get; private set; }
        public String UndefinedAttribute { get; private set; }
        public LevelKey LevelKey { get; private set; }
        public String TypeName { get; private set; }

        #region LastType

        private IVertexType _LastType = null;
        public IVertexType LastType
        {
            get
            {
                System.Diagnostics.Debug.Assert(IsValidated);
                return _LastType;
            }
        }

        #endregion

        #region LastAttribute

        private IAttributeDefinition _LastAttribute;
        public IAttributeDefinition LastAttribute
        {
            get
            {
                System.Diagnostics.Debug.Assert(IsValidated);
                return _LastAttribute;
            }
        }

        #endregion

        #region Depth

        public Int32 Depth
        {
            get
            {
                System.Diagnostics.Debug.Assert(IsValidated);
                if (_Edges == null || _Edges.Count == 0)
                    return 0;
                else if (_Edges.Count == 1 && !_Edges[0].IsAttributeSet)
                    return 0;
                else
                    return _Edges.Count;
            }
        }

        #endregion

        #region IsUndefinedAttribute

        public Boolean IsUndefinedAttribute
        {
            get
            {
                System.Diagnostics.Debug.Assert(IsValidated);
                return UndefinedAttribute != null;
            }
        }

        #endregion

        #region Edges

        /// <summary>
        /// List of EdgeKeys
        /// </summary>
        public List<EdgeKey> Edges
        {
            get
            {
                System.Diagnostics.Debug.Assert(IsValidated);
                return _Edges;
            }
        }

        #endregion

        #region Reference

        private Tuple<string, IVertexType> _Reference;
        public Tuple<string, IVertexType> Reference
        {
            get
            {
                System.Diagnostics.Debug.Assert(IsValidated);
                return _Reference;
            }
        }

        #endregion

        #region Level

        public int Level
        {
            get
            {
                System.Diagnostics.Debug.Assert(IsValidated);
                return LevelKey.Level;
            }
        }

        #endregion

        #region Content string

        public String ContentString
        {
            get
            {
                return _IDChainString;
            }
        }

        #endregion

        #endregion

        #region Data

        private String                          _IDChainString;
        private List<TypeReferenceDefinition>  _References;
        private List<EdgeKey>                   _Edges = new List<EdgeKey>();

        #endregion

        #region Ctors

        public IDChainDefinition()
        { }

        /// <summary>
        /// Creates a not validated chain for just one type.
        /// </summary>
        /// <param name="myReferences">The name of the type</param>
        public IDChainDefinition(String myIDChainString, List<TypeReferenceDefinition> myReferences)
        {
            _IDChainString = myIDChainString;
            _References = myReferences;
        }

        /// <summary>
        /// Creates an validated asterisk chain for the given type.
        /// </summary>
        /// <param name="myIDChainPart">The type chain part.</param>
        public IDChainDefinition(ChainPartTypeOrAttributeDefinition myIDChainPart, TypesOfSelect mySelType, String myTypeName = null)
        {
            RootPart = myIDChainPart;

            IsValidated = true;
            SelectType = mySelType;

            _Edges = new List<EdgeKey>();
            LevelKey = new LevelKey();
        }

        #endregion

        #region AddPart

        /// <summary>
        /// Adds a new part and the precedent delemiter to the chain
        /// </summary>
        /// <param name="myPart">The part of the ID chain</param>
        /// <param name="myIDChainDelemiter">The precedent delemiter</param>
        public void AddPart(AIDChainPart myPart, IDChainDelemiter myIDChainDelemiter = null)
        {

            if (RootPart == null)
            {
                RootPart = myPart;
            }
            else
            {

                var curPart = RootPart;
                while (curPart.Next != null)
                {
                    curPart = curPart.Next;
                }
                curPart.Next = myPart;

                #region Really?

                if (myIDChainDelemiter == null)
                {
                    myIDChainDelemiter = new IDChainDelemiter(KindOfDelimiter.Dot);
                }

                #endregion

                curPart.NextDelemiter = myIDChainDelemiter;
                myPart.PrevDelemiter = myIDChainDelemiter;

            }
        }

        #endregion

        #region Validate

        /// <summary>
        /// Validates the id chain if it is not already validated and returns all errors and warnings.
        /// </summary>
        public void Validate(GQLPluginManager myPluginManager, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken, Boolean allowUndefinedAttributes, params IVertexType[] validationTypes)
        {

            if (IsValidated)
            {
                return;
            }

            var listOfRefs = new Dictionary<String, IVertexType>();
            if (validationTypes == null || validationTypes.Count() == 0)
            {
                foreach (var type in _References)
                {
                    var vertexType = myGraphDB.GetVertexType<IVertexType>(
                        mySecurityToken,
                        myTransactionToken,
                        new GraphDB.Request.RequestGetVertexType(type.TypeName),
                        (stats, theVertexType) => theVertexType);

                    listOfRefs.Add(type.Reference, vertexType);
                }
            }
            else
            {
                foreach (var type in validationTypes)
                {
                    listOfRefs.Add(type.Name, type);
                }
            }

            Validate(myPluginManager, myGraphDB, mySecurityToken, myTransactionToken, listOfRefs, allowUndefinedAttributes);
        }

        /// <summary>
        /// Validates the id chain if it is not already validated and returns all errors and warnings.
        /// </summary>
        public void Validate(GQLPluginManager myPluginManager, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken, Dictionary<String, IVertexType> myListOfReferences, Boolean allowUndefinedAttributes = false)
        {

            IsValidated = true;

            int _HashCode = 0;
            Object funcWorkingBase = null;
            var curPart = RootPart;

            if (curPart == null)
            {
                if (myListOfReferences.Count != 1)
                {
                    throw new DuplicateReferenceOccurrenceException("");
                }

                _Reference = new Tuple<string, IVertexType>(myListOfReferences.First().Key, myListOfReferences.First().Value);
                _LastType = _Reference.Item2;
                _Edges = new List<EdgeKey>();
                _Edges.Add(new EdgeKey(_LastType.ID));
                SelectType = TypesOfSelect.Asterisk;
                LevelKey = new LevelKey(_Edges, myGraphDB, mySecurityToken, myTransactionToken);
            }

            while (curPart != null)
            {

                #region Go through each part and try to fill lastType, lastAttribute etc...

                if (curPart is ChainPartFuncDefinition)
                {

                    #region ChainPartFuncDefinition

                    var funcPart = (curPart as ChainPartFuncDefinition);
                    funcPart.Validate(myPluginManager, myGraphDB, mySecurityToken, myTransactionToken);

                    if (!funcPart.Function.ValidateWorkingBase(funcWorkingBase, myGraphDB, mySecurityToken, myTransactionToken))
                    {
                        throw new InvalidFunctionBaseException(_LastAttribute.Name, funcPart.FuncName);
                    }

                    var returnType = funcPart.Function.GetReturnType();
                    if (returnType != null)
                    {
                        if (returnType is IAttributeDefinition)
                        {
                            //_LastAttribute = (returnType as DBTypeAttribute).GetValue();
                            //_LastType = GetDBTypeByAttribute(myDBContext, _LastAttribute);
                        }
                        else if (returnType is Object)
                        {
                            //_LastAttribute = null;
                            //_LastType = (returnType as DBType).GetValue();
                        }
                        else if (returnType is Type)
                        {
                            //_LastAttribute = null;
                            //_LastType = myDBContext.DBTypeManager.GetTypeByName((returnType as ADBBaseObject).ObjectName);
                        }
                        else
                        {
                            throw new InvalidFunctionReturnTypeException(funcPart.FuncName, returnType.GetType(), typeof(IAttributeDefinition), typeof(IVertexType), typeof(Object));
                        }
                        funcWorkingBase = returnType;
                    }

                    #endregion

                }
                else if (curPart is ChainPartTypeOrAttributeDefinition)
                {

                    var typeOrAttr = curPart as ChainPartTypeOrAttributeDefinition;

                    #region Check whether it is a reference

                    if (_LastAttribute != null)
                    {

                        #region Any attribute on an undefined attributes or not user defined (reference) type is not allowed

                        if (_LastAttribute is UnstructuredProperty || _LastAttribute.Kind == AttributeType.Property)
                        {
                            throw new VertexAttributeIsNotDefinedException(typeOrAttr.TypeOrAttributeName);
                        }

                        #endregion

                        #region The previous attribute seems to be an reference attribute

                        _LastType = GetTargetVertexTypeByAttribute(_LastAttribute);

                        typeOrAttr.DBType = _LastType;
                        typeOrAttr.TypeAttribute = typeOrAttr.DBType.GetAttributeDefinition(typeOrAttr.TypeOrAttributeName);
                        _LastAttribute = typeOrAttr.TypeAttribute;

                        if (_LastAttribute == null)
                        {

                            #region Undefined attribute

                            if (allowUndefinedAttributes)
                            {
                                UndefinedAttribute = typeOrAttr.TypeOrAttributeName;
                                _LastAttribute = new UnstructuredProperty(UndefinedAttribute);
                                typeOrAttr.TypeAttribute = _LastAttribute;
                                AddNewEdgeKey(_LastType, _LastAttribute.ID);
                            }
                            else
                            {
                                throw new VertexAttributeIsNotDefinedException(typeOrAttr.TypeOrAttributeName);
                            }

                            #endregion

                        }
                        else
                        {

                            AddNewEdgeKey(_LastType, _LastAttribute.ID);

                        }

                        #endregion

                    }
                    else if (_LastType == null)
                    {

                        #region We did not resolve any type - so we should have a reference (U.Friends) or an undefined attribute

                        if (myListOfReferences.ContainsKey(typeOrAttr.TypeOrAttributeName))
                        {

                            #region The current TypeOrAttributeName is either an reference U

                            typeOrAttr.DBType = myListOfReferences[typeOrAttr.TypeOrAttributeName];

                            //Case 1: sth like "U" (where U stands for User)
                            #region Case 1

                            _LastType = typeOrAttr.DBType;
                            _Reference = new Tuple<string, IVertexType>(typeOrAttr.TypeOrAttributeName, typeOrAttr.DBType);
                            
                            if (curPart.Next == null) // we just have an type definition
                            {
                                SelectType = TypesOfSelect.Asterisk;
                                _Edges.Add(new EdgeKey(_LastType.ID));
                            }

                            #endregion

                            //Case 2: an attribute like "Car" (of "User") is used and in parallel a similar written Type exists. In this case
                            // it is necessary to throw an error.
                            #region Case 2

                            var typesWithAttibute = (from aContextType in myListOfReferences
                                                     where (aContextType.Key != typeOrAttr.TypeOrAttributeName) && aContextType.Value.GetAttributeDefinition(typeOrAttr.TypeOrAttributeName) != null
                                                     select aContextType.Key).FirstOrDefault();

                            if (typesWithAttibute != null)
                            {
                                throw new DuplicateReferenceOccurrenceException(typeOrAttr.TypeOrAttributeName);
                            }

                            #endregion

                            #region Skip this part - this is a reference

                            RootPart = curPart.Next;

                            #endregion

                            #endregion

                        }
                        else
                        {

                            //Case 3: 
                            //  (3.1)In this case it must be an attribute. If it is used in an ambigous way, an exception would be thrown.
                            //  (3.2)In this case it can be an undefined attribute or if it was used like an edge it is treated like does not exist (U.NotExists.Name)

                            #region case 3

                            #region sth like Name --> we have to find out the corresponding type

                            Boolean foundSth = false;
                            String reference = null;
                            foreach (var contextElement in myListOfReferences)
                            {
                                var tempAttr = contextElement.Value.GetAttributeDefinition(typeOrAttr.TypeOrAttributeName);


                                //tempTypeAttribute = (from aAttribute in tempType.AttributeLookupTable where aAttribute.Value.Name == tempTypeOrAttributeName select aAttribute.Value).FirstOrDefault();

                                if (tempAttr != null)
                                {

                                    typeOrAttr.DBType = contextElement.Value;
                                    reference = contextElement.Key;

                                    if (foundSth == true)
                                    {
                                        throw new AmbiguousVertexAttributeException("The attribute or type \"" + typeOrAttr.TypeOrAttributeName + "\" has been used ambigous.");
                                    }
                                    else
                                    {
                                        typeOrAttr.TypeAttribute = tempAttr;
                                        foundSth = true;
                                    }
                                }
                            }

                            #endregion

                            if (foundSth)
                            {

                                #region 3.1

                                //_Edges.Add(new EdgeKey(typeOrAttr.DBType.VertexID, typeOrAttr.TypeAttribute.VertexID));
                                _LastAttribute = typeOrAttr.TypeAttribute;
                                _LastType = typeOrAttr.DBType;
                                _Reference = new Tuple<string, IVertexType>(reference, typeOrAttr.DBType); //T1 -->key in context dictionary

                                _Edges.ForEach(item => _HashCode = _HashCode ^ item.GetHashCode());
                                AddNewEdgeKey(_LastType, _LastAttribute.ID);

                                #endregion

                            }
                            else
                            {

                                #region 3.2 - Undefined attribute or attribute does not exist

                                if (curPart.Next != null || !allowUndefinedAttributes)
                                {

                                    #region Calling an attribute on an undefined attribute is not allowed

                                    throw new VertexAttributeIsNotDefinedException(typeOrAttr.TypeOrAttributeName);

                                    #endregion

                                }
                                else
                                {

                                    #region This is the last unknown attribute and with this, it is an undefined attribute

                                    UndefinedAttribute = typeOrAttr.TypeOrAttributeName;

                                    if (myListOfReferences.Count != 1)
                                    {
                                        throw new AmbiguousVertexAttributeException("The attribute or type \"" + typeOrAttr.TypeOrAttributeName + "\" has been used ambigous.");
                                    }
                                    else
                                    {
                                        var theRef = myListOfReferences.First();
                                        _Reference = new Tuple<string, IVertexType>(theRef.Key, theRef.Value); //T1 -->key in context dictionary
                                        _LastType = _Reference.Item2;
                                        _LastAttribute = new UnstructuredProperty(UndefinedAttribute);

                                        typeOrAttr.TypeAttribute = _LastAttribute;
                                        typeOrAttr.DBType = _LastType;

                                        AddNewEdgeKey(_LastType, _LastAttribute.ID);
                                    }

                                    #endregion

                                }

                                //_isValidated = false;

                                //_invalidNodeParts.AddRange(parseNode.ChildNodes);

                                #endregion

                            }

                            #endregion
                        }

                        #endregion

                    }
                    else
                    {

                        #region This is a regular attribute after a reference (U.Name)

                        typeOrAttr.DBType = _LastType;
                        typeOrAttr.TypeAttribute = typeOrAttr.DBType.GetAttributeDefinition(typeOrAttr.TypeOrAttributeName);
                        _LastAttribute = typeOrAttr.TypeAttribute;

                        if (_LastAttribute == null)
                        {

                            if (curPart.Next != null || !allowUndefinedAttributes)
                            {
                                #region Calling an attribute on an undefined attribute is not allowed - so we assume a typo

                                throw new VertexAttributeIsNotDefinedException(typeOrAttr.TypeOrAttributeName);
                                //continue;

                                #endregion

                            }
                            else
                            {
                                UndefinedAttribute = typeOrAttr.TypeOrAttributeName;
                                _LastAttribute = new UnstructuredProperty(UndefinedAttribute);
                                typeOrAttr.TypeAttribute = _LastAttribute;
                                AddNewEdgeKey(_LastType, _LastAttribute.ID);
                            }

                        }
                        else
                        {

                            AddNewEdgeKey(_LastType, _LastAttribute.ID);

                        }

                        #endregion

                    }

                    #endregion

                    funcWorkingBase = _LastAttribute;

                }

                curPart = curPart.Next;

                #endregion

            }

            LevelKey = new LevelKey(Edges, myGraphDB, mySecurityToken, myTransactionToken);
        }

        private IVertexType GetTargetVertexTypeByAttribute(IAttributeDefinition _LastAttribute)
        {
            switch (_LastAttribute.Kind)
            {
                case AttributeType.IncomingEdge:

                    var incomingEdgeAttribute = (IIncomingEdgeDefinition)_LastAttribute;
                    return incomingEdgeAttribute.RelatedEdgeDefinition.SourceVertexType;

                case AttributeType.OutgoingEdge:
                    
                    var outgoingEdgeAttribute = (IOutgoingEdgeDefinition)_LastAttribute;
                    return outgoingEdgeAttribute.TargetVertexType;

                case AttributeType.Property:
                default:

                    return null;
            }
        }

        /// <summary>
        /// This method adds a new EdgeKey to the IDNode
        /// </summary>
        /// <param name="myStartingType">The type that corresponds to the attribute.</param>
        /// <param name="tempTypeAttributeUUID">The attribute VertexID.</param>
        private void AddNewEdgeKey(IVertexType myStartingType, Int64 tempTypeAttributeUUID)
        {
            EdgeKey tempEdgeKey = new EdgeKey(myStartingType.ID, tempTypeAttributeUUID);

            _LastType = myStartingType;

            _Edges.Add(tempEdgeKey);
        }

        #endregion

        #region IEnumerable<AIDChainPart> Members

        public IEnumerator<AIDChainPart> GetEnumerator()
        {
            if (RootPart == null)
            {
                yield break;
            }

            var curPart = RootPart;
            do
            {
                yield return curPart;
                curPart = curPart.Next;
            }
            while (curPart != null);
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        public override bool Equals(object obj)
        {

            if (!(obj is IDChainDefinition))
            {
                return false;
            }
            var otherChain = (obj as IDChainDefinition);

            return _IDChainString != otherChain._IDChainString;

        }

        public override int GetHashCode()
        {
            if (_IDChainString == null)
            {
                return 0;
            }
            return _IDChainString.GetHashCode();
        }


        public bool IsSpecialTypeAttribute()
        {
            return (LastAttribute != null && LastAttribute.RelatedType.Name == SonesGQLConstants.BaseVertexTypeName);
        }

    }

}
