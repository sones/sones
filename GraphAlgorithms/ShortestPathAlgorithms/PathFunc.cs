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
#region usings

using System;
using System.Collections.Generic;
using System.Linq;
using GraphAlgorithms.PathAlgorithm.BreadthFirstSearch;
using sones.GraphDB.Exceptions;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.QueryLanguage.Enums;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Functions;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.TypeManagement.PandoraTypes;
using sones.Lib.DataStructures;
using sones.Lib.ErrorHandling;

using sones.GraphDB.TypeManagement;
using sones.Lib.DataStructures.UUID;
using sones.GraphDB.Errors;
using sones.GraphFS.DataStructures;
using sones.GraphFS.Session;
using sones.Lib.Session;

#endregion

namespace sones.GraphDB
{
    public class GraphFunc : ABaseFunction
    {
        public override string FunctionName
        {
            get { return "PATH"; }
        }

        #region GetDescribeOutput()

        public override String GetDescribeOutput()
        {
            return "A path algorithm.";
        }

        #endregion

        public GraphFunc()
        {
            /// these are the starting edges and TypeAttribute.
            /// This is not the starting DBObject but just the content of the attribute defined by TypeAttribute!!!
            Parameters.Add(new ParameterValue("TargetDBO", new DBEdge()));
            Parameters.Add(new ParameterValue("MaxDepth", new DBInt64()));
            Parameters.Add(new ParameterValue("MaxPathLength", new DBInt64()));
            Parameters.Add(new ParameterValue("OnlyShortestPath", new DBBoolean()));
            Parameters.Add(new ParameterValue("AllPaths", new DBBoolean()));
        }

        public override bool ValidateWorkingBase(TypeAttribute workingBase, DBTypeManager typeManager)
        {
            if (workingBase != null)
            {
                if (workingBase.TypeCharacteristics.IsBackwardEdge)
                {
                    return true;
                }
                else
                {
                    if (workingBase.GetDBType(typeManager).IsUserDefined)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }

        public override Exceptional<FuncParameter> ExecFunc(DBContext dbContext, params FuncParameter[] myParams)
        {
            // The edge we starting of (e.g. Friends)
            var typeAttribute = CallingAttribute;

            // The destination DBObjects, they are of the type "typeAttribute.RelatedDBType"
            var destDBOs = (myParams[0].Value as DBEdge).GetDBObjects();

            byte maxDepth = Convert.ToByte((myParams[1].Value as DBInt64).GetValue());

            byte maxPathLength = Convert.ToByte((myParams[2].Value as DBInt64).GetValue());

            //values incorrect
            if (maxDepth < 1 && maxPathLength < 2)
            {
                Exceptional<FuncParameter> errorResult = new Exceptional<FuncParameter>();
                IError error = new Error_InvalidFunctionParameter("maxDepth", ">= 1", maxDepth);
                errorResult.Push(error);
                error = new Error_InvalidFunctionParameter("maxPathLength", ">= 2", maxPathLength);
                errorResult.Push(error);

                return errorResult;
            }
            else if (maxDepth < 1)
            {
                return new Exceptional<FuncParameter>(new Error_InvalidFunctionParameter("maxDepth", ">= 1", maxDepth));
            }
            else if (maxPathLength < 2)
            {
                return new Exceptional<FuncParameter>(new Error_InvalidFunctionParameter("maxPathLength", ">= 2", maxPathLength));
            }

            bool onlyShortestPath = (myParams[3].Value as DBBoolean).GetValue();

            bool allPaths = (myParams[4].Value as DBBoolean).GetValue();

            if (!onlyShortestPath && !allPaths)
            {
                allPaths = true;
            }

            #region Call graph function

            if (destDBOs.Count() != 1)
                throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));

            var dbObject = destDBOs.First();
            if (dbObject.Failed)
            {
                throw new GraphDBException(dbObject.Errors);
            }

            HashSet<List<ObjectUUID>> paths;

            if (onlyShortestPath && allPaths) //use bi-directional search for "all shortest paths"
            {
                //normal BFS
                //paths = new BreadthFirstSearch().Find(typeAttribute, typeManager, cache, mySourceDBObject, dbObject, onlyShortestPath, allPaths, maxDepth, maxPathLength);

                //bidirectional BFS
                paths = new BidirectionalBFS().Find(typeAttribute, dbContext, CallingDBObjectStream as DBObjectStream, dbObject.Value, onlyShortestPath, allPaths, maxDepth, maxPathLength);
            }
            else //use uni-directional search for "shortest path and all paths up to given depth"
            {
                //normal BFS
                //paths = new BreadthFirstSearch().Find(typeAttribute, dbContext.DBTypeManager, dbContext.DBObjectCache, CallingDBObjectStream as DBObjectStream, dbObject.Value, onlyShortestPath, allPaths, maxDepth, maxPathLength);

                //bidirectional BFS
                paths = new BidirectionalBFS().Find(typeAttribute, dbContext, CallingDBObjectStream as DBObjectStream, dbObject.Value, onlyShortestPath, allPaths, maxDepth, maxPathLength);
            }

            //This variable will be returned
            Exceptional<FuncParameter> pResult = new Exceptional<FuncParameter>();
            
            if (paths != null)
            {
                pResult.Value = new FuncParameter(new EdgeTypePath(paths, typeAttribute), typeAttribute);
            }
            else
            {
                return new Exceptional<FuncParameter>(new FuncParameter(new EdgeTypePath(new HashSet<List<ObjectUUID>>(), typeAttribute), typeAttribute));
            }

            #endregion

            return pResult;
        }
    }
}
