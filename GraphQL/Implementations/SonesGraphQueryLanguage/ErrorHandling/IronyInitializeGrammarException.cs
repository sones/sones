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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphQL.ErrorHandling;
using Irony.Parsing;

namespace sones.GraphQL.GQL.ErrorHandling
{
    public sealed class IronyInitializeGrammarException : AGraphQLException
    {
        public String Info { get; private set; }

        private GrammarErrorList Errors;

		/// <summary>
		/// Initializes a new instance of the IronyInitializeGrammarException class.
		/// </summary>
		/// <param name="myErrors"></param>
		/// <param name="myInfo"></param>
		/// <param name="innerException">The exception that is the cause of the current exception, this parameter can be NULL.</param>
        public IronyInitializeGrammarException(GrammarErrorList myErrors, String myInfo, Exception innerException = null) : base(innerException)
        {
            Info = myInfo;

            Errors = myErrors;

            StringBuilder msg = new StringBuilder();

            msg.AppendLine("An error occurred during initializing the grammar: ");
            
            foreach (var error in myErrors)
            {
                msg.AppendLine(String.Format("{0} {1} {2}", error.Level.ToString(), error.Message, error.State.Name));
            }

            msg.AppendLine(Info);

            _msg = msg.ToString();
        }
    }
}
