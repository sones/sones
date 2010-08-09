/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
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
*/


/* <id name="sones GraphDB – CLI" />
 * <copyright file="AbstractCLICommand_PandoraDB.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Achim Friedland</developer>
 * <developer>Henning Rauch</developer>
 */
 
#region Usings

using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

using sones.Lib.CLI;
using sones.Lib.ErrorHandling;
using sones.Lib.Frameworks.CLIrony.Compiler;

using sones.GraphDB.Structures.Result;
using sones.GraphDB;
using sones.GraphDB.Errors;
using sones.GraphDB.Structures;
using sones.GraphDB.GraphQL;

#endregion

namespace sones.GraphDB.Connectors.GraphDBCLI
{

    /// <summary>
    /// The abstract class for all commands of the grammar-based
    /// command line interface.
    /// </summary>
    public abstract class AllGraphDBCLICommands : AllCLICommands
    {


        #region QueryDB(myQueryString, myIPandoraDBSession, myWithOutput = true)

        protected QueryResult QueryDB(String myQueryString, IGraphDBSession myIPandoraDBSession, Boolean myWithOutput = true)
        {

            if (myWithOutput)
                Write(myQueryString + " => ");

            var gqlQuery = new GraphQLQuery(myIPandoraDBSession.DBPluginManager);

            var _QueryResult = gqlQuery.Query(myQueryString, myIPandoraDBSession);

            if (myWithOutput)
                WriteLine(_QueryResult.ResultType.ToString());

            if (_QueryResult == null)
                WriteLine("The QueryResult is invalid!\n\n");

            else if (_QueryResult.ResultType != ResultType.Successful)
                foreach (var aError in _QueryResult.Errors)
                    WriteLine(aError.GetType().ToString() + ": " + aError.ToString() + "\n\n");

            return _QueryResult;

        }

        #endregion

        #region Terminals

        private     StringLiteral   stringLiteralPandoraType        = new StringLiteral("stringLiteralPandoraType",         "'", StringFlags.AllowsDoubledQuote);
        private     StringLiteral   stringLiteralPandoraAttribute   = new StringLiteral("stringLiteralPandoraAttribute",    "'", StringFlags.AllowsDoubledQuote);
        private     StringLiteral   stringLiteralObjectAlias        = new StringLiteral("stringLiteralObjectAlias",         "'", StringFlags.AllowsDoubledQuote);
        private     StringLiteral   stringLiteralCondition          = new StringLiteral("stringLiteralCondition",           "'", StringFlags.AllowsDoubledQuote);

        private     SymbolTerminal  LISTSymbol                      = Symbol("LIST");
        private     SymbolTerminal  LISTPrefixSymbol                = Symbol("<");
        private     SymbolTerminal  LISTPostfixSymbol               = Symbol(">");
        protected   SymbolTerminal  ValuesSymbol                    = Symbol("VALUES");
        protected   SymbolTerminal  NULLSymbol                      = Symbol("NULL");
        protected   SymbolTerminal  NewListSymbol                   = Symbol("NEWLIST");
        protected   SymbolTerminal  UUIDSymbol                      = Symbol("UUID");
        protected   SymbolTerminal  TYPESymbol                      = Symbol("TYPE");
        protected   SymbolTerminal  WHERESymbol                     = Symbol("WHERE");
        protected   SymbolTerminal  INSERT_CommandString            = Symbol("INSERT");
        protected   SymbolTerminal  INSERT_SETREFSymbol             = Symbol("SETREF");

        private     NonTerminal     _PandoraTypeNT                  = new NonTerminal(DBConstants.GraphDBType);
        private     NonTerminal     _PandoraAttributeNT             = new NonTerminal("PandoraAttributeNT");
        private     NonTerminal     _BasicTypeTermNT                = new NonTerminal("BasicTypeTerm");
        private     NonTerminal     _IdNT                           = new NonTerminal("ID");
        private     NonTerminal     _AType                          = new NonTerminal("aType");
        private     NonTerminal     _WhereClauseNT                  = new NonTerminal("WhereClauseNT");
        private     NonTerminal     _BooleanInfixOperation          = new NonTerminal("BooleanInfixOperation");
        private     NonTerminal     _PrefixOperationNT              = new NonTerminal("PrefixOperation");
        private     NonTerminal     _AttrAssignNT                   = new NonTerminal("AttrAssign");
        private     NonTerminal     _ObjectAliasNT                  = new NonTerminal("ObjectAlias");
        private     NonTerminal     _ConditionNT                    = new NonTerminal("Condition");

        #endregion

        #region protected NonTerminals

