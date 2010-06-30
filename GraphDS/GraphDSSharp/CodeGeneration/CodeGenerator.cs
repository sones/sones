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
 * CodeGenerator
 * Achim Friedland, 2009 - 2010
 */

#region Usings

using System;
using System.CodeDom.Compiler;
using System.Diagnostics;

using sones.GraphDB;
using sones.GraphDB.TypeManagement;

using sones.Lib;

#endregion

namespace sones.GraphDS.API.CSharp.CodeGeneration
{
    
    public static class CodeGenerator
    {

        #region GenerateCSharp(myTypeName, myDLL)

        public static Boolean GenerateCSharp(this GraphDSSharp myGraphDSSharp, String myTypeName, String myDLL, GraphDBSession _IGraphDBSession)
        {

            // Do it again using the DESCRIBE TYPE command!
            //            return false;

            if (myDLL == null && myDLL.Length < 1)
                throw new ArgumentException("Invalid parameter myDLL!");


            #region Start creation of C# class

            // http://msdn.microsoft.com/en-us/library/microsoft.csharp.csharpcodeprovider.aspx
            CodeDomProvider _Compiler;
            CompilerParameters _CompilerParameters;
            String[] _SourceCode = new String[1];

            _Compiler = CodeDomProvider.CreateProvider("CSharp");    // VB
            _CompilerParameters = new CompilerParameters();
            _CompilerParameters.GenerateExecutable = false;
            _CompilerParameters.OutputAssembly = myDLL;
            _CompilerParameters.GenerateInMemory = false;
            _CompilerParameters.TreatWarningsAsErrors = false;
            _CompilerParameters.ReferencedAssemblies.Add("PandoraFS.dll");
            _CompilerParameters.ReferencedAssemblies.Add("PandoraLIB.dll");

            _SourceCode[0] += "using System;" + Environment.NewLine;
            _SourceCode[0] += "using System.Collections.Generic;" + Environment.NewLine;
            _SourceCode[0] += Environment.NewLine;
            _SourceCode[0] += "namespace PandoraDBTests" + Environment.NewLine;
            _SourceCode[0] += "{" + Environment.NewLine + Environment.NewLine;

            #endregion


            //HACK: Using the TypeManager is EVIL!!!
            var _TypeManager = (DBTypeManager)UnitTestHelper.GetPrivateField("_PandoraDB.TypeManager", (GraphDBSession)_IGraphDBSession);

            var _ActualType = _TypeManager.GetTypeByName(myTypeName);

            #region Analyse GraphDB type

            String _ParentTypeName = "";

            if (_ActualType.ParentTypeUUID != null)
            {

                var _ParentType = _TypeManager.GetTypeByUUID(_ActualType.ParentTypeUUID);

                //if (_ParentTypeName != null)
                //    _ParentTypeName = " : " + _ParentType.Name;

            }

            _SourceCode[0] += "    public class " + myTypeName + _ParentTypeName + Environment.NewLine;
            _SourceCode[0] += "    {" + Environment.NewLine + Environment.NewLine;
            _SourceCode[0] += "        public " + myTypeName + "()" + Environment.NewLine;
            _SourceCode[0] += "        {" + Environment.NewLine;
            _SourceCode[0] += "        }" + Environment.NewLine + Environment.NewLine;

            foreach (var _TypeAttribute in _ActualType.Attributes.Values)
            {

                var _Type = _TypeManager.GetTypeByUUID(_TypeAttribute.DBTypeUUID).Name;
                var _Attr = _TypeAttribute.Name;

                if (_ActualType.Name == myTypeName)
                {

                    if (_TypeAttribute.KindOfType == KindsOfType.ListOfNoneReferences)
                    {
                        _SourceCode[0] += "        #region " + _Attr + Environment.NewLine;
                        _SourceCode[0] += "        private List<" + _Type + "> _" + _Attr + ";" + Environment.NewLine;
                        _SourceCode[0] += "        public  List<" + _Type + "> " + _Attr + " { get { return _" + _Attr + "; } set { _" + _Attr + " = value; } }" + Environment.NewLine;
                        _SourceCode[0] += "        #endregion " + Environment.NewLine + Environment.NewLine;
                    }

                    if (_TypeAttribute.KindOfType == KindsOfType.ListOfNoneReferences)
                    {
                        _SourceCode[0] += "        #region " + _Attr + Environment.NewLine;
                        _SourceCode[0] += "        private Set<" + _Type + "> _" + _Attr + ";" + Environment.NewLine;
                        _SourceCode[0] += "        public  Set<" + _Type + "> " + _Attr + " { get { return _" + _Attr + "; } set { _" + _Attr + " = value; } }" + Environment.NewLine;
                        _SourceCode[0] += "        #endregion " + Environment.NewLine + Environment.NewLine;
                    }

                    if (_TypeAttribute.KindOfType == KindsOfType.SingleReference)
                    {
                        _SourceCode[0] += "        #region " + _Attr + Environment.NewLine;
                        _SourceCode[0] += "        private " + _Type + " _" + _Attr + ";" + Environment.NewLine;
                        _SourceCode[0] += "        public  " + _Type + " " + _Attr + " { get { return _" + _Attr + "; } set { _" + _Attr + " = value; } }" + Environment.NewLine;
                        _SourceCode[0] += "        #endregion " + Environment.NewLine + Environment.NewLine;
                    }

                }

            }

            #endregion

            _SourceCode[0] += Environment.NewLine;
            _SourceCode[0] += "        public override String ToString()" + Environment.NewLine;
            _SourceCode[0] += "        {" + Environment.NewLine;
            _SourceCode[0] += "            return \"Generated class '" + myTypeName + "': \" + base.ToString();" + Environment.NewLine;
            _SourceCode[0] += "        }" + Environment.NewLine + Environment.NewLine;


            _SourceCode[0] += "    } " + Environment.NewLine + Environment.NewLine;
            _SourceCode[0] += "} " + Environment.NewLine + Environment.NewLine;

            Debug.WriteLine(_SourceCode[0] + Environment.NewLine);

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
