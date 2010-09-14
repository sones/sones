

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using sones.GraphDB.Result;

#endregion

namespace sones.GraphDB.Result
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

            if (myQueryResult.Vertices == null || !myQueryResult.Vertices.Any()) { return sResult; }

            var table = myQueryResult.Vertices;

            var lData = table;
            int iNr = 0;
            int iCount = 0;
            IDictionary<String, Object> dict = null;
            Object[] oLineData = null;

            sResult.Header = new List<KeyValuePair<String, Object>>();

            foreach (var _Vertex in lData)
            {

                if (iNr == 0)
                {
                    foreach (var _Attribute in _Vertex)
                    {

                        sResult.Header.Add(new KeyValuePair<String, Object>(_Attribute.Key, _Attribute.Value));

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
