namespace SchemaToClassesGenerator {
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://sones.com/QueryResultSchema.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://sones.com/QueryResultSchema.xsd", IsNullable=true)]
    public partial class SchemaHyperEdgeView {
        
        private string nameField;
        
        private int countOfPropertiesField;
        
        private Property[] propertyField;
        
        private SchemaSingleEdgeView[] singleEdgeField;
        
        /// <remarks/>
        public string Name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        public int CountOfProperties {
            get {
                return this.countOfPropertiesField;
            }
            set {
                this.countOfPropertiesField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Property")]
        public Property[] Property {
            get {
                return this.propertyField;
            }
            set {
                this.propertyField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("SingleEdge")]
        public SchemaSingleEdgeView[] SingleEdge {
            get {
                return this.singleEdgeField;
            }
            set {
                this.singleEdgeField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://sones.com/QueryResultSchema.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://sones.com/QueryResultSchema.xsd", IsNullable=true)]
    public partial class Property {
        
        private string idField;
        
        private string typeField;
        
        private string valueField;
        
        /// <remarks/>
        public string ID {
            get {
                return this.idField;
            }
            set {
                this.idField = value;
            }
        }
        
        /// <remarks/>
        public string Type {
            get {
                return this.typeField;
            }
            set {
                this.typeField = value;
            }
        }
        
        /// <remarks/>
        public string Value {
            get {
                return this.valueField;
            }
            set {
                this.valueField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://sones.com/QueryResultSchema.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://sones.com/QueryResultSchema.xsd", IsNullable=true)]
    public partial class SchemaSingleEdgeView {
        
        private int countOfPropertiesField;
        
        private bool countOfPropertiesFieldSpecified;
        
        private Property[] propertyField;
        
        private SchemaVertexView targetVertexField;
        
        /// <remarks/>
        public int CountOfProperties {
            get {
                return this.countOfPropertiesField;
            }
            set {
                this.countOfPropertiesField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool CountOfPropertiesSpecified {
            get {
                return this.countOfPropertiesFieldSpecified;
            }
            set {
                this.countOfPropertiesFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Property")]
        public Property[] Property {
            get {
                return this.propertyField;
            }
            set {
                this.propertyField = value;
            }
        }
        
        /// <remarks/>
        public SchemaVertexView TargetVertex {
            get {
                return this.targetVertexField;
            }
            set {
                this.targetVertexField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://sones.com/QueryResultSchema.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://sones.com/QueryResultSchema.xsd", IsNullable=true)]
    public partial class SchemaVertexView {
        
        private Property[] propertiesField;
        
        private BinaryData[] binaryPropertiesField;
        
        private SchemaHyperEdgeView[] edgesField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public Property[] Properties {
            get {
                return this.propertiesField;
            }
            set {
                this.propertiesField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("BinaryProperty", IsNullable=false)]
        public BinaryData[] BinaryProperties {
            get {
                return this.binaryPropertiesField;
            }
            set {
                this.binaryPropertiesField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Edge", IsNullable=false)]
        public SchemaHyperEdgeView[] Edges {
            get {
                return this.edgesField;
            }
            set {
                this.edgesField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://sones.com/QueryResultSchema.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://sones.com/QueryResultSchema.xsd", IsNullable=true)]
    public partial class BinaryData {
        
        private string idField;
        
        private byte[] contentField;
        
        /// <remarks/>
        public string ID {
            get {
                return this.idField;
            }
            set {
                this.idField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="hexBinary")]
        public byte[] Content {
            get {
                return this.contentField;
            }
            set {
                this.contentField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://sones.com/QueryResultSchema.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://sones.com/QueryResultSchema.xsd", IsNullable=false)]
    public partial class Query {
        
        private string languageField;
        
        private string valueField;
        
        private string resultTypeField;
        
        private string errorField;
        
        private ulong durationField;
        
        private long verticesCountField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Language {
            get {
                return this.languageField;
            }
            set {
                this.languageField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Value {
            get {
                return this.valueField;
            }
            set {
                this.valueField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ResultType {
            get {
                return this.resultTypeField;
            }
            set {
                this.resultTypeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Error {
            get {
                return this.errorField;
            }
            set {
                this.errorField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ulong Duration {
            get {
                return this.durationField;
            }
            set {
                this.durationField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public long VerticesCount {
            get {
                return this.verticesCountField;
            }
            set {
                this.verticesCountField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://sones.com/QueryResultSchema.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://sones.com/QueryResultSchema.xsd", IsNullable=false)]
    public partial class Result {
        
        private Query queryField;
        
        private SchemaVertexView[] vertexViewsField;
        
        private string versionField;
        
        /// <remarks/>
        public Query Query {
            get {
                return this.queryField;
            }
            set {
                this.queryField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("VertexView", IsNullable=false)]
        public SchemaVertexView[] VertexViews {
            get {
                return this.vertexViewsField;
            }
            set {
                this.vertexViewsField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Version {
            get {
                return this.versionField;
            }
            set {
                this.versionField = value;
            }
        }
    }
}
