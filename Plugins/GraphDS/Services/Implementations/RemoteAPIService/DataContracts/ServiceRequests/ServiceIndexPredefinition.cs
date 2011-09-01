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
    public class ServiceIndexPredefinition
    {
                     
        [DataMember]
        public string Edition;

        /// <summary>
        /// The name of the index type.
        /// </summary>
        [DataMember]
        public string TypeName;
        
        /// <summary>
        /// The name of the index
        /// </summary>
        [DataMember]
        public string Name;

        /// <summary>
        /// The options that will be passed to the index instance
        /// </summary>
        [DataMember]
        public String IndexOptions;

        

        /// <summary>
        /// The set of properties that will be indexed.
        /// </summary>
        [DataMember]
        public IEnumerable<String> Properties;

        /// <summary>
        /// The vertexTypeName that defines the index.
        /// </summary>
        [DataMember]
        public string VertexTypeName;

        [DataMember]
        public string Comment;


        public IndexPredefinition ToIndexPredefinition()
        {
            IndexPredefinition IndexPreDef = new IndexPredefinition(this.Name);

            IndexPreDef.SetComment(this.Comment);
            IndexPreDef.SetEdition(this.Edition);

            foreach (var Property in this.Properties)
            {
                IndexPreDef.AddProperty(Property);
            }

            IndexPreDef.SetVertexType(this.VertexTypeName);

            return IndexPreDef;
        }

    }
}
