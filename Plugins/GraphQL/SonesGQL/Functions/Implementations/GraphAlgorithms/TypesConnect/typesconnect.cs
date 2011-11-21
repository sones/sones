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
using sones.Library.VersionedPluginManager;
using sones.Library.PropertyHyperGraph;
using ISonesGQLFunction.Structure;
using sones.GraphDB.TypeSystem;
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphQL.Result;
using sones.Plugins.SonesGQL.Function.ErrorHandling;
using System.Diagnostics;
using System.Collections;
using sones.Plugins.SonesGQL.Functions;
using sones.Library.CollectionWrapper;

namespace sones.Plugins.SonesGQL.Functions.TypesConnect
{
   
	 public sealed class TypesConnect : ABaseFunction, IPluginable
	 {  
		
		private List<List<Tuple<long,long>>> _path = new List<List<Tuple<long,long>>>();
        private List<Tuple<String, String>>  _stringPath = new List<Tuple<string, string>>();

		#region depth-first search

		void DFS(IVertexType start, IVertexType end, List<Tuple<long,long>> Parents)
		{
		
		  var outEdges =  start.GetOutgoingEdgeDefinitions(true);
		  var incEdges = start.GetIncomingEdgeDefinitions(true);


		  
		  foreach (IOutgoingEdgeDefinition vertexType in outEdges)
		  {
			  if (Parents.Where(x=>x.Item1 == vertexType.TargetVertexType.ID).Count()==0)//(Tuple.Create(vertexType.TargetVertexType.ID,vertexType.ID)))
			  {
				  
					 var current_parents =  Parents.ToList();
					 Parents.Add(Tuple.Create(vertexType.TargetVertexType.ID,vertexType.ID));

					 if (Parents.Last().Item1 == end.ID && !_path.Contains(Parents))
						 _path.Add(Parents);
					 else
					 {
						 DFS(vertexType.TargetVertexType, end, Parents);
					 }
				  Parents = current_parents;
			  }
		  }
		  foreach (IIncomingEdgeDefinition vertexType in incEdges)
		  {
			  if (Parents.Where(x => x.Item1 == vertexType.RelatedEdgeDefinition.SourceVertexType.ID).Count() == 0) //if (!Parents.Contains(Tuple.Create(vertexType.RelatedEdgeDefinition.SourceVertexType.ID, vertexType.ID)))
			  {
				  var current_parents = Parents.ToList();
				  Parents.Add(Tuple.Create(vertexType.RelatedEdgeDefinition.SourceVertexType.ID,vertexType.ID));

				 
				  if (Parents.Last().Item1 == end.ID && !_path.Contains(Parents))
				  {
					  _path.Add(Parents);
				  }
				  else
				  {
					 DFS(vertexType.RelatedEdgeDefinition.SourceVertexType, end, Parents);
				  }
				  Parents = current_parents;
			  }
		  }
		}
		#endregion

		#region Dijkstra for VertexTypes

