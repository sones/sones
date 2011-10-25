using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Description;
using System.Web.Services.Description;
using System.CodeDom;
using WCFExtras.Utils;
using System.Runtime.Serialization;
using System.Xml.Schema;
using ServiceDescription = System.Web.Services.Description.ServiceDescription;
using System.Xml;
using System.Configuration;
using System.IO;

namespace WCFExtras.Wsdl.Documentation
{
    public class XmlCommentsImporter : IServiceBehavior, IWsdlImportExtension
    {
        internal static ImportOptions options = new ImportOptions();
        internal ServiceDescriptionCollection wsdlDocuments;
        internal XmlSchemaSet xmlSchemas;

        void IWsdlImportExtension.BeforeImport(ServiceDescriptionCollection wsdlDocuments,
            XmlSchemaSet xmlSchemas, ICollection<System.Xml.XmlElement> policy)
        {
            this.wsdlDocuments = wsdlDocuments;
            this.xmlSchemas = xmlSchemas;
        }

        internal static void AddXmlComment(CodeTypeMember member, string documentation, ImportOptions options)
        {
            IEnumerable<string> commentLines = XmlCommentsUtils.ParseAndReformatComment(documentation, options.Format, options.WrapLongLines);
            CodeCommentStatement[] commentStatements = commentLines.Select(s => new CodeCommentStatement(s, true)).ToArray();
            member.Comments.AddRange(commentStatements);
        }

        private static string GetDocumentation(DocumentableItem item)
        {
            if (item.DocumentationElement != null)
                return item.DocumentationElement.InnerText;
            return item.Documentation;
        }

        void IWsdlImportExtension.ImportContract(WsdlImporter importer, WsdlContractConversionContext context)
        {
            string documentation = GetDocumentation(context.WsdlPortType);
            context.Contract.Behaviors.Add(new XmlCommentsSvcExtension(this, documentation));

            foreach (Operation operation in context.WsdlPortType.Operations)
            {
                documentation = GetDocumentation(operation);
                if (!String.IsNullOrEmpty(documentation))
                {
                    OperationDescription operationDescription = context.Contract.Operations.Find(operation.Name);
                    operationDescription.Behaviors.Add(new XmlCommentsOpExtension(this, documentation));
                }
            }
        }

        void IWsdlImportExtension.ImportEndpoint(WsdlImporter importer, WsdlEndpointConversionContext context)
        {
        }

        void IServiceBehavior.AddBindingParameters(System.ServiceModel.Description.ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
        }

        void IServiceBehavior.ApplyDispatchBehavior(System.ServiceModel.Description.ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase)
        {
        }

        void IServiceBehavior.Validate(System.ServiceModel.Description.ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase)
        {
        }
    }

