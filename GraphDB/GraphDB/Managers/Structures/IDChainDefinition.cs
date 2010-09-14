/*
 * IDChainDefinition
 * (c) Stefan Licht, 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using sones.Lib.ErrorHandling;
using sones.GraphDB.Errors;
using sones.GraphDB.TypeManagement;
using sones.Lib;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Structures.Enums;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.Structures.ExpressionGraph;
using sones.GraphDB.Functions;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.GraphDB.Aggregates;

using sones.GraphDB.TypeManagement;

using sones.GraphDB.Context;
using sones.GraphDB.TypeManagement.SpecialTypeAttributes;

#endregion

namespace sones.GraphDB.Managers.Structures
{


	#region ChainPartFuncDefinition

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

		public ABaseAggregate Aggregate { get; private set; }

		public override Exceptional Validate(DBContext dbContext)
		{

			if (!dbContext.DBPluginManager.HasAggregate(FuncName))
			{
				return new Exceptional(new Error_AggregateOrFunctionDoesNotExist(FuncName));
			}
			Aggregate = dbContext.DBPluginManager.GetAggregate(FuncName);

			if (Parameters.Count != 1)
			{
				return new Exceptional(new Error_AggregateParameterCountMismatch(FuncName, 1, Parameters.Count));
			}

			_Parameter = Parameters.FirstOrDefault() as IDChainDefinition;
			if (_Parameter == null)
			{
				return new Exceptional(new Error_AggregateNotAllowed(this));
			}

			var result = _Parameter.Validate(dbContext, false);

			return result;
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
		public ABaseFunction Function { get; private set; }

		/// <summary>
		/// Refactor me. This could be SimpleValue or BinExpr as well
		/// </summary>
		public List<AExpressionDefinition> Parameters { get; set; }

		public ChainPartFuncDefinition()
		{
			Parameters = new List<AExpressionDefinition>();
		}

		public virtual Exceptional Validate(DBContext dbContext)
		{
			if (dbContext.DBPluginManager.HasFunction(FuncName))
			{
				Function = dbContext.DBPluginManager.GetFunction(FuncName);
			}
			else
			{
				return new Exceptional(new Error_AggregateOrFunctionDoesNotExist(FuncName));
			}

			#region parameter exceptions

			#region check number of parameters

			Boolean containsVariableNumOfParams = this.Function.GetParameters().Exists(p => p.VariableNumOfParams);

			if (this.Function.GetParameters().Count != Parameters.Count && (!containsVariableNumOfParams))
			{
				return new Exceptional<FuncParameter>(new Error_FunctionParameterCountMismatch(this.Function, this.Function.GetParameters().Count, Parameters.Count));
			}
			else if (containsVariableNumOfParams && Parameters.Count == 0)
			{
				return new Exceptional<FuncParameter>(new Error_FunctionParameterCountMismatch(this.Function, 1, Parameters.Count));
			}

			#endregion

			#endregion

			return Exceptional.OK;

		}


		public Exceptional<FuncParameter> Execute(GraphDBType myTypeOfDBObject, DBObjectStream myDBObject, String myReference, DBContext dbContext)
		{

			List<FuncParameter> evaluatedParams = new List<FuncParameter>();
			int paramCounter = 0;
			Exceptional<FuncParameter> validationOutput;
			//ParameterValue currentParameter;
			var _warnings = new List<IWarning>();

			for (int i = 0; i < Parameters.Count; i++)
			{
				ParameterValue currentParameter = Function.GetParameter(paramCounter);

				if (Parameters[i] is BinaryExpressionDefinition)
				{

					var validateResult = ((BinaryExpressionDefinition)Parameters[i]).Validate(dbContext);
					if (validateResult.Failed())
					{
						return new Exceptional<FuncParameter>(validateResult);
					}

					#region handle BinExp

					var calculatedGraphResult = ((BinaryExpressionDefinition)Parameters[i]).Calculon(dbContext, new CommonUsageGraph(dbContext));

					if (calculatedGraphResult.Failed())
					{
						return new Exceptional<FuncParameter>(calculatedGraphResult);
					}

					var extractedDBOs = calculatedGraphResult.Value.Select(new LevelKey(myTypeOfDBObject, dbContext.DBTypeManager), null, true);

					#region validation

					validationOutput = ValidateAndAddParameter(currentParameter, extractedDBOs, null);

					if (validationOutput.Failed())
					{
						return new Exceptional<FuncParameter>(validationOutput);
					}
					else
					{
						evaluatedParams.Add(validationOutput.Value);
					}

					#region expressionGraph error handling

					_warnings.AddRange(calculatedGraphResult.Value.GetWarnings());

					#endregion

					#endregion

					#endregion
				}
				else
				{
					if (Parameters[i] is IDChainDefinition)
					{
						#region handle IDNode

						var tempIDChain = (IDChainDefinition)Parameters[i];

						var validateResult = tempIDChain.Validate(dbContext, false);
						if (validateResult.Failed())
						{
							return new Exceptional<FuncParameter>(validateResult);
						}

						if (currentParameter.DBType is DBTypeAttribute)
						{
							//if (myTypeOfDBObject == tempIDNode.LastType)
							//{
							#region validation

							validationOutput = ValidateAndAddParameter(currentParameter, tempIDChain.LastAttribute, null);

							if (validationOutput.Failed())
							{
								return new Exceptional<FuncParameter>(validationOutput);
							}
							else
							{
								evaluatedParams.Add(validationOutput.Value);
							}

							#endregion
							//}
						}
						else
						{
							if ((tempIDChain.LastAttribute == null) && (tempIDChain.SelectType != TypesOfSelect.None))
							{
								#region IDNode with asterisk

								#region validation

								validationOutput = ValidateAndAddParameter(currentParameter, tempIDChain, null);

								if (validationOutput.Failed())
								{
									return new Exceptional<FuncParameter>(validationOutput);
								}
								else
								{
									evaluatedParams.Add(validationOutput.Value);
								}

								#endregion

								#endregion
							}
							else
							{
								if (tempIDChain.LastAttribute.IsBackwardEdge)
								{
									#region BackwardEdge

									TypeAttribute typeAttr = dbContext.DBTypeManager.GetTypeAttributeByEdge(tempIDChain.LastAttribute.BackwardEdgeDefinition);
									var dbos = myDBObject.GetBackwardEdges(tempIDChain.LastAttribute.BackwardEdgeDefinition, dbContext, dbContext.DBObjectCache, tempIDChain.LastAttribute.GetDBType(dbContext.DBTypeManager));

									if (dbos.Failed())
										return new Exceptional<FuncParameter>(dbos);

									if (dbos.Value == null)
										return new Exceptional<FuncParameter>(new FuncParameter(null, tempIDChain.LastAttribute));

									#region validation

									validationOutput = ValidateAndAddParameter(currentParameter, dbos.Value, tempIDChain.LastAttribute);

									if (validationOutput.Failed())
									{
										return new Exceptional<FuncParameter>(validationOutput);
									}
									else
									{
										evaluatedParams.Add(validationOutput.Value);
									}

									#endregion

									#endregion
								}
								else
								{
									if (myDBObject.HasAttribute(tempIDChain.LastAttribute.UUID, tempIDChain.LastType))
									{
										#region DBObject has attribute

										#region validation

										validationOutput = ValidateAndAddParameter(currentParameter, myDBObject.GetAttribute(tempIDChain.LastAttribute.UUID, tempIDChain.LastType, dbContext), tempIDChain.LastAttribute);

										if (validationOutput.Failed())
										{
											return new Exceptional<FuncParameter>(validationOutput);
										}
										else
										{
											evaluatedParams.Add(validationOutput.Value);
										}

										#endregion

										#endregion
									}
									else
									{
										return new Exceptional<FuncParameter>(new Error_DBObjectDoesNotHaveAttribute(tempIDChain.LastAttribute.Name));
									}
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

						if (validationOutput.Failed())
						{
							return new Exceptional<FuncParameter>(validationOutput);
						}
						else
						{
							evaluatedParams.Add(validationOutput.Value);
						}

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

			//_Function.CallingAttribute = _CallingAttribute;
			//_Function.CallingObject = _CallingObject;
			//_Function.CallingDBObjectStream = _CallingDBObjectStream;

			var result = Function.ExecFunc(dbContext, evaluatedParams.ToArray());

			foreach (var _wa in _warnings)
				result.PushIWarning(_wa);

			return result;

		}
		/*
		private Exceptional<FuncParameter> ValidateAndAddParameter(ParameterValue myParameter, TypeAttribute myValue, TypeAttribute myTypeAttribute)
		{

			var val = myValue;
			if (!myParameter.DBType.IsValidValue(val))
			{
				return new Exceptional<FuncParameter>(new Error_FunctionParameterTypeMismatch(myParameter.DBType.Value.GetType(), val.GetType()));
			}
			myParameter.DBType.SetValue(val);

			return new Exceptional<FuncParameter>(new FuncParameter(myParameter.DBType, myTypeAttribute));
		}

		private Exceptional<FuncParameter> ValidateAndAddParameter(ParameterValue myParameter, IEnumerable<Exceptional<DBObjectStream>> myValue, TypeAttribute myTypeAttribute)
		{

			var val = myValue;
			if (!myParameter.DBType.IsValidValue(val))
			{
				return new Exceptional<FuncParameter>(new Error_FunctionParameterTypeMismatch(myParameter.DBType.Value.GetType(), val.GetType()));
			}
			myParameter.DBType.SetValue(val);

			return new Exceptional<FuncParameter>(new FuncParameter(myParameter.DBType, myTypeAttribute));
		}
		*/
		private Exceptional<FuncParameter> ValidateAndAddParameter(ParameterValue myParameter, Object myValue, TypeAttribute myTypeAttribute)
		{

			var val = myValue;
			if (!myParameter.DBType.IsValidValue(val))
			{
				return new Exceptional<FuncParameter>(new Error_FunctionParameterTypeMismatch(myParameter.DBType.Value.GetType(), val.GetType()));
			}
			myParameter.DBType.SetValue(val);

			return new Exceptional<FuncParameter>(new FuncParameter(myParameter.DBType, myTypeAttribute));
		}

		private Exceptional<FuncParameter> ValidateAndAddParameter(ParameterValue myParameter, AExpressionDefinition myValue, TypeAttribute myTypeAttribute)
		{

			if (!(myValue is ValueDefinition))
			{
				return new Exceptional<FuncParameter>(new Error_InvalidAttributeValue(myParameter.Name, myValue));
			}

			var val = (myValue as ValueDefinition).Value.Value;

			if (!myParameter.DBType.IsValidValue(val))
			{
				return new Exceptional<FuncParameter>(new Error_FunctionParameterTypeMismatch(myParameter.DBType.Value.GetType(), val.GetType()));
			}

			myParameter.DBType.SetValue(val);

			return new Exceptional<FuncParameter>(new FuncParameter(myParameter.DBType, myTypeAttribute));
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

		public TypeAttribute TypeAttribute { get; set; }
		public GraphDBType DBType { get; set; }

		public EdgeKey EdgeKey
		{
			get
			{
                System.Diagnostics.Debug.Assert(TypeAttribute != null);
				return new EdgeKey(DBType.UUID, TypeAttribute.UUID);
			}
		}

		public ChainPartTypeOrAttributeDefinition(String myTypeOrAttributeName)
		{
			TypeOrAttributeName = myTypeOrAttributeName;
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

		#region Properties

		public Boolean          IsValidated         { get; private set; }
		public AIDChainPart     RootPart            { get; private set; }
		public Exceptional      ValidateResult      { get; private set; }
		public TypesOfSelect    SelectType          { get; private set; }        
		public String           UndefinedAttribute  { get; private set; }
		public LevelKey         LevelKey            { get; private set; }
		public String           TypeName            { get; private set; }

		#region LastType

		private GraphDBType _LastType = null;
		public GraphDBType LastType
		{
			get
			{
				System.Diagnostics.Debug.Assert(IsValidated);
				return _LastType;
			}
		}
		
		#endregion

		#region LastAttribute

		private TypeAttribute _LastAttribute;
		public TypeAttribute LastAttribute
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
				if (_Edges.IsNullOrEmpty())
					return 0;
				else if (_Edges.Count == 1 && _Edges[0].AttrUUID == null)
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

		private Tuple<string, GraphDBType> _Reference;
		public Tuple<string, GraphDBType> Reference
		{
			get {
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

			if (SelectType == TypesOfSelect.Ad)
			{
				TypeName = myTypeName;
			}

			ValidateResult = new Exceptional();
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
		/// <param name="myDBContext">The DBContext</param>
		/// <param name="validationTypes">A list of type on which is chain is going to be validated. If null, any predefined reference definition is used!</param>
		/// <returns></returns>
		public Exceptional Validate(DBContext myDBContext, Boolean allowUndefinedAttributes, params GraphDBType[] validationTypes)
		{

			if (IsValidated)
			{
				return ValidateResult;
			}

			var listOfRefs = new Dictionary<String, GraphDBType>();
			if (validationTypes.IsNullOrEmpty())
			{
				foreach (var type in _References)
				{

					var graphDBType = myDBContext.DBTypeManager.GetTypeByName(type.TypeName);
					if (graphDBType == null)
					{
						return new Exceptional(new Error_TypeDoesNotExist(type.TypeName));
					}

					listOfRefs.Add(type.Reference, graphDBType);
				}
			}
			else
			{
				foreach (var type in validationTypes)
				{
					listOfRefs.Add(type.Name, type);
				}
			}
			return Validate(myDBContext, listOfRefs, allowUndefinedAttributes);
		}

		/// <summary>
		/// Validates the id chain if it is not already validated and returns all errors and warnings.
		/// </summary>
		/// <param name="myDBContext">The DBContext</param>
		/// <param name="myListOfReferences">A list of references</param>
		/// <returns></returns>
		public Exceptional Validate(DBContext myDBContext, Dictionary<String, GraphDBType> myListOfReferences, Boolean allowUndefinedAttributes = false)
		{

			IsValidated = true;
			ValidateResult = new Exceptional();

			int _HashCode = 0;
			IObject funcWorkingBase = null;
			var curPart = RootPart;

			if (curPart == null)
			{
				if (myListOfReferences.Count != 1)
				{
					ValidateResult.PushIError(new Error_DuplicateReferenceOccurence(""));
				}
				//typeOrAttr.DBType = myListOfReferences.First().Value;

				_Reference = new Tuple<string, GraphDBType>(myListOfReferences.First().Key, myListOfReferences.First().Value);
				_LastType = _Reference.Item2;
				_Edges = new List<EdgeKey>();
				_Edges.Add(new EdgeKey(_LastType.UUID, null));
				SelectType = TypesOfSelect.Asterisk;
				ValidateResult = new Exceptional();
				LevelKey = new LevelKey(_Edges, myDBContext.DBTypeManager);

				return ValidateResult;
			}

			while (curPart != null && ValidateResult.Success())
			{

				#region Go through each part and try to fill lastType, lastAttribute etc...

				if (curPart is ChainPartFuncDefinition)
				{

					#region ChainPartFuncDefinition

					var funcPart = (curPart as ChainPartFuncDefinition);
					var funcValidateResult = funcPart.Validate(myDBContext);
					ValidateResult.PushIExceptional(funcValidateResult);

					if (ValidateResult.Failed())
					{
						return ValidateResult;
					}

					//if (curPart.Next != null)
					//{
					//    return ValidateResult.Push(new Error_NotImplemented(new System.Diagnostics.StackTrace(true), "Currently it is not implemented to have a funtion in between two edges."));
					//}

					//if (funcWorkingBase == null) // the first time, it is null and should be set to the _LastAttribute
					//{
					//    funcWorkingBase = new DBTypeAttribute(_LastAttribute);
					//}

					if (!funcPart.Function.ValidateWorkingBase(funcWorkingBase, myDBContext.DBTypeManager))
					{
						return ValidateResult.PushIError(new Error_InvalidFunctionBase(_LastAttribute, funcPart.FuncName));
					}

					var returnType = funcPart.Function.GetReturnType(new DBTypeAttribute(_LastAttribute), myDBContext.DBTypeManager);
					if (returnType != null)
					{
						if (returnType is DBTypeAttribute)
						{
							_LastAttribute = (returnType as DBTypeAttribute).GetValue();
							_LastType = GetDBTypeByAttribute(myDBContext, _LastAttribute);
						}
						else if (returnType is DBType)
						{
							_LastAttribute = null;
							_LastType = (returnType as DBType).GetValue();
						}
						else if (returnType is ADBBaseObject)
						{
							_LastAttribute = null;
							_LastType = myDBContext.DBTypeManager.GetTypeByName((returnType as ADBBaseObject).ObjectName);
						}
						else
						{
							return new Exceptional(new Error_InvalidFunctionReturnType(funcPart.FuncName, returnType.GetType(), typeof(DBTypeAttribute), typeof(DBType), typeof(ADBBaseObject)));
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

                        #region Any attribute on an undefined attribute is not allowed

                        if (_LastAttribute is UndefinedTypeAttribute)
                        {
                            ValidateResult.PushIError(new Error_AttributeIsNotDefined(typeOrAttr.TypeOrAttributeName));
                        }
                        
                        #endregion

						#region The previous attribute was seems to be an reference attribute

						_LastType = GetDBTypeByAttribute(myDBContext, _LastAttribute);

						typeOrAttr.DBType = _LastType;
						typeOrAttr.TypeAttribute = typeOrAttr.DBType.GetTypeAttributeByName(typeOrAttr.TypeOrAttributeName);
						_LastAttribute = typeOrAttr.TypeAttribute;

						if (_LastAttribute == null)
						{

							#region Undefined attribute

							if (allowUndefinedAttributes)
							{
								UndefinedAttribute = typeOrAttr.TypeOrAttributeName;
                                _LastAttribute = new UndefinedTypeAttribute(UndefinedAttribute);
                                typeOrAttr.TypeAttribute = _LastAttribute;
                                AddNewEdgeKey(_LastType, _LastAttribute.UUID);
                            }
							else
							{
								ValidateResult.PushIError(new Error_AttributeIsNotDefined(typeOrAttr.TypeOrAttributeName));
							}

							#endregion

						}
						else
						{

							AddNewEdgeKey(_LastType, _LastAttribute.UUID);

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
							_Reference = new Tuple<string, GraphDBType>(typeOrAttr.TypeOrAttributeName, typeOrAttr.DBType);

							if (curPart.Next == null) // we just have an type definition
							{
								SelectType = TypesOfSelect.Asterisk;
								_Edges.Add(new EdgeKey(_LastType.UUID));
							}

							#endregion

							//Case 2: an attribute like "Car" (of "User") is used and in parallel a similar written Type exists. In this case
							// it is necessary to throw an error.
							#region Case 2

							var typesWithAttibute = (from aContextType in myListOfReferences
													 where (aContextType.Key != typeOrAttr.TypeOrAttributeName) && aContextType.Value.GetTypeAttributeByName(typeOrAttr.TypeOrAttributeName) != null
													 select aContextType.Key).FirstOrDefault();

							if (typesWithAttibute != null)
							{
								ValidateResult.PushIError(new Error_DuplicateReferenceOccurence(typeOrAttr.TypeOrAttributeName));
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
								var tempAttr = contextElement.Value.GetTypeAttributeByName(typeOrAttr.TypeOrAttributeName);


								//tempTypeAttribute = (from aAttribute in tempType.AttributeLookupTable where aAttribute.Value.Name == tempTypeOrAttributeName select aAttribute.Value).FirstOrDefault();

								if (tempAttr != null)
								{

									typeOrAttr.DBType = contextElement.Value;
									reference = contextElement.Key;
									
									if (foundSth == true)
									{
										ValidateResult.PushIError(new Error_AmbiguousAttribute("The attribute or type \"" + typeOrAttr.TypeOrAttributeName + "\" has been used ambigous."));
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

								//_Edges.Add(new EdgeKey(typeOrAttr.DBType.UUID, typeOrAttr.TypeAttribute.UUID));
								_LastAttribute = typeOrAttr.TypeAttribute;
								_LastType = typeOrAttr.DBType;
								_Reference = new Tuple<string, GraphDBType>(reference, typeOrAttr.DBType); //T1 -->key in context dictionary

								_Edges.ForEach(item => _HashCode = _HashCode ^ item.GetHashCode());
								AddNewEdgeKey(_LastType, _LastAttribute.UUID);

								#endregion

							}
							else
							{

								#region 3.2 - Undefined attribute or attribute does not exist

								if (curPart.Next != null || !allowUndefinedAttributes)
								{

									#region Calling an attribute on an undefined attribute is not allowed

									ValidateResult.PushIError(new Error_AttributeIsNotDefined(typeOrAttr.TypeOrAttributeName));
									//ValidateResult.Push(new Error_InvalidUndefinedAttributeName());

									#endregion

								}
								else
								{

									#region This is the last unknown attribute and with this, it is an undefined attribute

									UndefinedAttribute = typeOrAttr.TypeOrAttributeName;

									if (myListOfReferences.Count != 1)
									{
										ValidateResult.PushIError(new Error_AmbiguousAttribute("The attribute or type \"" + typeOrAttr.TypeOrAttributeName + "\" has been used ambigous."));
									}
									else
									{
										var theRef = myListOfReferences.First();
										_Reference = new Tuple<string, GraphDBType>(theRef.Key, theRef.Value); //T1 -->key in context dictionary
										_LastType = _Reference.Item2;
                                        _LastAttribute = new UndefinedTypeAttribute(UndefinedAttribute);
                                        
                                        typeOrAttr.TypeAttribute = _LastAttribute;
                                        typeOrAttr.DBType = _LastType;
                                        
                                        AddNewEdgeKey(_LastType, _LastAttribute.UUID);
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
						typeOrAttr.TypeAttribute = typeOrAttr.DBType.GetTypeAttributeByName(typeOrAttr.TypeOrAttributeName);
						_LastAttribute = typeOrAttr.TypeAttribute;

						if (_LastAttribute == null)
						{   
							
							if (curPart.Next != null || !allowUndefinedAttributes)
							{                                
								#region Calling an attribute on an undefined attribute is not allowed - so we assume a typo

								ValidateResult.PushIError(new Error_AttributeIsNotDefined(typeOrAttr.TypeOrAttributeName));
								continue;

								#endregion

							}
							else
							{
                                UndefinedAttribute = typeOrAttr.TypeOrAttributeName;
                                _LastAttribute = new UndefinedTypeAttribute(UndefinedAttribute);
                                typeOrAttr.TypeAttribute = _LastAttribute;
                                AddNewEdgeKey(_LastType, _LastAttribute.UUID);
                            }

						}
						else
						{

							AddNewEdgeKey(_LastType, _LastAttribute.UUID);

						}

						#endregion

					}

					#endregion

					funcWorkingBase = new DBTypeAttribute(_LastAttribute);

				}

				curPart = curPart.Next;

				#endregion

			}

			LevelKey = new LevelKey(Edges, myDBContext.DBTypeManager);

			return ValidateResult;

		}

		private GraphDBType GetDBTypeByAttribute(DBContext myDBContext, TypeAttribute _LastAttribute)
		{

			if (_LastAttribute.IsBackwardEdge)
			{
				return myDBContext.DBTypeManager.GetTypeByUUID(_LastAttribute.BackwardEdgeDefinition.TypeUUID);
			}
			else
			{
				return _LastAttribute.GetDBType(myDBContext.DBTypeManager);
			}
		
		}

		/// <summary>
		/// This method adds a new EdgeKey to the IDNode
		/// </summary>
		/// <param name="myStartingType">The type that corresponds to the attribute.</param>
		/// <param name="tempTypeAttributeUUID">The attribute uuid.</param>
		private void AddNewEdgeKey(GraphDBType myStartingType, AttributeUUID tempTypeAttributeUUID)
		{
			EdgeKey tempEdgeKey = new EdgeKey(myStartingType.UUID, tempTypeAttributeUUID);

			_LastType = myStartingType;

			_Edges.Add(tempEdgeKey);
		}

		#endregion
		
		#region ToString()

		public override string ToString()
		{
			return _IDChainString +
				((IsValidated) ? " [validated] " : " [notValidated] ") +
				((ValidateResult == null || ValidateResult.Success()) ? "" : ValidateResult.GetIErrorsAsString())
				;
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
            return (LastAttribute != null && LastAttribute is ASpecialTypeAttribute);
        }

    }


}