		private Tuple<double, List<Tuple<long, long>>, IVertexType, IVertexType> ShortPath(IVertexType start, IVertexType end)
		{
			#region initialization

			var currentVertex = start;

			List<UInt64> depthBuffer = new List<UInt64>();

			List<long> edgeBuffer = new List<long>();

			List<double> distanceBuffer = new List<double>();
			List<IVertexType> VertexBuffer = new List<IVertexType>();




			BufferForFindPathSchema buf = new BufferForFindPathSchema();
            DataForFindPathSchema lists = new DataForFindPathSchema();
            
			buf.Add(start, 0, 0);
			lists.Add(start, 0, 0, 0, start);

			bool endVertexFlag = false;

			#endregion

			#region Dijkstra algorithm


			double current_distance = 1;

			double currentVertexDistance = 0;
			ulong currentVertexDepth = 0;

			double endVertexDistance = 0;
			ulong endvertexDepth = 0;

			Stopwatch clock = new Stopwatch();
			clock.Start();

			while (buf.Count != 0)
			{
				var hyperEdgeOut = currentVertex.GetOutgoingEdgeDefinitions(true);
				var hyperEdgeInc = currentVertex.GetIncomingEdgeDefinitions(true);

				if (hyperEdgeOut != null || hyperEdgeInc != null)
				{

					for (int iCount = 0; iCount < hyperEdgeOut.Count(); iCount++)
					{

						var TargetVertexType = hyperEdgeOut.ElementAt(iCount).TargetVertexType;


						var current_Edge = hyperEdgeOut.ElementAt(iCount).ID;

						var TargetVertexID = lists.GetElement(TargetVertexType.ID);

						if (TargetVertexID == null)
						{

							if (!endVertexFlag)
							{



								buf.Add(TargetVertexType,
									current_distance + currentVertexDistance,
								   currentVertexDepth + 1);



								lists.Add(TargetVertexType,
									current_distance + currentVertexDistance,
									currentVertexDepth + 1,
									current_Edge,
									currentVertex);

							}
							else
								if (endVertexDistance > currentVertexDistance + current_distance ||
									(endVertexDistance == currentVertexDistance + current_distance &&
									endvertexDepth > currentVertexDepth + 1))
								{



									buf.Add(TargetVertexType,
								   current_distance + currentVertexDistance,
								  currentVertexDepth + 1);


									lists.Add(TargetVertexType,
								   current_distance + currentVertexDistance,
								   currentVertexDepth + 1,
								   current_Edge,
								   currentVertex);


								}
						}

						else
						{
							if (currentVertexDistance + current_distance < TargetVertexID.Item2)
							{
								if (!endVertexFlag)
								{



									buf.Set(TargetVertexID.Item2, TargetVertexType,
								   current_distance + currentVertexDistance,
								  currentVertexDepth + 1);

									lists.Set(TargetVertexType,
								   current_distance + currentVertexDistance,
								   currentVertexDepth + 1,
								   current_Edge,
								   currentVertex);




								}

								else
									if (endVertexDistance > currentVertexDistance + current_distance ||
									(endVertexDistance == currentVertexDistance + current_distance &&
									endvertexDepth > currentVertexDepth + 1))
									{

										buf.Set(TargetVertexID.Item2, TargetVertexType,
									current_distance + currentVertexDistance,
								   currentVertexDepth + 1);

										lists.Set(TargetVertexType,
								   current_distance + currentVertexDistance,
								   currentVertexDepth + 1,
								   current_Edge,
								   currentVertex);

									}
							}
							else if (currentVertexDistance + current_distance == TargetVertexID.Item2 && currentVertexDepth + 1 < TargetVertexID.Item3)
							{
								if (!endVertexFlag)
								{



									buf.Set(TargetVertexID.Item2, TargetVertexType,
										 current_distance + currentVertexDistance,
										currentVertexDepth + 1);

									lists.Set(TargetVertexType,
								   current_distance + currentVertexDistance,
								   currentVertexDepth + 1,
								   current_Edge,
								   currentVertex);



								}
								else
									if (endVertexDistance > currentVertexDistance + current_distance ||
									   (endVertexDistance == currentVertexDistance + current_distance &&
										endvertexDepth > currentVertexDepth + 1))
									{



										buf.Set(TargetVertexID.Item2, TargetVertexType,
											 current_distance + currentVertexDistance,
											 currentVertexDepth + 1);

										lists.Set(TargetVertexType,
								   current_distance + currentVertexDistance,
								   currentVertexDepth + 1,
								   current_Edge,
								   currentVertex);

									}
							}
						}


						if (TargetVertexType == end)
						{


							endVertexFlag = true;
							var endNode = lists.GetElement(end.ID);
							endVertexDistance = endNode.Item2;
							endvertexDepth = endNode.Item3;



						}

					}


					for (int iCount = 0; iCount < hyperEdgeInc.Count(); iCount++)
					{

						var TargetVertexType = hyperEdgeInc.ElementAt(iCount).RelatedEdgeDefinition.SourceVertexType;


						var current_Edge = hyperEdgeInc.ElementAt(iCount).ID;//.RelatedEdgeDefinition.EdgeType;

						var TargetVertexID = lists.GetElement(TargetVertexType.ID);

						if (TargetVertexID == null)
						{

							if (!endVertexFlag)
							{



								buf.Add(TargetVertexType,
									current_distance + currentVertexDistance,
								   currentVertexDepth + 1);



								lists.Add(TargetVertexType,
									current_distance + currentVertexDistance,
									currentVertexDepth + 1,
									current_Edge,
									currentVertex);

							}
							else
								if (endVertexDistance > currentVertexDistance + current_distance ||
									(endVertexDistance == currentVertexDistance + current_distance &&
									endvertexDepth > currentVertexDepth + 1))
								{



									buf.Add(TargetVertexType,
								   current_distance + currentVertexDistance,
								  currentVertexDepth + 1);


									lists.Add(TargetVertexType,
								   current_distance + currentVertexDistance,
								   currentVertexDepth + 1,
								   current_Edge,
								   currentVertex);


								}
						}

						else
						{
							if (currentVertexDistance + current_distance < TargetVertexID.Item2)
							{
								if (!endVertexFlag)
								{



									buf.Set(TargetVertexID.Item2, TargetVertexType,
								   current_distance + currentVertexDistance,
								  currentVertexDepth + 1);

									lists.Set(TargetVertexType,
								   current_distance + currentVertexDistance,
								   currentVertexDepth + 1,
								   current_Edge,
								   currentVertex);




								}

								else
									if (endVertexDistance > currentVertexDistance + current_distance ||
									(endVertexDistance == currentVertexDistance + current_distance &&
									endvertexDepth > currentVertexDepth + 1))
									{

										buf.Set(TargetVertexID.Item2, TargetVertexType,
									current_distance + currentVertexDistance,
								   currentVertexDepth + 1);

										lists.Set(TargetVertexType,
								   current_distance + currentVertexDistance,
								   currentVertexDepth + 1,
								   current_Edge,
								   currentVertex);

									}
							}
							else if (currentVertexDistance + current_distance == TargetVertexID.Item2 && currentVertexDepth + 1 < TargetVertexID.Item3)
							{
								if (!endVertexFlag)
								{



									buf.Set(TargetVertexID.Item2, TargetVertexType,
										 current_distance + currentVertexDistance,
										currentVertexDepth + 1);

									lists.Set(TargetVertexType,
								   current_distance + currentVertexDistance,
								   currentVertexDepth + 1,
								   current_Edge,
								   currentVertex);



								}
								else
									if (endVertexDistance > currentVertexDistance + current_distance ||
									   (endVertexDistance == currentVertexDistance + current_distance &&
										endvertexDepth > currentVertexDepth + 1))
									{



										buf.Set(TargetVertexID.Item2, TargetVertexType,
											 current_distance + currentVertexDistance,
											 currentVertexDepth + 1);

										lists.Set(TargetVertexType,
								   current_distance + currentVertexDistance,
								   currentVertexDepth + 1,
								   current_Edge,
								   currentVertex);

									}
							}
						}


						if (TargetVertexType == end)
						{


							endVertexFlag = true;
							var endNode = lists.GetElement(end.ID);
							endVertexDistance = endNode.Item2;
							endvertexDepth = endNode.Item3;



						}

					}


				}
				//delate from Buffer current Vertex or all 
				if (currentVertex == end)
				{

					buf.Clear();

				}
				else
				{

					buf.Remove(currentVertexDistance, currentVertex.ID);

				}

				//Minimum distance from Buffer
				if (buf.Count != 0)
				{

					var minVertex = buf.Min();
					currentVertex = minVertex.Item1;
					currentVertexDistance = minVertex.Item2;
					currentVertexDepth = minVertex.Item3;
				}

			}
			#endregion
            
			#region create output

			List<Tuple<long, long>> parents = new List<Tuple<long, long>>();
			currentVertex = end;

			while (currentVertex != start)
			{

				var current_tuple = lists.GetElement(currentVertex.ID);

				if (current_tuple == null)
					return null;

				VertexBuffer.Add(currentVertex);


				distanceBuffer.Add(current_tuple.Item2);
				depthBuffer.Add(current_tuple.Item3);
				edgeBuffer.Add(current_tuple.Item4);
				parents.Add(Tuple.Create(currentVertex.ID, current_tuple.Item4));
				currentVertex = current_tuple.Item5;


			}
			parents.Add(Tuple.Create(start.ID, 0L));


			return Tuple.Create(distanceBuffer.First(), parents, start, end);


		}

