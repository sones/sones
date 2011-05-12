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
  public class LiteralValueNode : AstNode {
    public object Value; 

    public override void Init(ParsingContext context, ParseTreeNode treeNode) {
      base.Init(context, treeNode); 
      Value = treeNode.Token.Value;
      AsString = Value == null ? "null" : Value.ToString();
      if (Value is string)
        AsString = "\"" + AsString + "\""; 
    }

    public override void EvaluateNode(EvaluationContext context, AstMode mode) {
      switch (mode) {
        case AstMode.Read: 
          context.Data.Push(Value); 
          break;
        case AstMode.Write: 
          context.ThrowError(Resources.ErrAssignLiteralValue);  
          break;  
      }
    }
  
  }//class
}