        protected NonTerminal AttrAssignNT
        {
            get
            {
                _AttrAssignNT.Rule = IdNT + Eq_Equals + BasicTypeTermNT
                                            |   IdNT + INSERT_SETREFSymbol + PandoraTypeNT + UUIDSymbol + Eq_Equals + stringLiteral
                                            |   IdNT + INSERT_SETREFSymbol + PandoraTypeNT + UUIDSymbol + Eq_Equals + NULLSymbol
                                            |   IdNT + Eq_Equals + NewListSymbol; 

                return _AttrAssignNT;

            }
        }

        protected NonTerminal PrefixOperationNT
        {
            get
            {

                NonTerminal IdList = new NonTerminal("IdList");
                IdList.Rule =       IdNT + CommaSymbol + IdList
                                |   IdNT + BracketRoundCloseSymbol;

                NonTerminal ParameterList = new NonTerminal("ParameterList");
                ParameterList.Rule =        stringLiteral + IdNT + CommaSymbol + ParameterList
                                        |   stringLiteral + IdNT + BracketRoundCloseSymbol;

                NonTerminal IdOrParameterListAction = new NonTerminal("IdOrParameterListAction");
                IdOrParameterListAction.Rule =      IdList 
                                                |   ParameterList;

                _PrefixOperationNT.Rule = stringLiteral + BracketRoundOpenSymbol + IdOrParameterListAction;

                return _PrefixOperationNT;

            }
        }

        protected NonTerminal BooleanInfixOperationNT
        {
            get
            {

                SymbolTerminal ANDSymbol                = Symbol("AND");
                SymbolTerminal ORSymbol                 = Symbol("OR");
                SymbolTerminal XORSymbol                = Symbol("XOR");

                _BooleanInfixOperation.Rule = ANDSymbol | ORSymbol | XORSymbol;

                return _BooleanInfixOperation;

            }
        }

        protected NonTerminal WhereClauseNT
        {
            get
            {

                //Todo: implement the real condition
                //simplification

                _WhereClauseNT.Rule = WHERESymbol + ConditionNT;

                return _WhereClauseNT;

            }
        }

        protected NonTerminal ConditionNT
        {
            get
            {
                _ConditionNT.Rule = stringLiteralCondition;
                return _ConditionNT;
            }
        }

        protected NonTerminal AType
        {
            get
            {
                _AType.Rule = PandoraTypeNT + ObjectAliasNT;
                return _AType;
            }
        }

        protected NonTerminal IdNT
        {
            get
            {
                _IdNT.Rule = PandoraTypeNT + DotSymbol + PandoraAttributeNT;
                return _IdNT;
            }
        }

        protected NonTerminal BasicTypeTermNT
        {
            get
            {
                
                SymbolTerminal CONCATSymbol     = Symbol("CONCAT");
                SymbolTerminal MODULOSymbol     = Symbol("MODULO");
                SymbolTerminal TRUESymbol       = Symbol("TRUE");
                SymbolTerminal FALSESymbol      = Symbol("FALSE");

                NonTerminal StringInfixOperation = new NonTerminal("StringInfixOperation");
                StringInfixOperation.Rule = CONCATSymbol;

                NonTerminal NumericInfixOperation = new NonTerminal("NumericInfixOperation");
                NumericInfixOperation.Rule = Symbol("+") | "-" | "*" | "/" | "^" | MODULOSymbol;

                NonTerminal BasicTypeInfixOperation = new NonTerminal("BasicTypeInfixOperation");
                BasicTypeInfixOperation.Rule = BooleanInfixOperationNT | StringInfixOperation | NumericInfixOperation;

                NonTerminal BooleanConstant = new NonTerminal("BooleanConstant");
                BooleanConstant.Rule = TRUESymbol | FALSESymbol;

                NonTerminal ConstantValue = new NonTerminal("ConstantValue");
                ConstantValue.Rule = stringLiteral | numberLiteral | BooleanConstant;

                NonTerminal ExtraOrdinaryValues = new NonTerminal("ExtraOrdinaryValues");
                ExtraOrdinaryValues.Rule =      NULLSymbol
                                            |   NewListSymbol + LISTPrefixSymbol + PandoraTypeNT + LISTPostfixSymbol;

                NonTerminal BasicTypeAtom = new NonTerminal("BasicTypeAtom");
                BasicTypeAtom.Rule =        IdNT
                                        |   ConstantValue
                                        |   ExtraOrdinaryValues;

                _BasicTypeTermNT.Rule =         PrefixOperationNT
                                            |   BracketRoundOpenSymbol + _BasicTypeTermNT + BracketRoundCloseSymbol
                                            |   BasicTypeAtom;
                /*
                _BasicTypeTermNT.Rule = _BasicTypeTermNT + BasicTypeInfixOperation + BasicTypeAtom
                                            | PrefixOperationNT
                                            | BracketRoundOpenSymbol + _BasicTypeTermNT + BracketRoundCloseSymbol
                                            | BasicTypeAtom;
                */
                return _BasicTypeTermNT;

            }
        }

