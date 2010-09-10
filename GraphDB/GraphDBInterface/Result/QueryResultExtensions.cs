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



#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using sones.GraphDBInterface.Result;

#endregion

namespace sones.GraphDBInterface.Result
{

    public static class QueryResultExtensions
    {

        #region ToSimpleResult

        public static SimpleResult ToSimpleResult1(this QueryResult myQueryResult)
        {
            return myQueryResult.ToSimpleResult();
        }

        #endregion

        #region ToSimpleResult(mySelectionListNumber)

        public static SimpleResult ToSimpleResult(this QueryResult myQueryResult)
        {

            var sResult = new SimpleResult();

            if (myQueryResult.ResultType != ResultType.Successful)
            {

                sResult.iResultType = SimpleResult.Failed;

                var errors = myQueryResult.Errors.ToList();
                sResult.Errors = new List<String>();

                foreach (var error in errors)
                    sResult.Errors.Add(error.ToString());

                return sResult;

            }

            if (myQueryResult.Results == null || !myQueryResult.Results.Any()) { return sResult; }

            SelectionResultSet table = myQueryResult.Results;

            var lData = table.Objects;
            int iNr = 0;
            int iCount = 0;
            IDictionary<String, Object> dict = null;
            Object[] oLineData = null;

            sResult.Header = new List<KeyValuePair<String, Object>>();

            foreach (var _DBObjectReadout in lData)
            {

                dict = _DBObjectReadout.Attributes;
                if (iNr == 0)
                {
                    foreach (KeyValuePair<String, Object> attribute in dict)
                    {
                        sResult.Header.Add(new KeyValuePair<String, Object>(attribute.Key, attribute.Value));

                        //if (attribute.Value is String)        type = ""; 
                        //else if (attribute.Value is int)      type = 0; 
                        //else if (attribute.Value is double)   type = 0D; 
                        //else if (attribute.Value is DateTime) type = DateTime.Now; 
                        //else Console.WriteLine(attribute.Value.ToString());

                        //sResult.Header.Add(new KeyValuePair<String, Object>(attribute.Key, type));

                        iNr++;
                    }
                    sResult.Data = new List<object[]>();
                }
                if (iNr == 0) break;  // no attributes ???

                oLineData = new object[iNr];
                iCount = 0;

                foreach (var data in dict)
                {
                    if (iCount == iNr) break; // reached array end.....should not happen
                    oLineData[iCount] = data.Value;
                    iCount++;
                }

                sResult.Data.Add(oLineData);

            }

            sResult.iResultType = SimpleResult.Successful;
            // end values to be filled

            return sResult;

        }
        #endregion

    }

}
