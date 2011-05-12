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
  public enum WikiTermType {
    Text,
    Element,
    Format,
    Heading,
    List,
    Block,
    Table
  }

  public abstract class WikiTerminalBase : Terminal {
    public readonly WikiTermType TermType;
    public readonly string OpenTag, CloseTag;
    public string HtmlElementName, ContainerHtmlElementName;
    public string OpenHtmlTag, CloseHtmlTag;
    public string ContainerOpenHtmlTag, ContainerCloseHtmlTag;

    public WikiTerminalBase(string name, WikiTermType termType, string openTag, string closeTag, string htmlElementName) : base(name) {
      TermType = termType;
      OpenTag = openTag;
      CloseTag = closeTag;
      HtmlElementName = htmlElementName; 
      this.Priority = OpenTag.Length; //longer tags have higher priority
    }

    public override IList<string> GetFirsts() {
      return new string[] {OpenTag};
    }
    public override void Init(GrammarData grammarData) {
      base.Init(grammarData);
      if (!string.IsNullOrEmpty(HtmlElementName)) {
        if (string.IsNullOrEmpty(OpenHtmlTag)) OpenHtmlTag = "<" + HtmlElementName + ">";
        if (string.IsNullOrEmpty(CloseHtmlTag)) CloseHtmlTag = "</" + HtmlElementName + ">";
      }
      if (!string.IsNullOrEmpty(ContainerHtmlElementName)) {
        if (string.IsNullOrEmpty(ContainerOpenHtmlTag)) ContainerOpenHtmlTag = "<" + ContainerHtmlElementName + ">";
        if (string.IsNullOrEmpty(ContainerCloseHtmlTag)) ContainerCloseHtmlTag = "</" + ContainerHtmlElementName + ">";
      }

    }

  }//class



}//namespace