        #endregion

		#endregion

		#region constructor

		public TypesConnect()
		{
			Parameters.Add(new ParameterValue("Types", typeof(String)));
			Parameters.Add(new ParameterValue("Method", typeof(bool)));
		}

		#endregion

        #region ValidateWorkingBase

        public override bool ValidateWorkingBase(Object myWorkingBase, IGraphDB myGraphDB, SecurityToken mySecurityToken, Int64 myTransactionToken)
		{
			return myWorkingBase == null;
		}

        #endregion

        #region find all Path in the set of edges
        
        private void Helper(IGraphDB graph, SecurityToken sec, Int64 trans, List<TypeWithProperty> type_property_object, List<Tuple<long, long>> PATHShort)	
        {
           List<Tuple<String, String>> output = new List<Tuple<string, string>>();
		   this._stringPath.Clear();
		   var StartTypeLong =  PATHShort.Find(x => x.Item2 == 0);
		   var StartType = graph.GetVertexType(sec,
												trans, 
												new sones.GraphDB.Request.RequestGetVertexType(StartTypeLong.Item1),
													 (statistics, type) => type);

           this._stringPath.Add(Tuple.Create("Starting VertexType for SELECT", StartType.Name));
			

		   String str = StartType.Name;

		   foreach (TypeWithProperty value in type_property_object.Where(x => x.Type.ID == StartType.ID))
			   this._stringPath.Add(Tuple.Create(StartType.Name + '.' + value.PropertyDefinition.Name,StartType.Name + '.' + value.PropertyDefinition.Name));
			
		   Recurs(str, StartType, type_property_object, PATHShort);

		}
       

