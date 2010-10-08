/*
 * DBCLI_UPDATE
 * (c) Henning Rauch, 2009 - 2010
 */

#region Usings

using System;
using System.Collections.Generic;

using sones.GraphFS.Session;
using sones.GraphDS.Connectors.CLI;

using sones.Lib.Frameworks.CLIrony.Compiler;
using sones.Lib.ErrorHandling;
using sones.GraphFS.Errors;
using sones.GraphDS.API.CSharp;

#endregion

namespace sones.GraphDB.Connectors.GraphDBCLI
{

    /// <summary>
    /// Updates myAttributes of certain objects.
    /// </summary>

    public class DBCLI_UPDATE : AllBasicDBCLICommands
    {
        
        #region Constructor

        public DBCLI_UPDATE()
        {

            // Command name and description
            InitCommand("UPDATE",
                        "Updates myAttributes of certain objects",
                        "Updates myAttributes of certain objects");

            #region Symbol declaration

            SymbolTerminal ADDREFSymbol = Symbol("ADDREF");
            SymbolTerminal REMREFSymbol = Symbol("REMREF");

            #endregion

            #region Non-terminal declaration

            NonTerminal AttrUpdateList = new NonTerminal("AttrUpdateList");
            NonTerminal AttrUpdate = new NonTerminal("AttrUpdate");
            NonTerminal WhereOrValuesAction = new NonTerminal("WhereOrValuesAction");

            #endregion

            // BNF rule
            CreateBNFRule(CLICommandSymbolTerminal + TYPESymbol + AType + WhereOrValuesAction);

            WhereOrValuesAction.Rule = WhereClauseNT + ValuesSymbol + BracketRoundOpenSymbol + AttrUpdateList
                                        | ValuesSymbol + BracketRoundOpenSymbol + AttrUpdateList;

            AttrUpdateList.Rule = AttrUpdate + CommaSymbol + AttrUpdateList
                                        | AttrUpdate + BracketRoundCloseSymbol;

            AttrUpdate.Rule = AttrAssignNT
                                | IdNT + ADDREFSymbol + GraphTypeNT + UUIDSymbol + Eq_Equals + stringLiteral
                                | IdNT + REMREFSymbol + GraphTypeNT + UUIDSymbol + Eq_Equals + stringLiteral;


            #region Non-terminal integration

            _CommandNonTerminals.Add(AttrUpdateList);
            _CommandNonTerminals.Add(AttrUpdate);
            _CommandNonTerminals.Add(WhereOrValuesAction);

            #endregion


        }

        #endregion

        #region Execute Command

        public override Exceptional Execute(AGraphDSSharp myAGraphDSSharp, ref String myCurrentPath, Dictionary<String, List<AbstractCLIOption>> myOptions, String myInputString)
        {

            if (myAGraphDSSharp == null)
                return new Exceptional(new GraphDSError("myAGraphDSSharp must not be null!"));

            _CancelCommand = false;

            var QueryInputString = myInputString.Replace("'", "");

            HandleQueryResult(QueryDB(myAGraphDSSharp, QueryInputString), true);

            return Exceptional.OK;

        }

        #endregion

    }

}
