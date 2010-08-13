/* 
 * MediaRSS_IO
 * Achim 'ahzf' Friedland, 2009-2010
 */

#region Usings

using System;
using System.Text;
using System.Net.Mime;

using sones.GraphDB.Structures.Result;

#endregion

namespace sones.GraphIO.MediaRSS
{

    /// <summary>
    /// Transforms a QueryResult and a DBObjectReadout into a text/html representation
    /// </summary>

    public class MediaRSS_IO : IGraphDBExport
    {

        #region Data

        private readonly ContentType _ExportContentType;

        #endregion

        #region Constructor

        public MediaRSS_IO()
        {
            _ExportContentType = new ContentType("application/media-rss") { CharSet = "UTF-8" };
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
            return Encoding.UTF8.GetBytes(myQueryResult.ToMediaRSS());
        }

        #endregion


        #region ExportVertex(myDBVertex)

        public Object ExportVertex(DBObjectReadout myDBVertex)
        {
            return myDBVertex.ToMediaRSS();
        }

        #endregion

        #endregion

    }

}
