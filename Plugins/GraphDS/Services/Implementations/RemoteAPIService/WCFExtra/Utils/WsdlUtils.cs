using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Services.Description;
using System.Xml.Schema;
using System.Xml;

namespace WCFExtras.Utils
{
    static class WsdlUtils
    {
        public static ServiceDescription FindRootDescription(ServiceDescriptionCollection wsdls)
        {
            ServiceDescription rootDescription = null;

            //Find the "root" service description
            foreach (ServiceDescription description in wsdls)
            {
                if (description.Services.Count > 0)
                {
                    rootDescription = description.Services[0].ServiceDescription;
                    break;
                }
            }
            return rootDescription;
        }

        private static void EnumerateDocumentedItems(XmlSchemaObjectCollection schemaItems, Dictionary<string, string> documentedItems)
        {
            foreach (XmlSchemaObject schemaObj in schemaItems)
            {
                string documentation = GetDocumenation(schemaObj);
                if (documentation != null)
                {
                    string uniqueName = GetUniqueName(schemaObj);
                    documentedItems[uniqueName] = documentation;
                }
                EnumerateDocumentedItems(schemaObj, documentedItems);
            }
        }

        private static void EnumerateDocumentedItems(XmlSchemaObject schemaObj, Dictionary<string, string> documentedItems)
        {
            XmlSchemaComplexType complexType = schemaObj as XmlSchemaComplexType;
            if (complexType != null)
            {
                XmlSchemaSequence seq = complexType.ContentTypeParticle as XmlSchemaSequence;
                if (seq != null)
                {
                    EnumerateDocumentedItems(seq.Items, documentedItems);
                }
            }
            else
            {
                XmlSchemaSimpleType simpleType = schemaObj as XmlSchemaSimpleType;
                if (simpleType != null && simpleType.Content != null)
                {
                    XmlSchemaSimpleTypeRestriction restriction = simpleType.Content as XmlSchemaSimpleTypeRestriction;
                    if (restriction != null)
                        EnumerateDocumentedItems(restriction.Facets, documentedItems);
                }
            }
        }

        private static string GetUniqueName(XmlSchemaObject schemaObj)
        {
            if (schemaObj is XmlSchemaType)
            {
                return XmlConvert.DecodeName(((XmlSchemaType)schemaObj).QualifiedName.ToString());
            }
            else if (schemaObj is XmlSchemaElement)
            {
                string parentName = GetUniqueName(schemaObj.Parent.Parent);
                return parentName + "." + XmlConvert.DecodeName(((XmlSchemaElement)schemaObj).Name);
            }
            else if (schemaObj is XmlSchemaEnumerationFacet)
            {
                string parentName = GetUniqueName(schemaObj.Parent.Parent);
                return parentName + "." + XmlConvert.DecodeName(((XmlSchemaEnumerationFacet)schemaObj).Value);
            }
            throw new NotImplementedException();
        }

        private static string GetDocumenation(XmlSchemaObject schemaObj)
        {
            XmlSchemaAnnotated annotated = schemaObj as XmlSchemaAnnotated;
            if (annotated == null || annotated.Annotation == null)
                return null;

            foreach (XmlSchemaObject annotation in annotated.Annotation.Items)
            {
                XmlSchemaDocumentation doc = annotation as XmlSchemaDocumentation;
                if (doc != null && doc.Markup.Length > 0)
                {
                    return doc.Markup[0].Value;
                }
            }
            return null;
        }

        internal static void EnumerateDocumentedItems(XmlSchemaSet xmlSchemaSet, Dictionary<string, string> documentedItems)
        {
            foreach (XmlSchema schema in xmlSchemaSet.Schemas())
            {
                EnumerateDocumentedItems(schema.Items, documentedItems);
            }
        }

        internal static void EnumerateDocumentedItems(ServiceDescriptionCollection wsdls, Dictionary<string, string> documentedItems)
        {
            foreach (ServiceDescription wsdl in wsdls)
            {
                foreach (XmlSchema schema in wsdl.Types.Schemas)
                {
                    EnumerateDocumentedItems(schema.Items, documentedItems);
                }
            }
        }
    }
}
