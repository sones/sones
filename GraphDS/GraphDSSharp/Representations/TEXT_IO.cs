/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
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
* 
*/

/* 
 * TEXT_IO
 * Achim 'ahzf' Friedland, 2009-2010
 */

#region Usings

using System;
using System.Text;
using System.Linq;
using System.Net.Mime;
using System.Collections.Generic;

using sones.GraphDB.TypeManagement;
using sones.GraphDS.API.CSharp;

using sones.Lib.CLI;

#endregion

namespace sones.GraphDB.QueryLanguage.Result
{

    /// <summary>
    /// Transforms a QueryResult and a DBObjectReadout into a text/plain representation
    /// </summary>

    public class TEXT_IO : IDBExport
    {

        #region Data

        private readonly ContentType _ExportContentType;

        #endregion

        #region Constructor

        public TEXT_IO()
        {
            _ExportContentType = new ContentType("text/plain") { CharSet = "UTF-8" };
        }

        #endregion


        #region IDBExport Members

        #region ExportContentType

        public ContentType ExportContentType
        {
            get
            {
                return _ExportContentType;
            }
        }

        #endregion


        #region Export(myQueryResult)

        public Object Export(QueryResult myQueryResult)
        {
            return Export(myQueryResult, new StringBuilder());
        }

        #endregion

        #region Export(myQueryResult, myStringBuilder)

        public Object Export(QueryResult myQueryResult, StringBuilder myStringBuilder)
        {

            myStringBuilder.AppendLine();

            // result -------------------------------
            myStringBuilder.Append("result: ").AppendLine(myQueryResult.ResultType.ToString());

            // duration -----------------------------
            myStringBuilder.Append("duration: ").Append(myQueryResult.Duration.ToString()).AppendLine(" ms");

            // warnings -----------------------------
            if (myQueryResult.Warnings.Any())
            {
                
                myStringBuilder.AppendLine(myQueryResult.Warnings.Count() + " warning(s): ");

                foreach (var _Warning in myQueryResult.Warnings)
                    myStringBuilder.AppendLine("  " + _Warning.GetType().Name + " => " + _Warning.ToString());
                
                myStringBuilder.AppendLine();

            }

            // errors -------------------------------
            if (myQueryResult.Errors.Any())
            {

                myStringBuilder.AppendLine(myQueryResult.Warnings.Count() + " errors(s): ");

                foreach (var _Error in myQueryResult.Errors)
                    myStringBuilder.AppendLine("  " + _Error.GetType().Name + " => " + _Error.ToString());

                myStringBuilder.AppendLine();

            }

            // results ------------------------------
            if (myQueryResult.Results.Any())
            {

                myStringBuilder.AppendLine();
                myStringBuilder.AppendLine(myQueryResult.Results.Count() + " result(s): ");

                foreach (var _SelectionListElementResult in myQueryResult.Results)
                    if (_SelectionListElementResult.Objects != null)
                        foreach (var _DBObject in _SelectionListElementResult.Objects)
                            Export(_DBObject, myStringBuilder);

                myStringBuilder.AppendLine();

            }

            return myStringBuilder;

        }

        #endregion


        #region Export(myDBObjectReadout, myRecursion)

        public Object Export(DBObjectReadout myDBObjectReadout)
        {
            return Export(myDBObjectReadout, new StringBuilder(), 0);
        }

        #endregion

        #region Export(myDBObjectReadout, myStringBuilder, myIndendLevel = 0)

        public object Export(DBObjectReadout myDBObjectReadout, StringBuilder myStringBuilder, Int32 myIndent = 0)
        {

            var _TypeObject = myDBObjectReadout["TYPE"] as GraphDBType;
            String _Type = (_TypeObject != null) ? _TypeObject.Name : "";
            var _UUID = myDBObjectReadout["UUID"];

            IEnumerable<DBObjectReadout> _DBObjects          = null;
            IEnumerable<Object>          _AttributeValueList = null;


            #region DBWeightedObjectReadout

            var _WeightedDBObject1 = myDBObjectReadout as DBWeightedObjectReadout;

            if (_WeightedDBObject1 != null)
            {
                
                for (var i = 0; i < myIndent; i++)
                    myStringBuilder.Append("\t");

                myStringBuilder.Append("edgelabel: ");
                myStringBuilder.AppendLine("weight: " + _WeightedDBObject1.Weight.ToString() + " (" + _WeightedDBObject1.Weight.Type.ToString() + ")");

            }

            #endregion

