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

  public enum WikiBlockType {
    EscapedText,
    CodeBlock,
    Anchor,
    LinkToAnchor,
    Url,
    FileLink, //looks like it is the same as Url
    Image,
  }

  public class WikiBlockTerminal : WikiTerminalBase {
    public readonly WikiBlockType BlockType;

    public WikiBlockTerminal(string name, WikiBlockType blockType, string openTag, string closeTag, string htmlElementName)
          : base(name, WikiTermType.Block, openTag, closeTag, htmlElementName) { 
      BlockType = blockType;
    }

    public override Token TryMatch(ParsingContext context, ISourceStream source) {
      if (!source.MatchSymbol(OpenTag, true)) return null;
      source.PreviewPosition += OpenTag.Length;
      var endPos = source.Text.IndexOf(CloseTag, source.PreviewPosition);
      string content; 
      if(endPos > 0) {
        content = source.Text.Substring(source.PreviewPosition, endPos - source.PreviewPosition); 
        source.PreviewPosition = endPos + CloseTag.Length;
      } else {
        content = source.Text.Substring(source.PreviewPosition, source.Text.Length - source.PreviewPosition); 
        source.PreviewPosition = source.Text.Length;
      }
      var token = source.CreateToken(this.OutputTerminal, content); 
      return token;      
    }

  
  }//class
}//namespace
