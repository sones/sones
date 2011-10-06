using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Description;
using System.Xml.Schema;
using ServiceDescription = System.Web.Services.Description.ServiceDescription;
using System.Xml.Serialization;
using System.Web.Services.Description;

namespace WCFExtras.Wsdl
{
    class SingleFileExporter
    {
        internal static void ExportEndpoint(WsdlExporter wsdlExporter)
        {
            if (wsdlExporter.GeneratedWsdlDocuments.Count > 1)
                throw new ApplicationException("Single file option is not supported in multiple wsdl files");

            ServiceDescription rootDescription = wsdlExporter.GeneratedWsdlDocuments[0];
            XmlSchemas imports = new XmlSchemas();
            foreach (XmlSchema schema in wsdlExporter.GeneratedXmlSchemas.Schemas())
            {
                imports.Add(schema);
            }
            foreach (XmlSchema schema in imports)
            {
                schema.Includes.Clear();
            }

            rootDescription.Types.Schemas.Clear();
            rootDescription.Types.Schemas.Add(imports);
        }
    }
}
