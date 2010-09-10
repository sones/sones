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
using System.IO; 
using System.Xml;

namespace sones.Lib.Frameworks.Irony.Parsing { 
  public static class XmlExtensions {

    public static string ToXml(this ParseTree parseTree) {
      if (parseTree == null || parseTree.Root == null) return string.Empty;
      var xdoc = ToXmlDocument(parseTree); 
      StringWriter sw = new StringWriter();
      XmlTextWriter xw = new XmlTextWriter(sw);
      xw.Formatting = Formatting.Indented;
      xdoc.WriteTo(xw);
      xw.Flush();
      return sw.ToString();
    }
    
    public static XmlDocument ToXmlDocument(this ParseTree parseTree) {
      var xdoc = new XmlDocument();
      if (parseTree == null || parseTree.Root == null) return xdoc;
      var xTree = xdoc.CreateElement("ParseTree");
      xdoc.AppendChild(xTree); 
      var xRoot = parseTree.Root.ToXmlElement(xdoc);
      xTree.AppendChild(xRoot);
      return xdoc; 
    }

    public static XmlElement ToXmlElement(this ParseTreeNode node, XmlDocument ownerDocument) {
      var xElem = ownerDocument.CreateElement("Node");
      xElem.SetAttribute("Term", node.Term.Name);
      if (node.Term.AstNodeType != null) 
        xElem.SetAttribute("AstNodeType", node.Term.AstNodeType.Name);
      if (node.Token != null) {
        xElem.SetAttribute("Terminal", node.Term.GetType().Name);
        //xElem.SetAttribute("Text", node.Token.Text);
        if (node.Token.Value != null)
          xElem.SetAttribute("Value", node.Token.Value.ToString()); 
      } else 
        foreach (var child in node.ChildNodes) {
          var xChild = child.ToXmlElement(ownerDocument);
          xElem.AppendChild(xChild); 
        }
      return xElem;
    }//method

  }//class
}//namespace
