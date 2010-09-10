/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
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
* 
*/

using System;
using System.Collections.Generic;
using sones.GraphDB.Indices;
using sones.GraphDB.QueryLanguage.ExpressionGraph;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;
using sones.GraphDB.TypeManagement;

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Statements.Select
{


    class SelectionElement
    {
        public String Alias { get; set; }
        public List<IDNode> ContainingIDNodes { get; set; }

        /// <summary>
        /// In this case, we will return ALL attributes of each DBO
        /// </summary>
        public Boolean IsAsterisk { get; set; }

        public Boolean IsGroupedOrAggregated { get; set; }
        public AttributeIndex IndexAggregate { get; set; }

        #region Element

        private TypeAttribute _Element;
        // TODO: change this to a NON object!!
        /// <summary>
        /// Either an TypeAttribute, a FuncCallNode or AggregateNode
        /// </summary>
        public TypeAttribute Element
        {
            get
            {
                return _Element;
            }
            set
            {
                _Element = value;
            }
        }

        #endregion

        public AStructureNode StructureNode { get; set; }

        /// <summary>
        /// Checks whether this element is a reference AND has some following selections
        /// </summary>
        /// <returns></returns>
        public Boolean IsReferenceToSkip(LevelKey levelKey)
        {
            if (Element != null)
            {
                if (Element.KindOfType == KindsOfType.SetOfReferences || Element.KindOfType == KindsOfType.SingleReference)
                {
                    if (ContainingIDNodes[0].Depth > levelKey.Depth + 1) // if the IDNode is only one level above we can't skip this selection elemen because it is the last one: U.Friends 
                        return true;
                    else
                    {
                        return false;
                        //return (ContainingIDNodes[0].Edges.Count > levelKey.Edges.Count);
                        /*
                        if (ContainingIDNodes[0].LastAttribute.DBType.IsUserDefined)
                            return (ContainingIDNodes[0].Edges.Count > levelKey.Edges.Count);
                        else
                            return false;
                        */
                    }
                }
            }

            return false;
        }
    }
}
