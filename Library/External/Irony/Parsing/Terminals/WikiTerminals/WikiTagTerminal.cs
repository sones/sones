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

namespace Irony.Parsing {

  //Handles formatting tags like *bold*, _italic_; also handles headings and lists
  public class WikiTagTerminal : WikiTerminalBase {

    public WikiTagTerminal(string name,  WikiTermType termType, string tag, string htmlElementName)
      : this (name, termType, tag, string.Empty, htmlElementName) { }

    public WikiTagTerminal(string name,  WikiTermType termType, string openTag, string closeTag, string htmlElementName)
      : base (name, termType, openTag, closeTag, htmlElementName) { }

    public override Token TryMatch(ParsingContext context, ISourceStream source) {
      bool isHeadingOrList = TermType == WikiTermType.Heading || TermType == WikiTermType.List;
      if(isHeadingOrList) {
          bool isAfterNewLine = (context.PreviousToken == null || context.PreviousToken.Terminal == Grammar.NewLine);
          if(!isAfterNewLine)  return null;
      }
      if(!source.MatchSymbol(OpenTag, true)) return null;
      source.PreviewPosition += OpenTag.Length;
      //For headings and lists require space after
      if(TermType == WikiTermType.Heading || TermType == WikiTermType.List) {
        const string whitespaces = " \t\r\n\v";
        if (!whitespaces.Contains(source.PreviewChar)) return null; 
      }
      var token = source.CreateToken(this.OutputTerminal);
      return token; 
    }
 
  }//class

}//namespace
