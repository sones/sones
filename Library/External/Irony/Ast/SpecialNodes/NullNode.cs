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
using Irony.Parsing;
using Irony.Interpreter;

namespace Irony.Ast {
  //A stub to use when AST node was not created (type not specified on NonTerminal, or error on creation)
  // The purpose of the stub is to throw a meaningful message when interpreter tries to evaluate null node.
  public class NullNode : AstNode {

    public NullNode(BnfTerm term) {
      this.Term = term; 
    }
        
    public override void Evaluate(EvaluationContext context, AstMode mode) {
      context.ThrowError(Resources.ErrNullNodeEval, this.Term);  
    }
  }//class
}
