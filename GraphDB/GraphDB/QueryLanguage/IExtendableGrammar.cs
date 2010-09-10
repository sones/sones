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

using System.Collections.Generic;
using sones.GraphDB.Indices;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Aggregates;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Functions;
using sones.GraphDB.QueryLanguage.Operators;
using sones.GraphDB.Settings;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphFS.DataStructures;
using sones.GraphFS.Objects;
using sones.GraphDB.ImportExport;

namespace sones.GraphDB.QueryLanguage
{

    /// <summary>
    /// marks a grammar as extendable
    /// </summary>
    public interface IExtendableGrammar
    {

        void SetAggregates      (IEnumerable<ABaseAggregate>    aggregates);
        void SetFunctions       (IEnumerable<ABaseFunction>     functions);
        void SetOperators       (IEnumerable<ABinaryOperator>   operators);
        void SetSettings        (IEnumerable<ADBSettingsBase>   settings);
        void SetEdges           (IEnumerable<AEdgeType>         edges);
        void SetIndices         (IEnumerable<IVersionedIndexObject<IndexKey, ObjectUUID>> indices);
        void SetGraphDBImporter (IEnumerable<AGraphDBImport>    graphDBImporter);

    }
}
