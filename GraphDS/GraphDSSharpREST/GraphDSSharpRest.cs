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

/* GraphDSSharpRest
 * (c) Stefan Licht, 2009-2010
 * 
 * Lead programmer:
 *      Stefan Licht
 * 
 */

#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.Transactions;
using sones.GraphFS.Transactions;

#endregion

namespace sones.GraphDS.API.CSharp
{

    public class GraphDSSharpRest : AGraphDSSharp
    {

        #region Data

        private String _RestURL = null;
        private string username = null;
        private string password = null;

        #endregion

        #region Properties

        #endregion


        #region Constructors

        #region GraphDBSharpRest()

        public GraphDSSharpRest(Uri myRestURI, String myDatabase, String myUsername, String myPassword)
        {
            
            _RestURL = String.Format("{0}gql?", myRestURI.ToString());
            username = myUsername;
            password = myPassword;

        }

        #endregion

        #endregion

        #region Query(myQuery)

        public override QueryResult Query(String myQuery)
        {

            var readout = new DBObjectReadout();
            readout.Attributes.Add("query", QueryXml(myQuery));
            return new QueryResult(new SelectionResultSet(new List<DBObjectReadout>() { readout }));

        }

        internal String QueryXml(String myQuery)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_RestURL + HttpUtility.UrlEncode(myQuery));
            
            #region Add credentials

            string usernamePassword = username + ":" + password;
            if (!String.IsNullOrEmpty(usernamePassword))
            {
                CredentialCache ccache = new CredentialCache();
                ccache.Add(request.RequestUri, "Basic", new NetworkCredential(username, password));
                request.Credentials = ccache;
                request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(new ASCIIEncoding().GetBytes(usernamePassword)));
            }
            
            #endregion

            request.Accept = "application/xml";

            var stream = new StreamReader(request.GetResponse().GetResponseStream());
            var result = stream.ReadToEnd();

            return result; // .FromBase64();

        }

        #endregion


        public override SelectToObjectGraph QuerySelect(String myQuery)
        {
            return new SelectToObjectGraph(QueryXml(myQuery));
        }


        public override DBTransaction BeginTransaction(Boolean myDistributed = false, Boolean myLongRunning = false, IsolationLevel myIsolationLevel = IsolationLevel.Serializable, String myName = "", DateTime? myCreated = null)
        {
            throw new NotImplementedException();
        }

    }
}
