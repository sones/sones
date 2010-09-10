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
using System.Linq;
using System.Text;
using sones.GraphFS.DataStructures;
using sones.Lib.DataStructures.UUID;

namespace sones.GraphDB.QueryLanguage.ExpressionGraph
{
    public class ExpressionLevelEntry : IExpressionLevelEntry
    {

        Dictionary<ObjectUUID, IExpressionNode> _Objects;
        LevelKey _CorrespondingLevelKey;

        public ExpressionLevelEntry(LevelKey myCorrespondingLevelKey, Dictionary<ObjectUUID, IExpressionNode> myObjects)
        {
            _Objects = myObjects;
            _CorrespondingLevelKey = myCorrespondingLevelKey;
        }

        public ExpressionLevelEntry(LevelKey myCorrespondingLevelKey)
            : this(myCorrespondingLevelKey, new Dictionary<ObjectUUID, IExpressionNode>())
        { }

        #region IExpressionLevelEntry Members

        public Dictionary<ObjectUUID, IExpressionNode> Nodes
        {
            get { return _Objects; }
        }

        public LevelKey CorrespondingLevelKey
        {
            get { return _CorrespondingLevelKey; }
        }

        #endregion

    }
}
