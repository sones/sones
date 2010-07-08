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
 * <summary></summary>
 */

#region Usings

using System;
using System.Text;
using System.Collections.Generic;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure;
using sones.Lib.ErrorHandling;

#endregion

namespace sones.GraphDB.TypeManagement
{

    /// <summary>
    /// Internal datastructure
    /// </summary>

    

    public class GraphDBTypeDefinition
    {


        #region Properties

        #region Name

        private String _Name;

        public String Name
        {
            get
            {
                return _Name;
            }
        }

        #endregion

        #region ParentType

        private String _ParentType;

        public String ParentType
        {
            get
            {
                return _ParentType;
            }
        }

        #endregion

        #region Attributes

        private Dictionary<TypeAttribute, String> _Attributes;

        public Dictionary<TypeAttribute, String> Attributes
        {
            get
            {
                return _Attributes;
            }
        }

        #endregion

        #region KindOfType

        private KindsOfType _KindOfType;
        public KindsOfType KindOfType
        {
            get { return _KindOfType; }
            set { _KindOfType = value; }
        }

        #endregion

        #region BackwardEdgeNode

        private List<BackwardEdgeNode> _BackwardEdgeNodes;
        public List<BackwardEdgeNode> BackwardEdgeNodes
        {
            get { return _BackwardEdgeNodes; }
        }

        #endregion

        private List<Exceptional<IndexOptOnCreateTypeMemberNode>> _Indices;
        public List<Exceptional<IndexOptOnCreateTypeMemberNode>> Indices
        {
            get { return _Indices; }
        }

        #region Abstract
        private Boolean _IsAbstract;

        public Boolean IsAbstract
        {
            get { return _IsAbstract; }
        }
        #endregion

        #region Comment

        private String _Comment = String.Empty;

        /// <summary>
        /// A comment for the type
        /// </summary>
        public String Comment
        {
            get
            {
                return _Comment;
            }
        }

        #endregion

        #endregion


        #region Constructor

        public GraphDBTypeDefinition(String myName, String myParentType, Boolean myIsAbstract, Dictionary<TypeAttribute, String> myAttributes, List<BackwardEdgeNode> myBackwardEdgeNodes, List<Exceptional<IndexOptOnCreateTypeMemberNode>> myIndices, String myComment = null)
        {

            _Name       = myName;
            _ParentType = myParentType;
            _Attributes = myAttributes;
            _BackwardEdgeNodes = myBackwardEdgeNodes;
            _Indices = myIndices;
            _IsAbstract = myIsAbstract;
            _Comment = myComment;
            
            //TODO: KindOfType

        }

        public GraphDBTypeDefinition(String myName, String myParentType, Boolean myIsAbstract, Dictionary<String, String> myAttributes, String myComment = null)
        {

            _Name = myName;
            _ParentType = myParentType;
            _Attributes = new Dictionary<TypeAttribute, string>();
            _IsAbstract = myIsAbstract;
            _Comment = myComment;

            foreach (var attr in myAttributes)
                _Attributes.Add(new TypeAttribute() { Name = attr.Key}, attr.Value);

            //TODO: KindOfType

        }

        #endregion


    }

}
