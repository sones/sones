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
 * JSON_IO
 * Achim 'ahzf' Friedland, 2009 - 2010
 */

#region Usings

using System;
using System.Linq;
using System.Net.Mime;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using sones.GraphDB.TypeManagement;

using sones.GraphDB.ObjectManagement;
using sones.GraphFS.DataStructures;
using sones.GraphFS.Objects;
using System.Text;
using sones.GraphDB.Result;
using sones.Lib.ErrorHandling;
using sones.GraphDB.NewAPI;

#endregion

namespace sones.GraphIO.JSON
{

    /// <summary>
    /// Transforms all graph objects into an application/json
    /// representation and vice versa.
    /// </summary>

    public class JSON_IO : IObjectsIO
    {

        #region Data

        private readonly ContentType _ExportContentType;
        private readonly ContentType _ImportContentType;

        #endregion

        #region Constructor

        public JSON_IO()
        {
            _ExportContentType = new ContentType("application/json") { CharSet = "UTF-8" };
            _ImportContentType = new ContentType("application/json");
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
            return Encoding.UTF8.GetBytes(myQueryResult.ToJSON().ToString(Formatting.Indented));
        }

        #endregion


        #region ExportVertex(myVertex)

        public Object ExportVertex(Vertex myVertex)
        {
            return myVertex.ToJSON();
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

        public Vertex ParseVertex(String myInput)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion


        #region IObjectsIO Members

        #region ExportINode(myINode)

        public Byte[] ExportINode(INode myINode)
        {
            return new UTF8Encoding().GetBytes(myINode.ToJSON().ToString(Formatting.Indented));
        }

        #endregion

        #region ExportObjectLocator(myObjectLocator)

        public Byte[] ExportObjectLocator(ObjectLocator myObjectLocator)
        {
            return null;
        }

        #endregion

        #region ExportAFSObject(myAFSObject)

        public Byte[] ExportAFSObject(AFSObject myAFSObject)
        {
            return null;
        }

        #endregion

        #region ExportIJSON_IO(myIJSON_IO)

        public Byte[] ExportIJSON_IO(IJSON_IO myIJSON_IO)
        {
            return new UTF8Encoding().GetBytes(myIJSON_IO.ToJSON().ToString());
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


        #region GenerateUnspecifiedWarning(myWarningXElement)

        /// <summary>
        /// Generates an UnspecifiedWarning from its XML representation
        /// </summary>
        /// <param name="myErrorXML">The XML representation of an UnspecifiedError</param>
        public UnspecifiedWarning GenerateUnspecifiedWarning(Object myWarningXElement)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region GenerateUnspecifiedError(myErrorXElement)

        /// <summary>
        /// Generates an UnspecifiedError from its XML representation
        /// </summary>
        /// <param name="myErrorXML">The XML representation of an UnspecifiedError</param>
        public UnspecifiedError GenerateUnspecifiedError(Object myErrorXElement)
        {
            throw new NotImplementedException();
        }

        #endregion

        
    }

}
