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

namespace sones.Library.Commons.VertexStore.Definitions.Update
{
    /// <summary>
    /// The update for graph elements
    /// </summary>
    public abstract class AGraphElementUpdateDefinition
    {
        /// <summary>
        /// A comment for the graphelement
        /// </summary>
        public readonly String CommentUpdate;

        /// <summary>
        /// The structured properties
        /// </summary>
        public readonly StructuredPropertiesUpdate UpdatedStructuredProperties;

        /// <summary>
        /// The unstructured properties
        /// </summary>
        public readonly UnstructuredPropertiesUpdate UpdatedUnstructuredProperties;

        #region constructor

        /// <summary>
        /// Creates a new graph element update definition
        /// </summary>
        /// <param name="myCommentUpdate">A comment for the graphelement</param>
        /// <param name="myUpdatedStructuredProperties">The structured properties</param>
        /// <param name="myUpdatedUnstructuredProperties">The unstructured properties</param>
        protected AGraphElementUpdateDefinition(
            String myCommentUpdate = null, 
            StructuredPropertiesUpdate myUpdatedStructuredProperties = null, 
            UnstructuredPropertiesUpdate myUpdatedUnstructuredProperties = null)
        {
            CommentUpdate = myCommentUpdate;
            UpdatedStructuredProperties = myUpdatedStructuredProperties;
            UpdatedUnstructuredProperties = myUpdatedUnstructuredProperties;
        }

        #endregion
    }

}
