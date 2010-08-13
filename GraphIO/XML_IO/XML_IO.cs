/* 
 * XML_IO
 * Achim 'ahzf' Friedland, 2009-2010
 */

#region Usings

using System;
using System.Linq;
using System.Net.Mime;
using System.Xml.Linq;

using sones.GraphDB.Structures.Result;
using sones.GraphFS.DataStructures;
using sones.GraphFS.Objects;
using System.Text;

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
            return Encoding.UTF8.GetBytes(XML_IO_Extensions.BuildXMLDocument(myQueryResult.ToXML()).XMLDocument2String());
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