        protected NonTerminal PandoraTypeNT
        {
            get
            {

                stringLiteralPandoraType.PandoraOptions.Add(PandoraOption.IsUsedForAutocompletion);
                _PandoraTypeNT.Rule = stringLiteralPandoraType;
                return _PandoraTypeNT;

            }
        }

        protected NonTerminal ObjectAliasNT
        {
            get
            {
                stringLiteralObjectAlias.PandoraOptions.Add(PandoraOption.IsUsedForAutocompletion);
                _ObjectAliasNT.Rule = stringLiteralObjectAlias;
                return _ObjectAliasNT;

            }
        }

        protected NonTerminal PandoraAttributeNT
        {
            get
            {
                stringLiteralPandoraAttribute.PandoraOptions.Add(PandoraOption.IsUsedForAutocompletion);
                _PandoraAttributeNT.Rule = stringLiteralPandoraAttribute;
                
                return _PandoraAttributeNT;

            }
        }

        #endregion


        // Helper functions

        #region CheckResult(myQueryResult)

        protected void CheckResult(QueryResult myQueryResult)
        {

            switch (myQueryResult.ResultType)
            {

                case ResultType.Successful:
                    Console.WriteLine(myQueryResult.Query + " => Successful!");
                    break;

                case ResultType.PartialSuccessful:
                    Console.WriteLine(myQueryResult.Query + " => PartialSuccessful!");
                    foreach (var aError in myQueryResult.Errors)
                        Console.WriteLine(aError.GetType().Name + ": " + aError.ToString() + "\n\n");
                    break;

                case ResultType.Failed:
                    Console.WriteLine(myQueryResult.Query + " => Failed!");
                    foreach (var aError in myQueryResult.Errors)
                        Console.WriteLine(aError.GetType().Name + ": " + aError.ToString() + "\n\n");
                    break;

            }

        }

        #endregion

        #region CheckResult(myQueryResult)

        protected void CheckResult(String myQueryResult)
        {
            Console.WriteLine(myQueryResult);
        }

        #endregion

        #region HandleQueryResult(queryResult, printResult)

        protected void HandleQueryResult(QueryResult queryResult, Boolean printResult)
        {

            #region Data

            Boolean hasError = false;

            #endregion

            #region print ErrorCode

            foreach (GraphDBError aError in queryResult.Errors)
            {
                        WriteLine("ERROR!");
                        WriteLine("Errorclass: " + aError.GetType().Name);
                        WriteLine(aError.ToString());
                        WriteLine(Environment.NewLine);
                        hasError = true;
            }

            

            #endregion

            #region print result

            if (!hasError)
            {
                if (printResult)
                {
                    int nrOfSelList = queryResult.Results.Count();

                    foreach (SelectionResultSet aSelElement in queryResult.Results)
                    {
                        foreach (DBObjectReadout dataObject in aSelElement.Objects)
                        {
                            printDBReadout(dataObject, 0);
                            if (CLI_Output == CLI_Output.Standard)
                                WriteLine(Environment.NewLine);
                        }
                        if (CLI_Output == CLI_Output.Standard)
                            WriteLine(Environment.NewLine);
                    }
                }
            }

            #endregion

        }

        #endregion

        #region printDBReadout(dataObject, level)

        private void printDBReadout(DBObjectReadout dataObject, UInt16 level)
        {
            foreach (KeyValuePair<String, Object> attribute in dataObject.Attributes)
            {
                if (attribute.Value != null)
                {
                    if (attribute.Value is DBObjectReadout)
                    {
                        Write(attribute.Key.ToString() + " <resolved>:" + Environment.NewLine);
                        printDBReadout((DBObjectReadout)attribute.Value, (UInt16)(level + 1));
                    }
                    else
                    {
                        if (attribute.Value is IEnumerable<DBObjectReadout>)
                        {
                            for (UInt16 i = 0; i < level; i++)
                            {
                                Write("\t");
                            }
                            Write(attribute.Key.ToString() + " <resolved>:" + Environment.NewLine);

                            foreach (DBObjectReadout adbo in (IEnumerable<DBObjectReadout>)attribute.Value)
                            {
                                printDBReadout(adbo, (UInt16)(level + 1));
                                Write(Environment.NewLine);
                                Write(Environment.NewLine);
                            }
                            Write("\n");
                        }
                        else
                        {
                            for (UInt16 i = 0; i < level; i++)
                            {
                                Write("\t");
                            }

                            Write(attribute.Key.ToString() + " = " + attribute.Value.ToString() + Environment.NewLine);
                        }
                    }                    
                }
                else
                {
                    for (UInt16 i = 0; i < level; i++)
                    {
                        Write("\t");
                    }
                    Write(attribute.Key.ToString() + " = not resolved\n");
                }
            }
        }

        #endregion

    }

}
