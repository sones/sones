/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

#region Usings

using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using sones.Library.LanguageExtensions;
using sones.Library.Network.HttpServer;

#endregion

namespace sones.Plugins.GraphDS.RESTService
{
    public class GraphDSREST_Errors
    {
        #region Data

        private String _ServerID;

        #endregion

        #region Constructors

        public GraphDSREST_Errors(String myServerID)
        {
            _ServerID = myServerID;
        }

        #endregion

        #region BadRequest(myErrorMessage)

        public void Error400_BadRequest(String myErrorMessage)
        {
            var response = HttpServer.HttpContext.Response;

            response.SetHttpStatusCode(HttpStatusCode.BadRequest);
            response.SetCacheControl("no-cache");
            response.SetServerName(_ServerID);
            response.SetContentType(GraphDSREST_Constants._TEXT_UTF8);
            //response.StatusDescription = "Error 400 - Bad Request : " + myErrorMessage;

        }

        #endregion

        #region QueryFailed(myErrorMessage)

        public void Error400_QueryFailed(String myErrorMessage)
        {
            var response = HttpServer.HttpContext.Response;

            response.SetHttpStatusCode(HttpStatusCode.BadRequest);
            response.SetCacheControl("no-cache");
            response.SetServerName(_ServerID);
            response.SetContentType(GraphDSREST_Constants._TEXT_UTF8);
            //response.StatusDescription = "Error 400 - Query Failed : " + myErrorMessage;

        }

        #endregion

        #region Error404_NotFound(myStream)

        public void Error404_NotFound(Stream myCustom404Error)
        {
            var response = HttpServer.HttpContext.Response;

            response.SetHttpStatusCode(HttpStatusCode.NotFound);
            response.SetCacheControl("no-cache");
            response.SetServerName(_ServerID);

            #region Send custom Error404page...

            if (myCustom404Error != null && myCustom404Error.Length > 0)
            {
                response.SetContentType(GraphDSREST_Constants._HTML);
            }

            #endregion

            #region ...or non-custom Error404page!

            else
            {
                response.SetContentType(GraphDSREST_Constants._TEXT_UTF8);
            }

            #endregion

            myCustom404Error.CopyTo(response.OutputStream);

        }

        #endregion

        #region Error406_NotAcceptable(myStream)

        public void Error406_NotAcceptable(String myCustom406Error)
        {
            var response = HttpServer.HttpContext.Response;

            response.SetHttpStatusCode(HttpStatusCode.NotAcceptable);
            response.SetCacheControl("no-cache");
            response.SetServerName(_ServerID);
            response.SetContentType(GraphDSREST_Constants._TEXT_UTF8);
            //response.StatusDescription = "Error 406 - Not Acceptable : " + myCustom406Error;
        }

        #endregion

        #region checkAuthentication(mySettings)

        public Boolean checkAuthentication(GraphDSREST_Settings mySettings)
        {

            if (mySettings != null)
            {
                Debug.WriteLine("Currently not authentication mechanism was choosen!");
                return true;
            }

            else
                return false;

        }

        #endregion

        #region Print Error Messages internally

       /* public String PrintErrorToString(ResultType myResultType, IEnumerable<IError> myIErrors)
        {

            var Output = new StringBuilder();

            if (myResultType == ResultType.Successful)
            {
                Output.AppendLine("OK");
            }

            else
            {
                foreach (var aError in myIErrors)
                {
                    Output.AppendLine("ERROR!");
                    Output.AppendLine("Errorclass: " + aError.GetType().Name);
                    Output.AppendLine(aError.ToString());
                    Output.AppendLine(Environment.NewLine);
                }
                return Output.ToString();
            }

            return Output.ToString();
        }*/

        #endregion
    }
}
