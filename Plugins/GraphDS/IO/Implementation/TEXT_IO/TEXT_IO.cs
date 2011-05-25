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
using System.Net.Mime;
using sones.Library.VersionedPluginManager;
using sones.Library.Settings;
using sones.GraphQL.Result;
using System.IO;
using sones.Library.CollectionWrapper;

namespace sones.Plugins.GraphDS.IO
{
    public sealed class TEXT_IO : IOInterface
    {

        #region Data

        private readonly ContentType _contentType;

        #endregion

        #region Constructors

        public TEXT_IO()
        {
            _contentType = new ContentType("text/plain") { CharSet = "UTF-8" };
        }

        #endregion

        #region IPluginable

        public string PluginName
        {
            get { return "sones.text_io"; }
        }

        public Dictionary<string, Type> SetableParameters
        {
            get { return new Dictionary<string, Type>(); }
        }
/*
 * ASK: whats this?
        public IPluginable InitializePlugin(Dictionary<string, object> myParameters, GraphApplicationSettings myApplicationSetting)
        {
            return InitializePlugin();
        }
*/
        public IPluginable InitializePlugin(String myUniqueString, Dictionary<string, object> myParameters = null)
        {
            var result = new TEXT_IO();

            return (IPluginable)result;
        }

        #endregion

        #region IOInterface

        #region Generate Output from Query Result

        public string GenerateOutputResult(QueryResult myQueryResult)
        {
            StringBuilder Output = new StringBuilder();
            Output.AppendLine("Query:\t\t"+myQueryResult.Query);
            Output.AppendLine("Result:\t\t"+myQueryResult.TypeOfResult.ToString());
            Output.AppendLine("Duration:\t"+myQueryResult.Duration+" ms");

            if (myQueryResult.Error != null)
            {
                Output.AppendLine("Error: \t\t"+myQueryResult.Error.GetType().ToString() + " - " + HandleQueryExceptions(myQueryResult));
            }

            if (myQueryResult.Vertices != null)
            {
                Output.AppendLine("Vertices:");

                foreach (IVertexView _vertex in myQueryResult.Vertices)
                {
                    Output.Append(GenerateVertexViewText("\t\t",_vertex));
                }
            }

            return Output.ToString();
        }

        private String HandleQueryExceptions(QueryResult queryresult)
        {
            StringBuilder SB = new StringBuilder();

            SB.Append(queryresult.Error.ToString());
            if (queryresult.Error.InnerException != null)
                SB.Append(" InnerException: " + queryresult.Error.InnerException.Message);

            return SB.ToString();
        }

        #region private toHTML
        private String GenerateVertexViewText(String Header, IVertexView aVertex)
        {
            StringBuilder Output = new StringBuilder();
            // take one IVertexView and traverse through it
            #region Vertex Properties
            Output.AppendLine();
            if (aVertex.GetCountOfProperties() > 0)
            {
                foreach (var _property in aVertex.GetAllProperties())
                {
                    if (_property.Item2 == null)
                    {
                        Output.AppendLine(Header + _property.Item1);
                    }
                    else
                    {
                        if (_property.Item2 is Stream)
                        {
                            Output.AppendLine(Header + _property.Item1 + "\t BinaryProperty");
                        }
                        else
                        {
                            if (_property.Item2 is ICollectionWrapper)
                            {
                                Output.AppendLine(Header + _property.Item1);

                                foreach (var item in ((ICollectionWrapper)_property.Item2))
                                {
                                    Output.AppendLine(Header + "\t " + item.ToString());
                                }
                            }
                            else
                            {
                                Output.AppendLine(Header + _property.Item1 + "\t " + _property.Item2.ToString());
                            }
                        }
                    }
                }
            }
            #endregion

            #region Edges
            Output.AppendLine(Header + "\t Edges:");
            foreach (var _edge in aVertex.GetAllEdges())
            {
                if (_edge.Item2 == null)
                {
                    Output.AppendLine(Header+"\t\t"+_edge.Item1);
                }
                else
                {
                    Output.AppendLine(Header+"\t\t"+_edge.Item2.GetType().Name);
                    Output.AppendLine(Header+"\t\t"+_edge.Item1).Append(GenerateEdgeViewText(Header+"\t\t\t",_edge.Item2));
                }
            }
            #endregion

            return Output.ToString();
        }

        private String GenerateEdgeViewText(String Header, IEdgeView aEdge)
        {
            StringBuilder Output = new StringBuilder();

            #region Edge Properties
            Output.AppendLine(Header + "\t Edge");

            Output.AppendLine(Header + "\t Properties");
            if (aEdge.GetCountOfProperties() > 0)
            {
                foreach (var _property in aEdge.GetAllProperties())
                {
                    if (_property.Item2 == null)
                        Output.AppendLine(Header+"\t\t"+_property.Item1);
                    else
                        if (_property.Item2 is Stream)
                        {
                            Output.AppendLine(Header + "\t\t"+_property.Item1+"\t BinaryProperty");
                        }
                        else
                            Output.AppendLine(Header + "\t\t"+_property.Item1 + "\t " + _property.Item2.ToString());
                }
            }
            #endregion

            if (aEdge is IHyperEdgeView)
            {                
                foreach (var singleEdge in ((IHyperEdgeView)aEdge).GetAllEdges())
                {
                    Output.AppendLine(Header + "\t SingleEdge");
                    Output.AppendLine(Header + "\t\t\tProperties");
                    foreach (var _property in singleEdge.GetAllProperties())
                    {
                        if (_property.Item2 == null)
                        {
                            Output.AppendLine(Header + "\t\t\t\t " + _property.Item1);
                        }
                        else
                        {
                            if (_property.Item2 is Stream)
                            {
                                Output.AppendLine(Header + "\t\t\t\t " + _property.Item1 + "\t\t\t BinaryProperty");
                            }
                            else
                            {
                                Output.AppendLine(Header + "\t\t\t\t " + _property.Item1 + "\t\t " + _property.Item2.ToString());
                            }
                        }
                    }

                    if (singleEdge.GetTargetVertex() != null)
                    {
                        Output.AppendLine(Header + "\t\t\tTargetVertex");
                        Output.Append(GenerateVertexViewText(Header + "\t\t\t", singleEdge.GetTargetVertex()));
                    }                    
                }
            }
            else
            {
                if (((ISingleEdgeView)aEdge).GetTargetVertex() != null)
                {
                    Output.AppendLine(Header + "\t\t\t\tTargetVertex");
                    Output.Append(GenerateVertexViewText(Header + "\t\t\t", ((ISingleEdgeView)aEdge).GetTargetVertex()));
                }

            }

            return Output.ToString();
        }


        #endregion

        #region Generate a QueryResult from Text - not really needed right now
        public QueryResult GenerateQueryResult(string myResult)
        {
            throw new NotImplementedException();
        }

        public ContentType ContentType
        {
            get { return _contentType; }
        }
        #endregion

        #region HTMLBuilder(myGraphDBName, myFunc)

        #endregion

        #endregion

        #endregion

    }

}
