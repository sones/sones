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
 * XML_IO
 * Achim 'ahzf' Friedland, 2009 - 2010
 */

#region Usings

using System;
using System.Linq;
using System.Net.Mime;
using System.Xml.Linq;


using sones.GraphFS.DataStructures;
using sones.GraphFS.Objects;
using System.Text;
using sones.GraphDBInterface.Result;
using System.Xml;
using System.IO;

#endregion

namespace sones.GraphIO.XML
{

    /// <summary>
    /// Transforms all graph objects into an application/xml
    /// representation and vice versa.
    /// </summary>

    public class XML_IO : IObjectsIO
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
            var xDoc = XML_IO_Extensions.BuildXMLDocument(myQueryResult.ToXML());

            MemoryStream ms = new MemoryStream();

            xDoc.Save(ms, SaveOptions.DisableFormatting);

            return ms.ToArray();
        }

        #endregion


        #region ExportVertex(myDBVertex)

        public Object ExportVertex(DBObjectReadout myDBVertex)
        {
            return myDBVertex.ToXML();
        }

        #endregion

        #endregion


        #region IGraphImport Members

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


        #region IObjectsIO Members

        #region ExportINode(myINode)

        public Byte[] ExportINode(INode myINode)
        {
            return new UTF8Encoding().GetBytes(myINode.ToXML().ToString());
        }

        #endregion

        #region ExportObjectLocator(myObjectLocator)

        public Byte[] ExportObjectLocator(ObjectLocator myObjectLocator)
        {
            return new UTF8Encoding().GetBytes(myObjectLocator.ToXML().ToString());
        }

        #endregion

        #region ExportAFSObject(myAFSObject)

        public Byte[] ExportAFSObject(AFSObject myAFSObject)
        {
            return null;
        }

        #endregion

        #region ExportIXML_IO(myIXML_IO)

        public Byte[] ExportIXML_IO(IXML_IO myIXML_IO)
        {
            return new UTF8Encoding().GetBytes(myIXML_IO.ToXML().ToString());
        }

        #endregion


        #region ImportINode(mySerializedINode)

        public INode ImportINode(Byte[] mySerializedINode)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ImportObjectLocator(mySerializedObjectLocator)

        public ObjectLocator ImportObjectLocator(Byte[] mySerializedObjectLocator)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion

    }

}
