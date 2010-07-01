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

/* 
 * Copyright (c) sones GmbH 2007-2010
 * <developer>Henning Rauch</developer>
 * <developer>Stefan Licht</developer>
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;

using sones.GraphDB.Exceptions;
using sones.GraphDB.Indices;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.QueryLanguage.Enums;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Statements;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Statements.Drop;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Statements.Dump;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Statements.InsertOrReplace;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Statements.Replace;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Statements.Select;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Statements.Setting;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Statements.Transaction;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.PandoraTypes;
using sones.Lib;
using sones.Lib.ErrorHandling;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.Lib.Frameworks.Irony.Scripting.Ast;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Statements.Import;

#endregion

namespace sones.GraphDB.QueryLanguage
{

    /// <summary>
    /// This class defines the GraphQueryLanguage.
    /// </summary>
    public class GraphQL : Grammar, IDumpable
    {

        #region Consts

        public const String DOT = ".";
        public const String TERMINAL_BRACKET_LEFT = "(";
        public const String TERMINAL_BRACKET_RIGHT = ")";
        public const String TERMINAL_QUEUESIZE  = "QUEUESIZE";
        public const String TERMINAL_WEIGHTED   = "WEIGHTED";
        public const String TERMINAL_UNIQUE     = "UNIQUE";
        public const String TERMINAL_MANDATORY  = "MANDATORY";
        public const String TERMINAL_SORTED     = "SORTED";        
        public const String TERMINAL_ASC        = "ASC";
        public const String TERMINAL_DESC       = "DESC";
        public const String TERMINAL_TRUE       = "TRUE";
        public const String TERMINAL_FALSE      = "FALSE";
        public const String TERMINAL_LIST       = "LIST";
        public const String TERMINAL_SET        = "SET";
        public const String TERMINAL_LT         = "<";
        public const String TERMINAL_GT         = ">";
        
        #endregion

        #region Properties

        public SymbolTerminal S_CREATE  { get; private set; }
        public SymbolTerminal S_comma   { get; private set; }
        public SymbolTerminal S_dot     { get; private set; }
        public SymbolTerminal S_colon   { get; private set; }

        #region Brackets

        public SymbolTerminal S_BRACKET_LEFT        { get; private set; }
        public SymbolTerminal S_BRACKET_RIGHT       { get; private set; }
        public SymbolTerminal S_TUPLE_BRACKET_LEFT  { get; private set; }
        public SymbolTerminal S_TUPLE_BRACKET_RIGHT { get; private set; }
        public SymbolTerminal S_TUPLE_BRACKET_LEFT_EXCLUSIVE
        {
            get { return S_BRACKET_LEFT; }
        }
        public SymbolTerminal S_TUPLE_BRACKET_RIGHT_EXCLUSIVE
        {
            get { return S_BRACKET_RIGHT; }
        }

        #endregion

        public SymbolTerminal S_edgeInformationDelimiterSymbol { get; private set; }
        public SymbolTerminal S_edgeTraversalDelimiter { get; private set; }
        public SymbolTerminal S_NULL            { get; private set; }
        public SymbolTerminal S_NOT             { get; private set; }
        public SymbolTerminal S_UNIQUE          { get; private set; }
        public SymbolTerminal S_WITH            { get; private set; }
        public SymbolTerminal S_TABLE           { get; private set; }
        public SymbolTerminal S_ALTER           { get; private set; }
        public SymbolTerminal S_ADD             { get; private set; }
        public SymbolTerminal S_TO              { get; private set; }
        public SymbolTerminal S_COLUMN          { get; private set; }
        public SymbolTerminal S_DROP            { get; private set; }
        public SymbolTerminal S_RENAME          { get; private set; }
        public SymbolTerminal S_CONSTRAINT      { get; private set; }
        public SymbolTerminal S_INDEX           { get; private set; }
        public SymbolTerminal S_INDICES         { get; private set; }
        public SymbolTerminal S_ON              { get; private set; }
        public SymbolTerminal S_KEY             { get; private set; }
        public SymbolTerminal S_PRIMARY         { get; private set; }
        public SymbolTerminal S_INSERT          { get; private set; }
        public SymbolTerminal S_INTO            { get; private set; }
        public SymbolTerminal S_UPDATE          { get; private set; }
        public SymbolTerminal S_INSERTORUPDATE  { get; private set; }
        public SymbolTerminal S_INSERTORREPLACE { get; private set; }
        public SymbolTerminal S_INSERTIFNOTEXIST { get; private set; }
        public SymbolTerminal S_REPLACE         { get; private set; }
        public SymbolTerminal S_SET             { get; private set; }
        public SymbolTerminal S_REMOVE          { get; private set; }
        public SymbolTerminal S_VALUES          { get; private set; }
        public SymbolTerminal S_DELETE          { get; private set; }
        public SymbolTerminal S_SELECT          { get; private set; }
        public SymbolTerminal S_FROM            { get; private set; }
        public SymbolTerminal S_AS              { get; private set; }
        public SymbolTerminal S_COUNT           { get; private set; }
        public SymbolTerminal S_JOIN            { get; private set; }
        public SymbolTerminal S_BY              { get; private set; }
        public SymbolTerminal S_WHERE           { get; private set; }
        public SymbolTerminal S_TYPE            { get; private set; }
        public SymbolTerminal S_TYPES           { get; private set; }
        public SymbolTerminal S_EDITION         { get; private set; }
        public SymbolTerminal S_INDEXTYPE       { get; private set; }
        public SymbolTerminal S_LIST            { get; private set; }
        public SymbolTerminal S_ListTypePrefix  { get; private set; }
        public SymbolTerminal S_ListTypePostfix { get; private set; }
        public SymbolTerminal S_EXTENDS         { get; private set; }
        public SymbolTerminal S_ATTRIBUTES      { get; private set; }
        public SymbolTerminal S_MATCHES         { get; private set; }
        public SymbolTerminal S_LIMIT           { get; private set; }
        public SymbolTerminal S_DEPTH           { get; private set; }

        #region REFERENCE former SETREF

        public SymbolTerminal S_REFERENCE { get; private set; }
        public SymbolTerminal S_REF { get; private set; }

        #endregion

        #region REFERENCEUUID

        public SymbolTerminal S_REFUUID { get; private set; }
        public SymbolTerminal S_REFERENCEUUID { get; private set; }

        #endregion

        #region LISTOF/SETOF/SETOFUUIDS

        public SymbolTerminal S_LISTOF          { get; private set; }
        public SymbolTerminal S_SETOF           { get; private set; }
        public SymbolTerminal S_SETOFUUIDS      { get; private set; }
       
        #endregion

        public SymbolTerminal S_UUID            { get; private set; }
        public SymbolTerminal S_OFFSET          { get; private set; }
        public SymbolTerminal S_TRUNCATE        { get; private set; }
        public SymbolTerminal S_TRUE            { get; private set; }
        public SymbolTerminal S_FALSE           { get; private set; }
        public SymbolTerminal S_SORTED          { get; private set; }
        public SymbolTerminal S_ASC             { get; private set; }

        public SymbolTerminal S_DESC            { get; private set; }
        public SymbolTerminal S_QUEUESIZE       { get; private set; }
        public SymbolTerminal S_WEIGHTED        { get; private set; }
        public SymbolTerminal S_SETTING         { get; private set; }
        public SymbolTerminal S_GET             { get; private set; }
        public SymbolTerminal S_DB              { get; private set; }
        public SymbolTerminal S_SESSION         { get; private set; }
        public SymbolTerminal S_ATTRIBUTE       { get; private set; }
        public SymbolTerminal S_DEFAULT         { get; private set; }

        public SymbolTerminal S_BACKWARDEDGES   { get; private set; }
        public SymbolTerminal S_BACKWARDEDGE    { get; private set; }
        public SymbolTerminal S_DESCRIBE        { get; private set; }
        public SymbolTerminal S_DESCFUNC        { get; private set; }
        public SymbolTerminal S_DESCAGGR        { get; private set; }
        public SymbolTerminal S_DESCAGGRS       { get; private set; }
        public SymbolTerminal S_DESCSETT        { get; private set; }
        public SymbolTerminal S_DESCSETTINGS    { get; private set; }
        public SymbolTerminal S_DESCTYPE        { get; private set; }
        public SymbolTerminal S_DESCTYPES       { get; private set; }
        public SymbolTerminal S_DESCFUNCTIONS   { get; private set; }
        public SymbolTerminal S_DESCIDX         { get; private set; }
        public SymbolTerminal S_DESCIDXS        { get; private set; }
        public SymbolTerminal S_DESCEDGE        { get; private set; }
        public SymbolTerminal S_DESCEDGES       { get; private set; }
        public SymbolTerminal S_MANDATORY       { get; private set; }
        public SymbolTerminal S_ABSTRACT        { get; private set; }

        #region Transactions

        public SymbolTerminal S_TRANSACTBEGIN           { get; private set; }
        public SymbolTerminal S_TRANSACT                { get; private set; }
        public SymbolTerminal S_TRANSACTDISTRIBUTED     { get; private set; }
        public SymbolTerminal S_TRANSACTLONGRUNNING     { get; private set; }
        public SymbolTerminal S_TRANSACTISOLATION       { get; private set; }
        public SymbolTerminal S_TRANSACTNAME            { get; private set; }
        public SymbolTerminal S_TRANSACTTIMESTAMP       { get; private set; }
        public SymbolTerminal S_TRANSACTCOMMIT          { get; private set; }
        public SymbolTerminal S_TRANSACTROLLBACK        { get; private set; }
        public SymbolTerminal S_TRANSACTCOMROLLASYNC    { get; private set; }

        #endregion

        public SymbolTerminal S_REMOVEFROMLIST  { get; private set; }
        public SymbolTerminal S_ADDTOLIST       { get; private set; }
        public SymbolTerminal S_COMMENT         { get; private set; }
        public SymbolTerminal S_REBUILD         { get; private set; }

        #region IMPORT

        public SymbolTerminal S_IMPORT          { get; private set; }
        public SymbolTerminal S_COMMENTS        { get; private set; }
        public SymbolTerminal S_PARALLELTASKS   { get; private set; }
        public SymbolTerminal S_VERBOSITY       { get; private set; }
        public SymbolTerminal S_FORMAT          { get; private set; }

        #endregion

        #region DUMP

        public SymbolTerminal S_DUMP            { get; private set; }
        public SymbolTerminal S_DUMP_TYPE_ALL   { get; private set; }
        public SymbolTerminal S_DUMP_TYPE_GDDL  { get; private set; }
        public SymbolTerminal S_DUMP_TYPE_GDML  { get; private set; }
        public SymbolTerminal S_DUMP_FORMAT_GQL { get; private set; }
        public SymbolTerminal S_DUMP_FORMAT_CSV { get; private set; }

        #endregion

        #endregion

        #region Grammarhooks - replace by interface

        public NonTerminal BNF_ImportFormat { get; set; }
        
        #endregion

        #region Constructor and definitions

        #region GraphQL() and definitions

        public GraphQL(DBContext doNotUseMe)
            : this()
        { }

        public GraphQL()
            : base(false)
        {

            #region SetLanguageFlags

            this.SetLanguageFlags(LanguageFlags.CreateAst);
            this.SetLanguageFlags(LanguageFlags.AutoDetectTransient, false);

            //Todo: think about this:
            //this.SetLanguageFlags(LanguageFlags.TailRecursive);

            #endregion

            #region Terminals

            #region Comments

            //Terminals
            var comment             = new CommentTerminal("comment", "/*", "*/");
            var lineComment = new CommentTerminal("line_comment", "--", "\n", "\r\n");
            //TODO: remove block comment, added for testing LUA-style comments
            var blockComment        = new CommentTerminal("block_comment", "--[[", "]]");
            NonGrammarTerminals.Add(comment);
            NonGrammarTerminals.Add(lineComment);
            NonGrammarTerminals.Add(blockComment);

            #endregion

            #region Available value defs: Number, String, Name

            var number              = new NumberLiteral("number", NumberFlags.AllowSign | NumberFlags.DisableQuickParse);
            number.DefaultIntTypes  = new TypeCode[] { TypeCode.UInt64, TypeCode.Int64, NumberLiteral.TypeCodeBigInt };
            var string_literal      = new StringLiteral("string", "'", StringFlags.AllowsDoubledQuote | StringFlags.AllowsLineBreak);
            var name                = new IdentifierTerminal("name", "ÄÖÜäöüß0123456789_", "ÄÖÜäöü0123456789$_");


            #endregion

            //var name_ext            = TerminalFactory.CreateSqlExtIdentifier("name_ext"); //removed, because we do not want to hav types or sth else with whitespaces, otherwise it conflicts with tupleSet

            #region Symbols

            S_CREATE                          = Symbol("CREATE");
            S_comma                           = Symbol(",");
            S_dot                             = Symbol(".");
            S_colon                           = Symbol(":");
            S_BRACKET_LEFT                    = Symbol(TERMINAL_BRACKET_LEFT);
            S_BRACKET_RIGHT                   = Symbol(TERMINAL_BRACKET_RIGHT);
            S_TUPLE_BRACKET_LEFT              = Symbol("[");
            S_TUPLE_BRACKET_RIGHT             = Symbol("]");
            S_edgeInformationDelimiterSymbol  = Symbol(DBConstants.EdgeInformationDelimiterSymbol);
            S_edgeTraversalDelimiter          = Symbol(DBConstants.EdgeTraversalDelimiterSymbol);
            S_NULL                            = Symbol("NULL");
            S_NOT                             = Symbol("NOT");
            S_UNIQUE                          = Symbol("UNIQUE");
            S_WITH                            = Symbol("WITH");
            S_TABLE                           = Symbol("TABLE");
            S_ALTER                           = Symbol("ALTER");
            S_ADD                             = Symbol("ADD");
            S_TO                              = Symbol("TO");
            S_COLUMN                          = Symbol("COLUMN");
            S_DROP                            = Symbol("DROP");
            S_RENAME                          = Symbol("RENAME");
            S_CONSTRAINT                      = Symbol("CONSTRAINT");
            S_INDEX                           = Symbol("INDEX");
            S_INDICES                         = Symbol("INDICES");
            S_ON                              = Symbol("ON");
            S_KEY                             = Symbol("KEY");
            S_PRIMARY                         = Symbol("PRIMARY");
            S_INSERT                          = Symbol("INSERT");
            S_INTO                            = Symbol("INTO");
            S_UPDATE                          = Symbol("UPDATE");
            S_INSERTORUPDATE                  = Symbol("INSERTORUPDATE");
            S_INSERTORREPLACE                 = Symbol("INSERTORREPLACE");
            S_INSERTIFNOTEXIST                = Symbol("INSERTIFNOTEXIST");
            S_REPLACE                         = Symbol("REPLACE");
            S_SET                             = Symbol(TERMINAL_SET);
            S_REMOVE                          = Symbol("REMOVE");
            S_VALUES                          = Symbol("VALUES");
            S_DELETE                          = Symbol("DELETE");
            S_SELECT                          = Symbol("SELECT");
            S_FROM                            = Symbol("FROM");
            S_AS                              = Symbol("AS");
            S_COUNT                           = Symbol("COUNT");
            S_JOIN                            = Symbol("JOIN");
            S_BY                              = Symbol("BY");
            S_WHERE                           = Symbol("WHERE");
            S_TYPE                            = Symbol("TYPE");
            S_TYPES                           = Symbol("TYPES");
            S_EDITION                         = Symbol("EDITION");
            S_INDEXTYPE                       = Symbol("INDEXTYPE");
            S_LIST                            = Symbol(TERMINAL_LIST);
            S_ListTypePrefix                  = Symbol(TERMINAL_LT);
            S_ListTypePostfix                 = Symbol(TERMINAL_GT);
            S_EXTENDS                         = Symbol("EXTENDS");
            S_ATTRIBUTES                      = Symbol("ATTRIBUTES");
            S_MATCHES                         = Symbol("MATCHES");
            S_LIMIT                           = Symbol("LIMIT");
            S_DEPTH                           = Symbol("DEPTH");
            S_REFERENCE                       = Symbol("REFERENCE");
            S_REF                             = Symbol("REF");
            S_REFUUID                         = Symbol("REFUUID");
            S_REFERENCEUUID                   = Symbol("REFERENCEUUID");
            S_LISTOF                          = Symbol(DBConstants.LISTOF);
            S_SETOF                           = Symbol(DBConstants.SETOF);
            S_SETOFUUIDS                      = Symbol(DBConstants.SETOFUUIDS);
            S_UUID                            = Symbol("UUID");
            S_OFFSET                          = Symbol("OFFSET");
            S_TRUNCATE                        = Symbol("TRUNCATE");
            S_TRUE                            = Symbol(TERMINAL_TRUE);
            S_FALSE                           = Symbol(TERMINAL_FALSE);
            S_SORTED                          = Symbol(TERMINAL_SORTED);
            S_ASC                             = Symbol(TERMINAL_ASC);
            S_DESC                            = Symbol(TERMINAL_DESC);
            S_QUEUESIZE                       = Symbol(TERMINAL_QUEUESIZE);
            S_WEIGHTED                        = Symbol(TERMINAL_WEIGHTED);
            S_SETTING                         = Symbol("SETTING");
            S_GET                             = Symbol("GET");
            S_DB                              = Symbol("DB");
            S_SESSION                         = Symbol("SESSION");
            S_ATTRIBUTE                       = Symbol("ATTRIBUTE");
            S_DEFAULT                         = Symbol("DEFAULT");
            S_BACKWARDEDGES                   = Symbol("BACKWARDEDGES");
            S_BACKWARDEDGE                    = Symbol("BACKWARDEDGE");
            S_DESCRIBE                        = Symbol("DESCRIBE");
            S_DESCFUNC                        = Symbol("FUNCTION");
            S_DESCAGGR                        = Symbol("AGGREGATE");
            S_DESCAGGRS                       = Symbol("AGGREGATES");
            S_DESCSETT                        = Symbol("SETTING");
            S_DESCSETTINGS                    = Symbol("SETTINGS");
            S_DESCTYPE                        = Symbol("TYPE");
            S_DESCTYPES                       = Symbol("TYPES");
            S_DESCFUNCTIONS                   = Symbol("FUNCTIONS");
            S_DESCIDX                         = Symbol("INDEX");
            S_DESCIDXS                        = Symbol("INDICES");
            S_DESCEDGE                        = Symbol("EDGE");
            S_DESCEDGES                       = Symbol("EDGES");
            S_MANDATORY                       = Symbol("MANDATORY");
            S_ABSTRACT                        = Symbol("ABSTRACT");
            S_TRANSACTBEGIN                   = Symbol("BEGIN");
            S_TRANSACT                        = Symbol("TRANSACTION");
            S_TRANSACTDISTRIBUTED             = Symbol(DBConstants.TRANSACTION_DISTRIBUTED);
            S_TRANSACTLONGRUNNING             = Symbol(DBConstants.TRANSACTION_LONGRUNNING);
            S_TRANSACTISOLATION               = Symbol(DBConstants.TRANSACTION_ISOLATION);
            S_TRANSACTNAME                    = Symbol(DBConstants.TRANSACTION_NAME);
            S_TRANSACTTIMESTAMP               = Symbol(DBConstants.TRANSACTION_TIMESTAMP);
            S_TRANSACTROLLBACK                = Symbol(DBConstants.TRANSACTION_ROLLBACK);
            S_TRANSACTCOMMIT                  = Symbol(DBConstants.TRANSACTION_COMMIT);
            S_TRANSACTCOMROLLASYNC            = Symbol(DBConstants.TRANSACTION_COMROLLASYNC);
            S_ADDTOLIST                       = Symbol("+=");
            S_REMOVEFROMLIST                  = Symbol("-=");
            S_DUMP                            = Symbol("DUMP");
            S_DUMP_TYPE_ALL                   = Symbol("ALL");
            S_DUMP_TYPE_GDDL                  = Symbol("GDDL");
            S_DUMP_TYPE_GDML                  = Symbol("GDML");
            S_DUMP_FORMAT_GQL                 = Symbol("GQL");
            S_DUMP_FORMAT_CSV                 = Symbol("CSV");
            S_COMMENT                         = Symbol("COMMENT");
            S_REBUILD                         = Symbol("REBUILD");

            #region IMPORT

            S_IMPORT                          = Symbol("IMPORT");
            S_COMMENTS                        = Symbol("COMMENTS");
            S_PARALLELTASKS                   = Symbol("PARALLELTASKS");
            S_VERBOSITY                       = Symbol("VERBOSITY");
            S_FORMAT                          = Symbol("FORMAT");

            #endregion

            #endregion

            #endregion

            #region Non-Terminals

            #region ID related

            var Id                              = new NonTerminal("Id", CreateIDNode);
            var Id_simple                       = new NonTerminal("id_simple", typeof(AstNode));
            var id_typeAndAttribute             = new NonTerminal("id_typeAndAttribute");
            var idlist                          = new NonTerminal("idlist");
            var id_simpleList                   = new NonTerminal("id_simpleList");
            var id_simpleDotList                = new NonTerminal("id_simpleDotList");
            var IdOrFunc                        = new NonTerminal("IdOrFunc");
            var IdOrFuncList                    = new NonTerminal("IdOrFuncList", CreateIDNode);
            var IDOrFuncDelimiter               = new NonTerminal("IDOrFuncDelimiter");
            var dotWrapper                      = new NonTerminal("dotWrapper", CreateDotDelimiter);
            var edgeAccessorWrapper             = new NonTerminal("edgeAccessorWrapper", CreateEdgeAccessorDelimiter);
            var EdgeInformation                 = new NonTerminal("EdgeInformation", CreateEdgeInformation);
            var EdgeTraversalWithFunctions      = new NonTerminal("EdgeTraversalWithFunctions", CreateEdgeTraversal);
            var EdgeTraversalWithOutFunctions   = new NonTerminal("EdgeTraversalWithOutFunctions", CreateEdgeTraversal);
            
            #endregion

            #region AStatements

            var singlestmt                  = new NonTerminal("singlestmt");
