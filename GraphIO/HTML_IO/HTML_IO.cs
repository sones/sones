/* 
 * HTML_IO
 * Achim 'ahzf' Friedland, 2009-2010
 */

#region Usings

using System;
using System.Text;
using System.Linq;
using System.Net.Mime;
using System.Collections.Generic;

using sones.GraphDB.TypeManagement;


using sones.GraphFS.DataStructures;
using sones.GraphFS.Objects;

using sones.Lib;
using sones.GraphDB.Result;
using sones.GraphDB.NewAPI;

#endregion

namespace sones.GraphIO.HTML
{

    /// <summary>
    /// Transforms a QueryResult and a DBObjectReadout into a text/html representation
    /// </summary>

    public class HTML_IO : IObjectsExport
    {

        #region Data

        private readonly ContentType _ExportContentType;

        #endregion

        #region Constructor

        public HTML_IO()
        {
            _ExportContentType = new ContentType("text/html") { CharSet = "UTF-8" };
        }

        #endregion


        #region IGraphExport Members

        #region ExportContentType

        public ContentType ExportContentType
        {
            get
            {
                return _ExportContentType;
            }
        }

        #endregion


        #region ExportQueryResult(myQueryResult)

        public Byte[] ExportQueryResult(QueryResult myQueryResult)
        {
            return Encoding.UTF8.GetBytes(myQueryResult.ToHTML());
        }

        #endregion


        #region ExportVertex(myVertex)

        public Object ExportVertex(Vertex myVertex)
        {
            return myVertex.ToHTML();
        }

        #endregion

        #endregion


        #region IObjectsExport Members

        public Byte[] ExportINode(INode myINode)
        {
            throw new NotImplementedException();
        }

        public Byte[] ExportObjectLocator(ObjectLocator myObjectLocator)
        {
            throw new NotImplementedException();
        }

        public Byte[] ExportAFSObject(AFSObject myAFSObject)
        {
            throw new NotImplementedException();
        }

        #endregion

    }

}
