/* 
 * TEXT_IO_Extensions
 * Achim 'ahzf' Friedland, 2009-2010
 */

#region Usings

using System;
using System.Linq;
using System.Text;

using sones.GraphDB.Structures.Result;
using System.Collections.Generic;
using sones.GraphDB.TypeManagement;

#endregion

namespace sones.GraphIO.TEXT
{

    /// <summary>
    /// Extension methods to transform a QueryResult and a DBObjectReadout into a text/plain representation
    /// </summary>

    public static class TEXT_IO_Extensions
    {

        #region ToTEXT(this myQueryResult)

        public static String ToTEXT(this QueryResult myQueryResult)
        {

            var _StringBuilder = new StringBuilder();

            _StringBuilder.AppendLine();

            // result -------------------------------
            _StringBuilder.Append("result: ").AppendLine(myQueryResult.ResultType.ToString());

            // duration -----------------------------
            _StringBuilder.Append("duration: ").Append(myQueryResult.Duration.ToString()).AppendLine(" ms");

            // warnings -----------------------------
            if (myQueryResult.Warnings.Any())
            {

                _StringBuilder.AppendLine(myQueryResult.Warnings.Count() + " warning(s): ");

                foreach (var _Warning in myQueryResult.Warnings)
                    _StringBuilder.AppendLine("  " + _Warning.GetType().Name + " => " + _Warning.ToString());

                _StringBuilder.AppendLine();

            }

            // errors -------------------------------
            if (myQueryResult.Errors.Any())
            {

                _StringBuilder.AppendLine(myQueryResult.Warnings.Count() + " errors(s): ");

                foreach (var _Error in myQueryResult.Errors)
                    _StringBuilder.AppendLine("  " + _Error.GetType().Name + " => " + _Error.ToString());

                _StringBuilder.AppendLine();

            }

            // results ------------------------------
            if (myQueryResult.Results.Any())
            {

                _StringBuilder.AppendLine();
                _StringBuilder.AppendLine(myQueryResult.Results.Count() + " result(s): ");

                foreach (var _SelectionListElementResult in myQueryResult.Results)
                    if (_SelectionListElementResult.Objects != null)
                        foreach (var _DBObject in _SelectionListElementResult.Objects)
                            _StringBuilder.Append(_DBObject.ToTEXT());

                _StringBuilder.AppendLine();

            }

            return _StringBuilder.ToString();

        }

        #endregion


        #region ToTEXT(myDBObjectReadout, myRecursion)

        public static Object ToTEXT(this DBObjectReadout myDBVertex)
        {
            return ToTEXT(myDBVertex, new StringBuilder(), 0);
        }

        #endregion

        #region (private) ToTEXT(myDBObjectReadout, myStringBuilder, myIndendLevel = 0)

        private static object ToTEXT(this DBObjectReadout myDBObjectReadout, StringBuilder myStringBuilder, Int32 myIndent = 0)
        {

            var _TypeObject = myDBObjectReadout["TYPE"] as GraphDBType;
            String _Type = (_TypeObject != null) ? _TypeObject.Name : "";
            var _UUID = myDBObjectReadout["UUID"];

            IEnumerable<DBObjectReadout> _DBObjects = null;
            IEnumerable<Object> _AttributeValueList = null;


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
                            for (var i = 0; i < myIndent; i++)
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
                                    _DBObjectReadout.ToTEXT(myStringBuilder, myIndent++);

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

    }

}
