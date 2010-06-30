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
using System.Text;

namespace sones.Lib.Frameworks.CLIrony.Compiler {

  public class GrammarErrorException : Exception {
    public GrammarErrorException(String message) : base(message) { }
    public GrammarErrorException(String message, Exception inner) : base(message, inner) { }

  }//class

  public class CompilerException : Exception {
    public CompilerException(String message) : base(message) { }
    public CompilerException(String message, Exception inner) : base(message, inner) { }

  }//class

}//namespace
