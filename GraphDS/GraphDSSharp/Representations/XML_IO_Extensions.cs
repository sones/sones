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
 * XML_IO_Extensions
 * Achim Friedland, 2009 - 2010
 */

#region Usings

using System;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;

using sones.Lib;
using sones.GraphDB.QueryLanguage.Result;
using System.IO;
using System.Xml;
using System.Text;

#endregion

namespace sones.GraphDS.API.CSharp
{

    public static class XML_IO_Extensions
    {

        #region ToXML(this myQueryResult)

        public static XElement ToXML(this QueryResult myQueryResult)
        {
            return new XML_IO().Export(myQueryResult) as XElement;
        }

        #endregion

        #region ToXML(this myDBObjectReadout)

        private static XElement ToXML(this DBObjectReadout myDBObjectReadout)
        {
            return new XML_IO().Export(myDBObjectReadout) as XElement;
        }

        #endregion


        #region BuildXMLDocument(params myXElements)

        public static XDocument BuildXMLDocument(params XElement[] myXElements)
        {

            var _XMLDocument = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"));

            var _Sones       = new XElement("sones",   new XAttribute("version", "1.0"));
            var _GraphDB     = new XElement("GraphDB", new XAttribute("version", "1.0"));

            foreach (var _XElement in myXElements)
                _GraphDB.Add(_XElement);

            _Sones.Add(_GraphDB);
            _XMLDocument.Add(_Sones);

            return _XMLDocument;

        }

        #endregion

        #region XDocument2String(this myXDocument)

        public static String XDocument2String(this XDocument myXDocument)
        {

         //   var _StringWriter = new StringWriter();

         //   var _XmlWriterSettings = new XmlWriterSettings()
         //   {
         //       Encoding         = Encoding.UTF8,
         //   //    ConformanceLevel = ConformanceLevel.Document,
         //   //    Indent           = true
         //   };

         //   var writer = XmlWriter.Create(_StringWriter, _XmlWriterSettings);
         //   myXDocument.Save(writer);

         //   return _StringWriter.GetStringBuilder().ToString();

            var _String = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" + Environment.NewLine;
            _String += myXDocument.ToString();

            return _String;

        }

        #endregion

    }

}
