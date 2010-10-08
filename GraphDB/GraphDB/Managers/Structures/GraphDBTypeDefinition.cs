/* <id name="GraphDBTypeDefinitionn" />
 * <copyright file="GraphDBTypeDefinition.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
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

        public List<IndexDefinition> Indices
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

        public GraphDBTypeDefinition(String myName, String myParentType, Boolean myIsAbstract, Dictionary<AttributeDefinition, String> myAttributes, List<BackwardEdgeDefinition> myBackwardEdgeNodes = null, List<IndexDefinition> myIndices = null, String myComment = null)
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
