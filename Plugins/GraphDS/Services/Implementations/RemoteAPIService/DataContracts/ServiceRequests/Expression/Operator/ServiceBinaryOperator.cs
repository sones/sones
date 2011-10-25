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

using System.Runtime.Serialization;

namespace sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceRequests.Expression
{
    /// <summary>
    /// The enum for all binary operators like =, >= or AND
    /// </summary>
    [DataContract(Namespace = sonesRPCServer.Namespace)]
    public enum ServiceBinaryOperator
    {
        //TODO: Implement In, NotIN

        #region Comparative
        /// <summary>
        /// Comparative operators compare the left and right side of a binary expression
        /// </summary>
        [EnumMember]
        Equals,
        [EnumMember]
        GreaterOrEqualsThan,
        [EnumMember]
        GreaterThan,
        //In,
        [EnumMember]
        InRange,
        [EnumMember]
        LessOrEqualsThan,
        [EnumMember]
        LessThan,
        [EnumMember]
        NotEquals,
        //NotIn,
        [EnumMember]
        Like,

        #endregion

        #region Logic
        /// <summary>
        /// Logic operators process the result of two expressions
        /// </summary>

        [EnumMember]
        AND,
        [EnumMember]
        OR

        #endregion
    }
}