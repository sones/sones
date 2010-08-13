/* 
 * TEXT_IO
 * Achim 'ahzf' Friedland, 2009-2010
 */

#region Usings

using System;
using System.Text;
using System.Linq;
using System.Net.Mime;
using System.Collections.Generic;

using sones.GraphDB.TypeManagement;
using sones.GraphDB.Structures.Result;
using sones.GraphFS.DataStructures;

#endregion

namespace sones.GraphIO.TEXT
{

    /// <summary>
    /// Transforms a QueryResult and a DBObjectReadout into a text/plain representation
    /// </summary>

    public class TEXT_IO : IGraphDBExport
    {

        #region Data

        private readonly ContentType _ExportContentType;

        #endregion

        #region Constructor

        public TEXT_IO()
        {
            _ExportContentType = new ContentType("text/plain") { CharSet = "UTF-8" };
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
            return Encoding.UTF8.GetBytes(myQueryResult.ToTEXT().ToString());
        }

        #endregion


        #region ExportVertex(myDBVertex)

        public Object ExportVertex(DBObjectReadout myDBVertex)
        {
            return myDBVertex.ToTEXT();
        }

        #endregion

        #endregion

    }

}
