/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
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
*/

/* <id name="sones GraphDB – PandoraTypeDefinition" />
 * <copyright file="PandoraTypeDefinition.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Achim 'ahzf' Friedland</developer>
 * <developer>Stefan Licht</developer>
 * <summary></summary>
 */

#region Usings

using System;
using System.Text;
using System.Collections.Generic;

using sones.Lib.ErrorHandling;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.TypeManagement;

#endregion

namespace sones.GraphDB.Managers.Structures
{

    /// <summary>
    /// Internal datastructure
    /// </summary>
    public class GraphDBTypeDefinition
    {

        #region Properties

        #region Name

        public String Name { get; private set; }

        #endregion

        #region ParentType

        public String ParentType
        {
            get;
            private set;
        }

        #endregion

        #region Attributes

        public Dictionary<AttributeDefinition, String> Attributes
        {
            get;
            private set;
        }

        #endregion

        #region BackwardEdgeNode

        public List<BackwardEdgeDefinition> BackwardEdgeNodes
        {
            get;
            private set;
        }

        #endregion

        #region Indices

        public List<Exceptional<IndexDefinition>> Indices
        {
            get;
            private set;
        }

        #endregion

        #region Abstract

        public Boolean IsAbstract
        {
            get;
            private set;
        }

        #endregion

        #region Comment

        /// <summary>
        /// A comment for the type
        /// </summary>
        public String Comment
        {
            get;
            private set;
        }

        #endregion

        #endregion

        #region Constructor

        public GraphDBTypeDefinition(String myName, String myParentType, Boolean myIsAbstract, Dictionary<AttributeDefinition, String> myAttributes, List<BackwardEdgeDefinition> myBackwardEdgeNodes = null, List<Exceptional<IndexDefinition>> myIndices = null, String myComment = null)
        {

            Name       = myName;
            ParentType = myParentType;
            Attributes = myAttributes;
            BackwardEdgeNodes = myBackwardEdgeNodes;
            Indices = myIndices;
            IsAbstract = myIsAbstract;
            Comment = myComment;

        }

        #endregion


    }

}
