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


using System;
using System.Collections.Generic;
using sones.GraphDB.Indices;
using sones.GraphDB.QueryLanguage.ExpressionGraph;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Functions;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Aggregates;

namespace sones.GraphDB.Managers.Select
{

    #region SelectionElement

    /// <summary>
    /// Stores some information about the current selection element. This is one part of a IDCHain.
    /// </summary>
    public class SelectionElement
    {

        #region Properties

        public IDChainDefinition RelatedIDChainDefinition { get; set; }
        public String Alias { get; set; }
        public EdgeList EdgeList { get; protected set; }
        public LevelKey LevelKey { get; protected set; }
        public Boolean IsGroupedOrAggregated { get; protected set; }
        public Boolean IsAsterisk { get; protected set; }
        public TypeAttribute Element { get; set; }

        #endregion

        #region Ctors

        public SelectionElement() { }

        public SelectionElement(Boolean myIsAsterisk)
        {
            IsAsterisk = myIsAsterisk;
        }

        public SelectionElement(string myAlias, IDChainDefinition myRelatedIDChainDefinition)
        {
            Alias = myAlias;
            RelatedIDChainDefinition = myRelatedIDChainDefinition;
        }

        public SelectionElement(string myAlias, Select.EdgeList myEdgeList, bool myIsGroupedOrAggregated, IDChainDefinition myRelatedIDChainDefinition, TypeAttribute myElement = null)
            : this(myAlias, myRelatedIDChainDefinition)
        {
            EdgeList = myEdgeList;
            IsGroupedOrAggregated = myIsGroupedOrAggregated;
            Element = myElement;
        }

        #endregion

        /// <summary>
        /// Checks whether this element is a reference AND has some following selections
        /// </summary>
        /// <returns></returns>
        public Boolean IsReferenceToSkip(EdgeList myEdgeList)
        {
            if (Element != null)
            {
                if (Element.KindOfType == KindsOfType.SetOfReferences || Element.KindOfType == KindsOfType.SingleReference)
                {
                    if (RelatedIDChainDefinition.Edges.Count > myEdgeList.Edges.Count) // if the IDNode is only one level above we can't skip this selection elemen because it is the last one: U.Friends 
                        return true;
                    else
                    {
                        return false;
                    }
                }
            }

            return false;
        }

    }

    #endregion

    #region SelectionElementFunction

    public class SelectionElementFunction : SelectionElement
    {

        public ChainPartFuncDefinition Function { get; private set; }
        public List<AExpressionDefinition> Parameters { get; private set; }

        public SelectionElementFunction(SelectionElement mySelectionElement, ChainPartFuncDefinition myFunction, List<AExpressionDefinition> myParameters)
        {
            this.Alias = mySelectionElement.Alias;
            this.EdgeList = mySelectionElement.EdgeList;
            this.Element = mySelectionElement.Element;
            this.IsAsterisk = mySelectionElement.IsAsterisk;
            this.IsGroupedOrAggregated = mySelectionElement.IsGroupedOrAggregated;
            this.LevelKey = mySelectionElement.LevelKey;
            this.RelatedIDChainDefinition = mySelectionElement.RelatedIDChainDefinition;

            Function = myFunction;
            Parameters = myParameters;
        }

    }

    #endregion

    #region SelectionElementAggregate

    public class SelectionElementAggregate : SelectionElement
    {

        #region Properties

        public ABaseAggregate Aggregate { get; private set; }
        public AggregateDefinition AggregateDefinition { get; private set; }
        public IDChainDefinition Parameter { get; private set; }
        public AAttributeIndex IndexAggregate { get; set; }

        #endregion

        #region Ctor

        public SelectionElementAggregate(ABaseAggregate myBaseAggregate, string myAlias, EdgeList myEdgeList, LevelKey myLevelKey, IDChainDefinition myParameter, AggregateDefinition myAggregateDefinition)
        {
            Alias = myAlias;
            EdgeList = myEdgeList;
            LevelKey = myLevelKey;
            Aggregate = myBaseAggregate;
            AggregateDefinition = myAggregateDefinition;
            Parameter = myParameter;
        }

        #endregion

    }

    #endregion

}