    public class XmlCommentsSvcExtension : IContractBehavior, IServiceContractGenerationExtension
    {
        #region IContractBehavior
        void IContractBehavior.AddBindingParameters(ContractDescription contractDescription, ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters) { }
        void IContractBehavior.ApplyClientBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime) { }
        void IContractBehavior.ApplyDispatchBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.DispatchRuntime dispatchRuntime) { }
        void IContractBehavior.Validate(ContractDescription contractDescription, ServiceEndpoint endpoint) { }
        #endregion

        string documentation;
        XmlCommentsImporter importer;

        public XmlCommentsSvcExtension(XmlCommentsImporter importer, string documentation)
        {
            this.documentation = documentation;
            this.importer = importer;
        }

        void IServiceContractGenerationExtension.GenerateContract(ServiceContractGenerationContext context)
        {
            ReadConfiguration(context.ServiceContractGenerator.Configuration);
            if (!String.IsNullOrEmpty(documentation))
                XmlCommentsImporter.AddXmlComment(context.ContractType, documentation, XmlCommentsImporter.options);
            AddXmlCommentsToDataContracts(context);
        }

        private void AddXmlCommentsToDataContracts(ServiceContractGenerationContext context)
        {
            Dictionary<string, CodeTypeMember> codeMembers = CodeDomUtils.EnumerareCodeMembers(context.ServiceContractGenerator.TargetCompileUnit);

            Dictionary<string, string> documentedItems = new Dictionary<string, string>();
            WsdlUtils.EnumerateDocumentedItems(importer.wsdlDocuments, documentedItems);
            WsdlUtils.EnumerateDocumentedItems(importer.xmlSchemas, documentedItems);

            foreach (KeyValuePair<string, string> documentedItem in documentedItems)
            {
                CodeTypeMember codeMember;
                if (codeMembers.TryGetValue(documentedItem.Key, out codeMember))
                    XmlCommentsImporter.AddXmlComment(codeMember, documentedItem.Value, XmlCommentsImporter.options);
            }


            PostProcessCodeMembers(context, codeMembers.Values);
        }

        protected virtual void PostProcessCodeMembers(ServiceContractGenerationContext context, IEnumerable<CodeTypeMember> members)
        {
            if (XmlCommentsImporter.options.Documentable)
            {
                context.ServiceContractGenerator.Options = ServiceContractGenerationOptions.None;
                RemoveIExtensibleDataObjectFromDeclaration(members);
            }
        }

        private void RemoveIExtensibleDataObjectFromDeclaration(IEnumerable<CodeTypeMember> members)
        {
            foreach (CodeTypeMember member in members)
            {
                if (member is CodeTypeDeclaration)
                    RemoveIExtensibleDataObjectFromDeclaration((CodeTypeDeclaration)member);
            }
        }

        private void RemoveIExtensibleDataObjectFromDeclaration(CodeTypeDeclaration codeTypeDeclaration)
        {
            int index = codeTypeDeclaration.BaseTypes.IndexOf("System.Runtime.Serialization.IExtensibleDataObject");
            if (index >= 0)
            {
                codeTypeDeclaration.BaseTypes.RemoveAt(index);
                index = codeTypeDeclaration.Members.IndexOf("ExtensionData");
                if (index >= 0)
                    codeTypeDeclaration.Members.RemoveAt(index);
                index = codeTypeDeclaration.Members.IndexOf("extensionDataField");
                if (index >= 0)
                    codeTypeDeclaration.Members.RemoveAt(index);
            }
        }

        private void ReadConfiguration(Configuration configuration)
        {
            XmlCommentsConfig config = XmlCommentsConfig.GetConfiguration(configuration);
            if (config != null)
            {
                XmlCommentsImporter.options.Documentable = config.Documentable;
                XmlCommentsImporter.options.Format = config.Format;
                XmlCommentsImporter.options.WrapLongLines = config.WrapLongLines;
            }
        }
    }

    public class XmlCommentsOpExtension : IOperationBehavior, IOperationContractGenerationExtension
    {
        #region IOperationBehavior
        void IOperationBehavior.AddBindingParameters(OperationDescription operationDescription, System.ServiceModel.Channels.BindingParameterCollection bindingParameters) { }
        void IOperationBehavior.ApplyClientBehavior(OperationDescription operationDescription, System.ServiceModel.Dispatcher.ClientOperation clientOperation) { }
        void IOperationBehavior.ApplyDispatchBehavior(OperationDescription operationDescription, System.ServiceModel.Dispatcher.DispatchOperation dispatchOperation) { }
        void IOperationBehavior.Validate(OperationDescription operationDescription) { }
        #endregion

        string documentation;
        XmlCommentsImporter importer;

        public XmlCommentsOpExtension(XmlCommentsImporter importer, string documentation)
        {
            this.documentation = documentation;
            this.importer = importer;
        }

        void IOperationContractGenerationExtension.GenerateOperation(OperationContractGenerationContext context)
        {
            XmlCommentsImporter.AddXmlComment(context.SyncMethod, documentation, XmlCommentsImporter.options);
        }
    }
}