		private String Recurs(String str,IVertexType StartType, List<TypeWithProperty> type_property_object, List<Tuple<long, long>> PATHShortDSF)
		{
			var hyperEdgeOut = StartType.GetOutgoingEdgeDefinitions(true);
			var hyperEdgeInc = StartType.GetIncomingEdgeDefinitions(true);

			foreach (IOutgoingEdgeDefinition value in hyperEdgeOut)
			{
				if (PATHShortDSF.Find(x => x.Item2 == value.ID) != null)
				{
					if (type_property_object.Where(x => x.Type.ID == value.TargetVertexType.ID).Count() > 0)
					{
						var temp = str;
						foreach (TypeWithProperty val in type_property_object.Where(x => x.Type.ID == value.TargetVertexType.ID))
						{
							_stringPath.Add(Tuple.Create(val.Type.Name+'.'+val.PropertyDefinition.Name,str + '.' + value.Name + '.' + val.PropertyDefinition.Name));
						}
						 str = temp + '.' + value.Name;
						Recurs(str, value.TargetVertexType, type_property_object, PATHShortDSF);
						str = temp;
					}
					else
					{
						var temp = str;
						str += '.' + value.Name;
						Recurs(str, value.TargetVertexType, type_property_object, PATHShortDSF);
						str = temp;
					}
				}
			}

			foreach (IIncomingEdgeDefinition value in hyperEdgeInc)
			{
				if (PATHShortDSF.Find(x => x.Item2 == value.ID) != null)
				{
					if (type_property_object.Where(x => x.Type.ID == value.RelatedEdgeDefinition.SourceVertexType.ID).Count() > 0)
					{
						var temp = str;
						foreach (TypeWithProperty val in type_property_object.Where(x => x.Type.ID == value.RelatedEdgeDefinition.SourceVertexType.ID))
						{
				          _stringPath.Add(Tuple.Create(val.Type.Name + '.' + val.PropertyDefinition.Name, str + '.' + value.Name + '.' + val.PropertyDefinition.Name));
                        }
					str = temp + '.' + value.Name;
						Recurs(str, value.RelatedEdgeDefinition.SourceVertexType, type_property_object, PATHShortDSF);
						str = temp;
					}
					else
					{
						var temp = str;
						str += '.' + value.Name;
						Recurs(str, value.RelatedEdgeDefinition.SourceVertexType, type_property_object, PATHShortDSF);
						str = temp;
					}
				}
			}
			str = "";
			return null;
		} 

