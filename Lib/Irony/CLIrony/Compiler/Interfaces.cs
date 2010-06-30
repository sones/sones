#region License
/* **********************************************************************************
 * Copyright (c) Roman Ivantsov
 * This source code is subject to terms and conditions of the MIT License
 * for CLIrony. A copy of the license can be found in the License.txt file
 * at the root of this distribution. 
 * By using this source code in any fashion, you are agreeing to be bound by the terms of the 
 * MIT License.
 * You must not remove this notice from this software.
 * **********************************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.Frameworks.CLIrony.Compiler.Lalr;

namespace sones.Lib.Frameworks.CLIrony.Compiler {
  public interface IParser {
    AstNode Parse(CompilerContext context, IEnumerable<Token> tokenStream);
    string GetStateList();

    List<ParserReturn> GetPossibleTokens(CompilerContext MyCompilerContext, List<List<Token>> tokenStream, String InputString);

    AstNode ParseNonDeterministic(CompilerContext _CompilerContext, List<List<Token>> _TokenStream);

    List<String> GetCorrectElements(CompilerContext _CompilerContext, List<List<Token>> _TokenStream);
  }
}
