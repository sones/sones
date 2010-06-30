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
 * TEXTRepresentation
 * Achim Friedland, 2009 - 2010
 */

#region Usings

using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using sones.Lib.CLI;
using sones.GraphDB.TypeManagement;

#endregion

namespace sones.GraphDB.QueryLanguage.Result
{

    public static class TEXTRepresentation
    {


        public static String toTEXT(this QueryResult myQueryResult, CLI_Output myOutputFormat)
        {

            #region Data

            var _ReturnValue = new StringBuilder();
            var _hasError = false;

            #endregion

            #region print ErrorCode

            foreach (var _PandoraError in myQueryResult.Errors)
            {
                _ReturnValue.AppendLine("ERROR!");
                _ReturnValue.AppendLine("Errorclass: " + _PandoraError.GetType().ToString());
                _ReturnValue.AppendLine(_PandoraError.ToString());
                if (myOutputFormat == CLI_Output.Standard)
                    _ReturnValue.AppendLine(Environment.NewLine);
                _hasError = true;
            }



            #endregion

            #region print result

            if (!_hasError)
            {

                int nrOfSelList = myQueryResult.Results.Count();

                foreach (var _SelectionListElementResult in myQueryResult.Results)
                {
                    if (_SelectionListElementResult.Objects != null)
                    {
                        foreach (var _DBObjectReadout in _SelectionListElementResult.Objects)
                        {
                            toTEXT(_DBObjectReadout, 0, ref _ReturnValue, myOutputFormat);
                            if (myOutputFormat == CLI_Output.Standard)
                                _ReturnValue.AppendLine(Environment.NewLine);
                        }

                        if (myOutputFormat == CLI_Output.Standard)
                            _ReturnValue.AppendLine(Environment.NewLine);
                    }
                }

            }

            #endregion

            return _ReturnValue.ToString();

        }



        #region toTEXT(dataObject, level)

        private static void toTEXT(this DBObjectReadout myDBObjectReadout, UInt16 level, ref StringBuilder myStringBuilder, CLI_Output myOutputFormat)
        {

            foreach (var _Attribute in myDBObjectReadout.Attributes)
            {
                if (_Attribute.Value != null)
                {

                    if (_Attribute.Value is DBObjectReadout)
                    {
                        myStringBuilder.AppendLine(_Attribute.Key.ToString() + " <resolved>:");
                        toTEXT((DBObjectReadout)_Attribute.Value, (UInt16)(level + 1), ref myStringBuilder, myOutputFormat);
                    }

                    else
                    {
                        if (_Attribute.Value is IEnumerable<DBObjectReadout>)
                        {
                            for (UInt16 i = 0; i < level; i++)
                            {
                                myStringBuilder.Append("\t");
                            }
                            myStringBuilder.AppendLine(_Attribute.Key.ToString() + " <resolved>:");

                            foreach (var _DBObjectReadout in (IEnumerable<DBObjectReadout>)_Attribute.Value)
                            {
                                toTEXT(_DBObjectReadout, (UInt16)(level + 1), ref myStringBuilder, myOutputFormat);
                                if (myOutputFormat == CLI_Output.Standard)
                                {
                                    myStringBuilder.Append(Environment.NewLine);
                                    myStringBuilder.Append(Environment.NewLine);
                                }
                            }
                            if (myOutputFormat == CLI_Output.Standard)
                                myStringBuilder.AppendLine();
                        }
                        else
                        {
                            for (UInt16 i = 0; i < level; i++)
                            {
                                myStringBuilder.Append("\t");
                            }

                            if((_Attribute.Value is GraphDBType[]))
                            {
                                myStringBuilder.AppendLine(_Attribute.Key.ToString() + " = " + ((GraphDBType[])_Attribute.Value).First().Name + " []");

                            }else if (_Attribute.Value is GraphDBType)
                            {   
                                myStringBuilder.AppendLine(_Attribute.Key.ToString() + " = " + ((GraphDBType)_Attribute.Value).Name);
                            }
                            else if (_Attribute.Value is TypeAttribute)
                            {
                                myStringBuilder.AppendLine(_Attribute.Key.ToString() + " = " + ((TypeAttribute)_Attribute.Value).Name);
                            }
                            else
                                myStringBuilder.AppendLine(_Attribute.Key.ToString() + " = " + _Attribute.Value.ToString());
                        }
                    }

                }
                else
                {

                    for (UInt16 i = 0; i < level; i++)
                        myStringBuilder.Append("\t");

                    myStringBuilder.AppendLine(_Attribute.Key.ToString() + " = not resolved");

                }
            }
        }

        #endregion


    }

}