        #endregion

		#region ExecFunc

		public override FuncParameter ExecFunc(IAttributeDefinition myAttributeDefinition,
												Object myCallingObject,
												IVertex myDBObject, 
												IGraphDB myGraphDB, 
												SecurityToken mySecurityToken,
												Int64 myTransactionToken, 
												params FuncParameter[] myParams)
        {
            #region initialisation

            List<TypeWithProperty> ObjectList = new List<TypeWithProperty>();
			 
			var str =  myParams[0].Value.ToString();
			var method = Convert.ToBoolean(myParams[1].Value);

			if (str == "")
				throw new InvalidFunctionParameterException("Input String", "Input String is leer", "null");

			ObjectList = new TypeWithProperty().StringParser(str, myGraphDB, mySecurityToken, myTransactionToken);

			if (ObjectList.Count<2)
				throw new InvalidFunctionParameterException("Input String", "Input String has insufficient quantity objects", "null");

			List<Tuple<double, List<Tuple<long,long>>, IVertexType, IVertexType>> all = new List<Tuple<double, List<Tuple<long,long>>, IVertexType, IVertexType>>();
			Dictionary<Tuple<long, long>, List<List<Tuple<long, long>>>> allPath = new Dictionary<Tuple<long, long>, List<List<Tuple<long, long>>>>();
            #endregion
            #region search Path between two any VertexTypes
            foreach (TypeWithProperty vertexStart in ObjectList)
			{
				foreach (TypeWithProperty vertexEnd in ObjectList)
				{

					if (vertexStart != vertexEnd && vertexStart.Type.ID!=vertexEnd.Type.ID)
					{
						if (method)
						{
							var output = this.ShortPath(vertexStart.Type, vertexEnd.Type);
							if (output != null)
								all.Add(output);
						}
						else
						{
							List<Tuple<long, long>> value = new List<Tuple<long, long>>();
							value.Add(Tuple.Create(vertexStart.Type.ID, 0L));
							this.DFS(vertexStart.Type, vertexEnd.Type, value);
						}


						
					}

				}
			}
			#endregion
           
			
			if (!method)
			{ 
                #region DFS
                #region path->allPath->PATH for better representation
				foreach (List<Tuple<long, long>> value in _path)
				{
					if (!allPath.ContainsKey(Tuple.Create(value.First().Item1, value.Last().Item1)))
					{
						List<List<Tuple<long, long>>> temp = new List<List<Tuple<long, long>>>();
						temp.Add(value);
						allPath.Add(Tuple.Create(value.First().Item1, value.Last().Item1), temp);
					}
					else
					{
						allPath[Tuple.Create(value.First().Item1, value.Last().Item1)].Add(value);
					}
				}

				List<List<Tuple<long, long>>> PATH = new List<List<Tuple<long, long>>>();

				foreach (KeyValuePair<Tuple<long, long>, List<List<Tuple<long, long>>>> value in allPath)
				{
					var paths = value.Value;
					foreach (List<Tuple<long, long>> abc in paths)
					{
						PATH.Add(abc);
					}
				}

				if (PATH.Count < 1)
					throw new InvalidFunctionParameterException("PATH", "PATH with DFS not found", "null");

			#endregion
				#region all paths merge
				foreach (KeyValuePair<Tuple<long, long>, List<List<Tuple<long, long>>>> value in allPath)
				{
					var paths = value.Value;
					foreach (List<Tuple<long, long>> abc in paths)
					{
						var flag_ends = false;
						var iCount = 0;
						do
						{


							if (PATH.Count > 0)
							{
								var detekt = (PATH[iCount].Where(x => x.Item1 == value.Key.Item1).Count() > 0) && (PATH[iCount].Where(x => x.Item1 == value.Key.Item2).Count() > 0); //PATH[iCount].Contains(value.Key.Item2);

								if (!detekt)
								{

									var test = 0;
									foreach (long wert in ObjectList.Select(x => x.Type.ID))
									{
										if (PATH[iCount].Where(x => x.Item1 == wert).Count() > 0)
											test++;
									}


									if (test == ObjectList.Count)
										flag_ends = true;
									else
									{

										if (PATH[iCount].Contains(abc.Where(x => x.Item2 == 0).First()))
											PATH.Add(PATH[iCount].Union(abc).ToList());


									}
								}

							}
							else
							{
								flag_ends = true;
							}
							if (PATH.Count - 1 > iCount)
								iCount++;
							else
								flag_ends = true;
						}
						while (!flag_ends);

					}
				}

				#endregion
				#region redundance kill zone


				var jCount = 0;
				var flag_ends2 = false;
				do
				{


					var test = 0;
					test = 0;
					foreach (long value in ObjectList.Select(x => x.Type.ID))
					{
						if (PATH[jCount].Where(x => x.Item1 == value).Count() > 0)
							test++;
					}


					if (test < ObjectList.Count)
					{
						PATH.Remove(PATH[jCount]);
                        if (PATH.Count == 0 || PATH.Count == jCount)
							flag_ends2 = true;
					}
					else
					{
						if (PATH.Count - 1 > jCount)
							jCount++;
						else
							flag_ends2 = true;
					}
				}
				while (!flag_ends2);

				flag_ends2 = false;
				var iCount2 = 0;
				do
				{

					List<List<Tuple<long, long>>> delete = new List<List<Tuple<long, long>>>();

					foreach (List<Tuple<long, long>> wert in PATH)
					{
						var flag = true;
						var flagEnd = true;
						var jCountB = 0;
						if (PATH[iCount2].Count == wert.Count)
						{
							do
							{
								if (!wert.Contains(PATH[iCount2][jCountB]))
								{
									flag = false;
									flagEnd = false;
								}

								if (PATH[iCount2].Count - 1 > jCountB)
									jCountB++;
								else
									flagEnd = false;

							}
							while (flagEnd);
							if (flag)
								if (flag && !PATH[iCount2].Equals(wert))
									delete.Add(wert);
						}
					}
					
					if (delete.Count > 0)//(test > 1)
					{
						foreach (List<Tuple<long, long>> value in delete)
							PATH.Remove(value);
                        if (PATH.Count == 0 || PATH.Count == iCount2)
							flag_ends2 = true;
					}
					else
					{
						if (PATH.Count - 1 > iCount2)
							iCount2++;
						else
							flag_ends2 = true;
					}
				}
				while (!flag_ends2);

				if (PATH.Count < 1)
					throw new InvalidFunctionParameterException("PATH", "PATH with DFS not found", "null");

				var minPATH = PATH.Min(x => x.Count);
				var indexPATH = PATH.First(x => x.Count == minPATH);
			    this.Helper(myGraphDB, mySecurityToken, myTransactionToken, ObjectList, indexPATH);

			}
				#endregion
            #endregion
            else
            {
                #region Dijkstra
                #region all shortest paths between two any VertexTypes to PATHShort for better representation
                List<List<Tuple<long, long>>> PATHShort = new List<List<Tuple<long, long>>>();

				foreach (Tuple<double, List<Tuple<long, long>>, IVertexType, IVertexType> value in all)
				{

					PATHShort.Add(value.Item2);

				}
				if (PATHShort.Count < 1)
					throw new InvalidFunctionParameterException("PATH", "PATH with Dijkstra not found", "null");

				#endregion
				#region shortest paths merge

				foreach (Tuple<double, List<Tuple<long, long>>, IVertexType, IVertexType> value in all)
				{
					var abc = value.Item2;

					bool flag_ends = false;
					int iCount = 0;
					do
					{


						if (PATHShort.Count > 0)
						{
							var detekt = (PATHShort[iCount].Where(x => x.Item1 == value.Item3.ID).Count() > 0) && (PATHShort[iCount].Where(x => x.Item1 == value.Item4.ID).Count() > 0); //PATH[iCount].Contains(value.Key.Item2);

							if (!detekt)
							{

								var test = 0;
								foreach (long wert in ObjectList.Select(x => x.Type.ID))
								{
									if (PATHShort[iCount].Where(x => x.Item1 == wert).Count() > 0)
										test++;
								}


								if (test == ObjectList.Count)
									flag_ends = true;
								else
								{


									if (PATHShort[iCount].Contains(abc.Where(x => x.Item2 == 0).First()))
										PATHShort.Add(PATHShort[iCount].Union(abc).ToList());


								}
							}

						}
						else
						{
							flag_ends = true;
						}
						if (PATHShort.Count - 1 > iCount)
							iCount++;
						else
							flag_ends = true;
					}
					while (!flag_ends);

				}
				#endregion
				#region redudance kill zone for shortest paths
				var jCount = 0;
				var flag_ends2 = false;
				do
				{


					var test = 0;
					test = 0;
					foreach (long value in ObjectList.Select(x => x.Type.ID))
					{
						if (PATHShort[jCount].Where(x => x.Item1 == value).Count() > 0)
							test++;
					}


					if (test < ObjectList.Count)
					{
						PATHShort.Remove(PATHShort[jCount]);
						if (PATHShort.Count == 0||PATHShort.Count==jCount)
							flag_ends2 = true;

					}
					else
					{
						if (PATHShort.Count - 1 > jCount)
							jCount++;
						else
							flag_ends2 = true;
					}
				}
				while (!flag_ends2);


				flag_ends2 = false;
				var iCount2 = 0;
				do
				{

					List<List<Tuple<long, long>>> delete = new List<List<Tuple<long, long>>>();
					foreach (List<Tuple<long, long>> wert in PATHShort)
					{
						var flag = true;
						var flagEnd = true;
						var jCountB = 0;
						if (PATHShort[iCount2].Count == wert.Count)
						{
							do
							{
								if (!wert.Contains(PATHShort[iCount2][jCountB]))
								{
									flag = false;
									flagEnd = false;
								}

								if (PATHShort[iCount2].Count - 1 > jCountB)
									jCountB++;
								else
									flagEnd = false;

							}
							while (flagEnd);
							if (flag && !PATHShort[iCount2].Equals(wert))
								delete.Add(wert);
						}

				
					}
					if (delete.Count > 0)//(test > 1)
					{
						foreach (List<Tuple<long, long>> value in delete)
							PATHShort.Remove(value);
                        if (PATHShort.Count == 0 || PATHShort.Count == iCount2)
							flag_ends2 = true;
					}
					else
					{
						if (PATHShort.Count - 1 > iCount2)
							iCount2++;
						else
							flag_ends2 = true;
					}
				}
				while (!flag_ends2);

                if (PATHShort.Count < 1)
                    throw new InvalidFunctionParameterException("PATH", "PATH with Dijkstra not found", "null");

				var minPATHShort = PATHShort.Min(x => x.Count);
				var indexPATHShort = PATHShort.First(x => x.Count == minPATHShort);
			       this.Helper(myGraphDB, mySecurityToken, myTransactionToken, ObjectList,indexPATHShort);
			}
			#endregion
                #endregion
            #region Output
           
		
           var result = new ListCollectionWrapper(_stringPath.Select(x => x.Item1+" = "+ x.Item2));
            this._path.Clear();
            this._stringPath.Clear();
			return new FuncParameter(result);
			 #endregion
		}
		#endregion

        #region IPluginable

		public override string PluginName
		{
            get { return "sones.typesconnect"; }
		}

		public override PluginParameters<Type> SetableParameters
		{
			get { return new PluginParameters<Type>(); }
		}

		public override IPluginable InitializePlugin(String myUniqueString, Dictionary<string, object> myParameters = null)
		{
			return new TypesConnect();
		}

		public override string PluginShortName
		{
			get { return "typesconnect"; }
		}

		public override string PluginDescription
		{
            get { return "typesconnect is graph algorithm plugin"; }
		}

		public override void Dispose()
		{ }
		
		#endregion

		#region IGQLFunction

		public override Type GetReturnType()
		{
			return typeof(IVertexView);
		}

		#endregion
	}
 }
