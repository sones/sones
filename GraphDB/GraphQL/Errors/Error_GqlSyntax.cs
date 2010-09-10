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
using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.Errors
{

    public class Error_GqlSyntax : GraphDBError
    {

        public String Info { get; private set; }
        public Exception Exception { get; private set; }
        public SyntaxError SyntaxError { get; private set; }
        public String SyntaxErrorMessage
        {
            get
            {
                return SyntaxError.Message;
            }
        }

        public Error_GqlSyntax(SyntaxError mySyntaxError, String myQuery)
        {
            SyntaxError = mySyntaxError;
            Info = myQuery;
        }

        public Error_GqlSyntax(String myInfo)
        {
            Info = myInfo;
        }

        public Error_GqlSyntax(String myInfo, Exception myException)
        {
            Info = myInfo;
            Exception = myException;
        }

        public override string ToString()
        {

            if (SyntaxError != null)
            {
                if (SyntaxError.Exception != null)
                    return String.Format("Syntax error in query: [{0}]\n\n gql: [{1}]\n\nAt position: {2}\n\nException:{3}", Info, SyntaxError.Message, SyntaxError.Location.ToString(), SyntaxError.Exception.ToString());
                else
                    return String.Format("Syntax error in query: [{0}]\n\n gql: [{1}]\n\nAt position: {2}", Info, SyntaxError.Message, SyntaxError.Location.ToString());
            }

            else if (Exception != null)
            {
                return Info + Environment.NewLine + Exception.Message;
            }

            else
            {
                return Info;
            }

        }
        
    }

}
