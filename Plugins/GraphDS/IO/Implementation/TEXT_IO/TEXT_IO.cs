using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mime;
using sones.Library.VersionedPluginManager;
using sones.Library.Settings;
using sones.GraphQL.Result;
using System.IO;

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

        public IPluginable InitializePlugin(Dictionary<string, object> myParameters, GraphApplicationSettings myApplicationSetting)
        {
            return InitializePlugin();
        }

        public IPluginable InitializePlugin(Dictionary<string, object> myParameters = null)
        {
            object result = Activator.CreateInstance(typeof(TEXT_IO));

            return (IPluginable)result;
        }

        #endregion

        #region IOInterface

        #region Generate Output from Query Result

        public string GenerateOutputResult(QueryResult myQueryResult)
        {
            StringBuilder Output = new StringBuilder();
            Output.AppendLine("query:\t\t"+myQueryResult.Query);
            Output.AppendLine("result:\t\t"+myQueryResult.TypeOfResult.ToString());
            Output.AppendLine("duration:\t"+myQueryResult.Duration+" ms");

            if (myQueryResult.Error != null)
            {
                Output.AppendLine("error: \t\t"+myQueryResult.Error.GetType().ToString() + " - " + myQueryResult.Error.ToString());
            }

            if (myQueryResult.Vertices != null)
            {
                Output.AppendLine("vertices:");

                foreach (IVertexView _vertex in myQueryResult.Vertices)
                {
                    Output.Append(GenerateVertexViewText("\t\t",_vertex));
                }
            }

            return Output.ToString();
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
                        Output.AppendLine(Header+_property.Item1);
                    else
                        if (_property.Item2 is Stream)
                            Output.AppendLine(Header+_property.Item1+"\t BinaryProperty");
                        else
                            Output.AppendLine(Header+_property.Item1+"\t "+_property.Item2.ToString());
                }
            }
            #endregion

            #region Edges
            Output.AppendLine(Header + "\t edges:");
            foreach (var _edge in aVertex.GetAllEdges())
            {
                if (_edge.Item2 == null)
                {
                    Output.AppendLine(Header+"\t\t"+_edge.Item1);
                }
                else
                {
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
            if (aEdge.GetCountOfProperties() > 0)
            {
                Output.AppendLine(Header+"\t edges");
                foreach (var _property in aEdge.GetAllProperties())
                {
                    if (_property.Item2 == null)
                        Output.AppendLine(Header+"\t\t"+_property.Item1);
                    else
                        if (_property.Item2 is Stream)
                        {
                            Output.AppendLine(Header + _property.Item1+"\t BinaryProperty");
                        }
                        else
                            Output.AppendLine(Header + _property.Item1 + "\t " + _property.Item2.ToString());
                }
            }
            #endregion

            #region Target Vertices
            Output.AppendLine(Header + "\t targetvertices");
            foreach (IVertexView _vertex in aEdge.GetTargetVertices())
            {
                Output.Append(GenerateVertexViewText(Header+"\t\t",_vertex));
            }
            #endregion

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
