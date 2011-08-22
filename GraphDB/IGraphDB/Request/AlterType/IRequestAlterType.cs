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
using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// Request to alter a type.
    /// </summary>
    public interface IRequestAlterType: IRequest
    {
        #region Data

        /// <summary>
        /// The name of the type that is going to be altered
        /// </summary>
        string TypeName { get; }
        
        #region counter

        int                                         AddPropertyCount { get; }
        int                                         AddUnknownPropertyCount { get; }
        int                                         AddAttributeCount { get; }
        int                                         RemoveAttributeCount { get; }
        int                                         RenameAttributeCount { get; }

        #endregion

        #region add

        /// <summary>
        /// Properties to be added to the altered type.
        /// </summary>
        IEnumerable<PropertyPredefinition>          ToBeAddedProperties { get; }

        /// <summary>
        /// Unknown attributes to be added to the altered type.
        /// </summary>
        IEnumerable<UnknownAttributePredefinition>  ToBeAddedUnknownAttributes { get; }

        #endregion

        #region remove

        /// <summary>
        /// Properties to be removed from the altered type.
        /// </summary>
        IEnumerable<String>                         ToBeRemovedProperties { get; }

        /// <summary>
        /// Incoming edges to be removed from the altered type.
        /// </summary>
        IEnumerable<String>                         ToBeRemovedUnknownAttributes { get; }

        /// <summary>
        /// To reset the IEnumerable which holds the to be removed unknown attributes.
        /// </summary>
        void ClearToBeRemovedUnknownAttributes();

        #endregion

        #region rename

        /// <summary>
        /// The renamed attributes
        /// </summary>
        Dictionary<String, String>                  ToBeRenamedProperties { get; }

        #endregion

        /// <summary>
        /// Gets the altered comment for this type.
        /// </summary>
        string                                      AlteredComment { get; }

        /// <summary>
        /// Gets the altered type name
        /// </summary>
        string                                      AlteredTypeName { get; }

        #endregion

        /// <summary>
        /// To reset the IEnumerable which holds the unknown attributes.
        /// </summary>
        void ResetUnknown();
    }
}
