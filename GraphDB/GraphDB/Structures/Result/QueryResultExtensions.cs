

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace sones.GraphDB.Structures.Result
{

    public static class QueryResultExtensions
    {

        #region ToSimpleResult

        public static SimpleResult ToSimpleResult1(this QueryResult myQueryResult)
        {
            return myQueryResult.ToSimpleResult(0);
        }

        #endregion

        #region ToSimpleResult(mySelectionListNumber)

        public static SimpleResult ToSimpleResult(this QueryResult myQueryResult, int mySelectionListNumber)
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

            SelectionResultSet table = myQueryResult.Results.ElementAt(mySelectionListNumber);

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
