using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;

namespace WCFExtras.Utils
{
    static class CodeDomUtils
    {
        class QualifiedName
        {
            public QualifiedName(string Namespace, string Name)
            {
                this.Namespace = Namespace;
                this.Name = Name;
            }
            public QualifiedName(QualifiedName name, QualifiedName parent)
            {
                if (parent != null)
                {
                    this.Namespace = (name.Namespace ?? parent.Namespace);
                    this.Name = parent.Name + "." + name.Name;
                }
                else
                {
                    this.Namespace = name.Namespace;
                    this.Name = name.Name;
                }

            }
            public override string ToString()
            {
                return Namespace + ":" + Name;
            }
            public string Namespace;
            public string Name;
        }

        public static Dictionary<string, CodeTypeMember> EnumerareCodeMembers(CodeCompileUnit unit)
        {
            Dictionary<string, CodeTypeMember> result = new Dictionary<string, CodeTypeMember>();
            foreach (CodeNamespace ns in unit.Namespaces)
            {
                foreach (CodeTypeDeclaration decl in ns.Types)
                {
                    EnumerareCodeMembers(decl, null, result);
                }
            }
            return result;
        }

        private static void EnumerareCodeMembers(CodeTypeMember member, QualifiedName parentName, Dictionary<string, CodeTypeMember> members)
        {
            QualifiedName memberName = GetUniqueName(member, parentName);
            members[memberName.ToString()] = member;
            CodeTypeDeclaration decl = member as CodeTypeDeclaration;
            if (decl != null)
            {
                foreach (CodeTypeMember subMember in decl.Members)
                {
                    EnumerareCodeMembers(subMember, memberName, members);
                }
            }
        }

        private static QualifiedName GetUniqueName(CodeTypeMember member, QualifiedName parentName)
        {
            if (member is CodeTypeDeclaration)
            {
                return new QualifiedName(GetUniqueName((CodeTypeDeclaration)member), null);
            }
            else
            {
                return new QualifiedName(GetUniqueName(member), parentName);
            }
        }

        private static QualifiedName GetUniqueName(CodeTypeMember codeMember)
        {
            string name = codeMember.Name;
            CodeAttributeDeclaration dmAttribute = codeMember.CustomAttributes.Find("System.Runtime.Serialization.DataMemberAttribute");
            if (dmAttribute != null)
            {
                CodeAttributeArgument nameAttrib = dmAttribute.Arguments.Find("Name");
                if (nameAttrib != null)
                    name = nameAttrib.GetStringValue();
            }
            else
            {
                dmAttribute = codeMember.CustomAttributes.Find("System.Runtime.Serialization.EnumMemberAttribute");
                if (dmAttribute != null)
                {
                    CodeAttributeArgument nameAttrib = dmAttribute.Arguments.Find("Value");
                    if (nameAttrib != null)
                        name = nameAttrib.GetStringValue();
                }
            }
            return new QualifiedName(null, name);
        }

        private static QualifiedName GetUniqueName(CodeTypeDeclaration codeType)
        {
            string name = codeType.Name;
            string ns = null;
            CodeAttributeDeclaration dcAttribute = codeType.CustomAttributes.Find("System.Runtime.Serialization.DataContractAttribute",
                "System.Runtime.Serialization.CollectionDataContractAttribute");
            if (dcAttribute != null)
            {
                CodeAttributeArgument nameAttrib = dcAttribute.Arguments.Find("Name");
                if (nameAttrib != null)
                    name = nameAttrib.GetStringValue();
                CodeAttributeArgument namespaceAttrib = dcAttribute.Arguments.Find("Namespace");
                if (namespaceAttrib != null)
                    ns = namespaceAttrib.GetStringValue();
            }
            return new QualifiedName(ns, name);
        }

        //Extensions
        public static CodeAttributeDeclaration Find(this CodeAttributeDeclarationCollection attributes, params string[] attributeTypes)
        {
            return attributes.Cast<CodeAttributeDeclaration>().FirstOrDefault(attribute => attributeTypes.Contains(attribute.AttributeType.BaseType));
        }

        public static CodeAttributeArgument Find(this CodeAttributeArgumentCollection attributes, string argName)
        {
            return attributes.Cast<CodeAttributeArgument>().FirstOrDefault(arg => arg.Name == argName);
        }

        public static int IndexOf(this CodeTypeMemberCollection members, string memberName)
        {
            for (int i = 0; i < members.Count; i++)
            {
                if (members[i].Name == memberName)
                    return i;
            }
            return -1;
        }

        public static int IndexOf(this CodeTypeReferenceCollection references, string typeName)
        {
            for (int i = 0; i < references.Count; i++)
            {
                if (references[i].BaseType == typeName)
                    return i;
            }
            return -1;
        }

        public static string GetStringValue(this CodeAttributeArgument argument)
        {
            return (argument.Value as CodePrimitiveExpression).Value.ToString();
        }
    }
}
