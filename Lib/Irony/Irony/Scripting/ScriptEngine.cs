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
using sones.Lib.Frameworks.Irony.Parsing;
using sones.Lib.Frameworks.Irony.Scripting.Ast;
using sones.Lib.Frameworks.Irony.Scripting.Runtime;

namespace sones.Lib.Frameworks.Irony.Scripting {
  public class ScriptEngine {
    public readonly LanguageData Language;
    public readonly Compiler Compiler;

    #region constructors
    public ScriptEngine(Compiler compiler) {
      Compiler = compiler;
      Language = Compiler.Language;
    }
    public ScriptEngine(LanguageData language) {
      Language = language;
      Compiler = new Compiler(Language); 
    }
    public ScriptEngine(Grammar grammar) {
      Compiler = new Compiler(grammar);
      Language = Compiler.Language;
    }
    #endregion

/*
    public void AnalyzeCode(ParseTree parseTree, CompilerContext context) {
      var astRoot = parseTree.Root.AstNode as AstNode;
      if (astRoot == null) return;
      RunAnalysisPhases(astRoot, context,
           CodeAnalysisPhase.Init, CodeAnalysisPhase.AssignScopes, CodeAnalysisPhase.Allocate,
           CodeAnalysisPhase.Binding, CodeAnalysisPhase.MarkTailCalls, CodeAnalysisPhase.Optimization);
      //sort errors if there are any
      if (context.CurrentParseTree.Errors.Count > 0)
        context.CurrentParseTree.Errors.Sort(SyntaxErrorList.ByLocation);
    }

    private void RunAnalysisPhases(AstNode astRoot, CompilerContext context, params CodeAnalysisPhase[] phases) {
      CodeAnalysisArgs args = new CodeAnalysisArgs(context);
      foreach (CodeAnalysisPhase phase in phases) {
        switch (phase) {
          case CodeAnalysisPhase.AssignScopes:
            astRoot.Scope = new Scope(astRoot, null);
            break;

          case CodeAnalysisPhase.MarkTailCalls:
            if (!Language.Grammar.FlagIsSet(LanguageFlags.TailRecursive)) continue;//foreach loop - don't run the phase
            astRoot.Flags |= AstNodeFlags.IsTail;
            break;
        }//switch
        args.Phase = phase;
        astRoot.OnCodeAnalysis(args);
      }//foreach phase
    }//method
  
 */

  
  }//class
}
