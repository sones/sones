namespace SchemaToClassesGenerator {
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/QueryResultSchema.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://tempuri.org/QueryResultSchema.xsd", IsNullable=true)]
    public partial class VertexRevisionID {
        
        private long timestampField;
        
        private ulong idField;
        
        /// <remarks/>
        public long Timestamp {
            get {
                return this.timestampField;
            }
            set {
                this.timestampField = value;
            }
        }
        
        /// <remarks/>
        public ulong ID {
            get {
                return this.idField;
            }
            set {
                this.idField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/QueryResultSchema.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://tempuri.org/QueryResultSchema.xsd", IsNullable=true)]
    public partial class Properties {
        
        private string nameField;
        
        private string typeField;
        
        private string valueField;
        
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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/QueryResultSchema.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://tempuri.org/QueryResultSchema.xsd", IsNullable=true)]
    public partial class BinaryData {
        
        private string nameField;
        
        private byte[] contentField;
        
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
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(EdgeView))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/QueryResultSchema.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://tempuri.org/QueryResultSchema.xsd", IsNullable=true)]
    public abstract partial class GraphElementView {
        
        private int countOfPropertiesField;
        
        private string commentField;
        
        private System.DateTime creationDateField;
        
        private System.DateTime modificationDateField;
        
        private Properties[] propertiesField;
        
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
        public string Comment {
            get {
                return this.commentField;
            }
            set {
                this.commentField = value;
            }
        }
        
        /// <remarks/>
        public System.DateTime CreationDate {
            get {
                return this.creationDateField;
            }
            set {
                this.creationDateField = value;
            }
        }
        
        /// <remarks/>
        public System.DateTime ModificationDate {
            get {
                return this.modificationDateField;
            }
            set {
                this.modificationDateField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public Properties[] Properties {
            get {
                return this.propertiesField;
            }
            set {
                this.propertiesField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/QueryResultSchema.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://tempuri.org/QueryResultSchema.xsd", IsNullable=true)]
    public partial class EdgeView : GraphElementView {
        
        private string edgeTypeNameField;
        
        private VertexView sourceVertexField;
        
        private VertexView[] targetVerticesField;
        
        /// <remarks/>
        public string EdgeTypeName {
            get {
                return this.edgeTypeNameField;
            }
            set {
                this.edgeTypeNameField = value;
            }
        }
        
        /// <remarks/>
        public VertexView SourceVertex {
            get {
                return this.sourceVertexField;
            }
            set {
                this.sourceVertexField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public VertexView[] TargetVertices {
            get {
                return this.targetVerticesField;
            }
            set {
                this.targetVerticesField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/QueryResultSchema.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://tempuri.org/QueryResultSchema.xsd", IsNullable=true)]
    public partial class VertexView {
        
        private string vertexTypeNameField;
        
        private long vertexIDField;
        
        private string editionNameField;
        
        private VertexRevisionID vertexRevisionIDField;
        
        private EdgeTuple[] edgesField;
        
        private BinaryData[] binaryPropertysField;
        
        /// <remarks/>
        public string VertexTypeName {
            get {
                return this.vertexTypeNameField;
            }
            set {
                this.vertexTypeNameField = value;
            }
        }
        
        /// <remarks/>
        public long VertexID {
            get {
                return this.vertexIDField;
            }
            set {
                this.vertexIDField = value;
            }
        }
        
        /// <remarks/>
        public string EditionName {
            get {
                return this.editionNameField;
            }
            set {
                this.editionNameField = value;
            }
        }
        
        /// <remarks/>
        public VertexRevisionID VertexRevisionID {
            get {
                return this.vertexRevisionIDField;
            }
            set {
                this.vertexRevisionIDField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("ETupleList", IsNullable=false)]
        public EdgeTuple[] Edges {
            get {
                return this.edgesField;
            }
            set {
                this.edgesField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public BinaryData[] BinaryPropertys {
            get {
                return this.binaryPropertysField;
            }
            set {
                this.binaryPropertysField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/QueryResultSchema.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://tempuri.org/QueryResultSchema.xsd", IsNullable=true)]
    public partial class EdgeTuple {
        
        private string nameField;
        
        private EdgeView edgeField;
        
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
        public EdgeView Edge {
            get {
                return this.edgeField;
            }
            set {
                this.edgeField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/QueryResultSchema.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://tempuri.org/QueryResultSchema.xsd", IsNullable=true)]
    public partial class EdgeTupleList {
        
        private EdgeTuple[] eTupleListField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ETupleList")]
        public EdgeTuple[] ETupleList {
            get {
                return this.eTupleListField;
            }
            set {
                this.eTupleListField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/QueryResultSchema.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://tempuri.org/QueryResultSchema.xsd", IsNullable=true)]
    public partial class VertexViewList {
        
        private VertexView[] vertexViewField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("VertexView")]
        public VertexView[] VertexView {
            get {
                return this.vertexViewField;
            }
            set {
                this.vertexViewField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/QueryResultSchema.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://tempuri.org/QueryResultSchema.xsd", IsNullable=true)]
    public partial class PropertyList {
        
        private Properties[] propertiesField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Properties")]
        public Properties[] Properties {
            get {
                return this.propertiesField;
            }
            set {
                this.propertiesField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/QueryResultSchema.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://tempuri.org/QueryResultSchema.xsd", IsNullable=true)]
    public partial class BinaryDataList {
        
        private BinaryData[] binaryDataField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("BinaryData")]
        public BinaryData[] BinaryData {
            get {
                return this.binaryDataField;
            }
            set {
                this.binaryDataField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/QueryResultSchema.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://tempuri.org/QueryResultSchema.xsd", IsNullable=true)]
    public partial class Result {
        
        private Query queryField;
        
        private string errorField;
        
        private ulong numberField;
        
        private ulong durationField;
        
        private VertexView[] verticesField;
        
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
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string Error {
            get {
                return this.errorField;
            }
            set {
                this.errorField = value;
            }
        }
        
        /// <remarks/>
        public ulong Number {
            get {
                return this.numberField;
            }
            set {
                this.numberField = value;
            }
        }
        
        /// <remarks/>
        public ulong Duration {
            get {
                return this.durationField;
            }
            set {
                this.durationField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public VertexView[] Vertices {
            get {
                return this.verticesField;
            }
            set {
                this.verticesField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://tempuri.org/QueryResultSchema.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://tempuri.org/QueryResultSchema.xsd", IsNullable=false)]
    public partial class Query {
        
        private string languageField;
        
        private string valueField;
        
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
    }
}
