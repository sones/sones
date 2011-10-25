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
using sones.GraphDB.Expression.Tree;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceRequests.Expression
{
    /// <summary>
    /// A collection literal expression
    /// </summary>
    [DataContract(Namespace = sonesRPCServer.Namespace)]
    public class ServiceCollectionLiteralExpression : ServiceBaseExpression
    {
   
        /// <summary>
        /// A collection literal expression... sth like [13, 14, 15] or ["Alice", "Bob", "Carol"]
        /// </summary>
        [DataMember]
        public List<Object> CollectionLiteral;

 
    }
}
