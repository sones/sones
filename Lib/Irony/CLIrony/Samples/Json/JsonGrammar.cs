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
using sones.Lib.Frameworks.CLIrony.Compiler;

namespace sones.Lib.Frameworks.CLIrony.Samples.Json {
  [Language("JSON", "1.0", "JSON data format")]
  public class JsonGrammar : Grammar {
    public JsonGrammar() {
      //Terminals
      var jstring = new StringLiteral("string", "\"");
      var jnumber = new NumberLiteral("number");
      var comma = Symbol(","); 
      
      //Nonterminals
      var jobject = new NonTerminal("Object");
      var jarray = new NonTerminal("Array");
      var jvalue = new NonTerminal("Value");
      var jprop = new NonTerminal("Property"); 
      var jproplist = new NonTerminal("PropertyList"); 
      var jlist = new NonTerminal("List"); 

      //Rules
      jvalue.Rule = jstring | jnumber | jobject | jarray | "true" | "false" | "null";
      jobject.Rule = "{" + jproplist + "}";
      jproplist.Rule = MakeStarRule(jproplist, comma, jprop);
      jprop.Rule = jstring + ":" + jvalue;
      jarray.Rule = "[" + jlist + "]";
      jlist.Rule = MakeStarRule(jlist, comma, jvalue);

      //Set grammar root
      this.Root = jvalue;
      RegisterPunctuation("{", "}", "[", "]", ":", ",");
      this.MarkTransient(jvalue, jlist, jproplist); 
      this.LanguageFlags = LanguageFlags.None; //.BubbleNodes;

    }//constructor
  }//class
}//namespace
