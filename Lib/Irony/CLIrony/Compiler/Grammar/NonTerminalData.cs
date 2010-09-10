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

namespace sones.Lib.Frameworks.CLIrony.Compiler {

  [Flags]
  public enum ProductionFlags {
    None = 0,
    IsInitial = 0x01,    //is initial production
    HasTerminals = 0x02, //contains terminal
    IsError = 0x04,      //contains Error terminal
    IsEmpty = 0x08,
  }

  public class LR0ItemList : List<LR0Item> { }
  public class ProductionList : List<Production> { }

  public class Production {
    public ProductionFlags Flags;
    public readonly NonTerminal LValue;                              // left-side element
    public readonly BnfTermList RValues = new BnfTermList();         //the right-side elements sequence
    public readonly GrammarHintList Hints = new GrammarHintList();
    public readonly LR0ItemList LR0Items = new LR0ItemList();        //LR0 items based on this production 
    public Production(NonTerminal lvalue) {
      LValue = lvalue;
    }//constructor

    public bool IsSet(ProductionFlags flag) {
      return (Flags & flag) != ProductionFlags.None;
    }

    public override string ToString() {
      return TextUtils.ProductionToString(this, -1); //no dot
    }

  }//Production class

  public partial class LR0Item {
    public readonly Production Production;
    public readonly int Position;

    public readonly StringSet TailFirsts = new StringSet(); //tail is a set of elements after the Current element
    public bool TailIsNullable = false;

    //automatically generated IDs - used for building keys for lists of kernel LR0Items
    // which in turn are used to quickly lookup parser states in hash
    internal readonly int ID;

    public LR0Item(Production production, int position, int id) {
      Production = production;
      Position = position;
      ID = id;
    }
    //The after-dot element
    public ABnfTerm Current {
      get {
        if (Position < Production.RValues.Count)
          return Production.RValues[Position];
        else
          return null;
      }
    }
    public bool IsKernel {
      get { return Position > 0 || (Production.IsSet(ProductionFlags.IsInitial) && Position == 0); }
    }
    public override string ToString() {
      return TextUtils.ProductionToString(this.Production, Position);
    }
  }//LR0Item


}//namespace