//            var stmt = new NonTerminal("stmt", typeof(StatementNode)); 
            var createTableStmt             = new NonTerminal("createTableStmt");
            var createIndexStmt             = new NonTerminal("createIndexStmt", CreateCreateIndexStatementNode);
            var alterStmt                   = new NonTerminal("alterStmt", CreateAlterStmNode);
            var dropTypeStmt                = new NonTerminal("dropTypeStmt", CreateDropTypeStmNode);
            var dropIndexStmt               = new NonTerminal("dropIndexStmt", CreateDropIndexStmNode);
            var InsertStmt                  = new NonTerminal("InsertStmt", CreateInsertStatementNode);
            var updateStmt                  = new NonTerminal("updateStmt", CreateUpdateStatementNode);
            var deleteStmt                  = new NonTerminal("deleteStmt", CreateDeleteStatementNode);
            var SelectStmtPandora           = new NonTerminal("SelectStmtPandora", CreateSelectStatementNode);
            var parSelectStmt               = new NonTerminal("parSelectStmt", CreatePartialSelectStmtNode);
            var createTypesStmt             = new NonTerminal("createTypesStmt", CreateCreateTypesStatementNode);
            var insertorupdateStmt          = new NonTerminal("insertorupdateStmt", CreateInsertOrUpdateStatementNode);
            var insertorreplaceStmt         = new NonTerminal("insertorreplaceStmt", CreateInsertOrReplaceStatementNode);
            var replaceStmt                 = new NonTerminal("replaceStmt", CreateReplaceStatementNode);
            var transactStmt                = new NonTerminal("transactStmt", CreateTransActionStatementNode);
            var commitRollBackTransactStmt  = new NonTerminal("commitRollBackTransactStmt", CreateCommitRollbackTransActionNode);
            
            #endregion

            var deleteStmtMember            = new NonTerminal("deleteStmtMember");
            var uniqueOpt                   = new NonTerminal("uniqueOpt", typeof(uniqueOptNode));
            var IndexAttributeList          = new NonTerminal("IndexAttributeList", typeof(CreateIndexAttributeListNode));
            var IndexAttributeMember        = new NonTerminal("IndexAttributeMember", typeof(CreateIndexAttributeNode));
            var IndexAttributeType          = new NonTerminal("IndexAttributeType");
            var orderByAttributeList        = new NonTerminal("orderByAttributeList");
            var orderByAttributeListMember  = new NonTerminal("orderByAttributeListMember");
            var AttributeOrderDirectionOpt  = new NonTerminal("AttributeOrderDirectionOpt");
            var indexTypeOpt                = new NonTerminal("indexTypeOpt", typeof(IndexTypeOptNode));
            var indexNameOpt                = new NonTerminal("indextNameOpt", typeof(IndexNameOptNode));
            var editionOpt                  = new NonTerminal("editionOpt", typeof(EditionOptNode));
            var alterCmd                    = new NonTerminal("alterCmd", typeof(AlterCommandNode));
            var insertData                  = new NonTerminal("insertData");
            var intoOpt                     = new NonTerminal("intoOpt");
            var assignList                  = new NonTerminal("assignList");
            var whereClauseOpt              = new NonTerminal("whereClauseOpt", CreateWhereExpressionNode);
            var extendsOpt                  = new NonTerminal("extendsOpt");
            var abstractOpt                 = new NonTerminal("abstractOpt");
            var commentOpt                  = new NonTerminal("CommentOpt");
            var bulkTypeList                = new NonTerminal("bulkTypeList");
            var attributesOpt               = new NonTerminal("attributesOpt");
            var insertValuesOpt             = new NonTerminal("insertValuesOpt");

            #region Expression

            var expression                  = new NonTerminal("expression", typeof(ExpressionNode));
            var expressionOfAList           = new NonTerminal("expressionOfAList", typeof(ExpressionOfAListNode));
            var exprList                    = new NonTerminal("exprList");
            var exprListOfAList             = new NonTerminal("exprListOfAList");
            var unExpr                      = new NonTerminal("unExpr", CreateUnExpressionNode);
            var unOp                        = new NonTerminal("unOp");
            var binExpr                     = new NonTerminal("binExpr", CreateBinaryExpressionNode);
            var binOp                       = new NonTerminal("binOp");
            var inExpr                      = new NonTerminal("inExpr");
            
            #endregion

            #region Select

            var selList                     = new NonTerminal("selList");
            var fromClauseOpt               = new NonTerminal("fromClauseOpt");
            var groupClauseOpt              = new NonTerminal("groupClauseOpt");
            var havingClauseOpt             = new NonTerminal("havingClauseOpt", typeof(HavingExpressionNode));
            var orderClauseOpt              = new NonTerminal("orderClauseOpt", typeof(OrderByNode));
            var selectionList               = new NonTerminal("selectionList");
            var selectionListElement        = new NonTerminal("selectionListElement", typeof(SelectionListElementNode));
            var selectionSource             = new NonTerminal("selectionSource");
            var asOpt                       = new NonTerminal("asOpt");
            var aliasOpt                    = new NonTerminal("aliasOpt");
            var aliasOptName                = new NonTerminal("aliasOptName");
            var selectOutputOpt             = new NonTerminal("selectOutputOpt", typeof(SelectOutputOptNode));

            #endregion

            #region Aggregates & Functions

            var aggregate                   = new NonTerminal("aggregate", CreateAggregateNode);
            var aggregateArg                = new NonTerminal("aggregateArg");
            var aggregateName               = new NonTerminal("aggregateName");
            var function                    = new NonTerminal("function", CreateFunctionCallNode);
            var functionName                = new NonTerminal("functionName");

            #endregion

            #region Tuple

            var tuple                       = new NonTerminal("tuple", typeof(TupleNode));
            var bracketLeft                 = new NonTerminal(DBConstants.BracketLeft);
            var bracketRight                = new NonTerminal(DBConstants.BracketRight);
            

            #endregion

            var term                        = new NonTerminal("term");
            var notOpt                      = new NonTerminal("notOpt");
            var funcCall                    = new NonTerminal("funCall", CreateFunctionCallNode);
            var funArgs                     = new NonTerminal("funArgs");
            var GraphDBType                 = new NonTerminal(DBConstants.GraphDBType, CreateGraphDBTypeNode);
            var AttributeList               = new NonTerminal("AttributeList");
            var AttrDefinition              = new NonTerminal("AttrDefinition", CreateAttributeDefinitionNode);
            var ResultObject                = new NonTerminal("ResultObject");
            var ResultList                  = new NonTerminal("ResultList");
            var MatchingClause              = new NonTerminal("MatchingClause");
            var Matching                    = new NonTerminal("MatchingClause");
            var PrefixOperation             = new NonTerminal("PrefixOperation");
            var ParameterList               = new NonTerminal("ParameterList");
            var TypeList                    = new NonTerminal("TypeList");
            var AType                       = new NonTerminal("AType", CreateATypeNode);
            var TypeWrapper                 = new NonTerminal("TypeWrapper");
            var AttrAssignList              = new NonTerminal("AttrAssignList");
            var AttrUpdateList              = new NonTerminal("AttrUpdateList", typeof(AttrUpdateOrAssignListNode));
            var AttrAssign                  = new NonTerminal("AttrAssign", typeof(AttributeAssignNode));
            var AttrRemove                  = new NonTerminal("AttrRemove", typeof(AttrRemoveNode));
            var ListAttrUpdate              = new NonTerminal("AttrUpdate");
            var AddToListAttrUpdate         = new NonTerminal("AddToListAttrUpdate", typeof(AddToListAttrUpdateNode));
            var AddToListAttrUpdateAddTo    = new NonTerminal("AddToListAttrUpdateAddTo", CreateAddToListAttrUpdateAddToNode);
            var AddToListAttrUpdateOperator = new NonTerminal("AddToListAttrUpdateOperator", CreateAddToListAttrUpdateOperatorNode);
            var RemoveFromListAttrUpdate    = new NonTerminal("RemoveFromListAttrUpdate", typeof(RemoveFromListAttrUpdateNode));
            var RemoveFromListAttrUpdateAddToRemoveFrom = new NonTerminal("RemoveFromListAttrUpdateAddToRemoveFrom", CreateRemoveFromListAttrUpdateAddToRemoveFromNode);
            var RemoveFromListAttrUpdateAddToOperator   = new NonTerminal("RemoveFromListAttrUpdateAddToOperator", CreateRemoveFromListAttrUpdateAddToOperatorNode);
            var AttrUpdateOrAssign          = new NonTerminal("AttrUpdateOrAssign");
            var CollectionOfDBObjects       = new NonTerminal("ListOfDBObjects", typeof(CollectionOfDBObjectsNode));
            var CollectionTuple = new NonTerminal("CollectionTuple", typeof(TupleNode));
            var ExtendedExpressionList = new NonTerminal("ExtendedExpressionList");
            var ExtendedExpression = new NonTerminal("ExtendedExpression", typeof(ExpressionOfAListNode));

            

            var Reference                   = new NonTerminal(S_REFERENCE.Symbol, typeof(SetRefNode));
            var offsetOpt                   = new NonTerminal("offsetOpt", typeof(OffsetNode));
            var resolutionDepthOpt          = new NonTerminal("resolutionDepthOpt");
            var limitOpt                    = new NonTerminal("limitOpt", typeof(LimitNode));
            var SimpleIdList                = new NonTerminal("SimpleIdList");
            var bulkTypeListMember          = new NonTerminal("bulkTypeListMember", CreateBulkTypeListMemberNode);
            var bulkType                    = new NonTerminal("bulkType", CreateBulkTypeNode);
            var truncateStmt                = new NonTerminal("truncateStmt", CreateTruncateStmNode);
            var uniquenessOpt               = new NonTerminal("UniquenessOpt", typeof(UniqueAttributesOptNode));
            var mandatoryOpt                = new NonTerminal("MandatoryOpt", typeof(MandatoryOptNode));
            var TransactOptions             = new NonTerminal("TransactOptions");
            var TransactAttributes          = new NonTerminal("TransactAttributes");
            var TransactIsolation           = new NonTerminal("TransactIsolation");
            var TransactName                = new NonTerminal("TransactName");
            var TransactTimestamp           = new NonTerminal("TransactTimestamp");
            var TransactCommitRollbackOpt   = new NonTerminal("TransactCommitRollbackOpt");
            var TransactCommitRollbackType  = new NonTerminal("TransactCommitRollbackType");            
            
            var Value                       = new NonTerminal("Value");
            var ValueList                   = new NonTerminal("ValueList");
            var BooleanVal                  = new NonTerminal("BooleanVal");            

            var ListType                    = new NonTerminal("ListType");
            var ListParametersForExpression = new NonTerminal("ListParametersForExpression", typeof(ParametersNode));

            #region EdgeType

            var EdgeTypeDef                 = new NonTerminal("EdgeTypeDef", CreateEdgeTypeDefNode);
            var SingleEdgeTypeDef           = new NonTerminal("EdgeTypeDef", CreateSingleEdgeTypeDefNode);
            var DefaultValueDef             = new NonTerminal("DefaultValueDef", typeof(DefaultValueDefNode));
            var EdgeTypeParams              = new NonTerminal("EdgeTypeParams", typeof(EdgeTypeParamsNode));
            var EdgeTypeParam               = new NonTerminal("EdgeTypeParamNode", typeof(EdgeTypeParamNode));
            var EdgeType_Sorted             = new NonTerminal("ListPropertyAssign_Sorted", typeof(EdgeType_SortedNode));
            var EdgeType_SortedMember       = new NonTerminal("ListPropertyAssign_SortedMember");
            var AttrDefaultOpValue          = new NonTerminal("AttrDefaultOpValue", CreateAttrDefaultValueNode);

            #endregion

            #region Settings

            var SettingsStatement           = new NonTerminal("SettingStatement", CreateSettingStatementNode);
            var SettingTypeNode             = new NonTerminal("SettingTypeNode", CreateSettingTypeNode);
            var SettingAttrNode             = new NonTerminal("SettingAttrNode", CreateSettingAttrNode);
            var SettingScope                = new NonTerminal("SettingScope", typeof(SettingScopeNode));
            var SettingOpGet                = new NonTerminal("SettingOpGet");
            var SettingOpSet                = new NonTerminal("SettingOpSet");
            var SettingOpRemove             = new NonTerminal("SettingOpRemove");
            var SettingOperation            = new NonTerminal("SettingOperation", typeof(SettingOperationNode));
            var SettingOpSetLst             = new NonTerminal("SettingOpSetLst");
            var SettingOpGetLst             = new NonTerminal("SettingOpGetLst");
            var SettingOpRemLst             = new NonTerminal("SettingOpRemLst");
            var SettingItems                = new NonTerminal("SettingItems");
            var SettingItemsSet             = new NonTerminal("SettingItemsSet");
            var SettingItemSetVal           = new NonTerminal("SettingItemSetVal");
            var SettingItemSetLst           = new NonTerminal("SettingItemSetLst");
            var SettingTypeStmLst           = new NonTerminal("SettingTypeStmLst");
            var SettingAttrStmLst           = new NonTerminal("SettingAttrStmLst");

            #endregion

            #region BackwardEdges

            var backwardEdgesOpt            = new NonTerminal("BackwardEdges", CreateBackwardEdgesNode);
            var BackwardEdgesSingleDef      = new NonTerminal("BackwardEdgesSingleDef", CreateBackwardEdgeNode);
            var BackwardEdgesList           = new NonTerminal("BackwardEdgesList");

            #endregion

            #region Index

            var indexOptOnCreateType        = new NonTerminal("IndexOptOnCreateType");
            var IndexOptOnCreateTypeMember = new NonTerminal("IndexOptOnCreateTypeMember", typeof(IndexOptOnCreateTypeMemberNode));
            var IndexOptOnCreateTypeMemberList = new NonTerminal("IndexOptOnCreateTypeMemberList");

            #endregion

            #region Dump

            var dumpStmt                        = new NonTerminal("Dump", CreateDumpNode);

            #endregion

            #region Describe

            var DescrInfoStmt               = new NonTerminal("DescrInfoStmt", CreateDescribeNode);
            var DescrArgument               = new NonTerminal("DescrArgument");
            var DescrFuncStmt               = new NonTerminal("DescrFuncStmt", CreateDescrFunc);
            var DescrFunctionsStmt          = new NonTerminal("DescrFunctionsStmt", CreateDescrFunctions);
            var DescrAggrStmt               = new NonTerminal("DescrAggrStmt", CreateDescrAggr);
            var DescrAggrsStmt              = new NonTerminal("DescrAggrsStmt", CreateDescrAggrs);
            var DescrSettStmt               = new NonTerminal("DescrSettStmt", CreateDescrSett);
            var DescrSettItem               = new NonTerminal("DescrSettItem", CreateDescrSettItem);
            var DescrSettingsItems          = new NonTerminal("DescrSettingsItems", CreateDescrSettingsItems); 
            var DescrSettingsStmt           = new NonTerminal("DescrSettingsStmt", CreateDescrSettings);
            var DescrObjStmt                = new NonTerminal("DescrObjStmt", CreateDescrObj);
            var DescrTypeStmt               = new NonTerminal("DescrTypeStmt", CreateDescrType);
            var DescrTypesStmt              = new NonTerminal("DescrTypesStmt", CreateDescrTypes);
            var DescrIdxStmt                = new NonTerminal("DescrIdxStmt", CreateDescrIdx);
            var DescrIdxsStmt               = new NonTerminal("DescrIdxsStmt", CreateDescrIdxs);
            var DescrIdxEdtStmt             = new NonTerminal("DescrIdxEdtStmt");
            var DescrEdgeStmt               = new NonTerminal("DescrEdgeStmt", CreateDescrEdge);
            var DescrEdgesStmt              = new NonTerminal("DescrEdgesStmt", CreateDescrEdges);

            #endregion

            #region REBUILD INDICES

            var rebuildIndicesStmt          = new NonTerminal("rebuildIndicesStmt", CreateRebuildIndicesNode);
            var rebuildIndicesTypes         = new NonTerminal("rebuildIndiceTypes");

            #endregion

            #region Import

            var importStmt          = new NonTerminal("import", CreateImportNode);
            var paramParallelTasks  = new NonTerminal("parallelTasks", CreateParallelTaskNode);
            var paramComments       = new NonTerminal("comments", CreateCommentsNode);
            var verbosity           = new NonTerminal("verbosity", CreateVerbosityNode);
            var verbosityTypes      = new NonTerminal("verbosityTypes");

            #endregion

            #endregion

            #region Statements

            #region root

            //BNF Rules
            this.Root = singlestmt;

            singlestmt.Rule = SelectStmtPandora
                            | InsertStmt
                            | alterStmt
                            | updateStmt
                            | dropTypeStmt
                            | dropIndexStmt
                            | createIndexStmt
                            | createTypesStmt
                            | deleteStmt
                            | SettingsStatement
                            | truncateStmt
                            | DescrInfoStmt
                            | insertorupdateStmt
                            | insertorreplaceStmt
                            | replaceStmt
                            | dumpStmt
                            | transactStmt
                            | commitRollBackTransactStmt
                            | rebuildIndicesStmt
                            | importStmt;
                            

            #endregion

            #region misc

            #region ID

            #region wo functions

            Id_simple.Rule = name;

            EdgeTraversalWithOutFunctions.Rule = dotWrapper + Id_simple;

            Id.SetOption(TermOptions.IsList);
            Id.Rule =       Id_simple
                        |   Id + EdgeTraversalWithOutFunctions;
            //old
            //Id.Rule = MakePlusRule(Id, dotWrapper, Id_simple);

            Id.Description = "an id is composed by an identifier a dot and a second identifier -  or a list of them an id could be ‘U.Name’ or ‘U.Friends.Age’\n";
            idlist.Rule = MakePlusRule(idlist, S_comma, Id);
            id_simpleList.Rule = MakePlusRule(id_simpleList, S_comma, Id_simple);
            id_simpleDotList.Rule = MakePlusRule(id_simpleDotList, S_dot, Id_simple);
            id_typeAndAttribute.Rule = TypeWrapper + S_dot + Id;

            #endregion

            #region ID_or_Func

            IdOrFunc.Rule =     name 
                            |   funcCall;

            dotWrapper.Rule = S_edgeTraversalDelimiter;

            edgeAccessorWrapper.Rule = S_edgeInformationDelimiterSymbol;

            IDOrFuncDelimiter.Rule =        dotWrapper
                                        |   edgeAccessorWrapper;

            EdgeTraversalWithFunctions.Rule = dotWrapper + IdOrFunc;

            EdgeInformation.Rule = edgeAccessorWrapper + Id_simple;

            IdOrFuncList.SetOption(TermOptions.IsList);
            IdOrFuncList.Rule =         IdOrFunc
                                    |   IdOrFuncList + EdgeInformation
                                    |   IdOrFuncList + EdgeTraversalWithFunctions;

            //old
            //IdOrFuncList.Rule = MakePlusRule(IdOrFuncList, IDOrFuncDelimiter, IdOrFunc);

            #endregion

            #endregion

            #region typeList

            TypeList.Rule = MakePlusRule(TypeList, S_comma, AType);
            TypeList.Description = "specify the type object to be selected for example a type list could be ‘User U’, ‘Car C’, …\n";

            AType.Rule = Id_simple + Id_simple
                        | Id_simple;

            //AType.Rule = Id + Id_simple
            //                | Id;

            TypeWrapper.Rule = AType;

            #endregion

            #region CreateIndexAttribute

            IndexAttributeList.Rule = MakePlusRule(IndexAttributeList, S_comma, IndexAttributeMember);

            IndexAttributeMember.Rule = IndexAttributeType;// + AttributeOrderDirectionOpt;

            IndexAttributeType.Rule = Id_simple | id_typeAndAttribute;

            #endregion

            #region OrderDirections

            AttributeOrderDirectionOpt.Rule = Empty
                                                | S_ASC
                                                | S_DESC;

            #endregion

            #region Boolean

            BooleanVal.Rule = S_TRUE | S_FALSE;

            #endregion

            #region Value

            Value.Rule = string_literal | number | BooleanVal;

            ValueList.Rule = MakeStarRule(ValueList, S_comma, Value);

            #endregion

            #region ListType

            ListType.Rule = S_LIST;

            ListParametersForExpression.Rule = Empty
                                         | S_colon + S_BRACKET_LEFT + ValueList + S_BRACKET_RIGHT;

            EdgeType_SortedMember.Rule = S_ASC | S_DESC;
            EdgeType_Sorted.Rule = S_SORTED + "=" + EdgeType_SortedMember;

            #endregion

            #region PandoraType

            //                 SET<                   WEIGHTED  (Double, DEFAULT=2, SORTED=DESC)<   [idsimple]  >>
            EdgeTypeDef.Rule = S_SET + S_ListTypePrefix + Id_simple + S_BRACKET_LEFT + EdgeTypeParams + S_BRACKET_RIGHT + S_ListTypePrefix + Id_simple + S_ListTypePostfix + S_ListTypePostfix;
            //                       COUNTED        (Integer, DEFAULT=2)                   <   [idsimple]  >
            SingleEdgeTypeDef.Rule = Id_simple + S_BRACKET_LEFT + EdgeTypeParams + S_BRACKET_RIGHT + S_ListTypePrefix + Id_simple + S_ListTypePostfix;

            EdgeTypeParams.Rule = MakeStarRule(EdgeTypeParams, S_comma, EdgeTypeParam);
            EdgeTypeParam.Rule = Id_simple
                               | DefaultValueDef
                               | EdgeType_Sorted
                               | string_literal;

            EdgeTypeParam.SetOption(TermOptions.IsTransient, false);

            DefaultValueDef.Rule = S_DEFAULT + "=" + Value;

            GraphDBType.Rule = Id_simple
                                   // LIST<[idsimple]>
                                   | S_LIST + S_ListTypePrefix + Id_simple + S_ListTypePostfix
                                   | S_SET + S_ListTypePrefix + Id_simple + S_ListTypePostfix
                                   | EdgeTypeDef
                                   | SingleEdgeTypeDef;

            #endregion

            #region AttributeList

            AttributeList.Rule = MakePlusRule(AttributeList, S_comma, AttrDefinition);

            AttrDefinition.Rule = GraphDBType + Id_simple + AttrDefaultOpValue;

            #endregion

            #region BackwardEdgesList

            BackwardEdgesList.Rule = MakePlusRule(BackwardEdgesList, S_comma, BackwardEdgesSingleDef);

            BackwardEdgesSingleDef.Rule = Id_simple + S_dot + Id_simple + Id_simple;
                                            //| Id_simple + S_dot + Id_simple + S_ListTypePrefix + Id_simple + S_BRACKET_LEFT + EdgeTypeParams + S_BRACKET_RIGHT + S_ListTypePostfix + Id_simple;

            #endregion

            #region id_simple list

            SimpleIdList.Rule = MakePlusRule(SimpleIdList, S_comma, Id_simple);

            #endregion

            #region expression

            //Expression
            exprList.Rule = MakeStarRule(exprList, S_comma, expression);

            exprListOfAList.Rule = MakePlusRule(exprListOfAList, S_comma, expressionOfAList);

            expression.Rule =       term
                                |   unExpr
                                |   binExpr;

            expressionOfAList.Rule = expression + ListParametersForExpression;


            term.Rule =         IdOrFuncList                  //d.Name 
                            |   string_literal      //'lala'
                            |   number              //10
                            //|   funcCall            //EXISTS ( SelectStatement )
                            |   aggregate           //COUNT ( SelectStatement )
                            |   tuple               //(d.Name, 'Henning', (SelectStatement))
                            |   parSelectStmt      //(FROM User u Select u.Name)
                            | S_TRUE
                            | S_FALSE;

            #region Tuple

            tuple.Rule = bracketLeft + exprList + bracketRight;

            bracketLeft.Rule = S_BRACKET_LEFT | S_TUPLE_BRACKET_LEFT;
            bracketRight.Rule = S_BRACKET_RIGHT | S_TUPLE_BRACKET_RIGHT;

            #endregion

            parSelectStmt.Rule = S_BRACKET_LEFT + SelectStmtPandora + S_BRACKET_RIGHT;

            unExpr.Rule = unOp + term;

            unOp.Rule =         S_NOT 
                            |   "+" 
                            |   "-" 
                            |   "~";

            binExpr.Rule = expression + binOp + expression;

            binOp.Rule =        Symbol("+") 
                            |   "-" 
                            |   "*" 
                            |   "/" 
                            |   "%" //arithmetic
                            |   "&" 
                            |   "|" 
                            |   "^"                     //bit
                            |   "=" 
                            |   ">" 
                            |   "<" 
                            |   ">=" 
                            |   "<=" 
                            |   "<>" 
                            |   "!=" 
                            |   "!<" 
                            |   "!>"
                            |   "AND" 
                            |   "OR" 
                            |   "LIKE"
                            |   S_NOT + "LIKE" 
                            |   "IN" 
                            |   "NOTIN" | "NOT_IN" | "NIN" | "!IN"
                            |   "INRANGE";

            notOpt.Rule =       Empty
                            |   S_NOT;

            #endregion

            #region Functions

            //funcCall covers some psedo-operators and special forms like ANY(...), SOME(...), ALL(...), EXISTS(...), IN(...)
            funcCall.Rule = name + S_BRACKET_LEFT + funArgs + S_BRACKET_RIGHT;

            funArgs.Rule =      SelectStmtPandora 
                            |   exprList;

            #endregion

            #region operators

            //Operators
            RegisterOperators(10, "*", "/", "%");
            RegisterOperators(9, "+", "-");
            RegisterOperators(8, "=", ">", "<", ">=", "<=", "<>", "!=", "!<", "!>", "INRANGE");
            RegisterOperators(7, "^", "&", "|");
            RegisterOperators(6, "NOT");
            RegisterOperators(5, "AND");
            RegisterOperators(4, "OR", "LIKE", "IN", "NOTIN", "NOT_IN", "NIN", "!IN");

            #endregion

            #region prefixOperation

            PrefixOperation.Rule =      Id_simple + S_BRACKET_LEFT + ParameterList + S_BRACKET_RIGHT;

            ParameterList.Rule =        ParameterList + S_comma + expression
                                    |   expression;

            #endregion

            #endregion

            #region CREATE INDEX

            createIndexStmt.Rule = S_CREATE + S_INDEX + indexNameOpt + editionOpt + S_ON + TypeWrapper + S_BRACKET_LEFT + IndexAttributeList + S_BRACKET_RIGHT + indexTypeOpt;
            uniqueOpt.Rule = Empty | S_UNIQUE;

            editionOpt.Rule =       Empty
                                | S_EDITION + Id_simple;

            indexTypeOpt.Rule =     Empty
                                | S_INDEXTYPE + Id_simple;

            indexNameOpt.Rule = Empty
                                |   Id_simple;

            #endregion

            #region CREATE TYPE(S)

            createTypesStmt.Rule    = S_CREATE + S_TYPES + bulkTypeList
                                    | S_CREATE +  abstractOpt + S_TYPE + bulkType;

            bulkTypeList.Rule       = MakePlusRule(bulkTypeList, S_comma, bulkTypeListMember);

            bulkTypeListMember.Rule = abstractOpt + bulkType;

            bulkType.Rule           = Id_simple + extendsOpt + attributesOpt + backwardEdgesOpt + uniquenessOpt + mandatoryOpt + indexOptOnCreateType + commentOpt;

            commentOpt.Rule         =   Empty
                                    |   S_COMMENT + "=" + string_literal; 

            abstractOpt.Rule        = Empty
                                    | S_ABSTRACT; 

            extendsOpt.Rule         = Empty
                                    | S_EXTENDS + Id_simple;

            attributesOpt.Rule      = Empty
                                    | S_ATTRIBUTES + S_BRACKET_LEFT + AttributeList + S_BRACKET_RIGHT;

            backwardEdgesOpt.Rule   = Empty
                                    | S_BACKWARDEDGES + S_BRACKET_LEFT + BackwardEdgesList + S_BRACKET_RIGHT;

            uniquenessOpt.Rule = Empty
                                    | S_UNIQUE + S_BRACKET_LEFT + id_simpleList + S_BRACKET_RIGHT;

            mandatoryOpt.Rule = Empty
                                    | S_MANDATORY + S_BRACKET_LEFT + id_simpleList + S_BRACKET_RIGHT;

            indexOptOnCreateType.Rule = Empty
                                    | S_INDICES + S_BRACKET_LEFT + IndexOptOnCreateTypeMemberList + S_BRACKET_RIGHT
                                    | S_INDICES + IndexOptOnCreateTypeMember;

            IndexOptOnCreateTypeMemberList.Rule = MakePlusRule(IndexOptOnCreateTypeMemberList, S_comma, IndexOptOnCreateTypeMember);

            IndexOptOnCreateTypeMember.Rule = S_BRACKET_LEFT + indexNameOpt + editionOpt + indexTypeOpt + S_ON + IndexAttributeList + S_BRACKET_RIGHT
                                            | S_BRACKET_LEFT + IndexAttributeList + S_BRACKET_RIGHT;

            AttrDefaultOpValue.Rule = Empty
                                    | "=" + Value
                                    | "=" + S_LISTOF + S_BRACKET_LEFT + ValueList + S_BRACKET_RIGHT
                                    | "=" + S_SETOF + S_BRACKET_LEFT + ValueList + S_BRACKET_RIGHT;
            #endregion

            #region ALTER TYPE

            alterStmt.Rule = S_ALTER + S_TYPE + Id_simple + alterCmd + uniquenessOpt + mandatoryOpt;

            alterCmd.Rule = Empty
                            | S_ADD + S_ATTRIBUTES + S_BRACKET_LEFT + AttributeList + S_BRACKET_RIGHT
                            | S_DROP + S_ATTRIBUTES + S_BRACKET_LEFT + SimpleIdList + S_BRACKET_RIGHT
                            | S_ADD + S_BACKWARDEDGES + S_BRACKET_LEFT + BackwardEdgesList + S_BRACKET_RIGHT
                            | S_DROP + S_BACKWARDEDGES + S_BRACKET_LEFT + SimpleIdList + S_BRACKET_RIGHT
                            | S_RENAME + S_ATTRIBUTE + Id_simple + S_TO + Id_simple
                            | S_RENAME + S_BACKWARDEDGE + Id_simple + S_TO + Id_simple
                            | S_RENAME + S_TO + Id_simple
                            | S_DROP + S_UNIQUE
                            | S_DROP + S_MANDATORY
                            | S_COMMENT + "=" + string_literal;
            #endregion

            #region SELECT

            SelectStmtPandora.Rule = S_FROM + TypeList + S_SELECT + selList + whereClauseOpt + groupClauseOpt + havingClauseOpt + orderClauseOpt + MatchingClause + offsetOpt + limitOpt + resolutionDepthOpt + selectOutputOpt;
            SelectStmtPandora.Description = "The select statement is used to query the database and retrieve one or more types of objects in the database.\n";

            MatchingClause.Rule =       Empty
                                    |   MatchingClause + Matching;

            Matching.Rule =             S_MATCHES + S_BRACKET_LEFT + number + S_BRACKET_RIGHT + PrefixOperation;

            resolutionDepthOpt.Rule =       Empty
                                        |   S_DEPTH + number;

            selectOutputOpt.Rule    =       Empty
                                        |   "OUTPUT" + name;

            offsetOpt.Rule =       Empty
                            |   S_OFFSET + number;

            limitOpt.Rule =     Empty
                            |   S_LIMIT + number;

            selList.Rule =      selectionList;

            selectionList.Rule = MakePlusRule(selectionList, S_comma, selectionListElement);            

            selectionListElement.Rule =     "*"
                                        |   selectionSource + aliasOpt; 

            aliasOptName.Rule = Id_simple | string_literal;

            aliasOpt.Rule =     Empty
                            |   S_AS + aliasOptName;

            selectionSource.Rule = aggregate
                //|   funcCall
                //|   Id;
                                | IdOrFuncList;

            #region Aggregate

            aggregate.Rule = aggregateName + S_BRACKET_LEFT + aggregateArg + S_BRACKET_RIGHT;

            aggregateArg.Rule =     Id
                                |   "*";

            aggregateName.Rule =        S_COUNT 
                                    |   "AVG" 
                                    |   "MIN" 
                                    |   "MAX" 
                                    |   "STDEV" 
                                    |   "STDEVP" 
                                    |   "SUM" 
                                    |   "VAR" 
                                    |   "VARP";

            #endregion

            #region Functions

            //function.Rule           = functionName + S_BRACKET_LEFT + term + S_BRACKET_RIGHT;

            //functionName.Rule       = FUNC_WEIGHT;

            #endregion

            whereClauseOpt.Rule =       Empty 
                                    |   S_WHERE + expression;

            groupClauseOpt.Rule =       Empty 
                                    |   "GROUP" + S_BY + idlist;

            havingClauseOpt.Rule =      Empty 
                                    |   "HAVING" + expression;


            orderByAttributeListMember.Rule =       Id
                                                |   string_literal;

            orderByAttributeList.Rule = MakePlusRule(orderByAttributeList, S_comma, orderByAttributeListMember);

            orderClauseOpt.Rule =       Empty 
                                    |   "ORDER" + S_BY + orderByAttributeList + AttributeOrderDirectionOpt;

            #endregion

            #region INSERT

            InsertStmt.Rule = S_INSERT + S_INTO + TypeWrapper + insertValuesOpt;

            insertValuesOpt.Rule =      Empty
                                    |   S_VALUES + S_BRACKET_LEFT + AttrAssignList + S_BRACKET_RIGHT;

            AttrAssignList.Rule = MakePlusRule(AttrAssignList, S_comma, AttrAssign);

            AttrAssign.Rule =       Id + "=" + expression
                                |   Id + "=" + Reference
                                |   Id + "=" + CollectionOfDBObjects;

            CollectionOfDBObjects.Rule = S_SETOF + CollectionTuple
                                            | S_LISTOF + CollectionTuple
                                            | S_SETOFUUIDS + CollectionTuple
                                            | S_SETOF + "()";

            CollectionTuple.Rule = S_BRACKET_LEFT + ExtendedExpressionList + S_BRACKET_RIGHT;

            ExtendedExpressionList.Rule = MakePlusRule(ExtendedExpressionList, S_comma, ExtendedExpression);

            ExtendedExpression.Rule = expression + ListParametersForExpression;

            Reference.Rule = S_REFERENCE + tuple + ListParametersForExpression
                           | S_REF + tuple + ListParametersForExpression
                           | S_REFUUID + tuple + ListParametersForExpression
                           | S_REFERENCEUUID + tuple + ListParametersForExpression;

                //| S_SETREF + tupleRangeSet + ListParametersForExpression;

            #endregion

            #region UPDATE

            updateStmt.Rule = S_UPDATE + TypeWrapper + S_SET + S_BRACKET_LEFT + AttrUpdateList + S_BRACKET_RIGHT + whereClauseOpt;

            AttrUpdateList.Rule = MakePlusRule(AttrUpdateList, S_comma, AttrUpdateOrAssign);

            AttrUpdateOrAssign.Rule =       AttrAssign
                                        |   AttrRemove
                                        |   ListAttrUpdate;

            AttrRemove.Rule = S_REMOVE + S_ATTRIBUTES + S_BRACKET_LEFT + id_simpleList + S_BRACKET_RIGHT;

            ListAttrUpdate.Rule =       AddToListAttrUpdate
                                    |   RemoveFromListAttrUpdate;

            AddToListAttrUpdate.Rule =      AddToListAttrUpdateAddTo
                                        |   AddToListAttrUpdateOperator;

            AddToListAttrUpdateAddTo.Rule = S_ADD + S_TO + Id + CollectionOfDBObjects;
            AddToListAttrUpdateOperator.Rule = Id + S_ADDTOLIST + CollectionOfDBObjects;

            RemoveFromListAttrUpdate.Rule =         RemoveFromListAttrUpdateAddToRemoveFrom
                                            |       RemoveFromListAttrUpdateAddToOperator;

            RemoveFromListAttrUpdateAddToRemoveFrom.Rule = S_REMOVE + S_FROM + Id + tuple;
            RemoveFromListAttrUpdateAddToOperator.Rule = Id + S_REMOVEFROMLIST + tuple;


            #endregion

            #region DROP TYPE

            dropTypeStmt.Rule = S_DROP + S_TYPE + Id_simple;

            #endregion

            #region DROP INDEX

            dropIndexStmt.Rule = S_FROM + TypeWrapper + S_DROP + S_INDEX + Id_simple + editionOpt;

            #endregion

            #region TRUNCATE

            truncateStmt.Rule = S_TRUNCATE + Id_simple;

            #endregion

            #region DELETE

            deleteStmtMember.Rule = Empty | idlist;
            deleteStmt.Rule = S_FROM + TypeList + S_DELETE + deleteStmtMember + whereClauseOpt;

            #endregion

            #region SETTING

            SettingsStatement.Rule = S_SETTING + SettingScope + SettingOperation;

            SettingScope.Rule = S_DB | S_SESSION | SettingTypeNode | SettingAttrNode;

            SettingTypeNode.Rule = S_TYPE + SettingTypeStmLst;

            SettingTypeStmLst.Rule = MakePlusRule(SettingTypeStmLst, S_comma, TypeWrapper);

            SettingAttrNode.Rule = S_ATTRIBUTE + SettingAttrStmLst;

            SettingAttrStmLst.Rule = MakePlusRule(SettingAttrStmLst, S_comma, id_typeAndAttribute);

            SettingOperation.Rule = SettingOpSet | SettingOpGet | SettingOpRemove;

            SettingOpSet.Rule = S_SET + S_BRACKET_LEFT + SettingItemSetLst + S_BRACKET_RIGHT;

            SettingItemsSet.Rule = string_literal + "=" + SettingItemSetVal;

            SettingItemSetLst.Rule = MakePlusRule(SettingItemSetLst, S_comma, SettingItemsSet);

            SettingItemSetVal.Rule =        number 
                                        |   S_DEFAULT
                                        |   string_literal;

            SettingOpGet.Rule = S_GET + S_BRACKET_LEFT + SettingItems + S_BRACKET_RIGHT;

            SettingOpRemove.Rule = S_REMOVE + S_BRACKET_LEFT + SettingItems + S_BRACKET_RIGHT;

            SettingItems.Rule = MakePlusRule(SettingItems, S_comma, string_literal);

            #endregion

            #region DESCRIBE

            DescrInfoStmt.Rule = S_DESCRIBE + DescrArgument;
            DescrInfoStmt.Description = "This statement gives you all information about an type, a function, an index, an setting, an object, an edge or an aggregate.\n";

            DescrArgument.Rule = DescrAggrStmt | DescrAggrsStmt | DescrEdgeStmt | DescrEdgesStmt | DescrTypeStmt | DescrTypesStmt | DescrFuncStmt | DescrFunctionsStmt | DescrSettStmt | DescrSettingsStmt | DescrIdxStmt | DescrIdxsStmt;

            DescrAggrStmt.Rule = S_DESCAGGR + Id_simple;

            DescrAggrsStmt.Rule = S_DESCAGGRS;

            DescrEdgeStmt.Rule = S_DESCEDGE + Id_simple;

            DescrEdgesStmt.Rule = S_DESCEDGES;

            DescrTypeStmt.Rule = S_DESCTYPE + Id_simple;

            DescrTypesStmt.Rule = S_DESCTYPES;

            DescrFuncStmt.Rule = S_DESCFUNC + Id_simple;

            DescrFunctionsStmt.Rule = S_DESCFUNCTIONS;

            DescrSettStmt.Rule = S_DESCSETT + DescrSettItem | S_DESCSETTINGS + DescrSettingsItems;

            DescrSettItem.Rule = Id_simple + Empty | Id_simple + S_ON + S_TYPE + AType | Id_simple + S_ON + S_ATTRIBUTE + id_typeAndAttribute | Id_simple + S_ON + S_DB | Id_simple + S_ON + S_SESSION;

            DescrSettingsItems.Rule = S_ON + S_TYPE + TypeList | S_ON + S_ATTRIBUTE + id_typeAndAttribute | S_ON + S_DB | S_ON + S_SESSION;

            DescrSettingsStmt.Rule = S_DESCSETTINGS;

            DescrIdxStmt.Rule = S_DESCIDX + id_simpleDotList + DescrIdxEdtStmt;

            DescrIdxEdtStmt.Rule = Empty | Id_simple;

            DescrIdxsStmt.Rule = S_DESCIDXS;
            
            #endregion

            #region INSERTORUPDATE

            insertorupdateStmt.Rule = S_INSERTORUPDATE + TypeWrapper + S_VALUES + S_BRACKET_LEFT + AttrAssignList + S_BRACKET_RIGHT + whereClauseOpt;

            #endregion

            #region INSERTORREPLACE

            insertorreplaceStmt.Rule = S_INSERTORREPLACE + TypeWrapper + S_VALUES + S_BRACKET_LEFT + AttrAssignList + S_BRACKET_RIGHT + whereClauseOpt;

            #endregion

            #region REPLACE

            replaceStmt.Rule = S_REPLACE + TypeWrapper + S_VALUES + S_BRACKET_LEFT + AttrAssignList + S_BRACKET_RIGHT + S_WHERE + expression;

            #endregion

            #region DUMP

            var dumpType   = new NonTerminal("dumpType",   CreateDumpTypeNode);
            var dumpFormat = new NonTerminal("dumpFormat", CreateDumpFormatNode);

            dumpType.Rule   = S_DUMP_TYPE_ALL | S_DUMP_TYPE_GDDL | S_DUMP_TYPE_GDML | Empty;     // If empty => create both
            dumpFormat.Rule = S_AS + S_DUMP_FORMAT_GQL | S_AS + S_DUMP_FORMAT_CSV | Empty;      // if empty => create GQL
            //dumpFormat.Rule = S_AS + MakePlusRule(_S_DUMP_FORMAT_GQL, S_DUMP_FORMAT_CSV) | Empty;      // if empty => create GQL
            dumpStmt.Rule   = S_DUMP + dumpType + dumpFormat;

            #endregion

            #region TRANSACTION


            #region BeginTransAction

            transactStmt.Rule = S_TRANSACTBEGIN + TransactOptions + S_TRANSACT + TransactAttributes;

            TransactOptions.Rule = Empty |
                                S_TRANSACTDISTRIBUTED + S_TRANSACTLONGRUNNING |
                                S_TRANSACTDISTRIBUTED |
                                S_TRANSACTLONGRUNNING;

            TransactAttributes.Rule = Empty |
                                TransactIsolation |
                                TransactName | 
                                TransactTimestamp |
                                TransactIsolation + TransactName |
                                TransactIsolation + TransactTimestamp |
                                TransactName + TransactTimestamp |
                                TransactIsolation + TransactName + TransactTimestamp;

            TransactIsolation.Rule = S_TRANSACTISOLATION + "=" + string_literal;

            TransactName.Rule = S_TRANSACTNAME + "=" + string_literal;

            TransactTimestamp.Rule = S_TRANSACTTIMESTAMP + "=" + string_literal;

            #endregion

            #region CommitRollbackTransAction            
            
            commitRollBackTransactStmt.Rule = TransactCommitRollbackType + S_TRANSACT + TransactCommitRollbackOpt;

            TransactCommitRollbackType.Rule = S_TRANSACTCOMMIT | S_TRANSACTROLLBACK;
            
            TransactCommitRollbackOpt.Rule = Empty |
                                        TransactName |
                                        S_TRANSACTCOMROLLASYNC |
                                        TransactName + S_TRANSACTCOMROLLASYNC;

            #endregion            

            #endregion

            #region Rebuild Indices

            rebuildIndicesStmt.Rule = S_REBUILD + S_INDICES + rebuildIndicesTypes;

            rebuildIndicesTypes.Rule = Empty | TypeList;

            #endregion

            #region IMPORT

            paramComments.Rule      = S_COMMENTS + tuple | Empty;
            paramParallelTasks.Rule = S_PARALLELTASKS + "(" + number + ")" | Empty;
            verbosityTypes.Rule     = Symbol(VerbosityTypes.Silent.ToString()) | Symbol(VerbosityTypes.Errors.ToString()) | Symbol(VerbosityTypes.Full.ToString());
            verbosity.Rule          = S_VERBOSITY + verbosityTypes | Empty;

            BNF_ImportFormat        = new NonTerminal("importformat");
            //BNF_ImportFormat.Rule = Empty;

            importStmt.Rule = S_IMPORT + S_FROM + string_literal + S_FORMAT + BNF_ImportFormat + paramParallelTasks + paramComments + offsetOpt + limitOpt + verbosity;

            #endregion

            #endregion

            #region misc

            #region operators
            RegisterOperators(1, Associativity.Neutral, "AND", "OR");
            RegisterOperators(2, Associativity.Neutral, "=", "!=", ">", ">=", "<", "<=", "<>", "!<", "!>", "IN", "NOTIN", "INRANGE");
            RegisterOperators(3, "+", "-");
            RegisterOperators(4, "*", "/");
            RegisterOperators(5, Associativity.Right, "**");
            #endregion

            #region punctuation

            RegisterPunctuation(",", S_BRACKET_LEFT.Symbol, S_BRACKET_RIGHT.Symbol, "[", "]");
            //RegisterPunctuation(",", S_BRACKET_LEFT.Symbol, S_BRACKET_RIGHT.Symbol, S_TUPLE_BRACKET_LEFT.Symbol, S_TUPLE_BRACKET_RIGHT.Symbol);
            //RegisterPunctuation(",");
            //RegisterBracePair(_S_BRACKET_LEFT.Symbol, S_BRACKET_RIGHT.Symbol);
            //RegisterBracePair(_S_TUPLE_BRACKET_LEFT.Symbol, S_TUPLE_BRACKET_RIGHT.Symbol);
            //RegisterBracePair(_S_TUPLE_BRACKET_LEFT_EXCLUSIVE.Symbol, S_TUPLE_BRACKET_RIGHT.Symbol);
            //RegisterBracePair(_S_TUPLE_BRACKET_LEFT.Symbol, S_TUPLE_BRACKET_RIGHT_EXCLUSIVE.Symbol);
            //RegisterBracePair(_S_TUPLE_BRACKET_LEFT_EXCLUSIVE.Symbol, S_TUPLE_BRACKET_RIGHT_EXCLUSIVE.Symbol);

            #endregion

            base.MarkTransient(
                singlestmt, Id_simple, selList, selectionSource, aggregateName, expression, term, funArgs
                , unOp, binOp, aliasOpt, aliasOptName, orderByAttributeListMember
                , Value
                //, EdgeTypeParam
                , EdgeType_SortedMember, AttrUpdateOrAssign, ListAttrUpdate, SettingItemSetVal, DescrArgument,
                TypeWrapper //is used as a wrapper for AType
                , IdOrFunc //, IdOrFuncList
                , exprList, aggregateArg,
                ExtendedExpressionList,
                BNF_ImportFormat, verbosityTypes
                );

            #endregion
        
        }


        #endregion

        #endregion

        #region Node Delegates

        private void CreateUnExpressionNode(CompilerContext context, ParseTreeNode parseNode)
        {
            UnaryExpressionNode aUnExpressionNode = new UnaryExpressionNode();

            aUnExpressionNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)aUnExpressionNode;
        }

        private void CreateGraphDBTypeNode(CompilerContext context, ParseTreeNode parseNode)
        {
            GraphDBTypeNode aPandoraTypeNode = new GraphDBTypeNode();

            aPandoraTypeNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)aPandoraTypeNode;
        }
 
        private void CreateBulkTypeListMemberNode(CompilerContext context, ParseTreeNode parseNode)
        {
            BulkTypeListMemberNode aBulkTypeListMemberNode = new BulkTypeListMemberNode();

            aBulkTypeListMemberNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)aBulkTypeListMemberNode;
        }

        private void CreateBulkTypeNode(CompilerContext context, ParseTreeNode parseNode)
        {
            BulkTypeNode aBulkTypeNode = new BulkTypeNode();

            aBulkTypeNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)aBulkTypeNode;
        }

        private void CreateReplaceStatementNode(CompilerContext context, ParseTreeNode parseNode)
        {
            ReplaceNode aReplaceNode = new ReplaceNode();

            aReplaceNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)aReplaceNode;
        }

        private void CreateInsertOrReplaceStatementNode(CompilerContext context, ParseTreeNode parseNode)
        {
            InsertOrReplaceNode aInsertOrReplaceNode = new InsertOrReplaceNode();

            aInsertOrReplaceNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)aInsertOrReplaceNode;
        }

        private void CreateInsertOrUpdateStatementNode(CompilerContext context, ParseTreeNode parseNode)
        {
            InsertOrUpdateNode aInsertOrUpdateNode = new InsertOrUpdateNode();

            aInsertOrUpdateNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)aInsertOrUpdateNode;
        }        
        
        private void CreateCreateTypesStatementNode(CompilerContext context, ParseTreeNode parseNode)
        {

            CreateTypesNode aCreateTypesNode = new CreateTypesNode();

            aCreateTypesNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)aCreateTypesNode;
        }

        private void CreateAttributeDefinitionNode(CompilerContext context, ParseTreeNode parseNode)
        {
            AttributeDefinitionNode aCreateAttributeNode = new AttributeDefinitionNode();

            aCreateAttributeNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)aCreateAttributeNode;
        }

        private void CreateInsertStatementNode(CompilerContext context, ParseTreeNode parseNode)
        {

            InsertNode aInsertNode = new InsertNode();

            aInsertNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)aInsertNode;
        }

        private void CreateCreateIndexStatementNode(CompilerContext context, ParseTreeNode parseNode)
        {

            CreateIndexNode aIndexNode = new CreateIndexNode();

            aIndexNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)aIndexNode;

        }

        private void CreateIDNode(CompilerContext context, ParseTreeNode parseNode)
        {

            IDNode aIDNode = new IDNode();

            aIDNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)aIDNode;

        }

        private void CreateDotDelimiter(CompilerContext context, ParseTreeNode parseNode)
        {
            SelectionDelimiterNode aDelimitter = new SelectionDelimiterNode();

            aDelimitter.GetContent(context, parseNode, KindOfDelimiter.Dot);

            parseNode.AstNode = (object)aDelimitter;
        }

        private void CreateEdgeInformation(CompilerContext context, ParseTreeNode parseNode)
        {
            EdgeInformationNode aEdgeInformation = new EdgeInformationNode();

            aEdgeInformation.GetContent(context, parseNode);

            parseNode.AstNode = (object)aEdgeInformation;
        }

        private void CreateEdgeTraversal(CompilerContext context, ParseTreeNode parseNode)
        {
            EdgeTraversalNode aEdgeTraversal = new EdgeTraversalNode();

            aEdgeTraversal.GetContent(context, parseNode);

            parseNode.AstNode = (object)aEdgeTraversal;
        }

        private void CreateEdgeAccessorDelimiter(CompilerContext context, ParseTreeNode parseNode)
        {
            SelectionDelimiterNode aDelimitter = new SelectionDelimiterNode();

            aDelimitter.GetContent(context, parseNode, KindOfDelimiter.EdgeInformationDelimiter);

            parseNode.AstNode = (object)aDelimitter;
        }

        private void CreateUpdateStatementNode(CompilerContext context, ParseTreeNode parseNode)
        {

            UpdateNode aUpdateNode = new UpdateNode();

            aUpdateNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)aUpdateNode;

        }

        private void CreateBinaryExpressionNode(CompilerContext context, ParseTreeNode parseNode)
        {
            BinaryExpressionNode aNode = new BinaryExpressionNode();

            aNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)aNode;

        }


        private void CreateWhereExpressionNode(CompilerContext context, ParseTreeNode parseNode)
        {
            WhereExpressionNode aWhereNode = new WhereExpressionNode();

            aWhereNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)aWhereNode;
        }

        private void CreateSelectStatementNode(CompilerContext context, ParseTreeNode parseNode)
        {
            var aSelectStatementNode = new SelectNode();

            aSelectStatementNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)aSelectStatementNode;
        }

        private void CreateATypeNode(CompilerContext context, ParseTreeNode parseNode)
        {
            ATypeNode aATypeNode = new ATypeNode();

            aATypeNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)aATypeNode;

            if (context.PandoraListOfReferences == null)
                context.PandoraListOfReferences = new Dictionary<string, object>();

            if (aATypeNode.Reference != null && !context.PandoraListOfReferences.ContainsKey(aATypeNode.Reference))
            {
                context.PandoraListOfReferences.Add(aATypeNode.Reference, (object)aATypeNode);
            }
        }

        private void CreateAlterStmNode(CompilerContext context, ParseTreeNode parseNode)
        {
            AlterTypeNode aAlterTypeStatementNode = new AlterTypeNode();

            aAlterTypeStatementNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)aAlterTypeStatementNode;
        }

        private void CreatePartialSelectStmtNode(CompilerContext context, ParseTreeNode parseNode)
        {
            PartialSelectStmtNode aPartialSelectStmtNode = new PartialSelectStmtNode();

            aPartialSelectStmtNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)aPartialSelectStmtNode;
        }

        private void CreateAggregateNode(CompilerContext context, ParseTreeNode parseNode)
        {
            AggregateNode aAggregateNode = new AggregateNode();

            aAggregateNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)aAggregateNode;
        }

        private void CreateFunctionCallNode(CompilerContext context, ParseTreeNode parseNode)
        {
            FuncCallNode functionCallNode = new FuncCallNode();

            functionCallNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)functionCallNode;
        }

        private void CreateDropTypeStmNode(CompilerContext context, ParseTreeNode parseNode)
        {
            DropTypeNode dropTypeNode = new DropTypeNode();

            dropTypeNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)dropTypeNode;
        }

        private void CreateDropIndexStmNode(CompilerContext context, ParseTreeNode parseNode)
        {
            DropIndexNode dropIndexNode = new DropIndexNode();

            dropIndexNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)dropIndexNode;
        }

        private void CreateTruncateStmNode(CompilerContext context, ParseTreeNode parseNode)
        {
            TruncateNode truncateNode = new TruncateNode();

            truncateNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)truncateNode;
        }

        private void CreateDeleteStatementNode(CompilerContext context, ParseTreeNode parseNode)
        {
            DeleteNode deleteNode = new DeleteNode();

            deleteNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)deleteNode;
        }

        private void CreateSettingStatementNode(CompilerContext context, ParseTreeNode parseNode)
        {
            SettingNode settingNode = new SettingNode();

            settingNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)settingNode;
        }

        private void CreateSettingTypeNode(CompilerContext context, ParseTreeNode parseNode)
        {
            SettingTypeNode settingTypeNode = new SettingTypeNode();

            settingTypeNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)settingTypeNode;
        }

        private void CreateSettingAttrNode(CompilerContext context, ParseTreeNode parseNode)
        {
            SettingAttrNode settingAttrNode = new SettingAttrNode();

            settingAttrNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)settingAttrNode;
        }


        private void CreateBackwardEdgesNode(CompilerContext context, ParseTreeNode parseNode)
        {
            BackwardEdgesNode backwardEdgesNode = new BackwardEdgesNode();

            backwardEdgesNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)backwardEdgesNode;
        }

        private void CreateBackwardEdgeNode(CompilerContext context, ParseTreeNode parseNode)
        {
            BackwardEdgeNode backwardEdgeNode = new BackwardEdgeNode();

            backwardEdgeNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)backwardEdgeNode;
        }

        private void CreateEdgeTypeDefNode(CompilerContext context, ParseTreeNode parseNode)
        {
            EdgeTypeDefNode edgeTypeDefNode = new EdgeTypeDefNode();

            edgeTypeDefNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)edgeTypeDefNode;
        }

        private void CreateSingleEdgeTypeDefNode(CompilerContext context, ParseTreeNode parseNode)
        {
            SingleEdgeTypeDefNode edgeTypeDefNode = new SingleEdgeTypeDefNode();

            edgeTypeDefNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)edgeTypeDefNode;
        }

        private void CreateDumpNode(CompilerContext context, ParseTreeNode parseNode)
        {
            var dumpNode = new DumpNode();

            dumpNode.GetContent(context, parseNode);

            parseNode.AstNode = dumpNode;
        }

        private void CreateDumpTypeNode(CompilerContext context, ParseTreeNode parseNode)
        {
            var dumpTypeNode = new DumpTypeNode();

            dumpTypeNode.GetContent(context, parseNode);

            parseNode.AstNode = dumpTypeNode;
        }

        private void CreateDumpFormatNode(CompilerContext context, ParseTreeNode parseNode)
        {
            var dumpFormatNode = new DumpFormatNode();

            dumpFormatNode.GetContent(context, parseNode);

            parseNode.AstNode = dumpFormatNode;
        }

        private void CreateAddToListAttrUpdateAddToNode(CompilerContext context, ParseTreeNode parseNode)
        {
            var addToListAttrUpdateAddToNode = new AddToListAttrUpdateAddToNode();

            addToListAttrUpdateAddToNode.GetContent(context, parseNode);

            parseNode.AstNode = addToListAttrUpdateAddToNode;
        }

        private void CreateAddToListAttrUpdateOperatorNode(CompilerContext context, ParseTreeNode parseNode)
        {
            var addToListAttrUpdateOperatorNode = new AddToListAttrUpdateOperatorNode();

            addToListAttrUpdateOperatorNode.GetContent(context, parseNode);

            parseNode.AstNode = addToListAttrUpdateOperatorNode;
        }

        private void CreateRemoveFromListAttrUpdateAddToRemoveFromNode(CompilerContext context, ParseTreeNode parseNode)
        {
            var removeFromListAttrUpdateAddToRemoveFromNode = new RemoveFromListAttrUpdateAddToRemoveFromNode();

            removeFromListAttrUpdateAddToRemoveFromNode.GetContent(context, parseNode);

            parseNode.AstNode = removeFromListAttrUpdateAddToRemoveFromNode;
        }

        private void CreateRemoveFromListAttrUpdateAddToOperatorNode(CompilerContext context, ParseTreeNode parseNode)
        {
            var removeFromListAttrUpdateAddToOperatorNode = new RemoveFromListAttrUpdateAddToOperatorNode();

            removeFromListAttrUpdateAddToOperatorNode.GetContent(context, parseNode);

            parseNode.AstNode = removeFromListAttrUpdateAddToOperatorNode;
        }


        private void CreateAttrDefaultValueNode(CompilerContext context, ParseTreeNode parseNode)
        {
            var attrDefaultValueNode = new AttrDefaultValueNode();

            attrDefaultValueNode.GetContent(context, parseNode);

            parseNode.AstNode = attrDefaultValueNode;
        }

        #region RebuildIndices

        private void CreateRebuildIndicesNode(CompilerContext context, ParseTreeNode parseNode)
        {
            var rebuildIndicesNode = new RebuildIndicesNode();

            rebuildIndicesNode.GetContent(context, parseNode);

            parseNode.AstNode = rebuildIndicesNode;
        }

        #endregion

        #region Transaction

        private void CreateTransActionStatementNode(CompilerContext context, ParseTreeNode parseNode)
        {
            BeginTransactionNode beginTransactNode = new BeginTransactionNode();

            beginTransactNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)beginTransactNode;
        }

        private void CreateCommitRollbackTransActionNode(CompilerContext context, ParseTreeNode parseNode)
        {
            CommitRollbackTransactionNode commitRollBackNode = new CommitRollbackTransactionNode();

            commitRollBackNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)commitRollBackNode;
        }

        #endregion

        #region Describe

        private void CreateDescribeNode(CompilerContext context, ParseTreeNode parseNode)
        {
            DescribeNode DescrNode = new DescribeNode();

            DescrNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)DescrNode;
        }

        private void CreateDescrFunc(CompilerContext context, ParseTreeNode parseNode)
        {
            DescrFuncNode funcInfoNode = new DescrFuncNode();

            funcInfoNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)funcInfoNode;
        }

        private void CreateDescrFunctions(CompilerContext context, ParseTreeNode parseNode)
        {
            DescrFunctionsNode funcInfoNode = new DescrFunctionsNode();

            funcInfoNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)funcInfoNode;    
        }

        private void CreateDescrAggr(CompilerContext context, ParseTreeNode parseNode)
        {
            DescrAggrNode aggrInfoNode = new DescrAggrNode();

            aggrInfoNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)aggrInfoNode;
        }

        private void CreateDescrAggrs(CompilerContext context, ParseTreeNode parseNode)
        {
            DescrAggrsNode aggrInfoNode = new DescrAggrsNode();

            aggrInfoNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)aggrInfoNode;
        }

        private void CreateDescrSett(CompilerContext context, ParseTreeNode parseNode)
        {
            DescribeSettingNode settInfoNode = new DescribeSettingNode();

            settInfoNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)settInfoNode;
        }


        private void CreateDescrObj(CompilerContext context, ParseTreeNode parseNode)
        {
            DescribeObjectNode objInfoNode = new DescribeObjectNode();

            objInfoNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)objInfoNode;
        }

        private void CreateDescrType(CompilerContext context, ParseTreeNode parseNode)
        {
            DescribeTypeNode typeInfoNode = new DescribeTypeNode();

            typeInfoNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)typeInfoNode;
        }

        private void CreateDescrTypes(CompilerContext context, ParseTreeNode parseNode)
        {
            DescribeTypesNode typeInfoNode = new DescribeTypesNode();

            typeInfoNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)typeInfoNode;        
        }

        private void CreateDescrIdx(CompilerContext context, ParseTreeNode parseNode)
        {
            DescribeIndexNode idxInfoNode = new DescribeIndexNode();

            idxInfoNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)idxInfoNode;
        }

        private void CreateDescrIdxs(CompilerContext context, ParseTreeNode parseNode)
        {
            DescribeIndicesNode idxInfoNode = new DescribeIndicesNode();

            idxInfoNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)idxInfoNode;
        }

        private void CreateDescrEdge(CompilerContext context, ParseTreeNode parseNode)
        { 
            DescribeEdgeNode edgeInfoNode = new DescribeEdgeNode();

            edgeInfoNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)edgeInfoNode;
        }

        private void CreateDescrEdges(CompilerContext context, ParseTreeNode parseNode)
        {
            DescribeEdgesNode edgeInfoNode = new DescribeEdgesNode();

            edgeInfoNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)edgeInfoNode;
        }

        private void CreateDescrSettItem(CompilerContext context, ParseTreeNode parseNode)
        {
            DescribeSettItemNode setItemInfoNode = new DescribeSettItemNode();

            setItemInfoNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)setItemInfoNode;
        }

        private void CreateDescrSettingsItems(CompilerContext context, ParseTreeNode parseNode)
        {
            DescribeSettingsItemsNode setItemInfoNode = new DescribeSettingsItemsNode();

            setItemInfoNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)setItemInfoNode;    
        }

        private void CreateDescrSettings(CompilerContext context, ParseTreeNode parseNode)
        {
            DescribeSettingsNode setItemInfoNode = new DescribeSettingsNode();

            setItemInfoNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)setItemInfoNode;    
        }
        #endregion

        #region Import

        private void CreateImportNode(CompilerContext context, ParseTreeNode parseNode)
        {
            var importNode = new ImportNode();

            importNode.GetContent(context, parseNode);

            parseNode.AstNode = importNode;
        }

        private void CreateParallelTaskNode(CompilerContext context, ParseTreeNode parseNode)
        {
            var parallelTaskNode = new ParallelTasksNode();

            parallelTaskNode.GetContent(context, parseNode);

            parseNode.AstNode = parallelTaskNode;
        }

        private void CreateCommentsNode(CompilerContext context, ParseTreeNode parseNode)
        {
            var commentsNode = new CommentsNode();

            commentsNode.GetContent(context, parseNode);

            parseNode.AstNode = commentsNode;
        }

        private void CreateVerbosityNode(CompilerContext context, ParseTreeNode parseNode)
        {
            var verbosityNode = new VerbosityNode();

            verbosityNode.GetContent(context, parseNode);

            parseNode.AstNode = verbosityNode;
        }

        #endregion

        #endregion


        #region IDumpable Members

        #region Export GraphDDL

        public Exceptional<List<String>> ExportGraphDDL(DumpFormats myDumpFormat, DBContext myDBContext)
        {

            var _StringBuilder = new StringBuilder(String.Concat(S_CREATE.ToUpperString(), " ", S_TYPES.ToUpperString(), " "));
            var _List = new StringBuilder(String.Concat(S_CREATE.ToUpperString(), " ", S_TYPES.ToUpperString(), " "));
            var delimiter = ", ";

            foreach (var _GraphDBType in myDBContext.DBTypeManager.GetAllTypes(false))
                _StringBuilder.Append(String.Concat(CreateGraphDDL(myDumpFormat, _GraphDBType, myDBContext), delimiter));

            var _String = _StringBuilder.ToString();

            if (_String.EndsWith(delimiter))
                _String = _String.Substring(0, _String.Length - delimiter.Length);

            return new Exceptional<List<String>>(new List<String>() { _String });

        }

        private String CreateGraphDDL(DumpFormats myDumpFormat, GraphDBType myGraphDBType, DBContext myDBContext)
        {

            var _StringBuilder = new StringBuilder();
            _StringBuilder.AppendFormat("{0} ", myGraphDBType.Name);

            if (myGraphDBType.ParentTypeUUID != null)
            {

                _StringBuilder.AppendFormat("{0} {1} ", S_EXTENDS.ToUpperString(), myGraphDBType.GetParentType(myDBContext.DBTypeManager).Name);//builder.AppendLine();

                #region Not backwardEdge attributes

                if (myGraphDBType.GetSpecificAttributes(ta => !ta.IsBackwardEdge).CountIsGreater(0))
                {
                    _StringBuilder.Append(S_ATTRIBUTES.ToUpperString() + S_BRACKET_LEFT.ToUpperString() + CreateGraphDDLOfAttributes(myDumpFormat, myGraphDBType.GetSpecificAttributes(ta => !ta.IsBackwardEdge), myDBContext) + S_BRACKET_RIGHT.ToUpperString() + " ");
                }

                #endregion

                #region BackwardEdge attributes

                if (myGraphDBType.GetSpecificAttributes(ta => ta.IsBackwardEdge).CountIsGreater(0))
                {
                    _StringBuilder.Append(S_BACKWARDEDGES.ToUpperString() + S_BRACKET_LEFT.ToUpperString() + CreateGraphDDLOfBackwardEdges(myDumpFormat, myGraphDBType.GetSpecificAttributes(ta => ta.IsBackwardEdge), myDBContext) + S_BRACKET_RIGHT.ToUpperString() + " ");
                }

                #endregion

                #region Uniques

                if (myGraphDBType.GetUniqueAttributes().CountIsGreater(0))
                {
                    _StringBuilder.Append(S_UNIQUE.ToUpperString() + S_BRACKET_LEFT.Symbol + CreateGraphDDLOfAttributeUUIDs(myDumpFormat, myGraphDBType.GetUniqueAttributes(), myGraphDBType) + S_BRACKET_RIGHT.Symbol + " ");
                }

                #endregion

                #region Mandatory attributes

                if (myGraphDBType.GetMandatoryAttributes().CountIsGreater(0))
                {
                    _StringBuilder.Append(S_MANDATORY.ToUpperString() + S_BRACKET_LEFT.Symbol + CreateGraphDDLOfAttributeUUIDs(myDumpFormat, myGraphDBType.GetMandatoryAttributes(), myGraphDBType) + S_BRACKET_RIGHT.Symbol + " ");
                }

                #endregion

                #region Indices

                if (myGraphDBType.GetAllAttributeIndices(false).CountIsGreater(0))
                {
                    _StringBuilder.Append(S_INDICES.ToUpperString() + S_BRACKET_LEFT.Symbol + CreateGraphDDLOfIndices(myDumpFormat, myGraphDBType.GetAllAttributeIndices(false), myGraphDBType) + S_BRACKET_RIGHT.Symbol + " ");
                }

                #endregion

            }

            return _StringBuilder.ToString();

        }

        private String CreateGraphDDLOfAttributes(DumpFormats myDumpFormat, IEnumerable<TypeAttribute> myTypeAttributes, DBContext myDBContext)
        {

            var _StringBuilder = new StringBuilder();
            var delimiter = ", ";

            foreach (var _Attribute in myTypeAttributes)
            {
                _StringBuilder.Append(CreateGraphDDLOfAttributeDefinition(myDumpFormat, _Attribute, myDBContext));
                _StringBuilder.Append(delimiter);
            }

            if (_StringBuilder.Length > delimiter.Length)
                _StringBuilder.Remove(_StringBuilder.Length - delimiter.Length, 2);

            return _StringBuilder.ToString();

        }

        private String CreateGraphDDLOfAttributeDefinition(DumpFormats myDumpFormat, TypeAttribute myTypeAttribute, DBContext myDBContext)
        {

            if (myTypeAttribute.EdgeType != null)
                return String.Concat(myTypeAttribute.EdgeType.GetGDDL(myTypeAttribute.GetDBType(myDBContext.DBTypeManager)), " ", myTypeAttribute.Name);

            else
                return String.Concat(myTypeAttribute.GetDBType(myDBContext.DBTypeManager).Name, " ", myTypeAttribute.Name);

        }

        private String CreateGraphDDLOfBackwardEdges(DumpFormats myDumpFormat, IEnumerable<TypeAttribute> myTypeAttributes, DBContext myDBContext)
        {

            var _StringBuilder = new StringBuilder();
            var delimiter = ", ";

            foreach (var _Attribute in myTypeAttributes)
            {
                var typeAttrInfos = _Attribute.BackwardEdgeDefinition.GetTypeAndAttributeInformation(myDBContext);
                _StringBuilder.Append(String.Concat(typeAttrInfos.Item1.Name, ".", typeAttrInfos.Item2.Name, " ", _Attribute.Name));
                _StringBuilder.Append(delimiter);
            }

            if (_StringBuilder.Length > delimiter.Length)
                _StringBuilder.Remove(_StringBuilder.Length - delimiter.Length, 2);

            return _StringBuilder.ToString();

        }

        /// <summary>
        /// Add just the Attribute names
        /// </summary>
        /// <param name="myDumpFormat"></param>
        /// <param name="typeAttribute"></param>
        /// <param name="indent"></param>
        /// <param name="indentWidth"></param>
        /// <returns></returns>
        private String CreateGraphDDLOfAttributeUUIDs(DumpFormats myDumpFormat, IEnumerable<AttributeUUID> myAttributes, GraphDBType myGraphDBType)
        {

            var _StringBuilder = new StringBuilder();
            var delimiter = ", ";

            foreach (var _Attribute in myAttributes)
            {
                _StringBuilder.Append(myGraphDBType.GetTypeAttributeByUUID(_Attribute).Name);
                _StringBuilder.Append(delimiter);
            }

            if (_StringBuilder.Length > delimiter.Length)
                _StringBuilder.Remove(_StringBuilder.Length - delimiter.Length, 2);

            return _StringBuilder.ToString();

        }

        /// <summary>
        /// Create the DDL for attributeIndices
        /// </summary>
        /// <param name="myDumpFormat"></param>
        /// <param name="myAttributeIndices"></param>
        /// <param name="indent"></param>
        /// <param name="indentWidth"></param>
        /// <returns></returns>
        private String CreateGraphDDLOfIndices(DumpFormats myDumpFormat, IEnumerable<AttributeIndex> myAttributeIndices, GraphDBType myGraphDBType)
        {

            var _StringBuilder = new StringBuilder();
            var delimiter = ", ";

            foreach (var _AttributeIndex in myAttributeIndices)
            {

                if (_AttributeIndex.IsUuidIndex || _AttributeIndex.IndexEdition == DBConstants.UNIQUEATTRIBUTESINDEX)
                    continue;

                _StringBuilder.Append(String.Concat(S_BRACKET_LEFT, _AttributeIndex.IndexName));

                if (_AttributeIndex.IsUniqueIndex)
                    _StringBuilder.Append(String.Concat(" ", S_UNIQUE.ToUpperString()));

                _StringBuilder.Append(String.Concat(" ", S_EDITION.ToUpperString(), " ", _AttributeIndex.IndexEdition));

                _StringBuilder.Append(String.Concat(" ", S_INDEXTYPE.ToUpperString(), " ", _AttributeIndex.IndexType.ToString()));
                _StringBuilder.Append(String.Concat(" ", S_ON.ToUpperString(), " ", CreateGraphDDLOfAttributeUUIDs(myDumpFormat, _AttributeIndex.IndexKeyDefinition.IndexKeyAttributeUUIDs, myGraphDBType)));

                _StringBuilder.Append(S_BRACKET_RIGHT);

                _StringBuilder.Append(delimiter);

            }

            if (_StringBuilder.Length > delimiter.Length)
                _StringBuilder.Remove(_StringBuilder.Length - delimiter.Length, 2);

            return _StringBuilder.ToString();

        }

        #endregion

        #region Export GraphDML

        /// <summary>
        /// Create the GraphDML of all DBObjects in the database.
        /// </summary>
        /// <param name="myDumpFormat"></param>
        /// <param name="myDBContext"></param>
        /// <param name="objectManager"></param>
        /// <returns></returns>
        public Exceptional<List<String>> ExportGraphDML(DumpFormats myDumpFormat, DBContext myDBContext)
        {

            //var _StringBuilder  = new StringBuilder();
            var _List           = new List<String>();
            var _Exceptional    = new Exceptional<List<String>>();
            var _DBObject_Edges = new List<String>();

            #region Go through each type

            foreach (var _GraphDBType in myDBContext.DBTypeManager.GetAllTypes(false))
            {

                var _IndexReference = _GraphDBType.GetUUIDIndex(myDBContext.DBTypeManager).GetIndexReference(myDBContext.DBIndexManager);

                if (!_IndexReference.Success)
                    return new Exceptional<List<String>>(_IndexReference);

                #region Take UUID index

                foreach (var _IndexEntry in _IndexReference.Value)
                {

                    #region Load DBObject and create GraphDML

                    foreach (var _DBObject in _IndexEntry.Value)
                    {

                        var _DBObjectStream = myDBContext.DBObjectManager.LoadDBObject(_GraphDBType, _DBObject);

                        if (!_DBObjectStream.Success)
                            _Exceptional.AddErrorsAndWarnings(_DBObjectStream);

                        else
                        {

                            var _GDMLExceptional = CreateGraphDMLforDBObject(myDumpFormat, myDBContext, _GraphDBType, _DBObjectStream.Value, _DBObject_Edges);

                            if (!_GDMLExceptional.Success)
                                _Exceptional.AddErrorsAndWarnings(_DBObjectStream);

                            else
                                //_StringBuilder.AppendLine(_GDMLExceptional.Value);
                                _List.Add(_GDMLExceptional.Value);

                        }

                    }

                    #endregion

                }

                #endregion

            }

            #endregion

            #region Append all edges as UPDATE GQL

            // after dumping all objects we will add the edges
            if (_DBObject_Edges.Count > 0)
                //_StringBuilder.AppendLine(_DBObject_Edges.ToString());
                _List.AddRange(_DBObject_Edges);

            #endregion

            //_Exceptional.Value = _StringBuilder.ToString();
            _Exceptional.Value = _List;

            return _Exceptional;

        }

        private Exceptional<String> CreateGraphDMLforDBObject(DumpFormats myDumpFormat, DBContext myDBContext, GraphDBType myGraphDBType, DBObjectStream myDBObjectStream, List<String> myEdges)
        {

            var _StringBuilder = new StringBuilder();
            var delimiter = ", ";

            _StringBuilder.Append(String.Concat(S_INSERT.ToUpperString(), " ", S_INTO.ToUpperString(), " ", myGraphDBType.Name, " ", S_VALUES.ToUpperString(), " ", S_BRACKET_LEFT));
            _StringBuilder.Append(String.Concat(S_UUID.ToUpperString(), " = '", myDBObjectStream.ObjectUUID.ToString(), "'", delimiter));

            #region CreateGraphDMLforDBODefinedAttributes

            var edges = new StringBuilder();

            var defAttrsDML = CreateGraphDMLforDBObjectDefinedAttributes(myDumpFormat, myDBObjectStream.GetAttributes(), myGraphDBType, myDBObjectStream, edges, myDBContext);

            if (!defAttrsDML.Success)
                return defAttrsDML;

            _StringBuilder.Append(defAttrsDML.Value);

            #region For edges create UPDATE command

            if (edges.Length > 0)
            {
                edges.RemoveEnding(delimiter);
                myEdges.Add(String.Concat(S_UPDATE.ToUpperString(), " ", myGraphDBType.Name, " ", S_SET.ToUpperString(), " ", S_BRACKET_LEFT, edges.ToString(), S_BRACKET_RIGHT, " ", S_WHERE.ToUpperString(), " ", S_UUID.ToUpperString(), " = '", myDBObjectStream.ObjectUUID.ToString(), "'"));
            }

            #endregion

            #endregion

            #region CreateGDMLforDBOUnDefinedAttributes

            var undefAttrs = myDBObjectStream.GetUndefinedAttributes(myDBContext.DBObjectManager);

            if (!undefAttrs.Success)
                return new Exceptional<String>(undefAttrs);

            if (undefAttrs.Value.Count > 0)
            {

                Exceptional<String> undefAttrsDML = CreateGraphDMLforDBObjectUndefinedAttributes(myDumpFormat, undefAttrs.Value, myGraphDBType, myDBObjectStream);
                
                if (!undefAttrsDML.Success)
                    return undefAttrsDML;

                _StringBuilder.Append(undefAttrsDML.Value);

            }

            #endregion

            _StringBuilder.RemoveEnding(delimiter);
            _StringBuilder.Append(S_BRACKET_RIGHT);

            return new Exceptional<String>(_StringBuilder.ToString());

        }

        private Exceptional<String> CreateGraphDMLforDBObjectDefinedAttributes(DumpFormats myDumpFormat, IDictionary<AttributeUUID, AObject> myAttributes, GraphDBType myGraphDBType, DBObjectStream myDBObjectStream, StringBuilder myEdgeBuilder, DBContext myDBContext)
        {

            var _StringBuilder = new StringBuilder();
            var delimiter = ", ";

            foreach (var _Attribute in myAttributes)
            {

                if (_Attribute.Value == null)
                    continue;

                var typeAttribute = myGraphDBType.GetTypeAttributeByUUID(_Attribute.Key);

                #region Reference attributes

                if (typeAttribute.GetDBType(myDBContext.DBTypeManager).IsUserDefined)
                {

                    #region SetOfReferences
                    if (_Attribute.Value is ASetReferenceEdgeType)
                    {

                        #region Create edge GDML

                        myEdgeBuilder.Append(String.Concat(typeAttribute.Name, " = ", S_SETOF.ToUpperString(), " ", S_BRACKET_LEFT));

                        #region Create an assignment content - if edge does not contain any elements create an empty one

                        if ((_Attribute.Value as ASetReferenceEdgeType).GetEdges().CountIsGreater(0))
                        {

                            #region Create attribute assignments

                            foreach (var val in (_Attribute.Value as ASetReferenceEdgeType).GetEdges())
                            {
                                myEdgeBuilder.Append(String.Concat(S_UUID.ToUpperString(), " = '", val.Item1.ToString(), "'"));
                                if (val.Item2 != null)
                                {
                                    myEdgeBuilder.Append(String.Concat(S_colon, S_BRACKET_LEFT, CreateGraphDMLforADBBaseObject(myDumpFormat, val.Item2), S_BRACKET_RIGHT));
                                }
                                myEdgeBuilder.Append(delimiter);
                            }
                            myEdgeBuilder.RemoveEnding(delimiter);

                            #endregion

                        }

                        #endregion

                        myEdgeBuilder.Append(S_BRACKET_RIGHT);

                        #endregion

                    }

                    #endregion

                    #region SingleReference

                    else if (typeAttribute.KindOfType == KindsOfType.SingleReference)
                    {
                        myEdgeBuilder.Append(String.Concat(typeAttribute.Name, " = ", S_REFERENCE.ToUpperString(), " ", S_BRACKET_LEFT));
                        myEdgeBuilder.Append(String.Concat(S_UUID.ToUpperString(), " = '", (_Attribute.Value as ASingleReferenceEdgeType).GetUUID().ToString(), "'"));
                        myEdgeBuilder.Append(S_BRACKET_RIGHT);
                    }

                    #endregion

                    else
                    {
                        return new Exceptional<String>(new Errors.Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                    }

                    myEdgeBuilder.Append(delimiter);

                }

                #endregion

                #region NonReference attributes

                else
                {

                    #region ListOfNoneReferences

                    if (typeAttribute.KindOfType == KindsOfType.ListOfNoneReferences)
                    {
                        _StringBuilder.Append(String.Concat(typeAttribute.Name, " = ", S_LISTOF.ToUpperString(), " ", S_BRACKET_LEFT));
                        foreach (var val in (_Attribute.Value as AListBaseEdgeType))
                        {
                            _StringBuilder.Append(CreateGraphDMLforADBBaseObject(myDumpFormat, val as ADBBaseObject) + delimiter);
                        }
                        _StringBuilder.RemoveEnding(delimiter);
                        _StringBuilder.Append(S_BRACKET_RIGHT);
                    }

                    #endregion

                    #region SetOfNoneReferences

                    else if (typeAttribute.KindOfType == KindsOfType.SetOfNoneReferences)
                    {
                        _StringBuilder.Append(String.Concat(typeAttribute.Name, " = ", S_SETOF.ToUpperString(), " ", S_BRACKET_LEFT));
                        foreach (var val in (_Attribute.Value as AListBaseEdgeType))
                        {
                            _StringBuilder.Append(CreateGraphDMLforADBBaseObject(myDumpFormat, val as ADBBaseObject) + delimiter);
                        }
                        _StringBuilder.RemoveEnding(delimiter);
                        _StringBuilder.Append(S_BRACKET_RIGHT);

                    }

                    #endregion

                    #region SpecialAttribute

                    else if (typeAttribute.KindOfType == KindsOfType.SpecialAttribute)
                    {
                        throw new GraphDBException(new Errors.Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                    }

                    #endregion

                    #region Single value

                    else
                    {
                        _StringBuilder.Append(String.Concat(typeAttribute.Name, " = ", CreateGraphDMLforADBBaseObject(myDumpFormat, _Attribute.Value as ADBBaseObject)));
                    }

                    #endregion

                    _StringBuilder.Append(delimiter);

                }

                #endregion

            }

            return new Exceptional<String>(_StringBuilder.ToString());

        }

        private Exceptional<String> CreateGraphDMLforDBObjectUndefinedAttributes(DumpFormats myDumpFormat, IDictionary<String, AObject> myAttributes, GraphDBType myGraphDBType, DBObjectStream myDBObjectStream)
        {

            var _StringBuilder = new StringBuilder();
            var delimiter = ", ";

            foreach (var _Attribute in myAttributes)
            {

                #region A single value...

                if (_Attribute.Value is ADBBaseObject)
                    _StringBuilder.Append(String.Concat(_Attribute.Key, " = ", CreateGraphDMLforADBBaseObject(myDumpFormat, _Attribute.Value as ADBBaseObject)));

                #endregion

                #region ..or, it is a List or Set, since the Set constraint was already verified we can use a list

                else if (_Attribute.Value is AListBaseEdgeType)
                {

                    _StringBuilder.Append(String.Concat(_Attribute.Key, " = ", S_LISTOF.ToUpperString(), " ", S_BRACKET_LEFT));

                    foreach (var val in (_Attribute.Value as AListBaseEdgeType))
                        _StringBuilder.Append(CreateGraphDMLforADBBaseObject(myDumpFormat, val as ADBBaseObject) + delimiter);

                    _StringBuilder.RemoveEnding(delimiter);
                    _StringBuilder.Append(S_BRACKET_RIGHT);

                }

                #endregion

                else
                    return new Exceptional<String>(new Errors.Error_NotImplemented(new StackTrace(true)));

                _StringBuilder.Append(delimiter);

            }

            return new Exceptional<String>(_StringBuilder.ToString());

        }

        private String CreateGraphDMLforADBBaseObject(DumpFormats myDumpFormat, ADBBaseObject myADBBaseObject)
        {

            var _DBNumber = myADBBaseObject as DBNumber;

            if (_DBNumber != null)
                return _DBNumber.ToString(new CultureInfo("en-US"));

            return String.Concat("'", myADBBaseObject.ToString(new CultureInfo("en-US")), "'");

        }

        #endregion

        #endregion   

    }

}
