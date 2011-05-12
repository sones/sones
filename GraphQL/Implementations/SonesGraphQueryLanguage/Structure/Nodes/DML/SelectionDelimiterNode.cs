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
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.Structure.Helper.Enums;
using sones.GraphQL.ErrorHandling;

namespace sones.GraphQL.Structure.Nodes.DML
{
    public sealed class SelectionDelimiterNode : AStructureNode, IAstNodeInit
    {
        #region Data

        private KindOfDelimiter _KindOfDelimiter;

        #endregion

        #region constructor

        public SelectionDelimiterNode()
        {

        }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
        }

        #endregion

        #region public methods

        internal void SetDelimiter(KindOfDelimiter myKindOfDelimiter)
        {
            _KindOfDelimiter = myKindOfDelimiter;
        }

        public KindOfDelimiter GetKindOfDelimiter()
        {
            return _KindOfDelimiter;
        }

        public String GetDelimiterString()
        {
            switch (_KindOfDelimiter)
            {
                case KindOfDelimiter.Dot:
                    return SonesGQLConstants.EdgeTraversalDelimiterSymbol;

                case KindOfDelimiter.EdgeInformationDelimiter:
                    return SonesGQLConstants.EdgeInformationDelimiterSymbol;

                default:
                    throw new NotImplementedQLException("");
            }
        }

        #endregion
    }
}
