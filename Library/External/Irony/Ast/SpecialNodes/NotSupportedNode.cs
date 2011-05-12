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
  //A substitute node to use on constructs that are not yet supported by language implementation.
  // The script would compile Ok but on attempt to evaluate the node would throw a runtime exception
  public class NotSupportedNode : AstNode {
    string Name; 
    public override void  Init(ParsingContext context, ParseTreeNode treeNode)  {
 	    base.Init(context, treeNode);
      Name = treeNode.Term.ToString();
      AsString = Name + " (not supported)";
    }

    public override void EvaluateNode(EvaluationContext context, AstMode mode) {
      context.ThrowError(Resources.ErrConstructNotSupported, Name); 
    }

  }//class
}
