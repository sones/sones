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
using System.Runtime.Serialization;
using sones.GraphDB.Request;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceRequests
{
    [DataContract(Namespace = sonesRPCServer.Namespace)]
    public class ServiceEdgeTypePredefinition
    {
        /// <summary>
        /// The name of the edge type that is going to be created
        /// </summary>
        [DataMember(IsRequired = true)]
        public string EdgeTypeName;
        
        /// <summary>
        /// The name of the edge type this vertex types inherites from.
        /// </summary>
        [DataMember]
        public string SuperEdgeTypeName;

        /// <summary>
        /// The properties of the edge type.
        /// </summary>
        [DataMember]
        public IEnumerable<ServicePropertyPredefinition> Properties;
        
        /// <summary>
        /// Gets if the edge type will be sealed.
        /// </summary>
        [DataMember]
        public bool IsSealed;

        /// <summary>
        /// Gets the comment for this edge type.
        /// </summary>
        [DataMember]
        public string Comment;


        public EdgeTypePredefinition ToEdgeTypePredefinition()
        {
            EdgeTypePredefinition EdgeTypePreDef = new EdgeTypePredefinition(this.EdgeTypeName);
           
            if (this.IsSealed)
                EdgeTypePreDef.MarkAsSealed();

            EdgeTypePreDef.SetComment(this.Comment);
            EdgeTypePreDef.SetSuperTypeName(this.SuperEdgeTypeName);

            if (this.Properties != null)
            {
                foreach (var Property in this.Properties)
                {
                    EdgeTypePreDef.AddProperty(Property.ToPropertyPredefinition());
                }
            }

            return EdgeTypePreDef;
        }
    }
}
