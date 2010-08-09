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

/* 
 * XML_IO
 * Achim Friedland, 2009 - 2010
 */

#region Usings

using System;
using System.Net.Mime;
using System.Xml.Linq;

using sones.GraphDB.Structures.Result;

#endregion

namespace sones.GraphDS.API.CSharp
{

    /// <summary>
    /// Transforms a QueryResult and a DBObjectReadout into an application/xml
    /// representation and vice versa.
    /// </summary>

    public class XML_IO : IDBExport, IDBImport
    {

        #region Data

        private readonly ContentType _ExportContentType;
        private readonly ContentType _ImportContentType;

        #endregion

        #region Constructor

        public XML_IO()
        {
            _ExportContentType = new ContentType("application/xml") { CharSet = "UTF-8" };
            _ImportContentType = new ContentType("application/xml");
        }

        #endregion


        #region IDBExport Members

        #region ExportContentType

        public ContentType ExportContentType
        {
            get
            {
                return _ExportContentType;
            }
        }

        #endregion

        #region Export(myQueryResult)

        public Object Export(QueryResult myQueryResult)
        {
            return myQueryResult.ToXML();
        }

        #endregion

        #region Export(myDBObjectReadout)

        public Object Export(DBObjectReadout myDBObjectReadout)
        {
            return myDBObjectReadout.ToXML();
        }

        #endregion

        #region ExportString(myQueryResult)

        public String ExportString(QueryResult myQueryResult)
        {
            return XML_IO_Extensions.BuildXMLDocument(Export(myQueryResult) as XElement).XMLDocument2String();
        }

        #endregion

        #endregion


        #region IDBImport Members

        #region ImportContentType

        public ContentType ImportContentType
        {
            get
            {
                return _ImportContentType;
            }
        }

        #endregion

        #region ParseQueryResult(myInput)

        public QueryResult ParseQueryResult(String myInput)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ParseDBObject(myInput)

        public DBObjectReadout ParseDBObject(String myInput)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion

    }

}
