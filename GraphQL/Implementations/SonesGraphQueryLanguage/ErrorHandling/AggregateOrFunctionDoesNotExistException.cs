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
using sones.Library.ErrorHandling;

namespace sones.GraphQL.GQL.ErrorHandling
{
    public sealed class AggregateOrFunctionDoesNotExistException : AGraphQLException
    {
        #region data
        
        public String Info { get; private set; }
        public String AggrOrFuncName { get; private set; }
        public ASonesException Exception { get; private set; }
        public Type AggrOrFuncType { get; private set; }

        #endregion

        #region constructor
        
        /// <summary>
        /// Creates a new AggregateOrFunctionDoesNotExistException exception
        /// </summary>
        public AggregateOrFunctionDoesNotExistException(Type myAggrOrFuncType, String myAggrOrFuncName, String myInfo, ASonesException myException = null)
        {
            Info = myInfo;
            Exception = myException;
            AggrOrFuncType = myAggrOrFuncType;
            AggrOrFuncName = myAggrOrFuncName;
            
            if (Exception != null)
            {
                if (Exception.Message != null && !Exception.Message.Equals(""))
                    _msg = String.Format("Error during loading the aggregate plugin of type: [{0}] name: [{1}]\n\nInner Exception: {2}\n\n{3}", myAggrOrFuncType.ToString(), myAggrOrFuncName, Exception.Message, myInfo);
                else
                    _msg = String.Format("Error during loading the aggregate plugin of type: [{0}] name: [{1}]\n\n{2}", myAggrOrFuncType.ToString(), myAggrOrFuncName, myInfo);
            }
        }

        #endregion
    }
}