            foreach (var _Attribute in myDBObjectReadout.Attributes)
            {

                switch (_Attribute.Key)
                {

                    case "TYPE":
                    case "Type":
                        
                        var _ThisTypeObject = _Attribute.Value as GraphDBType;

                        String _ThisType = (_ThisTypeObject != null) ? _ThisTypeObject.Name : "";

                        // Indent output!
                        for (var i = 0; i < myIndent; i++)
                            myStringBuilder.Append("\t");

                        myStringBuilder.Append("TYPE").Append(": ").AppendLine(_ThisType);
                        
                        break;

                    default:

                        if (_Attribute.Value != null)
                        {

                            // Indent output!
                            for (var i=0; i<myIndent; i++)
                                myStringBuilder.Append("\t");

                            myStringBuilder.Append(_Attribute.Key.ToString() + ": ");

                            #region IEnumerable<DBObjectReadout>

                            _DBObjects = _Attribute.Value as IEnumerable<DBObjectReadout>;

                            if (_DBObjects != null && _DBObjects.Count() > 0)
                            {

                                var _EdgeInfo = (_Attribute.Value as Edge);
                                var _EdgeType = (_EdgeInfo != null) ? _EdgeInfo.EdgeTypeName : "";

                                myStringBuilder.AppendLine();

                                // An edgelabel for all edges together...
                                //_ListAttribute.Add(new XElement("hyperedgelabel"));
                                
                                foreach (var _DBObjectReadout in _DBObjects)
                                    Export(_DBObjectReadout, myStringBuilder, myIndent++);

                                continue;

                            }

                            #endregion

                            #region Attribute value and attribute value lists

                            _AttributeValueList = _Attribute.Value as IEnumerable<Object>;

                            if (_AttributeValueList != null)
                            {

                                foreach (var _Value in _AttributeValueList)
                                    myStringBuilder.Append(_Value.ToString());

                                myStringBuilder.AppendLine();

                                continue;

                            }

                            #endregion

                            myStringBuilder.AppendLine(_Attribute.Value.ToString());

                        }

                        break;

                }

            }

            return myStringBuilder;

            //foreach (var _Attribute in myDBObjectReadout.Attributes)
            //{
            //    if (_Attribute.Value != null)
            //    {

            //        if (_Attribute.Value is DBObjectReadout)
            //        {
            //            myStringBuilder.AppendLine(_Attribute.Key.ToString() + " <resolved>:");
            //            toTEXT((DBObjectReadout)_Attribute.Value, (UInt16)(level + 1), ref myStringBuilder, myOutputFormat);
            //        }

            //        else
            //        {
            //            if (_Attribute.Value is IEnumerable<DBObjectReadout>)
            //            {
            //                for (UInt16 i = 0; i < level; i++)
            //                {
            //                    myStringBuilder.Append("\t");
            //                }
            //                myStringBuilder.AppendLine(_Attribute.Key.ToString() + " <resolved>:");

            //                foreach (var _DBObjectReadout in (IEnumerable<DBObjectReadout>)_Attribute.Value)
            //                {
            //                    toTEXT(_DBObjectReadout, (UInt16)(level + 1), ref myStringBuilder, myOutputFormat);
            //                    if (myOutputFormat == CLI_Output.Standard)
            //                    {
            //                        myStringBuilder.Append(Environment.NewLine);
            //                        myStringBuilder.Append(Environment.NewLine);
            //                    }
            //                }
            //                if (myOutputFormat == CLI_Output.Standard)
            //                    myStringBuilder.AppendLine();
            //            }
            //            else
            //            {
            //                for (UInt16 i = 0; i < level; i++)
            //                {
            //                    myStringBuilder.Append("\t");
            //                }

            //                if((_Attribute.Value is GraphDBType[]))
            //                {
            //                    myStringBuilder.AppendLine(_Attribute.Key.ToString() + " = " + ((GraphDBType[])_Attribute.Value).First().Name + " []");

            //                }else if (_Attribute.Value is GraphDBType)
            //                {   
            //                    myStringBuilder.AppendLine(_Attribute.Key.ToString() + " = " + ((GraphDBType)_Attribute.Value).Name);
            //                }
            //                else if (_Attribute.Value is TypeAttribute)
            //                {
            //                    myStringBuilder.AppendLine(_Attribute.Key.ToString() + " = " + ((TypeAttribute)_Attribute.Value).Name);
            //                }
            //                else
            //                    myStringBuilder.AppendLine(_Attribute.Key.ToString() + " = " + _Attribute.Value.ToString());
            //            }
            //        }

            //    }
            //    else
            //    {

            //        for (UInt16 i = 0; i < level; i++)
            //            myStringBuilder.Append("\t");

            //        myStringBuilder.AppendLine(_Attribute.Key.ToString() + " = not resolved");

            //    }
            //}


        }

        #endregion


        #region ExportString(myQueryResult)

        public String ExportString(QueryResult myQueryResult)
        {

            var _StringBuilder = new StringBuilder();

            Export(myQueryResult, _StringBuilder);

            return _StringBuilder.ToString();

        }

        #endregion

        #endregion

    }

}
