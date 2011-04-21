namespace SchemaToClassesGenerator {
    
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(SchemaEdgeView))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://sones.com/QueryResultSchema.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://sones.com/QueryResultSchema.xsd", IsNullable=true)]
    public abstract partial class GraphElementView {
        
        private int countOfPropertiesField;
        
        private Property[] propertiesField;
        
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
        [System.Xml.Serialization.XmlArrayItemAttribute("Properties", IsNullable=false)]
        public Property[] Properties {
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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://sones.com/QueryResultSchema.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://sones.com/QueryResultSchema.xsd", IsNullable=true)]
    public partial class SchemaEdgeView : GraphElementView {
        
        private SchemaVertexView[] targetVerticesField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("VertexView", IsNullable=false)]
        public SchemaVertexView[] TargetVertices {
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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://sones.com/QueryResultSchema.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://sones.com/QueryResultSchema.xsd", IsNullable=true)]
    public partial class SchemaVertexView {
        
        private Property[] propertiesField;
        
        private BinaryData[] binaryPropertiesField;
        
        private EdgeTuple[] edgesField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Properties", IsNullable=false)]
        public Property[] Properties {
            get {
                return this.propertiesField;
            }
            set {
                this.propertiesField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public BinaryData[] BinaryProperties {
            get {
                return this.binaryPropertiesField;
            }
            set {
                this.binaryPropertiesField = value;
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
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://sones.com/QueryResultSchema.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://sones.com/QueryResultSchema.xsd", IsNullable=true)]
    public partial class EdgeTuple {
        
        private string nameField;
        
        private SchemaEdgeView edgeField;
        
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
        public SchemaEdgeView Edge {
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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://sones.com/QueryResultSchema.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://sones.com/QueryResultSchema.xsd", IsNullable=true)]
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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://sones.com/QueryResultSchema.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://sones.com/QueryResultSchema.xsd", IsNullable=true)]
    public partial class VertexViewList {
        
        private SchemaVertexView[] vertexViewField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("VertexView")]
        public SchemaVertexView[] VertexView {
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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://sones.com/QueryResultSchema.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://sones.com/QueryResultSchema.xsd", IsNullable=true)]
    public partial class PropertyList {
        
        private Property[] propertiesField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Properties")]
        public Property[] Properties {
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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://sones.com/QueryResultSchema.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://sones.com/QueryResultSchema.xsd", IsNullable=true)]
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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://sones.com/QueryResultSchema.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://sones.com/QueryResultSchema.xsd", IsNullable=true)]
    public partial class Result {
        
        private Query queryField;
        
        private string errorField;
        
        private ulong numberField;
        
        private ulong durationField;
        
        private SchemaVertexView[] verticesField;
        
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
        [System.Xml.Serialization.XmlArrayAttribute(IsNullable=true)]
        [System.Xml.Serialization.XmlArrayItemAttribute("VertexView", IsNullable=false)]
        public SchemaVertexView[] Vertices {
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
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://sones.com/QueryResultSchema.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://sones.com/QueryResultSchema.xsd", IsNullable=false)]
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
