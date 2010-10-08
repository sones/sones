/* 
 * CodeGenerator
 * (c) Achim 'ahzf' Friedland, 2009 - 2010
 */

#region Usings

using System;
using System.Linq;
using System.Diagnostics;
using System.CodeDom.Compiler;

using sones.GraphDB;
using sones.GraphDB.TypeManagement;
using sones.Lib;
using System.Text;
using sones.GraphDS.API.CSharp.Fluent;

#endregion

namespace sones.GraphDS.API.CSharp
{
    
    public static class CodeGenerator
    {

        #region GenerateClasses(myDLL, params myCreateVertexQuery)

        public static Boolean GenerateClasses(this GraphDSSharp myGraphDSSharp, String myDLL, String myNamespace, params CreateTypeQuery[] myCreateTypeQuery)
        {
            return GenerateClasses(myGraphDSSharp, myDLL, myNamespace, (from _CreateTypeQuery in myCreateTypeQuery select _CreateTypeQuery.Name).ToArray());
        }

        #endregion

        #region GenerateClasses(myDLL, params myTypeNames)

        public static Boolean GenerateClasses(this GraphDSSharp myGraphDSSharp, String myDLL, String myNamespace, params String[] myTypeNames)
        {

            // Do it again using the DESCRIBE TYPE command!
            //            return false;

            if (myDLL == null || myDLL.Length == 0)
                throw new ArgumentException("Invalid parameter myDLL!");

            #region Start creation of C# class

            // http://msdn.microsoft.com/en-us/library/microsoft.csharp.csharpcodeprovider.aspx
            CodeDomProvider _Compiler;
            CompilerParameters _CompilerParameters;
            var _SourceCode = new String[myTypeNames.Length];

            _Compiler                                   = CodeDomProvider.CreateProvider("CSharp");    // VB
            _CompilerParameters                         = new CompilerParameters();
            _CompilerParameters.GenerateExecutable      = false;
            _CompilerParameters.OutputAssembly          = myDLL;
            _CompilerParameters.GenerateInMemory        = false;
            _CompilerParameters.TreatWarningsAsErrors   = false;
            _CompilerParameters.ReferencedAssemblies.Add("GraphFS.dll");
            _CompilerParameters.ReferencedAssemblies.Add("GraphLIB.dll");

            //HACK: Using the TypeManager is EVIL!!!
            dynamic _DynamicUnitTestHelper = new DynamicUnitTestHelper((GraphDBSession)myGraphDSSharp.IGraphDBSession);
            var _TypeManager = (DBTypeManager)_DynamicUnitTestHelper.TypeManager;
            //var _TypeManager = (DBTypeManager) UnitTestHelper.GetPrivateField("_GraphDB.TypeManager", (GraphDBSession) myIGraphDBSession);

            var _StringBuilder = new StringBuilder();

            #endregion

            for (var i = 0; i < myTypeNames.Length; i++)
            {

                _StringBuilder.AppendLine("/*");
                _StringBuilder.AppendLine(" * sones GraphDB");
                _StringBuilder.AppendLine(" * Generated Vertex class");
                _StringBuilder.AppendLine(" */");
                _StringBuilder.AppendLine();
                _StringBuilder.AppendLine("using System;");
                _StringBuilder.AppendLine("using System.Linq;");
                _StringBuilder.AppendLine("using System.Collections.Generic;");
                _StringBuilder.AppendLine("using sones.GraphDS.API.CSharp;");
                _StringBuilder.AppendLine("using sones.GraphDS.API.CSharp.Reflection;");
                _StringBuilder.AppendLine();
                _StringBuilder.AppendLine("namespace " + myNamespace);
                _StringBuilder.AppendLine("{");

                var _ActualType = _TypeManager.GetTypeByName(myTypeNames[i]);

                #region Analyse GraphDB type

                String _ParentTypeName = "";

                if (_ActualType.ParentTypeUUID != null)
                {

                    var _ParentType = _TypeManager.GetTypeByUUID(_ActualType.ParentTypeUUID);

                    if (_ParentTypeName != null)
                        _ParentTypeName = " : " + _ParentType.Name;
                    
                }

                _StringBuilder.AppendLine("    /// <summary>");
                _StringBuilder.AppendLine("    /// Generated Vertex class: " + _ActualType.Comment);
                _StringBuilder.AppendLine("    /// </summary>");

                _StringBuilder.AppendLine("    public class " + myTypeNames[i] + _ParentTypeName);
                _StringBuilder.AppendLine("    {");
                _StringBuilder.AppendLine();
                _StringBuilder.AppendLine("        #region Constructor(s)");
                _StringBuilder.AppendLine();
                _StringBuilder.AppendLine("        public " + myTypeNames[i] + "()");
                _StringBuilder.AppendLine("        {");
                _StringBuilder.AppendLine("            Comment = \"" + _ActualType.Comment + "\";"); 
                _StringBuilder.AppendLine("        }");
                _StringBuilder.AppendLine();
                _StringBuilder.AppendLine("        #endregion");

                foreach (var _TypeAttribute in _ActualType.Attributes.Values)
                {

                    var _Type = _TypeManager.GetTypeByUUID(_TypeAttribute.DBTypeUUID).Name;
                    var _Attr = _TypeAttribute.Name;

                    if (_ActualType.Name == myTypeNames[i])
                    {

                        if (_TypeAttribute.KindOfType == KindsOfType.ListOfNoneReferences)
                        {
                            _StringBuilder.AppendLine("        #region " + _Attr);
                            _StringBuilder.AppendLine("        private List<" + _Type + "> _" + _Attr + ";");
                            _StringBuilder.AppendLine("        public  List<" + _Type + "> " + _Attr + " { get { return _" + _Attr + "; } set { _" + _Attr + " = value; } }");
                            _StringBuilder.AppendLine("        #endregion ");
                        }

                        if (_TypeAttribute.KindOfType == KindsOfType.ListOfNoneReferences)
                        {
                            _StringBuilder.AppendLine("        #region " + _Attr);
                            _StringBuilder.AppendLine("        private Set<" + _Type + "> _" + _Attr + ";");
                            _StringBuilder.AppendLine("        public  Set<" + _Type + "> " + _Attr + " { get { return _" + _Attr + "; } set { _" + _Attr + " = value; } }");
                            _StringBuilder.AppendLine("        #endregion ");
                        }

                        if (_TypeAttribute.KindOfType == KindsOfType.SingleReference)
                        {
                            _StringBuilder.AppendLine("        #region " + _Attr);
                            _StringBuilder.AppendLine("        private " + _Type + " _" + _Attr + ";");
                            _StringBuilder.AppendLine("        public  " + _Type + " " + _Attr + " { get { return _" + _Attr + "; } set { _" + _Attr + " = value; } }");
                            _StringBuilder.AppendLine("        #endregion ");
                        }

                    }

                }

                #endregion

                _StringBuilder.AppendLine();
                _StringBuilder.AppendLine("        public override String ToString()");
                _StringBuilder.AppendLine("        {");
                _StringBuilder.AppendLine("            return \"Generated class '" + myTypeNames[i] + "': \" + base.ToString();");
                _StringBuilder.AppendLine("        }");


                _StringBuilder.AppendLine("    } ");
                _StringBuilder.AppendLine("} ");

                _SourceCode[i] = _StringBuilder.ToString();
                _StringBuilder.Clear();

                Debug.WriteLine(_SourceCode[i]);

            }

            #region Invoke compilation of the generated source code

            var _CompilerResults = _Compiler.CompileAssemblyFromSource(_CompilerParameters, _SourceCode);

            if (_CompilerResults.Errors.Count > 0)
            {

                Debug.WriteLine("Errors building {0}", _CompilerResults.PathToAssembly);

                foreach (var _CompilerError in _CompilerResults.Errors)
                {
                    Debug.WriteLine("  {0}", _CompilerError.ToString());
                    Debug.WriteLine("");
                }

            }

            else
                Debug.WriteLine("Source successfully built into '{1}'.", _CompilerResults.PathToAssembly);

            //Assert.IsTrue(_CompilerResults.Errors.Count == 0, "There had been " + _CompilerResults.Errors.Count + " compilation error(s)!");

            #endregion

            return true;

        }

        #endregion

    }

}
