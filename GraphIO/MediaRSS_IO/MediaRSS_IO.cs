/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
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
* 
*/

/* 
 * MediaRSS_IO
 * Achim 'ahzf' Friedland, 2009-2010
 */

#region Usings

using System;
using System.Text;
using System.Net.Mime;


using sones.GraphDB.Result;
using sones.GraphDB.NewAPI;

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


        #region ExportVertex(myVertex)

        public Object ExportVertex(Vertex myVertex)
        {
            return myVertex.ToMediaRSS();
        }

        #endregion

        #endregion

    }

}
