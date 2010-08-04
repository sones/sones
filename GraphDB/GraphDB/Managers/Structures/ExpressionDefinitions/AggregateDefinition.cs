/*
 * AggregateDefinition
 * (c) Stefan Licht, 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;

#endregion

namespace sones.GraphDB.Managers.Structures
{
    public class AggregateDefinition : ATermDefinition
    {

        #region Properties

        public ChainPartAggregateDefinition ChainPartAggregateDefinition { get; private set; }

        #endregion

        #region Ctor

        public AggregateDefinition(ChainPartAggregateDefinition myChainPartAggregateDefinition)
        {
            ChainPartAggregateDefinition = myChainPartAggregateDefinition;
        }

        #endregion

    }
}
