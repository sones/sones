/* 
 * GEXF_IO_Extensions
 * Achim 'ahzf' Friedland, 2010
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

    /// <summary>
    /// Extension methods to transform a QueryResult and a DBObjectReadout into an
    /// application/gexf representation an vice versa.
    /// </summary>

    public static class GEXF_IO_Extensions
    {

        #region ToGEXF(this myQueryResult)

        public static XElement ToGEXF(this QueryResult myQueryResult)
        {
            return new GEXF_IO().Export(myQueryResult) as XElement;
        }

        #endregion

        #region ToGEXF(this myDBObjectReadout)

        private static XElement ToGEXF(this DBObjectReadout myDBObjectReadout)
        {
            return new GEXF_IO().Export(myDBObjectReadout) as XElement;
        }

        #endregion


        #region BuildGEXFDocument(params myXElements)

        public static XDocument BuildGEXFDocument(params XElement[] myXElements)
        {

            //<gexf xmlns="http://www.gexf.net/1.1draft"
            //      xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
            //      xsi:schemaLocation="http://www.gexf.net/1.1draft http://www.gexf.net/1.1draft/gexf.xsd"
            //      version="1.1">
            //    <meta lastmodifieddate="2009-03-20">
            //        <creator>Gexf.net</creator>
            //        <description>A hello world! file</description>
            //    </meta>
            //    <graph mode="static" defaultedgetype="directed">
            //     ...
            //    </graph>
            //</gexf>

            var _GEXFDocument = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"));

            var _GEXF         = new XElement("gexf",
                                        //              new XAttribute("xmlns",              "http://www.gexf.net/1.1draft"),
                                        //              new XAttribute(XNamespace.Xmlns + "xsi",          "http://www.w3.org/2001/XMLSchema-instance"),
                                        //              new XAttribute(XNamespace.Xmlns + "xsi" + "schemaLocation", "http://www.gexf.net/1.1draft http://www.gexf.net/1.1draft/gexf.xsd"),
                                                      new XAttribute("version", "1.1"));
            
            var _Meta         = new XElement("meta",  new XAttribute("lastmodifieddate", DateTime.Now),
                                                      new XElement("creator",              "sones GraphDS"),
                                                      new XElement("description",          "sones GEXF output plug-in"));
            
            var _Graph        = new XElement("graph", new XAttribute("mode", "static"), // really important?
                                                      new XAttribute("defaultedgetype", "directed"));

            foreach (var _XElement in myXElements)
                _Graph.Add(_XElement);

            _GEXF.Add(_Meta);
            _GEXF.Add(_Graph);
            _GEXFDocument.Add(_GEXF);

            return _GEXFDocument;

        }

        #endregion

        #region GEXFDocument2String(this myXDocument)

        public static String GEXFDocument2String(this XDocument myXDocument)
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
